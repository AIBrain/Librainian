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
// File "Nanoseconds.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
[Immutable]
public record Nanoseconds( BigDecimal Value ) : IQuantityOfTime, IComparable<Nanoseconds> {

	/// <summary>1000</summary>
	public const UInt16 InOneMicrosecond = 1000;

	/// <summary>Ten <see cref="Nanoseconds" /> s.</summary>
	public static Nanoseconds Fifteen { get; } = new( 15 );

	/// <summary>Five <see cref="Nanoseconds" /> s.</summary>
	public static Nanoseconds Five { get; } = new( 5 );

	/// <summary>Five Hundred <see cref="Nanoseconds" /> s.</summary>
	public static Nanoseconds FiveHundred { get; } = new( 500 );

	/// <summary>One <see cref="Nanoseconds" />.</summary>
	public static Nanoseconds One { get; } = new( 1 );

	/// <summary>One Thousand Nine <see cref="Nanoseconds" /> (Prime).</summary>
	public static Nanoseconds OneThousandNine { get; } = new( 1009 );

	/// <summary>Sixteen <see cref="Nanoseconds" />.</summary>
	public static Nanoseconds Sixteen { get; } = new( 16 );

	/// <summary>Ten <see cref="Nanoseconds" /> s.</summary>
	public static Nanoseconds Ten { get; } = new( 10 );

	/// <summary>Three <see cref="Nanoseconds" /> s.</summary>
	public static Nanoseconds Three { get; } = new( 3 );

	/// <summary>Three Three Three <see cref="Nanoseconds" />.</summary>
	public static Nanoseconds ThreeHundredThirtyThree { get; } = new( 333 );

	/// <summary>Two <see cref="Nanoseconds" /> s.</summary>
	public static Nanoseconds Two { get; } = new( 2 );

	/// <summary>Two Hundred <see cref="Nanoseconds" />.</summary>
	public static Nanoseconds TwoHundred { get; } = new( 200 );

	/// <summary>Two Hundred Eleven <see cref="Nanoseconds" /> (Prime).</summary>
	public static Nanoseconds TwoHundredEleven { get; } = new( 211 );

	/// <summary>Two Thousand Three <see cref="Nanoseconds" /> (Prime).</summary>
	public static Nanoseconds TwoThousandThree { get; } = new( 2003 );

	/// <summary>Zero <see cref="Nanoseconds" />.</summary>
	public static Nanoseconds Zero { get; } = new( 0 );

	public Int32 CompareTo( Nanoseconds other ) {
		if ( other is null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this.ToPicoseconds();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneNanosecond );

	public Seconds ToSeconds() => this.ToMicroseconds().ToSeconds();

	public IQuantityOfTime ToCoarserGranularity() => this.ToMicroseconds();

	TimeSpan IQuantityOfTime.ToTimeSpan() => this.ToSeconds();

	public static Nanoseconds Combine( Nanoseconds left, Nanoseconds right ) => Combine( left, right.Value );

	public static Nanoseconds Combine( Nanoseconds left, BigDecimal nanoseconds ) => new( left.Value + nanoseconds );

	public static Nanoseconds Combine( Nanoseconds left, BigInteger nanoseconds ) => new( left.Value + nanoseconds );

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Nanoseconds left, Nanoseconds right ) => left.Value == right.Value;

	public static implicit operator Microseconds( Nanoseconds nanoseconds ) => nanoseconds.ToMicroseconds();

	public static implicit operator Picoseconds( Nanoseconds nanoseconds ) => nanoseconds.ToPicoseconds();

	public static implicit operator SpanOfTime( Nanoseconds nanoseconds ) => new( nanoseconds: nanoseconds );

	public static Nanoseconds operator -( Nanoseconds nanoseconds ) => new( nanoseconds.Value * -1 );

	public static Nanoseconds operator -( Nanoseconds left, Nanoseconds right ) => Combine( left, -right );

	public static Nanoseconds operator -( Nanoseconds left, BigDecimal nanoseconds ) => Combine( left, -nanoseconds );

	public static Nanoseconds operator +( Nanoseconds left, Nanoseconds right ) => Combine( left, right );

	public static Nanoseconds operator +( Nanoseconds left, BigDecimal nanoseconds ) => Combine( left, nanoseconds );

	public static Nanoseconds operator +( Nanoseconds left, BigInteger nanoseconds ) => Combine( left, nanoseconds );

	public static Boolean operator <( Nanoseconds left, Nanoseconds right ) => left.Value < right.Value;

	public static Boolean operator <( Nanoseconds left, Microseconds right ) => ( Microseconds )left < right;

	public static Boolean operator >( Nanoseconds left, Nanoseconds right ) => left.Value > right.Value;

	public static Boolean operator >( Nanoseconds left, Microseconds right ) => ( Microseconds )left > right;

	public Microseconds ToMicroseconds() => new( this.Value / InOneMicrosecond );

	public Picoseconds ToPicoseconds() => new( this.Value * Picoseconds.InOneNanosecond );

	public override String ToString() => $"{this.Value} ns";

	public TimeSpan? ToTimeSpan() => this.ToSeconds();

	public static Boolean operator <=( Nanoseconds left, Nanoseconds right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Nanoseconds left, Nanoseconds right ) => left.CompareTo( right ) >= 0;
}