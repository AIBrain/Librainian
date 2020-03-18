// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "HashSetLinqAccess.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "HashSetLinqAccess.cs" was last formatted by Protiguous on 2020/03/16 at 9:36 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// This extension method class will add a ToHashSet <typeparamref name="&gt;" /> in exactly the same way it is provided by the others: ToList(), ToArray(), ToDictionary()..
    /// Now ToHashSet() is available. UPDATE: These might be available in the newer Libraries.
    /// </summary>
    /// <seealso cref="http://blogs.windowsclient.net/damonwildercarr/archive/2008/09/10/expose-new-linq-operations-from-the-screaming-hashset-lt-t-gt-collection.aspx" />
    public static class HashSetLinqAccess {

        [NotNull]
        public static HashSet<T> AddRange<T>( [NotNull] this HashSet<T> hashSet, [NotNull] IEnumerable<T> range ) {
            if ( Equals( objA: hashSet, objB: null ) ) {
                throw new ArgumentNullException( paramName: nameof( hashSet ) );
            }

            if ( Equals( objA: range, objB: null ) ) {
                throw new ArgumentNullException( paramName: nameof( range ) );
            }

            foreach ( var item in range ) {
                hashSet.Add( item: item );
            }

            return hashSet;
        }

        [NotNull]
        public static HashSet<T> ToHashSet<T>( [NotNull] this IEnumerable<T> fromEnumerable, IEqualityComparer<T> comparer ) {
            if ( null == fromEnumerable ) {
                throw new ArgumentNullException( paramName: nameof( fromEnumerable ) );
            }

            if ( null == comparer ) {
                comparer = EqualityComparer<T>.Default;
            }

            if ( fromEnumerable is HashSet<T> set ) {
                return set;
            }

            return new HashSet<T>( collection: fromEnumerable, comparer: comparer );
        }

        //public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable ) => ToHashSet( fromEnumerable, EqualityComparer<T>.Default );

    }

}