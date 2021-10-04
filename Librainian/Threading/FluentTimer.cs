// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "FluentTimer.cs" last touched on 2021-04-25 at 12:17 PM by Protiguous.

#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Timers;
	using Exceptions;
	using Measurement.Frequency;
	using Measurement.Time;
	using Utilities.Disposables;

	public static class FluentTimerExt {

		/// <summary>
		///     <para>Start the <paramref name="timer" />.</para>
		///     <para>Same as <see cref="Begin" />.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static FluentTimer AndStart( this FluentTimer timer ) => timer.Begin();

		/// <summary>Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.</summary>
		/// <param name="timer"></param>
		/// <param name="set"></param>
		public static FluentTimer AutoReset( this FluentTimer timer, Boolean set = true ) {
			if ( timer is null ) {
				throw new ArgumentEmptyException( nameof( timer ) );
			}

			timer.Timer.AutoReset = set;

			return timer;
		}

		/// <summary>
		///     <para>Start the <paramref name="timer" />.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static FluentTimer Begin( this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentEmptyException( nameof( timer ) );
			}

			timer.Timer.Start();

			return timer;
		}


		public static FluentTimer End( this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentEmptyException( nameof( timer ) );
			}

			timer.Timer.Stop();

			return timer;
		}

		public static FluentTimer Once( this FluentTimer timer ) {
			timer.Timer.AutoReset = false;

			return timer;
		}

		/// <summary>
		///     <para>Start the <paramref name="timer" />.</para>
		/// </summary>
		/// <param name="timer"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static FluentTimer Start( this FluentTimer timer ) => timer.Begin();

		public static FluentTimer Stop( this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentEmptyException( nameof( timer ) );
			}

			timer.Timer.Stop();

			return timer;
		}
	}

	public class FluentTimer : ABetterClassDispose {


		public static FluentTimer Create( Hertz frequency, Action onTick ) => Create( ( TimeSpan )frequency, onTick );

		/// <summary>
		///     <para>Creates, but does not start, the <see cref="Timer" />.</para>
		///     <para>Defaults to a one-time tick.</para>
		/// </summary>
		/// <param name="interval"> </param>
		/// <param name="onTick"></param>
		/// <exception cref="ArgumentException"></exception>
		public static FluentTimer Create( TimeSpan interval, Action? onTick = null ) {
			if ( interval < Milliseconds.One ) {
				interval = Milliseconds.One;
			}

			var milliseconds = interval.TotalMilliseconds;

			if ( milliseconds <= 0 ) {
				milliseconds = 1;
			}

			var create = new FluentTimer( milliseconds ).Once();

			create.Timer.Elapsed += ( sender, args ) => {
				try {
					//create.Timer.Stop();
					onTick?.Invoke();
				}
				finally {
					//if ( create.Timer.AutoReset ) {
					//	_ = create.Start();
					//}
				}
			};

			return create;
		}

		internal Timer Timer { get; }

		/// <summary>
		///     Defaults to 1 millisecond.
		/// </summary>
		public FluentTimer() : this( Milliseconds.One ) { }

		public FluentTimer( Double milliseconds ) : this( new Milliseconds( ( Decimal )milliseconds ) ) { }

		public FluentTimer( IQuantityOfTime quantityOfTime ) : base( nameof( FluentTimer ) ) {
			if ( quantityOfTime == null ) {
				throw new ArgumentEmptyException( nameof( quantityOfTime ) );
			}

			this.Timer = new Timer( quantityOfTime.ToTimeSpan().TotalMilliseconds );
		}

		public override void DisposeManaged() {
			using ( this.Timer ) {
				this.Timer.Stop();
			}

			base.DisposeManaged();
		}
	}
}