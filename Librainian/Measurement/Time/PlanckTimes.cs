// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "PlanckTimes.cs" last formatted on 2021-01-31 at 4:43 PM.

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Exceptions;
using ExtendedNumerics;
using Newtonsoft.Json;

/// <summary>
///     <para>In physics, the Planck time (tP) is the unit of time in the system of natural units known as Planck units.</para>
///     <para>It is the time required for light to travel, in a vacuum, a distance of 1 Planck length.</para>
///     <para>The Planck time is defined as:</para>
///     <para>t_P \equiv \sqrt{\frac{\hbar G}{c^5}} ≈ 5.39106(32) × 10−44 s</para>
///     <para>
///         where: \hbar = h / 2 \pi is the reduced Planck constant (sometimes h is used instead of \hbar in the
///         definition[1]) G = gravitational constant c = speed of light in a
///         vacuum s is the SI unit of time, the second. The two digits between parentheses denote the standard error of
///         the estimated value.
///     </para>
/// </summary>
/// <see cref="http://wikipedia.org/wiki/Planck_time" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public record PlanckTimes( BigInteger Value ) : IQuantityOfTime, IComparable<IQuantityOfTime> {
	public static BigDecimal InOneAttosecond => InOneFemtosecond / Attoseconds.InOneFemtosecond;

	public static BigDecimal InOneDay => InOneSecond * ( BigInteger )Seconds.InOneDay;

	public static BigDecimal InOneFemtosecond => InOnePicosecond / Femtoseconds.InOnePicosecond;

	public static BigDecimal InOneHour => InOneSecond * Seconds.InOneHour;

	public static BigDecimal InOneMicrosecond => InOneMillisecond / Microseconds.InOneMillisecond;

	public static BigDecimal InOneMillisecond => InOneSecond / Milliseconds.InOneSecond;

	public static BigDecimal InOneMinute => InOneSecond * Seconds.InOneMinute;

	public static BigDecimal InOneMonth => InOneSecond * ( BigInteger )Seconds.InOneMonth;

	public static BigDecimal InOneNanosecond => InOneMicrosecond / Nanoseconds.InOneMicrosecond;

	public static BigDecimal InOnePicosecond => InOneNanosecond / Picoseconds.InOneNanosecond;

	/// <summary>
	///     <para>18550948324478400000 (where did I get this number??? It's so.. specific?)</para>
	/// </summary>
	public static BigDecimal InOneSecond => new( 18550948324478E30 );

	public static BigDecimal InOneWeek => InOneSecond * Seconds.InOneWeek;

	public static BigDecimal InOneYear => InOneSecond * Seconds.InOneCommonYear;

	public static BigDecimal InOneYoctosecond => InOneZeptosecond / Yoctoseconds.InOneZeptosecond;

	public static BigDecimal InOneZeptosecond => InOneAttosecond / Zeptoseconds.InOneAttosecond;

	public PlanckTimes( Decimal value ) : this( ( BigInteger )value ) { }
	public PlanckTimes( BigDecimal value ) : this( ( BigInteger )value ) { }

	public PlanckTimes( UInt64 value ) : this( ( BigInteger )value ) { }
	public PlanckTimes( Int64 value ) : this( ( BigInteger )value ) { }

	/// <summary>
	///     One <see cref="PlanckTimes" />.
	/// </summary>
	public static PlanckTimes One { get; } = new( 1L );

	/// <summary>
	///     Two <see cref="PlanckTimes" />.
	/// </summary>
	public static PlanckTimes Two { get; } = new( 2L );

	/// <summary>
	///     Zero <see cref="PlanckTimes" />.
	/// </summary>
	public static PlanckTimes Zero { get; } = new( 0L );

	/// <summary>
	///     Compares the current instance with another object of the same type and returns an integer that indicates whether
	///     the current instance precedes, follows, or occurs
	///     in the same position in the sort order as the other object.
	/// </summary>
	/// <param name="other">An object to compare with this instance.</param>
	/// <returns>
	///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
	///     Meaning Less than zero This instance precedes
	///     <paramref name="other" /> in the sort order. Zero This instance occurs in the same position in the sort order as
	///     <paramref name="other" />. Greater than zero This
	///     instance follows <paramref name="other" /> in the sort order.
	/// </returns>
	public Int32 CompareTo( IQuantityOfTime? other ) {
		if ( other is null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public PlanckTimes ToPlanckTimes() => this;

	public Seconds ToSeconds() => new( this.Value * InOneSecond );
	public IQuantityOfTime ToCoarserGranularity() => this.ToYoctoseconds();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public static PlanckTimes Combine( PlanckTimes left, PlanckTimes right ) => new( left.Value + right.Value );

	public static PlanckTimes Combine( PlanckTimes left, BigInteger planckTimes ) => new( left.Value + planckTimes );

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	public static Boolean Equals( PlanckTimes left, PlanckTimes right ) => left.Value.Equals( right.Value );

	public static implicit operator SpanOfTime( PlanckTimes planckTimes ) => new( planckTimes );

	/// <summary>
	///     Implicitly convert the number of <paramref name="planckTimes" /> to <see cref="Yoctoseconds" />.
	/// </summary>
	/// <param name="planckTimes"></param>
	public static implicit operator Yoctoseconds( PlanckTimes planckTimes ) => ToYoctoseconds( planckTimes );

	public static PlanckTimes operator -( PlanckTimes left, BigInteger planckTimes ) => Combine( left, -planckTimes );

	public static PlanckTimes operator +( PlanckTimes left, PlanckTimes right ) => Combine( left, right );

	public static PlanckTimes operator +( PlanckTimes left, BigInteger planckTimes ) => Combine( left, planckTimes );

	public static Boolean operator <( PlanckTimes left, PlanckTimes right ) => left.Value < right.Value;

	public static Boolean operator <=( PlanckTimes left, PlanckTimes right ) => left.Value <= right.Value;

	public static Boolean operator >( PlanckTimes left, PlanckTimes right ) => left.Value > right.Value;

	public static Boolean operator >=( PlanckTimes left, PlanckTimes right ) => left.Value >= right.Value;

	/// <summary>
	///     <para>Convert to a larger unit.</para>
	/// </summary>
	/// <param name="planckTimes"></param>
	public static Yoctoseconds ToYoctoseconds( PlanckTimes planckTimes ) => new( planckTimes.Value / InOneYoctosecond );

	public Yoctoseconds ToYoctoseconds() => new( this.Value / InOneYoctosecond );

	public Int32 CompareTo( PlanckTimes other ) => this.Value.CompareTo( other.Value );

	public override String ToString() => $"{this.Value} tP";
}