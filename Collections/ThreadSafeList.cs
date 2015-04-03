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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ThreadSafeList.cs" was last cleaned by Rick on 2014/08/13 at 10:37 PM

#endregion License & Information

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Threading;

    /// <summary>
    ///     Just a simple thread safe collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <value>Version 1.7</value>
    /// <remarks>TODO replace locks with AsyncLocks</remarks>
    [CollectionDataContract]
	
	[DebuggerDisplay( "Count={Count}" )]
    public sealed class ThreadSafeList<T> : IList<T> {

        /// <summary>
        ///     TODO replace the locks with a ReaderWriterLockSlim
        /// </summary>
        [DataMember]
        private readonly List<T> _items = new List<T>();

        public ThreadSafeList( IEnumerable<T> items = null ) {
            this.AddRange( items );
        }

        public long LongCount {
            get {
                lock ( this._items ) {
                    return this._items.LongCount();
                }
            }
        }

        public int Count {
            get {
                lock ( this._items ) {
                    return this._items.Count;
                }
            }
        }

        public Boolean IsReadOnly => false;

        public T this[ int index ] {
            get {
                lock ( this._items ) {
                    return this._items[ index ];
                }
            }

            set {
                lock ( this._items ) {
                    this._items[ index ] = value;
                }
            }
        }

        public void Add( T item ) {
            lock ( this._items ) {
                this._items.Add( item );
            }
        }

        public void Clear() {
            lock ( this._items ) {
                this._items.Clear();
            }
        }

        public Boolean Contains( T item ) {
            lock ( this._items ) {
                return this._items.Contains( item );
            }
        }

        public void CopyTo( T[] array, int arrayIndex ) {
            lock ( this._items ) {
                this._items.CopyTo( array, arrayIndex );
            }
        }

        public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int IndexOf( T item ) {
            lock ( this._items ) {
                return this._items.IndexOf( item );
            }
        }

        public void Insert( int index, T item ) {
            lock ( this._items ) {
                this._items.Insert( index, item );
            }
        }

        public Boolean Remove( T item ) {
            lock ( this._items ) {
                return this._items.Remove( item );
            }
        }

        public void RemoveAt( int index ) {
            lock ( this._items ) {
                this._items.RemoveAt( index );
            }
        }

        public Task AddAsync( T item ) => Task.Run( () => {
                                                        this.TryAdd( item );
                                                    } );

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void AddRange( IEnumerable<T> collection, Boolean asParallel = true ) {
            if ( null == collection ) {
                return;
            }
            lock ( this._items ) {
                this._items.AddRange( asParallel ? collection.AsParallel() : collection );
            }
        }

        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        public List<T> Clone( Boolean asParallel = true ) {
            lock ( this._items ) {
                return asParallel ? new List<T>( this._items.AsParallel() ) : new List<T>( this._items );
            }
        }

        /// <summary>
        ///     Perform the <paramref name="action" /> on each item in the list.
        /// </summary>
        /// <param name="action"><paramref name="action" /> to perform on each item.</param>
        /// <param name="performActionOnClones">
        ///     If true, the <paramref name="action" /> will be performed on a <see cref="Clone" /> of
        ///     the items.
        /// </param>
        /// <param name="asParallel">Use the <see cref="ParallelQuery{TSource}" /> method.</param>
        /// <param name="inParallel">
        ///     Use the
        ///     <see
        ///         cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Action{TSource})" />
        ///     method.
        /// </param>
        public void ForAll( Action<T> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false ) {
            if ( action == null ) {
                throw new ArgumentNullException( nameof( action ) );
            }
            var wrapper = new Action<T>( obj => {
                try {
                    action( obj );
                }
                catch ( ArgumentNullException ) {
                    //if a null gets into the list then swallow an ArgumentNullException so we can continue adding
                }
            } );
            if ( performActionOnClones ) {
                var clones = this.Clone( asParallel: asParallel );
                if ( asParallel ) {
                    clones.AsParallel().ForAll( wrapper );
                }
                else if ( inParallel ) {
                    Parallel.ForEach( clones, wrapper );
                }
                else {
                    clones.ForEach( wrapper );
                }
            }
            else {
                lock ( this._items ) {
                    if ( asParallel ) {
                        this._items.AsParallel().ForAll( wrapper );
                    }
                    else if ( inParallel ) {
                        Parallel.ForEach( this._items, wrapper );
                    }
                    else {
                        this._items.ForEach( wrapper );
                    }
                }
            }
        }

        //    return Task.Factory.StartNew( async () => {
        //        collection.AsParallel()
        //                  .ForAll( item => produce.SendAsync( item ) );
        //        produce.Complete();
        //        await consume.Completion;
        //    } );
        //}
        /// <summary>
        ///     Perform the <paramref name="action" /> on each item in the list.
        /// </summary>
        /// <param name="action">
        ///     <paramref name="action" /> to perform on each item.
        /// </param>
        /// <param name="performActionOnClones">
        ///     If true, the <paramref name="action" /> will be performed on a <see cref="Clone" /> of the items.
        /// </param>
        /// <param name="asParallel">
        ///     Use the <see cref="ParallelQuery{TSource}" /> method.
        /// </param>
        /// <param name="inParallel">
        ///     Use the
        ///     <see
        ///         cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Action{TSource})" />
        ///     method.
        /// </param>
        public void ForEach( Action<T> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false ) {
            if ( action == null ) {
                throw new ArgumentNullException( nameof( action ) );
            }
            var wrapper = new Action<T>( obj => {
                try {
                    action( obj );
                }
                catch ( ArgumentNullException ) {
                    //if a null gets into the list then swallow an ArgumentNullException so we can continue adding
                }
            } );
            if ( performActionOnClones ) {
                var clones = this.Clone( asParallel: asParallel );
                if ( asParallel ) {
                    clones.AsParallel().ForAll( wrapper );
                }
                else if ( inParallel ) {
                    Parallel.ForEach( clones, wrapper );
                }
                else {
                    clones.ForEach( wrapper );
                }
            }
            else {
                lock ( this._items ) {
                    if ( asParallel ) {
                        this._items.AsParallel().ForAll( wrapper );
                    }
                    else if ( inParallel ) {
                        Parallel.ForEach( this._items, wrapper );
                    }
                    else {
                        this._items.ForEach( wrapper );
                    }
                }
            }
        }

        public Boolean TryAdd( T item ) {
            try {
                lock ( this._items ) {
                    this._items.Add( item );
                    return true;
                }
            }
            catch ( NullReferenceException ) {
            }
            catch ( ObjectDisposedException ) {
            }
            catch ( ArgumentNullException ) {
            }
            catch ( ArgumentOutOfRangeException ) {
            }
            catch ( ArgumentException ) {
            }
            return false;
        }

        public Boolean TryTake( out T item ) {
            lock ( this._items ) {
                var count = this._items.Count;
                if ( count >= 1 ) {
                    var idx = Randem.Next( 0, count );
                    item = this._items[ idx ];
                    this._items.RemoveAt( idx );
                    return true;
                }
            }
            item = default( T );
            return false;
        }

        /// <summary>
        ///     Remove one item, and return a list-copy of the rest.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public Boolean TryTakeOneCopyRest( out T item, out List<T> rest ) {
            lock ( this._items ) {
                var count = this._items.Count;
                if ( count >= 1 ) {
                    item = this._items[ 0 ];
                    this._items.RemoveAt( 0 );
                    rest = new List<T>( this._items );
                    return true;
                }
            }
            item = default( T );
            rest = default( List<T> );
            return false;
        }

        ///// <summary>
        /////     Add in an enumerable of items.
        ///// </summary>
        ///// <param name="collection"></param>
        //[Obsolete]
        //public Task AddAsync( IEnumerable<T> collection ) {
        //    if ( collection == null ) {
        //        throw new ArgumentNullException( "collection" );
        //    }

        // var produce = new TransformBlock<T, T>( item => item,
        // Blocks.ManyProducers.ConsumeSensible );

        // var consume = new ActionBlock<T>( action: async obj => await this.AddAsync( obj ),
        // dataflowBlockOptions: new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism =
        // Threads.ProcessorCount } ); produce.LinkTo( consume );
    }
}