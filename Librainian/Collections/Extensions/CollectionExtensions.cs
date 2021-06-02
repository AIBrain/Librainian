// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "CollectionExtensions.cs" last touched on 2021-04-25 at 7:56 AM by Protiguous.

#nullable enable

namespace Librainian.Collections.Extensions {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Numerics;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	using Exceptions;
	using JetBrains.Annotations;
	using JM.LinqFaster.SIMD;
	using Maths;
	using Threading;

	public static class CollectionExtensions {

		public static void Add<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection, [CanBeNull] T item ) => collection.TryAdd( item );

		/// <summary>
		///     Does not guarantee <paramref name="items" /> will be added in given order.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="items"></param>
		public static void AddRange<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection, [DisallowNull][JetBrains.Annotations.NotNull] [ItemCanBeNull] IEnumerable<T> items ) =>
			Parallel.ForEach( items.AsParallel(), CPU.AllExceptOne, obj => collection.TryAdd( obj ) );

		/// <summary>
		///     Returns whether or not there are at least <paramref name="minInstances" /> elements in the source sequence
		///     that satisfy the given <paramref name="predicate" />.
		/// </summary>
		/// <param name="self">        The extended IEnumerable{T}.</param>
		/// <param name="minInstances">The number of elements that must satisfy the <paramref name="predicate" />.</param>
		/// <param name="predicate">   The function that determines whether or not an element is counted.</param>
		/// <returns>
		///     This method will immediately return true upon finding the <paramref name="minInstances" /> th element that
		///     satisfies the predicate, or if <paramref name="minInstances" />
		///     is 0. Otherwise, if <paramref name="minInstances" /> is greater than the size of the source sequence, or less than
		///     <paramref name="minInstances" /> elements are found to match the
		///     <paramref name="predicate" />, it will return false.
		/// </returns>
		[Pure]
		public static Boolean AtLeast<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> self, UInt64 minInstances, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, Boolean> predicate ) {
			if ( minInstances == 0 ) {
				return true;
			}

			UInt64 numInstSoFar = 0;

			return self.Any( element => predicate( element ) && ++numInstSoFar >= minInstances );
		}

		/// <summary>
		///     Ascertains whether there are no more than <paramref name="maxInstances" /> elements in the source sequence
		///     that satisfy the given <paramref name="predicate" />.
		/// </summary>
		/// <param name="self">        The extended IEnumerable{T}.</param>
		/// <param name="maxInstances">The maximum number of elements that can satisfy the <paramref name="predicate" />.</param>
		/// <param name="predicate">   The function that determines whether or not an element is counted.</param>
		/// <returns>
		///     This method will immediately return false upon finding the ( <paramref name="maxInstances" /> + 1)th element that
		///     satisfies the predicate. Otherwise, if
		///     <paramref name="maxInstances" /> is greater than the size of the source sequence, or less than
		///     <paramref name="maxInstances" /> elements are found to match the
		///     <paramref name="predicate" />, it will return true.
		/// </returns>
		[Pure]
		public static Boolean AtMost<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> self, UInt64 maxInstances, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, Boolean> predicate ) {
			if ( self is null ) {
				throw new ArgumentNullException( nameof( self ), "AtMost called on a null IEnumerable<>." );
			}

			if ( predicate is null ) {
				throw new ArgumentNullException( nameof( predicate ) );
			}

			UInt64 numInstSoFar = 0;

			return self.All( element => !predicate( element ) || ++numInstSoFar <= maxInstances );
		}

		public static Int32 Clear<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection ) => collection.RemoveAll();

		public static void Clear<T>( [DisallowNull][JetBrains.Annotations.NotNull] this ConcurrentBag<T> bag ) {
			while ( !bag.IsEmpty ) {
				bag.TryTake( out var _ );
			}
		}

		public static Task ClearAsync<T>( [DisallowNull][JetBrains.Annotations.NotNull] this ConcurrentBag<T> bag ) => Task.Run( bag.Clear );

		/*

		/// <summary>
		///     Side effects of <paramref name="items" /> other than a byte[] (array) are unknown!
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		[NotNull]
		[Pure]
		public static T[] Clone<T>( [NotNull] this T[] items ) {
			if ( items is Byte[] bytes ) {
				var clone = new T[bytes.Length];
				Buffer.BlockCopy( bytes, 0, clone, 0, bytes.Length );

				return clone;
			}

			var index = 0;
			var copy = new T[items.Length];

			foreach ( var VARIABLE in items ) {
				copy[index++] = VARIABLE.Copy() as T;
			}

			return copy;
		}
        */

		[JetBrains.Annotations.NotNull]
		[Pure]
		public static Byte[] ClonePortion( [DisallowNull][JetBrains.Annotations.NotNull] this Byte[] bytes, Int32 offset, Int32 length ) {
			if ( bytes is null ) {
				throw new ArgumentNullException( nameof( bytes ) );
			}

			if ( offset < 0 ) {
				throw new ArgumentOutOfRangeException( nameof( offset ) );
			}

			if ( length < 0 ) {
				throw new ArgumentOutOfRangeException( nameof( length ) );
			}

			var copy = new Byte[ length ];
			Buffer.BlockCopy( bytes, offset, copy, 0, length );

			return copy;
		}

		/// <summary>Concat multiple byte arrays into one new larger array.</summary>
		/// <param name="arrays"></param>
		/// <returns></returns>
		[JetBrains.Annotations.NotNull]
		[Pure]
		public static Byte[] Concat<T>( [DisallowNull][JetBrains.Annotations.NotNull] params T[][] arrays ) {
			var totalLength = arrays.Select( bytes => ( UInt64 )bytes.Length ).Aggregate<UInt64, UInt64>( 0, ( current, i ) => current + i );

			if ( totalLength > Int32.MaxValue ) {
				//throw new OutOfRangeException( $"The total size of the arrays ({totalLength:N0}) is too large." );
			}

			var both = new Byte[ totalLength ]; //BUG Let it throw if the memory cannot be allocated.
			var offset = 0;

			foreach ( var data in arrays ) {
				var length = Buffer.ByteLength( data );
				Buffer.BlockCopy( data, 0, both, offset, length );
				offset += length;
			}

			return both;
		}

		/// <summary>Checks if two IEnumerables contain the exact same elements and same number of elements. Order does not matter.</summary>
		/// <typeparam name="T">The Type of object.</typeparam>
		/// <param name="a">The first collection.</param>
		/// <param name="b">The second collection.</param>
		/// <returns>True if both IEnumerables contain the same items, and same number of items; otherwise, false.</returns>
		[Pure]
		public static Boolean ContainSameElements<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IList<T> a, [DisallowNull][JetBrains.Annotations.NotNull] IList<T> b ) {
			if ( a is null ) {
				throw new ArgumentNullException( nameof( a ) );
			}

			if ( b is null ) {
				throw new ArgumentNullException( nameof( b ) );
			}

			if ( a.Count != b.Count ) {
				return false;
			}

			if ( a.Count == 0 && b.Count == 0 ) {
				return true; //empty set matches empty set. expected result?
			}

			//BUG Needs unit testing to verify if this works as expected. 1,2,3 == 3,1,2 == 2,3,1
			var o = a.OrderBy( arg => arg );
			var p = b.OrderBy( arg => arg );

			return o.SequenceEqual( p );
		}

		[Pure]
		public static BigInteger CountBig<TType>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<TType> items ) {
			if ( items is null ) {
				throw new ArgumentNullException( nameof( items ) );
			}

			return items.LongCount();

			//return items.Aggregate( BigInteger.Zero, ( current, item ) => current + BigInteger.One );
		}

		/// <summary>
		///     Counts the number of times each element appears in a collection, and returns a
		///     <see cref="IDictionary{T, V}">dictionary</see>; where each key is an element and its value
		///     is the number of times that element appeared in the source collection.
		/// </summary>
		/// <param name="values">The extended IEnumerable{T}.</param>
		/// <returns>A dictionary of elements mapped to the number of times they appeared in <paramref name="values" />.</returns>
		[JetBrains.Annotations.NotNull]
		[Pure]
		public static IDictionary<T, Int32> CountInstances<T>( [DisallowNull][JetBrains.Annotations.NotNull] [ItemNotNull] this IEnumerable<T> values ) where T : notnull {
			if ( values is null ) {
				throw new ArgumentNullException( nameof( values ), "CountInstances called on a null IEnumerable<T>." );
			}

			IDictionary<T, Int32> result = new Dictionary<T, Int32>();

			foreach ( var element in values ) {
				if ( result.ContainsKey( element ) ) {
					++result[ element ];
				}
				else {
					result[ element ] = 1;
				}
			}

			return result;
		}

		/// <summary>
		///     Counts how many pairs of elements in the source sequence share the relationship defined by
		///     <paramref name="relationship" />.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationship">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>The number of pairs found.</returns>
		[Pure]
		public static Int32 CountRelationship<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> self, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, T, Boolean> relationship ) =>
			Relationships( self, relationship ).Count();

		/// <summary>Returns duplicate items found in the <see cref="sequence" /> .</summary>
		/// <param name="sequence">todo: describe sequence parameter on Duplicates</param>
		[JetBrains.Annotations.NotNull]
		[Pure]
		public static IEnumerable<T> Duplicates<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> sequence ) {
			var set = new HashSet<T>();

			return sequence.Where( item => !set.Add( item ) );
		}

		/// <summary>
		///     Return an empty set of type of <paramref name="self" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		[Pure]
		public static IEnumerable<T> Empty<T>( [DisallowNull][JetBrains.Annotations.NotNull] this T self ) {
			yield break;
		}

		/// <summary>
		///     Return an empty set of type of <paramref name="self" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		[Pure]
