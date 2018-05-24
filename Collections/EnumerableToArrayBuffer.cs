// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EnumerableToArrayBuffer.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/EnumerableToArrayBuffer.cs" was last formatted by Protiguous on 2018/05/22 at 5:40 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;

    public struct EnumerableToArrayBuffer<T> {

        private ICollection<T> _collection { get; }

        private Int32 _count { get; }

        private T[] _items { get; }

        internal Int32 Count => this._collection?.Count ?? this._count;

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
                    Buffer.BlockCopy( array, 0, destinationArray, 0, length );
                    array = destinationArray;
                }

                array[length] = local;
                length++;
            }

            this._items = array;
            this._count = length;
        }

        /// <summary>
        ///     Caller to guarantee items.Length &gt; index &gt;= 0
        /// </summary>
        internal void CopyTo( T[] items, Int32 index ) {
            if ( this._collection != null && this._collection.Count > 0 ) { this._collection.CopyTo( array: items, arrayIndex: index ); }
            else if ( this._count > 0 ) { Buffer.BlockCopy( this._items, 0, items, index, this._count ); }
        }

        internal T[] ToArray() {
            var count = this.Count;

            if ( count == 0 ) { return new T[0]; }

            T[] destinationArray;

            if ( this._collection is null ) {
                if ( this._items.Length == this._count ) { return this._items; }

                destinationArray = new T[this._count];
                Buffer.BlockCopy( this._items, 0, destinationArray, 0, this._count );

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