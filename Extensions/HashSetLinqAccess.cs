// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HashSetLinqAccess.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     This extension method class will add a ToHashSet <typeparamref name="&gt;" /> in exactly the
    ///     same way it is provided by the others: ToList(), ToArray(), ToDictionary().. Now ToHashSet()
    ///     is available.
    /// UPDATE: These might be available in the newer Libraries.
    /// </summary>
    /// <seealso
    ///     cref="http://blogs.windowsclient.net/damonwildercarr/archive/2008/09/10/expose-new-linq-operations-from-the-screaming-hashset-lt-t-gt-collection.aspx" />
    public static class HashSetLinqAccess {

        public static HashSet<T> AddRange<T>( this HashSet<T> hashSet, IEnumerable<T> range ) {
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

        public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable, IEqualityComparer<T> comparer ) {
            if ( null == fromEnumerable ) {
                throw new ArgumentNullException( nameof( fromEnumerable ) );
            }

            if ( null == comparer ) {
                comparer = EqualityComparer<T>.Default;
            }

            if ( fromEnumerable is HashSet<T> set ) {
                return set;
            }

            return new HashSet<T>( fromEnumerable, comparer );
        }

        //public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable ) => ToHashSet( fromEnumerable, EqualityComparer<T>.Default );
    }
}