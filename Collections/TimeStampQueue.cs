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
// "Librainian/TimeStampQueue.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    [JsonObject]
    public class TimeStampQueue<T> : IEnumerable<WithTime<T>> where T : class {

        public IEnumerable<T> Items => this.Queue.Select( selector: item => item.Item );

        [JsonProperty]
        public ConcurrentQueue<WithTime<T>> Queue { get; } = new ConcurrentQueue<WithTime<T>>();

        /// <summary>
        /// Adds the data to the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        public DateTime Add( T item ) {
            if ( null == item ) {
                return default;
            }

            this.Queue.Enqueue( item: new WithTime<T>( item: item ) );

            return new WithTime<T>( item ).TimeStamp;
        }

        public void AddRange( params T[] items ) {
            if ( null != items ) {
                Parallel.ForEach( source: items, body: obj => this.Add( item: obj ) );
            }
        }

        public Boolean Contains( T value ) => this.Queue.Any( q => Equals( q.Item, value ) );

        public IEnumerator<WithTime<T>> GetEnumerator() => this.Queue.GetEnumerator();

        /// <summary>
        /// Returns the next <see cref="T"/> in the <see cref="Queue"/> or null.
        /// </summary>
        /// <returns></returns>
        public T Next() {
            var temp = this.Pull();

            return temp.Item;
        }

        /// <summary>
        /// Does a Dequeue for each item in the <see cref="Queue"/> ?or null?
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> NextAll() => this.Queue.Select( selector: o => this.Next() );

        /// <summary>
        /// Returns the next Object in the <see cref="Queue"/> or null.
        /// </summary>
        /// <returns></returns>
        public WithTime<T> Pull() => this.Queue.TryDequeue( result: out var temp ) ? temp : default;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}