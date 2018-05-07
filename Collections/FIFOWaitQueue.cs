// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FIFOWaitQueue.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    /// Simple linked list queue used in FIFOSemaphore. Methods are not locked; they depend on synch of callers.
    /// NOTE: this class is NOT present in java.util.concurrent.
    /// </summary>
    /// <author>Doug Lea</author>
    /// <author>Griffin Caprio (.NET)</author>
    /// <author>Kenneth Xu</author>
    [JsonObject]
    internal class FifoWaitQueue : IWaitQueue {

        [NonSerialized]
        protected WaitNode Head;

        [NonSerialized]
        protected WaitNode Tail;

        public Boolean HasNodes => this.Head != null;

        public Int32 Length {
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
                var list = new List<Thread>();
                var node = this.Head;
                while ( node != null ) {
                    if ( node.IsWaiting ) {
                        list.Add( item: node.Owner );
                    }

                    node = node.NextWaitNode;
                }

                return list;
            }
        }

        public WaitNode Dequeue() {
            if ( this.Head is null ) {
                return null;
            }

            var w = this.Head;
            this.Head = w.NextWaitNode;
            if ( this.Head is null ) {
                this.Tail = null;
            }

            w.NextWaitNode = null;
            return w;
        }

        public void Enqueue( WaitNode w ) {
            if ( this.Tail is null ) {
                this.Head = this.Tail = w;
            }
            else {
                this.Tail.NextWaitNode = w;
                this.Tail = w;
            }
        }

        public Boolean IsWaiting( Thread thread ) {
            if ( thread is null ) {
                throw new ArgumentNullException( paramName: nameof( thread ) );
            }

            for ( var node = this.Head; node != null; node = node.NextWaitNode ) {
                if ( node.IsWaiting && node.Owner == thread ) {
                    return true;
                }
            }

            return false;
        }
    }
}