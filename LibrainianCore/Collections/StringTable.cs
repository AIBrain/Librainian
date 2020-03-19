// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "StringTable.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "StringTable.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using OperatingSystem.FileSystem;
    using Persistence;

    [JsonObject]
    public class StringTable {

        [JsonProperty]
        public PersistTable<UInt64, String> Ints { get; }

        [JsonProperty]
        public PersistTable<String, UInt64> Words { get; }

        /// <summary>Get or set the <paramref name="key" /> for this word.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UInt64 this[ [NotNull] String key ] {
            get => this.Words.TryGetValue( key: key, value: out var result ) ? result : default;

            set {
                if ( String.IsNullOrEmpty( value: key ) ) {
                    return;
                }

                this.Words[ key: key ] = value;
                this.Ints[ key: value ] = key;
            }
        }

        /// <summary>Get or set the word for this guid.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public String this[ UInt64 key ] {
            get => this.Ints[ key: key ];

            set {
                if ( value != null ) {
                    this.Words[ key: value ] = key;
                }

                this.Ints[ key: key ] = value;
            }
        }

        public StringTable( [NotNull] Folder commonName ) {
            if ( commonName is null ) {
                throw new ArgumentNullException( paramName: nameof( commonName ) );
            }

            this.Ints = new PersistTable<UInt64, String>( folder: new Folder( folder: commonName, subFolder: nameof( this.Ints ) ), testForReadWriteAccess: true );
            this.Words = new PersistTable<String, UInt64>( folder: new Folder( folder: commonName, subFolder: nameof( this.Words ) ), testForReadWriteAccess: true );
        }

        public void Clear() {
            this.Words.Clear();
            this.Ints.Clear();
        }

        /// <summary>Returns true if the word is contained in the collections.</summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public Boolean Contains( [CanBeNull] String word ) {
            if ( String.IsNullOrEmpty( value: word ) ) {
                return default;
            }

            return this.Words.TryGetValue( key: word, value: out _ );
        }

        /// <summary>Returns true if the guid is contained in the collection.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean Contains( UInt64 key ) => this.Ints.TryGetValue( key: key, value: out _ );

        [NotNull]
        public ICollection<UInt64> EachInt() => this.Ints.Keys;

        [NotNull]
        public ICollection<String> EachWord() => this.Words.Keys;
    }
}