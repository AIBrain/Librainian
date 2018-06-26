// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Day.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Day.cs" was last formatted by Protiguous on 2018/06/26 at 1:26 AM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Linq;
	using Extensions;
	using Maths;
	using Newtonsoft.Json;

	/// <summary>A simple struct for a Day of the month.</summary>
	[JsonObject]
	[Immutable]
	public struct Day : IClockPart {

		public static readonly Byte[] ValidDays = 1.To( 31 ).Select( i => ( Byte ) i ).OrderBy( b => b ).ToArray();

		public static Day Maximum { get; } = new Day( MaximumValue );

		/// <summary>should be 31</summary>
		public static Byte MaximumValue { get; } = ValidDays.Max();

		public static Day Minimum { get; } = new Day( MinimumValue );

		/// <summary>should be 1</summary>
		public static Byte MinimumValue { get; } = ValidDays.Min();

		[JsonProperty]
		public Byte Value { get; }

		public Day( Byte value ) : this() {
			if ( !ValidDays.Contains( value ) ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		public static explicit operator SByte( Day value ) => ( SByte ) value.Value;

		public static implicit operator Byte( Day value ) => value.Value;

		/// <summary>Provide the next <see cref="Day" />.</summary>
		public Day Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				tocked = true;
			}

			return new Day( ( Byte ) next );
		}

		/// <summary>Provide the previous <see cref="Day" />.</summary>
		public Day Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				tocked = true;
			}

			return new Day( ( Byte ) next );
		}
	}
}