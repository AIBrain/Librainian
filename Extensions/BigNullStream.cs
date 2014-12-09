namespace Librainian.Extensions {
    using System;
    using System.IO;

    /// <summary>
    /// TODO make this class able to use a BigInteger
    /// </summary>
    public abstract class BigNullStream : Stream {
        private long _length;

        private long _position;

        public override Boolean CanRead { get { return false; } }

        public override Boolean CanSeek { get { return true; } }

        public override Boolean CanWrite { get { return true; } }

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

        public override IAsyncResult BeginRead( byte[] buffer, int offset, int count, AsyncCallback callback, object state ) {
            throw new NotImplementedException( "This stream doesn't support reading." );
        }

        public override void Flush() { }

        public override int Read( byte[] buffer, int offset, int count ) {
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

        public override void SetLength( long value ) => this._length = value;

        public override void Write( byte[] buffer, int offset, int count ) => this.Seek( count, SeekOrigin.Current );
    }
}