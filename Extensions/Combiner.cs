#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/Combiner.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Extensions {

    using System.Collections.Generic;
    using System.Linq;

    public static class Combiner {

        public static IEnumerable<T> Append<T>( this IEnumerable<T> a, IEnumerable<T> b ) {
            foreach ( var item in a ) {
                yield return item;
            }
            foreach ( var item in b ) {
                yield return item;
            }
        }

        /// <summary>
        /// add the new item <paramref name="b" /> at the beginning of the collection <paramref
        /// name="a" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>( this T a, IEnumerable<T> b ) {
            yield return a;
            foreach ( var item in b ) {
                yield return item;
            }
        }

        public static IEnumerable<T> Append<T>( this IEnumerable<T> a, T b ) {
            foreach ( var item in a ) {
                yield return item;
            }
            yield return b;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>( params IEnumerable<T>[] input ) {
            IEnumerable<IEnumerable<T>> result = new T[ 0 ][];
            return input.Aggregate( result, ( current, item ) => current.Combine( item.Combinations() ) );
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>( this IEnumerable<T> input ) {
            return input.Select( item => new[] { item } );
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<IEnumerable<T>> groupAs, IEnumerable<IEnumerable<T>> groupBs ) {
            var found = false;

            foreach ( var groupA in groupAs ) {
                found = true;
                foreach ( var groupB in groupBs ) {
                    yield return groupA.Append( groupB );
                }
            }

            if ( found ) {
                yield break;
            }
            foreach ( var groupB in groupBs ) {
                yield return groupB;
            }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<T> a, IEnumerable<IEnumerable<T>> b ) {
            var found = false;

            foreach ( var bGroup in b ) {
                found = true;
                yield return a.Append( bGroup );
            }

            if ( !found ) {
                yield return a;
            }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<IEnumerable<T>> a, IEnumerable<T> b ) {
            var found = false;

            foreach ( var aGroup in a ) {
                found = true;
                yield return aGroup.Append( b );
            }

            if ( !found ) {
                yield return b;
            }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this T a, IEnumerable<IEnumerable<T>> b ) {
            var found = false;

            foreach ( var bGroup in b ) {
                found = true;
                yield return a.Append( bGroup );
            }

            if ( !found ) {
                yield return new[] { a };
            }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<IEnumerable<T>> a, T b ) {
            var found = false;

            foreach ( var aGroup in a ) {
                found = true;
                yield return aGroup.Append( b );
            }

            if ( !found ) {
                yield return new[] { b };
            }
        }

        public static IEnumerable<T> Group<T>( this T a, T b ) {
            yield return a;
            yield return b;
        }

        // add the new item at the end of the collection
    }
}