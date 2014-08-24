
namespace Librainian.IO.Compression {
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Numerics;
    using Annotations;
    using Extensions;
    using Numerics;

    public class RandomnessTest {

        public BigInteger HowManyBytesFed = BigInteger.Zero;
        public BigInteger HowManyBytesAsCompressed = BigInteger.Zero;

        /// <summary>
        /// The higher the compressed 'data' is, the less the randomness it was.
        /// </summary>
        /// <returns></returns>
        public Double GetCurrentCompressionRatio() {
            var result = 1.0 - new BigRational( HowManyBytesAsCompressed, this.HowManyBytesFed );
            return ( Double )result;
        }

        private readonly GZipStream _gZipStream;

        private readonly NullStream _nullStream;

        public RandomnessTest() {
            this._nullStream = new NullStream();
            this._gZipStream = new GZipStream( stream: this._nullStream, compressionLevel: CompressionLevel.Optimal);
        }

        public void FeedItData( [NotNull] byte[] data) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            HowManyBytesFed += data.LongLength;
            this._gZipStream.Write( data, 0, data.Length );
            HowManyBytesAsCompressed += this._nullStream.Length;
            this._nullStream.Seek( 0, SeekOrigin.Begin );   //rewind our 'position' so we don't overrun a long
        }

        private void Report() {
            Console.WriteLine( String.Format( "Current compression is now {0:P4}", this.GetCurrentCompressionRatio() ) );
        }

        public void FeedItData( Document document ) {
            var data = document.AsByteArray().ToArray();
            FeedItData( data );
        }

    }
}
