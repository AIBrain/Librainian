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
// "Librainian/HashSetLinqAccess.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This extension method class will add a ToHashSet <typeparamref name="&gt;" /> in exactly the
    /// same way it is provided by the others: ToList(), ToArray(), ToDictionary().. Now ToHashSet()
    /// is available
    /// </summary>
    /// <seealso cref="http://blogs.windowsclient.net/damonwildercarr/archive/2008/09/10/expose-new-linq-operations-from-the-screaming-hashset-lt-t-gt-collection.aspx" />
    public static class HashSetLinqAccess {

        public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range) {
            if ( Equals( hashSet, null ) ) {
                throw new ArgumentNullException( nameof( hashSet ) );
            }
            if ( Equals( range, null ) ) {
                throw new ArgumentNullException( nameof( range ) );
            }
            foreach ( var item in range ) {
                hashSet.Add( item: item );
            }
            return hashSet;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> fromEnumerable, IEqualityComparer<T> comparer) {
            if ( null == fromEnumerable ) {
                throw new ArgumentNullException( nameof( fromEnumerable ) );
            }

            if ( null == comparer ) {
                comparer = EqualityComparer<T>.Default;
            }

            if ( fromEnumerable is HashSet<T> ) {
                return fromEnumerable as HashSet<T>;
            }

            return new HashSet<T>( fromEnumerable, comparer );
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> fromEnumerable) => ToHashSet( fromEnumerable, EqualityComparer<T>.Default );
    }
}