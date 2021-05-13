// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Duration.cs" last formatted on 2020-08-14 at 8:38 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Linq;
	using System.Numerics;
	using ExtendedNumerics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Expands <see cref="TimeSpan" /> to include microseconds, weeks (7 days), and years (365 days).</para>
	///     <para>Internally stores the value as the total microseconds (<see cref="Microseconds" />).</para>
	/// </summary>
	/// <see cref="SpanOfTime" />
	/// TODO This class is likely full of math bugs and rounding errors.
	[JsonObject]
	[Immutable]
	public record Duration( BigDecimal Microseconds ) : IComparable<Duration>, IComparable<TimeSpan> {

		public static BigDecimal MicrosecondsPerDay => MicrosecondsPerHour * Measurement.Time.Hours.InOneDay;

		public static BigDecimal MicrosecondsPerHour => MicrosecondsPerMinute * Measurement.Time.Minutes.InOneHour;

		public const Decimal MicrosecondsPerMicrosecond = 1;

		public static BigDecimal MicrosecondsPerMillisecond => MicrosecondsPerMicrosecond * Measurement.Time.Microseconds.InOneMillisecond;

		public static BigDecimal MicrosecondsPerMinute => MicrosecondsPerSecond * Measurement.Time.Seconds.InOneMinute;

		public static BigDecimal MicrosecondsPerSecond => MicrosecondsPerMillisecond * Measurement.Time.Milliseconds.InOneSecond;

		public static BigDecimal MicrosecondsPerWeek => MicrosecondsPerDay * Measurement.Time.Days.InOneWeek;

		public static BigDecimal MicrosecondsPerYear => MicrosecondsPerDay * Measurement.Time.Days.InOneCommonYear;


		public Duration( Microseconds microseconds ) : this( microseconds.Value * MicrosecondsPerMicrosecond ) { }

		public Duration( Milliseconds milliseconds ) : this( milliseconds.Value * MicrosecondsPerMillisecond ) { }

		public Duration( Seconds seconds ) : this( seconds.Value * MicrosecondsPerSecond ) { }

		public Duration( Minutes minutes ) : this( minutes.Value * MicrosecondsPerMinute ) { }

		public Duration( Hours hours ) : this( hours.Value * MicrosecondsPerHour ) { }

		public Duration( Days days ) : this( days.Value * MicrosecondsPerDay ) { }

		public Duration( Weeks weeks ) : this( weeks.Value * MicrosecondsPerWeek ) { }

		public Duration( Years years ) : this( years.Value * MicrosecondsPerYear ) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ticks"></param>
		public Duration( Int64 ticks ) : this( ticks / 10.0m ) { } //TODO Is /10 correct for ticks to microseconds?

		public Duration( TimeSpan time ) : this( time.Ticks ) { }

		public Duration( [NotNull] params TimeSpan[] times ) : this( times.Where( span => span != default( TimeSpan ) ).Sum( timeSpan => timeSpan.TotalMilliseconds ) * MicrosecondsPerMillisecond ) {
			
			
		}

		public static Duration FromDays( BigDecimal value ) => new( new Days( value ) );

		public static Duration FromHours( BigDecimal value ) => new( new Hours( value ) );

		public static Duration FromMicroseconds( BigDecimal value ) => new( new Microseconds( value ) );

		public static Duration FromMilliseconds( BigDecimal value ) => new( new Milliseconds( value ) );

		public static Duration FromMinutes( BigDecimal value ) => new( new Minutes( value ) );

		public static Duration FromSeconds( BigDecimal value ) => new( new Seconds( value ) );

		public static Duration FromTicks( Int64 value ) => new( value );

		public static Duration FromWeeks( Decimal value ) => new( new Weeks( value ) );

		public static Duration FromWeeks( BigDecimal value ) => new( new Weeks( value ) );

		public static Duration FromYears( BigDecimal value ) => new( new Years( value ) );

		/// <summary>
		///     <para>Compares <see cref="Microseconds" /></para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( Duration? other ) => this.Microseconds.CompareTo( other.Microseconds );

		/// <summary>
		///     <para>Compares <see cref="TotalMilliseconds" /></para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( TimeSpan other ) => this.TotalMilliseconds().CompareTo( other.TotalMilliseconds );

		public BigDecimal Days() => (BigInteger)this.TotalHours() % Measurement.Time.Hours.InOneDay;

		/// <summary>Returns the hash code for this instance.</summary>
		[Pure]
		public override Int32 GetHashCode() => this.Microseconds.GetHashCode();

		[Pure]
		public BigDecimal Hours() => ( BigInteger )this.TotalMinutes()  % Measurement.Time.Minutes.InOneHour;

		[Pure]
		public BigDecimal Milliseconds() => ( BigInteger )this.TotalMicroseconds() % Measurement.Time.Microseconds.InOneMillisecond;

		[Pure]
		public BigDecimal Minutes() => ( BigInteger )this.TotalSeconds()  % Measurement.Time.Seconds.InOneMinute;

		[Pure]
		public BigDecimal Seconds() => ( BigInteger )this.TotalMilliseconds() % Measurement.Time.Milliseconds.InOneSecond;

		[Pure]
		[NotNull]
		public override String ToString() => this.Simpler();

		[Pure]
		public BigDecimal TotalDays() => this.TotalHours() / Measurement.Time.Hours.InOneDay;

		[Pure]
		public BigDecimal TotalHours() => this.TotalMinutes() / Measurement.Time.Minutes.InOneHour;

		[Pure]
		public BigDecimal TotalMicroseconds() => this.Microseconds;

		[Pure]
		public BigDecimal TotalMilliseconds() => this.TotalMicroseconds() / Measurement.Time.Microseconds.InOneMillisecond;

		[Pure]
		public BigDecimal TotalMinutes() => this.TotalSeconds() / Measurement.Time.Seconds.InOneMinute;

		[Pure]
		public BigDecimal TotalSeconds() => this.TotalMilliseconds() / Measurement.Time.Milliseconds.InOneSecond;

		[Pure]
		public BigDecimal TotalWeeks() => this.TotalDays() / Measurement.Time.Days.InOneWeek;

		[Pure]
		public BigDecimal TotalYears() => this.TotalDays() / Measurement.Time.Days.InOneCommonYear;

		[Pure]
		public BigDecimal Weeks() => ( BigInteger )this.TotalDays()  % Measurement.Time.Days.InOneWeek;

		[Pure]
		public BigDecimal Years() => ( BigInteger )this.TotalDays() % ( BigInteger )Measurement.Time.Days.InOneCommonYear;
	}
}