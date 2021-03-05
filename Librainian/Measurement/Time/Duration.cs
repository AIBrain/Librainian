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
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Rationals;

	/// <summary>
	///     <para>Expands <see cref="TimeSpan" /> to include microseconds, weeks (7 days), and years (365 days).</para>
	///     <para>Internally based upon the total number of microseconds (<see cref="Microseconds" />).</para>
	/// </summary>
	/// <see cref="SpanOfTime" />
	[JsonObject]
	[Immutable]
	public struct Duration : IComparable<Duration>, IComparable<TimeSpan> {

		public const Double MicrosecondsPerDay = MicrosecondsPerHour * Measurement.Time.Hours.InOneDay;

		public const Double MicrosecondsPerHour = MicrosecondsPerMinute * Measurement.Time.Minutes.InOneHour;

		public const Double MicrosecondsPerMicrosecond = 1;

		public const Double MicrosecondsPerMillisecond = MicrosecondsPerMicrosecond * Measurement.Time.Microseconds.InOneMillisecond;

		public const Double MicrosecondsPerMinute = MicrosecondsPerSecond * Measurement.Time.Seconds.InOneMinute;

		public const Double MicrosecondsPerSecond = MicrosecondsPerMillisecond * Measurement.Time.Milliseconds.InOneSecond;

		public const Double MicrosecondsPerWeek = MicrosecondsPerDay * Measurement.Time.Days.InOneWeek;

		public const Double MicrosecondsPerYear = MicrosecondsPerDay * Measurement.Time.Days.InOneCommonYear;

		[JsonProperty]
		public Double Microseconds { get; }

		public Duration( Microseconds microseconds ) => this.Microseconds = ( Double )microseconds.Value * MicrosecondsPerMicrosecond;

		public Duration( Milliseconds milliseconds ) => this.Microseconds = ( Double )milliseconds.Value * MicrosecondsPerMillisecond;

		public Duration( Seconds seconds ) => this.Microseconds = ( Double )seconds.Value * MicrosecondsPerSecond;

		public Duration( Minutes minutes ) => this.Microseconds = ( Double )minutes.Value * MicrosecondsPerMinute;

		public Duration( Hours hours ) => this.Microseconds = ( Double )hours.Value * MicrosecondsPerHour;

		public Duration( Days days ) => this.Microseconds = ( Double )days.Value * MicrosecondsPerDay;

		public Duration( Weeks weeks ) => this.Microseconds = ( Double )weeks.Value * MicrosecondsPerWeek;

		public Duration( Years years ) => this.Microseconds = ( Double )years.Value * MicrosecondsPerYear;

		public Duration( Int64 ticks ) => this.Microseconds = ticks / 10.0;

		public Duration( TimeSpan time ) : this( time.Ticks ) { }

		public Duration( [NotNull] params TimeSpan[] times ) {
			if ( times is null ) {
				throw new ArgumentNullException( nameof( times ) );
			}

			var totalMilliseconds = times.Where( span => span != default ).Sum( timeSpan => timeSpan.TotalMilliseconds );

			this.Microseconds = totalMilliseconds * MicrosecondsPerMillisecond;
		}

		public static Duration FromDays( Double value ) => new( new Days( ( Rational )value ) );

		public static Duration FromHours( Double value ) => new( new Hours( ( Rational )value ) );

		public static Duration FromMicroseconds( Double value ) => new( new Microseconds( ( Rational )value ) );

		public static Duration FromMilliseconds( Double value ) => new( new Milliseconds( value ) );

		public static Duration FromMinutes( Double value ) => new( new Minutes( ( Rational )value ) );

		public static Duration FromSeconds( Double value ) => new( new Seconds( ( Rational )value ) );

		public static Duration FromTicks( Int64 value ) => new( value );

		public static Duration FromWeeks( Double value ) => new( new Weeks( ( Rational )value ) );

		public static Duration FromYears( Double value ) => new( new Years( ( Rational )value ) );

		/// <summary>
		///     <para>Compares <see cref="Microseconds" /></para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( Duration other ) => this.Microseconds.CompareTo( other.Microseconds );

		/// <summary>
		///     <para>Compares <see cref="TotalMilliseconds" /></para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( TimeSpan other ) => this.TotalMilliseconds().CompareTo( other.TotalMilliseconds );

		public Double Days() => this.Hours() / Measurement.Time.Hours.InOneDay % Measurement.Time.Hours.InOneDay;

		/// <summary>Returns the hash code for this instance.</summary>
		[Pure]
		public override Int32 GetHashCode() => this.Microseconds.GetHashCode();

		[Pure]
		public Double Hours() => ( Byte )( this.Minutes() / Measurement.Time.Minutes.InOneHour % Measurement.Time.Minutes.InOneHour );

		[Pure]
		public Double Milliseconds() => ( UInt16 )( this.Microseconds / Measurement.Time.Microseconds.InOneMillisecond % Measurement.Time.Microseconds.InOneMillisecond );

		[Pure]
		public Double Minutes() => ( Byte )( this.Seconds() / Measurement.Time.Seconds.InOneMinute % Measurement.Time.Seconds.InOneMinute );

		[Pure]
		public Double Seconds() => ( Byte )( this.Milliseconds() / Measurement.Time.Milliseconds.InOneSecond % Measurement.Time.Milliseconds.InOneSecond );

		[Pure]
		[NotNull]
		public override String ToString() => this.Simpler();

		[Pure]
		public Double TotalDays() => this.TotalHours() / Measurement.Time.Hours.InOneDay;

		[Pure]
		public Double TotalHours() => this.TotalMinutes() / Measurement.Time.Minutes.InOneHour;

		[Pure]
		public Double TotalMicroseconds() => this.Microseconds;

		[Pure]
		public Double TotalMilliseconds() => this.TotalMicroseconds() / Measurement.Time.Microseconds.InOneMillisecond;

		[Pure]
		public Double TotalMinutes() => this.TotalSeconds() / Measurement.Time.Seconds.InOneMinute;

		[Pure]
		public Double TotalSeconds() => this.TotalMilliseconds() / Measurement.Time.Milliseconds.InOneSecond;

		[Pure]
		public Double TotalWeeks() => this.TotalDays() / Measurement.Time.Days.InOneWeek;

		[Pure]
		public Double TotalYears() => this.TotalDays() / Measurement.Time.Days.InOneCommonYear;

		[Pure]
		public Double Weeks() => this.Days() / Measurement.Time.Days.InOneWeek % Measurement.Time.Days.InOneWeek;

		[Pure]
		public Double Years() => this.Days() / Measurement.Time.Days.InOneCommonYear % Measurement.Time.Days.InOneCommonYear;

	}

}