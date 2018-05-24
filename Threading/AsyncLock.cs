// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "AsyncLock.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/AsyncLock.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Magic;
    using Measurement.Time;

    /// <summary>
    ///     Usage: private readonly AsyncLock _lock = new AsyncLock(); using( var releaser = await _lock.LockAsync() ) {
    ///     /*...*/ }
    /// </summary>
    /// <remarks>(I have no idea how to use this class.)</remarks>
    public sealed class AsyncLock : ABetterClassDispose {

        private readonly Task<IDisposable> _releaser;

        private SemaphoreSlim Semaphore { get; } = new SemaphoreSlim( initialCount: 1 );

        public AsyncLock() => this._releaser = Task.FromResult( new Releaser( this ) as IDisposable );

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Semaphore.Dispose();

        public Task<IDisposable> LockAsync() {
            var wait = this.Semaphore.WaitAsync( Minutes.Ten );

            return wait.IsCompleted
                ? this._releaser
                : wait.ContinueWith( ( _, state ) => ( IDisposable )state, this._releaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
        }

        private sealed class Releaser : ABetterClassDispose {

            private readonly AsyncLock _mToRelease;

            internal Releaser( AsyncLock toRelease ) => this._mToRelease = toRelease;

            public override void DisposeManaged() {
                if ( !this._mToRelease.Semaphore.CurrentCount.Any() ) { this._mToRelease.Semaphore.Release(); }
                else { Debugger.Break(); }

                base.DisposeManaged();
            }
        }
    }
}