// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AbandonableTask.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/AbandonableTask.cs" was last formatted by Protiguous on 2018/05/24 at 7:33 PM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    /// <see cref="http://stackoverflow.com/a/4749401/956364" />
    public sealed class AbandonableTask {

        private readonly Action<Task> _afterComplete;

        private readonly Action _beginWork;

        private readonly Action _blockingWork;

        private readonly CancellationToken _cancellationToken;

        private AbandonableTask( CancellationToken cancellationToken, Action beginWork, Action blockingWork, Action<Task> afterComplete ) {
            this._cancellationToken = cancellationToken;
            this._beginWork = beginWork;
            this._blockingWork = blockingWork ?? throw new ArgumentNullException( nameof( blockingWork ) );
            this._afterComplete = afterComplete;
        }

        private void RunTask() {
            this._beginWork?.Invoke();

            var innerTask = new Task( this._blockingWork, this._cancellationToken, TaskCreationOptions.LongRunning );
            innerTask.Start();

            innerTask.Wait( this._cancellationToken );

            if ( innerTask.IsCompleted ) { this._afterComplete?.Invoke( innerTask ); }
        }

        public static Task Start( CancellationToken cancellationToken, Action blockingWork, Action beginWork = null, Action<Task> afterComplete = null ) {
            if ( blockingWork is null ) { throw new ArgumentNullException( nameof( blockingWork ) ); }

            var worker = new AbandonableTask( cancellationToken, beginWork, blockingWork, afterComplete );
            var outerTask = new Task( worker.RunTask, cancellationToken );
            outerTask.Start();

            return outerTask;
        }
    }
}