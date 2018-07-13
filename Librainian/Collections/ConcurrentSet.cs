// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentSet.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ConcurrentSet.cs" was last formatted by Protiguous on 2018/07/10 at 8:50 PM.

namespace Librainian.Collections {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>http://stackoverflow.com/questions/4306936/how-to-implement-concurrenthashset-in-net</remarks>
	[JsonObject]
	public class ConcurrentSet<T> : ISet<T> {

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
		public Boolean IsReadOnly => false;

		/// <summary>
		///     Gets the number of elements in the set.
		/// </summary>
		public Int32 Count => this.Dictionary.Count;

		/// <summary>
		///     Adds an element to the current set and returns a value to indicate if the element was successfully added.
		/// </summary>
		/// <returns>true if the element is added to the set; false if the element is already in the set.</returns>
		/// <param name="item">The element to add to the set.</param>
		public Boolean Add( T item ) => this.TryAdd( item: item );

		public void Clear() => this.Dictionary.Clear();

		// public T this[ int index ] {
		//     get { return this._dictionary.ElementAt( index ).Key; }
		//     set {
		//         var key = this._dictionary.ElementAt( index ).Key;
		//         T result;
		//         this._dictionary.TryGetValue( key, out result );
		//         return true;
		//     }
		//}
		public Boolean Contains( T item ) => item != null && this.Dictionary.ContainsKey( item );

		/// <summary>
		///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
		///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		/// </summary>
		/// <param name="array">
		///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from
		///     <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
		///     zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///     <paramref name="array" /> is multidimensional.-or-The number of elements in the source
		///     <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from
		///     <paramref
		///         name="arrayIndex" />
		///     to the end of the destination <paramref name="array" />.-or-Type T cannot be cast automatically to the type of the
		///     destination <paramref name="array" />.
		/// </exception>
		public void CopyTo( T[] array, Int32 arrayIndex ) => this.Dictionary.Keys.CopyTo( array: array, arrayIndex: arrayIndex );

		/// <summary>
		///     Removes all elements in the specified collection from the current set.
		/// </summary>
		/// <param name="other">The collection of items to remove from the set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public void ExceptWith( IEnumerable<T> other ) {
			foreach ( var item in other ) { this.TryRemove( item: item ); }
		}

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
		///     collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator() => this.Dictionary.Keys.GetEnumerator();

		/// <summary>
		///     Modifies the current set so that it contains only elements that are also in a specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public void IntersectWith( IEnumerable<T> other ) {
			var list = other as IList<T> ?? other.ToArray();

			Parallel.ForEach( this.AsParallel().Where( item => !list.Contains( item: item ) ), item => this.TryRemove( item: item ) );
		}

		/// <summary>
		///     Determines whether the current set is a property (strict) subset of a specified collection.
		/// </summary>
		/// <returns>true if the current set is a correct subset of <paramref name="other" />; otherwise, false.</returns>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public Boolean IsProperSubsetOf( IEnumerable<T> other ) {
			var list = other as IList<T> ?? other.ToArray();

			return this.Count != list.Count && this.IsSubsetOf( other: list );
		}

		/// <summary>
		///     Determines whether the current set is a correct superset of a specified collection.
		/// </summary>
		/// <returns>
		///     true if the <see cref="T:System.Collections.Generic.ISet`1" /> object is a correct superset of
		///     <paramref name="other" />; otherwise, false.
		/// </returns>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public Boolean IsProperSupersetOf( IEnumerable<T> other ) {
			var list = other as IList<T> ?? other.ToArray();

			return this.Count != list.Count && this.IsSupersetOf( other: list );
		}

		/// <summary>
		///     Determines whether a set is a subset of a specified collection.
		/// </summary>
		/// <returns>true if the current set is a subset of <paramref name="other" />; otherwise, false.</returns>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public Boolean IsSubsetOf( IEnumerable<T> other ) {
			var list = other as IList<T> ?? other.ToArray();

			return this.AsParallel().All( list.Contains );
		}

		/// <summary>
		///     Determines whether the current set is a superset of a specified collection.
		/// </summary>
		/// <returns>true if the current set is a superset of <paramref name="other" />; otherwise, false.</returns>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public Boolean IsSupersetOf( IEnumerable<T> other ) => other.AsParallel().All( this.Contains );

		/// <summary>
		///     Determines whether the current set overlaps with the specified collection.
		/// </summary>
		/// <returns>true if the current set and <paramref name="other" /> share at least one common element; otherwise, false.</returns>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public Boolean Overlaps( IEnumerable<T> other ) => other.AsParallel().Any( this.Contains );

		/// <summary>
		///     Removes the first occurrence of a specific object from the
		///     <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <returns>
		///     true if <paramref name="item" /> was successfully removed from the
		///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
		///     <paramref name="item" /> is not
		///     found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		/// <exception cref="T:System.NotSupportedException">
		///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
		///     read-only.
		/// </exception>
		public Boolean Remove( T item ) => item != null && this.TryRemove( item: item );

		/// <summary>
		///     Determines whether the current set and the specified collection contain the same elements.
		/// </summary>
		/// <returns>true if the current set is equal to <paramref name="other" />; otherwise, false.</returns>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public Boolean SetEquals( IEnumerable<T> other ) {
			var list = other as IList<T> ?? other.ToArray();

			return this.Count == list.Count && list.AsParallel().All( this.Contains );
		}

		/// <summary>
		///     Modifies the current set so that it contains only elements that are present either in the current set or in the
		///     specified collection, but not both.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public void SymmetricExceptWith( IEnumerable<T> other ) => throw new NotImplementedException();

		/// <summary>
		///     Modifies the current set so that it contains all elements that are present in both the current set and in the
		///     specified collection.
		/// </summary>
		/// <param name="other">The collection to compare to the current set.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
		public void UnionWith( IEnumerable<T> other ) {
			foreach ( var item in other ) { this.TryAdd( item: item ); }
		}

		/// <summary>
		///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		/// <exception cref="T:System.NotSupportedException">
		///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
		///     read-only.
		/// </exception>
		/// <exception cref="ArgumentException"></exception>
		void ICollection<T>.Add( T item ) {
			if ( item != null && !this.Add( item: item ) ) { throw new ArgumentException( "Item already exists in set." ); }
		}

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		/// </summary>
		[JsonProperty]
		private ConcurrentDictionary<T, Object> Dictionary { get; } = new ConcurrentDictionary<T, Object>( concurrencyLevel: Environment.ProcessorCount, capacity: 7 );

		/// <summary>
		///     Gets a value that indicates if the set is empty.
		/// </summary>
		public Boolean IsEmpty => this.Dictionary.IsEmpty;

		public ConcurrentSet() { }

		public ConcurrentSet( [NotNull] params T[] items ) => this.UnionWith( other: items );

		public ConcurrentSet( [NotNull] IEnumerable<T> items ) => this.UnionWith( other: items );

		/// <summary>
		///     Returns a copy of the items to an array.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public T[] ToArray() => this.Dictionary.Keys.ToArray();

		public Boolean TryAdd( [NotNull] T item ) => this.Dictionary.TryAdd( item, null );

		public Boolean TryGet( [NotNull] T item ) => this.Dictionary.TryGetValue( item, out _ );

		public Boolean TryRemove( [NotNull] T item ) => this.Dictionary.TryRemove( item, out _ );

		public Boolean TryTakeAny( out T item ) {
			foreach ( var pair in this.Dictionary ) {
				item = pair.Key;

				return true;
			}

			item = default;

			return false;
		}
	}
}