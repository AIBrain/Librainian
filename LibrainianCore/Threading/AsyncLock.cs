// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AsyncLock.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "AsyncLock.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace LibrainianCore.Threading {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Utilities;

    /// <summary>Usage: private  AsyncLock _lock = new AsyncLock(); using( var releaser = await _lock.LockAsync() ) { /*...*/ }</summary>
    public sealed class AsyncLock : ABetterClassDispose {

        [NotNull]
        private Task<IDisposable> _releaser { get; }

        [NotNull]
        private SemaphoreSlim Semaphore { get; } = new SemaphoreSlim( initialCount: 1 );

        public AsyncLock() => this._releaser = Task.FromResult( new Releaser( this ) as IDisposable );

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() {
            using ( this.Semaphore ) { }
        }

        [CanBeNull]
        public Task<IDisposable> LockAsync() {
            var wait = this.Semaphore.WaitAsync( Minutes.Ten );

            return wait.IsCompleted ?
                this._releaser :
                wait.ContinueWith( ( _, state ) => ( IDisposable )state, this._releaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default );
        }

        private sealed class Releaser : ABetterClassDispose {

            private AsyncLock _mToRelease { get; }

            internal Releaser( [CanBeNull] AsyncLock toRelease ) => this._mToRelease = toRelease;

            public override void DisposeManaged() {
                if ( !this._mToRelease.Semaphore.CurrentCount.Any() ) {
                    this._mToRelease.Semaphore.Release();
                }
                else {
                    Debugger.Break();
                }
            }
        }
    }
}