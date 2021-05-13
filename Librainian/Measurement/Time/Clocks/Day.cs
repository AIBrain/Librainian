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
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>A simple record for a Day of the month.</summary>
	[JsonObject]
	[Immutable]
	public record Day : IClockPart {
		public const Byte MaximumValue = 31;

		public const Byte MinimumValue = 1;

		public Day( Byte value ) {
			if ( value is < MinimumValue or > MaximumValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		/// <summary>31</summary>
		public static Day Maximum { get; } = new( MaximumValue );

		/// <summary>1</summary>
		public static Day Minimum { get; } = new( MinimumValue );

		[JsonProperty]
		public Byte Value { get; init; }

		public static implicit operator Byte( Day value ) => value.Value;

		public static implicit operator Day( Byte value ) => new( value );

		/// <summary>Provide the next <see cref="Day" />.</summary>
		public Day Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > Maximum ) {
				tocked = true;

				return Minimum;
			}

			return ( Day )next;
		}

		public static explicit operator Day( Int32 v ) => new( ( Byte )v );

		/// <summary>Provide the previous <see cref="Day" />.</summary>
		public Day Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < Minimum ) {
				tocked = true;

				return Maximum;
			}

			return ( Day )next;
		}
	}
}