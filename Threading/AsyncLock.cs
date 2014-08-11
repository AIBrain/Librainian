#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/AsyncLock.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Measurement.Time;

    /// <summary>
    ///     Usage:
    ///     private readonly AsyncLock _lock = new AsyncLock();
    ///     using( var releaser = await _lock.LockAsync() )  { /*...*/ }
    /// </summary>
    public sealed class AsyncLock {
        private readonly Task< IDisposable > _mReleaser;

        private readonly SemaphoreSlim _mSemaphore = new SemaphoreSlim( initialCount: 1 );

        public AsyncLock() {
            this._mReleaser = Task.FromResult( ( IDisposable ) new Releaser( this ) );
        }

        public Task< IDisposable > LockAsync() {
            var wait = this._mSemaphore.WaitAsync( Minutes.Ten );
            return wait.IsCompleted ? this._mReleaser : wait.ContinueWith( ( _, state ) => ( IDisposable ) state, this._mReleaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
        }

        private sealed class Releaser : IDisposable {
            private readonly AsyncLock _mToRelease;

            internal Releaser( AsyncLock toRelease ) {
                this._mToRelease = toRelease;
            }

            public void Dispose() {
                if ( this._mToRelease._mSemaphore.CurrentCount == 0 ) {
                    this._mToRelease._mSemaphore.Release();
                }
                else {
                    Debugger.Break();
                }
            }
        }
    }
}
