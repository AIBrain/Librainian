// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "QueueWaiting.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/QueueWaiting.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Magic;
    using Measurement.Time;

    public class QueueWaiting<T> : ABetterClassDispose, IEnumerable<T> where T : class {

        private ConcurrentQueue<T> Queue { get; } = new ConcurrentQueue<T>();

        private AutoResetEvent Wait { get; } = new AutoResetEvent( initialState: false );

        /// <summary>
        ///     Adds the data to the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        public QueueWaiting<T> Add( T item ) {
            if ( null != item ) {
                this.Queue.Enqueue( item: item );
                this.Wait.Set();
            }

            return this;
        }

        public QueueWaiting<T> AddRange( IEnumerable<T> items ) {
            foreach ( var item in items ) { this.Add( item: item ); }

            return this;
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Wait.Dispose();

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
        ///     Pulls out the next <see cref="T" /> in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        public T Next() => this.Queue.TryDequeue( result: out var temp ) ? temp : default;

        /// <summary>
        ///     Does a Dequeue for each item in the <see cref="Queue" /> ?or null?
        /// </summary>
        /// <returns></returns>
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
        ///             <description>Until the
        ///                 <param name="timeToStall">(default is 1 second)</param>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public void Stall( TimeSpan? timeToStall = null ) {
            if ( this.Any() || null == this.Wait ) { return; }

            this.Wait.WaitOne( timeout: timeToStall ?? Seconds.One );
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}