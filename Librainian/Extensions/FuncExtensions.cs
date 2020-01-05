// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FuncExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FuncExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 7:13 AM.

namespace Librainian.Extensions {

    using System;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;
    using Storage = System.Collections.Concurrent.ConcurrentDictionary<System.Object, System.Object>;

    /// <summary>
    ///     http://www.jaylee.org/post/2013/04/22/Immutable-Data-and-Memoization-in-CSharp-Part-2.aspx
    /// </summary>
    public static class FuncExtensions {

        public static ConditionalWeakTable<Object, Storage> WeakResults { get; } = new ConditionalWeakTable<Object, Storage>();

        // Since is not possible to implicitly make a Func<T,U> out of a method group, let's use the source as a function type inference.
        [CanBeNull]
        public static TResult ApplyMemoized<TSource, TResult, TParam>( [CanBeNull] this TSource source, [CanBeNull] Func<TSource, TParam, TResult> selector, [CanBeNull] TParam param ) =>
            selector.AsWeakMemoized( source )( param );

        [NotNull]
        public static Func<TParam, TResult> AsWeakMemoized<TSource, TResult, TParam>( [CanBeNull] this Func<TSource, TParam, TResult> selector, [CanBeNull] TSource source ) =>
            param => {

                // Get the dictionary that associates delegates to a parameter, on the specified source
                var values = WeakResults.GetOrCreateValue( source );

                var key = new {
                    selector, param
                };

                // Get the result for the combination source/selector/param
                var cached = values.TryGetValue( key, out var res );

                if ( !cached ) {
                    res = selector( source, param );

                    values[ key ] = res;
                }

                return ( TResult ) res;
            };
    }
}