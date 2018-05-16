// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "HashSetLinqAccess.cs",
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
// "Librainian/Librainian/HashSetLinqAccess.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     This extension method class will add a ToHashSet <typeparamref name="&gt;" /> in exactly the same way it is
    ///     provided by the others: ToList(), ToArray(), ToDictionary().. Now ToHashSet() is available.
    ///     UPDATE: These might be available in the newer Libraries.
    /// </summary>
    /// <seealso
    ///     cref="http://blogs.windowsclient.net/damonwildercarr/archive/2008/09/10/expose-new-linq-operations-from-the-screaming-hashset-lt-t-gt-collection.aspx" />
    public static class HashSetLinqAccess {

        public static HashSet<T> AddRange<T>( this HashSet<T> hashSet, IEnumerable<T> range ) {
            if ( Equals( hashSet, null ) ) { throw new ArgumentNullException( nameof( hashSet ) ); }

            if ( Equals( range, null ) ) { throw new ArgumentNullException( nameof( range ) ); }

            foreach ( var item in range ) { hashSet.Add( item: item ); }

            return hashSet;
        }

        public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable, IEqualityComparer<T> comparer ) {
            if ( null == fromEnumerable ) { throw new ArgumentNullException( nameof( fromEnumerable ) ); }

            if ( null == comparer ) { comparer = EqualityComparer<T>.Default; }

            if ( fromEnumerable is HashSet<T> set ) { return set; }

            return new HashSet<T>( fromEnumerable, comparer );
        }

        //public static HashSet<T> ToHashSet<T>( this IEnumerable<T> fromEnumerable ) => ToHashSet( fromEnumerable, EqualityComparer<T>.Default );
    }
}