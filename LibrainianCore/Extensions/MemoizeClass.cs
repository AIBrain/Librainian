// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MemoizeClass.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "MemoizeClass.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace LibrainianCore.Extensions {

    using System;
    using System.Collections.Concurrent;
    using JetBrains.Annotations;

    public static class MemoizeClass {

        [CanBeNull]
        private static Func<T, TR> CastByExample<T, TR>( [CanBeNull] Func<T, TR> f, [CanBeNull] T _ ) => f;

        //static Func<A, B, R> Memoize( this Func<A, B, R> f ) {
        //    return f.Tuplify().Memoize().Detuplify();
        //}

        [NotNull]
        private static Func<TA, TB, TR> Memoize<TA, TB, TR>( [CanBeNull] this Func<TA, TB, TR> f ) {
            var example = new {
                A = default( TA ),
                B = default( TB )
            };

            var tuplified = CastByExample( t => f( t.A, t.B ), example );
            var memoized = tuplified.Memoize();

            return ( a, b ) => memoized( new {
                A = a,
                B = b
            } );
        }

        [NotNull]
        public static Func<TA, TB, TR> Detuplify<TA, TB, TR>( [CanBeNull] this Func<Tuple<TA, TB>, TR> func ) => ( a, b ) => func( Tuple.Create( a, b ) );

        [NotNull]
        public static Func<TKey, TResult> Memoize<TKey, TResult>( [CanBeNull] this Func<TKey, TResult> f ) {
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

        [NotNull]
        public static Func<Tuple<TA, TB>, TR> Tuplify<TA, TB, TR>( [CanBeNull] this Func<TA, TB, TR> func ) => t => func( t.Item1, t.Item2 );
    }
}