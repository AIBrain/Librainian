// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CreateUnsafeSerializer.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Database.MMF {

    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.CSharp;

    /// <summary>
    /// Class which tries to create a ISerializeDeserialize based on pointer movement (unsafe).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreateUnsafeSerializer<T> {

        private readonly Type _type = typeof( T );

        private Int32 _addCount;

        private Int32 _ptrSize = 8;

        private String _ptrType = "Int64";

        private Int32 _size;

        private static void BytesToObjectCode( StringBuilder sb, String typeFullName ) {
            sb.Append( $"public unsafe {typeFullName} BytesToObject( byte[] bytes )" );
            sb.Append( "{" );

            sb.Append( @"
                fixed (byte* srcPtr = &bytes[0])
                {" );

            sb.Append( $"return *({typeFullName}*)srcPtr;" );
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
            var providerOptions = new Dictionary<String, String> { { "CompilerVersion", "v3.5" } };
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

            sb.Append( $"public class UnsafeConverter : {interfaceType.Namespace}.ISerializeDeserialize<{typeFullName}>" );
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
            var cParameters = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false, TreatWarningsAsErrors = false, IncludeDebugInformation = false, CompilerOptions = "/optimize /unsafe" };
            cParameters.ReferencedAssemblies.Add( Assembly.GetExecutingAssembly().Location );
            cParameters.ReferencedAssemblies.Add( this._type.Assembly.Location );

            return cParameters;
        }

        private void MovePointers( StringBuilder sb ) {
            var modifer = this._addCount / this._ptrSize;

            if ( modifer >= this._ptrSize ) {
                sb.Append( $"dest += {this._addCount};" );
                sb.Append( $"src += {this._addCount};" );
                this._addCount = 0;
            }
        }

        private void ObjectToBytesCode( StringBuilder sb, String typeFullName ) {
            sb.Append( $"public unsafe byte[] ObjectToBytes({typeFullName} srcObject)" );
            sb.Append( "{" );
            sb.Append( $"byte[] buffer = new byte[{this._size}];" );

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

        private void SetPointerLength( Int32 length ) {
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
                throw new SerializerException( res.Errors[0].ErrorText );
            }

            return ( ISerializeDeserialize<T> )res.CompiledAssembly.CreateInstance( "UnsafeConverter" );
        }
    }
}