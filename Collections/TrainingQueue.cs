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
// "Librainian/TrainingQueue.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    public class TrainingQueue {
        public readonly ConcurrentQueue< TrainingQueueItem > Items = new ConcurrentQueue< TrainingQueueItem >();

        //public readonly ConcurrentStack<TrainingQueueItem> Items = new ConcurrentStack<TrainingQueueItem>();

        private readonly ManualResetEvent bob = new ManualResetEvent( false );

        public TrainingQueue() {
            this.bob.Reset();
        }

        public TrainingQueueItem Dequeue() {
            return this.Items.Remove();
        }

        public void Enqueue( TrainingQueueItem train ) {
            this.Items.Add( train );
            this.bob.Set();
        }

        /// <summary>
        ///     Waits until an item in is the queue.
        /// </summary>
        public void Wait() {
            //this.bob.Reset();
            while ( !this.Items.Any() ) {
                this.bob.WaitOne( TimeSpan.FromMilliseconds( 1234 ) );
            }
            this.bob.Reset();
        }

        public class TrainingQueueItem {
            public TrainingQueueItem( Object question, Object answer, Action action ) {
                this.Question = question;
                this.Answer = answer;
                this.Action = action;
            }

            public Action Action { get; set; }

            public object Answer { get; set; }

            public object Question { get; set; }

            public override String ToString() {
                return String.Format( "{0} -> {1}", this.Question, this.Answer );
            }
        }
    }
}
