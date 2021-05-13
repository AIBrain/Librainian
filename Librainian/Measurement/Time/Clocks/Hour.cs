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
// File "Hour.cs" last formatted on 2021-02-03 at 3:02 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A simple record for an <see cref="Hour" />.</para>
	/// </summary>
	[JsonObject]
	[Immutable]
	public record Hour : IClockPart {
		public const Byte MaximumValue = Hours.InOneDay;

		public const Byte MinimumValue = 1;

		[JsonProperty]
		public readonly Byte Value;

		public Hour( Byte value ) {
			if ( value is < MinimumValue or > MaximumValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		public static Hour Maximum { get; } = new( MaximumValue );

		public static Hour Minimum { get; } = new( MinimumValue );

		public static explicit operator Byte( Hour value ) => value.Value;

		public static implicit operator Hour( Byte value ) => new( value );

		/// <summary>
		///     Provide the next <see cref="Hour" />.
		/// </summary>
		public Hour Next( out Boolean tocked ) {
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Hour )next;
		}

		/// <summary>
		///     Provide the previous <see cref="Hour" />.
		/// </summary>
		public Hour Previous( out Boolean tocked ) {
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Hour )next;
		}
	}
}