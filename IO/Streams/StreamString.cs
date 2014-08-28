
namespace Librainian.IO.Streams {
    using System.IO;
    using System;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://github.com/firepacket/anark.it/"/>
    public class StreamString {
        private readonly Stream _ioStream;
        private readonly UnicodeEncoding _streamEncoding;

        public StreamString( Stream ioStream ) {
            this._ioStream = ioStream;
            this._streamEncoding = new UnicodeEncoding();
        }

        public string ReadString() {
            var len = this._ioStream.ReadByte() * 256;
            len += this._ioStream.ReadByte();
            var inBuffer = new byte[ len ];
            this._ioStream.Read( inBuffer, 0, len );

            return this._streamEncoding.GetString( inBuffer );
        }

        public int WriteString( string outString ) {
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
