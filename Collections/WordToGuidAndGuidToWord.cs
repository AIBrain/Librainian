// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/WordToGuidAndGuidToWord.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Persistence;

    /// <summary>Contains Words and their guids. Persisted to and from storage? Thread-safe?</summary>
    [JsonObject]
    public class WordToGuidAndGuidToWord {
        private readonly String _baseCollectionName = "WordToGuidAndGuidToWord";
        private readonly String _baseCollectionNameExt;

        [JsonProperty]
        private readonly ConcurrentDictionary<Guid, String> _guids = new ConcurrentDictionary<Guid, String>();

        [JsonProperty]
        private readonly ConcurrentDictionary<String, Guid> _words = new ConcurrentDictionary<String, Guid>();

        public WordToGuidAndGuidToWord( [NotNull] String baseCollectionName, [NotNull] String baseCollectionNameExt ) {
            if ( baseCollectionName == null ) {
                throw new ArgumentNullException( nameof( baseCollectionName ) );
            }
            if ( baseCollectionNameExt == null ) {
                throw new ArgumentNullException( nameof( baseCollectionNameExt ) );
            }
            this.IsDirty = false;
            this._baseCollectionNameExt = String.Empty;

            if ( !String.IsNullOrEmpty( baseCollectionName ) ) {
                this._baseCollectionName = baseCollectionName;
            }

            this._baseCollectionNameExt = baseCollectionNameExt;
            if ( String.IsNullOrEmpty( this._baseCollectionNameExt ) ) {
                this._baseCollectionNameExt = "xml";
            }
        }

        public Int32 Count => Math.Min( this._words.Count, this._guids.Count );

        public IEnumerable<Guid> EachGuid => this._guids.Keys;

        public IEnumerable<String> EachWord => this._words.Keys;

        [JsonIgnore]
        public Boolean IsDirty {
            get; set;
        }

        /// <summary>Get or set the guid for this word.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[ String key ] {
            get {
                return String.IsNullOrEmpty( key ) ? Guid.Empty : this._words[ key ];
            }

            set {
                if ( String.IsNullOrEmpty( key ) ) {
                    return;
                }
                if ( this._words.ContainsKey( key ) && ( value == this._words[ key ] ) ) {
                    return;
                }
                this._words[ key ] = value;
                this[ value ] = key;

                this.IsDirty = true;
            }
        }

        /// <summary>Get or set the word for this guid.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[ Guid key ] {
            get {
                return Guid.Empty.Equals( key ) ? String.Empty : this._guids[ key ];
            }

            set {
                if ( Guid.Empty.Equals( key ) ) {
                    return;
                }

                //Are they removing the guid from both lists?
                if ( String.IsNullOrEmpty( value ) ) {
                    String oldstringfortheguid;
                    this._guids.TryRemove( key, out oldstringfortheguid );

                    if ( String.IsNullOrEmpty( oldstringfortheguid ) ) {
                        return;
                    }
                    Guid oldguid;
                    this._words.TryRemove( oldstringfortheguid, out oldguid );
                    oldguid.Equals( key ).BreakIfFalse();
                    this.IsDirty = true;
                }
                else {
                    if ( this._guids.ContainsKey( key ) && ( value == this._guids[ key ] ) ) {
                        return;
                    }
                    this._guids[ key ] = value;
                    this.IsDirty = true;
                }
            }
        }

        public void Clear() {
            if ( this._words.IsEmpty && this._guids.IsEmpty ) {
                return;
            }
            this._words.Clear();
            this._guids.Clear();
            this.IsDirty = true;
        }

        /// <summary>Returns true if the word is contained in the collections.</summary>
        /// <param name="theWord"></param>
        /// <returns></returns>
        public Boolean Contains( [NotNull] String theWord ) {
            if ( theWord == null ) {
                throw new ArgumentNullException( nameof( theWord ) );
            }
            return this._words.Keys.Contains( theWord ) && this._guids.Values.Contains( theWord );
        }

        /// <summary>Returns true if the guid is contained in the collection.</summary>
        /// <param name="theGuid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid theGuid ) => this._words.Values.Contains( theGuid ) && this._guids.Keys.Contains( theGuid );

        public Boolean Load() {
            if ( this._baseCollectionName == null ) {
                return false;
            }

            //DiagnosticTests.TestWordVsGuid( this );

            //var filename = Path.ChangeExtension( this.BaseCollectionName, this.BaseCollectionNameExt );
            //var storage = Storage.Loader<ConcurrentDictionary<String, Guid>>( filename, source => Cloning.DeepClone( Source: source, Destination: this ) );
            //if ( storage == null ) {
            //    return false;
            //}
            //var countBefore = this.Count;
            //foreach ( var word in storage.Keys ) {
            //    this[ word ] = storage[ word ];
            //}
            //this.IsDirty = this.Count != countBefore + storage.Keys.Count;
            return false;
        }

        /// <summary>Returns true if the collections are persisted to storage (or empty).</summary>
        /// <returns></returns>
        public Boolean Save() {
            if ( this.IsDirty ) {
                return !String.IsNullOrWhiteSpace( this._baseCollectionName ) && this._words.Saver( Path.ChangeExtension( this._baseCollectionName, this._baseCollectionNameExt ) );
            }
            return true;
        }
    }
}