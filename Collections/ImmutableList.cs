// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "ImmutableList.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/ImmutableList.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>
    /// A list that has been written to be observationally immutable. A mutable array is used as the backing store for the list, but no mutable operations are offered.
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the list.</typeparam>
    /// <seealso cref="http://joeduffyblog.com/2007/11/11/immutable-types-for-c/"/>
    [Immutable]
    public sealed class ImmutableList<T> : IList<T> {

        private readonly T[] _mArray;

        /// <summary>
        /// Create a new list.
        /// </summary>
        public ImmutableList() => this._mArray = new T[0];

        /// <summary>
        /// Create a new list, copying elements from the specified array.
        /// </summary>
        /// <param name="arrayToCopy">An array whose contents will be copied.</param>
        public ImmutableList( [NotNull] T[] arrayToCopy ) {
            if ( arrayToCopy is null ) { throw new ArgumentNullException( nameof( arrayToCopy ) ); }

            this._mArray = new T[arrayToCopy.Length];
            Array.Copy( sourceArray: arrayToCopy, destinationArray: this._mArray, arrayToCopy.Length );
        }

        /// <summary>
        /// Create a new list, copying elements from the specified enumerable.
        /// </summary>
        /// <param name="enumerableToCopy">An enumerable whose contents will be copied.</param>
        public ImmutableList( IEnumerable<T> enumerableToCopy ) => this._mArray = enumerableToCopy.ToArray();

        /// <summary>
        /// Retrieves the immutable count of the list.
        /// </summary>
        public Int32 Count => this._mArray.Length;

        /// <summary>
        /// Whether the list is read only: because the list is immutable, this is always true.
        /// </summary>
        public Boolean IsReadOnly => true;

        /// <summary>
        /// Accesses the element at the specified index. Because the list is immutable, the setter will always throw an exception.
        /// </summary>
        /// <param name="index">The index to access.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[Int32 index] {
            get => this._mArray[index];

            // ReSharper disable once ValueParameterNotUsed
            set => ThrowMutableException( copyMethod: "CopyAndSet" );
        }

        /// <summary>
        /// A helper method used below when a mutable method is accessed. Several operations on the collections interfaces IList&lt;T&gt; and ICollection&lt;T&gt; are mutable, so we cannot support them. We offer immutable
        /// versions of each.
        /// </summary>
        private static void ThrowMutableException( String copyMethod ) => throw new InvalidOperationException( message: $"Cannot mutate an immutable list; see copying method ‘{copyMethod}’" );

        /// <summary>
        /// Checks whether the specified item is contained in the list.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is found, false otherwise.</returns>
        public Boolean Contains( T item ) => Array.IndexOf( array: this._mArray, value: item ) != -1;

        /// <summary>
        /// Copies the list and adds a new value at the end.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList<T> CopyAndAdd( T value ) {
            var newArray = new T[this._mArray.Length + 1];
            this._mArray.CopyTo( array: newArray, index: 0 );
            newArray[this._mArray.Length] = value;

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>
        /// Returns a new, cleared (empty) immutable list.
        /// </summary>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList<T> CopyAndClear() => new ImmutableList<T>( arrayToCopy: new T[0] );

        /// <summary>
        /// Copies the list adn inserts a particular element.
        /// </summary>
        /// <param name="index">The index at which to insert an element.</param>
        /// <param name="item"> The element to insert.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList<T> CopyAndInsert( Int32 index, T item ) {
            var newArray = new T[this._mArray.Length + 1];
            Array.Copy( sourceArray: this._mArray, destinationArray: newArray, index );
            newArray[index] = item;
            Array.Copy( sourceArray: this._mArray, sourceIndex: index, destinationArray: newArray, destinationIndex: index + 1, this._mArray.Length - index );

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>
        /// Copies the list and removes a particular element.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList<T> CopyAndRemove( T item ) {
            var index = this.IndexOf( item: item );

            if ( index == -1 ) { throw new ArgumentException( message: "Item not found in list." ); }

            return this.CopyAndRemoveAt( index: index );
        }

        /// <summary>
        /// Copies the list and removes a particular element.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList<T> CopyAndRemoveAt( Int32 index ) {
            var newArray = new T[this._mArray.Length - 1];
            Array.Copy( sourceArray: this._mArray, destinationArray: newArray, index );
            Array.Copy( sourceArray: this._mArray, sourceIndex: index + 1, destinationArray: newArray, destinationIndex: index, this._mArray.Length - index - 1 );

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>
        /// Copies the list and modifies the specific value at the index provided.
        /// </summary>
        /// <param name="index">The index whose value is to be changed.</param>
        /// <param name="item"> The value to store at the specified index.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList<T> CopyAndSet( Int32 index, T item ) {
            var newArray = new T[this._mArray.Length];
            this._mArray.CopyTo( array: newArray, index: 0 );
            newArray[index] = item;

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>
        /// Copies the contents of this list to a destination array.
        /// </summary>
        /// <param name="array">The array to copy elements to.</param>
        /// <param name="index">The index at which copying begins.</param>
        public void CopyTo( T[] array, Int32 index ) => this._mArray.CopyTo( array: array, index: index );

        /// <summary>
        /// Retrieves an enumerator for the list’s collections.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<T> GetEnumerator() => ( ( IEnumerable<T> )this._mArray ).GetEnumerator();

        /// <summary>
        /// Finds the index of the specified element.
        /// </summary>
        /// <param name="item">An item to search for.</param>
        /// <returns>The index of the item, or -1 if it was not found.</returns>
        public Int32 IndexOf( T item ) => Array.IndexOf( array: this._mArray, value: item );

        /// <summary>
        /// This method is unsupported on this type, because it is immutable.
        /// </summary>
        void ICollection<T>.Add( T item ) => ThrowMutableException( copyMethod: "CopyAndAdd" );

        /// <summary>
        /// This method is unsupported on this type, because it is immutable.
        /// </summary>
        void ICollection<T>.Clear() => ThrowMutableException( copyMethod: "CopyAndClear" );

        /// <summary>
        /// Retrieves an enumerator for the list’s collections.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// This method is unsupported on this type, because it is immutable.
        /// </summary>
        void IList<T>.Insert( Int32 index, T item ) => ThrowMutableException( copyMethod: "CopyAndInsert" );

        /// <summary>
        /// This method is unsupported on this type, because it is immutable.
        /// </summary>
        Boolean ICollection<T>.Remove( T item ) {
            ThrowMutableException( copyMethod: "CopyAndRemove" );

            return false;
        }

        /// <summary>
        /// This method is unsupported on this type, because it is immutable.
        /// </summary>
        void IList<T>.RemoveAt( Int32 index ) => ThrowMutableException( copyMethod: "CopyAndRemoveAt" );
    }
}