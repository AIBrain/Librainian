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
// "Librainian/ContainerStream.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Persistence {
    using System;
    using System.IO;

    public abstract class ContainerStream : Stream {
        protected readonly Stream _stream;

        protected ContainerStream( Stream stream ) {
            if ( null == stream ) {
                throw new ArgumentNullException( "stream" );
            }
            this._stream = stream;
        }

        protected Stream ContainedStream { get { return this._stream; } }

        public override Boolean CanRead { get { return this._stream.CanRead; } }

        public override Boolean CanSeek { get { return this._stream.CanSeek; } }

        public override Boolean CanWrite { get { return this._stream.CanWrite; } }

        public override long Length { get { return this._stream.Length; } }

        public override long Position {
            get {
                //var str = this._stream as IsolatedStorageFileStream;
                //if ( null != str ) { return str.Position; }
                return this._stream.Position;
            }
            set { this._stream.Position = value; }
        }

        public override void Flush() {
            this._stream.Flush();
        }

        public override int Read( byte[] buffer, int offset, int count ) {
            return this._stream.Read( buffer, offset, count );
        }

        public override void Write( byte[] buffer, int offset, int count ) {
            this._stream.Write( buffer, offset, count );
        }
    }
}
