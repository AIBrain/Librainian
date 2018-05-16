// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TrainingQueue.cs",
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
// "Librainian/Librainian/TrainingQueue.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Magic;

    public class TrainingQueue : ABetterClassDispose {

        private readonly ManualResetEvent _bob = new ManualResetEvent( initialState: false );

        public readonly ConcurrentQueue<TrainingQueueItem> Items = new ConcurrentQueue<TrainingQueueItem>();

        //public readonly ConcurrentStack<TrainingQueueItem> Items = new ConcurrentStack<TrainingQueueItem>();
        public TrainingQueue() => this._bob.Reset();

        public TrainingQueueItem Dequeue() => this.Items.Remove();

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this._bob.Dispose();

        public void Enqueue( TrainingQueueItem train ) {
            this.Items.Add( item: train );
            this._bob.Set();
        }

        /// <summary>
        ///     Waits until an item in is the queue.
        /// </summary>
        public void Wait() {

            //this.bob.Reset();
            while ( !this.Items.Any() ) { this._bob.WaitOne( timeout: TimeSpan.FromMilliseconds( 1234 ) ); }

            this._bob.Reset();
        }

        public class TrainingQueueItem {

            public TrainingQueueItem( Object question, Object answer, Action action ) {
                this.Question = question;
                this.Answer = answer;
                this.Action = action;
            }

            public Action Action { get; set; }

            public Object Answer { get; }

            public Object Question { get; }

            public override String ToString() => $"{this.Question} -> {this.Answer}";
        }
    }
}