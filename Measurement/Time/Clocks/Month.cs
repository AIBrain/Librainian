// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Month.cs" belongs to Rick@AIBrain.org and
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
// File "Month.cs" was last formatted by Protiguous on 2018/06/04 at 4:12 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Linq;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     A simple struct for a <see cref="Month" />.
	/// </summary>
	[JsonObject]
	[Immutable]
	public struct Month : IComparable<Month>, IClockPart {

		/// <summary>
		///     12
		/// </summary>
		public static Byte Maximum { get; } = ValidMonths.Max();

		/// <summary>
		///     1
		/// </summary>
		public static Byte Minimum { get; } = ValidMonths.Min();

		[JsonProperty]
		public Byte Value { get; }

		public static readonly Byte[] ValidMonths = Enumerable.Range( 1, Months.InOneCommonYear + 1 ) //TODO //BUG ??
			.Select( i => ( Byte ) i ).OrderBy( b => b ).ToArray();

		public Month( Byte value ) {
			if ( !ValidMonths.Contains( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {Minimum} to {Maximum}." ); }

			this.Value = value;
		}

		public Int32 CompareTo( Month other ) => this.Value.CompareTo( other.Value );

		/// <summary>
		///     Provide the next <see cref="Month" />.
		/// </summary>
		/// <param name="tocked"></param>
		/// <returns></returns>
		public Month Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > Maximum ) {
				next = Minimum;
				tocked = true;
			}

			return new Month( ( Byte ) next );
		}

		/// <summary>
		///     Provide the previous <see cref="Month" />.
		/// </summary>
		public Month Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < Minimum ) {
				next = Maximum;
				tocked = true;
			}

			return new Month( ( Byte ) next );
		}

	}

}