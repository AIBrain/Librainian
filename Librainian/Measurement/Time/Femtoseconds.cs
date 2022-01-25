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
// File "Femtoseconds.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

/// <summary>
/// </summary>
/// <see cref="http://wikipedia.org/wiki/Femtosecond" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
[Immutable]
public record Femtoseconds( BigDecimal Value ) : IQuantityOfTime, IComparable<Femtoseconds> {

	/// <summary>1000</summary>
	public const UInt16 InOnePicosecond = 1000;

	public Femtoseconds( Decimal value ) : this( ( BigDecimal )value ) { }

	public Femtoseconds( Int32 value ) : this( ( BigDecimal )value ) { }

	public Femtoseconds( Int64 value ) : this( ( Decimal )value ) { }

	public Femtoseconds( BigInteger value ) : this( ( BigDecimal )value ) { }

	/// <summary>Ten <see cref="Femtoseconds" /> s.</summary>
	public static Femtoseconds Fifteen { get; } = new( 15 );

	/// <summary>Five <see cref="Femtoseconds" /> s.</summary>
	public static Femtoseconds Five { get; } = new( 5 );

	/// <summary>Five Hundred <see cref="Femtoseconds" /> s.</summary>
	public static Femtoseconds FiveHundred { get; } = new( 500 );

	/// <summary>One <see cref="Femtoseconds" />.</summary>
	public static Femtoseconds One { get; } = new( 1 );

	/// <summary>One Thousand Nine <see cref="Femtoseconds" /> (Prime).</summary>
	public static Femtoseconds OneThousandNine { get; } = new( 1009 );

	/// <summary>Sixteen <see cref="Femtoseconds" />.</summary>
	public static Femtoseconds Sixteen { get; } = new( 16 );

	/// <summary>Ten <see cref="Femtoseconds" /> s.</summary>
	public static Femtoseconds Ten { get; } = new( 10 );

	/// <summary>Three <see cref="Femtoseconds" /> s.</summary>
	public static Femtoseconds Three { get; } = new( 3 );

	/// <summary>Three Three Three <see cref="Femtoseconds" />.</summary>
	public static Femtoseconds ThreeHundredThirtyThree { get; } = new( 333 );

	/// <summary>Two <see cref="Femtoseconds" /> s.</summary>
	public static Femtoseconds Two { get; } = new( 2 );

	/// <summary>Two Hundred <see cref="Femtoseconds" />.</summary>
	public static Femtoseconds TwoHundred { get; } = new( 200 );

	/// <summary>Two Hundred Eleven <see cref="Femtoseconds" /> (Prime).</summary>
	public static Femtoseconds TwoHundredEleven { get; } = new( 211 );

	/// <summary>Two Thousand Three <see cref="Femtoseconds" /> (Prime).</summary>
	public static Femtoseconds TwoThousandThree { get; } = new( 2003 );

	/// <summary>Zero <see cref="Femtoseconds" />.</summary>
	public static Femtoseconds Zero { get; } = new( 0 );

	public Int32 CompareTo( Femtoseconds? other ) {
		if ( other == null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this.ToAttoseconds();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneFemtosecond );

	public Seconds ToSeconds() => this.ToPicoseconds().ToSeconds();

	public IQuantityOfTime ToCoarserGranularity() => this.ToPicoseconds();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public static Femtoseconds Combine( Femtoseconds left, Femtoseconds right ) => Combine( left, right.Value );

	public static Femtoseconds Combine( Femtoseconds left, BigDecimal femtoseconds ) => new( left.Value + femtoseconds );

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Femtoseconds left, Femtoseconds right ) => left.Value == right.Value;

	public static implicit operator Attoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToAttoseconds();

	public static implicit operator Picoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToPicoseconds();

	public static implicit operator SpanOfTime( Femtoseconds femtoseconds ) => new( femtoseconds: femtoseconds );

	public static Femtoseconds operator -( Femtoseconds femtoseconds ) => new( femtoseconds.Value * -1 );

	public static Femtoseconds operator -( Femtoseconds left, Femtoseconds right ) => Combine( left, -right );

	public static Femtoseconds operator -( Femtoseconds left, BigDecimal femtoseconds ) => Combine( left, -femtoseconds );

	public static Femtoseconds operator +( Femtoseconds left, Femtoseconds right ) => Combine( left, right );

	public static Femtoseconds operator +( Femtoseconds left, BigDecimal femtoseconds ) => Combine( left, femtoseconds );

	public static Boolean operator <( Femtoseconds left, Femtoseconds right ) => left.Value < right.Value;

	public static Boolean operator >( Femtoseconds left, Femtoseconds right ) => left.Value > right.Value;

	/// <summary>Convert to a smaller unit.</summary>
	public Attoseconds ToAttoseconds() => new( this.Value * Attoseconds.InOneFemtosecond );

	/// <summary>Convert to a larger unit.</summary>
	public Picoseconds ToPicoseconds() => new( this.Value / InOnePicosecond );

	public override String ToString() => $"{this.Value} fs";

	public static Boolean operator <=( Femtoseconds left, Femtoseconds right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Femtoseconds left, Femtoseconds right ) => left.CompareTo( right ) >= 0;
}