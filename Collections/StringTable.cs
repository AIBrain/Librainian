// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "StringTable.cs",
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
// "Librainian/Librainian/StringTable.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Persistence;

    [JsonObject]
    public class StringTable {

        public StringTable( Folder commonName ) {
            this.Ints = new PersistTable<UInt64, String>( folder: new Folder( folder: commonName, subFolder: nameof( this.Ints ) ), testForReadWriteAccess: true );
            this.Words = new PersistTable<String, UInt64>( folder: new Folder( folder: commonName, subFolder: nameof( this.Words ) ), testForReadWriteAccess: true );
        }

        [JsonProperty]
        public PersistTable<UInt64, String> Ints { get; }

        [JsonProperty]
        public PersistTable<String, UInt64> Words { get; }

        /// <summary>
        ///     Get or set the <paramref name="key" /> for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UInt64 this[[NotNull] String key] {
            get => this.Words.TryGetValue( key, out var result ) ? result : default;

            set {
                if ( String.IsNullOrEmpty( key ) ) { return; }

                this.Words[key] = value;
                this.Ints[value] = key;
            }
        }

        /// <summary>
        ///     Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[UInt64 key] {
            get => this.Ints[key];

            set {
                this.Words[value] = key;
                this.Ints[key] = value;
            }
        }

        public void Clear() {
            this.Words.Clear();
            this.Ints.Clear();
        }

        /// <summary>
        ///     Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Boolean Contains( String word ) {
            if ( String.IsNullOrEmpty( word ) ) { return false; }

            return this.Words.TryGetValue( word, out _ );
        }

        /// <summary>
        ///     Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean Contains( UInt64 key ) => this.Ints.TryGetValue( key, out _ );

        public IEnumerable<UInt64> EachInt() => this.Ints.Keys;

        public IEnumerable<String> EachWord() => this.Words.Keys;
    }
}