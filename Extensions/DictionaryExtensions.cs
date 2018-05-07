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
// "Librainian/DictionaryExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    public static class DictionaryExtensions {

        public static void Add<TKey, TValue>( [NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> otherKvp, Boolean ignoreUpdates = false ) {
            if ( dictionary is null ) {
                throw new ArgumentNullException( nameof( dictionary ) );
            }
            if ( otherKvp is null ) {
                throw new ArgumentNullException( nameof( otherKvp ) );
            }

            if ( ignoreUpdates ) {
                foreach ( var pair in otherKvp.Where( pair => !dictionary.ContainsKey( pair.Key ) ) ) {
                    dictionary.Add( pair.Key, pair.Value );
                }
            }
            else {
                foreach ( var pair in otherKvp ) {
                    if ( dictionary.ContainsKey( pair.Key ) ) {
                        dictionary[ pair.Key ] = pair.Value;
                    }
                    else {
                        dictionary.Add( pair.Key, pair.Value );
                    }
                }
            }
        }
    }
}