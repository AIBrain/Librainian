#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Collections.Concurrent;
	using System.Threading;
	using JetBrains.Annotations;
	using Threadsafe;
	using Utilities;

	/// <summary>
	/// Yah.. old class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BackgroundThreadQueue<T> : ABetterClassDispose {

		private VolatileBoolean _quit;

		private Thread? thread;

		[NotNull]
		private BlockingCollection<T> MessageQueue { get; } = new BlockingCollection<T>();

		private CancellationToken Token { get; set; }

		private void ProcessQueue( [NotNull] Action<T> action ) {
			if ( action is null ) {
				throw new ArgumentNullException( nameof( action ) );
			}

			try {
				var consume = this.MessageQueue.GetConsumingEnumerable( this.Token );

				if ( this._quit ) {
					return;
				}

				foreach ( var item in consume ) {
					if ( this._quit ) {
						return; //check after blocking
					}

					action( item );

					if ( this._quit ) {
						return; //check before blocking
					}
				}
			}
			catch ( OperationCanceledException ) { }
			catch ( ObjectDisposedException ) { }
		}

		/// <summary>Same as <see cref="Enqueue" />.</summary>
		/// <param name="message"></param>
		public void Add( [CanBeNull] T message ) => this.MessageQueue.Add( message, this.Token );

		public void Cancel() {
			this._quit = true;
			this.MessageQueue.CompleteAdding();
		}

		public override void DisposeManaged() => this.Cancel();

		/// <summary>Same as <see cref="Add" />.</summary>
		/// <param name="message"></param>
		public void Enqueue( [CanBeNull] T message ) => this.MessageQueue.Add( message, this.Token );

		/// <summary></summary>
		/// <param name="each">Action to perform (poke into <see cref="MessageQueue" />).</param>
		/// <param name="token"></param>
		public void Start( [NotNull] Action<T> each, CancellationToken token ) {
			if ( each is null ) {
				throw new ArgumentNullException( nameof( each ) );
			}

			this.Token = token;

			this.thread = new Thread( () => this.ProcessQueue( each ) ) {
				IsBackground = true
			};

			this.thread.Start();
		}

	}

}