// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ThreadSafeList.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ThreadSafeList.cs" was last formatted by Protiguous on 2018/11/20 at 10:30 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>
    ///     Just a simple thread safe collection. Doesn't scale well because of the locks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <value>Version 1.8</value>
    /// <remarks>TODO replace locks with AsyncLocks</remarks>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public sealed class ThreadSafeList<T> : IList<T> {

        /// <summary>
        ///     TODO replace the locks with a ReaderWriterLockSlim
        /// </summary>
        [JsonProperty]
        private List<T> Items { get; } = new List<T>();

        public Int32 Count {
            get {
                lock ( this.Items ) {
                    return this.Items.Count;
                }
            }
        }

        public Boolean IsReadOnly => false;

        public Int64 LongCount {
            get {
                lock ( this.Items ) {
                    return this.Items.LongCount();
                }
            }
        }

        public T this[ Int32 index ] {
            get {
                lock ( this.Items ) {
                    return this.Items[ index: index ];
                }
            }

            set {
                lock ( this.Items ) {
                    this.Items[ index: index ] = value;
                }
            }
        }

        public ThreadSafeList( [CanBeNull] IEnumerable<T> items = null ) => this.AddRange( collection: items );

        public void Add( T item ) {
            lock ( this.Items ) {
                this.Items.Add( item: item );
            }
        }

        [NotNull]
        public Task AddAsync( T item ) =>
            Task.Run( () => {
                this.TryAdd( item: item );
            } );

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void AddRange( [CanBeNull] IEnumerable<T> collection, Boolean asParallel = true ) {
            if ( null == collection ) {
                return;
            }

            lock ( this.Items ) {
                this.Items.AddRange( collection: asParallel ? collection.AsParallel() : collection );
            }
        }

        public void Clear() {
            lock ( this.Items ) {
                this.Items.Clear();
            }
        }

        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public List<T> Clone( Boolean asParallel = false /*is order guaranteed if true? Based upon ParallelEnumerableWrapper it seems it would be.*/ ) {
            lock ( this.Items ) {
                return asParallel ? new List<T>( collection: this.Items.AsParallel() ) : new List<T>( collection: this.Items );
            }
        }

        public Boolean Contains( T item ) {
            lock ( this.Items ) {
                return this.Items.Contains( item: item );
            }
        }

        public void CopyTo( T[] array, Int32 arrayIndex ) {
            lock ( this.Items ) {
                this.Items.CopyTo( array: array, arrayIndex: arrayIndex );
            }
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
        public void ForAll( [NotNull] Action<T> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false ) {
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
                    Parallel.ForEach( source: clones, body: wrapper );
                }
                else {
                    clones.ForEach( wrapper );
                }
            }
            else {
                lock ( this.Items ) {
                    if ( asParallel ) {
                        this.Items.AsParallel().ForAll( wrapper );
                    }
                    else if ( inParallel ) {
                        Parallel.ForEach( source: this.Items, body: wrapper );
                    }
                    else {
                        this.Items.ForEach( wrapper );
                    }
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
        public void ForEach( [NotNull] Action<T> action, Boolean performActionOnClones = true, Boolean asParallel = true, Boolean inParallel = false ) {
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
                    Parallel.ForEach( source: clones, body: wrapper );
                }
                else {
                    clones.ForEach( wrapper );
                }
            }
            else {
                lock ( this.Items ) {
                    if ( asParallel ) {
                        this.Items.AsParallel().ForAll( wrapper );
                    }
                    else if ( inParallel ) {
                        Parallel.ForEach( source: this.Items, body: wrapper );
                    }
                    else {
                        this.Items.ForEach( wrapper );
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator();

        public Int32 IndexOf( T item ) {
            lock ( this.Items ) {
                return this.Items.IndexOf( item: item );
            }
        }

        public void Insert( Int32 index, T item ) {
            lock ( this.Items ) {
                this.Items.Insert( index: index, item: item );
            }
        }

        public Boolean Remove( T item ) {
            lock ( this.Items ) {
                return this.Items.Remove( item: item );
            }
        }

        public void RemoveAt( Int32 index ) {
            lock ( this.Items ) {
                this.Items.RemoveAt( index: index );
            }
        }

        public Boolean TryAdd( T item ) {
            try {
                lock ( this.Items ) {
                    this.Items.Add( item: item );

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
            lock ( this.Items ) {
                var count = this.Items.Count;

                if ( count >= 1 ) {
                    var idx = 0.Next( maxValue: count );
                    item = this.Items[ index: idx ];
                    this.Items.RemoveAt( index: idx );

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
        public Boolean TryTakeOneCopyRest( out T item, [CanBeNull] out List<T> rest ) {
            lock ( this.Items ) {
                var count = this.Items.Count;

                if ( count >= 1 ) {
                    item = this.Items[ index: 0 ];
                    this.Items.RemoveAt( index: 0 );
                    rest = new List<T>( collection: this.Items );

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