// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "EnumerableToArrayBuffer.cs",
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
// "Librainian/Librainian/EnumerableToArrayBuffer.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;

    public struct EnumerableToArrayBuffer<T> {

        private readonly ICollection<T> _collection;

        private readonly Int32 _count;

        private readonly T[] _items;

        internal EnumerableToArrayBuffer( IEnumerable<T> source ) {
            T[] array = null;
            var length = 0;
            this._collection = source as ICollection<T>;

            if ( this._collection != null ) {
                this._items = null;
                this._count = 0;

                return;
            }

            foreach ( var local in source ) {
                if ( array is null ) { array = new T[4]; }
                else if ( array.Length == length ) {
                    var destinationArray = new T[length * 2];
                    Buffer.BlockCopy( sourceArray: array, sourceIndex: 0, destinationArray: destinationArray, destinationIndex: 0, length );
                    array = destinationArray;
                }

                array[length] = local;
                length++;
            }

            this._items = array;
            this._count = length;
        }

        internal Int32 Count => this._collection?.Count ?? this._count;

        /// <summary>
        ///     Caller to guarantee items.Length &gt; index &gt;= 0
        /// </summary>
        internal void CopyTo( T[] items, Int32 index ) {
            if ( this._collection != null && this._collection.Count > 0 ) { this._collection.CopyTo( array: items, arrayIndex: index ); }
            else if ( this._count > 0 ) { Buffer.BlockCopy( sourceArray: this._items, sourceIndex: 0, destinationArray: items, destinationIndex: index, this._count ); }
        }

        internal T[] ToArray() {
            var count = this.Count;

            if ( count == 0 ) { return new T[0]; }

            T[] destinationArray;

            if ( this._collection is null ) {
                if ( this._items.Length == this._count ) { return this._items; }

                destinationArray = new T[this._count];
                Buffer.BlockCopy( sourceArray: this._items, sourceIndex: 0, destinationArray: destinationArray, destinationIndex: 0, this._count );

                return destinationArray;
            }

            if ( this._collection is List<T> list ) { return list.ToArray(); }

            if ( this._collection is AbstractCollection<T> ac ) { return ac.ToArray(); }

            destinationArray = new T[count];
            this._collection.CopyTo( array: destinationArray, arrayIndex: 0 );

            return destinationArray;
        }
    }
}