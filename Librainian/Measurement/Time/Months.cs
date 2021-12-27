// Copyright � Protiguous. All Rights Reserved.
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
// File "Months.cs" last touched on 2021-03-07 at 3:04 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record Months( BigDecimal Value ) : IQuantityOfTime, IComparable<Months> {

	/// <summary>
	///     12
	/// </summary>
	public const Byte InOneCommonYear = 12;

	/// <summary>
	///     One <see cref="Months" /> .
	/// </summary>
	public static Months One { get; } = new( 1 );

	/// <summary>
	/// </summary>
	public static Months Ten { get; } = new( 10 );

	/// <summary>
	/// </summary>
	public static Months Thousand { get; } = new( 1000 );

	/// <summary>
	///     Zero <see cref="Months" />
	/// </summary>
	public static Months Zero { get; } = new( 0 );

	public static Months Twelve { get; } = new( InOneCommonYear );

	public Int32 CompareTo( Months? other ) {
		if ( other is null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	/// <summary>
	///     Return this value in <see cref="Weeks" />.
	/// </summary>
	public IQuantityOfTime ToFinerGranularity() => this.ToWeeks();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneMonth );

	public Seconds ToSeconds() => new( this.Value * Seconds.InOneMonth );

	/// <summary>
	///     Return this value in <see cref="Years" />.
	/// </summary>
	public IQuantityOfTime ToCoarserGranularity() => this.ToYears();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public static Months Combine( Months left, Months right ) => Combine( left, right.Value );

	public static Months Combine( Months left, BigDecimal months ) => new( left.Value + months );

	public static Months Combine( Months left, BigInteger months ) => new( left.Value + months );

	public static implicit operator SpanOfTime( Months months ) => new( months: months );

	public static implicit operator Weeks( Months months ) => months.ToWeeks();

	public static Months operator -( Months days ) => new( days.Value * -1 );

	public static Months operator -( Months left, Months right ) => Combine( left, -right );

	public static Months operator -( Months left, BigDecimal months ) => Combine( left, -months );

	public static Months operator +( Months left, Months right ) => Combine( left, right );

	public static Months operator +( Months left, BigDecimal months ) => Combine( left, months );

	public static Boolean operator <( Months left, Months right ) => left.Value < right.Value;

	public static Boolean operator >( Months left, Months right ) => left.Value > right.Value;

	public Years ToYears() => new( this.Value / InOneCommonYear );

	public override String ToString() => this.Value == 1 ? $"{this.Value} month" : $"{this.Value} months";

	public static implicit operator Years( Months months ) => months.ToYears();

	public Weeks ToWeeks() => new( this.Value * Weeks.InOneMonth );
}