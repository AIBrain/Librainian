// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "Combiner.cs" last formatted on 2020-08-14 at 8:33 PM.

#nullable enable

namespace Librainian.Extensions {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;

	public static class Combiner {

		[ItemCanBeNull]
		public static IEnumerable<T> Append<T>( [NotNull][ItemCanBeNull] this IEnumerable<T> a, [NotNull][ItemCanBeNull] IEnumerable<T> b ) {
			foreach ( var item in a ) {
				yield return item;
			}

			foreach ( var item in b ) {
				yield return item;
			}
		}

		/// <summary>add the new item <paramref name="a" /> at the beginning of the collection <paramref name="b" /></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		[ItemCanBeNull]
		public static IEnumerable<T> Append<T>( [NotNull] this T a, [ItemCanBeNull][NotNull] IEnumerable<T> b ) {
			yield return a;

			foreach ( var item in b ) {
				yield return item;
			}
		}

		/// <summary>add the collection <paramref name="a" /> at the beginning of the new item <paramref name="b" /></summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		[ItemCanBeNull]
		public static IEnumerable<T> Append<T>( [NotNull] this IEnumerable<T> a, [CanBeNull] T b ) {
			foreach ( var item in a ) {
				yield return item;
			}

			yield return b;
		}

		[NotNull]
		public static IEnumerable<IEnumerable<T>> CartesianProduct<T>( [NotNull] this IEnumerable<IEnumerable<T>> sequences ) {
			IEnumerable<IEnumerable<T>> emptyProduct = new[] {
				Enumerable.Empty<T>()
			};

			return sequences.Aggregate( emptyProduct, ( accumulator, sequence ) => {
				var enumerable = sequence as IList<T> ?? sequence.ToList();

				return from accseq in accumulator
					   from item in enumerable
					   where accseq?.Contains( item ) == false
					   select accseq.Concat( new[] {
						   item
					   } );
			} );
		}

		[CanBeNull]
		public static IEnumerable<IEnumerable<T>?>? Combinations<T>( [CanBeNull] params IEnumerable<T>[]? input ) {
			IEnumerable<IEnumerable<T>> result = Array.Empty<T[]>();

			return input?.Aggregate( result, ( current, item ) => current.Combine( item.Combinations() ) );
		}

		[NotNull]
		public static IEnumerable<IEnumerable<T>> Combinations<T>( [NotNull] this IEnumerable<T> input ) =>
			input.Select( item => new[] {
				item
			} );

		[ItemNotNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>(
			[NotNull][ItemCanBeNull] this IEnumerable<IEnumerable<T>?> setA,
			[NotNull][ItemCanBeNull] IEnumerable<IEnumerable<T>?> setB
		) {
			var found = false;

			var eachB = setB as IEnumerable<T>[] ?? setB.ToArray();

			foreach ( var a in setA ) {
				if ( a != null ) {
                    found = true;

					foreach ( var b in eachB ) {

                        // ReSharper disable once PossibleMultipleEnumeration
                        if ( b != null ) {
                            yield return a.Append( b );
                        }
                    }
				}
			}

			if ( found ) {
				yield break;
			}

			foreach ( var b in eachB ) {
                if ( b != null ) {
                    yield return b;
                }
            }
		}

		[ItemNotNull]
        [CanBeNull]
		public static IEnumerable<IEnumerable<T>> Combine<T>( [CanBeNull] this IEnumerable<T>? setA, [NotNull] IEnumerable<IEnumerable<T>?>? setB ) {
			var found = false;

            if ( setB != null ) {
                foreach ( var b in setB ) {
                    found = true;

                    if ( setA != null ) {
                        // ReSharper disable once PossibleMultipleEnumeration
                        if ( b != null ) {
                            yield return setA.Append( b );
                        }
                    }
                }
            }

            if ( !found ) {
                if ( setA != null ) {
                    // ReSharper disable once PossibleMultipleEnumeration
                    yield return setA;
                }
            }
		}

		[ItemCanBeNull]
		[CanBeNull]
		public static IEnumerable<IEnumerable<T>> Combine<T>(
			[NotNull] [ItemCanBeNull] this IEnumerable<IEnumerable<T>> setA,
			[NotNull] [ItemCanBeNull] IEnumerable<T> setB
		) {
			var found = false;

			foreach ( var a in setA ) {
                found = true;

                if ( setB != null ) {
                    // ReSharper disable once PossibleMultipleEnumeration
                    yield return a.Append( setB );
                }
            }

			if ( !found ) {
                if ( setB != null ) {
                    // ReSharper disable once PossibleMultipleEnumeration
                    yield return setB;
                }
            }
		}

		[CanBeNull]
		[ItemCanBeNull]
		public static IEnumerable<IEnumerable<T>> Combine<T>( [NotNull] this T a, [NotNull] [ItemNotNull] IEnumerable<IEnumerable<T>> setB ) {
			var found = false;

			foreach ( var bGroup in setB ) {
				found = true;

				yield return a.Append( bGroup );
			}

			if ( !found ) {
				yield return new[] {
					a
				};
			}
		}

		[CanBeNull]
        [ItemNotNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>( [NotNull] [ItemNotNull] this IEnumerable<IEnumerable<T>> setA, [NotNull] T b ) {
			var found = false;

			foreach ( var aGroup in setA ) {
				found = true;

				yield return aGroup.Append( b );
			}

			if ( !found ) {
				yield return new[] {
					b
				};
			}
		}

		[NotNull]
		public static T[][] FastPowerSet<T>( [NotNull] this T[] array ) {
			var powerSet = new T[ 1 << array.Length ][];
			powerSet[ 0 ] = Array.Empty<T>();

			for ( var i = 0; i < array.Length; i++ ) {
				var cur = array[ i ];
				var count = 1 << i;

				for ( var j = 0; j < count; j++ ) {
					var source = powerSet[ j ];

					var sourceLength = source.Length;

					var destination = powerSet[ count + j ] = new T[ sourceLength + 1 ];

					for ( var q = 0; q < sourceLength; q++ ) {
						destination[ q ] = source[ q ];
					}

					destination[ sourceLength ] = cur;
				}
			}

			return powerSet;
		}

		[ItemCanBeNull]
		public static IEnumerable<T> Group<T>( [NotNull] this T a, [NotNull] T b ) {
			yield return a;
			yield return b;
		}

	}

}