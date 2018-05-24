// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "CountDownWatch.cs",
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
// "Librainian/Librainian/CountDownWatch.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Threading;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Threading;
    using Timer = System.Timers.Timer;

    [TestFixture]
    public static class TestCoundownWatch {

        [Test]
        public static void TestCountdown() {
            var watch = new CountDownWatch( Seconds.Three, () => "Launched!".Info() );
            watch.Start();

            do {
                $"{watch.Remaining().Simpler()}".Info();
                Thread.Sleep( 333 );
            } while ( !watch.HasLaunched() );

            watch.Remaining().Should().BeLessThan( Seconds.One );

            $"{watch.Remaining().Simpler()}".Info();
            $"{watch.Remaining().Simpler()}".Info();
            $"{watch.Remaining().Simpler()}".Info();
        }
    }

    /// <summary>
    ///     The 'reverse' of the <see cref="StopWatch" /> class.
    ///     //TODO needs unit testing.
    /// </summary>
    [JsonObject( MemberSerialization.Fields )]
    public class CountDownWatch {

        private volatile Boolean _hasLaunched;

        private volatile Boolean _isRunning;

        [NotNull]
        private Action Liftoff { get; }

        private Timer Timer { get; set; }

        public TimeSpan Countdown { get; }

        public DateTime TargetTime { get; private set; }

        public DateTime WhenStarted { get; private set; }

        public DateTime WhenStopped { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="countdown"></param>
        /// <param name="liftoff">Action to invoke when countdown reaches zero.</param>
        public CountDownWatch( TimeSpan countdown, Action liftoff = null ) {
            if ( countdown < TimeSpan.Zero ) { throw new ArgumentOutOfRangeException( nameof( countdown ), "Must be a positive value." ); }

            this.Countdown = countdown;
            this.TargetTime = DateTime.UtcNow.Add( this.Countdown );

            this.Liftoff = () => {
                try {
                    this._hasLaunched = true;
                    liftoff?.Invoke();
                }
                catch ( Exception exception ) { exception.More(); }
            };
        }

        public Boolean HasLaunched() => this._hasLaunched;

        public Boolean IsRunning() => this._isRunning;

        public TimeSpan Remaining() {
            if ( this.IsRunning() ) { return this.Countdown.Subtract( DateTime.UtcNow - this.WhenStarted ); }

            if ( this.HasLaunched() ) { return this.Countdown.Subtract( this.WhenStopped - this.WhenStarted ); }

            throw new InvalidOperationException( "???" );
        }

        public void Start() {
            this.WhenStarted = DateTime.UtcNow;
            this._isRunning = true;

            this.Timer = this.Countdown.CreateTimer( () => {
                this.Stop();
                this.Liftoff();
            } ).Once().AndStart();
        }

        public void Stop() {
            this._isRunning = false;
            this.WhenStopped = DateTime.UtcNow;
            this.Timer.Stop();
        }
    }
}