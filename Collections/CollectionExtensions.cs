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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/CollectionExtensions.cs" was last cleaned by Rick on 2014/08/28 at 5:21 PM
#endregion

namespace Librainian.Collections {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Numerics;
	using System.Threading.Tasks;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Maths;
	using Threading;

	public static class CollectionExtensions {
		public static readonly List<String> EmptyList = new List<String>();

		/// <summary>
		///     <para>A list containing <see cref="bool.False" /> then <see cref="bool.True" />.</para>
		/// </summary>
		public static readonly Lazy<List<bool>> FalseThenTrue = new Lazy<List<bool>>( () => new List<bool>( new[ ] { false, true } ) );

		/// <summary>
		///     <para>A list containing <see cref="Boolean.True" /> then <see cref="Boolean.False" />.</para>
		/// </summary>
		public static readonly Lazy<List<bool>> TrueThenFalse = new Lazy<List<bool>>( () => new List<bool>( new[ ] { true, false } ) );

		public static void AddRange<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] IEnumerable<T> items ) {
			if ( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			if ( items == null ) {
				throw new ArgumentNullException( nameof( items ) );
			}
			Parallel.ForEach( source: items.AsParallel(), parallelOptions: ThreadingExtensions.Parallelism, body: collection.Add );
		}

		public static UInt64 LongSum( this IEnumerable<int> collection ) => collection.Aggregate( 0UL, ( current, u ) => current + ( UInt64 )u );

		public static void Add<T>( this IProducerConsumerCollection<T> collection, T item ) {
			if ( null == collection ) {
				throw new ArgumentNullException( nameof( collection ) );
			}

			collection.TryAdd( item );
		}

		/*
                public static T Append<T>( [NotNull] this Enum type, T value ) where T : struct {
                    if ( type == null ) {
                        throw new ArgumentNullException( "type" );
                    }
                    return ( T )( ValueType )( ( ( int )( ValueType )type | ( int )( ValueType )value ) );
                }
        */

		public static BigInteger CountBig<TType>( [NotNull] this IEnumerable<TType> items ) {
			if ( items == null ) {
				throw new ArgumentNullException( nameof( items ) );
			}
			return items.Aggregate( seed: BigInteger.Zero, func: ( current, item ) => current + BigInteger.One );
		}

		/// <summary>
		///     Returns duplicate items found in the <see cref="sequence" /> .
		/// </summary>
		public static HashSet<T> Duplicates<T>( this IEnumerable<T> sequence ) {
			if ( null == sequence ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}
			var set = new HashSet<T>();
			return new HashSet<T>( sequence.Where( item => !set.Add( item: item ) ) );
		}

		public static IEnumerable<T> EnumerableFromArray<T>( [NotNull] IEnumerable<T> array ) {
			if ( array == null ) {
				throw new ArgumentNullException( nameof( array ) );
			}
			return array;
		}

		/// <summary>
		///     The <seealso cref="List{T}.Capacity" /> is resized down to the <seealso cref="List{T}.Count" />.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="collection"> </param>
		public static void Fix<T>( [NotNull] this List<T> collection ) {
			if ( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}

			collection.Capacity = collection.Count();
			//collection.TrimExcess();
		}

		public static Boolean Has<T>( [NotNull] this Enum type, T value ) where T : struct {
			if ( type == null ) {
				throw new ArgumentNullException( nameof( type ) );
			}
			return ( ( int )( ValueType )type & ( int )( ValueType )value ) == ( int )( ValueType )value;
		}

		public static Boolean HasDuplicates<T>( [NotNull] this IEnumerable<T> sequence ) {
			if ( sequence == null ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}
			if ( Equals( sequence, null ) ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}
			return sequence.Duplicates().Any();
		}

		public static Boolean In<T>( this T value, [NotNull] params T[] items ) {
			if ( items == null ) {
				throw new ArgumentNullException( nameof( items ) );
			}
			return items.Contains( value );
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		/// <see cref="http://stackoverflow.com/a/3562370/956364" />
		public static int IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			if ( sequence == null ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}
			return source.IndexOfSequence( sequence, EqualityComparer<T>.Default );
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="sequence"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		/// <see cref="http://stackoverflow.com/a/3562370/956364" />
		public static int IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence, [NotNull] IEqualityComparer<T> comparer ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			if ( sequence == null ) {
				throw new ArgumentNullException( nameof( sequence ) );
			}
			if ( comparer == null ) {
				throw new ArgumentNullException( nameof( comparer ) );
			}

			var seq = sequence.ToArray();

			var p = 0; // current position in source sequence

			var i = 0; // current position in searched sequence

			var prospects = new List<int>(); // list of prospective matches

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
		///     <para>A list containing <see cref="Boolean.True" /> then <see cref="Boolean.False" />.</para>
		/// </summary>
		public static IEnumerable<bool> Infinitely( this Boolean value ) {
			do {
				yield return value;
			} while ( true );
			// ReSharper disable once FunctionNeverReturns
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="current"></param>
		/// <returns></returns>
		public static LinkedListNode<TType> NextOrFirst<TType>( [NotNull] this LinkedListNode<TType> current ) {
			if ( current == null ) {
				throw new ArgumentNullException( nameof( current ) );
			}
			return current.Next ?? current.List.First;
		}

		public static IEnumerable<IEnumerable<T>> Partition<T>( [NotNull] this IEnumerable<T> source, int size ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			T[] array = null;
			var count = 0;
			foreach ( var item in source ) {
				if ( array == null ) {
					array = new T[ size ];
				}
				array[ count ] = item;
				count++;
				if ( count != size ) {
					continue;
				}
				yield return new ReadOnlyCollection<T>( array );
				array = null;
				count = 0;
			}
			if ( array != null ) {
				Array.Resize( ref array, count );
				yield return new ReadOnlyCollection<T>( array );
			}
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="current"></param>
		/// <returns></returns>
		public static LinkedListNode<TType> PreviousOrLast<TType>( [NotNull] this LinkedListNode<TType> current ) {
			if ( current == null ) {
				throw new ArgumentNullException( nameof( current ) );
			}
			return current.Previous ?? current.List.Last;
		}

		public static IEnumerable<TU> Rank<T, TKey, TU>( [NotNull] this IEnumerable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, int, TU> selector ) {
			//if ( !source.Any() ) {
			//    yield break;
			//}

			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			if ( keySelector == null ) {
				throw new ArgumentNullException( nameof( keySelector ) );
			}
			if ( selector == null ) {
				throw new ArgumentNullException( nameof( selector ) );
			}
			var rank = 0;
			var itemCount = 0;
			var ordered = source.OrderBy( keySelector ).ToArray();
			var previous = keySelector( ordered[ 0 ] );
			foreach ( var t in ordered ) {
				itemCount += 1;
				var current = keySelector( t );
				if ( !current.Equals( previous ) ) {
					rank = itemCount;
				}
				yield return selector( t, rank );
				previous = current;
			}
		}

		public static T Remove<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			T result;
			return collection.TryTake( out result ) ? result : default(T);
		}

		public static T Remove<T>( [NotNull] this Enum type, T value ) where T : struct {
			if ( type == null ) {
				throw new ArgumentNullException( nameof( type ) );
			}
			return ( T )( ValueType )( ( ( int )( ValueType )type & ~( int )( ValueType )value ) );
		}

		/// <summary>
		///     Removes the <paramref name="specificItem" /> from the <paramref name="collection" /> and returns how many
		///     <paramref
		///         name="specificItem" />
		///     or null were removed.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="collection"> </param>
		/// <param name="specificItem"> </param>
		/// <returns> </returns>
		public static int Remove<T>( this IProducerConsumerCollection<T> collection, T specificItem ) {
			if ( null == collection ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			if ( Equals( specificItem, null ) ) {
				throw new ArgumentNullException( nameof( specificItem ) );
			}

			var removed = 0;

			while ( collection.Contains( specificItem ) ) {
				T temp;
				if ( !collection.TryTake( out temp ) ) {
					continue;
				}
				if ( Equals( temp, default(T) ) || Equals( temp, specificItem ) ) {
					removed += 1;
					continue;
				}

				collection.TryAdd( temp );
			}

			return removed;
		}

		public static int RemoveAll<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			T result;
			var removed = 0;
			while ( collection.TryTake( out result ) ) {
				removed++;
			}
			return removed;
		}

		public static void Clear<T>( [NotNull] this ConcurrentBag<T> bag ) {
			if ( bag == null ) {
				throw new ArgumentNullException( nameof( bag ) );
			}
			do {
				T result;
				bag.TryTake( out result );
			} while ( !bag.IsEmpty );
		}

		public static IEnumerable<T> RemoveEach<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
			if ( collection == null ) {
				throw new ArgumentNullException( nameof( collection ) );
			}
			T result;
			while ( collection.TryTake( out result ) ) {
				yield return result;
			}
		}

		/// <summary>
		///     <para>Shuffle an array[] in <paramref name="iterations" />.</para>
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="array"> </param>
		/// <param name="iterations"></param>
		/// <example>Deck.Shuffle( 7 );</example>
		[Obsolete("broken at the moment. seealso Shuffle<List>")]
		public static void Shuffle<T>( [NotNull] this T[] array, int iterations = 1 ) {
			if ( array == null ) {
				throw new ArgumentNullException( nameof( array ) );
			}
			if ( iterations < 1 ) {
				iterations = 1;
			}
			if ( array.Length < 1 ) {
				return;	//nothing to shuffle
			}

			while ( iterations > 0 ) {
				iterations--;

				// make a copy of all items
				var bag = new ConcurrentBag<T>( array );
				bag.Should().NotBeEmpty();
				var originalcount = bag.Count;

				var sqrt = ( int )Math.Sqrt( originalcount );
				if ( sqrt <= 1 ) {
					sqrt = 1;
				}

				// make some buckets.
				var buckets = new List<ConcurrentBag<T>>( capacity: sqrt );
				buckets.AddRange( 1.To( sqrt ).Select( i => new ConcurrentBag<T>() ) );

				// pull the items out of the bag, and put them into a random bucket each
				T item;
				while ( bag.TryTake( out item ) ) {
					var index = Randem.Next( 0, sqrt );
					buckets[ index ].Add( item );
				}
				bag.Should().BeEmpty( "All items should have been taken out of the bag" );

				while ( bag.Count < originalcount ) {
					var index = Randem.Next( minValue: 0, maxValue: buckets.Count );
					var bucket = buckets[ index ];

					if ( bucket.TryTake( out item ) ) {
						bag.Add( item );
					}
					if ( bucket.IsEmpty ) {
						buckets.Remove( bucket );
					}
				}
				bag.Count.Should().Be( originalcount );

				// put them back into the array
				var newArray = bag.ToArray();
				newArray.CopyTo( array, 0 );
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
		/// <typeparam name="T"> </typeparam>
		/// <param name="list"> </param>
		/// <param name="iterations"></param>
		/// <param name="shufflingType"></param>
		/// <example>Deck.Shuffle( 7 );</example>
		public static void Shuffle<T>( [NotNull] this List<T> list, UInt64 iterations = 1, ShufflingType shufflingType = ShufflingType.AutoChoice ) {
			if ( list == null ) {
				throw new ArgumentNullException( nameof( list ) );
			}
			try {
				if ( list.LongCount() <= 0 ) {
					return;	//nothing to shuffle
				}

				if ( iterations < 1 ) {
					iterations = 1;
				}

				switch ( shufflingType ) {
					case ShufflingType.ByGuid:
						{
							ShuffleByGuid( ref list, iterations );
						}
						break;

					case ShufflingType.ByRandom:
						{
							ShuffleByRandomThenByRandom( ref list, iterations );
						}
						break;

					case ShufflingType.ByHarker:
						{
							ShuffleByHarker( ref list, iterations );
						}
						break;

					case ShufflingType.ByBags:
						{
							ShuffleByBags( ref list, iterations, list.LongCount() );
						}
						break;

					case ShufflingType.AutoChoice:
						{
							ShuffleByHarker( ref list, iterations );
						}
						break;

					default:
						throw new ArgumentOutOfRangeException( nameof( shufflingType ) );
				}
			}
			catch ( IndexOutOfRangeException exception ) {
				exception.More();
			}
		}

		/// <summary>
		/// untested for speed and cpu/threading impact. Also, a lot of elements will NOT be shuffled much.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="iterations"></param>
		/// <param name="originalcount"></param>
		public static void ShuffleByBags<T>( ref List<T> list, UInt64 iterations, long originalcount ) {

			var bag = new ConcurrentBag<T>();

			while ( iterations > 0 ) {
				iterations--;

				bag.AddRange( list.AsParallel() );
				bag.Should().NotBeEmpty( because: "made an unordered copy of all items" );

				list.Clear();
				list.Should().BeEmpty( because: "emptied the original list" );

				list.AddRange( bag );
				list.LongCount().Should().Be( originalcount );

				bag.RemoveAll();
			}
		}

		/// <summary>
		/// Fast shuffle. Not guaranteed or tested to be the fastest, but it should shuffle *well* in reasonable time.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="iterations"></param>
		/// <remarks>The while.Any() could be replaced with a for loop.. the count is well known ahead of time.</remarks>
		/// <remarks>The new List(itemCount) could be streamlined to a lower number, once we know how much List.Add increments its allocations. For large lists hitting an OoM exception.</remarks>
		public static void ShuffleByHarker<T>( ref List<T> list, UInt64 iterations = 1 ) {
			var itemCount = list.Count;

			var copy = new List<T>( itemCount );

			while ( iterations > 0 ) {
				iterations--;

				while ( list.Any() ) {
					var index = Randem.Next( minValue: 0, maxValue: list.Count );
					copy.Add( list[ index ] );
					list.RemoveAt( index );
				}

				list.Should().BeEmpty();

				while ( copy.Any() ) {
					var index = Randem.Next( minValue: 0, maxValue: copy.Count );
					list.Add( copy[ index ] );
					copy.RemoveAt( index );
				}

				copy.Should().BeEmpty();
			}
			list.Count.Should().Be( itemCount );
		}

		public static void ShuffleByRandomThenByRandom<T>( ref List<T> list, UInt64 iterations = 1 ) {
			while ( iterations > 0 ) {
				iterations--;
				var copy = list.AsParallel().OrderBy( o => Randem.NextDouble() ).ThenBy( o => Randem.NextDouble() ).ToList();
				list.Clear();
				list.AddRange( copy );	//TODO can we just return/replace 'list' with 'copy' instead of addrange?
			}
		}

		public static void ShuffleByGuid<T>( ref List<T> list, UInt64 iterations = 1 ) {
			while ( iterations > 0 ) {
				iterations--;
				var copy = list.AsParallel().OrderBy( arg => Guid.NewGuid() ).ToList();
				list.Clear();
				list.AddRange( copy.AsParallel() );
			}
		}

		public static IEnumerable<IEnumerable<T>> Split<T>( [NotNull] this IEnumerable<T> list, int parts ) {
			if ( list == null ) {
				throw new ArgumentNullException( nameof( list ) );
			}
			var i = 0;
			var splits = from item in list
						 group item by i++ % parts
							 into part
						 select part; //.AsEnumerable();
			return splits;
		}

		/// <summary>
		///     Swap the two indexes
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="array"> </param>
		/// <param name="index1"> </param>
		/// <param name="index2"> </param>
		public static void Swap<T>( [NotNull] this T[] array, int index1, int index2 ) {
			if ( array == null ) {
				throw new ArgumentNullException( nameof( array ) );
			}
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
		public static Boolean TakeFirst<TType>( this IList<TType> list, out TType item ) {
			if ( list == null ) {
				throw new ArgumentNullException( nameof( list ) );
			}
			if ( list.Count <= 0 ) {
				item = default(TType);
				return false;
			}
			item = list[ 0 ];
			list.RemoveAt( 0 );
			return true;
		}

		/// <summary>
		/// <para>Remove and return the first item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[CanBeNull]
		public static TType TakeFirst<TType>( this IList<TType> list ) {
			if ( list == null ) {
				throw new ArgumentNullException( nameof( list ) );
			}
			if ( list.Count <= 0 ) {
				return default(TType);
			}
			var item = list[ 0 ];
			list.RemoveAt( 0 );
			return item;
		}

		/// <summary>
		///     <para>Remove and return the last item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static Boolean TakeLast<TType>( this IList<TType> list, out TType item ) {
			if ( list == null ) {
				throw new ArgumentNullException( nameof( list ) );
			}
			var index = list.Count - 1;
			if ( index < 0 ) {
				item = default(TType);
				return false;
			}
			item = list[ index ];
			list.RemoveAt( index );
			return true;
		}

		/// <summary>
		/// <para>Remove and return the last item in the list, otherwise return null.</para>
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		[CanBeNull]
		public static TType TakeLast<TType>( this IList<TType> list ) where TType : class {
			if ( list == null ) {
				throw new ArgumentNullException( nameof( list ) );
			}
			var index = list.Count - 1;
			if ( index < 0 ) {
				return null;
			}
			var item = list[ index ];
			list.RemoveAt( index );
			return item;
		}

		/// <summary>
		///     <para>
		///         Returns a String with the <paramref name="separator" /> between each item of an
		///         <paramref name="enumerable" />.
		///     </para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="separator"></param>
		/// <param name="atTheEnd"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static String ToStrings<T>( [NotNull] this IEnumerable<T> enumerable, [NotNull] String separator = ", ", String atTheEnd = null ) {
			if ( enumerable == null ) {
				throw new ArgumentNullException( nameof( enumerable ) );
			}
			if ( separator == null ) {
				throw new ArgumentNullException( nameof( separator ) );
			}

			String result;
			var list = enumerable as IList<T> ?? enumerable.ToList();

			if ( String.IsNullOrEmpty( atTheEnd ) || list.Count <= 2 ) {
				result = String.Join( separator, list );
			}
			else {
				result = String.Join( separator, list.Take( list.Count - 2 ) );
				while ( list.Count > 2 ) {
					list.RemoveAt( 0 );
				}
				result += separator;
				T item;
				if ( list.TakeFirst( out item ) ) {
					result += item;
				}
				result += atTheEnd;
				if ( list.TakeFirst( out item ) ) {
					result += item;
				}
			}
			return result;
		}

		/// <summary>
		///     Wrapper for <see cref="ConcurrentQueue{T}.TryDequeue" />
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="queue"> </param>
		/// <param name="item"> </param>
		/// <returns> </returns>
		public static Boolean TryTake<T>( [NotNull] this ConcurrentQueue<T> queue, out T item ) {
			if ( queue == null ) {
				throw new ArgumentNullException( nameof( queue ) );
			}
			if ( Equals( queue, null ) ) {
				throw new ArgumentNullException( nameof( queue ) );
			}
			return queue.TryDequeue( out item );
		}

		/// <summary>
		///     Wrapper for <see cref="ConcurrentStack{T}.TryPop" />
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="stack"> </param>
		/// <param name="item"> </param>
		/// <returns> </returns>
		public static Boolean TryTake<T>( this ConcurrentStack<T> stack, out T item ) {
			if ( null == stack ) {
				throw new ArgumentNullException( nameof( stack ) );
			}
			return stack.TryPop( out item );
		}

		public static void Update<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value ) {
			if ( dictionary == null ) {
				throw new ArgumentNullException( nameof( dictionary ) );
			}
			TValue dummy;
			dictionary.TryRemove( key, out dummy );	//HACK
			dictionary.TryAdd( key, value );
			//var wtf = default( TValue );
			//dictionary.TryUpdate( key, value, wtf );  //BUG I don't understand the whole if-same-then-replace-semantics. If we're going to replace the value, then why do we care what the current value is anyway?
		}

		public static void ForEach<T>( this IEnumerable<T> items, Action<T> action ) {
			if ( null == items ) {
				throw new ArgumentNullException( nameof( items ) );
			}
			if ( null == action ) {
				throw new ArgumentNullException( nameof( action ) );
			}

			foreach ( var item in items.Where( item => !Equals( item, null ) ) ) {
				action( item );
				//yield return item;
			}
		}

		public static byte[] ConcatenateByteArrays( params byte[][] bytearrays ) {
			var totalLength = bytearrays.Sum( t => t.Length );
			var idx = 0;
			var rv = new byte[ totalLength ];
			foreach ( var t in bytearrays ) {
				Array.Copy( t, 0, rv, idx, t.Length );
				idx += t.Length;
			}
			return rv;
		}

		public static byte[] ClonePortion( [NotNull] this byte[] bytes, int offset, int length ) {
			if ( bytes == null ) {
				throw new ArgumentNullException( "ba" );
			}
			if ( offset < 0 ) {
				throw new ArgumentOutOfRangeException();
			}
			if ( length < 0 ) {
				throw new ArgumentOutOfRangeException();
			}
			var copy = new byte[ length ];
			Array.Copy( bytes, offset, copy, 0, length );
			return copy;
		}

		public static byte[] CloneByteArray( this byte[] bytes ) {
			if ( bytes == null ) {
				return null;
			}
			var copy = new byte[ bytes.Length ];
			Array.Copy( bytes, copy, bytes.Length );
			return copy;
		}

		/// <summary>
		///     Do a Take() on the top X percent
		/// </summary>
		/// <typeparam name="TSource"> </typeparam>
		/// <param name="source"> </param>
		/// <param name="x"> </param>
		/// <returns> </returns>
		public static IEnumerable<TSource> Top<TSource>( [NotNull] this IEnumerable<TSource> source, Double x ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			var sources = source as IList<TSource> ?? source.ToList();
			return sources.Take( ( int )( x * sources.Count() ) );
		}
	}
}
