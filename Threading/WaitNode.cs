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
// "Librainian/WaitNode.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>
    ///     The wait node used by implementations of <see cref="Collections.IWaitQueue" />.
    ///     NOTE: this class is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    public class WaitNode // was WaitQueue.WaitNode in BACKPORT_3_1
    {
        internal readonly Thread _owner;
        internal WaitNode _nextWaitNode;
        internal Boolean _waiting = true;

        public WaitNode() {
            this._owner = Thread.CurrentThread;
        }

        internal virtual Thread Owner => this._owner;

        internal virtual Boolean IsWaiting => this._waiting;

        internal virtual WaitNode NextWaitNode { get { return this._nextWaitNode; } set { this._nextWaitNode = value; } }

        public virtual Boolean Signal( [NotNull] IQueuedSync sync ) {
            if ( sync == null ) {
                throw new ArgumentNullException( "sync" );
            }
            lock ( this ) {
                var signalled = this._waiting;
                if ( signalled ) {
                    this._waiting = false;
                    Monitor.Pulse( this );
                    sync.TakeOver( this );
                }
                return signalled;
            }
        }

        public virtual void DoWait( [NotNull] IQueuedSync sync ) {
            if ( sync == null ) {
                throw new ArgumentNullException( "sync" );
            }
            lock ( this ) {
                if ( sync.Recheck( this ) ) {
                    return;
                }
                try {
                    while ( this._waiting ) {
                        Monitor.Wait( this );
                    }
                }
                catch ( ThreadInterruptedException ) {
                    if ( this._waiting ) {
                        // no notification
                        this._waiting = false; // invalidate for the signaller
                        throw;
                    }
                    // thread was interrupted after it was notified
                    Thread.CurrentThread.Interrupt();
                }
            }
        }

        public virtual void DoWaitUninterruptibly( IQueuedSync sync ) {
            lock ( this ) {
                if ( !sync.Recheck( this ) ) {
                    var wasInterrupted = false;
                    while ( this._waiting ) {
                        try {
                            Monitor.Wait( this );
                        }
                        catch ( ThreadInterruptedException ) {
                            wasInterrupted = true;
                            // no need to notify; if we were signalled, we
                            // must be not waiting, and we'll act like signalled
                        }
                    }
                    if ( wasInterrupted ) {
                        Thread.CurrentThread.Interrupt();
                    }
                }
            }
        }

        public virtual Boolean DoTimedWait( IQueuedSync sync, TimeSpan duration ) {
            lock ( this ) {
                if ( sync.Recheck( this ) || !this._waiting ) {
                    return true;
                }
                if ( duration.Ticks <= 0 ) {
                    this._waiting = false;
                    return false;
                }
                var deadline = DateTime.UtcNow.Add( duration );
                try {
                    for ( ;; ) {
                        Monitor.Wait( this, duration );
                        if ( !this._waiting ) // definitely signalled
                        {
                            return true;
                        }
                        duration = deadline.Subtract( DateTime.UtcNow );
                        if ( duration.Ticks <= 0 ) // time out
                        {
                            this._waiting = false;
                            return false;
                        }
                    }
                }
                catch ( ThreadInterruptedException ) {
                    if ( this._waiting ) // no notification
                    {
                        this._waiting = false; // invalidate for the signaller
                        throw;
                    }
                    // thread was interrupted after it was notified
                    Thread.CurrentThread.Interrupt();
                    return true;
                }
            }
        }
    }
}
