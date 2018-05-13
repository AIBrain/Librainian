// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/EnumerableToArrayBuffer.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

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
                if ( array is null ) {
                    array = new T[4];
                }
                else if ( array.Length == length ) {
                    var destinationArray = new T[length * 2];
                    Array.Copy( sourceArray: array, sourceIndex: 0, destinationArray: destinationArray, destinationIndex: 0, length );
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
        /// Caller to guarantee items.Length &gt; index &gt;= 0
        /// </summary>
        internal void CopyTo( T[] items, Int32 index ) {
            if ( this._collection != null && this._collection.Count > 0 ) {
                this._collection.CopyTo( array: items, arrayIndex: index );
            }
            else if ( this._count > 0 ) {
                Array.Copy( sourceArray: this._items, sourceIndex: 0, destinationArray: items, destinationIndex: index, this._count );
            }
        }

        internal T[] ToArray() {
            var count = this.Count;

            if ( count == 0 ) {
                return new T[0];
            }

            T[] destinationArray;

            if ( this._collection is null ) {
                if ( this._items.Length == this._count ) {
                    return this._items;
                }

                destinationArray = new T[this._count];
                Array.Copy( sourceArray: this._items, sourceIndex: 0, destinationArray: destinationArray, destinationIndex: 0, this._count );

                return destinationArray;
            }

            if ( this._collection is List<T> list ) {
                return list.ToArray();
            }

            if ( this._collection is AbstractCollection<T> ac ) {
                return ac.ToArray();
            }

            destinationArray = new T[count];
            this._collection.CopyTo( array: destinationArray, arrayIndex: 0 );

            return destinationArray;
        }
    }
}