// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "CreateUnsafeSerializer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "CreateUnsafeSerializer.cs" was last formatted by Protiguous on 2020/03/18 at 10:22 AM.

namespace Librainian.Databases.MMF {

    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;
    using Microsoft.CSharp;

    /// <summary>Class which tries to create a ISerializeDeserialize based on pointer movement (unsafe).</summary>
    /// <typeparam name="T"></typeparam>
    public class CreateUnsafeSerializer<T> {

        private Int32 _addCount;

        private Int32 _ptrSize = sizeof( Int64 );

        private String _ptrType = "Int64";

        private Int32 _size;

        private Type Type { get; } = typeof( T );

        private static void BytesToObjectCode( [NotNull] StringBuilder sb, [CanBeNull] String? typeFullName ) {
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
                return default;
            }

            return true;
        }

        [NotNull]
        private CompilerResults CompileCode() {
            var providerOptions = new Dictionary<String, String> {
                {
                    "CompilerVersion", "v3.5"
                }
            };

            CodeDomProvider provider = new CSharpCodeProvider( providerOptions );
            var compilerParameters = this.GetCompilerParameters();

            return provider.CompileAssemblyFromSource( compilerParameters, this.GenerateCode() );
        }

        [NotNull]
        private String GenerateCode() {
            var typeFullName = this.Type.FullName.Replace( '+', '.' );

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

        private void GenerateMethodBodyCode( [NotNull] StringBuilder sb ) {
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

        [NotNull]
        private CompilerParameters GetCompilerParameters() {
            var cParameters = new CompilerParameters {
                GenerateInMemory = true, GenerateExecutable = false, TreatWarningsAsErrors = false, IncludeDebugInformation = false, CompilerOptions = "/optimize /unsafe"
            };

            cParameters.ReferencedAssemblies.Add( Assembly.GetExecutingAssembly().Location );
            cParameters.ReferencedAssemblies.Add( this.Type.Assembly.Location );

            return cParameters;
        }

        private void MovePointers( [CanBeNull] StringBuilder sb ) {
            var modifer = this._addCount / this._ptrSize;

            if ( modifer >= this._ptrSize ) {
                sb.Append( $"dest += {this._addCount};" );
                sb.Append( $"src += {this._addCount};" );
                this._addCount = 0;
            }
        }

        private void ObjectToBytesCode( [NotNull] StringBuilder sb, [CanBeNull] String? typeFullName ) {
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

        [CanBeNull]
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

            return ( ISerializeDeserialize<T> ) res.CompiledAssembly.CreateInstance( "UnsafeConverter" );
        }

    }

}