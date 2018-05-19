// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ThreadSafeList.cs",
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
// "Librainian/Librainian/ThreadSafeList.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>
    ///     Just a simple thread safe collection. Doesn't scale well because of the locks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <value>Version 1.8</value>
    /// <remarks>TODO replace locks with AsyncLocks</remarks>
    [JsonObject]
    [DebuggerDisplay( "Count={" + nameof( Count ) + "}" )]
    public sealed class ThreadSafeList<T> : IList<T> {

        /// <summary>
        ///     TODO replace the locks with a ReaderWriterLockSlim
        /// </summary>
        [JsonProperty]
        private readonly List<T> _items = new List<T>();

        public ThreadSafeList( IEnumerable<T> items = null ) => this.AddRange( collection: items );

        public Int32 Count {
            get {
                lock ( this._items ) { return this._items.Count; }
            }
        }

        public Boolean IsReadOnly => false;

        public Int64 LongCount {
            get {
                lock ( this._items ) { return this._items.LongCount(); }
            }
        }

        public T this[Int32 index] {
            get {
                lock ( this._items ) { return this._items[index: index]; }
            }

            set {
                lock ( this._items ) { this._items[index: index] = value; }
            }
        }

        public void Add( T item ) {
            lock ( this._items ) { this._items.Add( item: item ); }
        }

        public async Task AddAsync( T item ) => await Task.Run( () => { this.TryAdd( item: item ); } ).NoUI();

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void AddRange( IEnumerable<T> collection, Boolean asParallel = true ) {
            if ( null == collection ) { return; }

            lock ( this._items ) { this._items.AddRange( collection: asParallel ? collection.AsParallel() : collection ); }
        }

        public void Clear() {
            lock ( this._items ) { this._items.Clear(); }
        }

        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        public List<T> Clone( Boolean asParallel = false /*is order guaranteed if true? Based upon ParallelEnumerableWrapper it seems it would be.*/ ) {
            lock ( this._items ) { return asParallel ? new List<T>( collection: this._items.AsParallel() ) : new List<T>( collection: this._items ); }
        }

        public Boolean Contains( T item ) {
            lock ( this._items ) { return this._items.Contains( item: item ); }
        }

        public void CopyTo( T[] array, Int32 arrayIndex ) {
            lock ( this._items ) { this._items.CopyTo( array: array, arrayIndex: arrayIndex ); }
        }

        /// <summary>
        ///     Perform the <paramref name="action" /> on each item in the list.
        /// </summary>
        /// <param name="action">               <paramref name="action" /> to perform on each item.</param>
        /// <param name="performActionOnClones">
        ///     If true, the <paramref name="action" /> will be performed on a <see cref="Clone" />
        ///     of the items.
        /// </param>
        /// <param name="asParallel">           Use the <see cref="ParallelQuery{TSource}" /> method.</param>
        /// <param name="inParallel">
        ///     Use the
        ///     <see cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Action{TSource})" />
        ///     method.
        /// </param>
        public void ForAll( Action<T> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false ) {
            if ( action is null ) { throw new ArgumentNullException( nameof( action ) ); }

            var wrapper = new Action<T>( obj => {
                try { action( obj ); }
                catch ( ArgumentNullException ) {

                    //if a null gets into the list then swallow an ArgumentNullException so we can continue adding
                }
            } );

            if ( performActionOnClones ) {
                var clones = this.Clone( asParallel: asParallel );

                if ( asParallel ) { clones.AsParallel().ForAll( wrapper ); }
                else if ( inParallel ) { Parallel.ForEach( source: clones, body: wrapper ); }
                else { clones.ForEach( wrapper ); }
            }
            else {
                lock ( this._items ) {
                    if ( asParallel ) { this._items.AsParallel().ForAll( wrapper ); }
                    else if ( inParallel ) { Parallel.ForEach( source: this._items, body: wrapper ); }
                    else { this._items.ForEach( wrapper ); }
                }
            }
        }

        //}
        /// <summary>
        ///     Perform the <paramref name="action" /> on each item in the list.
        /// </summary>
        /// <param name="action">               <paramref name="action" /> to perform on each item.</param>
        /// <param name="performActionOnClones">
        ///     If true, the <paramref name="action" /> will be performed on a <see cref="Clone" />
        ///     of the items.
        /// </param>
        /// <param name="asParallel">           Use the <see cref="ParallelQuery{TSource}" /> method.</param>
        /// <param name="inParallel">
        ///     Use the
        ///     <see cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Action{TSource})" />
        ///     method.
        /// </param>
        public void ForEach( Action<T> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false ) {
            if ( action is null ) { throw new ArgumentNullException( nameof( action ) ); }

            var wrapper = new Action<T>( obj => {
                try { action( obj ); }
                catch ( ArgumentNullException ) {

                    //if a null gets into the list then swallow an ArgumentNullException so we can continue adding
                }
            } );

            if ( performActionOnClones ) {
                var clones = this.Clone( asParallel: asParallel );

                if ( asParallel ) { clones.AsParallel().ForAll( wrapper ); }
                else if ( inParallel ) { Parallel.ForEach( source: clones, body: wrapper ); }
                else { clones.ForEach( wrapper ); }
            }
            else {
                lock ( this._items ) {
                    if ( asParallel ) { this._items.AsParallel().ForAll( wrapper ); }
                    else if ( inParallel ) { Parallel.ForEach( source: this._items, body: wrapper ); }
                    else { this._items.ForEach( wrapper ); }
                }
            }
        }

        public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator();

        public Int32 IndexOf( T item ) {
            lock ( this._items ) { return this._items.IndexOf( item: item ); }
        }

        public void Insert( Int32 index, T item ) {
            lock ( this._items ) { this._items.Insert( index: index, item: item ); }
        }

        public Boolean Remove( T item ) {
            lock ( this._items ) { return this._items.Remove( item: item ); }
        }

        public void RemoveAt( Int32 index ) {
            lock ( this._items ) { this._items.RemoveAt( index: index ); }
        }

        public Boolean TryAdd( T item ) {
            try {
                lock ( this._items ) {
                    this._items.Add( item: item );

                    return true;
                }
            }
            catch ( NullReferenceException ) { }
            catch ( ObjectDisposedException ) { }
            catch ( ArgumentNullException ) { }
            catch ( ArgumentOutOfRangeException ) { }
            catch ( ArgumentException ) { }

            return false;
        }

        public Boolean TryTake( out T item ) {
            lock ( this._items ) {
                var count = this._items.Count;

                if ( count >= 1 ) {
                    var idx = 0.Next( maxValue: count );
                    item = this._items[index: idx];
                    this._items.RemoveAt( index: idx );

                    return true;
                }
            }

            item = default;

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
                    item = this._items[index: 0];
                    this._items.RemoveAt( index: 0 );
                    rest = new List<T>( collection: this._items );

                    return true;
                }
            }

            item = default;
            rest = default;

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}