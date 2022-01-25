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
// File "UniversalDateTime.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Extensions;
using Newtonsoft.Json;
using Utilities;

/// <summary>
///     <para>Absolute universal date and time.</para>
///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
/// </summary>
/// <see cref="http://wikipedia.org/wiki/Lol" />
[Immutable]
[JsonObject]
[DebuggerDisplay( "ToString()" )]
[NeedsTesting]
public record UniversalDateTime : IComparable<UniversalDateTime> {
	public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
		this.Value = planckTimesSinceBigBang;
		var span = new SpanOfTime( this.Value );

		this.Date = new Date( span.Years, span.Months, span.Days );

		this.Hours = span.Hours;
		this.Minutes = span.Minutes;
		this.Seconds = span.Seconds;
		this.Milliseconds = span.Milliseconds;
		this.Microseconds = span.Microseconds;
	}

	public UniversalDateTime( DateTime dateTime ) {
		var span = CalcSpanSince( dateTime );

		this.Value = span.CalcTotalPlanckTimes().Value;
		this.Date = new Date( new Years( dateTime.Year ), new Months( ( Byte )dateTime.Month ), new Days( ( Byte )dateTime.Day ) );

		this.Hours = span.Hours;
		this.Minutes = span.Minutes;
		this.Seconds = span.Seconds;
		this.Milliseconds = span.Milliseconds;
		this.Microseconds = span.Microseconds;
	}

	/// <summary>
	///     <para>1 planck times</para>
	/// </summary>
	public static UniversalDateTime One => new( BigInteger.One );

	/// <summary>
	///     <para>The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001.</para>
	///     <para>430,000,000,000,000,000 seconds</para>
	/// </summary>
	public static PlanckTimes PlancksUpToMinDateTime => new( new Seconds( 4.3E17m ).Value );

	/// <summary>
	///     <para>0 planck times</para>
	/// </summary>
	public static UniversalDateTime TheBeginning => new( BigInteger.Zero );

	public static UniversalDateTime Unix => new( DateTime.UnixEpoch );

	[JsonProperty]
	public Date Date { get; }

	[JsonProperty]
	public Hours Hours { get; }

	[JsonProperty]
	public Minutes Minutes { get; }

	[JsonProperty]
	public Seconds Seconds { get; }

	[JsonProperty]
	public Milliseconds Milliseconds { get; }

	[JsonProperty]
	public Microseconds Microseconds { get; }

	/// <summary>
	///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
	/// </summary>
	[JsonProperty]
	public BigInteger Value { get; }

	public Int32 CompareTo( UniversalDateTime? other ) => this.Value.CompareTo( other?.Value );

	private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) => new( left.Value + value );

	private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) => Combine( left, right.Value );

	/// <summary>Given a <see cref="DateTime" />, calculate the <see cref="SpanOfTime" />.</summary>
	/// <param name="dateTime"></param>
	public static SpanOfTime CalcSpanSince( DateTime dateTime ) {
		var sinceThen = new SpanOfTime( dateTime - DateTime.MinValue ); //TODO why?
		var plancksSinceThen = sinceThen.CalcTotalPlanckTimes();
		var span = new SpanOfTime( PlancksUpToMinDateTime.Value + plancksSinceThen.Value );

		return span;
	}

	public static UniversalDateTime Now() => new( DateTime.UtcNow );

	public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) => Combine( left, -right );

	public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) => new( universalDateTime.Value * -1 );

	public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) => left.Value < right.Value;

	public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) => left.Value > right.Value;

	public override String ToString() => this.Value.ToString();

	public static Boolean operator <=( UniversalDateTime left, UniversalDateTime right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( UniversalDateTime left, UniversalDateTime right ) => left.CompareTo( right ) >= 0;
}