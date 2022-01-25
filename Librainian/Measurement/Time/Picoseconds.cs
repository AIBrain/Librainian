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
// File "Picoseconds.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using Exceptions;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
[Immutable]
public record Picoseconds( BigDecimal Value ) : IQuantityOfTime, IComparable<Picoseconds> {

	/// <summary>
	///     1000
	/// </summary>
	public const UInt16 InOneNanosecond = 1000;

	/// <summary>
	///     Ten <see cref="Picoseconds" /> s.
	/// </summary>
	public static Picoseconds Fifteen { get; } = new( 15 );

	/// <summary>
	///     Five <see cref="Picoseconds" /> s.
	/// </summary>
	public static Picoseconds Five { get; } = new( 5 );

	/// <summary>
	///     Five Hundred <see cref="Picoseconds" /> s.
	/// </summary>
	public static Picoseconds FiveHundred { get; } = new( 500 );

	/// <summary>
	///     One <see cref="Picoseconds" />.
	/// </summary>
	public static Picoseconds One { get; } = new( 1 );

	/// <summary>
	///     One Thousand Nine <see cref="Picoseconds" /> (Prime).
	/// </summary>
	public static Picoseconds OneThousandNine { get; } = new( 1009 );

	/// <summary>
	///     Sixteen <see cref="Picoseconds" />.
	/// </summary>
	public static Picoseconds Sixteen { get; } = new( 16 );

	/// <summary>
	///     Ten <see cref="Picoseconds" /> s.
	/// </summary>
	public static Picoseconds Ten { get; } = new( 10 );

	/// <summary>
	///     Three <see cref="Picoseconds" /> s.
	/// </summary>
	public static Picoseconds Three { get; } = new( 3 );

	/// <summary>
	///     Three Three Three <see cref="Picoseconds" />.
	/// </summary>
	public static Picoseconds ThreeHundredThirtyThree { get; } = new( 333 );

	/// <summary>
	///     Two <see cref="Picoseconds" /> s.
	/// </summary>
	public static Picoseconds Two { get; } = new( 2 );

	/// <summary>
	///     Two Hundred <see cref="Picoseconds" />.
	/// </summary>
	public static Picoseconds TwoHundred { get; } = new( 200 );

	/// <summary>
	///     Two Hundred Eleven <see cref="Picoseconds" /> (Prime).
	/// </summary>
	public static Picoseconds TwoHundredEleven { get; } = new( 211 );

	/// <summary>
	///     Two Thousand Three <see cref="Picoseconds" /> (Prime).
	/// </summary>
	public static Picoseconds TwoThousandThree { get; } = new( 2003 );

	/// <summary>
	///     Zero <see cref="Picoseconds" />.
	/// </summary>
	public static Picoseconds Zero { get; } = new( 0 );

	public Int32 CompareTo( Picoseconds? other ) {
		if ( other == null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this.ToFemtoseconds();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOnePicosecond );

	public Seconds ToSeconds() => this.ToNanoseconds().ToSeconds();

	public IQuantityOfTime ToCoarserGranularity() => this.ToNanoseconds();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public static Picoseconds Combine( Picoseconds left, Picoseconds right ) => Combine( left, right.Value );

	public static Picoseconds Combine( Picoseconds left, BigDecimal picoseconds ) => new( left.Value + picoseconds );

	public static implicit operator Femtoseconds( Picoseconds picoseconds ) => picoseconds.ToFemtoseconds();

	public static implicit operator Nanoseconds( Picoseconds picoseconds ) => picoseconds.ToNanoseconds();

	public static implicit operator SpanOfTime( Picoseconds picoseconds ) => new( picoseconds: picoseconds );

	public static Picoseconds operator -( Picoseconds left, Picoseconds right ) => Combine( left, -right.Value );

	public static Picoseconds operator -( Picoseconds left, BigDecimal nanoseconds ) => Combine( left, -nanoseconds );

	public static Picoseconds operator +( Picoseconds left, Picoseconds right ) => Combine( left, right );

	public static Picoseconds operator +( Picoseconds left, BigDecimal nanoseconds ) => Combine( left, nanoseconds );

	public static Boolean operator <( Picoseconds left, Picoseconds right ) => left.Value < right.Value;

	public static Boolean operator >( Picoseconds left, Picoseconds right ) => left.Value > right.Value;

	/// <summary>
	///     Convert to a smaller unit.
	/// </summary>
	public Femtoseconds ToFemtoseconds() => new( this.Value * Femtoseconds.InOnePicosecond );

	/// <summary>
	///     Convert to a greater unit.
	/// </summary>
	public Nanoseconds ToNanoseconds() => new( this.Value / InOneNanosecond );

	public override String ToString() => $"{this.Value} ps";

	public static Boolean operator <=( Picoseconds left, Picoseconds right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Picoseconds left, Picoseconds right ) => left.CompareTo( right ) >= 0;
}