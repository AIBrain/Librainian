// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WordToGuidAndGuidToWord.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "WordToGuidAndGuidToWord.cs" was last formatted by Protiguous on 2018/07/10 at 8:52 PM.

namespace Librainian.Collections
{

    using ComputerSystem.FileSystem;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Persistence;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    ///     Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    [JsonObject]
    public class WordToGuidAndGuidToWord
    {

        [JsonProperty]
        private ConcurrentDictionary<Guid, String> Guids { get; } = new ConcurrentDictionary<Guid, String>();

        [JsonProperty]
        private ConcurrentDictionary<String, Guid> Words { get; } = new ConcurrentDictionary<String, Guid>();

        [NotNull]
        public IEnumerable<Guid> EachGuid => this.Guids.Keys;

        [NotNull]
        public IEnumerable<String> EachWord => this.Words.Keys;

        [JsonIgnore]
        public Boolean IsDirty { get; set; }

        public Int32 Count => (this.Words.Count + this.Guids.Count) / 2;

        /// <summary>
        ///     Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[[CanBeNull] String key] {
            get => String.IsNullOrEmpty(key) ? Guid.Empty : this.Words[key];

            set {
                if (String.IsNullOrEmpty(key)) { return; }

                if (this.Words.ContainsKey(key) && value == this.Words[key]) { return; }

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
            get => Guid.Empty.Equals(g: key) ? String.Empty : this.Guids[key];

            set {
                if (Guid.Empty.Equals(g: key)) { return; }

                //Are they removing the guid from both lists?
                if (String.IsNullOrEmpty(value))
                {
                    this.Guids.TryRemove(key, out var oldstringfortheguid);

                    if (String.IsNullOrEmpty(oldstringfortheguid)) { return; }

                    this.Words.TryRemove(oldstringfortheguid, out var oldguid);
                    //oldguid.Equals(g: key).BreakIfFalse();
                    this.IsDirty = true;
                }
                else
                {
                    if (this.Guids.ContainsKey(key) && value == this.Guids[key]) { return; }

                    this.Guids[key] = value;
                    this.IsDirty = true;
                }
            }
        }

        public WordToGuidAndGuidToWord([NotNull] String baseCollectionName, [NotNull] String baseCollectionNameExt) { }

        public void Clear()
        {
            if (this.Words.IsEmpty && this.Guids.IsEmpty) { return; }

            this.Words.Clear();
            this.Guids.Clear();
            this.IsDirty = true;
        }

        /// <summary>
        ///     Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="theWord"></param>
        /// <returns></returns>
        public Boolean Contains([NotNull] String theWord)
        {
            if (theWord == null) { throw new ArgumentNullException(nameof(theWord)); }

            return this.Words.Keys.Contains(item: theWord) && this.Guids.Values.Contains(item: theWord);
        }

        /// <summary>
        ///     Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="theGuid"></param>
        /// <returns></returns>
        public Boolean Contains(Guid theGuid) => this.Words.Values.Contains(item: theGuid) && this.Guids.Keys.Contains(item: theGuid);

        public Boolean Load()
        {

            "".Break();

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

        /// <summary>
        ///     Returns true if the collections are persisted to storage (or empty).
        /// </summary>
        /// <returns></returns>
        public Boolean Save() => !this.IsDirty || this.Words.TrySave(new Document(nameof(WordToGuidAndGuidToWord)));
    }
}