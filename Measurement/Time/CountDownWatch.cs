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
// "Librainian/CountDownWatch.cs" was last cleaned by Rick on 2016/07/17 at 9:43 PM

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
            var watch = new CountDownWatch( Seconds.Three, () => {
                "Launched!".Info();
            } );
            watch.Start();
            do {
                $"{watch.Remaining().Simpler()}".Info();
                Thread.Sleep( 333 );
            } while ( !watch.HasLaunched() );

            watch.Remaining()
                 .Should()
                 .BeLessThan( Seconds.One );

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

        /// <summary>
        /// </summary>
        /// <param name="countdown"></param>
        /// <param name="liftoff">Action to invoke when countdown reaches zero.</param>
        public CountDownWatch( TimeSpan countdown, Action liftoff = null ) {
            if ( countdown < TimeSpan.Zero ) {
                throw new ArgumentOutOfRangeException( nameof( countdown ), "Must be a positive value." );
            }
            this.Countdown = countdown;
            this.TargetTime = DateTime.UtcNow.Add( this.Countdown );
            this.Liftoff = () => {
                try {
                    this._hasLaunched = true;
                    liftoff?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.More();
                }
            };
        }

        public TimeSpan Countdown {
            get;
        }

        public DateTime TargetTime {
            get; private set;
        }

        public DateTime WhenStarted {
            get; private set;
        }

        public DateTime WhenStopped {
            get; private set;
        }

        [NotNull]
        private Action Liftoff {
            get;
        }

        private Timer Timer {
            get; set;
        }

        public Boolean HasLaunched() {
            return this._hasLaunched;
        }

        public Boolean IsRunning() {
            return this._isRunning;
        }

        public TimeSpan Remaining() {
            if ( this.IsRunning() ) {
                return this.Countdown.Subtract( DateTime.UtcNow - this.WhenStarted );
            }
            if ( this.HasLaunched() ) {
                return this.Countdown.Subtract( this.WhenStopped - this.WhenStarted );
            }
            throw new InvalidOperationException( "???" );
        }

        public void Start() {
            this.WhenStarted = DateTime.UtcNow;
            this._isRunning = true;
            this.Timer = this.Countdown.CreateTimer( () => {
                this.Stop();
                this.Liftoff();
            } )
                             .Once()
                             .AndStart();
        }

        public void Stop() {
            this._isRunning = false;
            this.WhenStopped = DateTime.UtcNow;
            this.Timer.Stop();
        }
    }
}