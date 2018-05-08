// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks goes
// to the Authors.
//
// Donations and royalties can be paid via
// 
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// 
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ETACalculator.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Timers;
    using FluentTime;
    using JetBrains.Annotations;
    using Maths;

    /// <summary>
    /// <para>Calculates the "Estimated Time of Arrival", aka ETA</para>
    /// </summary>
    public class EtaCalculator {

        /// <summary>
        /// At these points in time, how far along have we progressed?
        /// </summary>
        private readonly ConcurrentDictionary<TimeSpan, Single> _datapoints = new ConcurrentDictionary<TimeSpan, Single>();

        /// <summary>
        /// Start our timer so we can keep track of elapsed time.
        /// </summary>
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private volatile Single _progress;

        [CanBeNull]
        private Timer _timer;

        public EtaCalculator() => this.Reset( Seconds.One );

        /// <summary>
        /// <para>The value to be updated to a value between 0 and 1 when possible.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Single Progress {
            get => this._progress;

            set {
                if ( !value.IsNumber() ) {
                    throw new InvalidOperationException();
                }
                if ( value < 0 || value > 1 ) {
                    throw new ArgumentOutOfRangeException( nameof( this.Progress ), $"{value:R} is out of the range 0 to 1." );
                }
                this._progress = value;
            }
        }

        /// <summary>
        /// <para>Returns True when there is enough data to calculate the ETA.</para>
        /// <para>Returns False if the ETA is still calculating.</para>
        /// </summary>
        public Boolean DoWeHaveAnEta() => this._datapoints.Any();

        /// <summary>
        /// <para>Calculates the Estimated Time of Completion</para>
        /// </summary>
        public DateTime Eta() => DateTime.Now + this.Etr();

        /// <summary>
        /// Get the internal data points we have so far.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TimeProgression> GetDataPoints() => this._datapoints.OrderBy( pair => pair.Key ).Select( pair => new TimeProgression {
            MillisecondsPassed = pair.Key.TotalMilliseconds,
            Progress = pair.Value
        } );

        public void Reset( TimeSpan samplingPeriod ) {
            using ( this._timer ) {

                //TODO what happens if this is null?
                //this._timer?.Close();
            }
            this._stopwatch.Stop();
            this._datapoints.Clear();
            this._stopwatch.Reset();
            this._stopwatch.Start();
            this.Progress = 0;

            // ReSharper disable once UseObjectOrCollectionInitializer
            this._timer = new Timer { Interval = samplingPeriod.TotalMilliseconds, AutoReset = true };
            this._timer.Elapsed += ( sender, args ) => this.Update();
            this._timer?.Start();
        }

        /// <summary>
        /// <para>Manually add the known <see cref="Progress"/> to the internal data points.</para>
        /// </summary>
        public void Update() {
            if ( this.Progress >= 0 && this.Progress <= 1 && !this.Progress.IsNumber() ) {
                this._datapoints.TryAdd( this._stopwatch.Elapsed, this.Progress );
            }

            //throw new ArgumentOutOfRangeException( "Progress", "The Progress is out of the range 0 to 1." );
        }
    }
}
