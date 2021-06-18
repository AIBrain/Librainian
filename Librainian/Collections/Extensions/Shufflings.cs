// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

namespace Librainian.Collections.Extensions {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Exceptions;
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
                throw new ArgumentEmptyException( nameof( array ) );
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

		/// <summary>
		///     Not a true random. Just enough to supposedly throw the <paramref name="sequence" /> out of strict order.
		/// </summary>
		/// <param name="sequence"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> AsRandom<T>( this IEnumerable<T> sequence ) =>
			sequence.AsParallel().AsUnordered().WithDegreeOfParallelism( Environment.ProcessorCount - 1 ).WithExecutionMode( ParallelExecutionMode.ForceParallelism )
				.WithMergeOptions( ParallelMergeOptions.AutoBuffered );

		/// <summary>Take a buffer and scramble.</summary>
		/// <param name="buffer"></param>
		/// <remarks>Fisher-Yates shuffle</remarks>
		public static void Shuffle<T>( this T[] buffer ) {
			var length = buffer.Length;

			for ( var i = length - 1; i >= 0; i-- ) {
				retry:
				var indexa = 0.Next( length );
				var indexb = 0.Next( length );

				if ( indexa == indexb ) {
					goto retry;
				}

				(var a, var b) = (buffer[ indexa ], buffer[ indexb ]);
				buffer[ indexa ] = b;
				buffer[ indexb ] = a;
			}
		}

		/// <summary>Take a list and scramble the order of its items.</summary>
		/// <param name="list"></param>
		/// <remarks>Fisher-Yates shuffle</remarks>
		public static void Shuffle<T>( this IList<T> list ) {
			var length = list.Count;

			for ( var i = length - 1; i >= 0; i-- ) {
				retry:
				var indexa = 0.Next( length );
				var indexb = 0.Next( length );

				if ( indexa == indexb ) {
					goto retry;
				}

				(var a, var b) = (list[ indexa ], list[ indexb ]);
				list[ indexa ] = b;
				list[ indexb ] = a;
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
		public static void Shuffle<T>( this List<T> list, UInt32 iterations = 1, ShufflingType shufflingType = ShufflingType.BestChoice, TimeSpan? forHowLong = null,
			CancellationToken? token = null ) {
			if ( list is null ) {
				throw new ArgumentEmptyException( nameof( list ) );
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
						ShuffleByGuid( ref list, iterations );

						break;
					}

					case ShufflingType.ByRandom: {
						ShuffleByRandomThenByRandom( ref list, iterations );

						break;
					}

					case ShufflingType.ByHarker: {
						ShuffleByHarker( list, iterations, forHowLong, token );

						break;
					}

					case ShufflingType.ByBags: {
						ShuffleByBags( ref list, iterations );

						break;
					}

					case ShufflingType.BestChoice: {
						ShuffleByHarker( list, iterations, forHowLong, token );

						break;
					}

					default:
						throw new ArgumentOutOfRangeException( nameof( shufflingType ) );
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
		public static void ShuffleByBags<T>( ref List<T> list, UInt32 iterations ) {
			if ( list is null ) {
				throw new ArgumentEmptyException( nameof( list ) );
			}

			if ( iterations < 1 ) {
				return;
			}

			var bag = new ConcurrentBag<T>( list.AsParallel().AsUnordered().WithDegreeOfParallelism( Environment.ProcessorCount - 1 )
				.WithExecutionMode( ParallelExecutionMode.ForceParallelism ).WithMergeOptions( ParallelMergeOptions.AutoBuffered ) );

			while ( iterations.Any() ) {
				iterations--;

				list.Clear();
				list.AddRange( bag.AsRandom() );

				if ( iterations.Any() ) {
					bag = new ConcurrentBag<T>( list.AsRandom() );
				}
			}
		}

		public static void ShuffleByGuid<T>( ref List<T> list, UInt32 iterations = 1 ) {
			if ( list is null ) {
				throw new ArgumentEmptyException( nameof( list ) );
			}

			while ( iterations.Any() ) {
				iterations--;
				var temp = new List<T>( list.AsRandom().OrderBy( _ => Guid.NewGuid() ).AsRandom() );
				list.Clear();
				list.AddRange( temp.AsRandom().OrderBy( _ => Guid.NewGuid() ).AsRandom() );
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

		/// <summary>
		///     Not cryptographically guaranteed or tested to be the most performant, but it *should* shuffle *well enough* in
		///     reasonable time.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list to be shuffled.</param>
		/// <param name="iterations">At least 1 iterations to be done over the whole list.</param>
		/// <param name="forHowLong">Or for how long to run.</param>
		/// <param name="token">Or until cancelled.</param>
		public static void ShuffleByHarker<T>( IList<T> list, UInt32 iterations = 1, TimeSpan? forHowLong = null, CancellationToken? token = null ) {
			if ( list is null ) {
				throw new ArgumentEmptyException( nameof( list ) );
			}

			Stopwatch? started = null;

			if ( forHowLong.HasValue ) {
				started = Stopwatch.StartNew(); //don't allocate/start a stopwatch unless we're waiting for time to pass.
			}

			token ??= CancellationToken.None;

			do {
				list.Shuffle();

				if ( token.Value.IsCancellationRequested ) {
					return;
				}

				if ( forHowLong.HasValue ) {
					if ( started != null && started.Elapsed > forHowLong.Value ) {
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
		public static void ShuffleByRandomThenByRandom<T>( ref List<T> list, UInt32 iterations = 1 ) {
			if ( list == null ) {
				throw new ArgumentEmptyException( nameof( list ) );
			}

			while ( iterations.Any() ) {
				iterations--;
				list = list.OrderBy( o => Randem.Next() ).ThenBy( o => Randem.Next() ).ToList();
			}
		}
	}
}