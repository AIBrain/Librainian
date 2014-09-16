#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Factory.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Database.MMF {
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.CSharp;

    /// <summary>
    ///     Class which tries to create a ISerializeDeserialize based on pointer movement (unsafe).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreateUnsafeSerializer<T> {
        private readonly Type _type = typeof( T );

        private int _addCount;

        private int _ptrSize = 8;

        private String _ptrType = "Int64";

        private int _size;

        public ISerializeDeserialize<T> GetSerializer() {
            if ( !this.CanGetSize() ) {
                return null;
            }
            var checker = new ValueTypeCheck( typeof( T ) );
            if ( !checker.OnlyValueTypes() ) {
                return null;
            }
            var res = this.CompileCode();
            if ( res.Errors.Count > 0 ) {
                throw new SerializerException( res.Errors[ 0 ].ErrorText );
            }
            return ( ISerializeDeserialize<T> )res.CompiledAssembly.CreateInstance( "UnsafeConverter" );
        }

        private static void BytesToObjectCode( StringBuilder sb, String typeFullName ) {
            sb.AppendFormat( "public unsafe {0} BytesToObject( byte[] bytes )", typeFullName );
            sb.Append( "{" );
            sb.Append( @"
                fixed (byte* srcPtr = &bytes[0])
                {" );
            sb.AppendFormat( "return *({0}*)srcPtr;", typeFullName );
            sb.Append( "}}" );
        }

        private Boolean CanGetSize() {
            try {
                this._size = Marshal.SizeOf<T>();
            }
            catch ( ArgumentException ) {
                return false;
            }
            return true;
        }

        private CompilerResults CompileCode() {
            var providerOptions = new Dictionary<String, String> {
                                                                       { "CompilerVersion", "v3.5" }
                                                                   };
            CodeDomProvider provider = new CSharpCodeProvider( providerOptions );
            var compilerParameters = this.GetCompilerParameters();
            return provider.CompileAssemblyFromSource( compilerParameters, this.GenerateCode() );
        }

        private String GenerateCode() {
            var typeFullName = this._type.FullName.Replace( '+', '.' );

            var sb = new StringBuilder();
            sb.AppendLine( "using System;" );
            sb.AppendLine();

            var interfaceType = typeof( ISerializeDeserialize<T> );

            sb.AppendFormat( "public class UnsafeConverter : {0}.ISerializeDeserialize<{1}>", interfaceType.Namespace, typeFullName );
            sb.Append( "{" );
            sb.AppendFormat( "public Boolean CanSerializeType(){{return true;}}" );

            this.ObjectToBytesCode( sb, typeFullName );
            BytesToObjectCode( sb, typeFullName );

            sb.Append( "}" );
            return sb.ToString();
        }

        private void GenerateMethodBodyCode( StringBuilder sb ) {
            this._addCount = 0;
            var length = this._size;
            do {
                this.MovePointers( sb );
                this.SetPointerLength( length );
                sb.AppendFormat( @"*(({0}*)dest+{1}) = *(({0}*)src+{1});", this._ptrType, this._addCount / this._ptrSize );
                length -= this._ptrSize;
                this._addCount += this._ptrSize;
            } while ( length > 0 );
        }

        private CompilerParameters GetCompilerParameters() {
            var cParameters = new CompilerParameters {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/optimize /unsafe"
            };
            cParameters.ReferencedAssemblies.Add( Assembly.GetExecutingAssembly().Location );
            cParameters.ReferencedAssemblies.Add( this._type.Assembly.Location );
            return cParameters;
        }

        private void MovePointers( StringBuilder sb ) {
            var modifer = this._addCount / this._ptrSize;
            if ( modifer >= this._ptrSize ) {
                sb.AppendFormat( "dest += {0};", this._addCount );
                sb.AppendFormat( "src += {0};", this._addCount );
                this._addCount = 0;
            }
        }

        private void ObjectToBytesCode( StringBuilder sb, String typeFullName ) {
            sb.AppendFormat( "public unsafe byte[] ObjectToBytes({0} srcObject)", typeFullName );
            sb.Append( "{" );
            sb.AppendFormat( "byte[] buffer = new byte[{0}];", this._size );
            sb.Append( @"
                fixed (byte* destPtr = &buffer[0])
                {
                    " );
            sb.Append( "byte* src = (byte*)&srcObject;" );
            sb.Append( "byte* dest = destPtr;" );

            this.GenerateMethodBodyCode( sb );

            sb.Append( @"}
                return buffer;}" );
        }

        private void SetPointerLength( int length ) {
            if ( length >= 8 ) {
                this._ptrSize = 8;
                this._ptrType = "Int64";
            }
            else if ( length >= 4 ) {
                this._ptrSize = 4;
                this._ptrType = "Int32";
            }
            else if ( length >= 2 ) {
                this._ptrSize = 2;
                this._ptrType = "Int16";
            }
            else {
                this._ptrSize = 1;
                this._ptrType = "byte";
            }
        }
    }

    public class Factory<T> {
        private static readonly HashSet<Type> _compiledUnsafeSerializer = new HashSet<Type>();

        private static readonly Dictionary<Type, ISerializeDeserialize<T>> DictionaryCache = new Dictionary<Type, ISerializeDeserialize<T>>();

        public ISerializeDeserialize<T> GetSerializer() {
            ISerializeDeserialize<T> result;
            var objectType = typeof( T );
            if ( !DictionaryCache.TryGetValue( objectType, out result ) ) {
                DictionaryCache[ objectType ] = result = PickOptimalSerializer();
            }
            Debug.WriteLine( "{0} uses {1}", typeof( T ), result.GetType() );
            return result;
        }

        public ISerializeDeserialize<T> GetSerializer( String name ) {
            return ( from pair in DictionaryCache
                     where pair.Value.GetType().AssemblyQualifiedName == name
                     select pair.Value ).FirstOrDefault();
        }

        public List<ISerializeDeserialize<T>> GetValidSerializers() {
            CompileAndRegisterUnsafeSerializer();

            var listOfSerializers = GetListOfGenericSerializers().ToList();
            listOfSerializers.AddRange( GetListOfImplementedSerializers() );

            var benchmarkTimes = BenchmarkSerializers( listOfSerializers );
            if ( benchmarkTimes.Count == 0 ) {
                throw new SerializerException( "No serializer available for the type" );
            }

            return benchmarkTimes.Values.ToList();
        }

        private static int BenchMarkSerializer( ISerializeDeserialize<T> serDeser ) {
            object[] args = null;
            if ( typeof( T ) == typeof( String ) ) {
                args = new object[] { new[] { 'T', 'e', 's', 't', 'T', 'e', 's', 't', 'T', 'e', 's', 't' } };
            }
            try {
                var classInstance = ( T )Activator.CreateInstance( typeof( T ), args );
                var sw = Stopwatch.StartNew();
                var count = 0;
                while ( sw.ElapsedMilliseconds < 500 ) {
                    var bytes = serDeser.ObjectToBytes( classInstance );
                    serDeser.BytesToObject( bytes );
                    count++;
                }
                sw.Stop();
                return count;
            }
            catch ( MissingMethodException ) {
                // Missing default constructor
                return 0;
            }
        }

        private static SortedDictionary<int, ISerializeDeserialize<T>> BenchmarkSerializers( IEnumerable<Type> listOfSerializers ) {
            var benchmarkTimes = new SortedDictionary<int, ISerializeDeserialize<T>>();
            foreach ( var type in listOfSerializers ) {
                var serializer = InstantiateSerializer( type );
                if ( !serializer.CanSerializeType() ) {
                    continue;
                }
                var count = BenchMarkSerializer( serializer );
                if ( count > 0 ) {
                    benchmarkTimes.Add( count, serializer );
                }
            }

            foreach ( var valuePair in benchmarkTimes ) {
                Debug.WriteLine( "{0} : {1}", valuePair.Key, valuePair.Value.GetType() );
            }

            return benchmarkTimes;
        }

        private static void CompileAndRegisterUnsafeSerializer() {
            try {
                if ( _compiledUnsafeSerializer.Contains( typeof( T ) ) ) {
                    return;
                }
                var createUnsafeSerializer = new CreateUnsafeSerializer<T>();
                createUnsafeSerializer.GetSerializer();
                _compiledUnsafeSerializer.Add( typeof( T ) );
            }
            catch ( SerializerException ) {
                // ignore errors
            }
        }

        private static IEnumerable<Type> GetListOfGenericSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );
            var serializers = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              from genericType in assembly.GetTypes()
                              from interfaceType in genericType.GetInterfaces().Where( iType => ( iType.Name == interfaceGenricType.Name && genericType.IsGenericTypeDefinition ) )
                              select genericType;
            return serializers; //.ToList();
        }

        private static IEnumerable<Type> GetListOfImplementedSerializers() {
            var interfaceGenricType = typeof( ISerializeDeserialize<T> );
            var serializers = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              from implementedType in assembly.GetTypes()
                              from interfaceType in implementedType.GetInterfaces().Where( iType => iType == interfaceGenricType )
                              select implementedType;
            return serializers; //.ToList();
        }

        private static ISerializeDeserialize<T> InstantiateSerializer( Type type ) {
            var instType = type.IsGenericTypeDefinition ? type.MakeGenericType( typeof( T ) ) : type;
            return ( ISerializeDeserialize<T> )Activator.CreateInstance( instType );
        }

        private static ISerializeDeserialize<T> PickOptimalSerializer() {
            CompileAndRegisterUnsafeSerializer();

            var listOfSerializers = GetListOfGenericSerializers().ToList();
            listOfSerializers.AddRange( GetListOfImplementedSerializers() );

            var benchmarkTimes = BenchmarkSerializers( listOfSerializers );
            if ( benchmarkTimes.Count == 0 ) {
                throw new SerializerException( "No serializer available for the type" );
            }

            return benchmarkTimes.Last().Value;
        }
    }
}
