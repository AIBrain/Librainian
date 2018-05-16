// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "StreamString.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/StreamString.cs" was last cleaned by Protiguous on 2018/05/15 at 10:48 PM.

namespace Librainian.OperatingSystem.Streams {

    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://github.com/firepacket/anark.it/" />
    public class StreamString {

        private readonly Stream _ioStream;

        private readonly UnicodeEncoding _streamEncoding;

        public StreamString( Stream ioStream ) {
            this._ioStream = ioStream;
            this._streamEncoding = new UnicodeEncoding();
        }

        public String ReadString() {
            var len = this._ioStream.ReadByte() * 256;
            len += this._ioStream.ReadByte();
            var inBuffer = new Byte[len];
            this._ioStream.Read( inBuffer, 0, len );

            return this._streamEncoding.GetString( inBuffer );
        }

        public Int32 WriteString( String outString ) {
            var outBuffer = this._streamEncoding.GetBytes( outString );
            var len = outBuffer.Length;

            if ( len > UInt16.MaxValue ) { len = UInt16.MaxValue; }

            this._ioStream.WriteByte( ( Byte )( len / 256 ) );
            this._ioStream.WriteByte( ( Byte )( len & 255 ) );
            this._ioStream.Write( outBuffer, 0, len );
            this._ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}