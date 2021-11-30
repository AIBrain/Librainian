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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Days.cs" last touched on 2021-10-13 at 4:28 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Newtonsoft.Json;

[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
public record Days( BigDecimal Value ) : IQuantityOfTime, IComparable<Days> {

	/// <summary>
	///     365.2422 (days)
	/// </summary>
	public const Decimal InOneCommonYear = 365.2422m;

	/// <summary>
	///     7
	/// </summary>
	public const Byte InOneWeek = 7;

	/// <summary>
	///     One <see cref="Days" /> .
	/// </summary>
	public static Days One { get; } = new(1);

	/// <summary>
	///     Seven <see cref="Days" /> .
	/// </summary>
	public static Days Seven { get; } = new(7);

	/// <summary>
	///     Ten <see cref="Days" /> .
	/// </summary>
	public static Days Ten { get; } = new(10);

	/// <summary>
	/// </summary>
	public static Days Thousand { get; } = new(1000);

	/// <summary>
	///     Zero <see cref="Days" />
	/// </summary>
	public static Days Zero { get; } = new(0);

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
	/// <exception cref="NullException"><paramref name="other" /> is not the same type as this instance.</exception>
	public Int32 CompareTo( Days? other ) {
		if ( other is null ) {
			throw new NullException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this.ToHours();

	public PlanckTimes ToPlanckTimes() => new(this.Value * PlanckTimes.InOneDay);

	public Seconds ToSeconds() => new(this.Value * Seconds.InOneDay);

	public IQuantityOfTime ToCoarserGranularity() => this.ToWeeks();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

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
	public Int32 CompareTo( IQuantityOfTime? other ) => other is null ? SortOrder.NullsDefault : this.ToPlanckTimes().CompareTo( other.ToPlanckTimes() );

	public static Days Combine( Days left, Days right ) => Combine( left, right.Value );

	public static Days Combine( Days left, BigDecimal days ) => new(left.Value + days);

	public static Days Combine( Days left, BigInteger days ) => new(left.Value + days);

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	public static Boolean Equals( Days left, Days right ) => left.Value == right.Value;

	/// <summary>
	///     Implicitly convert the number of <paramref name="days" /> to <see cref="Hours" />.
	/// </summary>
	/// <param name="days"></param>
	public static implicit operator Hours( Days days ) => days.ToHours();

	public static implicit operator SpanOfTime( Days days ) => new(days);

	public static implicit operator TimeSpan( Days days ) => TimeSpan.FromDays( ( Double )days.Value );

	/// <summary>
	///     Implicitly convert the number of <paramref name="days" /> to <see cref="Weeks" />.
	/// </summary>
	/// <param name="days"></param>
	public static implicit operator Weeks( Days days ) => days.ToWeeks();

	public static Days operator -( Days days ) => new(days.Value * -1);

	public static Days operator -( Days left, Days right ) => Combine( left, -right );

	public static Days operator -( Days left, BigDecimal days ) => Combine( left, -days );

	public static Days operator +( Days left, Days right ) => Combine( left, right );

	public static Days operator +( Days left, BigDecimal days ) => Combine( left, days );

	public static Days operator +( Days left, BigInteger days ) => Combine( left, days );

	public static Boolean operator <( Days left, Days right ) => left.Value < right.Value;

	public static Boolean operator <( Days left, Hours right ) => left < ( Days )right;

	public static Boolean operator >( Days left, Hours right ) => left > ( Days )right;

	public static Boolean operator >( Days left, Days right ) => left.Value > right.Value;

	public Hours ToHours() => new(this.Value * Hours.InOneDay);

	public override String ToString() => $"{this.Value} days";

	public Weeks ToWeeks() => new(this.Value / InOneWeek);

}