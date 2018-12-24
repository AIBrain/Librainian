// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Year.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Year.cs" was last formatted by Protiguous on 2018/08/23 at 7:10 PM.

namespace Librainian.Measurement.Time.Clocks {

	using Extensions;
	using Newtonsoft.Json;
	using System;
	using System.Numerics;

	/// <summary>A simple struct for a Year.</summary>
	[JsonObject]
	[Immutable]
	public struct Year : IComparable<Year>, IClockPart {

		public static Year Zero { get; } = new Year(0);

		public Int32 MaxValue { get; }

		public Int32 MinValue { get; }

		[JsonProperty]
		public BigInteger Value { get; }

		public Year(BigInteger value) : this() => this.Value = value;

		public static implicit operator BigInteger(Year value) => value.Value;

		public static Boolean operator <(Year left, Year right) => left.Value < right.Value;

		public static Boolean operator >(Year left, Year right) => left.Value > right.Value;

		public Int32 CompareTo(Year other) => this.Value.CompareTo(other.Value);

		public Year Next() => new Year(this.Value + 1);

		public Year Previous() => new Year(this.Value - 1);
	}
}