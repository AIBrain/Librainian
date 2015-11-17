// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/MemoizeClass.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Concurrent;

    public static class MemoizeClass {

        public static Func<TA, TB, TR> Detuplify<TA, TB, TR>(this Func<Tuple<TA, TB>, TR> func) => (a, b) => func( Tuple.Create( a, b ) );

        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> f) {
            var d = new ConcurrentDictionary<TKey, TResult>();
            return a => {
                TResult value;
                if ( !d.TryGetValue( a, out value ) ) {
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

        public static Func<Tuple<TA, TB>, TR> Tuplify<TA, TB, TR>(this Func<TA, TB, TR> func) => t => func( t.Item1, t.Item2 );

        //static Func<A, B, R> Memoize( this Func<A, B, R> f ) {
        //    return f.Tuplify().Memoize().Detuplify();
        //}

        private static Func<T, TR> CastByExample<T, TR>(Func<T, TR> f, T t) => f;

        private static Func<TA, TB, TR> Memoize<TA, TB, TR>(this Func<TA, TB, TR> f) {
            var example = new {
                A = default(TA),
                B = default(TB)
            };
            var tuplified = CastByExample( t => f( t.A, t.B ), example );
            var memoized = tuplified.Memoize();
            return (a, b) => memoized( new {
                A = a,
                B = b
            } );
        }
    }
}