// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Hours.cs" last formatted on 2021-11-30 at 7:19 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;

[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record Hours( BigDecimal Value ) : IQuantityOfTime, IComparable<Hours> {

	/// <summary>24</summary>
	public const Byte InOneDay = 24;

	/// <summary>Eight <see cref="Hours" /> .</summary>
	public static Hours Eight { get; } = new( 8 );

	/// <summary>One <see cref="Hours" /> .</summary>
	public static Hours One { get; } = new( 1 );

	public static Hours Ten { get; } = new( 10 );

	public static Hours Thousand { get; } = new( 1000 );

	/// <summary>Zero <see cref="Hours" /></summary>
	public static Hours Zero { get; } = new( 0 );

	public Int32 CompareTo( Hours? other ) => this.Value.CompareTo( other?.Value );

	public IQuantityOfTime ToFinerGranularity() => this.ToMinutes();

	public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneHour );

	public Seconds ToSeconds() => new( this.Value / Seconds.InOneHour );

	public IQuantityOfTime ToCoarserGranularity() => this.ToDays();

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public static Hours Combine( Hours left, Hours right ) => Combine( left, right.Value );

	public static Hours Combine( Hours left, BigDecimal hours ) => new( left.Value + hours );

	public static Hours Combine( Hours left, BigInteger hours ) => new( left.Value + hours );

	/// <summary>Implicitly convert the number of <paramref name="hours" /> to <see cref="Days" />.</summary>
	/// <param name="hours"></param>
	public static implicit operator Days( Hours hours ) => hours.ToDays();

	/// <summary>Implicitly convert the number of <paramref name="hours" /> to <see cref="Minutes" />.</summary>
	/// <param name="hours"></param>
	public static implicit operator Minutes( Hours hours ) => hours.ToMinutes();

	public static implicit operator SpanOfTime( Hours hours ) => new( hours );

	public static implicit operator TimeSpan( Hours hours ) => TimeSpan.FromHours( ( Double )hours.Value );

	public static Hours operator -( Hours hours ) => new( hours.Value * -1 );

	public static Hours operator -( Hours left, Hours right ) => Combine( left, -right );

	public static Hours operator -( Hours left, BigDecimal hours ) => Combine( left, -hours );

	public static Hours operator +( Hours left, Hours right ) => Combine( left, right );

	public static Hours operator +( Hours left, BigDecimal hours ) => Combine( left, hours );

	public static Hours operator +( Hours left, BigInteger hours ) => Combine( left, hours );

	public static Boolean operator <( Hours left, Hours right ) => left.Value < right.Value;

	public static Boolean operator <( Hours left, Minutes right ) => left < ( Hours )right;

	public static Boolean operator >( Hours left, Minutes right ) => left > ( Hours )right;

	public static Boolean operator >( Hours left, Hours right ) => left.Value > right.Value;

	public Days ToDays() => new( this.Value / InOneDay );

	public Minutes ToMinutes() => new( this.Value * Minutes.InOneHour );

	public override String ToString() => $"{this.Value} hours";

	/*
	 * Add this? months aren't always 30 days..

	/// <summary>730 <see cref="Hours" /> in one month, according to WolframAlpha.</summary>
	/// <see cref="http://www.wolframalpha.com/input/?i=converts+1+month+to+hours" />
	public static BigInteger InOneMonth = 730;
	*/
}