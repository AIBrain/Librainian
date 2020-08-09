#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Timers;
	using JetBrains.Annotations;
	using Measurement.Frequency;
	using Measurement.Time;

	public static class FluentTimerExt {

		/// <summary>Make the <paramref name="timer" /> fire every <see cref="Timer.Interval" />.</summary>
		/// <param name="timer"></param>
		/// <returns></returns>
		[NotNull]
		public static FluentTimer AutoReset( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer.AutoReset = true;

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
		public static FluentTimer CreateTimer( this TimeSpan interval, [CanBeNull] Action onTick ) {
			if ( interval < Milliseconds.One ) {
				interval = Milliseconds.One;
			}

			var mills = interval.TotalMilliseconds;

			if ( mills <= 0 ) {
				mills = 1;
			}

			var create = new FluentTimer( mills ) {
				AutoReset = false
			};

			create.Elapsed += ( sender, args ) => {
				try {
					create.Stop();
					onTick();
				}
				finally {
					if ( create.AutoReset ) {
						create.Start();
					}
				}
			};

			return create;
		}

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

		[NotNull]
		public static FluentTimer Once( [NotNull] this FluentTimer timer ) {
			timer.AutoReset = false;

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

			timer.Start();

			return timer;
		}

		[NotNull]
		public static FluentTimer Stop( [NotNull] this FluentTimer timer ) {
			if ( timer is null ) {
				throw new ArgumentNullException( nameof( timer ) );
			}

			timer.Stop();

			return timer;
		}

	}

	public class FluentTimer : Timer {

		/// <summary>
		///     Defaults to 1 millisecond.
		/// </summary>
		public FluentTimer() : this( Milliseconds.One ) { }

		public FluentTimer( Double milliseconds ) : this( new Milliseconds( milliseconds ) ) { }

		public FluentTimer( [NotNull] IQuantityOfTime quantityOfTime ) {
			if ( quantityOfTime == null ) {
				throw new ArgumentNullException( nameof( quantityOfTime ) );
			}

			this.Interval = quantityOfTime.ToTimeSpan().TotalMilliseconds;
		}

	}

}