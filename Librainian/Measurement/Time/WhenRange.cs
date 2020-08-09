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

namespace Librainian.Measurement.Time {

	using Newtonsoft.Json;

	/// <summary>Represents a <see cref="UniversalDateTime" /> range with minimum and maximum values.</summary>
	[JsonObject]
	public struct WhenRange {

		/// <summary>Length of the range (difference between maximum and minimum values).</summary>
		[JsonProperty]
		public SpanOfTime Length;

		/// <summary>Maximum value</summary>
		[JsonProperty]
		public UniversalDateTime Max;

		/// <summary>Minimum value</summary>
		[JsonProperty]
		public UniversalDateTime Min;

		/// <summary>Initializes a new instance of the <see cref="WhenRange" /> struct</summary>
		/// <param name="min">Minimum value of the range</param>
		/// <param name="max">Maximum value of the range</param>
		public WhenRange( UniversalDateTime min, UniversalDateTime max ) {
			if ( min < max ) {
				this.Min = min;
				this.Max = max;
			}
			else {
				this.Min = max;
				this.Max = min;
			}

			var δ = this.Max.Value - this.Min.Value;
			this.Length = new SpanOfTime( δ );
		}
	}
}