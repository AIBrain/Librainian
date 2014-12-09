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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/DictionaryExtensions.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Annotations;

    public static class DictionaryExtensions {

        public static void Add<TKey, TValue>( [NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> otherKVP, Boolean ignoreUpdates = false ) {
            if ( dictionary == null ) {
                throw new ArgumentNullException( "dictionary" );
            }
            if ( otherKVP == null ) {
                throw new ArgumentNullException( "otherKVP" );
            }

            if ( ignoreUpdates ) {
                foreach ( var pair in otherKVP.Where( pair => !dictionary.ContainsKey( pair.Key ) ) ) {
                    dictionary.Add( pair.Key, pair.Value );
                }
            }
            else {
                foreach ( var pair in otherKVP ) {
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