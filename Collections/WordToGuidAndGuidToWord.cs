// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "WordToGuidAndGuidToWord.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original
// license has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/WordToGuidAndGuidToWord.cs" was last cleaned by Protiguous on 2018/05/15 at 1:29 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Persistence;

    /// <summary>
    /// Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    [JsonObject]
    public class WordToGuidAndGuidToWord {

        private readonly String _baseCollectionName = "WordToGuidAndGuidToWord";

        private readonly String _baseCollectionNameExt;

        [JsonProperty]
        private readonly ConcurrentDictionary<Guid, String> _guids = new ConcurrentDictionary<Guid, String>();

        [JsonProperty]
        private readonly ConcurrentDictionary<String, Guid> _words = new ConcurrentDictionary<String, Guid>();

        public WordToGuidAndGuidToWord( [NotNull] String baseCollectionName, [NotNull] String baseCollectionNameExt ) {
            if ( baseCollectionName is null ) { throw new ArgumentNullException( nameof( baseCollectionName ) ); }

            this.IsDirty = false;
            this._baseCollectionNameExt = String.Empty;

            if ( !String.IsNullOrEmpty( value: baseCollectionName ) ) { this._baseCollectionName = baseCollectionName; }

            this._baseCollectionNameExt = baseCollectionNameExt ?? throw new ArgumentNullException( nameof( baseCollectionNameExt ) );

            if ( String.IsNullOrEmpty( value: this._baseCollectionNameExt ) ) { this._baseCollectionNameExt = "xml"; }
        }

        public IEnumerable<Guid> EachGuid => this._guids.Keys;

        public IEnumerable<String> EachWord => this._words.Keys;

        [JsonIgnore]
        public Boolean IsDirty { get; set; }

        public Int32 Count => Math.Min( val1: this._words.Count, val2: this._guids.Count );

        /// <summary>
        /// Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[String key] {
            get => String.IsNullOrEmpty( value: key ) ? Guid.Empty : this._words[key];

            set {
                if ( String.IsNullOrEmpty( value: key ) ) { return; }

                if ( this._words.ContainsKey( key ) && value == this._words[key] ) { return; }

                this._words[key] = value;
                this[value] = key;

                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[Guid key] {
            get => Guid.Empty.Equals( g: key ) ? String.Empty : this._guids[key];

            set {
                if ( Guid.Empty.Equals( g: key ) ) { return; }

                //Are they removing the guid from both lists?
                if ( String.IsNullOrEmpty( value: value ) ) {
                    this._guids.TryRemove( key, value: out var oldstringfortheguid );

                    if ( String.IsNullOrEmpty( value: oldstringfortheguid ) ) { return; }

                    this._words.TryRemove( oldstringfortheguid, value: out var oldguid );
                    oldguid.Equals( g: key ).BreakIfFalse();
                    this.IsDirty = true;
                }
                else {
                    if ( this._guids.ContainsKey( key ) && value == this._guids[key] ) { return; }

                    this._guids[key] = value;
                    this.IsDirty = true;
                }
            }
        }

        public void Clear() {
            if ( this._words.IsEmpty && this._guids.IsEmpty ) { return; }

            this._words.Clear();
            this._guids.Clear();
            this.IsDirty = true;
        }

        /// <summary>
        /// Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="theWord"></param>
        /// <returns></returns>
        public Boolean Contains( [NotNull] String theWord ) {
            if ( theWord is null ) { throw new ArgumentNullException( nameof( theWord ) ); }

            return this._words.Keys.Contains( item: theWord ) && this._guids.Values.Contains( item: theWord );
        }

        /// <summary>
        /// Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="theGuid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid theGuid ) => this._words.Values.Contains( item: theGuid ) && this._guids.Keys.Contains( item: theGuid );

        public Boolean Load() {
            if ( this._baseCollectionName is null ) { return false; }

            //DiagnosticTests.TestWordVsGuid( this );

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
        /// Returns true if the collections are persisted to storage (or empty).
        /// </summary>
        /// <returns></returns>
        public Boolean Save() {
            if ( this.IsDirty ) {
                return !String.IsNullOrWhiteSpace( value: this._baseCollectionName ) && this._words.Saver( fileName: Path.ChangeExtension( this._baseCollectionName, extension: this._baseCollectionNameExt ) );
            }

            return true;
        }
    }
}