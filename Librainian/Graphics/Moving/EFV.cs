// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Graphics.Moving {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary> Experimental Full Video </summary>
	/// <remarks>
	/// Just for fun & learning. Prefer compression over [decoding/display] speed (assuming local cpu will be 'faster' than network transfer speed). Compressions must be
	/// lossless. Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay. Checksums are used on each <see cref="Pixelyx" /> to guard against (detect but
	/// not fix) corruption.
	/// </remarks>
	[JsonObject]
	public class Efv {

		public static readonly String Extension = ".efv";

		/// <summary>Human readable file header.</summary>
		public static readonly String Header = "EFV0.1";

		/// <summary>For each item here, draw them too.</summary>
		/// <remarks>I need to stop coding while I'm asleep.</remarks>
		[JsonProperty]
		[NotNull]
		public ConcurrentDictionary<UInt64, IList<UInt64>> Dopples = new ConcurrentDictionary<UInt64, IList<UInt64>>();

		[JsonProperty]
		[NotNull]
		public ConcurrentDictionary<UInt64, Pixelyx> Pixels = new ConcurrentDictionary<UInt64, Pixelyx>();

		/// <summary>Checksum guard</summary>
		[JsonProperty]
		public UInt64 Checksum { get; set; }

		[JsonProperty]
		public UInt16 Height { get; set; }

		[JsonProperty]
		public UInt16 Width { get; set; }

		public Efv() => this.Checksum = UInt64.MaxValue;

		public Boolean Add( [NotNull] Pixelyx pixelyx ) {
			var rgbMatchesJustNotTimestamp = this.Pixels.Where( pair => Pixelyx.Equal( pair.Value, pixelyx ) );

			foreach ( var pair in rgbMatchesJustNotTimestamp ) {
				this.Dopples[pixelyx.Timestamp] ??= new List<UInt64>();

				this.Dopples[ pixelyx.Timestamp ].Add( pair.Value.Timestamp );
			}

			this.Pixels[ pixelyx.Timestamp ] = pixelyx;

			return true;
		}

		public Int32 CalculateChecksum() {
			var sum = 0;

			foreach ( var pixelyx in this.Pixels ) {
				unchecked {
					sum += pixelyx.GetHashCode();
				}
			}

			return this.Pixels.Count + sum;
		}

		[NotNull]
		public Task<UInt64> CalculateChecksumAsync() =>
			Task.Run( () => {
				unchecked {
					return ( UInt64 )this.Pixels.GetHashCode();
				}
			} );

		[CanBeNull]
		public Pixelyx Get( UInt64 index ) => this.Pixels.TryGetValue( index, out var pixelyx ) ? pixelyx : null;

		[CanBeNull]
		public Pixelyx Get( UInt16 x, UInt16 y ) {
			if ( x == 0 ) {
				throw new ArgumentException( "x" );
			}

			if ( y == 0 ) {
				throw new ArgumentException( "y" );
			}

			var index = ( UInt64 )( ( this.Height * y ) + x );

			return this.Get( index );
		}
	}
}