// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "BigNullStream.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/BigNullStream.cs" was last formatted by Protiguous on 2018/05/24 at 7:07 PM.

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using Exceptions;

    /// <summary>
    ///     TODO make this class able to use a BigInteger?
    /// </summary>
    public abstract class BigNullStream : Stream {

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

        public override IAsyncResult BeginRead( Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state ) {
            if ( !this.CanRead ) { throw new StreamReadException( "This stream doesn't support reading." ); }

            throw new UnknownException();
        }

        public override void Flush() { }

        public override Int32 Read( Byte[] buffer, Int32 offset, Int32 count ) {
            if ( !this.CanRead ) { throw new StreamReadException( "This stream doesn't support reading." ); }

            throw new UnknownException();
        }

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