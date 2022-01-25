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
// File "Microseconds.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

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
public record Microseconds( BigDecimal Value ) : IComparable<Microseconds>, IQuantityOfTime {

	/// <summary>
	///     1000
	/// </summary>
	public const UInt16 InOneMillisecond = 1000;

	/// <summary>
	///     Ten <see cref="Microseconds" /> s.
	/// </summary>
	public static Microseconds Fifteen { get; } = new( 15 );

	/// <summary>
	///     Five <see cref="Microseconds" /> s.
	/// </summary>
	public static Microseconds Five { get; } = new( 5 );

	/// <summary>
	///     Five Hundred <see cref="Microseconds" /> s.
	/// </summary>
	public static Microseconds FiveHundred { get; } = new( 500 );

	/// <summary>
	///     One <see cref="Microseconds" />.
	/// </summary>
	public static Microseconds One { get; } = new( 1 );

	/// <summary>
	///     One Thousand Nine <see cref="Microseconds" /> (Prime).
	/// </summary>
	public static Microseconds OneThousandNine { get; } = new( 1009 );

	/// <summary>
	///     Sixteen <see cref="Microseconds" />.
	/// </summary>
	public static Microseconds Sixteen { get; } = new( 16 );

	/// <summary>
	///     Ten <see cref="Microseconds" /> s.
	/// </summary>
	public static Microseconds Ten { get; } = new( 10 );

	/// <summary>
	///     Three <see cref="Microseconds" /> s.
	/// </summary>
	public static Microseconds Three { get; } = new( 3 );

	/// <summary>
	///     Three Three Three <see cref="Microseconds" />.
	/// </summary>
	public static Microseconds ThreeHundredThirtyThree { get; } = new( 333 );

	/// <summary>
	///     Two <see cref="Microseconds" /> s.
	/// </summary>
	public static Microseconds Two { get; } = new( 2 );

	/// <summary>
	///     Two Hundred <see cref="Microseconds" />.
	/// </summary>
	public static Microseconds TwoHundred { get; } = new( 200 );

	/// <summary>
	///     Two Hundred Eleven <see cref="Microseconds" /> (Prime).
	/// </summary>
	public static Microseconds TwoHundredEleven { get; } = new( 211 );

	/// <summary>
	///     Two Thousand Three <see cref="Microseconds" /> (Prime).
	/// </summary>
	public static Microseconds TwoThousandThree { get; } = new( 2003 );

	/// <summary>
	///     Zero <see cref="Microseconds" />.
	/// </summary>
	public static Microseconds Zero { get; } = new( 0 );

	public Int32 CompareTo( Microseconds? other ) {
		if ( other == null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this.ToNanoseconds();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneMicrosecond );

	public Seconds ToSeconds() => new( this.ToMilliseconds().Value / Milliseconds.InOneSecond );

	public IQuantityOfTime ToCoarserGranularity() => this.ToMilliseconds();

	TimeSpan IQuantityOfTime.ToTimeSpan() => TimeSpan.FromMilliseconds( ( Double )( this.Value / InOneMillisecond ) );

	public static Microseconds Combine( Microseconds left, Microseconds right ) => Combine( left, right.Value );

	public static Microseconds Combine( Microseconds left, BigDecimal microseconds ) => new( left.Value + microseconds );

	public static Microseconds Combine( Microseconds left, BigInteger microseconds ) => new( left.Value + microseconds );

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Microseconds left, Microseconds right ) => left.Value == right.Value;

	public static implicit operator Milliseconds( Microseconds microseconds ) => microseconds.ToMilliseconds();

	public static implicit operator Nanoseconds( Microseconds microseconds ) => microseconds.ToNanoseconds();

	public static implicit operator TimeSpan( Microseconds microseconds ) => TimeSpan.FromMilliseconds( ( Double )microseconds.Value );

	public static Microseconds operator -( Microseconds milliseconds ) => new( milliseconds.Value * -1 );

	public static Microseconds operator -( Microseconds left, Microseconds right ) => Combine( left, -right );

	public static Microseconds operator -( Microseconds left, BigDecimal microseconds ) => Combine( left, -microseconds );

	public static Microseconds operator +( Microseconds left, Microseconds right ) => Combine( left, right );

	public static Microseconds operator +( Microseconds left, BigDecimal microseconds ) => Combine( left, microseconds );

	public static Microseconds operator +( Microseconds left, BigInteger microseconds ) => Combine( left, microseconds );

	public static Boolean operator <( Microseconds left, Microseconds right ) => left.Value < right.Value;

	public static Boolean operator <( Microseconds left, Milliseconds right ) => ( Milliseconds )left < right;

	public static Boolean operator >( Microseconds left, Microseconds right ) => left.Value > right.Value;

	public static Boolean operator >( Microseconds left, Milliseconds right ) => left.Value > right.Value;

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public Milliseconds ToMilliseconds() => new( this.Value / InOneMillisecond );

	public Nanoseconds ToNanoseconds() => new( this.Value * Nanoseconds.InOneMicrosecond );

	public override String ToString() => $"{this.Value} µs";

	public TimeSpan? ToTimeSpan() => this.ToSeconds();

	public static Boolean operator <=( Microseconds left, Microseconds right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Microseconds left, Microseconds right ) => left.CompareTo( right ) >= 0;
}