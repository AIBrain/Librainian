
namespace Librainian.IO.Compression {
    using System;
    using System.IO;
    using System.IO.Compression;
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

        public GZipStream GZipStream;

        public readonly NullStream NullStream;

        public RandomnessTest() {
            this.NullStream = new NullStream();
            this.GZipStream = new GZipStream( stream: NullStream, compressionLevel: CompressionLevel.Optimal);
        }

        public void FeedItData( [NotNull] byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            HowManyBytesFed += data.LongLength;
            var compressed = Compress( data );
            HowManyBytesAsCompressed += compressed.LongLength;

            Console.WriteLine( String.Format( "Current compression is now {0:P4}", this.GetCurrentCompressionRatio() ));
        }

        public void FeedItData( Document document ) {
            //TODO
        }

        public static byte[] Compress( [NotNull] byte[] data ) {
           if ( data == null ) {
               throw new ArgumentNullException( "data" );
           }
           using ( var output = new MemoryStream() ) {
               using ( var compress = new GZipStream( output, CompressionLevel.Optimal ) ) {
                   compress.Write( data, 0, data.Length );
               }
               return output.ToArray();
           }
       }
    }
}
