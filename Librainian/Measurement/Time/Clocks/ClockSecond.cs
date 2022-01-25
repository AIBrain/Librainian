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
// File "ClockSecond.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Time.Clocks;

using System;
using System.Linq;
using Extensions;
using Newtonsoft.Json;

/// <summary>A simple struct for a <see cref="ClockSecond" />.</summary>
[JsonObject]
[Immutable]
public record ClockSecond : IClockPart {
	public const Byte MaximumValue = 59;

	public const Byte MinimumValue = 0;

	public static readonly Byte[] ValidSeconds = Enumerable.Range( MinimumValue, MaximumValue ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

	public ClockSecond( Byte value ) {
		if ( value > MaximumValue ) {
			throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
		}

		this.Value = value;
	}

	public static ClockSecond Maximum { get; } = new( MaximumValue );

	public static ClockSecond Minimum { get; } = new( MinimumValue );

	[JsonProperty]
	public Byte Value { get; init; }

	public static implicit operator Byte( ClockSecond value ) => value.Value;

	public static implicit operator ClockSecond( Byte value ) => new( value );

	/// <summary>Provide the next second.</summary>
	public ClockSecond Next( out Boolean tocked ) {
		var next = this.Value + 1;

		if ( next > MaximumValue ) {
			tocked = true;

			return Minimum;
		}

		tocked = false;

		return ( ClockSecond )next;
	}

	/// <summary>Provide the previous second.</summary>
	public ClockSecond Previous( out Boolean tocked ) {
		var next = this.Value - 1;

		if ( next < MinimumValue ) {
			tocked = true;

			return Maximum;
		}

		tocked = false;

		return ( ClockSecond )next;
	}
}