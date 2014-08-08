#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/FIFOWaitQueue.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Annotations;
    using Threading;

    /// <summary>
    /// Simple linked list queue used in FIFOSemaphore. Methods are not locked; they depend on synch
    /// of callers.
    /// NOTE: this class is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    [Serializable]
    [UsedImplicitly]
    internal class FIFOWaitQueue : IWaitQueue {

        [NonSerialized]
        protected WaitNode Head;

        [NonSerialized]
        protected WaitNode Tail;

        public Boolean HasNodes { get { return this.Head != null; } }

        public int Length {
            get {
                var count = 0;
                var node = this.Head;
                while ( node != null ) {
                    if ( node.IsWaiting ) {
                        count++;
                    }
                    node = node.NextWaitNode;
                }
                return count;
            }
        }

        public ICollection<Thread> WaitingThreads {
            get {
                IList<Thread> list = new List<Thread>();
                var node = this.Head;
                while ( node != null ) {
                    if ( node.IsWaiting ) {
                        list.Add( node.Owner );
                    }
                    node = node.NextWaitNode;
                }
                return list;
            }
        }

        public WaitNode Dequeue() {
            if ( this.Head == null ) {
                return null;
            }

            var w = this.Head;
            this.Head = w.NextWaitNode;
            if ( this.Head == null ) {
                this.Tail = null;
            }
            w.NextWaitNode = null;
            return w;
        }

        public void Enqueue( WaitNode w ) {
            if ( this.Tail == null ) {
                this.Head = this.Tail = w;
            }
            else {
                this.Tail.NextWaitNode = w;
                this.Tail = w;
            }
        }

        public Boolean IsWaiting( Thread thread ) {
            if ( thread == null ) {
                throw new ArgumentNullException( "thread" );
            }
            for ( var node = this.Head; node != null; node = node.NextWaitNode ) {
                if ( node.IsWaiting && node.Owner == thread ) {
                    return true;
                }
            }
            return false;
        }

        // In backport 3.1 but not used.
        //public void PutBack(WaitNode w)
        //{
        //    w.NextWaitNode = _head;
        //    _head = w;
        //    if (_tail == null)
        //        _tail = w;
        //}
    }
}