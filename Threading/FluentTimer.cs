// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FluentTimer.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/FluentTimer.cs" was last formatted by Protiguous on 2018/05/24 at 7:33 PM.

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
        /// <remarks>Or do Timers hold their own reference?</remarks>
        [NotNull]
        private static ConcurrentDictionary<Timer, DateTime> Timers { get; } = new ConcurrentDictionary<Timer, DateTime>();

        /// <summary>
        ///     <para>Start the <paramref name="timer" />.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Timer AndStart( [NotNull] this Timer timer ) {
            if ( timer is null ) { throw new ArgumentNullException( nameof( timer ) ); }

            timer.Start();

            return timer;
        }

        /// <summary>
        ///     Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static Timer AutoResetting( [NotNull] this Timer timer ) {
            if ( timer is null ) { throw new ArgumentNullException( nameof( timer ) ); }

            timer.AutoReset = true;

            return timer;
        }

        public static Timer CreateTimer( this Hertz frequency, Action onElapsed ) => CreateTimer( ( TimeSpan )frequency, onElapsed );

        /// <summary>
        ///     <para>Creates, but does not start, the <see cref="Timer" />.</para>
        ///     <para>Defaults to a one-time <see cref="Timer.Elapsed" /></para>
        /// </summary>
        /// <param name="interval"> </param>
        /// <param name="onElapsed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Timer CreateTimer( this TimeSpan interval, [CanBeNull] Action onElapsed ) {
            if ( interval < Milliseconds.One ) { interval = Milliseconds.One; }

            if ( null == onElapsed ) { onElapsed = () => { }; }

            var mills = interval.TotalMilliseconds;
            mills.Should().BeGreaterThan( 0 );

            if ( mills <= 0 ) { mills = 1; }

            var timer = new Timer( interval: mills ) { AutoReset = false };
            timer.Should().NotBeNull();

            timer.Elapsed += ( sender, args ) => {
                try {
                    timer.Stop();
                    onElapsed();
                }
                finally {
                    if ( timer.AutoReset ) { timer.Start(); }
                    else { timer.DoneWith(); }
                }
            };

            Timers[timer] = DateTime.Now;

            return timer;
        }

        ///// <summary><see cref="TimeSpan" /> overload for <see cref="Create(Span,Action)" />.</summary>
        ///// <param name="interval"></param>
        ///// <param name="onElapsed"></param>
        ///// <returns></returns>
        //public static Timer Create(this TimeSpan interval, [CanBeNull] Action onElapsed) => Create( interval, onElapsed );
        public static void DoneWith( this Timer timer ) {
            if ( null == timer ) { return; }

            Timers.TryRemove( timer, out _ );

            using ( timer ) { timer.Stop(); }
        }

        public static IEnumerable<KeyValuePair<Timer, DateTime>> GetTimers() => Timers;

        /// <summary>
        ///     <para>Make the <paramref name="timer" /> fire only once.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static Timer Once( [NotNull] this Timer timer ) {
            if ( timer is null ) { throw new ArgumentNullException( nameof( timer ) ); }

            timer.AutoReset = false;

            return timer;
        }
    }
}