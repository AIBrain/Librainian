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
// File "Weeks.cs" last touched on 2021-03-07 at 3:03 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Exceptions;
	using Maths.Bigger;
	using Newtonsoft.Json;

	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public record Weeks( BigDecimal Value ) : IQuantityOfTime, IComparable<Weeks>, IComparable<IQuantityOfTime> {

		/// <summary>
		///     52
		/// </summary>
		public const Byte InOneCommonYear = 52;

		/// <summary>
		///     4.345
		/// </summary>
		public const Decimal InOneMonth = 4.345m;

		/// <summary>
		///     One <see cref="Weeks" />.
		/// </summary>
		public static Weeks One { get; } = new( 1 );

		/// <summary>
		/// </summary>
		public static Weeks Ten { get; } = new( 10 );

		/// <summary>
		/// </summary>
		public static Weeks Thousand { get; } = new( 1000 );

		/// <summary>
		///     Zero <see cref="Weeks" />.
		/// </summary>
		public static Weeks Zero { get; } = new( 0 );

		public Int32 CompareTo( Weeks? other ) {
			if ( other is null ) {
				throw new ArgumentEmptyException( nameof( other ) );
			}

			return this.Value.CompareTo( other.Value );
		}

		public Int32 CompareTo( IQuantityOfTime? other ) {
			if ( other is null ) {
				return Order.Before;
			}

			if ( ReferenceEquals( this, other ) ) {
				return Order.Same;
			}

			return this.ToPlanckTimes().CompareTo( other.ToPlanckTimes() );
		}

		public IQuantityOfTime ToFinerGranularity() => new Days( this.Value * Days.InOneWeek );

		public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneWeek );

		public Seconds ToSeconds() => new( this.Value * Seconds.InOneWeek );

		/// <summary>
		///     Return this value in <see cref="Months" />.
		/// </summary>
		/// <returns></returns>
		public IQuantityOfTime ToCoarserGranularity() => this.ToMonths();

		TimeSpan IQuantityOfTime.ToTimeSpan() => this.ToSeconds();

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public TimeSpan? ToTimeSpan() => this.ToSeconds();

		public static Weeks Combine( Weeks left, Weeks right ) => new( left.Value + right.Value );

		public static Weeks Combine( Weeks left, BigDecimal weeks ) => new( left.Value + weeks );

		public static Weeks Combine( Weeks left, BigInteger weeks ) => new( left.Value + weeks );

		public static implicit operator Days( Weeks weeks ) => weeks.ToDays();

		public static implicit operator Months( Weeks weeks ) => weeks.ToMonths();

		public static implicit operator SpanOfTime( Weeks weeks ) => new( weeks: weeks );

		public static Weeks operator -( Weeks days ) => new( days.Value * -1 );

		public static Weeks operator -( Weeks left, Weeks right ) => Combine( left, -right );

		public static Weeks operator +( Weeks left, Weeks right ) => Combine( left, right );

		public static Weeks operator +( Weeks left, BigDecimal weeks ) => Combine( left, weeks );

		public static Weeks operator +( Weeks left, BigInteger weeks ) => Combine( left, weeks );

		public static Boolean operator <( Weeks left, Weeks right ) => left.Value < right.Value;

		public static Boolean operator <( Weeks left, Days right ) => left < ( Weeks )right;

		public static Boolean operator <( Weeks left, Months right ) => ( Months )left < right;

		public static Boolean operator >( Weeks left, Months right ) => ( Months )left > right;

		public static Boolean operator >( Weeks left, Days right ) => left > ( Weeks )right;

		public static Boolean operator >( Weeks left, Weeks right ) => left.Value > right.Value;

		public Days ToDays() => new( this.Value * Days.InOneWeek );

		public Months ToMonths() => new( this.Value / InOneMonth );

		public override String ToString() => $"{this.Value} week" + ( this.Value==1 ? String.Empty : "s" );
	}
}