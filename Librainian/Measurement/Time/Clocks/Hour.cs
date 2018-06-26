// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Hour.cs" belongs to Protiguous@Protiguous.com
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
// File "Hour.cs" was last formatted by Protiguous on 2018/06/26 at 1:27 AM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Linq;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A simple struct for an <see cref="Hour" />.</para>
	/// </summary>
	[JsonObject]
	[Immutable]
	public struct Hour : IClockPart {

		public static readonly Byte[] ValidHours = Enumerable.Range( start: 0, count: Hours.InOneDay ).Select( i => ( Byte ) i ).OrderBy( b => b ).ToArray();

		[JsonProperty]
		public readonly Byte Value;

		public static Hour Maximum { get; } = new Hour( MaximumValue );

		/// <summary>
		///     should be 23
		/// </summary>
		public static Byte MaximumValue { get; } = ValidHours.Max();

		public static Hour Minimum { get; } = new Hour( MinimumValue );

		/// <summary>
		///     should be 0
		/// </summary>
		public static Byte MinimumValue { get; } = ValidHours.Min();

		public Hour( Byte value ) {
			if ( !ValidHours.Contains( value ) ) {
				throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
			}

			this.Value = value;
		}

		/// <summary>
		///     Allow this class to be visibly cast to a <see cref="SByte" />.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static explicit operator SByte( Hour value ) => ( SByte ) value.Value;

		public static implicit operator Byte( Hour value ) => value.Value;

		public static implicit operator Hour( Byte value ) => new Hour( value );

		/// <summary>
		///     Provide the next <see cref="Hour" />.
		/// </summary>
		public Hour Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				tocked = true;
			}

			return ( Byte ) next;
		}

		/// <summary>
		///     Provide the previous <see cref="Hour" />.
		/// </summary>
		public Hour Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				tocked = true;
			}

			return ( Byte ) next;
		}
	}
}