// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Hour.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Hour.cs" was last formatted by Protiguous on 2018/11/03 at 7:39 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A simple struct for an <see cref="Hour" />.</para>
	/// </summary>
	[JsonObject]
	[Immutable]
	public struct Hour : IClockPart {

		public const SByte MaxValue = Hours.InOneDay;

		public const SByte MinValue = 1;

		[JsonProperty]
		public readonly SByte Value;

		public static Hour Maximum { get; } = new Hour( MaxValue );

		public static Hour Minimum { get; } = new Hour( MinValue );

		public Hour( SByte value ) {
			if ( value < MinValue || value > MaxValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinValue} to {MaxValue}." );
			}

			this.Value = value;
		}

		public static explicit operator Byte( Hour value ) => ( Byte )value.Value;

		public static implicit operator Hour( Byte value ) => new Hour( ( SByte )value );

		public static implicit operator SByte( Hour value ) => value.Value;

		/// <summary>
		///     Provide the next <see cref="Hour" />.
		/// </summary>
		public Hour Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > Maximum ) {
				next = Minimum;
				tocked = true;
			}

			return ( Hour )next;
		}

		/// <summary>
		///     Provide the previous <see cref="Hour" />.
		/// </summary>
		public Hour Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < Minimum ) {
				next = Maximum;
				tocked = true;
			}

			return ( Hour )next;
		}
	}
}