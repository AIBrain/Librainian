// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FuncExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

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

		public static Func<TParam, TResult> AsWeakMemoized<TSource, TResult, TParam>( this Func<TSource, TParam, TResult> selector, TSource source ) => param => {

			// Get the dictionary that associates delegates to a parameter, on the specified source
			var values = WeakResults.GetOrCreateValue( source );


			var key = new {
				selector,
				param
			};

			// Get the result for the combination source/selector/param
			var cached = values.TryGetValue( key, out var res );

			if ( !cached ) {
				res = selector( source, param );

				values[ key ] = res;
			}

			return ( TResult )res;
		};
	}
}