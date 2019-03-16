// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "QueueWaiting.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "QueueWaiting.cs" was last formatted by Protiguous on 2019/02/13 at 3:00 PM.

namespace Librainian.Collections.Queues {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using Magic;
    using Measurement.Time;

    public class QueueWaiting<T> : ABetterClassDispose, IEnumerable<T> where T : class {

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
        ///     collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator() => this.Queue.GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private ConcurrentQueue<T> Queue { get; } = new ConcurrentQueue<T>();

        private AutoResetEvent Wait { get; } = new AutoResetEvent( initialState: false );

        /// <summary>
        ///     Adds the data to the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        [NotNull]
        public QueueWaiting<T> Add( [CanBeNull] T item ) {
            if ( null != item ) {
                this.Queue.Enqueue( item: item );
                this.Wait.Set();
            }

            return this;
        }

        [NotNull]
        public QueueWaiting<T> AddRange( [NotNull] IEnumerable<T> items ) {
            foreach ( var item in items ) {
                this.Add( item: item );
            }

            return this;
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Wait.Dispose();

        /// <summary>
        ///     Pulls out the next <see cref="T" /> in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public T Next() => this.Queue.TryDequeue( result: out var temp ) ? temp : default;

        /// <summary>
        ///     Does a Dequeue for each item in the <see cref="Queue" /> ?or null?
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<T> NextAll() => this.Queue.Select( selector: o => this.Next() ).Where( o => default( T ) != o );

        /// <summary>
        ///     Wait until:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Any <see cref="T" /> are already in the <see cref="Queue" />.</description>
        ///         </item>
        ///         <item>
        ///             <description>Any <see cref="T" /> is added to the <see cref="Queue" />.</description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Until the
        ///                 <param name="timeToStall">(default is 1 second)</param>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public void Stall( TimeSpan? timeToStall = null ) {
            if ( this.Any() || null == this.Wait ) {
                return;
            }

            this.Wait.WaitOne( timeout: timeToStall ?? Seconds.One );
        }

    }

}