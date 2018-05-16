// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "AsyncReaderWriterLock.cs",
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
// "Librainian/Librainian/AsyncReaderWriterLock.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
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
                if ( this._mStatus >= 0 && this._waitingWriters.Count == 0 ) {
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

                if ( this._mStatus == 0 && this._waitingWriters.Count > 0 ) {
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
                else { this._mStatus = 0; }
            }

            toWake?.SetResult( result: new Releaser( toRelease: this, writer: toWakeIsWriter ) );
        }
    }
}