// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CollectionExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "CollectionExtensions.cs" was last formatted by Protiguous on 2018/07/10 at 8:49 PM.

namespace Librainian.Collections {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.Diagnostics;
	using System.Linq;
	using System.Numerics;
	using System.Reflection;
	using System.Threading.Tasks;
	using Exceptions;
	using Extensions;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Maths;
	using Threading;

	public static class CollectionExtensions {

		public static void Add<T>( [NotNull] this IProducerConsumerCollection<T> collection, T item ) {
			if ( null == collection ) { throw new ArgumentNullException( nameof( collection ) ); }

			collection.TryAdd( item: item );
		}

		public static void AddRange<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] IEnumerable<T> items ) {
			if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

			if ( items is null ) { throw new ArgumentNullException( nameof( items ) ); }

			Parallel.ForEach( source: items.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive, body: collection.Add );
		}

		/// <summary>
		///     Determines whether or not the given sequence contains any duplicate elements.
		/// </summary>
		/// <param name="self">The extended <see cref="IEnumerable{T}" />.</param>
		/// <returns>True if the sequence contains duplicate elements, false if not.</returns>
		public static Boolean AnyDuplicates<T>( [NotNull] this IEnumerable<T> self ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "AnyDuplicates<T> called on a null IEnumerable<T>." ); }

			return AnyRelationship( self: self, relationshipFunc: ( arg1, arg2 ) => arg1.Equals( arg2 ) );
		}

		/// <summary>
		///     Determines whether or not a given relationship exists between any two elements in the sequence.
		/// </summary>
		/// <param name="self">            The extended <see cref="IEnumerable{T}" />.</param>
		/// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>True if the relationship exists between any two elements, false if not.</returns>
		public static Boolean AnyRelationship<T>( [NotNull] this IEnumerable<T> self, Func<T, T, Boolean> relationshipFunc ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "AnyRelationship called on a null IEnumerable<T>." ); }

			var enumerable = self as T[] ?? self.ToArray();

			return enumerable.Select( selector: ( a, aIndex ) =>
				enumerable.Skip( count: aIndex + 1 ).Any( b => ( relationshipFunc?.Invoke( arg1: a, arg2: b ) ?? default ) || ( relationshipFunc?.Invoke( arg1: b, arg2: a ) ?? default ) ) ).Any( value => value );
		}

		/// <summary>
		///     Returns whether or not there are at least <paramref name="minInstances" /> elements in the source sequence that
		///     satisfy the given <paramref name="predicate" />.
		/// </summary>
		/// <param name="self">        The extended IEnumerable{T}.</param>
		/// <param name="minInstances">The number of elements that must satisfy the <paramref name="predicate" />.</param>
		/// <param name="predicate">   The function that determines whether or not an element is counted.</param>
		/// <returns>
		///     This method will immediately return true upon finding the <paramref name="minInstances" /> th element that
		///     satisfies the predicate, or if <paramref name="minInstances" /> is 0. Otherwise, if
		///     <paramref
		///         name="minInstances" />
		///     is greater than the size of the source sequence, or less than <paramref name="minInstances" /> elements are found
		///     to match the <paramref name="predicate" />, it will return false.
		/// </returns>
		public static Boolean AtLeast<T>( [NotNull] this IEnumerable<T> self, UInt64 minInstances, [NotNull] Func<T, Boolean> predicate ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "AtLeast called on a null IEnumerable<>." ); }

			if ( predicate is null ) { throw new ArgumentNullException( nameof( predicate ) ); }

			if ( minInstances == 0 ) { return true; }

			UInt64 numInstSoFar = 0;

			return self.Any( element => predicate( arg: element ) && ++numInstSoFar == minInstances );
		}

		/// <summary>
		///     Ascertains whether there are no more than <paramref name="maxInstances" /> elements in the source sequence that
		///     satisfy the given <paramref name="predicate" />.
		/// </summary>
		/// <param name="self">        The extended IEnumerable{T}.</param>
		/// <param name="maxInstances">The maximum number of elements that can satisfy the <paramref name="predicate" />.</param>
		/// <param name="predicate">   The function that determines whether or not an element is counted.</param>
		/// <returns>
		///     This method will immediately return false upon finding the ( <paramref name="maxInstances" /> + 1)th element that
		///     satisfies the predicate. Otherwise, if <paramref name="maxInstances" /> is greater than the size
		///     of the source sequence, or less than <paramref name="maxInstances" /> elements are found to match the
		///     <paramref name="predicate" />, it will return true.
		/// </returns>
		public static Boolean AtMost<T>( [NotNull] this IEnumerable<T> self, UInt64 maxInstances, [NotNull] Func<T, Boolean> predicate ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "AtMost called on a null IEnumerable<>." ); }

			if ( predicate is null ) { throw new ArgumentNullException( nameof( predicate ) ); }

			UInt64 numInstSoFar = 0;

			return self.All( element => !predicate( arg: element ) || ++numInstSoFar <= maxInstances );
		}

		public static Int32 Clear<T>( [NotNull] this IProducerConsumerCollection<T> collection ) => collection.RemoveAll();

		public static void Clear<T>( [NotNull] this ConcurrentBag<T> bag ) {
			if ( bag is null ) { throw new ArgumentNullException( nameof( bag ) ); }

			while ( !bag.IsEmpty ) { bag.TryTake( result: out _ ); }
		}

		[CanBeNull]
		public static Byte[] Clone( [CanBeNull] this Byte[] bytes ) {
			if ( bytes is null ) { return null; }

			var copy = new Byte[ bytes.Length ];
			Buffer.BlockCopy( bytes, 0, copy, 0, bytes.Length );

			return copy;
		}

		[NotNull]
		public static Byte[] ClonePortion( [NotNull] this Byte[] bytes, Int32 offset, Int32 length ) {
			if ( bytes is null ) { throw new ArgumentNullException( nameof( bytes ) ); }

			if ( offset < 0 ) { throw new ArgumentOutOfRangeException(); }

			if ( length < 0 ) { throw new ArgumentOutOfRangeException(); }

			var copy = new Byte[ length ];
			Buffer.BlockCopy( bytes, offset, copy, 0, length );

			return copy;
		}

		/// <summary>
		///     Concat multiple byte arrays into one new array.
		///     Warning: limited to Int32 byte arrays (2GB).
		/// </summary>
		/// <param name="arrays"></param>
		/// <returns></returns>
		[NotNull]
		public static Byte[] Concat( [NotNull] params Byte[][] arrays ) {
			Int64 totalLength = arrays.Where( data => data != null ).Sum( Buffer.ByteLength );

			if ( totalLength > Int32.MaxValue ) { throw new OutOfRangeException( $"The total size of the arrays ({totalLength:N0}) is too large." ); }

			var ret = new Byte[ totalLength ];
			var offset = 0;

			foreach ( var data in arrays.Where( data => data != null ) ) {
				var length = Buffer.ByteLength( data );
				Buffer.BlockCopy( data, 0, ret, offset, length );
				offset += length;
			}

			return ret;
		}

		/// <summary>
		///     Checks if two IEnumerables contain the exact same elements and same number of elements. Order does not matter.
		/// </summary>
		/// <typeparam name="T">The Type of object.</typeparam>
		/// <param name="a">The first collection.</param>
		/// <param name="b">The second collection.</param>
		/// <returns>True if both IEnumerables contain the same items, and same number of items; otherwise, false.</returns>
		public static Boolean ContainSameElements<T>( this IList<T> a, IList<T> b ) {
			var aa = a; //.ToList();

			//aa.Fix();

			var bb = b; //.ToList();

			//bb.Fix();

			if ( aa.Count != bb.Count ) { return false; }

			if ( aa.Any( item => !bb.Remove( item: item ) ) ) { return false; }

			Debug.Assert( condition: !bb.Any() );

			return !bb.Any(); //is this correct? I need to sleep..
		}

		public static BigInteger CountBig<TType>( [NotNull] this IEnumerable<TType> items ) {
			if ( items is null ) { throw new ArgumentNullException( nameof( items ) ); }

			return items.Aggregate( seed: BigInteger.Zero, func: ( current, item ) => current + BigInteger.One );
		}

		/// <summary>
		///     Counts the number of times each element appears in a collection, and returns a
		///     <see cref="IDictionary{T, V}">dictionary</see>; where each key is an element and its value is the number of times
		///     that element
		///     appeared in the source collection.
		/// </summary>
		/// <param name="self">The extended IEnumerable{T}.</param>
		/// <returns>A dictionary of elements mapped to the number of times they appeared in <paramref name="self" />.</returns>
		[NotNull]
		public static IDictionary<T, Int32> CountInstances<T>( [NotNull] this IEnumerable<T> self ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "CountInstances called on a null IEnumerable<T>." ); }

			IDictionary<T, Int32> result = new Dictionary<T, Int32>();

			foreach ( var element in self.Where( t => t != null ) ) {
				if ( result.ContainsKey( element ) ) { ++result[ element ]; }
				else { result[ element ] = 1; }
			}

			return result;
		}

		/// <summary>
		///     Counts how many pairs of elements in the source sequence share the relationship defined by
		///     <paramref name="relationshipFunc" />.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>The number of pairs found.</returns>
		public static Int32 CountRelationship<T>( [NotNull] this IEnumerable<T> self, Func<T, T, Boolean> relationshipFunc ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "CountRelationship called on a null IEnumerable<T>." ); }

			var enumerable = self as T[] ?? self.ToArray();

			return enumerable.Select( selector: ( a, aIndex ) =>
				enumerable.Skip( count: aIndex + 1 ).Any( b => ( relationshipFunc?.Invoke( arg1: a, arg2: b ) ?? default ) || ( relationshipFunc?.Invoke( arg1: b, arg2: a ) ?? default ) ) ).Count( value => value );
		}

		/// <summary>
		///     Returns duplicate items found in the <see cref="sequence" /> .
		/// </summary>
		/// <param name="sequence">todo: describe sequence parameter on Duplicates</param>
		[NotNull]
		public static HashSet<T> Duplicates<T>( [NotNull] this IEnumerable<T> sequence ) {
			if ( null == sequence ) { throw new ArgumentNullException( nameof( sequence ) ); }

			var set = new HashSet<T>();

			return new HashSet<T>( collection: sequence.Where( item => !set.Add( item: item ) ) );
		}

		public static IEnumerable<T> Empty<T>() { yield break; }

		[NotNull]
		public static IEnumerable<T> EnumerableFromArray<T>( [NotNull] IEnumerable<T> array ) {
			if ( array is null ) { throw new ArgumentNullException( nameof( array ) ); }

			return array;
		}

		public static Boolean EqualLists<T>( [NotNull] this List<T> left, [NotNull] List<T> right ) {
			if ( left is null ) { throw new ArgumentNullException( nameof( left ) ); }

			if ( right is null ) { throw new ArgumentNullException( nameof( right ) ); }

			if ( left.Count != right.Count ) { return false; }

			var dict = new Dictionary<T, Int64>();

			foreach ( var member in left ) {
				if ( !dict.ContainsKey( member ) ) { dict[ member ] = 1; }
				else { dict[ member ]++; }
			}

			foreach ( var member in right ) {
				if ( !dict.ContainsKey( member ) ) { return false; }

				dict[ member ]--;
			}

			foreach ( var kvp in dict ) {
				if ( kvp.Value == 0 ) { continue; }

				return false;
			}

			return true;
		}

		/// <summary>
		///     Returns the first two items to in the source collection that satisfy the given <paramref name="relationshipFunc" />
		///     , or <c>null</c> if no match was found.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>
		///     A tuple of the first two elements that match the given relationship, or <c>null</c> if no such relationship
		///     exists.
		/// </returns>
		public static KeyValuePair<T, T>? FirstRelationship<T>( [NotNull] this IEnumerable<T> self, [NotNull] Func<T, T, Boolean> relationshipFunc ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "FirstRelationship called on a null IEnumerable<T>." ); }

			if ( relationshipFunc is null ) { throw new ArgumentNullException( nameof( relationshipFunc ) ); }

			var aIndex = 0;

			foreach ( var a in self ) {

				foreach ( var b in self.Skip( count: ++aIndex ).Where( b => relationshipFunc( arg1: a, arg2: b ) || relationshipFunc( arg1: b, arg2: a ) ) ) { return new KeyValuePair<T, T>( a, b ); }
			}

			return null;
		}

		/// <summary>
		///     The <seealso cref="List{T}.Capacity" /> is resized down to the <seealso cref="List{T}.Count" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		public static void Fix<T>( [NotNull] this List<T> collection ) {
			if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

			collection.Capacity = collection.Count;
		}

		public static void ForEach<T>( [NotNull] this IEnumerable<T> items, [NotNull] Action<T> action ) {
			if ( null == items ) { throw new ArgumentNullException( nameof( items ) ); }

			if ( null == action ) { throw new ArgumentNullException( nameof( action ) ); }

			foreach ( var item in items ) {

				//action.Invoke( item );	//is either way better?
				action( item );
			}
		}

		/// <summary>
		///     http://blogs.msdn.com/b/pfxteam/archive/2012/02/04/10264111.aspx
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict">     </param>
		/// <param name="key">      </param>
		/// <param name="generator"></param>
		/// <param name="added">    </param>
		/// <returns></returns>
		public static TValue GetOrAdd<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dict, [NotNull] TKey key, [NotNull] Func<TKey, TValue> generator, out Boolean added ) {
			if ( generator is null ) { throw new ArgumentNullException( nameof( generator ) ); }

			while ( true ) {
				if ( dict.TryGetValue( key, out var value ) ) {
					added = false;

					return value;
				}

				value = generator( arg: key );

				if ( !dict.TryAdd( key, value ) ) { continue; }

				added = true;

				return value;
			}
		}

		public static Boolean Has<T>( [NotNull] this Enum type, T value ) where T : struct {
			if ( type is null ) { throw new ArgumentNullException( nameof( type ) ); }

			return ( ( Int32 ) ( ValueType ) type & ( Int32 ) ( ValueType ) value ) == ( Int32 ) ( ValueType ) value;
		}

		public static Boolean HasDuplicates<T>( [NotNull] this IEnumerable<T> sequence ) {
			if ( sequence is null ) { throw new ArgumentNullException( nameof( sequence ) ); }

			if ( Equals( sequence, null ) ) { throw new ArgumentNullException( nameof( sequence ) ); }

			return sequence.Duplicates().Any();
		}

		public static Boolean In<T>( this T value, [NotNull] params T[] items ) {
			if ( items is null ) { throw new ArgumentNullException( nameof( items ) ); }

			return items.Contains( value );
		}

		public static Int32 IndexOf<T>( [NotNull] this T[] self, T item ) => Array.IndexOf( array: self, item );

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">  </param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		/// <remarks>http://stackoverflow.com/a/3562370/956364</remarks>
		public static Int32 IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence ) {
			if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

			if ( sequence is null ) { throw new ArgumentNullException( nameof( sequence ) ); }

			return source.IndexOfSequence( sequence: sequence, EqualityComparer<T>.Default );
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">  </param>
		/// <param name="sequence"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		/// <remarks>http://stackoverflow.com/a/3562370/956364</remarks>
		public static Int32 IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence, [NotNull] IEqualityComparer<T> comparer ) {
			if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

			if ( sequence is null ) { throw new ArgumentNullException( nameof( sequence ) ); }

			if ( comparer is null ) { throw new ArgumentNullException( nameof( comparer ) ); }

			var seq = sequence.ToArray();

			var p = 0; // current position in source sequence

			var i = 0; // current position in searched sequence

			var prospects = new List<Int32>(); // list of prospective matches

			foreach ( var item in source ) {

				// Remove bad prospective matches
				var item1 = item;
				var p1 = p;
				prospects.RemoveAll( match: k => !comparer.Equals( x: item1, y: seq[ p1 - k ] ) );

				// Is it the start of a prospective match ?
				if ( comparer.Equals( x: item, y: seq[ 0 ] ) ) { prospects.Add( item: p ); }

				// Does current character continues partial match ?
				if ( comparer.Equals( x: item, y: seq[ i ] ) ) {
					i++;

					// Do we have a complete match ?
					if ( i == seq.Length ) {

						// Bingo !
						return p - seq.Length + 1;
					}
				}
				else // Mismatch
				{
					// Do we have prospective matches to fall back to ?
					if ( prospects.Count > 0 ) {

						// Yes, use the first one
						var k = prospects[ index: 0 ];
						i = p - k + 1;
					}
					else {

						// No, start from beginning of searched sequence
						i = 0;
					}
				}

				p++;
			}

			// No match
			return -1;
		}

		/// <summary>
		///     <para>An infinite list.</para>
		/// </summary>
		/// <param name="value">todo: describe value parameter on Infinitely</param>
		public static IEnumerable<Boolean> Infinitely( this Boolean value ) {
			do { yield return value; } while ( true );

			// ReSharper disable once IteratorNeverReturns
		}

		/// <summary>
		///     Checks if an IEnumerable is empty.
		/// </summary>
		/// <typeparam name="T">The type of objects to enumerate.</typeparam>
		/// <param name="source">The IEnumerable to check if empty.</param>
		/// <returns>True if the <paramref name="source" /> is null or empty; otherwise false.</returns>
		public static Boolean IsEmpty<T>( [CanBeNull] this IEnumerable<T> source ) => source?.Any() != true;

		public static UInt64 LongSum( [NotNull] this IEnumerable<Int32> collection ) => collection.Aggregate( seed: 0UL, func: ( current, u ) => current + ( UInt64 ) u );

		/// <summary>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="current"></param>
		/// <returns></returns>
		public static LinkedListNode<TType> NextOrFirst<TType>( [NotNull] this LinkedListNode<TType> current ) {
			if ( current is null ) { throw new ArgumentNullException( nameof( current ) ); }

			return current.Next ?? current.List.First;
		}

		[NotNull]
		public static IEnumerable<T> OrderBy<T>( [NotNull] this IEnumerable<T> list, [NotNull] IEnumerable<T> guide ) {
			var toBeSorted = new HashSet<T>( collection: list );

			return guide.Where( member => toBeSorted.Contains( item: member ) );
		}

		[ItemNotNull]
		public static IEnumerable<IEnumerable<T>> Partition<T>( [NotNull] this IEnumerable<T> source, Int32 size ) {
			if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

			T[] array = null;
			var count = 0;

			foreach ( var item in source ) {
				if ( array is null ) { array = new T[ size ]; }

				array[ count ] = item;
				count++;

				if ( count != size ) { continue; }

				yield return new ReadOnlyCollection<T>( list: array );
				array = null;
				count = 0;
			}

			if ( array is null ) { yield break; }

			Array.Resize( array: ref array, newSize: count );

			yield return new ReadOnlyCollection<T>( list: array );
		}

		/// <summary>
		///     untested
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="self"></param>
		/// <param name="key"> </param>
		/// <returns></returns>
		public static TValue Pop<TKey, TValue>( [NotNull] this IDictionary<TKey, TValue> self, [NotNull] TKey key ) {
			var result = self[ key ];
			self.Remove( key );

			return result;
		}

		/// <summary>
		///     untested
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		public static T PopFirst<T>( [NotNull] this ICollection<T> self ) {
			var result = self.First();
			self.Remove( item: result );

			return result;
		}

		/// <summary>
		///     untested
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		public static T PopLast<T>( [NotNull] this ICollection<T> self ) {
			var result = self.Last();
			self.Remove( item: result );

			return result;
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="current"></param>
		/// <returns></returns>
		public static LinkedListNode<TType> PreviousOrLast<TType>( [NotNull] this LinkedListNode<TType> current ) {
			if ( current is null ) { throw new ArgumentNullException( nameof( current ) ); }

			return current.Previous ?? current.List.Last;
		}

		public static IEnumerable<TU> Rank<T, TKey, TU>( [NotNull] this IEnumerable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, Int32, TU> selector ) {

			//if ( !source.Any() ) {
			//    yield break;
			//}

			if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

			if ( keySelector is null ) { throw new ArgumentNullException( nameof( keySelector ) ); }

			if ( selector is null ) { throw new ArgumentNullException( nameof( selector ) ); }

			var rank = 0;
			var itemCount = 0;
			var ordered = source.OrderBy( keySelector: keySelector ).ToArray();
			var previous = keySelector( arg: ordered[ 0 ] );

			foreach ( var t in ordered ) {
				itemCount += 1;
				var current = keySelector( arg: t );

				if ( !current.Equals( previous ) ) { rank = itemCount; }

				yield return selector( arg1: t, arg2: rank );
				previous = current;
			}
		}

		[CanBeNull]
		public static T Remove<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

			return collection.TryTake( item: out var result ) ? result : default;
		}

		public static T Remove<T>( [NotNull] this Enum type, T value ) where T : struct {
			if ( type is null ) { throw new ArgumentNullException( nameof( type ) ); }

			return ( T ) ( ValueType ) ( ( Int32 ) ( ValueType ) type & ~( Int32 ) ( ValueType ) value );
		}

		/// <summary>
		///     Removes the <paramref name="specificItem" /> from the <paramref name="collection" /> and returns how many
		///     <paramref name="specificItem" /> or null were removed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">  </param>
		/// <param name="specificItem"></param>
		/// <returns></returns>
		[CanBeNull]
		public static T Remove<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] T specificItem ) {
			if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

			if ( Equals( specificItem, null ) ) { throw new ArgumentNullException( nameof( specificItem ) ); }

			var sanity = collection.LongCount() * 2; //or LongCount() + 1 ?

			while ( sanity.Any() && collection.Contains( specificItem ) ) {
				--sanity;

				if ( collection.TryTake( item: out var temp ) ) {
					if ( Equals( temp, specificItem ) ) { return specificItem; }

					collection.TryAdd( item: temp );
				}
			}

			return default;
		}

		public static Int32 RemoveAll<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

			var removed = 0;

			while ( collection.Any() ) {
				while ( collection.TryTake( item: out _ ) ) { ++removed; }
			}

			return removed;
		}

		public static IEnumerable<T> RemoveEach<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

			while ( collection.TryTake( item: out var result ) ) { yield return result; }
		}

		/// <summary>
		///     <para>Shuffle an array[] in <paramref name="iterations" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">     </param>
		/// <param name="iterations"></param>
		/// <example>Deck.Shuffle( 7 );</example>
		[Obsolete( "broken at the moment. seealso Shuffle<List>" )]
		public static void Shuffle<T>( [NotNull] this T[] array, Int32 iterations = 1 ) {
			if ( array is null ) { throw new ArgumentNullException( nameof( array ) ); }

			if ( iterations < 1 ) { iterations = 1; }

			if ( array.Length < 1 ) {
				return; //nothing to shuffle
			}

			while ( iterations > 0 ) {
				iterations--;

				// make a copy of all items
				var bag = new ConcurrentBag<T>( collection: array );
				bag.Should().NotBeEmpty();
				var originalcount = bag.Count;

				var sqrt = ( Int32 ) Math.Sqrt( d: originalcount );

				if ( sqrt <= 1 ) { sqrt = 1; }

				// make some buckets.
				var buckets = new List<ConcurrentBag<T>>( capacity: sqrt );
				buckets.AddRange( collection: 1.To( end: sqrt ).Select( selector: i => new ConcurrentBag<T>() ) );

				// pull the items out of the bag, and put them into a random bucket each
				T item;

				while ( bag.TryTake( result: out item ) ) {
					var index = 0.Next( maxValue: sqrt );
					buckets[ index: index ].Add( item: item );
				}

				bag.Should().BeEmpty( because: "All items should have been taken out of the bag" );

				while ( bag.Count < originalcount ) {
					var index = 0.Next( maxValue: buckets.Count );
					var bucket = buckets[ index: index ];

					if ( bucket.TryTake( result: out item ) ) { bag.Add( item: item ); }

					if ( bucket.IsEmpty ) { buckets.Remove( item: bucket ); }
				}

				bag.Count.Should().Be( expected: originalcount );

				// put them back into the array
				var newArray = bag.ToArray();
				newArray.CopyTo( array: array, index: 0 );
			}

			// Old, !bad! way.
			//var items = array.Count();
			//for ( var i = 0; i < items; i++ ) {
			//    var index1 = randomFunc( 0, items ); //Randem.Next( 0, items );
			//    var index2 = randomFunc( 0, items ); //Randem.Next( 0, items );
			//    array.Swap( index1, index2 );
			//}
		}

		/// <summary>
		///     <para>Shuffle a list in <paramref name="iterations" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">            </param>
		/// <param name="iterations">      </param>
		/// <param name="shufflingType">   </param>
		/// <param name="forHowLong">      </param>
		/// <param name="orUntilCancelled"></param>
		/// <example>Deck.Shuffle( 7 );</example>
		public static void Shuffle<T>( [NotNull] this List<T> list, Int32 iterations = 1, ShufflingType shufflingType = ShufflingType.AutoChoice, TimeSpan? forHowLong = null,
			[CanBeNull] SimpleCancel orUntilCancelled = null ) {
			if ( list is null ) { throw new ArgumentNullException( nameof( list ) ); }

			try {
				if ( !list.Any() ) {
					return; //nothing to shuffle
				}

				if ( iterations < 1 ) { iterations = 1; }

				switch ( shufflingType ) {
					case ShufflingType.ByGuid: {
						ShuffleByGuid( list: ref list, iterations: iterations );
					}

						break;

					case ShufflingType.ByRandom: {
						ShuffleByRandomThenByRandom( list: ref list, iterations: iterations );
					}

						break;

					case ShufflingType.ByHarker: {
						ShuffleByHarker( list: ref list, iterations: iterations, forHowLong: forHowLong, orUntilCancelled: orUntilCancelled );
					}

						break;

					case ShufflingType.ByBags: {
						ShuffleByBags( list: ref list, iterations: iterations, originalcount: list.LongCount() );
					}

						break;

					case ShufflingType.AutoChoice: {
						ShuffleByHarker( list: ref list, iterations: iterations, forHowLong: forHowLong, orUntilCancelled: orUntilCancelled );
					}

						break;

					default: throw new ArgumentOutOfRangeException( nameof( shufflingType ) );
				}
			}
			catch ( IndexOutOfRangeException exception ) { exception.More(); }
		}

		/// <summary>
		///     Untested for speed and cpu/threading impact. Also, a lot of elements will/could NOT be shuffled much.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">         </param>
		/// <param name="iterations">   </param>
		/// <param name="originalcount"></param>
		public static void ShuffleByBags<T>( ref List<T> list, Int32 iterations, Int64 originalcount ) {
			var bag = new ConcurrentBag<T>();

			while ( iterations > 0 ) {
				iterations--;

				bag.AddRange( items: list.AsParallel() );
				bag.Should().NotBeEmpty( because: "made an unordered copy of all items" );

				list.Clear();
				list.Should().BeEmpty( because: "emptied the original list" );

				list.AddRange( collection: bag );
				list.LongCount().Should().Be( expected: originalcount );

				bag.RemoveAll();
			}
		}

		public static void ShuffleByGuid<T>( ref List<T> list, Int32 iterations = 1 ) {
			while ( iterations.Any() ) {
				iterations--;
				var copy = list.AsParallel().OrderBy( keySelector: arg => Guid.NewGuid() ).ToList();
				list.Clear();
				list.AddRange( collection: copy.AsParallel() );
			}
		}

		/// <summary>
		///     Fast shuffle. Not guaranteed or tested to be the fastest, but it *should* shuffle *well enough* in reasonable time.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">            </param>
		/// <param name="iterations">      </param>
		/// <param name="forHowLong">      </param>
		/// <param name="orUntilCancelled"></param>
		/// <remarks>The while.Any() could be replaced with a for loop.. the count is well known ahead of time.</remarks>
		public static void ShuffleByHarker<T>( [NotNull] ref List<T> list, Int32 iterations = 1, TimeSpan? forHowLong = null, [CanBeNull] SimpleCancel orUntilCancelled = null ) {
			if ( list == null ) { throw new ArgumentNullException( nameof( list ) ); }

			var whenStarted = Stopwatch.StartNew();
			var itemCount = list.Count;
			var array = list.ToArray();

			if ( orUntilCancelled != null ) {
				do {
					var a = 0.Next( maxValue: itemCount );
					var b = 0.Next( maxValue: itemCount );
					array.Swap( index1: a, b );
				} while ( !orUntilCancelled.HaveAnyCancellationsBeenRequested() );

				goto AllDone;
			}

			if ( forHowLong.HasValue ) {
				do {
					var a = 0.Next( maxValue: itemCount );
					var b = 0.Next( maxValue: itemCount );
					array.Swap( a, b );
				} while ( whenStarted.Elapsed < forHowLong.Value );

				goto AllDone;
			}

			if ( iterations < 1 ) { iterations = 1; }

			if ( iterations.Any() ) {
				iterations *= itemCount;
				iterations *= iterations;

				do {
					iterations--;
					var a = 0.Next( maxValue: itemCount );
					var b = 0.Next( maxValue: itemCount );
					array.Swap( a, b );
				} while ( iterations.Any() );
			}

			AllDone:
			list = array.ToList();

			list.Count.Should().Be( expected: itemCount );
		}

		public static void ShuffleByHarkerList<T>( [NotNull] ref List<T> list, Int32 iterations = 1, TimeSpan? forHowLong = null, [CanBeNull] SimpleCancel orUntilCancelled = null ) {
			if ( list == null ) { throw new ArgumentNullException( nameof( list ) ); }

			var itemCount = list.Count;

			if ( orUntilCancelled != null ) {
				var buffer = new Byte[ 4 * sizeof( UInt32 ) ];

				while ( !orUntilCancelled.HaveAnyCancellationsBeenRequested() ) {
					Randem.Instance.NextBytes( buffer );
					var x = 0.Next( maxValue: itemCount );
					var y = 0.Next( maxValue: itemCount );

					var itemA = list[ x ];
					var itemB = list[ y ];
					list.RemoveAt( x );
					list.RemoveAt( y );
					list.Insert( 0.Next( maxValue: itemCount ), itemA );
					list.Insert( 0.Next( maxValue: itemCount ), itemB );
				}
			}
		}

		public static void ShuffleByRandomThenByRandom<T>( ref List<T> list, Int32 iterations = 1 ) {
			while ( iterations.Any() ) {
				iterations--;
				var copy = list.AsParallel().OrderBy( keySelector: o => Randem.Next() ).ThenBy( keySelector: o => Randem.Next() ).ToList();
				list.Clear();
				list.AddRange( collection: copy ); //TODO can we just return/replace 'list' with 'copy' instead of addrange?
			}
		}

		public static IEnumerable<T> SideEffects<T>( [NotNull] this IEnumerable<T> items, [CanBeNull] Action<T> perfomAction ) {
			if ( items is null ) { throw new ArgumentNullException( nameof( items ) ); }

			foreach ( var item in items ) {
				perfomAction?.Invoke( item );

				yield return item;
			}
		}

		[NotNull]
		public static IEnumerable<IEnumerable<T>> Split<T>( [NotNull] this IEnumerable<T> list, Int32 parts ) {
			if ( list is null ) { throw new ArgumentNullException( nameof( list ) ); }

			var i = 0;
			var splits = from item in list group item by i++ % parts into part select part; //.AsEnumerable();

			return splits;
		}

		/// <summary>
		///     Swap the two indexes
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"> </param>
		/// <param name="index1"></param>
		/// <param name="index2"></param>
		public static void Swap<T>( [NotNull] this T[] array, Int32 index1, Int32 index2 ) {
			if ( array is null ) { throw new ArgumentNullException( nameof( array ) ); }

			var temp = array[ index1 ];
			array[ index1 ] = array[ index2 ];
			array[ index2 ] = temp;
		}

		/// <summary>
		///     <para>Remove and return the first item in the list, otherwise return null (or the default() for value types).</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		public static Boolean TakeFirst<TType>( [NotNull] this IList<TType> list, out TType item ) {
			if ( list is null ) { throw new ArgumentNullException( nameof( list ) ); }

			if ( list.Count <= 0 ) {
				item = default;

				return false;
			}

			item = list[ index: 0 ];
			list.RemoveAt( index: 0 );

			return true;
		}

		/// <summary>
		///     <para>Remove and return the first item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[CanBeNull]
		public static TType TakeFirst<TType>( [NotNull] this IList<TType> list ) {
			if ( list is null ) { throw new ArgumentNullException( nameof( list ) ); }

			if ( list.Count <= 0 ) { return default; }

			var item = list[ index: 0 ];
			list.RemoveAt( index: 0 );

			return item;
		}

		/// <summary>
		///     <para>Remove and return the last item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Boolean TakeLast<TType>( [NotNull] this IList<TType> list, out TType item ) {
			if ( list is null ) { throw new ArgumentNullException( nameof( list ) ); }

			var index = list.Count - 1;

			if ( index < 0 ) {
				item = default;

				return false;
			}

			item = list[ index: index ];
			list.RemoveAt( index: index );

			return true;
		}

		/// <summary>
		///     <para>Remove and return the last item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[CanBeNull]
		public static TType TakeLast<TType>( [NotNull] this IList<TType> list ) where TType : class {
			if ( list is null ) { throw new ArgumentNullException( nameof( list ) ); }

			var index = list.Count - 1;

			if ( index < 0 ) { return null; }

			var item = list[ index: index ];
			list.RemoveAt( index: index );

			return item;
		}

		/// <summary>
		///     Untested.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[NotNull]
		public static DataTable ToDataTable<T>( [CanBeNull] this IEnumerable<T> list ) {
			var dt = new DataTable();

			if ( list != null ) {
				PropertyInfo[] columns = null;

				foreach ( var record in list ) {
					if ( columns is null ) {
						columns = record.GetType().GetProperties();

						foreach ( var getProperty in columns ) {
							var icolType = getProperty.PropertyType;

							if ( icolType.IsGenericType && icolType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) { icolType = icolType.GetGenericArguments()[ 0 ]; }

							dt.Columns.Add( column: new DataColumn( columnName: getProperty.Name, dataType: icolType ) );
						}
					}

					var dr = dt.NewRow();

					foreach ( var p in columns ) {
						if ( p.GetValue( record, index: null ) is null ) { dr[ columnName: p.Name ] = DBNull.Value; }
						else { dr[ columnName: p.Name ] = p.GetValue( record, index: null ); }
					}

					dt.Rows.Add( row: dr );
				}
			}

			return dt;
		}

		/// <summary>
		///     Optimally create a list from the <paramref name="source" />.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">  </param>
		/// <param name="capacity"></param>
		/// <returns></returns>
		[NotNull]
		public static List<TSource> ToList<TSource>( [NotNull] this IEnumerable<TSource> source, Int32 capacity ) {
			if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

			if ( capacity < 0 ) { throw new ArgumentOutOfRangeException( nameof( capacity ) ); }

			var list = new List<TSource>( capacity: capacity );
			list.AddRange( collection: source );

			return list;
		}

		/// <summary>
		///     Do a Take() on the top X percent
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="x">     </param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<TSource> Top<TSource>( [NotNull] this IEnumerable<TSource> source, Double x ) {
			if ( source is null ) { throw new ArgumentNullException( nameof( source ) ); }

			var sources = source as IList<TSource> ?? source.ToList();

			return sources.Take( count: ( Int32 ) ( x * sources.Count ) );
		}

		[NotNull]
		public static List<T> ToSortedList<T>( [NotNull] this IEnumerable<T> values ) {
			var list = new List<T>( collection: values );
			list.Sort();

			return list;
		}

		/*
                public static T Append<T>( [NotNull] this Enum type, T value ) where T : struct {
                    if ( type is null ) {
                        throw new ArgumentNullException( "type" );
                    }
                    return ( T )( ValueType )( ( ( int )( ValueType )type | ( int )( ValueType )value ) );
                }
        */

		public static String ToStrings( [NotNull] this IEnumerable<Object> enumerable, Char c ) =>
			ToStrings( enumerable: enumerable, separator: new String( new[] {
				c
			} ) );

		/// <summary>
		///     <para>
		///         Returns a String with the <paramref name="separator" /> between each item of an
		///         <paramref name="enumerable" />.
		///     </para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="separator"> </param>
		/// <param name="atTheEnd">  </param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static String ToStrings<T>( [NotNull] this IEnumerable<T> enumerable, [NotNull] String separator = ", ", [CanBeNull] String atTheEnd = null ) {
			if ( enumerable is null ) { throw new ArgumentNullException( nameof( enumerable ) ); }

			if ( separator is null ) { throw new ArgumentNullException( nameof( separator ) ); }

			String result;
			var list = enumerable as IList<T> ?? enumerable.ToList();

			if ( String.IsNullOrEmpty( atTheEnd ) || list.Count <= 2 ) { result = String.Join( separator: separator, values: list ); }
			else {
				result = String.Join( separator: separator, values: list.Take( count: list.Count - 2 ) );

				while ( list.Count > 2 ) { list.RemoveAt( index: 0 ); }

				result += separator;

				if ( list.TakeFirst( item: out var item ) ) { result += item; }

				result += atTheEnd;

				if ( list.TakeFirst( out item ) ) { result += item; }
			}

			return result;
		}

		/// <summary>
		///     Wrapper for <see cref="ConcurrentQueue{T}.TryDequeue" />
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queue"></param>
		/// <param name="item"> </param>
		/// <returns></returns>
		public static Boolean TryTake<T>( [NotNull] this ConcurrentQueue<T> queue, out T item ) {
			if ( queue is null ) { throw new ArgumentNullException( nameof( queue ) ); }

			if ( Equals( queue, null ) ) { throw new ArgumentNullException( nameof( queue ) ); }

			return queue.TryDequeue( result: out item );
		}

		/// <summary>
		///     Wrapper for <see cref="ConcurrentStack{T}.TryPop" />
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack"></param>
		/// <param name="item"> </param>
		/// <returns></returns>
		public static Boolean TryTake<T>( [NotNull] this ConcurrentStack<T> stack, out T item ) {
			if ( null == stack ) { throw new ArgumentNullException( nameof( stack ) ); }

			return stack.TryPop( result: out item );
		}

		public static void Update<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, [NotNull] TKey key, TValue value ) {
			if ( dictionary is null ) { throw new ArgumentNullException( nameof( dictionary ) ); }

			dictionary.TryRemove( key, out var dummy ); //HACK
			dictionary.TryAdd( key, value );

			//var wtf = default( TValue );
			//dictionary.TryUpdate( key, value, wtf );  //BUG I don't understand the whole if-same-then-replace-semantics. If we're going to replace the value, then why do we care what the current value is anyway?
		}

		/// <summary>
		///     Returns all combinations of items in the source collection that satisfy the given
		///     <paramref name="relationshipFunc" />.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>
		///     An enumeration of all combinations of items that satisfy the <paramref name="relationshipFunc" />. Each
		///     combination will only be returned once (e.g. <c>[a, b]</c> but not <c>[b, a]</c>).
		/// </returns>
		public static IEnumerable<KeyValuePair<T, T>> WhereRelationship<T>( [NotNull] this IEnumerable<T> self, [NotNull] Func<T, T, Boolean> relationshipFunc ) {
			if ( self is null ) { throw new ArgumentNullException( nameof( self ), "WhereRelationship called on a null IEnumerable<T>." ); }

			if ( relationshipFunc is null ) { throw new ArgumentNullException( nameof( relationshipFunc ) ); }

			var aIndex = 0;

			foreach ( var a in self ) {

				foreach ( var b in self.Skip( count: ++aIndex ).Where( b => relationshipFunc( arg1: a, arg2: b ) || relationshipFunc( arg1: b, arg2: a ) ) ) { yield return new KeyValuePair<T, T>( a, b ); }
			}
		}
	}
}