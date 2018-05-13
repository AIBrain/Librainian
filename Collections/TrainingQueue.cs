// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TrainingQueue.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

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
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this._bob.Dispose();

        public void Enqueue( TrainingQueueItem train ) {
            this.Items.Add( item: train );
            this._bob.Set();
        }

        /// <summary>
        /// Waits until an item in is the queue.
        /// </summary>
        public void Wait() {

            //this.bob.Reset();
            while ( !this.Items.Any() ) {
                this._bob.WaitOne( timeout: TimeSpan.FromMilliseconds( value: 1234 ) );
            }

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