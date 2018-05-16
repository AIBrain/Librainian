// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WaitNode.cs",
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
// "Librainian/Librainian/WaitNode.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

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

        internal Boolean Waiting = true;

        public WaitNode() => this._owner = Thread.CurrentThread;

        internal virtual Boolean IsWaiting => this.Waiting;

        internal virtual WaitNode NextWaitNode {
            get => this._nextWaitNode;

            set => this._nextWaitNode = value;
        }

        internal virtual Thread Owner => this._owner;

        public virtual Boolean DoTimedWait( IQueuedSync sync, TimeSpan duration ) {
            lock ( this ) {
                if ( sync.Recheck( this ) || !this.Waiting ) { return true; }

                if ( duration.Ticks <= 0 ) {
                    this.Waiting = false;

                    return false;
                }

                var deadline = DateTime.UtcNow.Add( duration );

                try {
                    for (; ; ) {
                        Monitor.Wait( this, duration );

                        if ( !this.Waiting ) // definitely signalled
                        {
                            return true;
                        }

                        duration = deadline.Subtract( DateTime.UtcNow );

                        if ( duration.Ticks <= 0 ) // time out
                        {
                            this.Waiting = false;

                            return false;
                        }
                    }
                }
                catch ( ThreadInterruptedException ) {
                    if ( this.Waiting ) // no notification
                    {
                        this.Waiting = false; // invalidate for the signaller

                        throw;
                    }

                    // thread was interrupted after it was notified
                    Thread.CurrentThread.Interrupt();

                    return true;
                }
            }
        }

        public virtual void DoWait( [NotNull] IQueuedSync sync ) {
            if ( sync is null ) { throw new ArgumentNullException( nameof( sync ) ); }

            lock ( this ) {
                if ( sync.Recheck( this ) ) { return; }

                try {
                    while ( this.Waiting ) { Monitor.Wait( this ); }
                }
                catch ( ThreadInterruptedException ) {
                    if ( this.Waiting ) {

                        // no notification
                        this.Waiting = false; // invalidate for the signaller

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

                    while ( this.Waiting ) {
                        try { Monitor.Wait( this ); }
                        catch ( ThreadInterruptedException ) {
                            wasInterrupted = true;

                            // no need to notify; if we were signalled, we must be not waiting, and we'll act like signalled
                        }
                    }

                    if ( wasInterrupted ) { Thread.CurrentThread.Interrupt(); }
                }
            }
        }

        public virtual Boolean Signal( [NotNull] IQueuedSync sync ) {
            if ( sync is null ) { throw new ArgumentNullException( nameof( sync ) ); }

            lock ( this ) {
                var signalled = this.Waiting;

                if ( signalled ) {
                    this.Waiting = false;
                    Monitor.Pulse( this );
                    sync.TakeOver( this );
                }

                return signalled;
            }
        }
    }
}