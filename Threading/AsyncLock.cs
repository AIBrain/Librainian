// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AsyncLock.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Magic;
    using Maths;
    using Measurement.Time;

    /// <summary>
    ///     Usage: private readonly AsyncLock _lock = new AsyncLock(); using( var releaser = await _lock.LockAsync() ) { /*...*/ }
    /// </summary>
    /// <remarks>(I have no idea how to use this class.)</remarks>
    public sealed class AsyncLock : ABetterClassDispose {
        private readonly Task<IDisposable> _releaser;

        private SemaphoreSlim Semaphore { get; } = new SemaphoreSlim( initialCount: 1 );

        public AsyncLock() => this._releaser = Task.FromResult( ( IDisposable )new Releaser( this ) );

	    public Task<IDisposable> LockAsync() {
            var wait = this.Semaphore.WaitAsync( Minutes.Ten );
            return wait.IsCompleted ? this._releaser : wait.ContinueWith( ( _, state ) => ( IDisposable )state, this._releaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
        }

        private sealed class Releaser : ABetterClassDispose {
            private readonly AsyncLock _mToRelease;

            internal Releaser( AsyncLock toRelease ) => this._mToRelease = toRelease;

            protected override void DisposeManaged() {
                if ( !this._mToRelease.Semaphore.CurrentCount.Any() ) {
                    this._mToRelease.Semaphore.Release();
                }
                else {
                    Debugger.Break();
                }
                base.DisposeManaged();
            }
        }

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this.Semaphore.Dispose();

	}
}