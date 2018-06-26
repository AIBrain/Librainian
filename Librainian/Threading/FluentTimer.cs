// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "FluentTimer.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "FluentTimer.cs" was last formatted by Protiguous on 2018/06/26 at 1:42 AM.

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
		[NotNull]
		public static Timer AndStart( [NotNull] this Timer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer.Start();

			return timer;
		}

		/// <summary>
		///     Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		[NotNull]
		public static Timer AutoResetting( [NotNull] this Timer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer.AutoReset = true;

			return timer;
		}

		public static Timer CreateTimer( this Hertz frequency, Action onElapsed ) => CreateTimer( ( TimeSpan ) frequency, onElapsed );

		/// <summary>
		///     <para>Creates, but does not start, the <see cref="Timer" />.</para>
		///     <para>Defaults to a one-time <see cref="Timer.Elapsed" /></para>
		/// </summary>
		/// <param name="interval"> </param>
		/// <param name="onElapsed"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[NotNull]
		public static Timer CreateTimer( this TimeSpan interval, [CanBeNull] Action onElapsed ) {
			if ( interval < Milliseconds.One ) {
				interval = Milliseconds.One;
			}

			if ( null == onElapsed ) {
				onElapsed = () => { };
			}

			var mills = interval.TotalMilliseconds;
			mills.Should().BeGreaterThan( 0 );

			if ( mills <= 0 ) {
				mills = 1;
			}

			var timer = new Timer( interval: mills ) {
				AutoReset = false
			};

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
		public static void DoneWith( [CanBeNull] this Timer timer ) {
			if ( null == timer ) {
				return;
			}

			Timers.TryRemove( timer, out _ );

			using ( timer ) {
				timer.Stop();
			}
		}

		[NotNull]
		public static IEnumerable<KeyValuePair<Timer, DateTime>> GetTimers() => Timers;

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