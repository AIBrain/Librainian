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
// File "ClockMicrosecond.cs" last touched on 2022-01-01 at 5:06 AM by Protiguous.

namespace Librainian.Measurement.Time.Clocks;

using System;
using Extensions;
using Newtonsoft.Json;
using Utilities;

/// <summary>A simple struct for a <see cref="ClockMicrosecond" />.</summary>
[JsonObject]
[Immutable]
[NeedsTesting]
public record ClockMicrosecond : IClockPart {

	public const UInt16 MaximumValue = 999;

	public const UInt16 MinimumValue = 0;

	public ClockMicrosecond( UInt16 value ) {
		if ( value > MaximumValue ) {
			//throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {Maximum}." );
			value = MaximumValue;
		}

		this.Value = value;
	}

	public static ClockMicrosecond Maximum { get; } = new(MaximumValue);

	public static ClockMicrosecond Minimum { get; } = new(MinimumValue);

	[JsonProperty]
	public UInt16 Value { get; init; }

	public static implicit operator UInt16( ClockMicrosecond value ) => value.Value;

	public static implicit operator ClockMicrosecond( UInt16 value ) => new(value);

	/// <summary>Provide the next <see cref="ClockMicrosecond" />.</summary>
	public ClockMicrosecond Next( out Boolean ticked ) {
		var next = this.Value + 1;

		if ( next > MaximumValue ) {
			ticked = true;

			return Minimum;
		}

		ticked = false;

		return ( UInt16 ) next;
	}

	/// <summary>Provide the previous <see cref="ClockMicrosecond" />.</summary>
	public ClockMicrosecond Previous( out Boolean ticked ) {
		var next = this.Value - 1;

		if ( next < MinimumValue ) {
			ticked = true;

			return Maximum;
		}

		ticked = false;

		return ( UInt16 ) next;
	}

}