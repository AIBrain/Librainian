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
// File "BackgroundThread.cs" last touched on 2021-04-25 at 6:51 AM by Protiguous.

namespace Librainian.Threading {

	using System;
	using System.Threading;
	using Exceptions;
	using Logging;
	using Measurement.Time;
	using Utilities;

	/// <summary>
	///     Accepts an <see cref="Action" /> to perform (in a loop) when the <see cref="signal" /> is Set (<see cref="Poke" />
	///     ).
	/// </summary>
	public class BackgroundThread : ABetterClassDispose {

		/// <summary></summary>
		/// <param name="actionToPerform">Action to perform on each <see cref="signal" />.</param>
		/// <param name="autoStart"></param>
		/// <param name="cancellationToken"></param>
		public BackgroundThread( Action actionToPerform, Boolean autoStart, CancellationToken cancellationToken ) {
			this.ActionToPerform = actionToPerform ?? throw new ArgumentEmptyException( nameof( actionToPerform ) );
			this.cancellationToken = cancellationToken;

			this.thread = new Thread( ThreadStart ) {
				IsBackground = true, Priority = ThreadPriority.BelowNormal
			};

			if ( autoStart ) {
				this.Start();
			}

			void ThreadStart() {
				while ( !this.cancellationToken.IsCancellationRequested ) {
					//Every second wake up and see if we can get the semaphore.
					//If we can, then run the ActionToPerform().
					if ( this.signal.Wait( Seconds.One, cancellationToken ) ) {
						try {
							this.ActionToPerform();
						}
						catch ( Exception exception ) {
							exception.Log();
						}
						finally {
							this.signal.Release();
						}
					}
					else {
						Thread.Yield();
					}
				}
			}
		}

		private Action ActionToPerform { get; }

		private CancellationToken cancellationToken { get; }

		private SemaphoreSlim signal { get; } = new(1, 1);

		private Thread thread { get; }

		private void Start() {
			if ( this.thread.ThreadState != ThreadState.Running ) {
				this.thread.Start();
			}
		}

		public override void DisposeManaged() {
			if ( this.thread.ThreadState == ThreadState.Running ) {
				this.thread.Join( Seconds.Seven );
			}

			base.DisposeManaged();
		}

		/// <summary>Do anything needed in this method before the loops run.</summary>
		public virtual void Initialize() { }

		//public Boolean IsRunningAction() => this.RunningAction;

		/// <summary>Set the signal.</summary>
		[Obsolete("using semaphore instead.")]
		public void Poke() {
			//this.signal.Release();
		}

	}

}