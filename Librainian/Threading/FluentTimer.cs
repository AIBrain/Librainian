﻿// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FluentTimer.cs" last formatted on 2020-08-21 at 10:48 AM.

#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Timers;
	using JetBrains.Annotations;
	using Measurement.Frequency;
	using Measurement.Time;

	public static class FluentTimerExt {

		/// <summary>
		///     <para>Start the <paramref name="timer" />.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[NotNull]
		public static FluentTimer AndStart( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer._timer.Start();

			return timer;
		}

		/// <summary>Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.</summary>
		/// <param name="timer"></param>
		/// <param name="set"></param>
		/// <returns></returns>
		[NotNull]
		public static FluentTimer AutoReset( [NotNull] this FluentTimer timer, Boolean set = true ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer._timer.AutoReset = set;

			return timer;
		}

		/// <summary>
		///     <para>Start the <paramref name="timer" />.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[NotNull]
		public static FluentTimer Begin( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer._timer.Start();

			return timer;
		}

		[NotNull]
		public static FluentTimer CreateTimer( [NotNull] this Hertz frequency, [NotNull] Action onTick ) => CreateTimer( ( TimeSpan )frequency, onTick );

		/// <summary>
		///     <para>Creates, but does not start, the <see cref="Timer" />.</para>
		///     <para>Defaults to a one-time <see cref="Timer.Elapsed" /></para>
		/// </summary>
		/// <param name="interval"> </param>
		/// <param name="onTick"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[NotNull]
		public static FluentTimer CreateTimer( this TimeSpan interval, [CanBeNull] Action? onTick = null ) {
			if ( interval < Milliseconds.One ) {
				interval = Milliseconds.One;
			}

			var mills = interval.TotalMilliseconds;

			if ( mills <= 0 ) {
				mills = 1;
			}

			var create = new FluentTimer( mills ).Once();

			create._timer.Elapsed += ( sender, args ) => {
				try {
					create._timer.Stop();
					onTick?.Invoke();
				}
				finally {
					if ( create._timer.AutoReset ) {
						create.Start();
					}
				}
			};

			return create;
		}

		[NotNull]
		public static FluentTimer End( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer._timer.Stop();

			return timer;
		}

		/*
		/// <summary>
		///     <para>Make the <paramref name="timer" /> fire only once.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		[NotNull]
		public static Timer Once( [NotNull] this Timer timer ) {
			timer.AutoReset = false;

			return timer;
		}
		*/

		[NotNull]
		public static FluentTimer Once( [NotNull] this FluentTimer timer ) {
			timer._timer.AutoReset = false;

			return timer;
		}

		/// <summary>
		///     <para>Start the <paramref name="timer" />.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[NotNull]
		public static FluentTimer Start( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer._timer.Start();

			return timer;
		}

		[NotNull]
		public static FluentTimer Stop( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer._timer.Stop();

			return timer;
		}

	}

	public class FluentTimer  {

		internal Timer _timer { get; }

		/// <summary>
		///     Defaults to 1 millisecond.
		/// </summary>
		public FluentTimer() : this( Milliseconds.One ) { }

		public FluentTimer( Double milliseconds ) : this( new Milliseconds( milliseconds ) ) { }

		public FluentTimer( [NotNull] IQuantityOfTime quantityOfTime ) {
			if ( quantityOfTime == null ) {
				throw new ArgumentNullException( nameof( quantityOfTime ) );
			}

			this._timer = new Timer( quantityOfTime.ToTimeSpan().TotalMilliseconds );
		}

	}

}