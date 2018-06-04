// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Year.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Year.cs" was last formatted by Protiguous on 2018/06/04 at 4:13 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Numerics;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>A simple struct for a Year.</summary>
	[JsonObject]
	[Immutable]
	public struct Year : IComparable<Year>, IClockPart {

		[JsonProperty]
		public BigInteger Value { get; }

		public static readonly Year Zero = new Year( 0 );

		public Year( BigInteger value ) : this() => this.Value = value;

		public static implicit operator BigInteger( Year value ) => value.Value;

		public static Boolean operator <( Year left, Year right ) => left.Value < right.Value;

		public static Boolean operator >( Year left, Year right ) => left.Value > right.Value;

		public Int32 CompareTo( Year other ) => this.Value.CompareTo( other.Value );

		public Year Next() => new Year( this.Value + 1 );

		public Year Previous() => new Year( this.Value - 1 );

	}

}