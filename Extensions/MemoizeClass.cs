// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "MemoizeClass.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/MemoizeClass.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Concurrent;

    public static class MemoizeClass {

        private static Func<T, TR> CastByExample<T, TR>( Func<T, TR> f, T t ) => f;

        //static Func<A, B, R> Memoize( this Func<A, B, R> f ) {
        //    return f.Tuplify().Memoize().Detuplify();
        //}
        private static Func<TA, TB, TR> Memoize<TA, TB, TR>( this Func<TA, TB, TR> f ) {
            var example = new { A = default( TA ), B = default( TB ) };
            var tuplified = CastByExample( t => f( t.A, t.B ), example );
            var memoized = tuplified.Memoize();

            return ( a, b ) => memoized( new { A = a, B = b } );
        }

        public static Func<TA, TB, TR> Detuplify<TA, TB, TR>( this Func<Tuple<TA, TB>, TR> func ) => ( a, b ) => func( Tuple.Create( a, b ) );

        public static Func<TKey, TResult> Memoize<TKey, TResult>( this Func<TKey, TResult> f ) {
            var d = new ConcurrentDictionary<TKey, TResult>();

            return a => {
                if ( !d.TryGetValue( a, out var value ) ) {
                    value = f( a );
                    d.TryAdd( a, value );
                }

                return value;
            };
        }

        //public static Func<Tuple<TA, TB>, TR> Memoize(this Func<Tuple<TA, TB>, TR> func) {
        //    Func<Tuple<TA, TB>, TR> tuplified = t => func( t.Item1, t.Item2 );
        //    Func<Tuple<TA, TB>, TR> memoized = tuplified.Memoize();
        //    return (a, b) => memoized( Tuple.Create( a, b ) );
        //}

        public static Func<Tuple<TA, TB>, TR> Tuplify<TA, TB, TR>( this Func<TA, TB, TR> func ) => t => func( t.Item1, t.Item2 );
    }
}