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
// File "Yoctoseconds.cs" last touched on 2021-12-28 at 2:12 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

/// <summary></summary>
/// <see cref="http://wikipedia.org/wiki/Yoctosecond" />
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record Yoctoseconds( BigDecimal Value ) : IQuantityOfTime, IComparable<Yoctoseconds> {

	/// <summary>1000</summary>
	public const UInt16 InOneZeptosecond = 1000;

	public Yoctoseconds( Decimal value ) : this( ( BigDecimal ) value ) { }

	public Yoctoseconds( Int32 value ) : this( ( BigDecimal ) value ) { }

	public Yoctoseconds( Int64 value ) : this( ( BigInteger ) value ) { }

	public Yoctoseconds( BigInteger value ) : this( new BigDecimal( value ) ) { }

	/// <summary><see cref="Five" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Five { get; } = new(5);

	/// <summary><see cref="One" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds One { get; } = new(1);

	/// <summary><see cref="Seven" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Seven { get; } = new(7);

	/// <summary><see cref="Ten" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Ten { get; } = new(10);

	/// <summary><see cref="Thirteen" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Thirteen { get; } = new(13);

	/// <summary><see cref="Thirty" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Thirty { get; } = new(30);

	/// <summary><see cref="Three" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Three { get; } = new(3);

	/// <summary><see cref="Two" /><see cref="Yoctoseconds" />.</summary>
	public static Yoctoseconds Two { get; } = new(2);

	/// <summary></summary>
	public static Yoctoseconds Zero { get; } = new(0);

	public static BigDecimal InOneSecond { get; } = BigDecimal.Parse( "10E24" );

	public Int32 CompareTo( Yoctoseconds? other ) {
		if ( other is null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public IQuantityOfTime ToFinerGranularity() => this.ToPlanckTimes();

	public PlanckTimes ToPlanckTimes() => new(  this.Value * PlanckTimes.InOneYoctosecond );

	public Seconds ToSeconds() => new(this.Value * InOneSecond);

	public IQuantityOfTime ToCoarserGranularity() => this.ToZeptoseconds();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public static Yoctoseconds Combine( Yoctoseconds left, Yoctoseconds right ) => Combine( left, right.Value );

	public static Yoctoseconds Combine( Yoctoseconds left, BigDecimal yoctoseconds ) => new(left.Value + yoctoseconds);

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Yoctoseconds left, Yoctoseconds right ) => left.Value.Equals( right.Value );

	/// <summary>Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="PlanckTimes" />.</summary>
	/// <param name="yoctoseconds"></param>
	public static implicit operator PlanckTimes( Yoctoseconds yoctoseconds ) => ToPlanckTimes( yoctoseconds );

	public static implicit operator SpanOfTime( Yoctoseconds yoctoseconds ) => new(yoctoseconds);

	/// <summary>Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="Zeptoseconds" />.</summary>
	/// <param name="yoctoseconds"></param>
	public static implicit operator Zeptoseconds( Yoctoseconds yoctoseconds ) => yoctoseconds.ToZeptoseconds();

	public static Yoctoseconds operator -( Yoctoseconds yoctoseconds ) => new(yoctoseconds.Value * -1);

	public static Yoctoseconds operator -( Yoctoseconds left, Yoctoseconds right ) => Combine( left, -right );

	public static Yoctoseconds operator -( Yoctoseconds left, Decimal seconds ) => Combine( left, -seconds );

	public static Yoctoseconds operator +( Yoctoseconds left, Yoctoseconds right ) => Combine( left, right );

	public static Yoctoseconds operator +( Yoctoseconds left, Decimal yoctoseconds ) => Combine( left, yoctoseconds );

	public static Boolean operator <( Yoctoseconds left, Yoctoseconds right ) => left.Value < right.Value;

	public static Boolean operator >( Yoctoseconds left, Yoctoseconds right ) => left.Value > right.Value;

	public static PlanckTimes ToPlanckTimes( Yoctoseconds yoctoseconds ) => new(( BigInteger ) ( yoctoseconds.Value * PlanckTimes.InOneYoctosecond ));

	public override String ToString() => $"{this.Value} ys";

	public Zeptoseconds ToZeptoseconds() => new(( BigInteger ) ( this.Value / InOneZeptosecond ));

	public static Boolean operator <=( Yoctoseconds left, Yoctoseconds right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Yoctoseconds left, Yoctoseconds right ) => left.CompareTo( right ) >= 0;

}