// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WaitNode.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "WaitNode.cs" was last formatted by Protiguous on 2018/07/13 at 1:42 AM.

namespace Librainian.Threading {

	using System;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>
	///     The wait node used by implementations of <see cref="Collections.IWaitQueue" />.
	///     NOTE: this class is NOT present in java.util.concurrent.
	/// </summary>
	/// <author>Doug Lea</author>
	/// <author>Griffin Caprio (.NET)</author>
	/// <author>Kenneth Xu</author>
	public class WaitNode // was WaitQueue.WaitNode in BACKPORT_3_1
	{

		internal Boolean Waiting = true;

		internal virtual Boolean IsWaiting => this.Waiting;

		internal virtual WaitNode NextWaitNode { get; set; }

		internal virtual Thread Owner { get; }

		public WaitNode() => this.Owner = Thread.CurrentThread;

		public virtual Boolean DoTimedWait( [NotNull] IQueuedSync sync, TimeSpan duration ) {
			lock ( this ) {
				if ( sync.Recheck( this ) || !this.Waiting ) { return true; }

				if ( duration.Ticks <= 0 ) {
					this.Waiting = false;

					return false;
				}

				var deadline = DateTime.UtcNow.Add( duration );

				try {
					for ( ;; ) {
						Monitor.Wait( this, duration );

						if ( !this.Waiting ) // definitely signalled
						{
							return true;
						}

						duration = deadline.Subtract( DateTime.UtcNow );

						if ( duration.Ticks <= 0 ) // time out
						{
							this.Waiting = false;

							return false;
						}
					}
				}
				catch ( ThreadInterruptedException ) {
					if ( this.Waiting ) // no notification
					{
						this.Waiting = false; // invalidate for the signaller

						throw;
					}

					// thread was interrupted after it was notified
					Thread.CurrentThread.Interrupt();

					return true;
				}
			}
		}

		public virtual void DoWait( [NotNull] IQueuedSync sync ) {
			if ( sync is null ) { throw new ArgumentNullException( nameof( sync ) ); }

			lock ( this ) {
				if ( sync.Recheck( this ) ) { return; }

				try {
					while ( this.Waiting ) { Monitor.Wait( this ); }
				}
				catch ( ThreadInterruptedException ) {
					if ( this.Waiting ) {

						// no notification
						this.Waiting = false; // invalidate for the signaller

						throw;
					}

					// thread was interrupted after it was notified
					Thread.CurrentThread.Interrupt();
				}
			}
		}

		public virtual void DoWaitUninterruptibly( [NotNull] IQueuedSync sync ) {
			lock ( this ) {
				if ( !sync.Recheck( this ) ) {
					var wasInterrupted = false;

					while ( this.Waiting ) {
						try { Monitor.Wait( this ); }
						catch ( ThreadInterruptedException ) {
							wasInterrupted = true;

							// no need to notify; if we were signalled, we must be not waiting, and we'll act like signalled
						}
					}

					if ( wasInterrupted ) { Thread.CurrentThread.Interrupt(); }
				}
			}
		}

		public virtual Boolean Signal( [NotNull] IQueuedSync sync ) {
			if ( sync is null ) { throw new ArgumentNullException( nameof( sync ) ); }

			lock ( this ) {
				var signalled = this.Waiting;

				if ( signalled ) {
					this.Waiting = false;
					Monitor.Pulse( this );
					sync.TakeOver( this );
				}

				return signalled;
			}
		}
	}
}