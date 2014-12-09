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
// "Librainian/QueueWaiting.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class QueueWaiting< T > : IEnumerable< T > where T : class {
        private static readonly TimeSpan oneSecond = TimeSpan.FromSeconds( 1 );

        public QueueWaiting() {
            this.Queue = new ConcurrentQueue< T >();
            this.Wait = new AutoResetEvent( false );
        }

        private ConcurrentQueue< T > Queue { get; set; }

        private AutoResetEvent Wait { get; set; }

        #region IEnumerable<T> Members
        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator< T > GetEnumerator() => this.Queue.GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion IEnumerable<T> Members

        /// <summary>
        ///     Adds the data to the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the DateTime the data was queued.</returns>
        public QueueWaiting< T > Add( T item ) {
            if ( null != item ) {
                this.Queue.Enqueue( item: item );
                this.Wait.Set();
            }
            return this;
        }

        public QueueWaiting< T > AddRange( IEnumerable< T > items ) {
            foreach ( var item in items ) {
                this.Add( item: item );
            }
            return this;
        }

        /// <summary>
        ///     Pulls out the next <see cref="T" /> in the <see cref="Queue" /> or null.
        /// </summary>
        /// <returns></returns>
        public T Next() {
            T temp;
            return this.Queue.TryDequeue( out temp ) ? temp : default( T );
        }

        /// <summary>
        ///     Does a Dequeue for each item in the <see cref="Queue" /> ?or null?
        /// </summary>
        /// <returns></returns>
        public IEnumerable< T > NextAll() {
            return this.Queue.Select( o => this.Next() ).Where( o => default( T ) != o );
        }

        /// <summary>
        ///     Wait until:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Any <see cref="T" /> are already in the <see cref="Queue" />.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Any <see cref="T" /> is added to the <see cref="Queue" />.
        ///             </description>
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
            if ( null == timeToStall ) {
                timeToStall = oneSecond;
            }
            if ( this.Any() || null == this.Wait ) {
                return;
            }
            this.Wait.WaitOne( timeout: timeToStall.Value );
        }
    }
}
