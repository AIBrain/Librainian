// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Cache.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Solution "ArtificialllyIntelligentBrain"   Project "Librainian"  ***
// File "Cache.cs" was last formatted by Protiguous on 2018/09/24 at 6:57 AM.

namespace Librainian.Persistence {

    using Collections;
    using JetBrains.Annotations;
    using Parsing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;

    public static class Cache {

        /// <summary>
        ///     Gets a reference to the default <see cref="T:System.Runtime.Caching.MemoryCache" /> instance.
        /// </summary>
        [NotNull]
        public static MemoryCache Memory {
            get;
        } = MemoryCache.Default;

        /// <summary>
        ///     Build a unique string given the <paramref name="parts" />. (<see cref="ParsingExtensions.TriplePipes" /> between
        ///     each item)
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerStepThrough]
        public static TrimmedString BuildKey<T>([NotNull] params T[] parts) {
            if (parts == null) {
                throw new ArgumentNullException(nameof(parts)).Log();
            }

            var list = new List<T>(parts.Length);
            list.AddRange(parts.Where(arg => arg != null));

            if (!list.Any()) {
                throw new InvalidOperationException($"Error: No key-parts given in {nameof(Cache)}.{nameof(BuildKey)}.");
            }

            return new TrimmedString(list.ToStrings(ParsingExtensions.TriplePipes));
        }

        /*

        /// <summary>
        ///     Build a unique string given the <paramref name="collection" />. ( <see cref="ParsingExtensions.TriplePipes" />
        ///     between each item)
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerStepThrough]
        public static TrimmedString BuildKey([NotNull] ConcurrentHashset<SqlParameter> collection) {
            if (collection == null) {
                throw new ArgumentNullException(paramName: nameof(collection));
            }

            var parts = collection.Select(parm => new KeyValuePair<String, String>(parm.ParameterName, parm.Value.ToString()));

            return new TrimmedString(parts.ToStrings(ParsingExtensions.TriplePipes));
        }
        */
    }
}