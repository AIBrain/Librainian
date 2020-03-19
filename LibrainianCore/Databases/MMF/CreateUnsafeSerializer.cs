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
// Project: "LibrainianCore", File: "CreateUnsafeSerializer.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.Databases.MMF {

    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>Class which tries to create a ISerializeDeserialize based on pointer movement (unsafe).</summary>
    /// <typeparam name="T"></typeparam>
    public class CreateUnsafeSerializer<T> {

        private Int32 _addCount;

        private Int32 _ptrSize = sizeof( Int64 );

        private String _ptrType = "Int64";

        private Int32 _size;

        private Type Type { get; } = typeof( T );

        private static void BytesToObjectCode( [NotNull] StringBuilder sb, [CanBeNull] String typeFullName ) {
            sb.Append( value: $"public unsafe {typeFullName} BytesToObject( byte[] bytes )" );
            sb.Append( value: "{" );

            sb.Append( value: @"
                fixed (byte* srcPtr = &bytes[0])
                {" );

            sb.Append( value: $"return *({typeFullName}*)srcPtr;" );
            sb.Append( value: "}}" );
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
        private String GenerateCode() {
            var typeFullName = this.Type.FullName.Replace( oldChar: '+', newChar: '.' );

            var sb = new StringBuilder();
            sb.AppendLine( value: "using System;" );
            sb.AppendLine();

            var interfaceType = typeof( ISerializeDeserialize<T> );

            sb.Append( value: $"public class UnsafeConverter : {interfaceType.Namespace}.ISerializeDeserialize<{typeFullName}>" );
            sb.Append( value: "{" );
            sb.AppendFormat( format: "public Boolean CanSerializeType(){{return true;}}" );

            this.ObjectToBytesCode( sb: sb, typeFullName: typeFullName );
            BytesToObjectCode( sb: sb, typeFullName: typeFullName );

            sb.Append( value: "}" );

            return sb.ToString();
        }

        private void GenerateMethodBodyCode( [NotNull] StringBuilder sb ) {
            this._addCount = 0;
            var length = this._size;

            do {
                this.MovePointers( sb: sb );
                this.SetPointerLength( length: length );
                sb.AppendFormat( format: @"*(({0}*)dest+{1}) = *(({0}*)src+{1});", arg0: this._ptrType, arg1: this._addCount / this._ptrSize );
                length -= this._ptrSize;
                this._addCount += this._ptrSize;
            } while ( length > 0 );
        }

        private void MovePointers( [CanBeNull] StringBuilder sb ) {
            var modifer = this._addCount / this._ptrSize;

            if ( modifer >= this._ptrSize ) {
                sb.Append( value: $"dest += {this._addCount};" );
                sb.Append( value: $"src += {this._addCount};" );
                this._addCount = 0;
            }
        }

        private void ObjectToBytesCode( [NotNull] StringBuilder sb, [CanBeNull] String typeFullName ) {
            sb.Append( value: $"public unsafe byte[] ObjectToBytes({typeFullName} srcObject)" );
            sb.Append( value: "{" );
            sb.Append( value: $"byte[] buffer = new byte[{this._size}];" );

            sb.Append( value: @"
                fixed (byte* destPtr = &buffer[0])
                {
                    " );

            sb.Append( value: "byte* src = (byte*)&srcObject;" );
            sb.Append( value: "byte* dest = destPtr;" );

            this.GenerateMethodBodyCode( sb: sb );

            sb.Append( value: @"}
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
    }
}