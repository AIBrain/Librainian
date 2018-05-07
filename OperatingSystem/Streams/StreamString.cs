// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/StreamString.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

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
            var inBuffer = new Byte[ len ];
            this._ioStream.Read( inBuffer, 0, len );

            return this._streamEncoding.GetString( inBuffer );
        }

        public Int32 WriteString( String outString ) {
            var outBuffer = this._streamEncoding.GetBytes( outString );
            var len = outBuffer.Length;
            if ( len > UInt16.MaxValue ) {
                len = UInt16.MaxValue;
            }
            this._ioStream.WriteByte( ( Byte )( len / 256 ) );
            this._ioStream.WriteByte( ( Byte )( len & 255 ) );
            this._ioStream.Write( outBuffer, 0, len );
            this._ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}