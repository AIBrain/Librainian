namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Timers;
    using Annotations;
    using FluentAssertions;
    using Measurement.Time;

    public static class FluentTimers {
        /// <summary>
        /// Container to keep track of any created <see cref="Timer"/> and the <see cref="DateTime"/>.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Timer, DateTime> Timers = new ConcurrentDictionary<Timer, DateTime>();

        /// <summary>
        /// <para>Creates, but does not start, the <see cref="Timer"/>.</para>
        /// <para>Defaults to a one-time <see cref="Timer.Elapsed"/></para>
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="onElapsed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        // ReSharper disable once HeapView.ClosureAllocation
        public static Timer Create( this Span interval, [CanBeNull] Action onElapsed ) {
            if ( null == onElapsed ) {
                onElapsed = () => { };
            }
            var timer = new Timer( interval: ( Double )interval.Milliseconds ) {
                AutoReset = false
            };
            timer.Should().NotBeNull();
            timer.Elapsed += ( sender, args ) => {
                try {
                    onElapsed();
                }
                finally {
                    if ( !timer.AutoReset ) {
                        timer.DoneWith();    
                    }
                }

            };
            Timers[ timer ] = DateTime.Now;
            return timer;
        }

        public static void DoneWith( this Timer timer ) {
            if ( null == timer ) {
                return;
            }
            DateTime value;
            Timers.TryRemove( timer, out value );
            using ( timer ) {
                timer.Stop();
            }
        }

        /// <summary>
        /// <para>Start the <paramref name="timer"/>.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Timer AndStart( [NotNull] this Timer timer ) {
            if ( timer == null ) {
                throw new ArgumentNullException( "timer" );
            }
            timer.Start();
            return timer;
        }

        /// <summary>
        /// <para>Make the <paramref name="timer"/> fire only once.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static Timer Once( [NotNull] this Timer timer ) {
            if ( timer == null ) {
                throw new ArgumentNullException( "timer" );
            }
            timer.AutoReset = false;
            return timer;
        }

        /// <summary>
        /// Make the <paramref name="timer"/> fire every <see cref="Timer.Interval"/>.
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static Timer AutoResetting( [NotNull] this Timer timer ) {
            if ( timer == null ) {
                throw new ArgumentNullException( "timer" );
            }
            timer.AutoReset = true;
            return timer;
        }
    }
}