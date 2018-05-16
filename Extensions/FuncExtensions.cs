// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FuncExtensions.cs",
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
// "Librainian/Librainian/FuncExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.Runtime.CompilerServices;
    using Storage = System.Collections.Concurrent.ConcurrentDictionary<System.Object, System.Object>;

    /// <summary>
    ///     http://www.jaylee.org/post/2013/04/22/Immutable-Data-and-Memoization-in-CSharp-Part-2.aspx
    /// </summary>
    public static class FuncExtensions {

        public static ConditionalWeakTable<Object, Storage> WeakResults { get; } = new ConditionalWeakTable<Object, Storage>();

        // Since is not possible to implicitly make a Func<T,U> out of a method group, let's use the source as a function type inference.
        public static TResult ApplyMemoized<TSource, TResult, TParam>( this TSource source, Func<TSource, TParam, TResult> selector, TParam param ) => selector.AsWeakMemoized( source )( param );

        public static Func<TParam, TResult> AsWeakMemoized<TSource, TResult, TParam>( this Func<TSource, TParam, TResult> selector, TSource source ) =>
            param => {

                // Get the dictionary that associates delegates to a parameter, on the specified source
                var values = WeakResults.GetOrCreateValue( source );

                var key = new { selector, param };

                // Get the result for the combination source/selector/param
                var cached = values.TryGetValue( key, out var res );

                if ( !cached ) {
                    res = selector( source, param );

                    values[key] = res;
                }

                return ( TResult )res;
            };
    }
}