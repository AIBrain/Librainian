// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/TimeStampQueue.cs" was last cleaned by Rick on 2015/06/12 at 2:51 PM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    [DataContract( IsReference = true )]
    public class TimeStampQueue<T> : IEnumerable<WithTime<T>> where T : class {

        public IEnumerable<T> Items => this.Queue.Select( item => item.Item );

        [DataMember]
        public ConcurrentQueue<WithTime<T>> Queue {
            get;
        }
        = new ConcurrentQueue<WithTime<T>>();

        /// <summary>Adds the data to the queue.</summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        public DateTime Add(T item) {
            if ( null == item ) {
                return default(DateTime);
            }
            this.Queue.Enqueue( new WithTime<T>( item: item ) );

            //this.bob.Set();
            return new WithTime<T>( item: item ).TimeStamp;
        }

        //private readonly ManualResetEventSlim bob = new ManualResetEventSlim( false );
        //private Atomic _AddedCounter { get; set; }
        //public Func<int> OnWait { get; set; }
        public void AddRange(IEnumerable<T> items) {
            if ( null != items ) {
                Parallel.ForEach( items, obj => this.Add( obj ) );
            }
        }

        //TODO maybe use BlockingCollection?
        //public readonly BlockingCollection<ObjectWithTimeStamp<T>> Queue = new ConcurrentQueue<ObjectWithTimeStamp<T>>();
        public Boolean Contains(T value) => this.Queue.Any( q => Equals( q.Item, value ) );

        public IEnumerator<WithTime<T>> GetEnumerator() => this.Queue.GetEnumerator();

        /// <summary>Returns the next <see cref="T" /> in the <see cref="Queue" /> or null.</summary>
        /// <returns></returns>
        public T Next() {
            var temp = this.Pull();
            return temp.Item;
        }

        /// <summary>Does a Dequeue for each item in the <see cref="Queue" /> ?or null?</summary>
        /// <returns></returns>
        public IEnumerable<T> NextAll() => this.Queue.Select( o => this.Next() );

        /// <summary>Returns the next Object in the <see cref="Queue" /> or null.</summary>
        /// <returns></returns>
        public WithTime<T> Pull() {
            WithTime<T> temp;
            return this.Queue.TryDequeue( out temp ) ? temp : default(WithTime<T>);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}