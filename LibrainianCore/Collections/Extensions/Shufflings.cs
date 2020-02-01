// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Shufflings.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "Shufflings.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace LibrainianCore.Collections.Extensions {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using Logging;
    using Maths;

    public static class Shufflings {

        /*

        /// <summary>
        ///     <para>Shuffle an array[] in <paramref name="iterations" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">     </param>
        /// <param name="iterations"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        [Obsolete( "Broken and untested. Just an idea to learn with. Meant to work with large arrays. See also Shuffle<List>()" )]
        public static void Shuffle<T>( [NotNull] this T[] array, Int32 iterations = 1 ) {
            if ( array is null ) {
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

                //bag.Should().NotBeEmpty();
                var originalcount = bag.Count;

                var sqrt = ( Int32 )Math.Sqrt( d: originalcount );

                if ( sqrt <= 1 ) {
                    sqrt = 1;
                }

                // make some buckets.
                var buckets = new List<ConcurrentBag<T>>( capacity: sqrt );
                buckets.AddRange( collection: 1.To( end: sqrt ).Select( i => new ConcurrentBag<T>() ) );

                // pull the items out of the bag, and put them into a random bucket each
                T item;

                while ( bag.TryTake( result: out item ) ) {
                    var index = 0.Next( maxValue: sqrt );
                    buckets[ index: index ].Add( item: item );
                }

                //bag.Should().BeEmpty( because: "All items should have been taken out of the bag." );

                while ( bag.Count < originalcount ) {
                    var index = 0.Next( maxValue: buckets.Count );
                    var bucket = buckets[ index: index ];

                    if ( bucket.TryTake( result: out item ) ) {
                        bag.Add( item: item );
                    }

                    if ( bucket.IsEmpty ) {
                        buckets.Remove( item: bucket );
                    }
                }

                //bag.Count.Should().Be( expected: originalcount );

                // put them back into the array
                var newArray = bag.ToArray();
                newArray.CopyTo( array: array, index: 0 );
            }
        }
        */

        /// <summary>Take a buffer and scramble.</summary>
        /// <param name="buffer"></param>
        /// <remarks>Isn't this just a really good (Fisher-Yates) shuffle??</remarks>
        public static void Shuffle<T>( [NotNull] this T[] buffer ) {
            if ( buffer is null ) {
                throw new ArgumentNullException( nameof( buffer ) );
            }

            var length = buffer.Length;

            for ( var i = length - 1; i >= 0; i-- ) {
                var a = 0.Next( length );
                var b = 0.Next( length );
                var (v1, v2) = (buffer[ a ], buffer[ b ]);
                buffer[ a ] = v2;
                buffer[ b ] = v1;
            }
        }

        /// <summary>Take a list and scramble the order of its items.</summary>
        /// <param name="list"></param>
        /// <remarks>Isn't this just the Fisher-Yates shuffle??</remarks>
        public static void Shuffle<T>( [NotNull] this IList<T> list ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            var length = list.Count;

            for ( var i = length - 1; i >= 0; i-- ) {
                var a = 0.Next( length );
                var b = 0.Next( length );
                var (v1, v2) = (list[ a ], list[ b ]);
                list[ a ] = v2;
                list[ b ] = v1;
            }
        }

        /// <summary>
        ///     <para>Shuffle a list in <paramref name="iterations" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">            </param>
        /// <param name="iterations">      </param>
        /// <param name="shufflingType">   </param>
        /// <param name="forHowLong">      </param>
        /// <param name="token"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        public static void Shuffle<T>( [NotNull] this List<T> list, UInt32 iterations = 1, ShufflingType shufflingType = ShufflingType.BestChoice, TimeSpan? forHowLong = null,
            CancellationToken? token = null ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            try {
                if ( !list.Any() ) {
                    return; //nothing to shuffle
                }

                if ( !iterations.Any() ) {
                    iterations = 1;
                }

                switch ( shufflingType ) {
                    case ShufflingType.ByGuid: {
                            ShuffleByGuid( list: ref list, iterations: iterations );

                            break;
                        }

                    case ShufflingType.ByRandom: {
                            ShuffleByRandomThenByRandom( list: ref list, iterations: iterations );

                            break;
                        }

                    case ShufflingType.ByHarker: {
                            ShuffleByHarker( list: list, iterations: iterations, forHowLong: forHowLong, token: token );

                            break;
                        }

                    case ShufflingType.ByBags: {
                            ShuffleByBags( list: ref list, iterations: iterations );

                            break;
                        }

                    case ShufflingType.BestChoice: {
                            ShuffleByHarker( list: list, iterations: iterations, forHowLong: forHowLong, token: token );

                            break;
                        }

                    default: throw new ArgumentOutOfRangeException( nameof( shufflingType ) );
                }
            }
            catch ( IndexOutOfRangeException exception ) {
                exception.Log();
            }
        }

        /// <summary>Untested for speed and cpu/threading impact. Also, a lot of elements will/could NOT be shuffled much.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">         </param>
        /// <param name="iterations">   </param>
        public static void ShuffleByBags<T>( [NotNull] ref List<T> list, UInt32 iterations ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            var bag = new ConcurrentBag<T>( list.AsParallel() );

            if ( iterations <= 1 ) {
                list.Clear();
                list.AddRange( bag.AsParallel() );

                return;
            }

            while ( iterations.Any() ) {
                iterations--;

                list.Clear();
                list.AddRange( bag.AsParallel().AsUnordered() );

                if ( iterations.Any() ) {
                    bag.RemoveAll();
                }
            }
        }

        public static void ShuffleByGuid<T>( [NotNull] ref List<T> list, UInt32 iterations = 1 ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            var l = new List<T>( list.Count );

            while ( iterations.Any() ) {
                iterations--;
                l.Clear();
                l.AddRange( list.AsParallel().AsUnordered().OrderBy( arg => Guid.NewGuid() ).AsUnordered() );

                //TODO this is not finished
            }
        }

        /* ignore this. just some latenight mind-think stuff
		[NotNull]
		private static readonly Type[] EmptyTypeArray = new Type[0];

		[NotNull]
		[Pure]
		public static Func<X> InstanceCreator<X>() {
			var type = typeof( X );
			var constructor = type.GetConstructor( EmptyTypeArray );
			var @new = Expression.New( constructor ?? throw new InvalidOperationException() );
			var lambda = Expression.Lambda<Func<X>>( @new ) ;
			return lambda.Compile();
		}
		*/

        /// <summary>Not cryptographically guaranteed or tested to be the most performant, but it *should* shuffle *well enough* in reasonable time.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to be shuffled.</param>
        /// <param name="iterations">At least 1 iterations to be done over the whole list.</param>
        /// <param name="forHowLong">Or for how long to run.</param>
        /// <param name="token">Or until cancelled.</param>
        public static void ShuffleByHarker<T>( [NotNull] IList<T> list, UInt32 iterations = 1, TimeSpan? forHowLong = null, CancellationToken? token = null ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            Stopwatch started = null;

            if ( forHowLong.HasValue ) {
                started = Stopwatch.StartNew(); //don't allocate/start a stopwatch unless we're waiting for time to pass.
            }

            if ( !token.HasValue ) {
                token = CancellationToken.None;
            }

            do {
                list.Shuffle();

                if ( token.Value.IsCancellationRequested ) {
                    return;
                }

                if ( forHowLong.HasValue ) {
                    if ( started.Elapsed > forHowLong.Value ) {
                        return;
                    }

                    iterations++; //we're waiting for time. increment the counter.
                }
            } while ( ( --iterations ).Any() );
        }

        /// <summary>Shuffle the whole list using OrderBy and ThenBy.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="iterations"></param>
        public static void ShuffleByRandomThenByRandom<T>( [NotNull] ref List<T> list, UInt32 iterations = 1 ) {
            if ( list == null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            while ( iterations.Any() ) {
                iterations--;
                list = list.OrderBy(  o => Randem.Next() ).ThenBy(  o => Randem.Next() ).ToList();
            }
        }
    }
}