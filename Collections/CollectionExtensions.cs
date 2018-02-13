// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CollectionExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

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
    using FluentAssertions;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Threading;

    public static class CollectionExtensions {

		/// <summary>
		///     <para>A list containing <see cref="Boolean.False" /> then <see cref="Boolean.True" />.</para>
		/// </summary>
		public static readonly Lazy<List<Boolean>> FalseThenTrue = new Lazy<List<Boolean>>( () => new List<Boolean>( new[] { false, true } ) );

        /// <summary>
        ///     <para>A list containing <see cref="Boolean.True" /> then <see cref="Boolean.False" />.</para>
        /// </summary>
        public static readonly Lazy<List<Boolean>> TrueThenFalse = new Lazy<List<Boolean>>( () => new List<Boolean>( new[] { true, false } ) );

        /// <summary>
        /// </summary>
        public static List<String> EmptyList { get; } = new List<String>( Empty<String>() );

        public static void Add<T>( this IProducerConsumerCollection<T> collection, T item ) {
            if ( null == collection ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            collection.TryAdd( item );
        }

        public static void AddRange<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] IEnumerable<T> items ) {
            if ( collection == null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }
            if ( items == null ) {
                throw new ArgumentNullException( nameof( items ) );
            }
            Parallel.ForEach( source: items.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive, body: collection.Add );
        }

        /// <summary>
        ///     Determines whether or not the given sequence contains any duplicate elements.
        /// </summary>
        /// <param name="this">The extended <see cref="IEnumerable{T}" />.</param>
        /// <returns>True if the sequence contains duplicate elements, false if not.</returns>
        public static Boolean AnyDuplicates<T>( this IEnumerable<T> @this ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "AnyDuplicates<T> called on a null IEnumerable<T>." );
            }
            return AnyRelationship( @this, ( arg1, arg2 ) => arg1.Equals( arg2 ) );
        }

        /// <summary>
        ///     Determines whether or not a given relationship exists between any two elements in the sequence.
        /// </summary>
        /// <param name="this">The extended <see cref="IEnumerable{T}" />.</param>
        /// <param name="relationshipFunc">
        ///     The function that determines whether the given relationship exists
        ///     between two elements.
        /// </param>
        /// <returns>True if the relationship exists between any two elements, false if not.</returns>
        public static Boolean AnyRelationship<T>( this IEnumerable<T> @this, Func<T, T, Boolean> relationshipFunc ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "AnyRelationship called on a null IEnumerable<T>." );
            }

            // ReSharper disable once PossibleMultipleEnumeration
	        var enumerable = @this as T[] ?? @this.ToArray();
	        return enumerable.Select( ( a, aIndex ) => {
                                     // ReSharper disable once PossibleMultipleEnumeration
                                     return enumerable.Skip( aIndex + 1 ).Any( b => ( relationshipFunc?.Invoke( a, b ) ?? default ) || ( relationshipFunc?.Invoke( b, a ) ?? default ) );
                                 } ).Any( value => value );
        }

        /// <summary>
        ///     Returns whether or not there are at least <paramref name="minInstances" /> elements in the source sequence
        ///     that satisfy the given <paramref name="predicate" />.
        /// </summary>
        /// <param name="this">The extended IEnumerable{T}.</param>
        /// <param name="minInstances">The number of elements that must satisfy the <paramref name="predicate" />.</param>
        /// <param name="predicate">The function that determines whether or not an element is counted.</param>
        /// <returns>
        ///     This method will immediately return true upon finding the <paramref name="minInstances" />th element
        ///     that satisfies the predicate, or if <paramref name="minInstances" /> is 0. Otherwise, if
        ///     <paramref name="minInstances" />
        ///     is greater than the size of the source sequence, or less than <paramref name="minInstances" /> elements are found
        ///     to match the <paramref name="predicate" />, it will return false.
        /// </returns>
        public static Boolean AtLeast<T>( this IEnumerable<T> @this, UInt64 minInstances, Func<T, Boolean> predicate ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "AtLeast called on a null IEnumerable<>." );
            }
            if ( predicate == null ) {
                throw new ArgumentNullException( nameof( predicate ) );
            }

            if ( minInstances == 0 ) {
                return true;
            }

            UInt64 numInstSoFar = 0;
            return @this.Any( element => predicate( element ) && ++numInstSoFar == minInstances );
        }

        /// <summary>
        ///     Ascertains whether there are no more than <paramref name="maxInstances" /> elements in the source sequence
        ///     that satisfy the given <paramref name="predicate" />.
        /// </summary>
        /// <param name="this">The extended IEnumerable{T}.</param>
        /// <param name="maxInstances">The maximum number of elements that can satisfy the <paramref name="predicate" />.</param>
        /// <param name="predicate">The function that determines whether or not an element is counted.</param>
        /// <returns>
        ///     This method will immediately return false upon finding the (<paramref name="maxInstances" /> + 1)th element
        ///     that satisfies the predicate. Otherwise, if <paramref name="maxInstances" />
        ///     is greater than the size of the source sequence, or less than <paramref name="maxInstances" /> elements are found
        ///     to match the <paramref name="predicate" />, it will return true.
        /// </returns>
        public static Boolean AtMost<T>( this IEnumerable<T> @this, UInt64 maxInstances, Func<T, Boolean> predicate ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "AtMost called on a null IEnumerable<>." );
            }
            if ( predicate == null ) {
                throw new ArgumentNullException( nameof( predicate ) );
            }

            UInt64 numInstSoFar = 0;
            return @this.All( element => !predicate( element ) || ++numInstSoFar <= maxInstances );
        }

        public static Int32 Clear<T>( [NotNull] this IProducerConsumerCollection<T> collection ) => collection.RemoveAll();

        public static void Clear<T>( [NotNull] this ConcurrentBag<T> bag ) {
            if ( bag == null ) {
                throw new ArgumentNullException( nameof( bag ) );
            }
            do {
				bag.TryTake( out var result );
			} while ( !bag.IsEmpty );
        }

        public static Byte[] CloneByteArray( this Byte[] bytes ) {
            if ( bytes == null ) {
                return null;
            }
            var copy = new Byte[ bytes.Length ];
            Array.Copy( bytes, copy, bytes.Length );
            return copy;
        }

        public static Byte[] ClonePortion( [NotNull] this Byte[] bytes, Int32 offset, Int32 length ) {
            if ( bytes == null ) {
                throw new ArgumentNullException( nameof( bytes ) );
            }
            if ( offset < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
            var copy = new Byte[ length ];
            Array.Copy( bytes, offset, copy, 0, length );
            return copy;
        }

        public static Byte[] ConcatenateByteArrays( params Byte[][] bytearrays ) {
            var totalLength = bytearrays.Sum( t => t.Length );
            var idx = 0;
            var rv = new Byte[ totalLength ];
            foreach ( var t in bytearrays ) {
                Array.Copy( t, 0, rv, idx, t.Length );
                idx += t.Length;
            }
            return rv;
        }

        /// <summary>
        ///     Checks if two IEnumerables contain the exact same elements and same number of elements.
        ///     Order does not matter.
        /// </summary>
        /// <typeparam name="T">The Type of object.</typeparam>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection.</param>
        /// <returns>
        ///     True if both IEnumerables contain the same items, and same number of items; otherwise, false.
        /// </returns>
        public static Boolean ContainSameElements<T>( this IEnumerable<T> a, IEnumerable<T> b ) {
            var aa = a.ToList();
            aa.Fix();

            var bb = b.ToList();
            bb.Fix();

            if ( aa.Count != bb.Count ) {
                return false;
            }

            if ( aa.Any( item => !bb.Remove( item ) ) ) {
                return false;
            }

            Debug.Assert( !bb.Any() );

            return true;
        }

        public static BigInteger CountBig<TType>( [NotNull] this IEnumerable<TType> items ) {
            if ( items == null ) {
                throw new ArgumentNullException( nameof( items ) );
            }
            return items.Aggregate( seed: BigInteger.Zero, func: ( current, item ) => current + BigInteger.One );
        }

        /// <summary>
        ///     Counts the number of times each element appears in a collection, and returns a
        ///     <see cref="IDictionary{T, V}">dictionary</see>; where each key is an element and its value is the number of times
        ///     that element appeared in the source collection.
        /// </summary>
        /// <param name="this">The extended IEnumerable{T}.</param>
        /// <returns>A dictionary of elements mapped to the number of times they appeared in <paramref name="this" />.</returns>
        public static IDictionary<T, Int32> CountInstances<T>( this IEnumerable<T> @this ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "CountInstances called on a null IEnumerable<T>." );
            }

            IDictionary<T, Int32> result = new Dictionary<T, Int32>();

            foreach ( var element in @this.Where( t => t != null ) ) {
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
        ///     <paramref name="relationshipFunc" />.
        /// </summary>
        /// <param name="this">The extended IEnumerable{T}.</param>
        /// <param name="relationshipFunc">
        ///     The function that determines whether the given relationship exists
        ///     between two elements.
        /// </param>
        /// <returns>The number of pairs found.</returns>
        public static Int32 CountRelationship<T>( this IEnumerable<T> @this, Func<T, T, Boolean> relationshipFunc ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "CountRelationship called on a null IEnumerable<T>." );
            }

            // ReSharper disable once PossibleMultipleEnumeration
	        var enumerable = @this as T[] ?? @this.ToArray();
	        return enumerable.Select( ( a, aIndex ) => {
                                     // ReSharper disable once PossibleMultipleEnumeration
                                     return enumerable.Skip( aIndex + 1 ).Any( b => ( relationshipFunc?.Invoke( a, b ) ?? default ) || ( relationshipFunc?.Invoke( b, a ) ?? default ) );
                                 } ).Count( value => value );
        }

		/// <summary>
		///     Returns duplicate items found in the <see cref="sequence" /> .
		/// </summary>
		/// <param name="sequence">todo: describe sequence parameter on Duplicates</param>
		public static HashSet<T> Duplicates<T>( this IEnumerable<T> sequence ) {
            if ( null == sequence ) {
                throw new ArgumentNullException( nameof( sequence ) );
            }
            var set = new HashSet<T>();
            return new HashSet<T>( sequence.Where( item => !set.Add( item: item ) ) );
        }

        public static IEnumerable<T> Empty<T>() {
            yield break;
        }

        public static IEnumerable<T> EnumerableFromArray<T>( [NotNull] IEnumerable<T> array ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            return array;
        }

        /// <summary>
        ///     Returns the first two items to in the source collection that satisfy the given <paramref name="relationshipFunc" />
        ///     ,
        ///     or <c>null</c> if no match was found.
        /// </summary>
        /// <param name="this">The extended IEnumerable{T}.</param>
        /// <param name="relationshipFunc">
        ///     The function that determines whether the given relationship exists
        ///     between two elements.
        /// </param>
        /// <returns>
        ///     A tuple of the first two elements that match the given relationship, or <c>null</c> if
        ///     no such relationship exists.
        /// </returns>
        public static KeyValuePair<T, T>? FirstRelationship<T>( this IEnumerable<T> @this, Func<T, T, Boolean> relationshipFunc ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "FirstRelationship called on a null IEnumerable<T>." );
            }
            if ( relationshipFunc == null ) {
                throw new ArgumentNullException( nameof( relationshipFunc ) );
            }

            var aIndex = 0;
            // ReSharper disable once PossibleMultipleEnumeration
            foreach ( var a in @this ) {
                // ReSharper disable once PossibleMultipleEnumeration
                foreach ( var b in @this.Skip( ++aIndex ).Where( b => relationshipFunc( a, b ) || relationshipFunc( b, a ) ) ) {
                    return new KeyValuePair<T, T>( a, b );
                }
            }

            return null;
        }

        /// <summary>
        ///     The <seealso cref="List{T}.Capacity" /> is resized down to the <seealso cref="List{T}.Count" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void Fix<T>( [NotNull] this List<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            collection.Capacity = collection.Count;
        }

        public static void ForEach<T>( this IEnumerable<T> items, Action<T> action ) {
            if ( null == items ) {
                throw new ArgumentNullException( nameof( items ) );
            }
            if ( null == action ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            foreach ( var item in items ) {
	            //action.Invoke( item );	//is either way better?
                action( item );
            }
        }

        public static IEnumerable<T> SideEffects<T>( [ NotNull ] this IEnumerable<T> items, Action<T> perfomAction ) {
	        if ( items == null ) {
		        throw new ArgumentNullException( nameof( items ) );
	        }

	        foreach ( var item in items ) {
				perfomAction?.Invoke( item );
				yield return item;
            }
        }
        /// <summary>
        ///     http://blogs.msdn.com/b/pfxteam/archive/2012/02/04/10264111.aspx
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="generator"></param>
        /// <param name="added"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>( this ConcurrentDictionary<TKey, TValue> dict, TKey key, [ NotNull ] Func<TKey, TValue> generator, out Boolean added ) {
	        if ( generator == null ) {
		        throw new ArgumentNullException( nameof( generator ) );
	        }
	        while ( true ) {
				if ( dict.TryGetValue( key, out var value ) ) {
					added = false;
					return value;
				}

				value = generator( key );
                if ( !dict.TryAdd( key, value ) ) {
                    continue;
                }
                added = true;
                return value;
            }
        }

	    public static Boolean Has<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type == null ) {
                throw new ArgumentNullException( nameof( type ) );
            }
            return ( ( Int32 )( ValueType )type & ( Int32 )( ValueType )value ) == ( Int32 )( ValueType )value;
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

        public static Int32 IndexOf<T>( this T[] @this, T item ) => Array.IndexOf( @this, item );

	    /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/a/3562370/956364" />
        public static Int32 IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence ) {
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
        public static Int32 IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence, [NotNull] IEqualityComparer<T> comparer ) {
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
		public static IEnumerable<Boolean> Infinitely( this Boolean value ) {
            do {
                yield return value;
            } while ( true );

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        ///     Checks if an IEnumerable is empty.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The IEnumerable to check if empty.</param>
        /// <returns>True if the <paramref name="source" /> is null or empty; otherwise false.</returns>
        public static Boolean IsEmpty<T>( this IEnumerable<T> source ) => null == source || !source.Any();

	    public static UInt64 LongSum( this IEnumerable<Int32> collection ) => collection.Aggregate( 0UL, ( current, u ) => current + ( UInt64 )u );

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

        public static IEnumerable<T> OrderBy<T>( this IEnumerable<T> list, IEnumerable<T> guide ) {
            var toBeSorted = new HashSet<T>( list );
            return guide.Where( member => toBeSorted.Contains( member ) );
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>( [NotNull] this IEnumerable<T> source, Int32 size ) {
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
            if ( array == null ) {
                yield break;
            }
            Array.Resize( ref array, count );
            yield return new ReadOnlyCollection<T>( array );
        }

        /// <summary>
        ///     untested
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="this"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue Pop<TKey, TValue>( this IDictionary<TKey, TValue> @this, TKey key ) {
            var result = @this[ key ];
            @this.Remove( key );
            return result;
        }

        /// <summary>
        ///     untested
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T PopFirst<T>( this ICollection<T> @this ) {
            var result = @this.First();
            @this.Remove( result );
            return result;
        }

        /// <summary>
        ///     untested
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T PopLast<T>( this ICollection<T> @this ) {
            var result = @this.Last();
            @this.Remove( result );
            return result;
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

        public static IEnumerable<TU> Rank<T, TKey, TU>( [NotNull] this IEnumerable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, Int32, TU> selector ) {

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
			return collection.TryTake( out var result ) ? result : default;
		}

        public static T Remove<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type == null ) {
                throw new ArgumentNullException( nameof( type ) );
            }
            return ( T )( ValueType )( ( Int32 )( ValueType )type & ~( Int32 )( ValueType )value );
        }

        /// <summary>
        ///     Removes the <paramref name="specificItem" /> from the <paramref name="collection" /> and
        ///     returns how many <paramref name="specificItem" /> or null were removed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="specificItem"></param>
        /// <returns></returns>
        [Obsolete]
        public static Int32 Remove<T>( this IProducerConsumerCollection<T> collection, T specificItem ) {
            if ( null == collection ) {
                throw new ArgumentNullException( nameof( collection ) );
            }
            if ( Equals( specificItem, null ) ) {
                throw new ArgumentNullException( nameof( specificItem ) );
            }

            var removed = 0;

            while ( collection.Contains( specificItem ) ) {
				if ( !collection.TryTake( out var temp ) ) {
					continue;
				}
				if ( Equals( temp, default ) || Equals( temp, specificItem ) ) {
                    removed += 1;
                    continue;
                }

                collection.TryAdd( temp );
            }

            return removed;
        }

        public static Int32 RemoveAll<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }
            var removed = 0;
            while ( collection.Any() ) {
				while ( collection.TryTake( out var result ) ) {
					removed++;
				}
			}
            return removed;
        }

        public static IEnumerable<T> RemoveEach<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }
			while ( collection.TryTake( out var result ) ) {
				yield return result;
			}
		}

        /// <summary>
        ///     <para>Shuffle an array[] in <paramref name="iterations" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="iterations"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        [Obsolete( "broken at the moment. seealso Shuffle<List>" )]
        public static void Shuffle<T>( [NotNull] this T[] array, Int32 iterations = 1 ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            if ( iterations < 1 ) {
                iterations = 1;
            }

            if ( array.Length < 1 ) {
                return; //nothing to shuffle
            }

            while ( iterations > 0 ) {
                iterations--;

                // make a copy of all items
                var bag = new ConcurrentBag<T>( array );
                bag.Should().NotBeEmpty();
                var originalcount = bag.Count;

                var sqrt = ( Int32 )Math.Sqrt( originalcount );
                if ( sqrt <= 1 ) {
                    sqrt = 1;
                }

                // make some buckets.
                var buckets = new List<ConcurrentBag<T>>( capacity: sqrt );
                buckets.AddRange( 1.To( sqrt ).Select( i => new ConcurrentBag<T>() ) );

                // pull the items out of the bag, and put them into a random bucket each
                T item;
                while ( bag.TryTake( out item ) ) {
                    var index = 0.Next( sqrt );
                    buckets[ index ].Add( item );
                }
                bag.Should().BeEmpty( "All items should have been taken out of the bag" );

                while ( bag.Count < originalcount ) {
                    var index = 0.Next( maxValue: buckets.Count );
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
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="iterations"></param>
        /// <param name="shufflingType"></param>
        /// <param name="forHowLong"></param>
        /// <param name="orUntilCancelled"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        public static void Shuffle<T>( [NotNull] this List<T> list, Int32 iterations = 1, ShufflingType shufflingType = ShufflingType.AutoChoice, TimeSpan? forHowLong = null, SimpleCancel orUntilCancelled = null ) {
            if ( list == null ) {
                throw new ArgumentNullException( nameof( list ) );
            }
            try {
                if ( !list.Any() ) {
                    return; //nothing to shuffle
                }

                if ( iterations < 1 ) {
                    iterations = 1;
                }

                switch ( shufflingType ) {
                    case ShufflingType.ByGuid: {
                            ShuffleByGuid( ref list, iterations );
                        }
                        break;

                    case ShufflingType.ByRandom: {
                            ShuffleByRandomThenByRandom( ref list, iterations );
                        }
                        break;

                    case ShufflingType.ByHarker: {
                            ShuffleByHarker( ref list, iterations, forHowLong, orUntilCancelled );
                        }
                        break;

                    case ShufflingType.ByBags: {
                            ShuffleByBags( ref list, iterations, list.LongCount() );
                        }
                        break;

                    case ShufflingType.AutoChoice: {
                            ShuffleByHarker( ref list, iterations, forHowLong, orUntilCancelled );
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
        ///     Untested for speed and cpu/threading impact. Also, a lot of elements will/could NOT be
        ///     shuffled much.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="iterations"></param>
        /// <param name="originalcount"></param>
        public static void ShuffleByBags<T>( ref List<T> list, Int32 iterations, Int64 originalcount ) {
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

        public static void ShuffleByGuid<T>( ref List<T> list, Int32 iterations = 1 ) {
            while ( iterations.Any() ) {
                iterations--;
                var copy = list.AsParallel().OrderBy( arg => Guid.NewGuid() ).ToList();
                list.Clear();
                list.AddRange( copy.AsParallel() );
            }
        }

        /// <summary>
        ///     Fast shuffle. Not guaranteed or tested to be the fastest, but it *should* shuffle *well enough* in reasonable time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="iterations"></param>
        /// <param name="forHowLong"></param>
        /// <param name="orUntilCancelled"></param>
        /// <remarks>
        ///     The while.Any() could be replaced with a for loop.. the count is well known ahead of time.
        /// </remarks>
        public static void ShuffleByHarker<T>( ref List<T> list, Int32 iterations = 1, TimeSpan? forHowLong = null, SimpleCancel orUntilCancelled = null ) {

            var itemCount = list.Count;
            var array = list.ToArray();

            if ( orUntilCancelled != null ) {
                do {
                    var a = 0.Next( itemCount );
                    var b = 0.Next( itemCount );
                    array.Swap( a, b );
                } while ( !orUntilCancelled.HaveAnyCancellationsBeenRequested() );
                goto AllDone;
            }

            if ( forHowLong.HasValue ) {
                var whenStarted = StopWatch.StartNew();
                do {
                    var a = 0.Next( itemCount );
                    var b = 0.Next( itemCount );
                    array.Swap( a, b );
                } while ( whenStarted.Elapsed < forHowLong.Value );
                goto AllDone;
            }

            if ( iterations < 1 ) {
                iterations = 1;
            }

            if ( iterations.Any() ) {
                iterations *= itemCount;
                iterations *= iterations;
                do {
                    iterations--;
                    var a = 0.Next( itemCount );
                    var b = 0.Next( itemCount );
                    array.Swap( a, b );
                } while ( iterations.Any() );
            }

            AllDone:
            list = array.ToList();

            list.Count.Should().Be( itemCount );
        }

		public static void ShuffleByRandomThenByRandom<T>( ref List<T> list, Int32 iterations = 1 ) {
            while ( iterations.Any() ) {
                iterations--;
                var copy = list.AsParallel().OrderBy( o => Randem.Next() ).ThenBy( o => Randem.Next() ).ToList();
                list.Clear();
                list.AddRange( copy ); //TODO can we just return/replace 'list' with 'copy' instead of addrange?
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>( [NotNull] this IEnumerable<T> list, Int32 parts ) {
            if ( list == null ) {
                throw new ArgumentNullException( nameof( list ) );
            }
            var i = 0;
            var splits = from item in list group item by i++ % parts into part select part; //.AsEnumerable();
            return splits;
        }

        /// <summary>
        ///     Swap the two indexes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public static void Swap<T>( [NotNull] this T[] array, Int32 index1, Int32 index2 ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            var temp = array[ index1 ];
            array[ index1 ] = array[ index2 ];
            array[ index2 ] = temp;
        }

        /// <summary>
        ///     <para>
        ///         Remove and return the first item in the list, otherwise return null (or the default()
        ///         for value types).
        ///     </para>
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
                item = default;
                return false;
            }
            item = list[ 0 ];
            list.RemoveAt( 0 );
            return true;
        }

        /// <summary>
        ///     <para>Remove and return the first item in the list, otherwise return null.</para>
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
                return default;
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
                item = default;
                return false;
            }
            item = list[ index ];
            list.RemoveAt( index );
            return true;
        }

        /// <summary>
        ///     <para>Remove and return the last item in the list, otherwise return null.</para>
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
        ///     Optimally create a list from the <paramref name="source" />.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static List<TSource> ToList<TSource>( this IEnumerable<TSource> source, Int32 capacity ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( capacity < 0 ) {
                throw new ArgumentOutOfRangeException( nameof( capacity ) );
            }
            var list = new List<TSource>( capacity: capacity );
            list.AddRange( source );
            return list;
        }

        /// <summary>
        ///     Do a Take() on the top X percent
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Top<TSource>( [NotNull] this IEnumerable<TSource> source, Double x ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            var sources = source as IList<TSource> ?? source.ToList();
            return sources.Take( ( Int32 )( x * sources.Count ) );
        }

        public static List<T> ToSortedList<T>( this IEnumerable<T> values ) {
            var list = new List<T>( values );
            list.Sort();
            return list;
        }

        /*
                public static T Append<T>( [NotNull] this Enum type, T value ) where T : struct {
                    if ( type == null ) {
                        throw new ArgumentNullException( "type" );
                    }
                    return ( T )( ValueType )( ( ( int )( ValueType )type | ( int )( ValueType )value ) );
                }
        */

        public static String ToStrings( this IEnumerable<Object> enumerable, Char c ) => ToStrings( enumerable, new String( new[] { c } ) );

	    /// <summary>
        ///     <para>
        ///         Returns a String with the <paramref name="separator" /> between each item of an <paramref name="enumerable" />.
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
				if ( list.TakeFirst( out var item ) ) {
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
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="item"></param>
        /// <returns></returns>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="item"></param>
        /// <returns></returns>
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
			dictionary.TryRemove( key, out var dummy ); //HACK
			dictionary.TryAdd( key, value );

            //var wtf = default( TValue );
            //dictionary.TryUpdate( key, value, wtf );  //BUG I don't understand the whole if-same-then-replace-semantics. If we're going to replace the value, then why do we care what the current value is anyway?
        }

        /// <summary>
        ///     Returns all combinations of items in the source collection that satisfy the given
        ///     <paramref name="relationshipFunc" />.
        /// </summary>
        /// <param name="this">The extended IEnumerable{T}.</param>
        /// <param name="relationshipFunc">
        ///     The function that determines whether the given relationship exists
        ///     between two elements.
        /// </param>
        /// <returns>
        ///     An enumeration of all combinations of items that satisfy the <paramref name="relationshipFunc" />.
        ///     Each combination will only be returned once (e.g. <c>[a, b]</c> but not <c>[b, a]</c>).
        /// </returns>
        public static IEnumerable<KeyValuePair<T, T>> WhereRelationship<T>( this IEnumerable<T> @this, [ NotNull ] Func<T, T, Boolean> relationshipFunc ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "WhereRelationship called on a null IEnumerable<T>." );
            }
	        if ( relationshipFunc == null ) {
		        throw new ArgumentNullException( nameof( relationshipFunc ) );
	        }

	        var aIndex = 0;
            // ReSharper disable once PossibleMultipleEnumeration
            foreach ( var a in @this ) {
                // ReSharper disable once PossibleMultipleEnumeration
                foreach ( var b in @this.Skip( ++aIndex ).Where( b => relationshipFunc( a, b ) || relationshipFunc( b, a ) ) ) {
                    yield return new KeyValuePair<T, T>( a, b );
                }
            }
        }

        /// <summary>
        /// Untested.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>( this IEnumerable<T> list )
        {
            var dt = new DataTable();

            if ( list != null ) {

                PropertyInfo[] columns = null;

                foreach ( var record in list ) {
                    if ( columns == null ) {
                        columns = record.GetType().GetProperties();
                        foreach ( var GetProperty in columns ) {
                            var IcolType = GetProperty.PropertyType;

                            if ( IcolType.IsGenericType && IcolType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) {
                                IcolType = IcolType.GetGenericArguments()[ 0 ];
                            }

                            dt.Columns.Add( new DataColumn( GetProperty.Name, IcolType ) );
                        }
                    }

                    var dr = dt.NewRow();
                    foreach ( var p in columns ) {
                        if ( p.GetValue( record, null ) == null ) {
                            dr[ p.Name ] = DBNull.Value;
                        }
                        else {
                            dr[ p.Name ] = p.GetValue( record, null );
                        }
                    }
                    dt.Rows.Add( dr );
                }
            }

            return dt;
        }
    }
}