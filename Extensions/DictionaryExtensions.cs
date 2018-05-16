// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DictionaryExtensions.cs",
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
// "Librainian/Librainian/DictionaryExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    public static class DictionaryExtensions {

        public static void Add<TKey, TValue>( [NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] IEnumerable<KeyValuePair<TKey, TValue>> otherKvp, Boolean ignoreUpdates = false ) {
            if ( dictionary is null ) { throw new ArgumentNullException( nameof( dictionary ) ); }

            if ( otherKvp is null ) { throw new ArgumentNullException( nameof( otherKvp ) ); }

            if ( ignoreUpdates ) {
                foreach ( var pair in otherKvp.Where( pair => !dictionary.ContainsKey( pair.Key ) ) ) { dictionary.Add( pair.Key, pair.Value ); }
            }
            else {
                foreach ( var pair in otherKvp ) {
                    if ( dictionary.ContainsKey( pair.Key ) ) { dictionary[pair.Key] = pair.Value; }
                    else { dictionary.Add( pair.Key, pair.Value ); }
                }
            }
        }
    }
}