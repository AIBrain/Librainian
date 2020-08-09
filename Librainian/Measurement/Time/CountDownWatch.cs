#nullable enable

namespace Librainian.Measurement.Time {

	using System;
	using JetBrains.Annotations;
	using Logging;
	using Newtonsoft.Json;
	using Threading;
	using Threadsafe;

	/// <summary>
	///     The 'reverse' of the Stopwatch struct.
	///     <remarks>
	///         //TODO needs unit testing.
	///     </remarks>
	/// </summary>
	[JsonObject( MemberSerialization.Fields )]
	public class CountDownWatch {

		/// <summary></summary>
		/// <param name="countdown"></param>
		/// <param name="liftoff">Action to invoke when countdown reaches zero.</param>
		public CountDownWatch( TimeSpan countdown, [CanBeNull] Action? liftoff = null ) {
			if ( countdown < TimeSpan.Zero ) {
				throw new ArgumentOutOfRangeException( nameof( countdown ), "Must be a positive value." );
			}

			this.Countdown = countdown;
			this.TargetTime = DateTime.UtcNow.Add( this.Countdown );

			this.Liftoff = () => {
				try {
					this.HasLaunched = true;
					liftoff?.Invoke();
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			};

			this.Timer = this.Countdown.CreateTimer( () => {
				this.Stop();
				this.Liftoff();
			} ).Once();
		}

		[NotNull]
		private Action Liftoff { get; }

		private FluentTimer Timer { get; set; }

		public TimeSpan Countdown { get; }

		public VolatileBoolean HasLaunched { get; private set; }

		public VolatileBoolean IsRunning { get; private set; }

		public DateTime TargetTime { get; private set; }

		public DateTime WhenStarted { get; private set; }

		public DateTime WhenStopped { get; private set; }

		public TimeSpan Remaining() {
			if ( this.IsRunning ) {
				return this.Countdown.Subtract( DateTime.UtcNow - this.WhenStarted );
			}

			if ( this.HasLaunched ) {
				return this.Countdown.Subtract( this.WhenStopped - this.WhenStarted );
			}

			throw new InvalidOperationException( "???" );
		}

		public void Start() {
			this.WhenStarted = DateTime.UtcNow;
			this.IsRunning = true;
			this.Timer.Start();
		}

		public void Stop() {
			this.IsRunning = false;
			this.WhenStopped = DateTime.UtcNow;
			this.Timer.Stop();
		}

	}

}