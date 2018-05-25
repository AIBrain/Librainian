// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "GameTimer.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/GameTimer.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Threading;
    using System.Timers;
    using Frequency;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Timer = System.Timers.Timer;

    /// <summary>
    ///     A timer that fires at 60fps and Reports() back.
    /// </summary>
    [JsonObject]
    public class GameTimer {

        [JsonObject]
        public struct ReportBack {

            [JsonProperty]
            public UInt64 Counter { get; set; }

            [JsonProperty]
            public TimeSpan Elapsed { get; set; }

            [JsonProperty]
            public Boolean RunningSlow { get; set; }
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        private UInt64 _counter;

        /// <summary>
        /// </summary>
        [JsonProperty]
        private volatile Boolean _isPaused;

        /// <summary>
        /// </summary>
        [JsonProperty]
        private TimeSpan _lastElapsed = TimeSpan.Zero;

        [JsonProperty]
        private DateTime _lastUpdate = DateTime.UtcNow;

        private DateTime LastProgressReport {
            get {
                try { return this._lastUpdate; }
                finally { Thread.MemoryBarrier(); }
            }

            set {
                try { Thread.MemoryBarrier(); }
                finally { this._lastUpdate = value; }
            }
        }

        [NotNull]
        private IProgress<ReportBack> Progress { get; }

        /// <summary>
        /// </summary>
        [NotNull]
        private Timer Timer { get; }

        private Double UpdateRate { get; } = ( Double )Fps.Sixty.Value;

        public UInt64 Counter {
            get => Thread.VolatileRead( ref this._counter );

            private set => Thread.VolatileWrite( ref this._counter, value );
        }

        /// <summary>
        ///     Time since last tick().
        /// </summary>
        /// <value></value>
        public TimeSpan Elapsed {
            get {
                this.LastElapsed = DateTime.UtcNow - this.LastProgressReport;

                return this.LastElapsed;
            }
        }

        public Boolean IsPaused {
            get => this._isPaused;

            private set => this._isPaused = value;
        }

        /// <summary>
        ///     A copy of the most recent <see cref="Elapsed" />.
        /// </summary>
        public TimeSpan LastElapsed {
            get {
                try { return this._lastElapsed; }
                finally { Thread.MemoryBarrier(); }
            }

            private set {
                try { Thread.MemoryBarrier(); }
                finally { this._lastElapsed = value; }
            }
        }

        public GameTimer( [NotNull] IProgress<ReportBack> progress ) {
            this.Progress = progress ?? throw new ArgumentNullException( nameof( progress ), "Progress must not be null." );

            // ReSharper disable once UseObjectOrCollectionInitializer
            this.Timer = new Timer( interval: this.UpdateRate ) { AutoReset = false };
            this.Timer.Elapsed += this.OnTimerElapsed;
            this.Resume();
        }

        private void OnTimerElapsed( Object sender, ElapsedEventArgs elapsedEventArgs ) {
            try {
                this.Pause();
                this.Counter++;
                this.Progress.Report( new ReportBack { Counter = this.Counter, Elapsed = this.Elapsed, RunningSlow = this.IsRunningSlow() } );
            }
            catch ( Exception exception ) { exception.More(); }
            finally {
                this.LastProgressReport = DateTime.UtcNow;
                this.Resume();
            }
        }

        public Boolean IsRunningSlow() => this.LastElapsed.TotalMilliseconds > 70;

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
        ///     Total time passed since timer was started.
        /// </summary>
        /// <returns></returns>
        public Span TotalElapsed() => new Span( milliseconds: this.Counter / this.UpdateRate );
    }
}