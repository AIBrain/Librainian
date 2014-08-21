
namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using Maths;
    using NUnit.Framework;

    public class TimeTimers {

        public System.Threading.Timer ThreadingTimer;

        public TimeTimers() {
            var state = new Object();
            this.ThreadingTimer = new System.Threading.Timer( Callback, state, 0, 0 );

        }

        private static void Callback( object state ) {
            throw new NotImplementedException();
        }

        public void RunTests() {
            RunSystemTimerTest( Milliseconds.Five );
        }

        [Test]
        public static ulong RunSystemTimerTest( Span howLong ) {
            var counter = 0UL;
            try {
                var systemTimer = new System.Timers.Timer();
                systemTimer.Stop();    //jic
                systemTimer.Interval = Double.Epsilon;
                systemTimer.Elapsed += ( sender, args ) => { counter++; };
                systemTimer.AutoReset = false;
                var stopwatch = Stopwatch.StartNew();
                systemTimer.Start();
                while ( stopwatch.Elapsed < howLong ) {
                    ;
                }
                stopwatch.Stop();
                systemTimer.Stop();
                var perMillisecond = counter / howLong.GetApproximateMilliseconds();
                Debug.WriteLine( "System.Timer.TimerTest counted {0} in {1} ({2})", counter, howLong, perMillisecond );
            }
            catch ( Exception ) {
            }
            return counter;
        }
    }
}
