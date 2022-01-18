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
// File "Years.cs" last touched on 2022-01-18 at 6:31 AM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time;

using System;
using System.Diagnostics;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Extensions;
using Newtonsoft.Json;
using Parsing;

[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record Years( BigDecimal Value ) : IQuantityOfTime, IComparable<Years>, IComparable<IQuantityOfTime> {

	/// <summary>One <see cref="Years" /> .</summary>
	public static Years One { get; } = new(1);

	/// <summary></summary>
	public static Years Ten { get; } = new(10);

	/// <summary></summary>
	public static Years Thousand { get; } = new(1000);

	/// <summary>Zero <see cref="Years" /></summary>
	public static Years Zero { get; } = new(0);

	public Int32 CompareTo( Years? other ) {
		if ( other is null ) {
			throw new ArgumentEmptyException( nameof( other ) );
		}

		return this.Value.CompareTo( other.Value );
	}

	public Int32 CompareTo( IQuantityOfTime? other ) {
		if ( ReferenceEquals( this, other ) ) {
			return 0;
		}

		if ( other is null ) {
			return 1;
		}

		return this.ToPlanckTimes().CompareTo( other.ToPlanckTimes() );
	}

	public IQuantityOfTime ToFinerGranularity() => new Months( this.Value * Months.InOneCommonYear );

	public PlanckTimes ToPlanckTimes() => new(this.Value * PlanckTimes.InOneYear);

	public Seconds ToSeconds() => new(this.Value * Seconds.InOneCommonYear);

	public IQuantityOfTime ToCoarserGranularity() => this;

	public TimeSpan ToTimeSpan() => this.ToSeconds();

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public static Years Combine( Years left, Years right ) => Combine( left, right.Value );

	public static Years Combine( Years left, BigDecimal years ) => new(left.Value + years);

	public static implicit operator Months( Years years ) => years.ToMonths();

	public static implicit operator SpanOfTime( Years years ) => new(years: years);

	public static Years operator -( Years years ) => new(years.Value * -1);

	public static Years operator -( Years left, Years right ) => Combine( left, -right );

	public static Years operator -( Years left, BigDecimal years ) => Combine( left, -years );

	public static Years operator +( Years left, Years right ) => Combine( left, right );

	public static Years operator +( Years left, BigDecimal years ) => Combine( left, years );

	public static Years operator +( Years left, BigInteger years ) => Combine( left, years );

	public static Boolean operator <( Years left, Years right ) => left.Value < right.Value;

	public static Boolean operator >( Years left, Years right ) => left.Value > right.Value;

	public Days ToDays() => new(this.Value * Days.InOneCommonYear);

	public Months ToMonths() => new(this.Value * Months.InOneCommonYear);

	public override String ToString() => $"{this.Value} {this.Value.PluralOf( "year" )}";

	public Weeks ToWeeks() => new(this.Value * Weeks.InOneCommonYear);

	public static Boolean operator <=( Years left, Years right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Years left, Years right ) => left.CompareTo( right ) >= 0;

}