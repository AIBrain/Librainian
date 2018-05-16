// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "NullStream.cs",
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
// "Librainian/Librainian/NullStream.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

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

                if ( this._position > this._length ) { this._length = this._position; }
            }
        }

        public override IAsyncResult BeginRead( Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state ) => throw new NotImplementedException( "This stream doesn't support reading." );

        public override void Flush() { }

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

            if ( newPosition < 0 ) { throw new ArgumentException( "Attempt to seek before start of stream." ); }

            this.Position = newPosition;

            return newPosition;
        }

        public override void SetLength( Int64 value ) => this._length = value;

        public override void Write( Byte[] buffer, Int32 offset, Int32 count ) => this.Seek( count, SeekOrigin.Current );
    }
}