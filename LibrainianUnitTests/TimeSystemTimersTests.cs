// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeSystemTimersTests.cs" belongs to Protiguous@Protiguous.com and
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
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "LibrainianTests", "TimeSystemTimersTests.cs" was last formatted by Protiguous on 2019/03/17 at 11:07 AM.

namespace LibrainianUnitTests {

	using System;
	using System.Threading;
	using Xunit;

	public static class TimeSystemTimers {
		private static Int64 _threadingCounter;

		private static void Callback( Object state ) => Interlocked.Increment( ref _threadingCounter );

		/*
        public static UInt64 RunSystemTimerTest( TimeSpan howLong ) {
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
		*/

		[Fact]
		public static void RunTests() {
			//Console.WriteLine( RunSystemTimerTest( Milliseconds.One ) );
			//Console.WriteLine( RunThreadingTimerTest( Milliseconds.One ) );
		}

		/*
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
		*/
	}
}