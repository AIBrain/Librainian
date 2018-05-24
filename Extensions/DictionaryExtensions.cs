// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DictionaryExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/DictionaryExtensions.cs" was last formatted by Protiguous on 2018/05/21 at 9:59 PM.

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