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
// "Librainian/FluentTimer.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Timers;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Measurement.Frequency;
    using Measurement.Time;

    public static class FluentTimer {

        /// <summary>
        ///     Container to keep track of any created <see cref="Timer" /> and the <see cref="DateTime" />.
        /// </summary>
        [NotNull]
        private static ConcurrentDictionary<Timer, DateTime> Timers { get; } = new ConcurrentDictionary<Timer, DateTime>();

        /// <summary>
        ///     <para>Start the <paramref name="timer" />.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Timer AndStart( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }
            timer.Start();
            return timer;
        }

        /// <summary>Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.</summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static Timer AutoResetting( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }
            timer.AutoReset = true;
            return timer;
        }

        public static Timer CreateTimer( this Hertz frequency, Action onElapsed ) => CreateTimer( ( TimeSpan )frequency, onElapsed );

        /// <summary>
        ///     <para>Creates, but does not start, the <see cref="Timer" />.</para>
        ///     <para>Defaults to a one-time <see cref="Timer.Elapsed" /></para>
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="onElapsed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Timer CreateTimer( this TimeSpan interval, [CanBeNull] Action onElapsed ) {
            if ( interval < Milliseconds.One ) {
                interval = Milliseconds.One;
            }

            if ( null == onElapsed ) {
                onElapsed = () => {
                };
            }

            var mills = interval.TotalMilliseconds;
            mills.Should().BeGreaterThan( 0 );
            if ( mills <= 0 ) {
                mills = 1;
            }

            var timer = new Timer( interval: mills ) { AutoReset = false };
            timer.Should().NotBeNull();
            timer.Elapsed += ( sender, args ) => {
                try {
                    timer.Stop();
                    onElapsed();
                }
                finally {
                    if ( timer.AutoReset ) {
                        timer.Start();
                    }
                    else {
                        timer.DoneWith();
                    }
                }
            };
            Timers[ timer ] = DateTime.Now;
            return timer;
        }

        ///// <summary><see cref="TimeSpan" /> overload for <see cref="Create(Span,Action)" />.</summary>
        ///// <param name="interval"></param>
        ///// <param name="onElapsed"></param>
        ///// <returns></returns>
        //public static Timer Create(this TimeSpan interval, [CanBeNull] Action onElapsed) => Create( interval, onElapsed );
        public static void DoneWith( this Timer timer ) {
            if ( null == timer ) {
                return;
            }
			Timers.TryRemove( timer, out var value );
			using ( timer ) {
                timer.Stop();
            }
        }

        public static IEnumerable<KeyValuePair<Timer, DateTime>> GetTimers() => Timers;

	    /// <summary>
        ///     <para>Make the <paramref name="timer" /> fire only once.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static Timer Once( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }
            timer.AutoReset = false;
            return timer;
        }
    }
}