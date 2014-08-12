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
// "Librainian/ImmutableList.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Annotations;
    using Extensions;

    /// <summary>
    ///     A list that has been written to be observationally immutable.  A mutable array
    ///     is used as the backing store for the list, but no mutable operations are offered.
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the list.</typeparam>
    /// <seealso cref="http://joeduffyblog.com/2007/11/11/immutable-types-for-c/" />
    [Immutable]
    public sealed class ImmutableList< T > : IList< T > {
        private readonly T[] _mArray;

        /// <summary>
        ///     Create a new list.
        /// </summary>
        public ImmutableList() {
            this._mArray = new T[0];
        }

        /// <summary>
        ///     Create a new list, copying elements from the specified array.
        /// </summary>
        /// <param name="arrayToCopy">An array whose contents will be copied.</param>
        public ImmutableList( [NotNull] T[] arrayToCopy ) {
            if ( arrayToCopy == null ) {
                throw new ArgumentNullException( "arrayToCopy" );
            }
            this._mArray = new T[arrayToCopy.Length];
            Array.Copy( arrayToCopy, this._mArray, arrayToCopy.Length );
        }

        /// <summary>
        ///     Create a new list, copying elements from the specified enumerable.
        /// </summary>
        /// <param name="enumerableToCopy">
        ///     An enumerable whose contents will
        ///     be copied.
        /// </param>
        public ImmutableList( [NotNull] IEnumerable< T > enumerableToCopy ) {
            if ( enumerableToCopy == null ) {
                throw new ArgumentNullException( "enumerableToCopy" );
            }
            this._mArray = new List< T >( enumerableToCopy ).ToArray();
        }

        /// <summary>
        ///     Retrieves the immutable count of the list.
        /// </summary>
        public int Count { get { return this._mArray.Length; } }

        /// <summary>
        ///     Whether the list is read only: because the list is immutable, this
        ///     is always true.
        /// </summary>
        public Boolean IsReadOnly { get { return true; } }

        /// <summary>
        ///     Accesses the element at the specified index.  Because the list is
        ///     immutable, the setter will always throw an exception.
        /// </summary>
        /// <param name="index">The index to access.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[ int index ] {
            get { return this._mArray[ index ]; }
// ReSharper disable once ValueParameterNotUsed
            set { ThrowMutableException( "CopyAndSet" ); }
        }

        /// <summary>
        ///     Checks whether the specified item is contained in the list.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is found, false otherwise.</returns>
        public Boolean Contains( T item ) {
            return Array.IndexOf( this._mArray, item ) != -1;
        }

        /// <summary>
        ///     Copies the contents of this list to a destination array.
        /// </summary>
        /// <param name="array">The array to copy elements to.</param>
        /// <param name="index">The index at which copying begins.</param>
        public void CopyTo( T[] array, int index ) {
            this._mArray.CopyTo( array, index );
        }

        /// <summary>
        ///     Retrieves an enumerator for the list’s collections.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator< T > GetEnumerator() {
            return ( ( IEnumerable< T > ) this._mArray ).GetEnumerator();
        }

        /// <summary>
        ///     This method is unsupported on this type, because it is immutable.
        /// </summary>
        void ICollection< T >.Add( T item ) {
            ThrowMutableException( "CopyAndAdd" );
        }

        /// <summary>
        ///     This method is unsupported on this type, because it is immutable.
        /// </summary>
        void ICollection< T >.Clear() {
            ThrowMutableException( "CopyAndClear" );
        }

        /// <summary>
        ///     This method is unsupported on this type, because it is immutable.
        /// </summary>
        Boolean ICollection< T >.Remove( T item ) {
            ThrowMutableException( "CopyAndRemove" );
            return false;
        }

        /// <summary>
        ///     Retrieves an enumerator for the list’s collections.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     This method is unsupported on this type, because it is immutable.
        /// </summary>
        void IList< T >.Insert( int index, T item ) {
            ThrowMutableException( "CopyAndInsert" );
        }

        /// <summary>
        ///     This method is unsupported on this type, because it is immutable.
        /// </summary>
        void IList< T >.RemoveAt( int index ) {
            ThrowMutableException( "CopyAndRemoveAt" );
        }

        /// <summary>
        ///     Finds the index of the specified element.
        /// </summary>
        /// <param name="item">An item to search for.</param>
        /// <returns>The index of the item, or -1 if it was not found.</returns>
        public int IndexOf( T item ) {
            return Array.IndexOf( this._mArray, item );
        }

        /// <summary>
        ///     Copies the list and adds a new value at the end.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList< T > CopyAndAdd( T value ) {
            var newArray = new T[this._mArray.Length + 1];
            this._mArray.CopyTo( newArray, 0 );
            newArray[ this._mArray.Length ] = value;
            return new ImmutableList< T >( newArray );
        }

        /// <summary>
        ///     Returns a new, cleared (empty) immutable list.
        /// </summary>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList< T > CopyAndClear() {
            return new ImmutableList< T >( new T[0] );
        }

        /// <summary>
        ///     Copies the list adn inserts a particular element.
        /// </summary>
        /// <param name="index">The index at which to insert an element.</param>
        /// <param name="item">The element to insert.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList< T > CopyAndInsert( int index, T item ) {
            var newArray = new T[this._mArray.Length + 1];
            Array.Copy( this._mArray, newArray, index );
            newArray[ index ] = item;
            Array.Copy( this._mArray, index, newArray, index + 1, this._mArray.Length - index );
            return new ImmutableList< T >( newArray );
        }

        /// <summary>
        ///     Copies the list and removes a particular element.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList< T > CopyAndRemove( T item ) {
            var index = this.IndexOf( item );
            if ( index == -1 ) {
                throw new ArgumentException( "Item not found in list." );
            }

            return this.CopyAndRemoveAt( index );
        }

        /// <summary>
        ///     Copies the list and removes a particular element.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList< T > CopyAndRemoveAt( int index ) {
            var newArray = new T[this._mArray.Length - 1];
            Array.Copy( this._mArray, newArray, index );
            Array.Copy( this._mArray, index + 1, newArray, index, this._mArray.Length - index - 1 );
            return new ImmutableList< T >( newArray );
        }

        /// <summary>
        ///     Copies the list and modifies the specific value at the index provided.
        /// </summary>
        /// <param name="index">The index whose value is to be changed.</param>
        /// <param name="item">The value to store at the specified index.</param>
        /// <returns>A modified copy of this list.</returns>
        public ImmutableList< T > CopyAndSet( int index, T item ) {
            var newArray = new T[this._mArray.Length];
            this._mArray.CopyTo( newArray, 0 );
            newArray[ index ] = item;
            return new ImmutableList< T >( newArray );
        }

        /// <summary>
        ///     A helper method used below when a mutable method is accessed.  Several
        ///     operations on the collections interfaces IList&lt;T&gt; and
        ///     ICollection&lt;T&gt; are mutable, so we cannot support them.  We offer
        ///     immutable versions of each.
        /// </summary>
        private static void ThrowMutableException( String copyMethod ) {
            throw new InvalidOperationException( String.Format( "Cannot mutate an immutable list; see copying method ‘{0}’", copyMethod ) );
        }
    }
}
