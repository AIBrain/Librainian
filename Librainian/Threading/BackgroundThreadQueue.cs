// Copyright © Protiguous. All Rights Reserved.
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
// File "BackgroundThreadQueue.cs" last formatted on 2020-08-14 at 8:46 PM.

#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Collections.Concurrent;
	using System.Threading;
	using Exceptions;
	using Threadsafe;
	using Utilities;
	using Utilities.Disposables;

	/// <summary>
	///     Yah.. old class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BackgroundThreadQueue<T> : ABetterClassDispose {

		private VolatileBoolean _quit;

		private Thread? thread;

		private BlockingCollection<T> MessageQueue { get; } = new();

		private CancellationToken Token { get; set; }

		private void ProcessQueue( Action<T> action ) {
			if ( action is null ) {
				throw new ArgumentEmptyException( nameof( action ) );
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
		public void Add( T? message ) => this.MessageQueue.Add( message, this.Token );

		public void Cancel() {
			this._quit = true;
			this.MessageQueue.CompleteAdding();
		}

		public override void DisposeManaged() => this.Cancel();

		/// <summary>Same as <see cref="Add" />.</summary>
		/// <param name="message"></param>
		public void Enqueue( T? message ) => this.MessageQueue.Add( message, this.Token );

		/// <summary></summary>
		/// <param name="each">Action to perform (poke into <see cref="MessageQueue" />).</param>
		/// <param name="cancellationToken"></param>
		public void Start( Action<T> each, CancellationToken cancellationToken ) {
			if ( each is null ) {
				throw new ArgumentEmptyException( nameof( each ) );
			}

			this.Token = cancellationToken;

			this.thread = new Thread( () => this.ProcessQueue( each ) ) {
				IsBackground = true
			};

			this.thread.Start();
		}
	}
}