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
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NullStream.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Extensions {

    using System;
    using System.IO;

    public class NullStream : Stream {
        private Int64 _length;
        private Int64 _position;

        public override Boolean CanRead => false;

        public override Boolean CanSeek => true;

        public override Boolean CanWrite => true;

        public override Int64 Length => this._length;

        public override Int64 Position {
            get => this._position;

	        set {
                this._position = value;
                if ( this._position > this._length ) {
                    this._length = this._position;
                }
            }
        }

        public override IAsyncResult BeginRead( Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state ) => throw new NotImplementedException( "This stream doesn't support reading." );

	    public override void Flush() {
        }

        public override Int32 Read( Byte[] buffer, Int32 offset, Int32 count ) => throw new NotImplementedException( "This stream doesn't support reading." );

	    public override Int64 Seek( Int64 offset, SeekOrigin origin ) {
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

        public override void SetLength( Int64 value ) => this._length = value;

        public override void Write( Byte[] buffer, Int32 offset, Int32 count ) => this.Seek( count, SeekOrigin.Current );
    }
}