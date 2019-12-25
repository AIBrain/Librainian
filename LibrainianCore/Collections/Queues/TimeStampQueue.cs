// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeStampQueue.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "TimeStampQueue.cs" was last formatted by Protiguous on 2019/08/08 at 6:36 AM.

namespace LibrainianCore.Collections.Queues {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    [JsonObject]
    public class TimeStampQueue<T> : IEnumerable<WithTime<T>> where T : class {

        public IEnumerator<WithTime<T>> GetEnumerator() => this.Queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [NotNull]
        public IEnumerable<T> Items => this.Queue.Select( selector: item => item.Item );

        [JsonProperty]
        public ConcurrentQueue<WithTime<T>> Queue { get; } = new ConcurrentQueue<WithTime<T>>();

        /// <summary>
        ///     Adds the data to the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        public DateTime Add( [CanBeNull] T item ) {
            if ( null == item ) {
                return default;
            }

            this.Queue.Enqueue( item: new WithTime<T>( item: item ) );

            return new WithTime<T>( item ).TimeStamp;
        }

        public void AddRange( [CanBeNull] params T[] items ) {
            if ( null != items ) {
                Parallel.ForEach( source: items, body: obj => this.Add( item: obj ) );
            }
        }

        public Boolean Contains( T value ) => this.Queue.Any( q => Equals( q.Item, value ) );

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
        [NotNull]
        public IEnumerable<T> NextAll() => this.Queue.Select( selector: o => this.Next() );

        /// <summary>
        ///     Returns the next Object in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public WithTime<T> Pull() => this.Queue.TryDequeue( result: out var temp ) ? temp : default;
    }
}