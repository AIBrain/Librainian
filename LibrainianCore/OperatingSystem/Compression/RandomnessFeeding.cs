// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "RandomnessFeeding.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "RandomnessFeeding.cs" was last formatted by Protiguous on 2019/08/08 at 9:13 AM.

namespace LibrainianCore.OperatingSystem.Compression {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Numerics;
    using FileSystem;
    using LibrainianCore.Extensions;
    using Utilities;

    public class RandomnessFeeding : ABetterClassDispose {

        [NotNull]
        private GZipStream GZipStream { get; }

        [NotNull]
        private NullStream NullStream { get; } = new NullStream();

        public BigInteger HowManyBytesAsCompressed { get; private set; }

        public BigInteger HowManyBytesFed { get; private set; }

        public RandomnessFeeding() {
            this.HowManyBytesAsCompressed = BigInteger.Zero;
            this.HowManyBytesFed = BigInteger.Zero;
            this.GZipStream = new GZipStream( stream: this.NullStream, compressionLevel: CompressionLevel.Optimal );
        }

        public override void DisposeManaged() {
            using ( this.GZipStream ) { }

            using ( this.NullStream ) { }
        }

        public void FeedItData( [NotNull] Byte[] data ) {
            if ( data is null ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            this.HowManyBytesFed += data.LongLength;
            this.GZipStream.Write( data, 0, data.Length );
            this.HowManyBytesAsCompressed += this.NullStream.Length;
            this.NullStream.Seek( 0, SeekOrigin.Begin ); //rewind our 'position' so we don't overrun a long
        }

        public void FeedItData( [NotNull] Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            var bytes = document.AsBytes();

            if ( bytes != null ) {
                var data = bytes.ToArray();
                this.FeedItData( data );
            }

        }

        /// <summary>
        ///     The smaller the compressed 'data' is, the less the random it was.
        /// </summary>
        /// <returns></returns>
        public Double GetCurrentCompressionRatio() {
            var d = ( Double )new Rational( this.HowManyBytesAsCompressed, this.HowManyBytesFed );

            return 1 - d; // BUG ?
        }

        public void Report() => Debug.WriteLine( $"Current compression is now {this.GetCurrentCompressionRatio():P4}" );
    }
}