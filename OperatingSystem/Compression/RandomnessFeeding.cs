// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "RandomnessFeeding.cs",
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
// "Librainian/Librainian/RandomnessFeeding.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.OperatingSystem.Compression {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Numerics;
    using Extensions;
    using FileSystem;
    using JetBrains.Annotations;
    using Magic;
    using Numerics;

    public class RandomnessFeeding : ABetterClassDispose {

        public RandomnessFeeding() {
            this.HowManyBytesAsCompressed = BigInteger.Zero;
            this.HowManyBytesFed = BigInteger.Zero;
            this.GZipStream = new GZipStream( stream: this.NullStream, compressionLevel: CompressionLevel.Optimal );
        }

        [NotNull]
        private GZipStream GZipStream { get; }

        [NotNull]
        private NullStream NullStream { get; } = new NullStream();

        public BigInteger HowManyBytesAsCompressed { get; private set; }

        public BigInteger HowManyBytesFed { get; private set; }

        public override void DisposeManaged() {
            using ( this.GZipStream ) { }

            using ( this.NullStream ) { }
        }

        public void FeedItData( [NotNull] Byte[] data ) {
            if ( data is null ) { throw new ArgumentNullException( nameof( data ) ); }

            this.HowManyBytesFed += data.LongLength;
            this.GZipStream.Write( data, 0, data.Length );
            this.HowManyBytesAsCompressed += this.NullStream.Length;
            this.NullStream.Seek( 0, SeekOrigin.Begin ); //rewind our 'position' so we don't overrun a long
        }

        public void FeedItData( Document document ) {
            var data = document.AsBytes().ToArray();
            this.FeedItData( data );
        }

        /// <summary>
        ///     The smaller the compressed 'data' is, the less the random it was.
        /// </summary>
        /// <returns></returns>
        public Double GetCurrentCompressionRatio() {
            var d = ( Double )new BigRational( this.HowManyBytesAsCompressed, this.HowManyBytesFed );

            return 1 - d; // BUG ?
        }

        public void Report() => Debug.WriteLine( $"Current compression is now {this.GetCurrentCompressionRatio():P4}" );
    }
}