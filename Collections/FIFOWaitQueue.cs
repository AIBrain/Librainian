// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "FIFOWaitQueue.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/FIFOWaitQueue.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

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
                    if ( node.IsWaiting ) { count++; }

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
                    if ( node.IsWaiting ) { list.Add( item: node.Owner ); }

                    node = node.NextWaitNode;
                }

                return list;
            }
        }

        public WaitNode Dequeue() {
            if ( this.Head is null ) { return null; }

            var w = this.Head;
            this.Head = w.NextWaitNode;

            if ( this.Head is null ) { this.Tail = null; }

            w.NextWaitNode = null;

            return w;
        }

        public void Enqueue( WaitNode w ) {
            if ( this.Tail is null ) { this.Head = this.Tail = w; }
            else {
                this.Tail.NextWaitNode = w;
                this.Tail = w;
            }
        }

        public Boolean IsWaiting( Thread thread ) {
            if ( thread is null ) { throw new ArgumentNullException( nameof( thread ) ); }

            for ( var node = this.Head; node != null; node = node.NextWaitNode ) {
                if ( node.IsWaiting && node.Owner == thread ) { return true; }
            }

            return false;
        }
    }
}