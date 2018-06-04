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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "EnumerableToArrayBuffer.cs" was last formatted by Protiguous on 2018/06/04 at 3:43 PM.

namespace Librainian.Collections {

	using System;
	using System.Collections.Generic;

	[Experimental( "untested" )]
	public struct EnumerableToArrayBuffer<T> {

		private Int32 _count { get; }

		private ICollection<T> Collection { get; }

		private T[] Items { get; }

		internal Int32 Count => this.Collection?.Count ?? this._count;

		internal EnumerableToArrayBuffer( IEnumerable<T> source ) {
			T[] array = null;
			var length = 0;
			this.Collection = source as ICollection<T>;

			if ( this.Collection != null ) {
				this.Items = null;
				this._count = 0;

				return;
			}

			foreach ( var local in source ) {
				if ( array is null ) { array = new T[ 4 ]; }
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
			if ( this.Collection != null && this.Collection.Count > 0 ) { this.Collection.CopyTo( array: items, arrayIndex: index ); }
			else if ( this._count > 0 ) { Buffer.BlockCopy( this.Items, 0, items, index, this._count ); }
		}

		internal T[] ToArray() {
			var count = this.Count;

			if ( count == 0 ) { return new T[ 0 ]; }

			T[] destinationArray;

			if ( this.Collection is null ) {
				if ( this.Items.Length == this._count ) { return this.Items; }

				destinationArray = new T[ this._count ];
				Buffer.BlockCopy( this.Items, 0, destinationArray, 0, this._count );

				return destinationArray;
			}

			if ( this.Collection is List<T> list ) { return list.ToArray(); }

			destinationArray = new T[ count ];
			this.Collection.CopyTo( array: destinationArray, arrayIndex: 0 );

			return destinationArray;
		}

	}

}