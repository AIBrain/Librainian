// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DecimalRange.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

namespace Librainian.Maths.Ranges;

using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Utilities;

/// <summary>Represents a Decimal range with minimum and maximum values</summary>
/// <remarks>Modified from the AForge Library Copyright © Andrew Kirillov, 2006, andrew.kirillov@gmail.com</remarks>
[JsonObject]
public record DecimalRange {

	/// <summary>Initializes a new instance of the <see cref="DecimalRange" /> class</summary>
	/// <param name="minimum">Minimum value of the range</param>
	/// <param name="maximum">Maximum value of the range</param>
	public DecimalRange( Decimal minimum, Decimal maximum ) {
		if ( minimum > maximum ) {
			this.Minimum = maximum;
			this.Maximum = minimum;
		}
		else {
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		this.Range = this.Maximum - this.Minimum;
	}

	public static DecimalRange ZeroToOne { get; } = new( 0.0M, 1.0M );

	/// <summary>Maximum value</summary>
	[JsonProperty]
	public Decimal Maximum { get; init; }

	/// <summary>Minimum value</summary>
	[JsonProperty]
	public Decimal Minimum { get; init; }

	/// <summary>The difference between maximum and minimum values.</summary>
	[JsonProperty]
	public Decimal Range { get; init; }

	/// <summary>Check if the specified value is inside this range</summary>
	/// <param name="x">Value to check</param>
	/// <returns><b>True</b> if the specified value is inside this range or <b>false</b> otherwise.</returns>
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public Boolean IsInside( Decimal x ) => x.Between( this.Minimum, this.Maximum );

	/// <summary>Check if the specified range is inside this range</summary>
	/// <param name="range">Range to check</param>
	/// <returns><b>True</b> if the specified range is inside this range or <b>false</b> otherwise.</returns>
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public Boolean IsInside( DecimalRange range ) => this.IsInside( range.Minimum ) && this.IsInside( range.Maximum );

	/// <summary>Check if the specified range overlaps with this range</summary>
	/// <param name="range">Range to check for overlapping</param>
	/// <returns><b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.</returns>
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public Boolean IsOverlapping( DecimalRange range ) => this.IsInside( range.Minimum ) || this.IsInside( range.Maximum );
}