// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WordToGuidAndGuidToWord.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/WordToGuidAndGuidToWord.cs" was last formatted by Protiguous on 2018/05/22 at 5:55 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using ComputerSystems.FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Persistence;

    /// <summary>
    ///     Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    [JsonObject]
    public class WordToGuidAndGuidToWord {

        [JsonProperty]
        private ConcurrentDictionary<Guid, String> Guids { get; } = new ConcurrentDictionary<Guid, String>();

        [JsonProperty]
        private ConcurrentDictionary<String, Guid> Words { get; } = new ConcurrentDictionary<String, Guid>();

        public IEnumerable<Guid> EachGuid => this.Guids.Keys;

        public IEnumerable<String> EachWord => this.Words.Keys;

        [JsonIgnore]
        public Boolean IsDirty { get; set; }

        public Int32 Count => ( this.Words.Count + this.Guids.Count ) / 2;

        public WordToGuidAndGuidToWord( [NotNull] String baseCollectionName, [NotNull] String baseCollectionNameExt ) { }

        /// <summary>
        ///     Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[String key] {
            get => String.IsNullOrEmpty( key ) ? Guid.Empty : this.Words[key];

            set {
                if ( String.IsNullOrEmpty( key ) ) { return; }

                if ( this.Words.ContainsKey( key ) && value == this.Words[key] ) { return; }

                this.Words[key] = value;
                this[value] = key;

                this.IsDirty = true;
            }
        }

        /// <summary>
        ///     Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[Guid key] {
            get => Guid.Empty.Equals( g: key ) ? String.Empty : this.Guids[key];

            set {
                if ( Guid.Empty.Equals( g: key ) ) { return; }

                //Are they removing the guid from both lists?
                if ( String.IsNullOrEmpty( value ) ) {
                    this.Guids.TryRemove( key, out var oldstringfortheguid );

                    if ( String.IsNullOrEmpty( oldstringfortheguid ) ) { return; }

                    this.Words.TryRemove( oldstringfortheguid, out var oldguid );
                    oldguid.Equals( g: key ).BreakIfFalse();
                    this.IsDirty = true;
                }
                else {
                    if ( this.Guids.ContainsKey( key ) && value == this.Guids[key] ) { return; }

                    this.Guids[key] = value;
                    this.IsDirty = true;
                }
            }
        }

        public void Clear() {
            if ( this.Words.IsEmpty && this.Guids.IsEmpty ) { return; }

            this.Words.Clear();
            this.Guids.Clear();
            this.IsDirty = true;
        }

        /// <summary>
        ///     Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="theWord"></param>
        /// <returns></returns>
        public Boolean Contains( [NotNull] String theWord ) {
            if ( theWord is null ) { throw new ArgumentNullException( nameof( theWord ) ); }

            return this.Words.Keys.Contains( item: theWord ) && this.Guids.Values.Contains( item: theWord );
        }

        /// <summary>
        ///     Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="theGuid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid theGuid ) => this.Words.Values.Contains( item: theGuid ) && this.Guids.Keys.Contains( item: theGuid );

        public Boolean Load() {

            var obj = 1;

            //var filename = Path.ChangeExtension( this.BaseCollectionName, this.BaseCollectionNameExt );
            //var storage = Storage.Loader<ConcurrentDictionary<String, Guid>>( filename, source => Cloning.DeepClone( Source: source, Destination: this ) );
            //if ( storage is null ) {
            //    return false;
            //}
            //var countBefore = this.Count;
            //foreach ( var word in storage.Keys ) {
            //    this[ word ] = storage[ word ];
            //}
            //this.IsDirty = this.Count != countBefore + storage.Keys.Count;
            return false;
        }

        /// <summary>
        ///     Returns true if the collections are persisted to storage (or empty).
        /// </summary>
        /// <returns></returns>
        public Boolean Save() => !this.IsDirty || this.Words.TrySave( new Document( nameof( WordToGuidAndGuidToWord ) ) );
    }
}