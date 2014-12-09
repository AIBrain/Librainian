// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/StreamString.cs" was last cleaned by Rick on 2014/12/09 at 5:56 AM

namespace Librainian.IO.Streams {

    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://github.com/firepacket/anark.it/"/>
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
            var inBuffer = new byte[ len ];
            this._ioStream.Read( inBuffer, 0, len );

            return this._streamEncoding.GetString( inBuffer );
        }

        public int WriteString( String outString ) {
            var outBuffer = this._streamEncoding.GetBytes( outString );
            var len = outBuffer.Length;
            if ( len > UInt16.MaxValue ) {
                len = UInt16.MaxValue;
            }
            this._ioStream.WriteByte( ( byte )( len / 256 ) );
            this._ioStream.WriteByte( ( byte )( len & 255 ) );
            this._ioStream.Write( outBuffer, 0, len );
            this._ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}