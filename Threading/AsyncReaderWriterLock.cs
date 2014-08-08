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
// "Librainian2/AsyncReaderWriterLock.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Threading {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    public class AsyncReaderWriterLock {
        private readonly Task< Releaser > _mReaderReleaser;

        private readonly Queue< TaskCompletionSource< Releaser > > _mWaitingWriters = new Queue< TaskCompletionSource< Releaser > >();

        private readonly Task< Releaser > _mWriterReleaser;

        private int _mReadersWaiting;

        private int _mStatus;

        private TaskCompletionSource< Releaser > _mWaitingReader = new TaskCompletionSource< Releaser >();

        public AsyncReaderWriterLock() {
            this._mReaderReleaser = Task.FromResult( result: new Releaser( toRelease: this, writer: false ) );
            this._mWriterReleaser = Task.FromResult( result: new Releaser( toRelease: this, writer: true ) );
        }

        public Task< Releaser > ReaderLockAsync() {
            lock ( this._mWaitingWriters ) {
                if ( this._mStatus >= 0 && this._mWaitingWriters.Count == 0 ) {
                    ++this._mStatus;
                    return this._mReaderReleaser;
                }
                ++this._mReadersWaiting;
                return this._mWaitingReader.Task.ContinueWith( t => t.Result );
            }
        }

        public void ReaderRelease() {
            TaskCompletionSource< Releaser > toWake = null;

            lock ( this._mWaitingWriters ) {
                --this._mStatus;
                if ( this._mStatus == 0 && this._mWaitingWriters.Count > 0 ) {
                    this._mStatus = -1;
                    toWake = this._mWaitingWriters.Dequeue();
                }
            }

            if ( toWake != null ) {
                toWake.SetResult( new Releaser( this, true ) );
            }
        }

        public Task< Releaser > WriterLockAsync() {
            lock ( this._mWaitingWriters ) {
                if ( this._mStatus == 0 ) {
                    this._mStatus = -1;
                    return this._mWriterReleaser;
                }
                var waiter = new TaskCompletionSource< Releaser >();
                this._mWaitingWriters.Enqueue( waiter );
                return waiter.Task;
            }
        }

        public void WriterRelease() {
            TaskCompletionSource< Releaser > toWake = null;
            var toWakeIsWriter = false;

            lock ( this._mWaitingWriters ) {
                if ( this._mWaitingWriters.Count > 0 ) {
                    toWake = this._mWaitingWriters.Dequeue();
                    toWakeIsWriter = true;
                }
                else if ( this._mReadersWaiting > 0 ) {
                    toWake = this._mWaitingReader;
                    this._mStatus = this._mReadersWaiting;
                    this._mReadersWaiting = 0;
                    this._mWaitingReader = new TaskCompletionSource< Releaser >();
                }
                else {
                    this._mStatus = 0;
                }
            }

            if ( toWake != null ) {
                toWake.SetResult( result: new Releaser( toRelease: this, writer: toWakeIsWriter ) );
            }
        }
    }
}
