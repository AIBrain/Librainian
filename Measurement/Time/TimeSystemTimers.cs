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
// "Librainian/TimeSystemTimers.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;
    using Threading;
    using Timer = System.Timers.Timer;

    [TestFixture]
    public static class TimeSystemTimers {
        private static Int64 _threadingCounter;

        public static UInt64 RunSystemTimerTest( Span howLong ) {
            var counter = 0UL;
            try {
                using ( var systemTimer = new Timer( ( Double )Milliseconds.One ) { AutoReset = true } ) {
                    systemTimer.Elapsed += ( sender, args ) => counter++;

                    systemTimer.Start();
                    var stopwatch = Stopwatch.StartNew();
                    while ( stopwatch.Elapsed < howLong ) {
                        ThreadingExtensions.DoNothing();
                    }

                    stopwatch.Stop();
                    systemTimer.Stop();

                    var mills = howLong.GetApproximateMilliseconds();
                    var millsPer = mills / counter;
                    Debug.WriteLine( $"System.Timer.TimerTest counted to {counter} in {howLong} ({millsPer})" );
                }
            }
            catch ( Exception ) {
                // ignored
            }

            return counter;
        }

        [Test]
        public static void RunTests() {
            Console.WriteLine( RunSystemTimerTest( Milliseconds.One ) );
            Console.WriteLine( RunThreadingTimerTest( Milliseconds.One ) );
        }

        public static UInt64 RunThreadingTimerTest( Span howLong ) {
            _threadingCounter = 0;
            try {
                var state = new Object();
                using ( var threadingTimer = new System.Threading.Timer( callback: Callback, state: state, dueTime: ( Int32 )Milliseconds.One.Value, period: ( Int32 )Milliseconds.One.Value ) ) {
                    var stopwatch = Stopwatch.StartNew();
                    while ( stopwatch.Elapsed < howLong ) {
                        ThreadingExtensions.DoNothing();
                    }
                    stopwatch.Stop();

                    var mills = howLong.GetApproximateMilliseconds();
                    var millsPer = mills / _threadingCounter;
                    Debug.WriteLine( $"System.Threading.TimerTest counted to {_threadingCounter} in {howLong} ({millsPer})" );
                }
            }
            catch {
                // ignored
            }

            return ( UInt64 )_threadingCounter;
        }

        private static void Callback( Object state ) => Interlocked.Increment( ref _threadingCounter );
    }
}