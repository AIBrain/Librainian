// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ETACalculator.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "ETACalculator.cs" was last formatted by Protiguous on 2019/08/08 at 9:01 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Timers;
    using FluentTime;
    using JetBrains.Annotations;
    using Magic;
    using Maths;

    /// <summary>
    ///     <para>Calculates the "Estimated Time of Arrival", aka ETA</para>
    /// </summary>
    public class EtaCalculator : ABetterClassDispose {

        /// <summary>
        ///     At these points in time, how far along have we progressed?
        /// </summary>
        private readonly ConcurrentDictionary<TimeSpan, Single> _datapoints = new ConcurrentDictionary<TimeSpan, Single>();

        /// <summary>
        ///     Start our timer so we can keep track of elapsed time.
        /// </summary>
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private volatile Single _progress;

        [CanBeNull]
        private Timer _timer;

        /// <summary>
        ///     <para>The value to be updated to a value between 0 and 1 when possible.</para>
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

        public EtaCalculator() => this.Reset( Seconds.One );

        /// <summary>
        ///     <para>Returns True when there is enough data to calculate the ETA.</para>
        ///     <para>Returns False if the ETA is still calculating.</para>
        /// </summary>
        public Boolean DoWeHaveAnEta() => this._datapoints.Any();

        /// <summary>
        ///     <para>Calculates the Estimated Time of Completion</para>
        /// </summary>
        public DateTime Eta() => DateTime.Now + this.Etr();

        /// <summary>
        ///     Get the internal data points we have so far.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<TimeProgression> GetDataPoints() =>
            this._datapoints.OrderBy( pair => pair.Key ).Select( pair => new TimeProgression {
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
            this._timer = new Timer {
                Interval = samplingPeriod.TotalMilliseconds,
                AutoReset = true
            };

            this._timer.Elapsed += ( sender, args ) => this.Update();
            this._timer?.Start();
        }

        /// <summary>
        ///     <para>Manually add the known <see cref="Progress" /> to the internal data points.</para>
        /// </summary>
        public void Update() {
            if ( this.Progress >= 0 && this.Progress <= 1 && !this.Progress.IsNumber() ) {
                this._datapoints.TryAdd( this._stopwatch.Elapsed, this.Progress );
            }

            //throw new ArgumentOutOfRangeException( "Progress", "The Progress is out of the range 0 to 1." );
        }

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() {
            using ( this._timer ) {
                
            }
        }

    }
}