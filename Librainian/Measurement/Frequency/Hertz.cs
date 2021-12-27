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
// File "Hertz.cs" last touched on 2021-09-04 at 3:26 PM by Protiguous.

namespace Librainian.Measurement.Frequency;

using System;
using System.Diagnostics;
using Maths;
using Newtonsoft.Json;
using Time;

/// <summary>http: //wikipedia.org/wiki/Frequency</summary>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
public record Hertz {

	public Hertz( Decimal frequency ) {
		if ( frequency <= 0m.Epsilon() ) {
			this.Value = 0m.Epsilon();
		}
		else {
			this.Value = frequency >= Decimal.MaxValue ? Decimal.MaxValue : frequency;
		}
	}

	public Hertz( UInt64 frequency ) : this( ( Decimal )frequency ) { }

	public Hertz( Double frequency ) : this( ( Decimal )frequency ) { }

	/// <summary>Fifteen <see cref="Hertz" /> s.</summary>
	public static Hertz Fifteen { get; } = new(15);

	/// <summary>59. 9 <see cref="Hertz" />.</summary>
	public static Hertz FiftyNinePointNine { get; } = new(59.9);

	/// <summary>Five <see cref="Hertz" /> s.</summary>
	public static Hertz Five { get; } = new(5);

	/// <summary>Five Hundred <see cref="Hertz" /> s.</summary>
	public static Hertz FiveHundred { get; } = new(500);

	/// <summary>111. 1 Hertz <see cref="Hertz" />.</summary>
	public static Hertz Hertz111 { get; } = new(111.1);

	/// <summary>One <see cref="Hertz" />.</summary>
	public static Hertz One { get; } = new(1);

	/// <summary>120 <see cref="Hertz" />.</summary>
	public static Hertz OneHundredTwenty { get; } = new(120);

	/// <summary>One Thousand Nine <see cref="Hertz" /> (Prime).</summary>
	public static Hertz OneThousandNine { get; } = new(1009);

	/// <summary>Sixty <see cref="Hertz" />.</summary>
	public static Hertz Sixty { get; } = new(60);

	/// <summary>Ten <see cref="Hertz" /> s.</summary>
	public static Hertz Ten { get; } = new(10);

	/// <summary>Thirty <see cref="Hertz" /> s.</summary>
	public static Hertz Thirty { get; } = new(30);

	/// <summary>Three <see cref="Hertz" /> s.</summary>
	public static Hertz Three { get; } = new(3);

	/// <summary>Three Three Three <see cref="Hertz" />.</summary>
	public static Hertz ThreeHundredThirtyThree { get; } = new(333);

	/// <summary>Two <see cref="Hertz" /> s.</summary>
	public static Hertz Two { get; } = new(2);

	/// <summary>Two Hundred <see cref="Hertz" />.</summary>
	public static Hertz TwoHundred { get; } = new(200);

	/// <summary>211 <see cref="Hertz" /> (Prime).</summary>
	public static Hertz TwoHundredEleven { get; } = new(211);

	/// <summary>Two.Five <see cref="Hertz" /> s.</summary>
	public static Hertz TwoPointFive { get; } = new(2.5);

	/// <summary>Two Thousand Three <see cref="Hertz" /> (Prime).</summary>
	public static Hertz TwoThousandThree { get; } = new(2003);

	//faster WPM than a female (~240wpm)
	/// <summary>One <see cref="Hertz" />.</summary>
	public static Hertz Zero { get; } = new(0);

	[JsonProperty]
	public Decimal Value { get; init; }

	public static implicit operator SpanOfTime( Hertz hertz ) => new Seconds( 1 / hertz.Value );

	public static implicit operator TimeSpan( Hertz hertz ) => TimeSpan.FromSeconds( ( Double )( 1 / hertz.Value ) );

	public static Boolean operator <( Hertz left, Hertz right ) => left.Value.CompareTo( right.Value ) < 0;

	public static Boolean operator >( Hertz left, Hertz right ) => left.Value.CompareTo( right.Value ) > 0;

	public override String ToString() => $"{this.Value} hertz ({( ( TimeSpan )this ).Simpler()})";

}