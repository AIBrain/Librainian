// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "RandomnessFeeding.cs" last touched on 2021-03-07 at 12:57 AM by Protiguous.

namespace Librainian.OperatingSystem.Compression {

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Numerics;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using Extensions;
	using FileSystem;
	using Rationals;
	using Utilities;

	public class RandomnessFeeding : ABetterClassDispose {

		private GZipStream GZipStream { get; }

		private NullStream NullStream { get; } = new();

		public BigInteger HowManyBytesAsCompressed { get; private set; }

		public BigInteger HowManyBytesFed { get; private set; }

		public RandomnessFeeding() {
			this.HowManyBytesAsCompressed = BigInteger.Zero;
			this.HowManyBytesFed = BigInteger.Zero;
			this.GZipStream = new GZipStream( this.NullStream, CompressionLevel.Optimal );
		}

		public override void DisposeManaged() {
			using ( this.GZipStream ) { }

			using ( this.NullStream ) { }
		}

		public void FeedItData( Byte[] data ) {
			if ( data is null ) {
				throw new ArgumentEmptyException( nameof( data ) );
			}

			this.HowManyBytesFed += data.LongLength;
			this.GZipStream.Write( data, 0, data.Length );
			this.HowManyBytesAsCompressed += this.NullStream.Length;
			this.NullStream.Seek( 0, SeekOrigin.Begin ); //rewind our 'position' so we don't overrun a long
		}

		public async Task FeedItDataAsync( Document document, CancellationToken cancellationToken ) {
			if ( document == null ) {
				throw new ArgumentEmptyException( nameof( document ) );
			}

			this.FeedItData( await document.AsBytes( cancellationToken ).ToArrayAsync( cancellationToken ).ConfigureAwait( false ) );
		}

		/// <summary>The smaller the compressed 'data' is, the less the random it was.</summary>
		/// <returns></returns>
		public Decimal GetCurrentCompressionRatio() {
			var d = ( Decimal )new Rational( this.HowManyBytesAsCompressed, this.HowManyBytesFed );

			return 1 - d; // BUG ?
		}

		public void Report() => Debug.WriteLine( $"Current compression is now {this.GetCurrentCompressionRatio():P4}" );
	}
}