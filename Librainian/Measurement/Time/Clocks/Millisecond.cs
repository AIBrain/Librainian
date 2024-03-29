// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Millisecond.cs" last formatted on 2020-08-27 at 7:33 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>A simple struct for a <see cref="Millisecond" />.</summary>
	[JsonObject]
	[Immutable]
	public struct Millisecond : IClockPart {

		public const Int32 MaxValue = 999;

		public const Int32 MinValue = 0;

		public static Millisecond Maximum { get; } = new( MaxValue );

		public static Millisecond Minimum { get; } = new( MinValue );

		[JsonProperty]
		public Int16 Value { get; }

		public Millisecond( Int16 value ) {
			if ( value < MinValue ) {
				value = MinValue;
			}
			else if ( value > MaxValue ) {
				value = MaxValue;
			}

			this.Value = value;
		}

		public static explicit operator UInt16( Millisecond value ) => ( UInt16 )value.Value;

		/// <summary>Allow this class to be visibly cast to an <see cref="Int16" />.</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator Int16( Millisecond value ) => value.Value;

		public static implicit operator Millisecond( Int16 value ) => new( value );

		/// <summary>
		///     Provide the next <see cref="Millisecond" />.
		/// </summary>
		/// <param name="tocked">True when the <see cref="Value" /> went higher than <see cref="Maximum" />.</param>
		/// <returns></returns>
		public Millisecond Next( out Boolean tocked ) {
			var next = this.Value + 1;

			if ( next > MaxValue ) {
				next = MinValue;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Int16 )next;
		}

		/// <summary>Provide the previous <see cref="Millisecond" />.</summary>
		public Millisecond Previous( out Boolean tocked ) {
			var next = this.Value - 1;

			if ( next < MinValue ) {
				next = MaxValue;
				tocked = true;
			}
			else {
				tocked = false;
			}

			return ( Int16 )next;
		}
	}
}