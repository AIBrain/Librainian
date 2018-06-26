// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Second.cs" belongs to Rick@AIBrain.org and
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
// File "Second.cs" was last formatted by Protiguous on 2018/06/04 at 4:12 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Linq;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>A simple struct for a <see cref="Second" />.</summary>
	[JsonObject]
	[Immutable]
	public sealed class Second : IClockPart {

		public static Second Maximum { get; } = new Second( MaximumValue );

		/// <summary>should be 59</summary>
		public static Byte MaximumValue { get; } = ValidSeconds.Max();

		public static Second Minimum { get; } = new Second( MinimumValue );

		/// <summary>should be 0</summary>
		public static Byte MinimumValue { get; } = ValidSeconds.Min();

		[JsonProperty]
		public readonly Byte Value;

		/// <summary>Allow this class to be visibly cast to a <see cref="SByte" />.</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static explicit operator SByte( [NotNull] Second value ) => ( SByte ) value.Value;

		/// <summary>Allow this class to be read as a <see cref="Byte" />.</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator Byte( [NotNull] Second value ) => value.Value;

		[NotNull]
		public static implicit operator Second( Byte value ) => new Second( value );

		/// <summary>Provide the next second.</summary>
		[NotNull]
		public Second Next( out Boolean tocked ) {
			tocked = false;
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				tocked = true;
			}

			return ( Byte ) next;
		}

		/// <summary>Provide the previous second.</summary>
		[NotNull]
		public Second Previous( out Boolean tocked ) {
			tocked = false;
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				tocked = true;
			}

			return ( Byte ) next;
		}

		public static readonly Byte[] ValidSeconds = Enumerable.Range( 0, Seconds.InOneMinute ).Select( i => ( Byte ) i ).OrderBy( b => b ).ToArray();

		public Second( Byte value ) {
			if ( !ValidSeconds.Contains( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." ); }

			this.Value = value;
		}

	}

}