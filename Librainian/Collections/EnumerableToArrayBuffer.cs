// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EnumerableToArrayBuffer.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "EnumerableToArrayBuffer.cs" was last formatted by Protiguous on 2019/08/08 at 6:30 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    [Experimental( "untested" )]
    public struct EnumerableToArrayBuffer<T> {

        private Int32 _count { get; }

        private ICollection<T> Collection { get; }

        private T[] Items { get; }

        internal Int32 Count => this.Collection?.Count ?? this._count;

        internal EnumerableToArrayBuffer( [NotNull] IEnumerable<T> source ) {
            T[] array = null;
            var length = 0;
            this.Collection = source as ICollection<T>;

            if ( this.Collection != null ) {
                this.Items = null;
                this._count = 0;

                return;
            }

            foreach ( var local in source ) {
                if ( array == null ) {
                    array = new T[ 4 ];
                }
                else if ( array.Length == length ) {
                    var destinationArray = new T[ length * 2 ];
                    Buffer.BlockCopy( array, 0, destinationArray, 0, length );
                    array = destinationArray;
                }

                array[ length ] = local;
                length++;
            }

            this.Items = array;
            this._count = length;
        }

        /// <summary>
        ///     Caller to guarantee items.Length &gt; index &gt;= 0
        /// </summary>
        internal void CopyTo( T[] items, Int32 index ) {
            if ( this.Collection != null && this.Collection.Count > 0 ) {
                this.Collection.CopyTo( array: items, arrayIndex: index );
            }
            else if ( this._count > 0 ) {
                Buffer.BlockCopy( this.Items, 0, items, index, this._count );
            }
        }

        [NotNull]
        internal T[] ToArray() {
            var count = this.Count;

            if ( count == 0 ) {
                return new T[ 0 ];
            }

            T[] destinationArray;

            switch ( this.Collection ) {
                case null when this.Items.Length == this._count: return this.Items;

                case null:
                    destinationArray = new T[ this._count ];
                    Buffer.BlockCopy( this.Items, 0, destinationArray, 0, this._count );

                    return destinationArray;

                case List<T> list: return list.ToArray();
            }

            destinationArray = new T[ count ];
            this.Collection.CopyTo( array: destinationArray, arrayIndex: 0 );

            return destinationArray;
        }
    }
}