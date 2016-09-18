// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AsyncReaderWriterLock.cs" was last cleaned by Rick on 2016/08/06 at 11:57 AM

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary></summary>
    public class AsyncReaderWriterLock {

        private readonly Task<Releaser> _readerReleaser;

        private readonly Queue<TaskCompletionSource<Releaser>> _waitingWriters = new Queue<TaskCompletionSource<Releaser>>();

        private readonly Task<Releaser> _writerReleaser;

        private Int32 _mReadersWaiting;

        private Int32 _mStatus;

        private TaskCompletionSource<Releaser> _mWaitingReader = new TaskCompletionSource<Releaser>();

        public AsyncReaderWriterLock() {
            this._readerReleaser = Task.FromResult( result: new Releaser( toRelease: this, writer: false ) );
            this._writerReleaser = Task.FromResult( result: new Releaser( toRelease: this, writer: true ) );
        }

        public Task<Releaser> ReaderLockAsync() {
            lock ( this._waitingWriters ) {
                if ( ( this._mStatus >= 0 ) && ( this._waitingWriters.Count == 0 ) ) {
                    ++this._mStatus;
                    return this._readerReleaser;
                }

                ++this._mReadersWaiting;
                return this._mWaitingReader.Task.ContinueWith( t => t.Result );
            }
        }

        public void ReaderRelease() {
            TaskCompletionSource<Releaser> toWake = null;

            lock ( this._waitingWriters ) {
                --this._mStatus;
                if ( ( this._mStatus == 0 ) && ( this._waitingWriters.Count > 0 ) ) {
                    this._mStatus = -1;
                    toWake = this._waitingWriters.Dequeue();
                }
            }

            toWake?.SetResult( new Releaser( this, true ) );
        }

        public Task<Releaser> WriterLockAsync() {
            lock ( this._waitingWriters ) {
                if ( this._mStatus == 0 ) {
                    this._mStatus = -1;
                    return this._writerReleaser;
                }

                var waiter = new TaskCompletionSource<Releaser>();
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
                else if ( this._mReadersWaiting > 0 ) {
                    toWake = this._mWaitingReader;
                    this._mStatus = this._mReadersWaiting;
                    this._mReadersWaiting = 0;
                    this._mWaitingReader = new TaskCompletionSource<Releaser>();
                }
                else {
                    this._mStatus = 0;
                }
            }

            toWake?.SetResult( result: new Releaser( toRelease: this, writer: toWakeIsWriter ) );
        }
    }
}
