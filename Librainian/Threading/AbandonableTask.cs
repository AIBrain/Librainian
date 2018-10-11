// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AbandonableTask.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "AbandonableTask.cs" was last formatted by Protiguous on 2018/07/13 at 1:39 AM.

namespace Librainian.Threading
{

    using JetBrains.Annotations;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    /// <see cref="http://stackoverflow.com/a/4749401/956364" />
    public sealed class AbandonableTask
    {

        private readonly Action<Task> _afterComplete;

        private readonly Action _beginWork;

        private readonly Action _blockingWork;

        private readonly CancellationToken _cancellationToken;

        private AbandonableTask(CancellationToken cancellationToken, Action beginWork, [NotNull] Action blockingWork, Action<Task> afterComplete)
        {
            this._cancellationToken = cancellationToken;
            this._beginWork = beginWork;
            this._blockingWork = blockingWork ?? throw new ArgumentNullException(nameof(blockingWork));
            this._afterComplete = afterComplete;
        }

        private void RunTask()
        {
            this._beginWork?.Invoke();

            var innerTask = new Task(this._blockingWork, this._cancellationToken, TaskCreationOptions.LongRunning);
            innerTask.Start();

            innerTask.Wait(this._cancellationToken);

            if (innerTask.IsCompleted) { this._afterComplete?.Invoke(innerTask); }
        }

        [NotNull]
        public static Task Start(CancellationToken cancellationToken, [NotNull] Action blockingWork, [CanBeNull] Action beginWork = null, [CanBeNull] Action<Task> afterComplete = null)
        {
            if (blockingWork == null) { throw new ArgumentNullException(nameof(blockingWork)); }

            var worker = new AbandonableTask(cancellationToken, beginWork, blockingWork, afterComplete);
            var outerTask = new Task(worker.RunTask, cancellationToken);
            outerTask.Start();

            return outerTask;
        }
    }
}