// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "JSONFile.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "JSONFile.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Newtonsoft.Json;
    using OperatingSystem.FileSystem;

    /// <summary>Persist a document to and from a JSON formatted text document.</summary>
    [JsonObject]
    public class JSONFile {

        [NotNull]
        public IEnumerable<String> AllKeys => this.Sections.SelectMany( selector: section => this.Data[ key: section ].Keys );

        /// <summary></summary>
        [JsonProperty]
        [CanBeNull]
        public Document Document { get; set; }

        [NotNull]
        public IEnumerable<String> Sections => this.Data.Keys;

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, ConcurrentDictionary<String, String>> Data { [DebuggerStepThrough] get; } =
            new ConcurrentDictionary<String, ConcurrentDictionary<String, String>>();

        [CanBeNull]
        public IReadOnlyDictionary<String, String> this[ [CanBeNull] String section ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return null;
                }

                if ( !this.Data.ContainsKey( key: section ) ) {
                    return null;
                }

                return this.Data.TryGetValue( key: section, value: out var result ) ? result : null;
            }
        }

        /// <summary></summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public String this[ [CanBeNull] String section, [CanBeNull] String key ] {
            //[DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return null;
                }

                if ( String.IsNullOrEmpty( value: key ) ) {
                    return null;
                }

                if ( !this.Data.ContainsKey( key: section ) ) {
                    return null;
                }

                return this.Data[ key: section ].TryGetValue( key: key, value: out var value ) ? value : null;
            }

            //[DebuggerStepThrough]
            set {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return;
                }

                if ( String.IsNullOrEmpty( value: key ) ) {
                    return;
                }

                this.Add( section: section, pair: new KeyValuePair<String, String>( key: key, value: value ) );
            }
        }

        /// <summary></summary>
        /// <param name="document"></param>
        public JSONFile( [CanBeNull] Document document ) : this() {
            this.Document = document;

            this.Document.ContainingingFolder().Create();

            this.Read().Wait();
        }

        public JSONFile() { }

        /// <summary>Removes all data from all sections.</summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( source: this.Data.Keys, body: section => this.TryRemove( section: section ) );

            return !this.Data.Keys.Any();
        }

        [NotNull]
        public Task<Boolean> Read( CancellationToken cancellationToken = default ) {
            var document = this.Document;

            return Task.Run( function: () => {
                if ( !document.Exists() ) {
                    return false;
                }

                try {
                    var data = document.LoadJSON<ConcurrentDictionary<String, ConcurrentDictionary<String, String>>>();

                    if ( data == null ) {
                        return false;
                    }

                    var result = Parallel.ForEach( source: data.Keys.AsParallel(),
                        body: section => Parallel.ForEach( source: data[ key: section ].Keys.AsParallel().AsUnordered(),
                            body: key => this.Add( section: section, pair: new KeyValuePair<String, String>( key: key, value: data[ key: section ][ key: key ] ) ) ) );

                    return result.IsCompleted;
                }
                catch ( JsonException exception ) {
                    exception.Log();
                }
                catch ( IOException exception ) {
                    //file in use by another app
                    exception.Log();
                }
                catch ( OutOfMemoryException exception ) {
                    //file is huge
                    exception.Log();
                }

                return false;
            }, cancellationToken: cancellationToken );
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        [NotNull]
        public override String ToString() => $"{this.Sections.Count()} sections, {this.AllKeys.Count()} keys";

        [DebuggerStepThrough]
        public Boolean TryRemove( String section ) {
            if ( section == null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            return this.Data.TryRemove( key: section, value: out var dict );
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( String section, [CanBeNull] String key ) {
            if ( section == null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            if ( !this.Data.ContainsKey( key: section ) ) {
                return false;
            }

            return this.Data[ key: section ].TryRemove( key: key, value: out var value );
        }

        /// <summary>Saves the <see cref="Data" /> to the <see cref="Document" />.</summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [NotNull]
        public Task<Boolean> Write( CancellationToken cancellationToken = default ) {
            var document = this.Document;

            return Task.Run( function: () => {

                if ( document.Exists() ) {
                    document.Delete();
                }

                return this.Data.TrySave( document: document, overwrite: true, formatting: Formatting.Indented );
            }, cancellationToken: cancellationToken );
        }

        /// <summary>(Trims whitespaces from section and key)</summary>
        /// <param name="section"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        private Boolean Add( String section, KeyValuePair<String, String> pair ) {
            if ( String.IsNullOrWhiteSpace( value: section ) ) {
                throw new ArgumentException( message: "Argument is null or whitespace", paramName: nameof( section ) );
            }

            section = section.Trim();

            if ( String.IsNullOrWhiteSpace( value: section ) ) {
                throw new ArgumentException( message: "Argument is null or whitespace", paramName: nameof( section ) );
            }

            var retries = 10;
            TryAgain:

            if ( !this.Data.ContainsKey( key: section ) ) {
                this.Data.TryAdd( key: section, value: new ConcurrentDictionary<String, String>() );
            }

            try {
                this.Data[ key: section ][ key: pair.Key.Trim() ] = pair.Value;

                return true;
            }
            catch ( KeyNotFoundException exception ) {
                retries--;

                if ( retries.Any() ) {
                    goto TryAgain;
                }

                exception.Log();
            }

            return false;
        }

    }

}