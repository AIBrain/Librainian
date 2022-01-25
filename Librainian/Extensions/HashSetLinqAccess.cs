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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "HashSetLinqAccess.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.Extensions;

using System.Collections.Generic;
using Exceptions;

/// <summary>
///     This extension method class will add a ToHashSet <typeparamref name="&gt;" /> in exactly the same way it is
///     provided by the others: ToList(), ToArray(), ToDictionary()..
///     Now ToHashSet() is available. UPDATE: These might be available in the newer Libraries.
/// </summary>
/// <seealso
///     cref="http://blogs.windowsclient.net/damonwildercarr/archive/2008/09/10/expose-new-linq-operations-from-the-screaming-hashset-lt-t-gt-collection.aspx" />
public static class HashSetLinqAccess {

	public static HashSet<T> AddRange<T>( this HashSet<T> hashSet, IEnumerable<T> range ) {
		if ( Equals( hashSet, null ) ) {
			throw new ArgumentEmptyException( nameof( hashSet ) );
		}

		if ( Equals( range, null ) ) {
			throw new ArgumentEmptyException( nameof( range ) );
		}

		foreach ( var item in range ) {
			hashSet.Add( item );
		}

		return hashSet;
	}

	public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable, IEqualityComparer<T>? comparer ) {
		if ( fromEnumerable is null ) {
			throw new ArgumentEmptyException( nameof( fromEnumerable ) );
		}

		comparer ??= EqualityComparer<T>.Default;

		if ( fromEnumerable is HashSet<T> set ) {
			return set;
		}

		return new HashSet<T>( fromEnumerable, comparer );
	}

	//public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable ) => ToHashSet( fromEnumerable, EqualityComparer<T>.Default );
}