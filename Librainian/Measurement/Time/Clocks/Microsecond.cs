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
// File "Microsecond.cs" last formatted on 2021-02-03 at 4:55 PM.

namespace Librainian.Measurement.Time.Clocks {
	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     A simple record for a <see cref="Microsecond" />.
	/// </summary>
	[JsonObject]
	[Immutable]
	public record Microsecond : IClockPart {

		public const Int16 MaximumValue = 999;

		public const Int16 MinimumValue = 0;

		public Microsecond( Int16 value ) {
			if ( value is < MinimumValue or > MaximumValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {Maximum}." );
			}

			this.Value = value;
		}

		public static Microsecond Maximum { get; } = new( MaximumValue );

		public static Microsecond Minimum { get; } = new( MinimumValue );

		[JsonProperty]
		public Int16 Value { get; }

		public static explicit operator UInt16( Microsecond value ) => ( UInt16 )value.Value;

		public static implicit operator Int16( Microsecond value ) => value.Value;

		public static implicit operator Microsecond( UInt16 value ) => new( ( Int16 )value );

		/// <summary>
		///     Provide the next <see cref="Microsecond" />.
		/// </summary>
		public Microsecond Next( out Boolean ticked ) {
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				ticked = true;
			}
			else {
				ticked = false;
			}

			return ( Microsecond )next;
		}

		/// <summary>
		///     Provide the previous <see cref="Microsecond" />.
		/// </summary>
		public Microsecond Previous( out Boolean ticked ) {
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				ticked = true;
			}
			else {
				ticked = false;
			}

			return ( Microsecond )next;
		}

	}
}