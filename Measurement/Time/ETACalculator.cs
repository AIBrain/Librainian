#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ETACalculator.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Annotations;
    using Maths;
    using Timer = System.Timers.Timer;

    /// <summary>
    ///     <para>Calculates the "Estimated Time of Arrival", aka ETA</para>
    /// </summary>
    [UsedImplicitly]
    public class ETACalculator {

        /// <summary>
        ///     At these points in time, how far along have we progressed?
        /// </summary>
        private readonly ConcurrentDictionary<TimeSpan, Single> _datapoints = new ConcurrentDictionary<TimeSpan, Single>();

        /// <summary>
        ///     Start our timer so we can keep track of elapsed time.
        /// </summary>
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private Single _progress;
        private Timer _timer;

        public ETACalculator() {
            this.Reset( Seconds.One );
        }

        /// <summary>
        ///     <para>The value to be updated to a value between 0 and 1 when possible.</para>
        /// </summary>
        public Single Progress {
            get {
                return Thread.VolatileRead( ref this._progress );
            }

            set {
                Thread.VolatileWrite( ref this._progress, value );
            }
        }

        /// <summary>
        ///     <para>Returns True when there is enough data to calculate the ETA.</para>
        ///     <para>Returns False if the ETA is still calculating.</para>
        /// </summary>
        public Boolean DoWeHaveAnETA() {
            return this._datapoints.Any();
        }

        /// <summary>
        ///     <para>Calculates the Estimated Time of Completion</para>
        /// </summary>
        public DateTime ETA() {
            return DateTime.Now + this.ETR();
        }

        /// <summary>
        ///     <para>Calculates the Estimated Time Remaining</para>
        /// </summary>
        public TimeSpan ETR() {
            if ( !this.DoWeHaveAnETA() ) {
                return TimeSpan.MaxValue;
            }

            var estimateTimeRemaing = TimeSpan.MaxValue; //assume forever

            //var datapoints = this.GetDataPoints().OrderBy( pair => pair.Key ).ToList();
            //var datapointCount = datapoints.Count;

            //var timeActuallyTakenSoFar = TimeSpan.Zero;

            //foreach ( var dataPoint in datapoints ) {
            //    var timePassed = dataPoint.Key;
            //    var progress = dataPoint.Value;
            //}

            var datapoints = this.GetDataPoints().ToList();

            var intercept = datapoints.Intercept();

            estimateTimeRemaing += TimeSpan.FromMilliseconds( intercept );

            return estimateTimeRemaing;
        }

        /// <summary>
        ///     Get the internal data points we have so far.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TimeProgression> GetDataPoints() {
            return this._datapoints.OrderBy( pair => pair.Key ).Select( pair => new TimeProgression {
                MillisecondsPassed = pair.Key.TotalMilliseconds,
                Progress = pair.Value
            } );
        }

        public void Reset( TimeSpan samplingPeriod ) {
            this._timer.Close();
            this._stopwatch.Stop();
            this._datapoints.Clear();
            this._stopwatch.Reset();
            this._stopwatch.Start();
            this.Progress = 0;
            this._timer = new Timer {
                Interval = samplingPeriod.TotalMilliseconds,
                AutoReset = true
            };
            this._timer.Elapsed += ( sender, args ) => this.Update();
            this._timer.Start();
        }

        /// <summary>
        ///     <para>Manually add the known <see cref="Progress" /> to the internal data points.</para>
        /// </summary>
        public void Update() {
            if ( this.Progress >= 0 && this.Progress <= 1 && !Single.IsNaN( this.Progress ) ) {
                this._datapoints.TryAdd( this._stopwatch.Elapsed, this.Progress );
            }

            //throw new ArgumentOutOfRangeException( "Progress", "The Progress is out of the range 0 to 1." );
        }
    }
}