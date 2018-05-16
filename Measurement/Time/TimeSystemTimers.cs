// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TimeSystemTimers.cs",
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
// "Librainian/Librainian/TimeSystemTimers.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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

        private static void Callback( Object state ) => Interlocked.Increment( ref _threadingCounter );

        public static UInt64 RunSystemTimerTest( Span howLong ) {
            var counter = 0UL;

            try {
                using ( var systemTimer = new Timer( ( Double )Milliseconds.One ) { AutoReset = true } ) {
                    systemTimer.Elapsed += ( sender, args ) => counter++;

                    systemTimer.Start();
                    var stopwatch = Stopwatch.StartNew();

                    while ( stopwatch.Elapsed < howLong ) { ThreadingExtensions.DoNothing(); }

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

                    while ( stopwatch.Elapsed < howLong ) { ThreadingExtensions.DoNothing(); }

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
    }
}