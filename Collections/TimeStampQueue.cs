// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeStampQueue.cs" belongs to Rick@AIBrain.org and
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
//
// "Librainian/Librainian/TimeStampQueue.cs" was last formatted by Protiguous on 2018/05/21 at 10:52 PM.

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
        ///     Adds the data to the queue.
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
        ///     Returns the next <see cref="T" /> in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        public T Next() {
            var temp = this.Pull();

            return temp.Item;
        }

        /// <summary>
        ///     Does a Dequeue for each item in the <see cref="Queue" /> ?or null?
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> NextAll() => this.Queue.Select( selector: o => this.Next() );

        /// <summary>
        ///     Returns the next Object in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        public WithTime<T> Pull() => this.Queue.TryDequeue( result: out var temp ) ? temp : default;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}