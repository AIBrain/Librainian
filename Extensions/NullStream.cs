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
// "Librainian2/NullStream.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.IO;

    public class NullStream : Stream {
        private long _length;

        private long _position;

        public override Boolean CanRead { get { return false; } }

        public override Boolean CanWrite { get { return true; } }

        public override Boolean CanSeek { get { return true; } }

        public override long Length { get { return this._length; } }

        public override long Position {
            get { return this._position; }
            set {
                this._position = value;
                if ( this._position > this._length ) {
                    this._length = this._position;
                }
            }
        }

        public override void Flush() { }

        public override IAsyncResult BeginRead( byte[] buffer, int offset, int count, AsyncCallback callback, object state ) {
            throw new NotImplementedException( "This stream doesn't support reading." );
        }

        public override long Seek( long offset, SeekOrigin origin ) {
            var newPosition = this.Position;

            switch ( origin ) {
                case SeekOrigin.Begin:
                    newPosition = offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = this.Position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = this.Length + offset;
                    break;
            }
            if ( newPosition < 0 ) {
                throw new ArgumentException( "Attempt to seek before start of stream." );
            }
            this.Position = newPosition;
            return newPosition;
        }

        public override void SetLength( long value ) {
            this._length = value;
        }

        public override int Read( byte[] buffer, int offset, int count ) {
            throw new NotImplementedException( "This stream doesn't support reading." );
        }

        public override void Write( byte[] buffer, int offset, int count ) {
            this.Seek( count, SeekOrigin.Current );
        }
    }
}
