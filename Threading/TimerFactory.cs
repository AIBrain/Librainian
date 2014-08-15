namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Timers;
    using Annotations;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Measurement.Time;

    public static class TimerFactory {
        /// <summary>
        /// Container to keep track of any created <see cref="Timer"/> and the <see cref="DateTime"/>.
        /// </summary>
        [NotNull] public static readonly ConcurrentDictionary<Timer, DateTime> Timers = new ConcurrentDictionary<Timer, DateTime>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="onEachInterval"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Timer Create( this Span interval, Action onEachInterval ) {
            if ( null == onEachInterval ) {
                onEachInterval = () => { };
            }
            var eachInterval = onEachInterval;
            var timer = new Timer( interval: interval.Milliseconds );
            timer.Elapsed += ( sender, args ) => eachInterval();
            timer.Should().NotBeNull();
            Timers[ timer ] = DateTime.Now;
            return timer;
        }

        public static void DoneWith( this Timer timer ) {
            if ( null == timer ) {
                return;
            }
            DateTime value;
            Timers.TryRemove( timer , out value);
            using ( timer ) {
                timer.Stop();
            }
        }
    }
}