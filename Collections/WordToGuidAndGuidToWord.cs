#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/WordToGuidAndGuidToWord.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Annotations;
    using Librainian.Extensions;
    using NUnit.Framework;
    using Parsing;
    using Persistence;

    /// <summary>
    /// Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    [DataContract( IsReference = true )]
    [Obsolete]
    public class WordToGuidAndGuidToWord : Dirtyable {
        private readonly String _baseCollectionName = "WordToGuidAndGuidToWord";

        private readonly String _baseCollectionNameExt = String.Empty;

        [DataMember]
        [OptionalField]
        private readonly ConcurrentDictionary<Guid, String> _guids = new ConcurrentDictionary<Guid, String>();

        [DataMember]
        [OptionalField]
        private readonly ConcurrentDictionary<String, Guid> _words = new ConcurrentDictionary<String, Guid>();

        public WordToGuidAndGuidToWord( [NotNull] String baseCollectionName, [NotNull] String baseCollectionNameExt ) {
            if ( baseCollectionName == null ) {
                throw new ArgumentNullException( "baseCollectionName" );
            }
            if ( baseCollectionNameExt == null ) {
                throw new ArgumentNullException( "baseCollectionNameExt" );
            }
            this.IsDirty = false;

            if ( !String.IsNullOrEmpty( baseCollectionName ) ) {
                this._baseCollectionName = baseCollectionName;
            }

            this._baseCollectionNameExt = baseCollectionNameExt;
            if ( String.IsNullOrEmpty( this._baseCollectionNameExt ) ) {
                this._baseCollectionNameExt = "xml";
            }
        }

        public int Count { get { return Math.Min( this._words.Count, this._guids.Count ); } }

        public IEnumerable<Guid> EachGuid { get { return this._guids.Keys; } }

        public IEnumerable<String> EachWord { get { return this._words.Keys; } }

        /// <summary>
        /// Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[ String key ] {
            get { return String.IsNullOrEmpty( key ) ? Guid.Empty : this._words[ key ]; }

            set {
                if ( String.IsNullOrEmpty( key ) ) {
                    return;
                }
                if ( this._words.ContainsKey( key ) && value == this._words[ key ] ) {
                    return;
                }
                this._words[ key ] = value;
                this[ value ] = String.Intern( key );

                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[ Guid key ] {
            get { return Guid.Empty.Equals( key ) ? String.Empty : this._guids[ key ]; }

            set {
                if ( Guid.Empty.Equals( key ) ) {
                    return;
                }

                //Are they removing the guid from both lists?
                if ( String.IsNullOrEmpty( value ) ) {
                    String oldstringfortheguid;
                    this._guids.TryRemove( key, out oldstringfortheguid );

                    if ( !String.IsNullOrEmpty( oldstringfortheguid ) ) {
                        Guid oldguid;
                        this._words.TryRemove( oldstringfortheguid, out oldguid );
                        oldguid.Equals( key ).DebugAssert();
                        this.IsDirty = true;
                    }
                }
                else {
                    if ( this._guids.ContainsKey( key ) && value == this._guids[ key ] ) {
                        return;
                    }
                    this._guids[ key ] = String.Intern( value );
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

        /// <summary>
        /// Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="daword"></param>
        /// <returns></returns>
        public Boolean Contains( [NotNull] String daword ) {
            if ( daword == null ) {
                throw new ArgumentNullException( "daword" );
            }
            return this._words.Keys.Contains( daword ) && this._guids.Values.Contains( daword );
        }

        /// <summary>
        /// Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="daguid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid daguid ) {
            return this._words.Values.Contains( daguid ) && this._guids.Keys.Contains( daguid );
        }

        [Test]
        public void InternalTest() {
            var g = new Guid( @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            this[ "AIBrain" ] = g;
            this[ g ].Same( "AIBrain" ).DebugAssert();
        }

        public Boolean Load() {
            if ( this._baseCollectionName == null ) {
                return false;
            }
            this.InternalTest();

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
        /// Returns true if the collections are persisted to storage (or empty).
        /// </summary>
        /// <returns></returns>
        public Boolean Save() {
            if ( this.IsDirty ) {
                return !String.IsNullOrWhiteSpace( this._baseCollectionName ) && this._words.Saver( Path.ChangeExtension( this._baseCollectionName, this._baseCollectionNameExt ) );
            }
            return true;
        }
    }
}