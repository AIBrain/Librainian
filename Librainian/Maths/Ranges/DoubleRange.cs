// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DoubleRange.cs" belongs to Rick@AIBrain.org and
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
// File "DoubleRange.cs" was last formatted by Protiguous on 2018/06/04 at 4:07 PM.

namespace Librainian.Maths.Ranges {

	using System;
	using Newtonsoft.Json;

	/// <summary>Represents a Double range with minimum and maximum values</summary>
	/// <remarks>
	///     Modified from the AForge Library Copyright © Andrew Kirillov, 2006,
	///     andrew.kirillov@gmail.com
	/// </remarks>
	[JsonObject]
	public struct DoubleRange {

		public static readonly DoubleRange ZeroToOne = new DoubleRange( 0, 1 );

		/// <summary>Length of the range (difference between maximum and minimum values)</summary>
		[JsonProperty]
		public readonly Double Length;

		/// <summary>Maximum value</summary>
		[JsonProperty]
		public readonly Double Max;

		/// <summary>Minimum value</summary>
		[JsonProperty]
		public readonly Double Min;

		/// <summary>Initializes a new instance of the <see cref="DoubleRange" /> class</summary>
		/// <param name="min">Minimum value of the range</param>
		/// <param name="max">Maximum value of the range</param>
		public DoubleRange( Double min, Double max ) {
			this.Min = Math.Min( min, max );
			this.Max = Math.Max( min, max );
			this.Length = this.Max - this.Min;
		}

		/// <summary>Check if the specified range is inside this range</summary>
		/// <param name="range">Range to check</param>
		/// <returns>
		///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
		/// </returns>
		public Boolean IsInside( DoubleRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

		/// <summary>Check if the specified value is inside this range</summary>
		/// <param name="x">Value to check</param>
		/// <returns>
		///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
		/// </returns>
		public Boolean IsInside( Double x ) => this.Min <= x && x <= this.Max;

		/// <summary>Check if the specified range overlaps with this range</summary>
		/// <param name="range">Range to check for overlapping</param>
		/// <returns>
		///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
		/// </returns>
		public Boolean IsOverlapping( DoubleRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );

	}

}