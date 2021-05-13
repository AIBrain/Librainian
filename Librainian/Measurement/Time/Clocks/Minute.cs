// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Minute.cs" last formatted on 2021-02-03 at 4:53 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     A simple struct for a <see cref="Minute" />.
	/// </summary>
	[JsonObject]
	[Immutable]
	public record Minute : IClockPart {
		public const Int32 MaximumValue = 59;

		public const Int32 MinimumValue = 0;

		public Minute( Byte value ) {
			if ( value > MaximumValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		public static Minute Maximum { get; } = new( MaximumValue );

		public static Minute Minimum { get; } = new( MinimumValue );

		[JsonProperty]
		public Byte Value { get; }

		public static explicit operator Byte( Minute value ) => value.Value;

		public static implicit operator Minute( Byte value ) => new( value );

		/// <summary>
		///     Provide the next minute.
		/// </summary>
		public Minute Next( out Boolean tocked ) {
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Minute )next;
		}

		/// <summary>
		///     Provide the previous minute.
		/// </summary>
		public Minute Previous( out Boolean tocked ) {
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Minute )next;
		}
	}
}