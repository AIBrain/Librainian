// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TrainingQueue.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Magic;

    public class TrainingQueue : ABetterClassDispose {
        public readonly ConcurrentQueue<TrainingQueueItem> Items = new ConcurrentQueue<TrainingQueueItem>();

        private readonly ManualResetEvent _bob = new ManualResetEvent( false );

		//public readonly ConcurrentStack<TrainingQueueItem> Items = new ConcurrentStack<TrainingQueueItem>();
		public TrainingQueue() => this._bob.Reset();

		public TrainingQueueItem Dequeue() => this.Items.Remove();

        public void Enqueue( TrainingQueueItem train ) {
            this.Items.Add( train );
            this._bob.Set();
        }

        /// <summary>Waits until an item in is the queue.</summary>
        public void Wait() {

            //this.bob.Reset();
            while ( !this.Items.Any() ) {
                this._bob.WaitOne( TimeSpan.FromMilliseconds( 1234 ) );
            }
            this._bob.Reset();
        }

        public class TrainingQueueItem {

            public TrainingQueueItem( Object question, Object answer, Action action ) {
                this.Question = question;
                this.Answer = answer;
                this.Action = action;
            }

            public Action Action {
                get; set;
            }

            public Object Answer {
                get;
            }

            public Object Question {
                get;
            }

            public override String ToString() => $"{this.Question} -> {this.Answer}";
        }

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this._bob.Dispose();

	}
}