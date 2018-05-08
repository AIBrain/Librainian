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
// "Librainian/TickingClock.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Timers;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Starts a forward-ticking clock at the given time with settable events.</para>
    ///     <para>Should be threadsafe.</para>
    ///     <para>
    ///         Settable events are:
    ///         <para>
    ///             <see cref="OnHourTick" />
    ///         </para>
    ///         <para>
    ///             <see cref="OnMinuteTick" />
    ///         </para>
    ///         <para>
    ///             <see cref="OnSecondTick" />
    ///         </para>
    ///         <para>
    ///             <see cref="OnMillisecondTick" />
    ///         </para>
    ///     </para>
    /// </summary>
    [JsonObject]
    public class TickingClock : IStandardClock {

        /// <summary>
        /// </summary>
        [CanBeNull]
        private Timer _timer;

        public TickingClock( DateTime time, Granularity granularity = Granularity.Seconds ) {
            this.Hour = ( Byte )time.Hour;
            this.Minute = ( Byte )time.Minute;
            this.Second = ( Byte )time.Second;
            this.Millisecond = ( UInt16 )time.Millisecond;
            this.Microsecond = 0; //TODO can we get using DateTime.Ticks vs StopWatch.TicksPer/Frequency stuff?
            this.ResetTimer( granularity );
        }

        public TickingClock( Time time, Granularity granularity = Granularity.Seconds ) {
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
            this.Millisecond = time.Millisecond;
            this.Microsecond = time.Microsecond;
            this.ResetTimer( granularity );
        }

        public enum Granularity {
            Microseconds,
            Milliseconds,
            Seconds,
            Minutes,
            Hours
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Hour Hour {
            get; private set;
        }

        [JsonProperty]
        public UInt16 Microsecond {
            get; private set;
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

        [CanBeNull]
        [JsonProperty]
        public Action<Hour> OnHourTick {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public Action OnMillisecondTick {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public Action OnMinuteTick {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public Action OnSecondTick {
            get; set;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Second Second {
            get; private set;
        }

        public Boolean IsAm() => !this.IsPm();

        public Boolean IsPm() => this.Hour.Value >= 12;

        public void ResetTimer( Granularity granularity ) {
            if ( null != this._timer ) {
                using ( this._timer ) {
                    this._timer.Stop();
                }
            }
            switch ( granularity ) {
                case Granularity.Milliseconds:

		            // ReSharper disable once UseObjectOrCollectionInitializer
                    this._timer = new Timer( interval: ( Double )Milliseconds.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnMillisecondElapsed;
                    break;

                case Granularity.Seconds:

		            // ReSharper disable once UseObjectOrCollectionInitializer
                    this._timer = new Timer( interval: ( Double )Seconds.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnSecondElapsed;
                    break;

                case Granularity.Minutes:

		            // ReSharper disable once UseObjectOrCollectionInitializer
                    this._timer = new Timer( interval: ( Double )Minutes.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnMinuteElapsed;
                    break;

                case Granularity.Hours:

		            // ReSharper disable once UseObjectOrCollectionInitializer
                    this._timer = new Timer( interval: ( Double )Hours.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnHourElapsed;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( granularity ) );
            }

            this._timer.Start();
        }

        public Time Time() {
            try {
                this._timer?.Stop(); //stop the timer so the seconds don't tick while we get the values.
                return new Time( hour: this.Hour.Value, minute: this.Minute.Value, second: this.Second.Value );
            }
            finally {
                this._timer?.Start();
            }
        }

        private void OnHourElapsed( Object sender, ElapsedEventArgs e ) {

			this.Hour = this.Hour.Next( out var ticked );
			if ( !ticked ) {
                return;
            }
            this.OnHourTick?.Invoke( this.Hour );
        }

        private void OnMillisecondElapsed( Object sender, ElapsedEventArgs e ) {

			this.Millisecond = this.Millisecond.Next( out var ticked );
			if ( !ticked ) {
                return;
            }
            this.OnMillisecondTick?.Invoke();

            this.OnSecondElapsed( sender, e );
        }

        private void OnMinuteElapsed( Object sender, ElapsedEventArgs e ) {

			this.Minute = this.Minute.Next( out var ticked );
			if ( !ticked ) {
                return;
            }
            this.OnMinuteTick?.Invoke();

            this.OnHourElapsed( sender, e );
        }

        private void OnSecondElapsed( Object sender, ElapsedEventArgs e ) {

			this.Second = this.Second.Next( out var ticked );
			if ( !ticked ) {
                return;
            }
            this.OnSecondTick?.Invoke();

            this.OnMinuteElapsed( sender, e );
        }
    }
}