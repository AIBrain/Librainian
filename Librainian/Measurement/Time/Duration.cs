// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Duration.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Duration.cs" was last formatted by Protiguous on 2018/06/26 at 1:28 AM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Linq;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Numerics;

	/// <summary>
	///     <para>Expands <see cref="TimeSpan" /> to include microseconds, weeks (7 days), and years (365 days).</para>
	///     <para>Internally based upon the total number of microseconds (<see cref="totalMicroseconds" />).</para>
	/// </summary>
	/// <seealso cref="SpanOfTime" />
	[JsonObject]
	[Immutable]
	public struct Duration : IComparable<Duration>, IComparable<TimeSpan> {

		public const Double MicsPerDay = MicsPerHour * Measurement.Time.Hours.InOneDay;

		public const Double MicsPerHour = MicsPerMinute * Measurement.Time.Minutes.InOneHour;

		public const Double MicsPerMicrosecond = 1;

		public const Double MicsPerMillisecond = MicsPerMicrosecond * Measurement.Time.Microseconds.InOneMillisecond;

		public const Double MicsPerMinute = MicsPerSecond * Measurement.Time.Seconds.InOneMinute;

		public const Double MicsPerSecond = MicsPerMillisecond * Measurement.Time.Milliseconds.InOneSecond;

		public const Double MicsPerWeek = MicsPerDay * Measurement.Time.Days.InOneWeek;

		public const Double MicsPerYear = MicsPerDay * Measurement.Time.Days.InOneCommonYear;

		[JsonProperty]

		// ReSharper disable once InconsistentNaming
		internal Double totalMicroseconds { get; }

		public Double Days => this.Hours / Measurement.Time.Hours.InOneDay % Measurement.Time.Hours.InOneDay;

		public Double Milliseconds => ( UInt16 ) ( this.Microseconds / Measurement.Time.Microseconds.InOneMillisecond % Measurement.Time.Microseconds.InOneMillisecond );

		public Double Minutes => ( Byte ) ( this.Seconds / Measurement.Time.Seconds.InOneMinute % Measurement.Time.Seconds.InOneMinute );

		public Double TotalDays => this.TotalHours / Measurement.Time.Hours.InOneDay;

		public Double TotalMilliseconds => this.TotalMicroseconds / Measurement.Time.Microseconds.InOneMillisecond;

		public Double TotalMinutes => this.TotalSeconds / Measurement.Time.Seconds.InOneMinute;

		public Double TotalWeeks => this.TotalDays / Measurement.Time.Days.InOneWeek;

		public Double TotalYears => this.TotalDays / Measurement.Time.Days.InOneCommonYear;

		public Double Weeks => this.Days / Measurement.Time.Days.InOneWeek % Measurement.Time.Days.InOneWeek;

		public Double Years => this.Days / Measurement.Time.Days.InOneCommonYear % Measurement.Time.Days.InOneCommonYear;

		public Double Hours => ( Byte ) ( this.Minutes / Measurement.Time.Minutes.InOneHour % Measurement.Time.Minutes.InOneHour );

		public Double Microseconds => this.totalMicroseconds;

		public Double Seconds => ( Byte ) ( this.Milliseconds / Measurement.Time.Milliseconds.InOneSecond % Measurement.Time.Milliseconds.InOneSecond );

		public Double TotalHours => this.TotalMinutes / Measurement.Time.Minutes.InOneHour;

		public Double TotalMicroseconds => this.totalMicroseconds;

		public Double TotalSeconds => this.TotalMilliseconds / Measurement.Time.Milliseconds.InOneSecond;

		public Duration( Microseconds microseconds ) => this.totalMicroseconds = ( Double ) microseconds.Value * MicsPerMicrosecond;

		public Duration( Milliseconds milliseconds ) => this.totalMicroseconds = ( Double ) milliseconds.Value * MicsPerMillisecond;

		public Duration( Seconds seconds ) => this.totalMicroseconds = ( Double ) seconds.Value * MicsPerSecond;

		public Duration( Minutes minutes ) => this.totalMicroseconds = ( Double ) minutes.Value * MicsPerMinute;

		public Duration( Hours hours ) => this.totalMicroseconds = ( Double ) hours.Value * MicsPerHour;

		public Duration( Days days ) => this.totalMicroseconds = ( Double ) days.Value * MicsPerDay;

		public Duration( Weeks weeks ) => this.totalMicroseconds = ( Double ) weeks.Value * MicsPerWeek;

		public Duration( Years years ) => this.totalMicroseconds = ( Double ) years.Value * MicsPerYear;

		public Duration( Int64 ticks ) => this.totalMicroseconds = ticks / 10.0;

		public Duration( TimeSpan time ) : this( ticks: time.Ticks ) { }

		public Duration( [NotNull] params TimeSpan[] times ) {
			if ( times is null ) {
				throw new ArgumentNullException( nameof( times ) );
			}

			var total = times.Select( timeSpan => new Duration( timeSpan ) ).Aggregate( BigRational.Zero, ( current, dur ) => current + dur.totalMicroseconds );

			this.totalMicroseconds = ( Double ) total;
		}

		public static Duration FromDays( Double value ) => new Duration( new Days( value ) );

		public static Duration FromHours( Double value ) => new Duration( new Hours( value ) );

		public static Duration FromMicroseconds( Double value ) => new Duration( new Microseconds( value ) );

		public static Duration FromMilliseconds( Double value ) => new Duration( new Milliseconds( value ) );

		public static Duration FromMinutes( Double value ) => new Duration( new Minutes( value ) );

		public static Duration FromSeconds( Double value ) => new Duration( new Seconds( value ) );

		public static Duration FromTicks( Int64 value ) => new Duration( ticks: value );

		public static Duration FromWeeks( Double value ) => new Duration( new Weeks( value ) );

		public static Duration FromYears( Double value ) => new Duration( new Years( value ) );

		/// <summary>
		///     <para>Compares <see cref="totalMicroseconds" /></para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( Duration other ) => this.totalMicroseconds.CompareTo( other.totalMicroseconds );

		/// <summary>
		///     <para>Compares <see cref="TotalMilliseconds" /></para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Int32 CompareTo( TimeSpan other ) => this.TotalMilliseconds.CompareTo( other.TotalMilliseconds );

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		///     A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override Int32 GetHashCode() => this.totalMicroseconds.GetHashCode();

		public override String ToString() => this.Simpler();
	}
}