// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Combiner.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Combiner.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using JetBrains.Annotations;

    [SuppressMessage( "ReSharper", "PossibleMultipleEnumeration" )]
    public static class Combiner {

        [ItemCanBeNull]
        public static IEnumerable<T> Append<T>( [CanBeNull] this IEnumerable<T> a, [CanBeNull] IEnumerable<T> b ) {
            if ( a != null ) {
                foreach ( var item in a ) {
                    yield return item;
                }
            }

            if ( b != null ) {
                foreach ( var item in b ) {
                    yield return item;
                }
            }
        }

        /// <summary>add the new item <paramref name="a" /> at the beginning of the collection <paramref name="b" /></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [ItemCanBeNull]
        public static IEnumerable<T> Append<T>( [CanBeNull] this T a, [ItemCanBeNull] [CanBeNull] IEnumerable<T> b ) {
            yield return a;

            if ( b != null ) {
                foreach ( var item in b ) {
                    yield return item;
                }
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

        [CanBeNull]
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>( [NotNull] this IEnumerable<IEnumerable<T>> sequences ) {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] {
                Enumerable.Empty<T>()
            };

            return sequences.Aggregate( emptyProduct, ( accumulator, sequence ) => {
                var enumerable = sequence as IList<T> ?? sequence?.ToList() ?? Enumerable.Empty<T>();

                return
                    from accseq in accumulator
                    from item in enumerable
                    where accseq?.Contains( item ) == false
                    select accseq?.Concat( new[] {
                        item
                    } );
            } );
        }

        [CanBeNull]
        public static IEnumerable<IEnumerable<T>> Combinations<T>( [NotNull] params IEnumerable<T>[] input ) {
            if ( input is null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            IEnumerable<IEnumerable<T>> result = new T[ 0 ][];

            return input.Aggregate( result, ( current, item ) => item != null ? current?.Combine( item.Combinations() ) : default );
        }

        [NotNull]
        public static IEnumerable<IEnumerable<T>> Combinations<T>( [NotNull] this IEnumerable<T> input ) =>
            input.Select( item => new[] {
                item
            } );

        [ItemCanBeNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>( [NotNull] this IEnumerable<IEnumerable<T>> groupAs, [NotNull] IEnumerable<IEnumerable<T>> groupBs ) {
            var found = false;

            var bs = groupBs as IEnumerable<T>[] ?? groupBs.ToArray();

            foreach ( var groupA in groupAs ) {
                found = true;

                foreach ( var groupB in bs ) {

                    yield return groupA.Append( groupB );
                }
            }

            if ( found ) {
                yield break;
            }

            foreach ( var groupB in bs ) {
                yield return groupB;
            }
        }

        [ItemCanBeNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>( [CanBeNull] this IEnumerable<T> a, [NotNull] IEnumerable<IEnumerable<T>> b ) {
            var found = false;

            foreach ( var bGroup in b ) {
                found = true;

                yield return a.Append( bGroup );
            }

            if ( !found ) {

                yield return a;
            }
        }

        [ItemCanBeNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>( [NotNull] this IEnumerable<IEnumerable<T>> a, [CanBeNull] IEnumerable<T> b ) {
            var found = false;

            foreach ( var aGroup in a ) {
                found = true;

                yield return aGroup.Append( b );
            }

            if ( !found ) {
                yield return b;
            }
        }

        [ItemCanBeNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>( [CanBeNull] this T a, [NotNull] IEnumerable<IEnumerable<T>> b ) {
            var found = false;

            foreach ( var bGroup in b ) {
                found = true;

                yield return a.Append( bGroup );
            }

            if ( !found ) {
                yield return new[] {
                    a
                };
            }
        }

        [ItemCanBeNull]
        public static IEnumerable<IEnumerable<T>> Combine<T>( [NotNull] this IEnumerable<IEnumerable<T>> a, [CanBeNull] T b ) {
            var found = false;

            foreach ( var aGroup in a ) {
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
        public static T[][] FastPowerSet<T>( [NotNull] this T[] seq ) {
            var powerSet = new T[ 1 << seq.Length ][];
            powerSet[ 0 ] = new T[ 0 ];

            for ( var i = 0; i < seq.Length; i++ ) {
                var cur = seq[ i ];
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
        public static IEnumerable<T> Group<T>( [CanBeNull] this T a, [CanBeNull] T b ) {
            yield return a;
            yield return b;
        }
    }
}