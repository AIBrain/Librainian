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
// "Librainian/AbstractCollection.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Serve as based class to be inherited by the classes that needs to implement both the <see cref="ICollection"/> and the <see cref="ICollection{T}"/> interfaces.
    /// </summary>
    /// <remarks>
    /// <para>By inheriting from this abstract class, subclass is only required to implement the <see cref="GetEnumerator()"/> to complete a concrete read only collection class.</para>
    /// <para><see cref="AbstractCollection{T}"/> throws <see cref="NotSupportedException"/> for all access to the collection mutating members.</para>
    /// </remarks>
    /// <typeparam name="T">Element type of the collection</typeparam>
    /// <author>Kenneth Xu</author>
    [JsonObject]
    public abstract class AbstractCollection<T> : ICollection<T>, ICollection //NET_ONLY
    {

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only. This implementation always return true;
        /// </summary>
        /// <returns>true if the <see cref="ICollection{T}"/> is read-only; otherwise, false. This implementation always return true;</returns>
        public virtual Boolean IsReadOnly => true;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}"/>. This implementation counts the elements by iterating through the enumerator returned by <see cref="GetEnumerator()"/> method.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="ICollection{T}"/>.</returns>
        // ReSharper disable once UseCollectionCountProperty
        public virtual Int32 Count => this.Count();

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <remarks>This implementation always return <see langword="false"/>.</remarks>
        /// <returns>true if access to the <see cref="ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        /// <filterpriority>2</filterpriority>
        protected virtual Boolean IsSynchronized => false;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        /// <filterpriority>2</filterpriority>
        Boolean ICollection.IsSynchronized => this.IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/> .
        /// </summary>
        /// <remarks>This implementation returns <see langword="null"/>.</remarks>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/> .</returns>
        /// <filterpriority>2</filterpriority>
        protected virtual Object SyncRoot => null;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/> .
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/> .</returns>
        /// <filterpriority>2</filterpriority>
        Object ICollection.SyncRoot => this.SyncRoot;

        /// <summary>
        /// Ensures the returned array has capacity specified by <paramref name="length"/>.
        /// </summary>
        /// <remarks>If <typeparamref name="T"/> is <see cref="Object"/> but array is actually <c>String[]</c>, the returned array is always <c>String[]</c>.</remarks>
        /// <param name="array"> The source array.</param>
        /// <param name="length">Expected length of array.</param>
        /// <returns><paramref name="array"/> itself if <c>array.Length &gt;= length</c>. Otherwise a new array of same type of <paramref name="array"/> of given <paramref name="length"/>.</returns>
        protected static T[] EnsureCapacity( T[] array, Int32 length ) {
            if ( array is null ) {
                return new T[length];
            }

            if ( array.Length >= length ) {
                return array;
            }

            // new T[size] won't work here when targetArray is subtype of T.
            return ( T[] )Array.CreateInstance( elementType: array.GetType().GetElementType() ?? throw new InvalidOperationException(), length );
        }

        /// <summary>
        /// Called by <see cref="AddRange"/> after the parameter is validated to be neither <c>null</c> nor this collection itself.
        /// </summary>
        /// <param name="collection">Collection of items to be added.</param>
        /// <returns><c>true</c> if this collection is modified, else <c>false</c>.</returns>
        protected virtual Boolean DoAddRange( IEnumerable<T> collection ) {
            var modified = false;

            foreach ( var element in collection ) {
                this.Add( item: element );
                modified = true;
            }

            return modified;
        }

        /// <summary>
        /// Does the actual work of copying to array. Subclass is recommended to override this method instead of <see cref="CopyTo(T[], Int32)"/> method, which does all necessary parameter checking and raises proper
        /// exception before calling this method.
        /// </summary>
        /// <param name="array">         The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">    The zero-based index in array at which copying begins.</param>
        /// <param name="ensureCapacity">If is <c>true</c>, calls <see cref="EnsureCapacity"/></param>
        /// <returns>
        /// A new array of same runtime type as <paramref name="array"/> if <paramref name="array"/> is too small to hold all elements and <paramref name="ensureCapacity"/> is <c>false</c>. Otherwise the <paramref
        /// name="array"/> instance itself.
        /// </returns>
        protected virtual T[] DoCopyTo( T[] array, Int32 arrayIndex, Boolean ensureCapacity ) {
            if ( ensureCapacity ) {
                array = EnsureCapacity( array: array, this.Count );
            }

            foreach ( var e in this ) {
                array[arrayIndex++] = e;
            }

            return array;
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>. This implementation always throw <see cref="NotSupportedException"/> .
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        /// <exception cref="NotSupportedException">The <see cref="ICollection{T}"/> is read-only. This implementation always throw this exception.</exception>
        public virtual void Add( T item ) => throw new NotSupportedException();

        /// <summary>
        /// Adds all of the elements in the supplied <paramref name="collection"/> to this collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Attempts to <see cref="AddRange"/> of a collection to itself result in <see cref="ArgumentException"/> . Further, the behavior of this operation is undefined if the specified collection is modified while the
        /// operation is in progress.
        /// </para>
        /// <para>
        /// This implementation iterates over the specified collection, and adds each element returned by the iterator to this collection, in turn. An exception encountered while trying to add an element may result in
        /// only some of the elements having been successfully added when the associated exception is thrown.
        /// </para>
        /// </remarks>
        /// <param name="collection">The collection containing the elements to be added to this collection.</param>
        /// <returns><c>true</c> if this collection is modified, else <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">If the supplied <paramref name="collection"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">If the collection is the current collection.</exception>
        public virtual Boolean AddRange( IEnumerable<T> collection ) {
            if ( collection is null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            if ( ReferenceEquals( collection, this ) ) {
                throw new ArgumentException( message: "Cannot add to itself.", nameof( collection ) );
            }

            return this.DoAddRange( collection: collection );
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>. This implementation always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The <see cref="ICollection{T}"/> is read-only. This implementation always throw exception.</exception>
        public virtual void Clear() => throw new NotSupportedException();

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value. This implementation searches the element by iterating through the enumerator returned by <see cref="GetEnumerator()"/> method.
        /// </summary>
        /// <returns>true if item is found in the <see cref="ICollection{T}"/>; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        public virtual Boolean Contains( T item ) => this.Contains( value: item );

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <remarks>This method is intentionally sealed. Subclass should override <see cref="DoCopyTo(T[], Int32, Boolean)"/> instead.</remarks>
        /// <param name="array">     The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional. <br/>-or- <br/> arrayIndex is equal to or greater than the length of array. <br/>-or- <br/> The number of elements in the source <see cref="ICollection{T}"/> is greater than the
        /// available space from arrayIndex to the end of the destination array. <br/>-or- <br/> Type T cannot be cast automatically to the type of the destination array.
        /// </exception>
        public void CopyTo( T[] array, Int32 arrayIndex ) {
            if ( array is null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            if ( arrayIndex < array.GetLowerBound( dimension: 0 ) ) {
                throw new ArgumentOutOfRangeException( nameof( arrayIndex ), actualValue: arrayIndex, message: "arrayIndex must not be less then the lower bound of the array." );
            }

            try {
                this.DoCopyTo( array: array, arrayIndex: arrayIndex, ensureCapacity: false );
            }
            catch ( IndexOutOfRangeException e ) {
                throw new ArgumentException( message: "array is too small to fit the collection.", nameof( array ), innerException: e );
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>Subclass must implement this method.</remarks>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public abstract IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>. This implementation always throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <returns>
        /// true if item was successfully removed from the <see cref="ICollection{T}"/>; otherwise, false. This method also returns false if item is not found in the original <see cref="ICollection{T}"/> .
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <exception cref="NotSupportedException">When the <see cref="ICollection{T}"/> is read-only. This implementation always throw this exception.</exception>
        public virtual Boolean Remove( T item ) => throw new NotSupportedException();

        /// <summary>
        /// Returns an array containing all of the elements in this collection, in proper sequence; the runtime type of the returned array is that of the specified array. If the collection fits in the specified array, it
        /// is returned therein. Otherwise, a new array is allocated with the runtime type of the specified array and the size of this collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Like the <see cref="ToArray()"/> method, this method acts as bridge between array-based and collection-based APIs. Further, this method allows precise control over the runtime type of the output array, and
        /// may, under certain circumstances, be used to save allocation costs.
        /// </para>
        /// <para>
        /// Suppose <i>x</i> is a collection known to contain only strings. The following code can be used to dump the collection into a newly allocated array of <see cref="String"/> s:
        /// <code language="c#">
        ///  String[] y = (String[])
        /// x.ToArray(new String[0]);
        /// </code>
        /// </para>
        /// <para>Note that <i>ToArray(new T[0])</i> is identical in function to <see cref="AbstractCollection{T}.ToArray()"/> .</para>
        /// </remarks>
        /// <param name="targetArray">The array into which the elements of the collection are to be stored, if it is big enough; otherwise, a new array of the same runtime type is allocated for this purpose.</param>
        /// <returns>An array containing all of the elements in this collection.</returns>
        /// <exception cref="ArgumentNullException">If the supplied <paramref name="targetArray"/> is <c>null</c>.</exception>
        /// <exception cref="ArrayTypeMismatchException">If type of <paramref name="targetArray"/> is a derived type of <typeparamref name="T"/> and the collection contains element that is not that derived type.</exception>
        public virtual T[] ToArray( T[] targetArray ) {
            if ( targetArray is null ) {
                throw new ArgumentNullException( nameof( targetArray ) );
            }

            return this.DoCopyTo( array: targetArray, arrayIndex: 0, ensureCapacity: true );
        }

        /// <summary>
        /// Returns an array containing all of the elements in this collection, in proper sequence.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned array will be "safe" in that no references to it are maintained by this collection. (In other words, this method must allocate a new array). The caller is thus free to modify the returned array.
        /// </para>
        /// <para>This method acts as bridge between array-based and collection-based APIs.</para>
        /// </remarks>
        /// <returns>An array containing all of the elements in this collection.</returns>
        public virtual T[] ToArray() => this.DoCopyTo( array: null, arrayIndex: 0, ensureCapacity: true );

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <remarks>This implementation list out all the elements separated by comma.</remarks>
        /// <returns>A <see cref="String"/> that represents the current <see cref="Object"/>.</returns>
        /// <filterpriority>2</filterpriority>
        public override String ToString() {
            var sb = new StringBuilder();
            sb.Append( value: GetType().Name ).Append( value: "(" );
            var first = true;

            foreach ( var e in this ) {
                if ( !first ) {
                    sb.Append( value: ", " );
                }

                sb.Append( value: e );
                first = false;
            }

            return sb.Append( value: ")" ).ToString();
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional.-or- index is equal to or greater than the length of array.
        /// - or- The number of elements in the source <see cref="ICollection"/> is greater than the available space from index to the end of the destination array.
        /// </exception>
        /// <exception cref="InvalidCastException">The type of the source <see cref="ICollection"/> cannot be cast automatically to the type of the destination array.</exception>
        /// <filterpriority>2</filterpriority>
        protected virtual void CopyTo( Array array, Int32 index ) {
            foreach ( var e in this ) {
                array.SetValue( value: e, index: index++ );
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/> , starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">array is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space
        /// from index to the end of the destination array.
        /// </exception>
        /// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination array.</exception>
        /// <filterpriority>2</filterpriority>
        void ICollection.CopyTo( Array array, Int32 index ) {
            if ( array is null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            if ( index < array.GetLowerBound( dimension: 0 ) ) {
                throw new ArgumentOutOfRangeException( nameof( index ), actualValue: index, message: "index must not be less then lower bound of the array" );
            }

            try {
                this.CopyTo( array: array, index: index );
            }
            catch ( RankException re ) {
                throw new ArgumentException( message: "array must not be multi-dimensional.", nameof( array ), innerException: re );
            }
            catch ( IndexOutOfRangeException e ) {
                throw new ArgumentException( message: "array is too small to fit the collection.", nameof( array ), innerException: e );
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}