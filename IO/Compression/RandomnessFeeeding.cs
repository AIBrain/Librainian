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
// "Librainian/RandomnessFeeeding.cs" was last cleaned by Rick on 2014/12/09 at 5:55 AM

namespace Librainian.IO.Compression {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Numerics;

    public class RandomnessFeeeding : IDisposable {
        [ NotNull ] private readonly NullStream _nullStream = new NullStream();
        [ NotNull ] private readonly GZipStream _gZipStream;

        public RandomnessFeeeding() {
            this.HowManyBytesAsCompressed = BigInteger.Zero;
            this.HowManyBytesFed = BigInteger.Zero;
            this._gZipStream = new GZipStream( stream: this._nullStream, compressionLevel: CompressionLevel.Optimal );
        }

        public BigInteger HowManyBytesAsCompressed { get; private set; }

        public BigInteger HowManyBytesFed { get; private set; }

        public void Dispose() {
            using ( _gZipStream ) {
                this._gZipStream.Close();
            }

            using ( _nullStream ) {
                this._nullStream.Close();
            }
        }

        public void FeedItData( [NotNull] Byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( nameof( data ) );
            }
            this.HowManyBytesFed += data.LongLength;
            this._gZipStream.Write( data, 0, data.Length );
            this.HowManyBytesAsCompressed += this._nullStream.Length;
            this._nullStream.Seek( 0, SeekOrigin.Begin ); //rewind our 'position' so we don't overrun a long
        }

        public void FeedItData( Document document ) {
            var data = document.AsByteArray().ToArray();
            this.FeedItData( data );
        }

        /// <summary>
        /// The smaller the compressed 'data' is, the less the random it was.
        /// </summary>
        /// <returns></returns>
        public Double GetCurrentCompressionRatio() {
	        var d = ( Double ) new BigRational( this.HowManyBytesAsCompressed, this.HowManyBytesFed );
            return 1 - d;
        }

        public void Report() => Debug.WriteLine( "Current compression is now {0:P4}", this.GetCurrentCompressionRatio() );
    }
}