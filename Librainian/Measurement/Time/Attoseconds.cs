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
// File "Attoseconds.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
[Immutable]
public record Attoseconds( BigDecimal Value ) : IQuantityOfTime {

	/// <summary>1000</summary>
	/// <see cref="Femtoseconds" />
	public const Int16 InOneFemtosecond = 1000;

	public Attoseconds( Int32 value ) : this( ( BigDecimal )value ) { }

	public Attoseconds( Int64 value ) : this( ( BigDecimal )value ) { }

	public Attoseconds( UInt64 value ) : this( ( BigDecimal )value ) { }

	public Attoseconds( BigInteger value ) : this( ( BigDecimal )value ) { }

	/// <summary>Ten <see cref="Attoseconds" /> s.</summary>
	public static Attoseconds Fifteen { get; } = new( 15M );

	/// <summary>Five <see cref="Attoseconds" /> s.</summary>
	public static Attoseconds Five { get; } = new( 5m );

	/// <summary>Five Hundred <see cref="Attoseconds" /> s.</summary>
	public static Attoseconds FiveHundred { get; } = new( 500m );

	/// <summary>111. 1 Hertz <see cref="Attoseconds" />.</summary>
	public static Attoseconds Hertz111 { get; } = new( 9m );

	/// <summary>One <see cref="Attoseconds" />.</summary>
	/// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
	public static Attoseconds One { get; } = new( 1m );

	/// <summary><see cref="OneHundred" /><see cref="Attoseconds" />.</summary>
	/// <remarks>fastest ever view of molecular motion</remarks>
	public static Attoseconds OneHundred { get; } = new( 100m );

	/// <summary>One Thousand Nine <see cref="Attoseconds" /> (Prime).</summary>
	public static Attoseconds OneThousandNine { get; } = new( 1009m );

	/// <summary>Sixteen <see cref="Attoseconds" />.</summary>
	public static Attoseconds Sixteen { get; } = new( 16m );

	/// <summary><see cref="SixtySeven" /><see cref="Attoseconds" />.</summary>
	/// <remarks>the shortest pulses of laser light yet created</remarks>
	public static Attoseconds SixtySeven { get; } = new( 67m );

	/// <summary>Ten <see cref="Attoseconds" /> s.</summary>
	public static Attoseconds Ten { get; } = new( 10m );

	/// <summary>Three <see cref="Attoseconds" /> s.</summary>
	public static Attoseconds Three { get; } = new( 3m );

	/// <summary>Three Three Three <see cref="Attoseconds" />.</summary>
	public static Attoseconds ThreeHundredThirtyThree { get; } = new( 333m );

	/// <summary><see cref="ThreeHundredTwenty" /><see cref="Attoseconds" />.</summary>
	/// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
	public static Attoseconds ThreeHundredTwenty { get; } = new( 320m );

	/// <summary><see cref="Twelve" /><see cref="Attoseconds" />.</summary>
	/// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
	public static Attoseconds Twelve { get; } = new( 12m );

	/// <summary><see cref="TwentyFour" /><see cref="Attoseconds" />.</summary>
	/// <remarks>the atomic unit of time</remarks>
	public static Attoseconds TwentyFour { get; } = new( 24 );

	/// <summary>Two <see cref="Attoseconds" /> s.</summary>
	public static Attoseconds Two { get; } = new( 2 );

	/// <summary><see cref="TwoHundred" /><see cref="Attoseconds" />.</summary>
	/// <remarks>
	///     (approximately) – half-life of beryllium-8, maximum time available for the triple-alpha process for the
	///     synthesis of carbon and heavier elements in stars
	/// </remarks>
	public static Attoseconds TwoHundred { get; } = new( 200 );

	/// <summary>Two Hundred Eleven <see cref="Attoseconds" /> (Prime).</summary>
	public static Attoseconds TwoHundredEleven { get; } = new( 211 );

	/// <summary>Two Thousand Three <see cref="Attoseconds" /> (Prime).</summary>
	public static Attoseconds TwoThousandThree { get; } = new( 2003 );

	/// <summary>Zero <see cref="Attoseconds" />.</summary>
	public static Attoseconds Zero { get; } = new( 0 );

	public IQuantityOfTime ToFinerGranularity() => this.ToZeptoseconds();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneAttosecond );

	public Seconds ToSeconds() => new( ( BigDecimal )this.ToTimeSpan().TotalSeconds );

	public IQuantityOfTime ToCoarserGranularity() => this.ToFemtoseconds();

	public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds( ( Double )this.ToSeconds().Value );

	public static Attoseconds Combine( Attoseconds left, Attoseconds right ) => new( left.Value + right.Value );

	public static Attoseconds Combine( Attoseconds left, BigDecimal attoseconds ) => new( left.Value + attoseconds );

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	public static Boolean Equals( Attoseconds left, Attoseconds right ) => left.Value == right.Value;

	public static implicit operator Femtoseconds( Attoseconds attoseconds ) => attoseconds.ToFemtoseconds();

	public static implicit operator SpanOfTime( Attoseconds attoseconds ) => new( attoseconds.ToPlanckTimes().Value );

	public static implicit operator Zeptoseconds( Attoseconds attoseconds ) => attoseconds.ToZeptoseconds();

	public static Attoseconds operator -( Attoseconds left, BigDecimal attoseconds ) => Combine( left, -attoseconds );

	public static Attoseconds operator +( Attoseconds left, Attoseconds right ) => Combine( left, right );

	public static Attoseconds operator +( Attoseconds left, BigDecimal attoseconds ) => Combine( left, attoseconds );

	public static Boolean operator <( Attoseconds left, Attoseconds right ) => left.Value < right.Value;

	public static Boolean operator >( Attoseconds left, Attoseconds right ) => left.Value > right.Value;

	/// <summary>Convert to a larger unit.</summary>
	public Femtoseconds ToFemtoseconds() => new( this.Value / InOneFemtosecond );

	public override String ToString() => $"{this.Value}as";

	/// <summary>Convert to a smaller unit.</summary>
	public Zeptoseconds ToZeptoseconds() => new( this.Value * Zeptoseconds.InOneAttosecond );
}