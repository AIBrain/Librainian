// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FIFOWaitQueue.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/FIFOWaitQueue.cs" was last formatted by Protiguous on 2018/05/24 at 6:59 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    ///     Simple linked list queue used in FIFOSemaphore. Methods are not locked; they depend on synch of callers.
    ///     NOTE: this class is NOT present in java.util.concurrent.
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