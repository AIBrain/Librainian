// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "CollectionExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "CollectionExtensions.cs" was last formatted by Protiguous on 2020/03/18 at 10:22 AM.

#nullable enable
namespace Librainian.Collections.Extensions {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using Exceptions;
    using JetBrains.Annotations;
    using JM.LinqFaster.SIMD;
    using Threading;

    public static class CollectionExtensions {

        public static void Add<T>( [NotNull] this IProducerConsumerCollection<T> collection, [CanBeNull] T item ) {
            if ( null == collection ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            collection.TryAdd( item );
        }

        public static void AddRange<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] IEnumerable<T> items ) {
            if ( collection is null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            Parallel.ForEach( items.AsParallel(), CPU.AllCPUExceptOne, collection.Add );
        }

        /// <summary>Determines whether or not the given sequence contains any duplicate elements.</summary>
        /// <param name="self">The extended <see cref="IEnumerable{T}" />.</param>
        /// <returns>True if the sequence contains duplicate elements, false if not.</returns>
        [Pure]
        public static Boolean AnyDuplicates<T>( [NotNull] this IEnumerable<T> self ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "AnyDuplicates<T> called on a null IEnumerable<T>." );
            }

            return AnyRelationship( self, ( arg1, arg2 ) => Equals( arg1, arg2 ) );
        }

        /// <summary>Determines whether or not a given relationship exists between any two elements in the sequence.</summary>
        /// <param name="self">            The extended <see cref="IEnumerable{T}" />.</param>
        /// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
        /// <returns>True if the relationship exists between any two elements, false if not.</returns>
        [Pure]
        public static Boolean AnyRelationship<T>( [NotNull] this IEnumerable<T> self, [CanBeNull] Func<T, T, Boolean> relationshipFunc ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "AnyRelationship called on a null IEnumerable<T>." );
            }

            var enumerable = self as T[] ?? self.ToArray();

            return enumerable.Select( ( a, aIndex ) => enumerable.Skip( aIndex + 1 ).Any( b =>
                                 ( relationshipFunc?.Invoke( a, b ) ?? default ) || ( relationshipFunc?.Invoke( b, a ) ?? default ) ) )
                             .Any( value => value );
        }

        /// <summary>Returns whether or not there are at least <paramref name="minInstances" /> elements in the source sequence that satisfy the given <paramref name="predicate" />.</summary>
        /// <param name="self">        The extended IEnumerable{T}.</param>
        /// <param name="minInstances">The number of elements that must satisfy the <paramref name="predicate" />.</param>
        /// <param name="predicate">   The function that determines whether or not an element is counted.</param>
        /// <returns>
        /// This method will immediately return true upon finding the <paramref name="minInstances" /> th element that satisfies the predicate, or if <paramref name="minInstances" />
        /// is 0. Otherwise, if <paramref name="minInstances" /> is greater than the size of the source sequence, or less than <paramref name="minInstances" /> elements are found to match the
        /// <paramref name="predicate" />, it will return false.
        /// </returns>
        [Pure]
        public static Boolean AtLeast<T>( [NotNull] this IEnumerable<T> self, UInt64 minInstances, [NotNull] Func<T, Boolean> predicate ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "AtLeast called on a null IEnumerable<>." );
            }

            if ( predicate is null ) {
                throw new ArgumentNullException( nameof( predicate ) );
            }

            if ( minInstances == 0 ) {
                return true;
            }

            UInt64 numInstSoFar = 0;

            return self.Any( element => predicate( element ) && ( ++numInstSoFar == minInstances ) );
        }

        /// <summary>Ascertains whether there are no more than <paramref name="maxInstances" /> elements in the source sequence that satisfy the given <paramref name="predicate" />.</summary>
        /// <param name="self">        The extended IEnumerable{T}.</param>
        /// <param name="maxInstances">The maximum number of elements that can satisfy the <paramref name="predicate" />.</param>
        /// <param name="predicate">   The function that determines whether or not an element is counted.</param>
        /// <returns>
        /// This method will immediately return false upon finding the ( <paramref name="maxInstances" /> + 1)th element that satisfies the predicate. Otherwise, if
        /// <paramref name="maxInstances" /> is greater than the size of the source sequence, or less than <paramref name="maxInstances" /> elements are found to match the
        /// <paramref name="predicate" />, it will return true.
        /// </returns>
        [Pure]
        public static Boolean AtMost<T>( [NotNull] this IEnumerable<T> self, UInt64 maxInstances, [NotNull] Func<T, Boolean> predicate ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "AtMost called on a null IEnumerable<>." );
            }

            if ( predicate is null ) {
                throw new ArgumentNullException( nameof( predicate ) );
            }

            UInt64 numInstSoFar = 0;

            return self.All( element => !predicate( element ) || ( ++numInstSoFar <= maxInstances ) );
        }

        public static Int32 Clear<T>( [NotNull] this IProducerConsumerCollection<T> collection ) => collection.RemoveAll();

        public static void Clear<T>( [NotNull] this ConcurrentBag<T> bag ) {
            if ( bag is null ) {
                throw new ArgumentNullException( nameof( bag ) );
            }

            while ( !bag.IsEmpty ) {
                bag.TryTake( out _ );
            }
        }

        [CanBeNull]
        [Pure]
        public static Byte[]? Clone( [CanBeNull] this Byte[] bytes ) {
            if ( bytes is null ) {
                return null;
            }

            var copy = new Byte[ bytes.Length ];
            Buffer.BlockCopy( bytes, 0, copy, 0, bytes.Length );

            return copy;
        }

        [NotNull]
        [Pure]
        public static Byte[] ClonePortion( [NotNull] this Byte[] bytes, Int32 offset, Int32 length ) {
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

        /// <summary>Concat multiple byte arrays into one new array. Warning: limited to Int32 byte arrays (2GB).</summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static Byte[] Concat( [NotNull] params Byte[][] arrays ) {
            Int64 totalLength = arrays.Where( data => data != null ).Sum( Buffer.ByteLength );

            if ( totalLength > Int32.MaxValue ) {
                throw new OutOfRangeException( $"The total size of the arrays ({totalLength:N0}) is too large." );
            }

            var ret = new Byte[ totalLength ];
            var offset = 0;

            foreach ( var data in arrays.Where( data => data != null ) ) {
                var length = Buffer.ByteLength( data );
                Buffer.BlockCopy( data, 0, ret, offset, length );
                offset += length;
            }

            return ret;
        }

        /// <summary>Checks if two IEnumerables contain the exact same elements and same number of elements. Order does not matter.</summary>
        /// <typeparam name="T">The Type of object.</typeparam>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection.</param>
        /// <returns>True if both IEnumerables contain the same items, and same number of items; otherwise, false.</returns>
        [Pure]
        public static Boolean ContainSameElements<T>( [NotNull] this IList<T> a, [NotNull] IList<T> b ) {
            if ( a is null ) {
                throw new ArgumentNullException( nameof( a ) );
            }

            if ( b is null ) {
                throw new ArgumentNullException( nameof( b ) );
            }

            if ( a.Count != b.Count ) {
                return default;
            }

            if ( a.Any( item => !b.Remove( item ) ) ) {
                return default;
            }

            Debug.Assert( !b.Any() );

            return !b.Any(); //is this correct? I need to sleep..
        }

        [Pure]
        public static BigInteger CountBig<TType>( [NotNull] this IEnumerable<TType> items ) {
            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            return items.Aggregate( BigInteger.Zero, ( current, item ) => current + BigInteger.One );
        }

        /// <summary>
        /// Counts the number of times each element appears in a collection, and returns a <see cref="IDictionary{T, V}">dictionary</see>; where each key is an element and its value
        /// is the number of times that element appeared in the source collection.
        /// </summary>
        /// <param name="self">The extended IEnumerable{T}.</param>
        /// <returns>A dictionary of elements mapped to the number of times they appeared in <paramref name="self" />.</returns>
        [NotNull]
        [Pure]
        public static IDictionary<T, Int32> CountInstances<T>( [NotNull] this IEnumerable<T> self ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "CountInstances called on a null IEnumerable<T>." );
            }

            IDictionary<T, Int32> result = new Dictionary<T, Int32>();

            foreach ( var element in self ) {
                if ( !( element is null ) ) {
                    if ( result.ContainsKey( element ) ) {
                        ++result[ element ];
                    }
                    else {
                        result[ element ] = 1;
                    }
                }
            }

            return result;
        }

        /// <summary>Counts how many pairs of elements in the source sequence share the relationship defined by <paramref name="relationshipFunc" />.</summary>
        /// <param name="self">            The extended IEnumerable{T}.</param>
        /// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
        /// <returns>The number of pairs found.</returns>
        [Pure]
        public static Int32 CountRelationship<T>( [NotNull] this IEnumerable<T> self, [CanBeNull] Func<T, T, Boolean> relationshipFunc ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "CountRelationship called on a null IEnumerable<T>." );
            }

            var enumerable = self as T[] ?? self.ToArray();

            return enumerable.Select( ( a, aIndex ) => enumerable.Skip( aIndex + 1 ).Any( b =>
                                 ( relationshipFunc?.Invoke( a, b ) ?? default ) || ( relationshipFunc?.Invoke( b, a ) ?? default ) ) )
                             .Count( value => value );
        }

        /// <summary>Returns duplicate items found in the <see cref="sequence" /> .</summary>
        /// <param name="sequence">todo: describe sequence parameter on Duplicates</param>
        [NotNull]
        [Pure]
        public static HashSet<T> Duplicates<T>( [NotNull] this IEnumerable<T> sequence ) {
            if ( null == sequence ) {
                throw new ArgumentNullException( nameof( sequence ) );
            }

            var set = new HashSet<T>();

            return new HashSet<T>( sequence.Where( item => !set.Add( item ) ) );
        }

        [Pure]
        public static IEnumerable<T> Empty<T>() {
            yield break;
        }

        [NotNull]
        [Pure]
        public static IEnumerable<T> EnumerableFromArray<T>( [NotNull] IEnumerable<T> array ) {
            if ( array is null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            return array;
        }

        [Pure]
        public static Boolean EqualLists<T>( [NotNull] this List<T> left, [NotNull] List<T> right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            if ( left.Count != right.Count ) {
                return default;
            }

            var dict = new Dictionary<T, Int64>();

            foreach ( var member in left ) {
                if ( !dict.ContainsKey( member ) ) {
                    dict[ member ] = 1;
                }
                else {
                    dict[ member ]++;
                }
            }

            foreach ( var member in right ) {
                if ( !dict.ContainsKey( member ) ) {
                    return default;
                }

                dict[ member ]--;
            }

            return dict.All( kvp => kvp.Value == 0 );
        }

        /// <summary>Returns the first two items to in the source collection that satisfy the given <paramref name="relationshipFunc" /> , or <c>null</c> if no match was found.</summary>
        /// <param name="self">            The extended IEnumerable{T}.</param>
        /// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
        /// <returns>A tuple of the first two elements that match the given relationship, or <c>null</c> if no such relationship exists.</returns>
        [Pure]
        public static KeyValuePair<T, T>? FirstRelationship<T>( [NotNull] this IEnumerable<T> self, [NotNull] Func<T, T, Boolean> relationshipFunc ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "FirstRelationship called on a null IEnumerable<T>." );
            }

            if ( relationshipFunc is null ) {
                throw new ArgumentNullException( nameof( relationshipFunc ) );
            }

            var aIndex = 0;

            var enumerable = self.ToList();

            foreach ( var a in enumerable ) {

                foreach ( var b in enumerable.Skip( ++aIndex ).Where( b => relationshipFunc( a, b ) || relationshipFunc( b, a ) ) ) {
                    return new KeyValuePair<T, T>( a, b );
                }
            }

            return null;
        }

        [ItemCanBeNull]
        [Pure]
        public static IEnumerable<T> ForEach<T>( [NotNull] this IEnumerable<T> items, [NotNull] Action<T> action ) {
            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            if ( action is null ) {
                throw new ArgumentNullException( nameof( action ) );
            }

            foreach ( var item in items ) {
                action( item );

                yield return item;
            }
        }

        /// <summary>http://blogs.msdn.com/b/pfxteam/archive/2012/02/04/10264111.aspx</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">     </param>
        /// <param name="key">      </param>
        /// <param name="generator"></param>
        /// <param name="added">    </param>
        /// <returns></returns>
        [Pure]
        public static TValue GetOrAdd<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dict, [NotNull] TKey key, [NotNull] Func<TKey, TValue> generator,
            out Boolean added ) {
            if ( generator is null ) {
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

        [Pure]
        public static Boolean Has<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type is null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            return ( ( Int32 ) ( ValueType ) type & ( Int32 ) ( ValueType ) value ) == ( Int32 ) ( ValueType ) value;
        }

        [Pure]
        public static Boolean HasDuplicates<T>( [NotNull] this IEnumerable<T> sequence ) {
            if ( sequence is null ) {
                throw new ArgumentNullException( nameof( sequence ) );
            }

            if ( Equals( sequence, null ) ) {
                throw new ArgumentNullException( nameof( sequence ) );
            }

            return sequence.Duplicates().Any();
        }

        [Pure]
        public static Boolean In<T>( [CanBeNull] this T value, [NotNull] params T[] items ) {
            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            return items.Contains( value );
        }

        [Pure]
        public static Int32 IndexOf<T>( [NotNull] this T[] self, [CanBeNull] T item ) => Array.IndexOf( self, item );

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">  </param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        /// <remarks>http://stackoverflow.com/a/3562370/956364</remarks>
        [Pure]
        public static Int32 IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence ) {
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
        public static Int32 IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence, [NotNull] IEqualityComparer<T> comparer ) {
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
                        return ( p - seq.Length ) + 1;
                    }
                }
                else // Mismatch
                {
                    // Do we have prospective matches to fall back to ?
                    if ( prospects.Count > 0 ) {

                        // Yes, use the first one
                        var k = prospects[ 0 ];
                        i = ( p - k ) + 1;
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
        public static Boolean IsEmpty<T>( [CanBeNull] this IEnumerable<T> source ) => source?.Any() != true;

        [Pure]
        public static Int64 LongSum( [NotNull] this IEnumerable<Int32> collection ) => collection.Aggregate( 0L, ( current, u ) => current + ( Int64 ) u );

        [CanBeNull]
        [Pure]
        public static LinkedListNode<TType> NextOrFirst<TType>( [NotNull] this LinkedListNode<TType> current ) {
            if ( current is null ) {
                throw new ArgumentNullException( nameof( current ) );
            }

            return current.Next ?? current.List.First;
        }

        [NotNull]
        [Pure]
        public static IEnumerable<T> OrderBy<T>( [NotNull] this IEnumerable<T> list, [NotNull] IEnumerable<T> guide ) {
            var toBeSorted = new HashSet<T>( list );

            return guide.Where( member => toBeSorted.Contains( member ) );
        }

        [ItemNotNull]
        [Pure]
        public static IEnumerable<IEnumerable<T>> Partition<T>( [NotNull] this IEnumerable<T> source, Int32 size ) {
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

            if ( array == null ) {
                yield break;
            }

            Array.Resize( ref array, count );

            yield return new ReadOnlyCollection<T>( array );
        }

        /// <summary>untested</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="self"></param>
        /// <param name="key"> </param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public static TValue Pop<TKey, TValue>( [NotNull] this IDictionary<TKey, TValue> self, [NotNull] TKey key ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            var result = self[ key ];
            self.Remove( key );

            return result;
        }

        /// <summary>untested</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public static T PopFirst<T>( [NotNull] this ICollection<T> self ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

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
        public static T PopLast<T>( [NotNull] this ICollection<T> self ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

            var result = self.Last();
            self.Remove( result );

            return result;
        }

        /// <summary></summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public static LinkedListNode<TType> PreviousOrLast<TType>( [NotNull] this LinkedListNode<TType> current ) {
            if ( current is null ) {
                throw new ArgumentNullException( nameof( current ) );
            }

            return current.Previous ?? current.List.Last;
        }

        [ItemCanBeNull]
        [Pure]
        public static IEnumerable<TU> Rank<T, TKey, TU>( [NotNull] this IEnumerable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, Int32, TU> selector ) {

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
                var current = keySelector( t ) ?? throw new NullReferenceException($"null key in function {nameof(keySelector)} in function {nameof(Rank)}.");

                if ( !current.Equals( previous ) ) {
                    rank = itemCount;
                }

                yield return selector( t, rank );
                previous = current;
            }
        }

        [Pure]
        public static T Remove<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type is null ) {
                throw new ArgumentNullException( nameof( type ) );
            }

            return ( T ) ( ValueType ) ( ( Int32 ) ( ValueType ) type & ~( Int32 ) ( ValueType ) value );
        }


        public static Int32 RemoveAll<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection is null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            var removed = 0;

            while ( collection.Any() ) {
                while ( collection.TryTake( out _ ) ) {
                    ++removed;
                }
            }

            return removed;
        }

        public static IEnumerable<T> RemoveEach<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection is null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }

            while ( collection.TryTake( out var result ) ) {
                yield return result;
            }
        }

        [ItemCanBeNull]
        public static IEnumerable<T> SideEffects<T>( [NotNull] this IEnumerable<T> items, [CanBeNull] Action<T> perfomAction ) {
            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            foreach ( var item in items ) {
                perfomAction?.Invoke( item );

                yield return item;
            }
        }

        [NotNull]
        [Pure]
        public static IEnumerable<IEnumerable<T>> Split<T>( [NotNull] this IEnumerable<T> list, Int32 parts ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            var i = 0;
            var splits = from item in list group item by ++i % parts into part select part;

            return splits;
        }

        /// <summary>Swap the two indexes</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> </param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public static void Swap<T>( [NotNull] this T[] array, Int32 index1, Int32 index2 ) {
            if ( array is null ) {
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
        public static Boolean TakeFirst<TType>( [NotNull] this IList<TType> list, [CanBeNull] out TType item ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            if ( list.Count <= 0 ) {
                item = default;

                return default;
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
        public static TType TakeFirst<TType>( [NotNull] this IList<TType> list ) {
            if ( list is null ) {
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
        public static Boolean TakeLast<TType>( [NotNull] this IList<TType> list, [CanBeNull] out TType item ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            var index = list.Count - 1;

            if ( index < 0 ) {
                item = default;

                return default;
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
        public static TType TakeLast<TType>( [NotNull] this IList<TType> list ) where TType : class {
            if ( list is null ) {
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

        /// <summary>Optimally create a list from the <paramref name="source" />.</summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">  </param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static List<TSource> ToList<TSource>( [NotNull] this IEnumerable<TSource> source, Int32 capacity ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }

            if ( capacity < 0 ) {
                throw new ArgumentOutOfRangeException( nameof( capacity ) );
            }

            var list = new List<TSource>( capacity );
            list.AddRange( source );

            return list;
        }

        /// <summary>Do a Take() on the top X percent</summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="x">     </param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static IEnumerable<TSource> Top<TSource>( [NotNull] this IEnumerable<TSource> source, Double x ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }

            var sources = source as IList<TSource> ?? source.ToList();

            return sources.Take( ( Int32 ) ( x * sources.Count ) );
        }

        [NotNull]
        [Pure]
        public static List<T> ToSortedList<T>( [NotNull] this IEnumerable<T> values ) {
            var list = new List<T>( values );
            list.Sort();

            return list;
        }

        [NotNull]
        [Pure]
        public static String ToStrings( [NotNull] this IEnumerable<Object> enumerable, Char c ) =>
            ToStrings( enumerable, new String( new[] {
                c
            } ) );

        /// <summary>
        ///     <para>Returns a String with the <paramref name="separator" /> between each item of an <paramref name="items" />.</para>
        ///     <para>If no separator is given, it defaults to ", ".</para>
        ///     <para>Additonally, <paramref name="atTheEnd" /> can optionally be added to the returned string.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="separator">Defaults to ", ".</param>
        /// <param name="atTheEnd">  </param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String ToStrings<T>( [NotNull] this IEnumerable<T> items, [CanBeNull] String? separator = ", ", [CanBeNull] String? atTheEnd = null ) {
            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            if ( String.IsNullOrEmpty( atTheEnd ) ) {
                return String.Join( separator, items ).TrimEnd();
            }

            return $"{String.Join( separator, items )}{separator}{atTheEnd}".TrimEnd();
        }

        /// <summary>Extension to aomtically remove a KVP.</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean TryRemove<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, [CanBeNull] TKey key, [CanBeNull] TValue value ) {
            if ( dictionary is null ) {
                throw new ArgumentNullException( nameof( dictionary ) );
            }

            return ( ( ICollection<KeyValuePair<TKey, TValue>> ) dictionary ).Remove( new KeyValuePair<TKey, TValue>( key, value ) );
        }

        /// <summary>Wrapper for <see cref="ConcurrentQueue{T}.TryDequeue" /></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="item"> </param>
        /// <returns></returns>
        [Pure]
        public static Boolean TryTake<T>( [NotNull] this ConcurrentQueue<T> queue, out T item ) {
            if ( queue is null ) {
                throw new ArgumentNullException( nameof( queue ) );
            }

            if ( Equals( queue, null ) ) {
                throw new ArgumentNullException( nameof( queue ) );
            }

            return queue.TryDequeue( out item );
        }

        /// <summary>Wrapper for <see cref="ConcurrentStack{T}.TryPop" /></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="item"> </param>
        /// <returns></returns>
        [Pure]
        public static Boolean TryTake<T>( [NotNull] this ConcurrentStack<T> stack, out T item ) {
            if ( null == stack ) {
                throw new ArgumentNullException( nameof( stack ) );
            }

            return stack.TryPop( out item );
        }

        [Pure]
        public static UInt64 ULongSum( [NotNull] this IEnumerable<Int32> collection ) => ( UInt64 ) ( collection as Int32[] ).SumS();

        /// <summary>why?</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Update<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [CanBeNull] TValue value ) {
            if ( dictionary is null ) {
                throw new ArgumentNullException( nameof( dictionary ) );
            }

            dictionary[ key ] = value;
        }

        /// <summary>Returns all combinations of items in the source collection that satisfy the given <paramref name="relationshipFunc" />.</summary>
        /// <param name="self">            The extended IEnumerable{T}.</param>
        /// <param name="relationshipFunc">The function that determines whether the given relationship exists between two elements.</param>
        /// <returns>
        /// An enumeration of all combinations of items that satisfy the <paramref name="relationshipFunc" />. Each combination will only be returned once (e.g. <c>[a, b]</c> but not
        /// <c>[b, a]</c>).
        /// </returns>
        [Pure]
        public static IEnumerable<KeyValuePair<T, T>> WhereRelationship<T>( [NotNull] this IEnumerable<T> self, [NotNull] Func<T, T, Boolean> relationshipFunc ) {
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

    }

}