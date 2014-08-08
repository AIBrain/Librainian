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
// "Librainian2/AbandonableTask.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    /// <see cref="http://stackoverflow.com/a/4749401/956364" />
    public sealed class AbandonableTask {
        private readonly Action< Task > _afterComplete;
        private readonly Action _beginWork;
        private readonly Action _blockingWork;
        private readonly CancellationToken _cancellationToken;

        private AbandonableTask( CancellationToken cancellationToken, Action beginWork, Action blockingWork, Action< Task > afterComplete ) {
            if ( blockingWork == null ) {
                throw new ArgumentNullException( "blockingWork" );
            }

            this._cancellationToken = cancellationToken;
            this._beginWork = beginWork;
            this._blockingWork = blockingWork;
            this._afterComplete = afterComplete;
        }

        public static Task Start( CancellationToken cancellationToken, Action blockingWork, Action beginWork = null, Action< Task > afterComplete = null ) {
            if ( blockingWork == null ) {
                throw new ArgumentNullException( "blockingWork" );
            }

            var worker = new AbandonableTask( cancellationToken, beginWork, blockingWork, afterComplete );
            var outerTask = new Task( worker.RunTask, cancellationToken );
            outerTask.Start();
            return outerTask;
        }

        private void RunTask() {
            if ( this._beginWork != null ) {
                this._beginWork();
            }

            var innerTask = new Task( this._blockingWork, this._cancellationToken, TaskCreationOptions.LongRunning );
            innerTask.Start();

            innerTask.Wait( this._cancellationToken );
            if ( innerTask.IsCompleted && this._afterComplete != null ) {
                this._afterComplete( innerTask );
            }
        }
    }
}
