// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FuncExtensions.cs" last touched on 2021-10-13 at 4:25 PM by Protiguous.

namespace Librainian.Extensions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Exceptions;
using Storage = System.Collections.Concurrent.ConcurrentDictionary<System.Object, System.Object>;

/// <summary>http://www.jaylee.org/post/2013/04/22/Immutable-Data-and-Memoization-in-CSharp-Part-2.aspx</summary>
public static class FuncExtensions {

	public static ConditionalWeakTable<Object, Storage> WeakResults { get; } = new();

	// Since is not possible to implicitly make a Func<T,U> out of a method group, let's use the source as a function type inference.
	public static TResult? ApplyMemoized<TSource, TResult, TParam>(
		[DisallowNull] this TSource source,
		Func<TSource, TParam, TResult> selector,
		[DisallowNull] TParam param
	) {
		if ( source is null ) {
			throw new NullException( nameof( source ) );
		}

		if ( selector == null ) {
			throw new NullException( nameof( selector ) );
		}

		if ( param is null ) {
			throw new NullException( nameof( param ) );
		}

		return selector.AsWeakMemoized( source )( param );
	}

	public static Func<TParam, TResult?> AsWeakMemoized<TSource, TResult, TParam>( this Func<TSource, TParam, TResult> selector, [DisallowNull] TSource source ) {
		if ( selector == null ) {
			throw new NullException( nameof( selector ) );
		}

		if ( source is null ) {
			throw new NullException( nameof( source ) );
		}

		return param => {
			// Get the dictionary that associates delegates to a parameter, on the specified source
			var values = WeakResults.GetOrCreateValue( source );

			var key = new {
				selector, param
			};

			// Get the result for the combination source/selector/param
			var cached = values.TryGetValue( key, out var res );

			if ( !cached ) {
				res = selector( source, param );

				if ( res != null ) {
					values[ key ] = res;
				}
			}

			if ( res != null ) {
				return ( TResult? )res;
			}

			return default;
		};
	}

}