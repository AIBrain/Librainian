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
// File "Minutes.cs" last touched on 2021-04-25 at 7:42 AM by Protiguous.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Exceptions;
	using Extensions;
	using JetBrains.Annotations;
	using Maths.Bigger;
	using Newtonsoft.Json;

	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public record Minutes( BigDecimal Value ) : IQuantityOfTime, IComparable<Minutes> {

		/// <summary>
		///     60
		/// </summary>
		public const Byte InOneHour = 60;

		public Minutes( Byte value ) : this( ( BigDecimal )value ) { }

		public Minutes( Int16 value ) : this( ( BigDecimal )value ) { }

		public Minutes( UInt16 value ) : this( ( BigDecimal )value ) { }

		public Minutes( Int32 value ) : this( ( BigDecimal )value ) { }

		public Minutes( UInt32 value ) : this( ( BigDecimal )value ) { }

		public Minutes( Int64 value ) : this( ( BigDecimal )value ) { }

		public Minutes( UInt64 value ) : this( ( BigDecimal )value ) { }

		public Minutes( Single value ) : this( ( BigDecimal )value ) { }

		public Minutes( Double value ) : this( ( BigDecimal )value ) { }

		/// <summary>
		///     15
		/// </summary>
		public static Minutes Fifteen { get; } = new( 15 );

		/// <summary>
		///     One <see cref="Minutes" /> .
		/// </summary>
		public static Minutes One { get; } = new( 1 );

		/// <summary>
		///     10
		/// </summary>
		public static Minutes Ten { get; } = new( 10 );

		/// <summary>
		///     30
		/// </summary>
		public static Minutes Thirty { get; } = new( 30 );

		/// <summary>
		/// </summary>
		public static Minutes Thousand { get; } = new( 1000 );

		/// <summary>
		///     Zero <see cref="Minutes" />
		/// </summary>
		public static Minutes Zero { get; } = new( 0 );

		public Int32 CompareTo( Minutes? other ) {
			if ( other == null ) {
				throw new ArgumentEmptyException( nameof( other ) );
			}

			return this.Value.CompareTo( other.Value );
		}

		public IQuantityOfTime ToFinerGranularity() => this.ToSeconds();

		public PlanckTimes ToPlanckTimes() => new( PlanckTimes.InOneMinute );

		[Pure]
		public Seconds ToSeconds() => new( this.Value * Seconds.InOneMinute );

		public IQuantityOfTime ToCoarserGranularity() => this.ToHours();

		public TimeSpan ToTimeSpan() => this.ToSeconds();

		public static Minutes Combine( Minutes left, Minutes right ) => Combine( left, right.Value );

		public static Minutes Combine( Minutes left, BigDecimal minutes ) => new( left.Value + minutes );

		public static Minutes Combine( Minutes left, BigInteger minutes ) => new( left.Value + minutes );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		public static Boolean Equals( Minutes left, Minutes right ) => left.Value == right.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Hours" />.
		/// </summary>
		/// <param name="minutes"></param>
		public static implicit operator Hours( Minutes minutes ) => minutes.ToHours();

		/// <summary>
		///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Seconds" />.
		/// </summary>
		/// <param name="minutes"></param>
		public static implicit operator Seconds( Minutes minutes ) => minutes.ToSeconds();

		/// <summary>
		///     Implicitly convert the number of <paramref name="minutes" /> to a <see cref="SpanOfTime" />.
		/// </summary>
		/// <param name="minutes"></param>
		public static implicit operator SpanOfTime( Minutes minutes ) => new( minutes );

		public static implicit operator TimeSpan( Minutes minutes ) => TimeSpan.FromMinutes( ( Double )minutes.Value );

		public static Minutes operator -( Minutes minutes ) => new( minutes.Value * -1 );

		public static Minutes operator -( Minutes left, Minutes right ) => Combine( left, -right );

		public static Minutes operator -( Minutes left, BigDecimal minutes ) => Combine( left, -minutes );

		public static Minutes operator +( Minutes left, Minutes right ) => Combine( left, right );

		public static Minutes operator +( Minutes left, BigDecimal minutes ) => Combine( left, minutes );

		public static Minutes operator +( Minutes left, BigInteger minutes ) => Combine( left, minutes );

		public static Boolean operator <( Minutes left, Minutes right ) => left.Value < right.Value;

		public static Boolean operator <( Minutes left, Hours right ) => ( Hours )left < right;

		public static Boolean operator <( Minutes left, Seconds right ) => left < ( Minutes )right;

		public static Boolean operator >( Minutes left, Hours right ) => ( Hours )left > right;

		public static Boolean operator >( Minutes left, Minutes right ) => left.Value > right.Value;

		public static Boolean operator >( Minutes left, Seconds right ) => left > ( Minutes )right;

		public Hours ToHours() => new( this.Value / InOneHour );

		public override String ToString() => $"{this.Value} minutes";
	}
}