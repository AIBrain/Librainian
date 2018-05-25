// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Combiner.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Combiner.cs" was last formatted by Protiguous on 2018/05/24 at 7:07 PM.

namespace Librainian.Extensions {

    using System.Collections.Generic;
    using System.Linq;

    public static class Combiner {

        public static IEnumerable<T> Append<T>( this IEnumerable<T> a, IEnumerable<T> b ) {
            foreach ( var item in a ) { yield return item; }

            foreach ( var item in b ) { yield return item; }
        }

        /// <summary>
        ///     add the new item <paramref name="a" /> at the beginning of the collection <paramref name="b" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>( this T a, IEnumerable<T> b ) {
            yield return a;

            foreach ( var item in b ) { yield return item; }
        }

        /// <summary>
        ///     add the collection <paramref name="a" /> at the beginning of the new item <paramref name="b" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>( this IEnumerable<T> a, T b ) {
            foreach ( var item in a ) { yield return item; }

            yield return b;
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>( this IEnumerable<IEnumerable<T>> sequences ) {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

            return sequences.Aggregate( emptyProduct, ( accumulator, sequence ) => {
                var enumerable = sequence as IList<T> ?? sequence.ToList();

                return from accseq in accumulator from item in enumerable where !accseq.Contains( item ) select accseq.Concat( new[] { item } );
            } );
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>( params IEnumerable<T>[] input ) {
            IEnumerable<IEnumerable<T>> result = new T[0][];

            return input.Aggregate( result, ( current, item ) => current.Combine( item.Combinationss() ) );
        }

        public static IEnumerable<IEnumerable<T>> Combinationss<T>( this IEnumerable<T> input ) => input.Select( item => new[] { item } );

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<IEnumerable<T>> groupAs, IEnumerable<IEnumerable<T>> groupBs ) {
            var found = false;

            var bs = groupBs as IEnumerable<T>[] ?? groupBs.ToArray();

            foreach ( var groupA in groupAs ) {
                found = true;

                foreach ( var groupB in bs ) { yield return groupA.Append( groupB ); }
            }

            if ( found ) { yield break; }

            foreach ( var groupB in bs ) { yield return groupB; }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<T> a, IEnumerable<IEnumerable<T>> b ) {
            var found = false;

            foreach ( var bGroup in b ) {
                found = true;

                yield return a.Append( bGroup );
            }

            if ( !found ) { yield return a; }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<IEnumerable<T>> a, IEnumerable<T> b ) {
            var found = false;

            foreach ( var aGroup in a ) {
                found = true;

                yield return aGroup.Append( b );
            }

            if ( !found ) { yield return b; }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this T a, IEnumerable<IEnumerable<T>> b ) {
            var found = false;

            foreach ( var bGroup in b ) {
                found = true;

                yield return a.Append( bGroup );
            }

            if ( !found ) { yield return new[] { a }; }
        }

        public static IEnumerable<IEnumerable<T>> Combine<T>( this IEnumerable<IEnumerable<T>> a, T b ) {
            var found = false;

            foreach ( var aGroup in a ) {
                found = true;

                yield return aGroup.Append( b );
            }

            if ( !found ) { yield return new[] { b }; }
        }

        public static T[][] FastPowerSet<T>( this T[] seq ) {
            var powerSet = new T[1 << seq.Length][];
            powerSet[0] = new T[0];

            for ( var i = 0; i < seq.Length; i++ ) {
                var cur = seq[i];
                var count = 1 << i;

                for ( var j = 0; j < count; j++ ) {
                    var source = powerSet[j];
                    var destination = powerSet[count + j] = new T[source.Length + 1];

                    for ( var q = 0; q < source.Length; q++ ) { destination[q] = source[q]; }

                    destination[source.Length] = cur;
                }
            }

            return powerSet;
        }

        public static IEnumerable<T> Group<T>( this T a, T b ) {
            yield return a;
            yield return b;
        }
    }
}