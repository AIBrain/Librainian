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
// File "AsyncReaderWriterLock.cs" last formatted on 2020-08-14 at 8:46 PM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	/// <summary></summary>
	public class AsyncReaderWriterLock {

		private Int32 _readersWaiting;

		private Int32 _status;

		public AsyncReaderWriterLock() {
			this._readerReleaser = Task.FromResult( new Releaser( this, false ) );
			this._writerReleaser = Task.FromResult( new Releaser( this, true ) );
		}

		[NotNull]
		private Task<Releaser> _readerReleaser { get; }

		[NotNull]
		private TaskCompletionSource<Releaser> _waitingReader { get; set; } = new TaskCompletionSource<Releaser>( TaskCreationOptions.RunContinuationsAsynchronously );

		[NotNull]
		private Queue<TaskCompletionSource<Releaser>> _waitingWriters { get; } = new Queue<TaskCompletionSource<Releaser>>();

		[NotNull]
		private Task<Releaser> _writerReleaser { get; }

		[NotNull]
		public Task<Releaser> ReaderLockAsync() {
			lock ( this._waitingWriters ) {
				if ( this._status >= 0 && !this._waitingWriters.Any() ) {
					++this._status;

					return this._readerReleaser;
				}

				++this._readersWaiting;

				return this._waitingReader.Task.ContinueWith( t => t.Result );
			}
		}

		public void ReaderRelease() {
			TaskCompletionSource<Releaser> toWake = null;

			lock ( this._waitingWriters ) {
				--this._status;

				if ( this._status == 0 && this._waitingWriters.Count > 0 ) {
					this._status = -1;
					toWake = this._waitingWriters.Dequeue();
				}
			}

			toWake?.SetResult( new Releaser( this, true ) );
		}

		[NotNull]
		public Task<Releaser> WriterLockAsync() {
			lock ( this._waitingWriters ) {
				if ( this._status == 0 ) {
					this._status = -1;

					return this._writerReleaser;
				}

				var waiter = new TaskCompletionSource<Releaser>( TaskCreationOptions.RunContinuationsAsynchronously );
				this._waitingWriters.Enqueue( waiter );

				return waiter.Task;
			}
		}

		public void WriterRelease() {
			TaskCompletionSource<Releaser> toWake = null;
			var toWakeIsWriter = false;

			lock ( this._waitingWriters ) {
				if ( this._waitingWriters.Count > 0 ) {
					toWake = this._waitingWriters.Dequeue();
					toWakeIsWriter = true;
				}
				else if ( this._readersWaiting > 0 ) {
					toWake = this._waitingReader;
					this._status = this._readersWaiting;
					this._readersWaiting = 0;
					this._waitingReader = new TaskCompletionSource<Releaser>( TaskCreationOptions.RunContinuationsAsynchronously );
				}
				else {
					this._status = 0;
				}
			}

			toWake?.SetResult( new Releaser( this, toWakeIsWriter ) );
		}

	}

}