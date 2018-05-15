// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "StringVersusGuid.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/StringVersusGuid.cs" was last cleaned by Protiguous on 2018/05/15 at 1:29 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    /// Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    /// <remarks>i can see places where the tables are locked independantly.. could cause issues??</remarks>
    [JsonObject]
    public class StringVersusGuid {

        public IEnumerable<Guid> EachGuid => this.Guids.Keys;

        public IEnumerable<String> EachWord => this.Words.Keys;

        /// <summary>
        /// </summary>
        /// <remarks>Two dictionaries for speed, one class to rule them all.</remarks>
        [JsonProperty]
        public ConcurrentDictionary<Guid, String> Guids { get; } = new ConcurrentDictionary<Guid, String>();

        /// <summary>
        /// </summary>
        /// <remarks>Two dictionaries for speed, one class to rule them all.</remarks>
        [JsonProperty]
        public ConcurrentDictionary<String, Guid> Words { get; } = new ConcurrentDictionary<String, Guid>();

        /// <summary>
        /// Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[String key] {
            get {
                if ( !String.IsNullOrEmpty( value: key ) ) {
                    if ( this.Words.TryGetValue( key, value: out var result ) ) { return result; }

                    var newValue = Guid.NewGuid();
                    this[key] = newValue;

                    return newValue;
                }

                return Guid.Empty;
            }

            set {
                if ( String.IsNullOrEmpty( value: key ) ) { return; }

                var guid = value;
                this.Words.AddOrUpdate( key, addValue: guid, updateValueFactory: ( s, g ) => guid );
                this.Guids.AddOrUpdate( guid, addValue: key, updateValueFactory: ( g, s ) => key );
            }
        }

        /// <summary>
        /// Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[Guid key] {
            get => Guid.Empty.Equals( g: key ) ? String.Empty : this.Guids[key];

            set {
                if ( Guid.Empty.Equals( g: key ) ) { return; }

                this.Guids.AddOrUpdate( key, addValue: value, updateValueFactory: ( g, s ) => value );
                this.Words.AddOrUpdate( value, addValue: key, updateValueFactory: ( s, g ) => key );
            }
        }

        public static void InternalTest( StringVersusGuid stringVersusGuid ) {
            var guid = new Guid( g: @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            stringVersusGuid["AIBrain"] = guid;
            stringVersusGuid[guid].Is( right: "AIBrain" ).BreakIfFalse();
        }

        public void Clear() {
            this.Words.Clear();
            this.Guids.Clear();
        }

        /// <summary>
        /// Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="daword"></param>
        /// <returns></returns>
        public Boolean Contains( String daword ) {
            if ( String.IsNullOrEmpty( value: daword ) ) { return false; }

            return this.Words.TryGetValue( daword, value: out var value );
        }

        /// <summary>
        /// Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="daguid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid daguid ) => this.Guids.TryGetValue( daguid, value: out var value );
    }
}