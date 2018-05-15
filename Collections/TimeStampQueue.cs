// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "TimeStampQueue.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
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
// "Librainian/TimeStampQueue.cs" was last cleaned by Protiguous on 2018/05/15 at 1:29 AM.

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
            if ( null == item ) { return default; }

            this.Queue.Enqueue( item: new WithTime<T>( item: item ) );

            return new WithTime<T>( item ).TimeStamp;
        }

        public void AddRange( params T[] items ) {
            if ( null != items ) { Parallel.ForEach( source: items, body: obj => this.Add( item: obj ) ); }
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