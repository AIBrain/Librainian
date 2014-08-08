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
// "Librainian2/TimeStampQueue.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

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

        [DataMember]
        [OptionalField]
        public readonly ConcurrentQueue<WithTime<T>> Queue = new ConcurrentQueue<WithTime<T>>();

        public IEnumerable<T> Items { get { return this.Queue.Select( item => item.Item ); } }

        /// <summary>
        /// Adds the data to the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        public DateTime Add( T item ) {
            if ( null == item ) {
                return default( DateTime );
            }
            var newQI = new WithTime<T>( item: item );
            this.Queue.Enqueue( newQI );

            //this.bob.Set();
            return newQI.TimeStamp;
        }

        //private readonly ManualResetEventSlim bob = new ManualResetEventSlim( false );
        //private Atomic _AddedCounter { get; set; }
        //public Func<int> OnWait { get; set; }
        public void AddRange( IEnumerable<T> items ) {
            if ( null != items ) {
                Parallel.ForEach( items, obj => this.Add( obj ) );
            }
        }

        //TODO maybe use BlockingCollection?
        //public readonly BlockingCollection<ObjectWithTimeStamp<T>> Queue = new ConcurrentQueue<ObjectWithTimeStamp<T>>();
        public Boolean Contains( T value ) {
            return this.Queue.Any( q => Equals( q.Item, value ) );
        }

        public IEnumerator<WithTime<T>> GetEnumerator() {
            return this.Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns the next <see cref="T" /> in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        public T Next() {
            var temp = this.Pull();
            return null == temp ? default( T ) : temp.Item;
        }

        /// <summary>
        /// Does a Dequeue for each item in the <see cref="Queue" /> ?or null?
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> NextAll() {
            return this.Queue.Select( o => this.Next() );
        }

        /// <summary>
        /// Returns the next Object in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        public WithTime<T> Pull() {
            WithTime<T> temp;
            return this.Queue.TryDequeue( out temp ) ? temp : default( WithTime<T> );
        }

        ///// <summary>
        /////   wait until an item is added to the queue or until the timeout (default is 1 second)
        ///// </summary>
        //public void Stall( TimeSpan? timeToStall = null ) {
        //    if ( null == timeToStall ) {
        //        timeToStall = oneSecond;
        //    }
        //    if ( this.Any() || null == this.bob ) {
        //        return;
        //    }
        //    try {
        //        this.bob.WaitOne( timeout: timeToStall.Value );
        //    }
        //    catch ( Exception exception ) {
        //        exception.Log();
        //    }
        //}
    }
}