#pragma warning disable 1998
		public static async IAsyncEnumerable<T> EmptyAsync<T>( [DisallowNull][JetBrains.Annotations.NotNull] this T self ) {
#pragma warning restore 1998
			yield break;
		}

		/// <summary>
		///     Returns the first two items to in the source collection that satisfy the given
		///     <paramref name="relationship" /> , or <c>null</c> if no match was found.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationship">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>
		///     A tuple of the first two elements that match the given relationship, or <c>null</c> if no such relationship
		///     exists.
		/// </returns>
		[Pure]
		public static (T a, T b)? FirstRelationship<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> self, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, T, Boolean> relationship ) {
			var index = 0;

			var enumerable = self.ToList();

			foreach ( var a in enumerable ) {
				foreach ( var b in enumerable.Skip( ++index ).Where( b => relationship( a, b ) || relationship( b, a ) ) ) {
					return ( a, b );
				}
			}

			return default( (T a, T b)? );
		}

		[ItemCanBeNull]
		[Pure]
		public static IEnumerable<T> ForEach<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> items, [DisallowNull][JetBrains.Annotations.NotNull] Action<T> action ) {
			foreach ( var item in items ) {
				action( item );

				yield return item;
			}
		}

		/// <summary>http://blogs.msdn.com/b/pfxteam/archive/2012/02/04/10264111.aspx</summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary">     </param>
		/// <param name="key">      </param>
		/// <param name="function"></param>
		/// <param name="added">    </param>
		/// <returns></returns>
		[Pure]
		[CanBeNull]
		public static TValue GetOrAdd<[JetBrains.Annotations.NotNull] TKey, TValue>(
			[JetBrains.Annotations.NotNull] this ConcurrentDictionary<TKey, TValue> dictionary,
			[JetBrains.Annotations.NotNull] TKey key,
			[JetBrains.Annotations.NotNull] Func<TKey, TValue> function,
			out Boolean added
		) where TKey : notnull {
			if ( dictionary == null ) {
				throw new ArgumentNullException( nameof( dictionary ) );
			}

			if ( key is null ) {
				throw new ArgumentNullException( nameof( key ) );
			}

			if ( function is null ) {
				throw new ArgumentNullException( nameof( function ) );
			}

			while ( true ) {
				dictionary.GetOrAdd( key, _ => function( key ), out added ); //BUG Does function run if the key is not added?

				return dictionary[ key ];
			}
		}

		[Pure]
		public static Boolean Has<T>( [DisallowNull][JetBrains.Annotations.NotNull] this Enum type, T value ) where T : struct {
			if ( type is null ) {
				throw new ArgumentNullException( nameof( type ) );
			}

			return ( ( Int32 )( ValueType )type & ( Int32 )( ValueType )value ) == ( Int32 )( ValueType )value;
		}

		[Pure]
		public static Boolean HasDuplicates<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> sequence ) {
			if ( sequence is null ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}

			if ( Equals( sequence, null ) ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}

			return sequence.Duplicates().Any();
		}

		[Pure]
		public static Boolean In<T>( [DisallowNull][JetBrains.Annotations.NotNull] this T value, [DisallowNull][JetBrains.Annotations.NotNull] params T[] items ) {
			if ( value is null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			return items.Contains( value );
		}

		[Pure]
		public static Int32 IndexOf<T>( [DisallowNull][JetBrains.Annotations.NotNull] this T[] self, [CanBeNull] T item ) => Array.IndexOf( self, item );

		/// <summary></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">  </param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		/// <remarks>http://stackoverflow.com/a/3562370/956364</remarks>
		[Pure]
		public static Int32 IndexOfSequence<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> source, [DisallowNull][JetBrains.Annotations.NotNull] IEnumerable<T> sequence ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( sequence is null ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}

			return source.IndexOfSequence( sequence, EqualityComparer<T>.Default );
		}

		/// <summary></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">  </param>
		/// <param name="sequence"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		/// <remarks>http://stackoverflow.com/a/3562370/956364</remarks>
		[Pure]
		public static Int32 IndexOfSequence<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> source, [DisallowNull][JetBrains.Annotations.NotNull] IEnumerable<T> sequence, [DisallowNull][JetBrains.Annotations.NotNull] IEqualityComparer<T> comparer ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( sequence is null ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}

			if ( comparer is null ) {
				throw new ArgumentNullException( nameof( comparer ) );
			}

			var seq = sequence.ToArray();

			var p = 0; // current position in source sequence

			var i = 0; // current position in searched sequence

			var prospects = new List<Int32>(); // list of prospective matches

			foreach ( var item in source ) {
				// Remove bad prospective matches
				var item1 = item;
				var p1 = p;
				prospects.RemoveAll( k => !comparer.Equals( item1, seq[ p1 - k ] ) );

				// Is it the start of a prospective match ?
				if ( comparer.Equals( item, seq[ 0 ] ) ) {
					prospects.Add( p );
				}

				// Does current character continues partial match ?
				if ( comparer.Equals( item, seq[ i ] ) ) {
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
						var k = prospects[ 0 ];
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
		[Pure]
		public static IEnumerable<Boolean> Infinitely( this Boolean value ) {
			do {
				yield return value;
			} while ( true );

			// ReSharper disable once IteratorNeverReturns
		}

		/// <summary>Checks if an IEnumerable is empty.</summary>
		/// <typeparam name="T">The type of objects to enumerate.</typeparam>
		/// <param name="source">The IEnumerable to check if empty.</param>
		/// <returns>True if the <paramref name="source" /> is null or empty; otherwise false.</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Boolean IsEmpty<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> source ) => source.Any() != true;

		[Pure]
		public static Int64 LongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Byte> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[Pure]
		public static Int64 LongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int16> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[Pure]
		public static Int64 LongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int32> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[Pure]
		public static Int64 LongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int64> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[CanBeNull]
		[Pure]
		public static LinkedListNode<TType>? NextOrFirst<TType>( [DisallowNull][JetBrains.Annotations.NotNull] this LinkedListNode<TType> current ) => current.Next ?? current.List?.First;

		[JetBrains.Annotations.NotNull]
		[Pure]
		public static IEnumerable<T> OrderBy<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> list, [DisallowNull][JetBrains.Annotations.NotNull] IEnumerable<T> guide ) {
			var toBeSorted = new HashSet<T>( list );

			return guide.Where( member => toBeSorted.Contains( member ) );
		}

		[ItemNotNull]
		[Pure]
		public static IEnumerable<IEnumerable<T>> Partition<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> source, Int32 size ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			T[]? array = null;
			var count = 0;

			foreach ( var item in source ) {
				array ??= new T[ size ];

				array[ count ] = item;
				count++;

				if ( count != size ) {
					continue;
				}

				yield return new ReadOnlyCollection<T>( array );
				array = null;
				count = 0;
			}

			if ( array is null ) {
				yield break;
			}

			Array.Resize( ref array, count );

			yield return new ReadOnlyCollection<T>( array! );
		}

		/// <summary>untested</summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="self"></param>
		/// <param name="key"> </param>
		/// <returns></returns>
		[CanBeNull]
		[Pure]
		public static TValue Pop<TKey, TValue>( [DisallowNull][JetBrains.Annotations.NotNull] this IDictionary<TKey, TValue> self, [DisallowNull][JetBrains.Annotations.NotNull] TKey key ) {
			if ( self is null ) {
				throw new ArgumentNullException( nameof( self ) );
			}

			if ( key is null ) {
				throw new ArgumentNullException( nameof( key ) );
			}

			//BUG Needs unit testing.
			if ( self.TryGetValue( key, out var value ) ) {
				self.Remove( key );

				return value;
			}

			return default( TValue )!;
		}

		/// <summary>untested</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		[CanBeNull]
		[Pure]
		public static T PopFirst<T>( [DisallowNull][JetBrains.Annotations.NotNull] this ICollection<T> self ) {
			if ( self is null ) {
				throw new ArgumentNullException( nameof( self ) );
			}

			//BUG Needs unit testing.
			var result = self.First();
			self.Remove( result );

			return result;
		}

		/// <summary>untested</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		[CanBeNull]
		[Pure]
		public static T PopLast<T>( [DisallowNull][JetBrains.Annotations.NotNull] this ICollection<T> self ) {
			if ( self is null ) {
				throw new ArgumentNullException( nameof( self ) );
			}

			//BUG Needs unit testing.
			var result = self.Last();
			self.Remove( result );

			return result;
		}

		/// <summary></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="current"></param>
		/// <returns></returns>
		/// <remarks>Basically if the previous node is null, then wrap back around to the last item.</remarks>
		[CanBeNull]
		[Pure]
		public static LinkedListNode<T>? PreviousOrLast<T>( [DisallowNull][JetBrains.Annotations.NotNull] this LinkedListNode<T> current ) => current.Previous ?? current.List?.Last;

		[ItemCanBeNull]
		[Pure]
		public static IEnumerable<TU> Rank<T, TKey, TU>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> source, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, TKey> keySelector, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, Int32, TU> selector ) {
			//if ( !source.Any() ) {
			//    yield break;
			//}

			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( keySelector is null ) {
				throw new ArgumentNullException( nameof( keySelector ) );
			}

			if ( selector is null ) {
				throw new ArgumentNullException( nameof( selector ) );
			}

			var rank = 0;
			var itemCount = 0;
			var ordered = source.OrderBy( keySelector ).ToArray();
			var previous = keySelector( ordered[ 0 ] );

			foreach ( var t in ordered ) {
				itemCount += 1;
				var current = keySelector( t );

				if ( current is { } && !current.Equals( previous ) ) {
					rank = itemCount;
				}

				yield return selector( t, rank );
				previous = current;
			}
		}

		/// <summary>
		///     Counts how many pairs of elements in the source sequence share the relationship defined by
		///     <paramref name="relationship" />.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationship">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>The number of pairs found.</returns>
		[Pure]
		public static IEnumerable<T> Relationships<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> self, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, T, Boolean> relationship ) {
			var enumerable = self as T[] ?? self.ToArray();

			return enumerable.Select( ( a, aIndex ) => enumerable.Skip( aIndex + 1 ).Where( b => relationship( a, b ) || relationship( b, a ) ) ).SelectMany( b => b );
		}

		[CanBeNull]
		[Pure]
		public static T Remove<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection.TryTake( out var result ) ) {
				return result;
			}

			return default( T )!;
		}

		[Pure]
		public static T Remove<T>( [DisallowNull][JetBrains.Annotations.NotNull] this Enum type, T value ) where T : struct {
			if ( type is null ) {
				throw new ArgumentNullException( nameof( type ) );
			}

			return ( T )( ( ( Int32 )( ValueType )type & ~( Int32 )( value as ValueType ) ) as ValueType );
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
		[Pure]
		public static Object? Remove<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection, [DisallowNull][JetBrains.Annotations.NotNull] T specificItem ) {
			if ( collection is null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}

			if ( Equals( specificItem, null ) ) {
				throw new ArgumentNullException( nameof( specificItem ) );
			}

			var sanity = collection.Count * 2;

			while ( sanity.Any() && collection.Contains( specificItem ) ) {
				--sanity;

				if ( collection.TryTake( out var temp ) ) {
					if ( Equals( temp, specificItem ) ) {
						return specificItem;
					}

					collection.TryAdd( temp );
				}
			}

			return null;
		}

		public static Int32 RemoveAll<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection is null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}

			var removed = 0;

			while ( collection.Any() ) {
				while ( collection.TryTake( out var _ ) ) {
					++removed;
				}
			}

			return removed;
		}

		[ItemCanBeNull]
		public static IEnumerable<T> RemoveEach<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection is null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}

			while ( collection.TryTake( out var result ) ) {
				yield return result;
			}
		}

		[ItemCanBeNull]
		public static IEnumerable<T> SideEffects<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> items, [CanBeNull] Action<T>? perfomAction ) {
			if ( items is null ) {
				throw new ArgumentNullException( nameof( items ) );
			}

			foreach ( var item in items ) {
				perfomAction?.Invoke( item );

				yield return item;
			}
		}

		[JetBrains.Annotations.NotNull]
		[Pure]
		public static IEnumerable<IEnumerable<T>> Split<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> list, Int32 parts ) {
			if ( list is null ) {
				throw new ArgumentNullException( nameof( list ) );
			}

			var i = 0;

			var splits = list.GroupBy( item => ++i % parts ).Select( part => part );

			return splits;
		}

		[Pure]
		public static Int64 SumLong( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Byte> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[Pure]
		public static Int64 SumLong( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int16> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[Pure]
		public static Int64 SumLong( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int32> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		[Pure]
		public static Int64 SumLong( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int64> collection ) => collection.Aggregate( 0L, ( current, u ) => current + u );

		/// <summary>Swap the two indexes</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"> </param>
		/// <param name="index1"></param>
		/// <param name="index2"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void Swap<T>( [DisallowNull][JetBrains.Annotations.NotNull] this T[] array, Int32 index1, Int32 index2 ) {
			if ( array is null ) {
				throw new ArgumentNullException( nameof( array ) );
			}

			var length = array.Length;

			if ( index1 < 0 ) {
				throw new OutOfRangeException( $"{nameof( index1 )} cannot be lower than 0." );
			}

			if ( index1 >= length ) {
				throw new OutOfRangeException( $"{nameof( index1 )} cannot be higher than {length - 1}." );
			}

			if ( index2 < 0 ) {
				throw new OutOfRangeException( $"{nameof( index2 )} cannot be lower than 0." );
			}

			if ( index2 >= length ) {
				throw new OutOfRangeException( $"{nameof( index2 )} cannot be higher than {length - 1}." );
			}

			var temp = array[ index1 ];
			array[ index1 ] = array[ index2 ];
			array[ index2 ] = temp;
		}

		/// <summary>
		///     <para>Remove and return the first item in the list, otherwise return null (or the default() for value types).</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		public static Boolean TakeFirst<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IList<T> list, [CanBeNull] out T item ) {
			if ( list is null ) {
				throw new ArgumentNullException( nameof( list ) );
			}

			if ( list.Count <= 0 ) {
				item = default( T )!;

				return false;
			}

			item = list[ 0 ];
			list.RemoveAt( 0 );

			return true;
		}

		/// <summary>
		///     <para>Remove and return the first item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[CanBeNull]
		public static T TakeFirst<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IList<T> list ) {
			if ( list is null ) {
				throw new ArgumentNullException( nameof( list ) );
			}

			if ( list.Count <= 0 ) {
				return default( T )!;
			}

			var item = list[ 0 ];
			list.RemoveAt( 0 );

			return item;
		}

		/// <summary>
		///     <para>Remove and return the last item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Boolean TakeLast<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IList<T> list, [CanBeNull] out T item ) {
			if ( list is null ) {
				throw new ArgumentNullException( nameof( list ) );
			}

			var index = list.Count - 1;

			if ( index < 0 ) {
				item = default( T )!;

				return false;
			}

			item = list[ index ];
			list.RemoveAt( index );

			return true;
		}

		/// <summary>
		///     <para>Remove and return the last item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[CanBeNull]
		public static T? TakeLast<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IList<T> list ) where T : class {
			if ( list is null ) {
				throw new ArgumentNullException( nameof( list ) );
			}

			var index = list.Count - 1;

			if ( index < 0 ) {
				return default( T? );
			}

			var item = list[ index ];
			list.RemoveAt( index );

			return item;
		}

		/// <summary>Optimally create a list from the <paramref name="source" />.</summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source">  </param>
		/// <returns></returns>
		[JetBrains.Annotations.NotNull]
		[Pure]
		public static List<TSource> ToListTrimExcess<TSource>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<TSource> source ) {
			var bob = new List<TSource>( source );
			bob.TrimExcess();

			return bob;
		}

		/// <summary>Do a Take() on the top X percent</summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <param name="x">The percent of <paramref name="source" /> to get.</param>
		/// <returns></returns>
		[JetBrains.Annotations.NotNull]
		[Pure]
		public static IEnumerable<TSource> Top<TSource>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<TSource> source, Double x ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			var sources = source as IList<TSource> ?? source.ToList();

			return sources.Take( ( Int32 )( x * sources.Count ) );
		}

		[JetBrains.Annotations.NotNull]
		[Pure]
		public static List<T> ToSortedList<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> values ) {
			var list = new List<T>( values );
			list.Sort();

			return list;
		}

		/// <summary>Extension to aomtically remove a KVP.</summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[Pure]
		public static Boolean TryRemove<TKey, TValue>( [DisallowNull][JetBrains.Annotations.NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, [DisallowNull][JetBrains.Annotations.NotNull] TKey key, [DisallowNull][JetBrains.Annotations.NotNull] TValue value )  where TKey:notnull{
			if ( dictionary is null ) {
				throw new ArgumentNullException( nameof( dictionary ) );
			}

			return ( ( ICollection<KeyValuePair<TKey, TValue>> )dictionary ).Remove( new KeyValuePair<TKey, TValue>( key, value ) );
		}

		/// <summary>Wrapper for <see cref="ConcurrentQueue{T}.TryDequeue" /></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queue"></param>
		/// <param name="item"> </param>
		/// <returns></returns>
		[Pure]
		public static Boolean TryTake<T>( [DisallowNull][JetBrains.Annotations.NotNull] this ConcurrentQueue<T> queue, [CanBeNull] out T item ) {
			if ( queue is null ) {
				throw new ArgumentNullException( nameof( queue ) );
			}

			if ( Equals( queue, null ) ) {
				throw new ArgumentNullException( nameof( queue ) );
			}

			return queue.TryDequeue( out item! );
		}

		/// <summary>Wrapper for <see cref="ConcurrentStack{T}.TryPop" /></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack"></param>
		/// <param name="item"> </param>
		/// <returns></returns>
		[Pure]
		public static Boolean TryTake<T>( [DisallowNull][JetBrains.Annotations.NotNull] this ConcurrentStack<T> stack, [CanBeNull] out T item ) {
			if ( null == stack ) {
				throw new ArgumentNullException( nameof( stack ) );
			}

			return stack.TryPop( out item! );
		}

		[Pure]
		public static UInt64 ULongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<SByte> collection ) => ( UInt64 )( ( SByte[] )collection ).SumS();

		[Pure]
		public static UInt64 ULongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Byte> collection ) => ( ( Byte[] )collection ).SumS();

		[Pure]
		public static UInt64 ULongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int16> collection ) => ( UInt64 )( ( Int16[] )collection ).SumS();

		[Pure]
		public static UInt64 ULongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int32> collection ) => ( UInt64 )( ( Int32[] )collection ).SumS();

		[Pure]
		public static UInt64 ULongSum( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<Int64> collection ) => ( UInt64 )( ( Int64[] )collection ).SumS();

		/// <summary>why?</summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void Update<TKey, TValue>( [DisallowNull][JetBrains.Annotations.NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, [DisallowNull][JetBrains.Annotations.NotNull] TKey key, [CanBeNull] TValue value ) where TKey:notnull{
			if ( dictionary is null ) {
				throw new ArgumentNullException( nameof( dictionary ) );
			}

			dictionary[ key ] = value;
		}

		/// <summary>
		///     Returns all combinations of items in the source collection that satisfy the given
		///     <paramref name="relationshipFunc" />.
		/// </summary>
		/// <param name="self">            The extended IEnumerable{T}.</param>
		/// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
		/// <returns>
		///     An enumeration of all combinations of items that satisfy the <paramref name="relationshipFunc" />. Each combination
		///     will only be returned once (e.g. <c>[a, b]</c> but not
		///     <c>[b, a]</c>).
		/// </returns>
		[Pure]
		public static IEnumerable<KeyValuePair<T, T>> WhereRelationship<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T> self, [DisallowNull][JetBrains.Annotations.NotNull] Func<T, T, Boolean> relationshipFunc ) {
			if ( self is null ) {
				throw new ArgumentNullException( nameof( self ), "WhereRelationship called on a null IEnumerable<T>." );
			}

			if ( relationshipFunc is null ) {
				throw new ArgumentNullException( nameof( relationshipFunc ) );
			}

			var aIndex = 0;

			var enumerable = self.ToList();

			foreach ( var a in enumerable ) {
				foreach ( var b in enumerable.Skip( ++aIndex ).Where( b => relationshipFunc( a, b ) || relationshipFunc( b, a ) ) ) {
					yield return new KeyValuePair<T, T>( a, b );
				}
			}
		}

		/// <summary>
		///     Returns a sequence with the null instances removed.
		/// </summary>
		[ItemNotNull]
		public static IEnumerable<T> WhereNotNull<T>( [DisallowNull][JetBrains.Annotations.NotNull] this IEnumerable<T?> source ) where T : class => source.Where( x => x is not null )!;

		/// <summary>
		///     Returns a sequence with the null instances removed.
		/// </summary>
		public static IAsyncEnumerable<T> WhereNotNull<T>( [DisallowNull] [JetBrains.Annotations.NotNull] this IAsyncEnumerable<T?> source ) where T : class => source.Where( x => x is not null )!;

	}

}