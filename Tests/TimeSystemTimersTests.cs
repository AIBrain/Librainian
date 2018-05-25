// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeSystemTimersTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/LibrainianTests/TimeSystemTimersTests.cs" was last formatted by Protiguous on 2018/05/24 at 7:37 PM.

namespace LibrainianTests {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using Librainian.Measurement.Time;
    using Librainian.Threading;
    using NUnit.Framework;
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