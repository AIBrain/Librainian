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
// File "ClockDay.cs" last touched on 2021-09-27 at 6:15 AM by Protiguous.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;
	using Utilities;

	/// <summary>A simple record for a ClockDay of the month.</summary>
	[JsonObject]
	[Immutable]
	[NeedsTesting]
	public record ClockDay : IClockPart {

		public const Byte MaximumValue = 31;

		public const Byte MinimumValue = 1;

		public ClockDay( Byte value ) {
			if ( value is < MinimumValue or > MaximumValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		/// <summary>31</summary>
		public static ClockDay Maximum { get; } = new(MaximumValue);

		/// <summary>1</summary>
		public static ClockDay Minimum { get; } = new(MinimumValue);

		[JsonProperty]
		public Byte Value { get; init; }

		public static implicit operator Byte( ClockDay value ) => value.Value;

		public static implicit operator ClockDay( Byte value ) => new(value);

		/// <summary>Provide the next <see cref="ClockDay" />.</summary>
		public ClockDay Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				tocked = true;

				return Minimum;
			}

			return ( ClockDay )next;
		}

		public static explicit operator ClockDay( Int32 v ) => new(( Byte )v);

		/// <summary>Provide the previous <see cref="ClockDay" />.</summary>
		public ClockDay Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				tocked = true;

				return Maximum;
			}

			return ( ClockDay )next;
		}

	}

}