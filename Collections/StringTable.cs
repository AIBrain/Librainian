// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/StringTable.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

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
        /// Get or set the <paramref name="key"/> for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UInt64 this[[NotNull] String key] {
            get => this.Words.TryGetValue( key, value: out var result ) ? result : default;

            set {
                if ( String.IsNullOrEmpty( value: key ) ) {
                    return;
                }

                this.Words[key] = value;
                this.Ints[value] = key;
            }
        }

        /// <summary>
        /// Get or set the word for this guid.
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
        /// Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Boolean Contains( String word ) {
            if ( String.IsNullOrEmpty( value: word ) ) {
                return false;
            }

            return this.Words.TryGetValue( word, value: out _ );
        }

        /// <summary>
        /// Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean Contains( UInt64 key ) => this.Ints.TryGetValue( key, value: out _ );

        public IEnumerable<UInt64> EachInt() => this.Ints.Keys;

        public IEnumerable<String> EachWord() => this.Words.Keys;
    }
}