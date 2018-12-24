namespace Librainian.Threading {

	using System;
	using System.Threading.Tasks;
	using Extensions;
	using JetBrains.Annotations;

	/// <summary>
	/// A task with an active progress, and a timer to complete or cancel.
	/// </summary>
	public class Job<T> {

		public TimeSpan EstimatedLengthOfJob { get; set; }

		public TimeSpan EstimatedTimeRemaining { get; set; }

		public Task<T> TheTask { get; }

		public Job( [NotNull] Func<T> func ) {
			if ( func == null ) {
				throw new ArgumentNullException( paramName: nameof( func ) );
			}

			this.TheTask = Task.Run( func );
		}

		public Job( [NotNull] Task<T> task ) {
			this.Nop();
			this.TheTask = task ?? throw new ArgumentNullException( paramName: nameof( task ) );
		}

		private void Done() {

			//Monitor.Pulse( notify );//pulse on completion or timeout?
		}
	}
}