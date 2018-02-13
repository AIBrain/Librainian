// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Duration.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

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
    /// <seealso cref="Span" />
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

        public Duration( Microseconds microseconds ) => this.totalMicroseconds = ( Double )microseconds.Value * MicsPerMicrosecond;

	    public Duration( Milliseconds milliseconds ) => this.totalMicroseconds = ( Double )milliseconds.Value * MicsPerMillisecond;

	    public Duration( Seconds seconds ) => this.totalMicroseconds = ( Double )seconds.Value * MicsPerSecond;

	    public Duration( Minutes minutes ) => this.totalMicroseconds = ( Double )minutes.Value * MicsPerMinute;

	    public Duration( Hours hours ) => this.totalMicroseconds = ( Double )hours.Value * MicsPerHour;

	    public Duration( Days days ) => this.totalMicroseconds = ( Double )days.Value * MicsPerDay;

	    public Duration( Weeks weeks ) => this.totalMicroseconds = ( Double )weeks.Value * MicsPerWeek;

	    public Duration( Years years ) => this.totalMicroseconds = ( Double )years.Value * MicsPerYear;

	    public Duration( Int64 ticks ) => this.totalMicroseconds = ticks / 10.0;

	    public Duration( TimeSpan time ) : this( ticks: time.Ticks ) {
        }

        public Duration( [NotNull] params TimeSpan[] times ) {
            if ( times == null ) {
                throw new ArgumentNullException( nameof( times ) );
            }

            var total = times.Select( timeSpan => new Duration( timeSpan ) ).Aggregate( BigRational.Zero, ( current, dur ) => current + dur.totalMicroseconds );

            this.totalMicroseconds = ( Double )total;
        }

        public Double Days => this.Hours / Measurement.Time.Hours.InOneDay % Measurement.Time.Hours.InOneDay;

        public Double Hours => ( Byte )( this.Minutes / Measurement.Time.Minutes.InOneHour % Measurement.Time.Minutes.InOneHour );

        public Double Microseconds => this.totalMicroseconds;

        public Double Milliseconds => ( UInt16 )( this.Microseconds / Measurement.Time.Microseconds.InOneMillisecond % Measurement.Time.Microseconds.InOneMillisecond );

        public Double Minutes => ( Byte )( this.Seconds / Measurement.Time.Seconds.InOneMinute % Measurement.Time.Seconds.InOneMinute );

        public Double Seconds => ( Byte )( this.Milliseconds / Measurement.Time.Milliseconds.InOneSecond % Measurement.Time.Milliseconds.InOneSecond );

        public Double TotalDays => this.TotalHours / Measurement.Time.Hours.InOneDay;

        public Double TotalHours => this.TotalMinutes / Measurement.Time.Minutes.InOneHour;

        public Double TotalMicroseconds => this.totalMicroseconds;

        public Double TotalMilliseconds => this.TotalMicroseconds / Measurement.Time.Microseconds.InOneMillisecond;

        public Double TotalMinutes => this.TotalSeconds / Measurement.Time.Seconds.InOneMinute;

        public Double TotalSeconds => this.TotalMilliseconds / Measurement.Time.Milliseconds.InOneSecond;

        public Double TotalWeeks => this.TotalDays / Measurement.Time.Days.InOneWeek;

        public Double TotalYears => this.TotalDays / Measurement.Time.Days.InOneCommonYear;

        public Double Weeks => this.Days / Measurement.Time.Days.InOneWeek % Measurement.Time.Days.InOneWeek;

        public Double Years => this.Days / Measurement.Time.Days.InOneCommonYear % Measurement.Time.Days.InOneCommonYear;

        [JsonProperty]

        // ReSharper disable once InconsistentNaming
        internal Double totalMicroseconds {
            get;
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