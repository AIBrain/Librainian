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
            this.Ints = new PersistTable<UInt64, String>( new Folder( commonName, nameof( this.Ints ) ), true );
            this.Words = new PersistTable<String, UInt64>( new Folder( commonName, nameof( this.Words ) ), true );
        }

        public IEnumerable<UInt64> EachInt() => this.Ints.Keys;

        public IEnumerable<String> EachWord() => this.Words.Keys;

        [JsonProperty]
        public PersistTable<UInt64, String> Ints { get; }

        [JsonProperty]
        public PersistTable<String, UInt64> Words { get; }

        /// <summary>Get or set the <paramref name="key"/> for this word.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UInt64 this[[NotNull] String key] {
            get => this.Words.TryGetValue( key, out var result ) ? result : default;

            set {
                if ( String.IsNullOrEmpty( key ) ) {
                    return;
                }

                this.Words[key] = value;
                this.Ints[value] = key;
            }
        }

        /// <summary>Get or set the word for this guid.</summary>
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

        /// <summary>Returns true if the word is contained in the collections.</summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Boolean Contains( String word ) {
            if ( String.IsNullOrEmpty( word ) ) {
                return false;
            }

            return this.Words.TryGetValue( key: word, value: out var _ );
        }

        /// <summary>Returns true if the guid is contained in the collection.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean Contains( UInt64 key ) => this.Ints.TryGetValue( key, out var _ );

    }
}