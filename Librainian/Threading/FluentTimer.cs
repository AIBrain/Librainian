// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FluentTimer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "FluentTimer.cs" was last formatted by Protiguous on 2020/03/18 at 10:30 AM.

namespace Librainian.Threading {

    using System;
    using System.Timers;
    using JetBrains.Annotations;
    using Measurement.Frequency;
    using Measurement.Time;

    public static class FluentTimer {

        /// <summary>Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.</summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        [NotNull]
        public static Timer AutoReset( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }

            timer.AutoReset = true;

            return timer;
        }

        /// <summary>
        ///     <para>Start the <paramref name="timer" />.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [NotNull]
        public static Timer Begin( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }

            timer.Start();

            return timer;
        }

        [NotNull]
        public static Timer Create( [CanBeNull] this Hertz frequency, [CanBeNull] Action onTick ) => Create( ( TimeSpan ) frequency, onTick );

        /// <summary>
        ///     <para>Creates, but does not start, the <see cref="Timer" />.</para>
        ///     <para>Defaults to a one-time <see cref="Timer.Elapsed" /></para>
        /// </summary>
        /// <param name="interval"> </param>
        /// <param name="onTick"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [NotNull]
        public static Timer Create( this TimeSpan interval, [CanBeNull] Action onTick ) {
            if ( interval < Milliseconds.One ) {
                interval = Milliseconds.One;
            }

            if ( onTick is null ) {
                onTick = () => { };
            }

            var mills = interval.TotalMilliseconds;

            if ( mills <= 0 ) {
                mills = 1;
            }

            var timer = new Timer( mills ) {
                AutoReset = false
            };

            timer.Elapsed += ( sender, args ) => {
                try {
                    timer.Stop();
                    onTick();
                }
                finally {
                    if ( timer.AutoReset ) {
                        timer.Start();
                    }
                }
            };

            return timer;
        }

        [NotNull]
        public static Timer End( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }

            timer.Stop();

            return timer;
        }

        /// <summary>
        ///     <para>Make the <paramref name="timer" /> fire only once.</para>
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        [NotNull]
        public static Timer Once( [NotNull] this Timer timer ) {
            if ( timer is null ) {
                throw new ArgumentNullException( nameof( timer ) );
            }

            timer.AutoReset = false;

            return timer;
        }

    }

}