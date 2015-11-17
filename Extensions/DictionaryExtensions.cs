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
// "Librainian/DictionaryExtensions.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    public static class DictionaryExtensions {

        public static void Add<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> otherKvp, Boolean ignoreUpdates = false) {
            if ( dictionary == null ) {
                throw new ArgumentNullException( nameof( dictionary ) );
            }
            if ( otherKvp == null ) {
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