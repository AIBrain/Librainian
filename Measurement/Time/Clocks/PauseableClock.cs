// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PauseableClock.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Timers;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Numerics;

    /// <summary>
    ///     A 'pause-able' clock.
    /// </summary>
    [JsonObject]
    public class PauseableClock : IStandardClock {

        /// <summary>
        /// </summary>
        private volatile Boolean _isPaused;

        /// <summary>
        ///     Default to year 0.
        /// </summary>
        public PauseableClock() : this( Measurement.Time.Date.Zero, Measurement.Time.Time.Zero ) { }

        public PauseableClock( Date date, Time time ) {
            this.Year = date.Year;
            this.Month = date.Month;
            this.Day = date.Day;
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
            this.Millisecond = time.Millisecond;
            this.Timer.Elapsed += this.OnTimerElapsed;
            this.Resume();
        }

        [JsonProperty]
        public Day Day {
            get; private set;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Hour Hour {
            get; private set;
        }

        [JsonProperty]
        public Boolean IsPaused {
            get => this._isPaused;

	        private set => this._isPaused = value;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Millisecond Millisecond {
            get; private set;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Minute Minute {
            get; private set;
        }

        [JsonProperty]
        public Month Month {
            get; private set;
        }

        public Action<DateAndTime> OnDay {
            get; set;
        }

        public Action<DateAndTime> OnHour {
            get; set;
        }

        public Action<DateAndTime> OnMillisecond {
            get; set;
        }

        public Action<DateAndTime> OnMinute {
            get; set;
        }

        public Action<DateAndTime> OnMonth {
            get; set;
        }

        public Action<DateAndTime> OnSecond {
            get; set;
        }

        public Action<DateAndTime> OnYear {
            get; set;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Second Second {
            get; private set;
        }

        [JsonProperty]
        public Year Year {
            get; private set;
        }

        /// <summary>
        /// </summary>
        [NotNull]
        private Timer Timer { get; } = new Timer( interval: ( Double )Milliseconds.One.Value ) { AutoReset = false };

        /// <summary>
        ///     Advance the clock by <paramref name="amount" /><see cref="Milliseconds" />.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="skipEvents"></param>
        /// <returns></returns>
        public Boolean Advance( Milliseconds amount, Boolean skipEvents = true ) {
            try {
                this.Pause();
                var right = amount.Value;
                while ( right > BigRational.Zero ) {
                    TickTock( fireEvents: false );
                    right--;
                }
                return true;
            }
            finally {
                this.Resume();
            }
        }

        public Date Date() => new Date( this.Year, this.Month, this.Day );

	    public DateAndTime DateAndTime() => new DateAndTime( this.Date(), this.Time() );

	    public Boolean IsAm() => !this.IsPm();

        public Boolean IsPm() => this.Hour >= 12;

        public Boolean Pause() {
            this.Timer.Stop();
            this.IsPaused = true;
            return this.IsPaused;
        }

        public Boolean Resume() {
            this.IsPaused = false;
            this.Timer.Start();
            return !this.IsPaused;
        }

        /// <summary>
        ///     Rewind the clock by <paramref name="amount" /><see cref="Milliseconds" />.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Boolean Rewind( Milliseconds amount ) {
            try {
                this.Pause();

                //TODO
                throw new NotImplementedException();

                // ReSharper disable once HeuristicUnreachableCode
                //return false;
            }
            finally {
                this.Resume();
            }
        }

        public Time Time() => new Time( this.Hour, this.Minute, this.Second, this.Millisecond );

	    private Boolean DaysTocked( Boolean fireEvents ) {
			this.Day = this.Day.Next( out var tocked );
			if ( !tocked ) {
                return false;
            }
            try {
                if ( fireEvents ) {
                    this.OnDay?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return true;
        }

        private Boolean HoursTocked( Boolean fireEvents ) {
			this.Hour = this.Hour.Next( out var tocked );
			if ( !tocked ) {
                return false;
            }
            try {
                if ( fireEvents ) {
                    this.OnHour?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return true;
        }

        private Boolean MillisecondsTocked( Boolean fireEvents ) {
			this.Millisecond = this.Millisecond.Next( out var tocked );
			if ( !tocked ) {
                return false;
            }
            try {
                if ( fireEvents ) {
                    this.OnMillisecond?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return true;
        }

        private Boolean MinutesTocked( Boolean fireEvents ) {
			this.Minute = this.Minute.Next( out var tocked );
			if ( !tocked ) {
                return false;
            }
            try {
                if ( fireEvents ) {
                    this.OnMinute?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return true;
        }

        private Boolean MonthsTocked( Boolean fireEvents ) {
			this.Month = this.Month.Next( out var tocked );
			if ( !tocked ) {
                return false;
            }
            try {
                if ( fireEvents ) {
                    this.OnMonth?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return true;
        }

        private void OnTimerElapsed( Object sender, ElapsedEventArgs elapsedEventArgs ) {
            this.Pause();
            try {
                this.TickTock();
            }
            catch ( Exception exception ) {
                exception.More();
            }
            finally {
                this.Resume();
            }
        }

        private Boolean SecondsTocked( Boolean fireEvents ) {
			this.Second = this.Second.Next( out var tocked );
			if ( !tocked ) {
                return false;
            }
            try {
                if ( fireEvents ) {
                    this.OnSecond?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return true;
        }

        private void TickTock( Boolean fireEvents = true ) {
            if ( !this.MillisecondsTocked( fireEvents ) ) {
                return;
            }

            if ( !this.SecondsTocked( fireEvents ) ) {
                return;
            }

            if ( !this.MinutesTocked( fireEvents ) ) {
                return;
            }

            if ( !this.HoursTocked( fireEvents ) ) {
                return;
            }

            if ( !this.DaysTocked( fireEvents ) ) {
                return;
            }

            if ( !this.MonthsTocked( fireEvents ) ) {
                return;
            }

            this.YearsTocked( fireEvents );
        }

        private void YearsTocked( Boolean fireEvents ) {
            this.Year = this.Year.Next();
            try {
                if ( fireEvents ) {
                    this.OnYear?.Invoke( this.DateAndTime() );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }
    }
}