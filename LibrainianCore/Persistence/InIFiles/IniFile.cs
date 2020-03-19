// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "IniFile.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "IniFile.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Persistence.InIFiles {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Newtonsoft.Json;
    using OperatingSystem.FileSystem;
    using Parsing;

    /// <summary>A human readable/editable text <see cref="Document" /> with <see cref="KeyValuePair{TKey,TValue}" /> under common Sections.</summary>
    [JsonObject]
    public class IniFile {

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, IniSection> Data { [DebuggerStepThrough] get; } = new ConcurrentDictionary<String, IniSection>();

        [NotNull]
        public IEnumerable<String> Sections => this.Data.Keys;

        [CanBeNull]
        public IniSection this[ [CanBeNull] String section ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return null;
                }

                if ( !this.Data.ContainsKey( key: section ) ) {
                    return null;
                }

                if ( this.Data.TryGetValue( key: section, value: out var result ) ) {
                    return result;
                }

                return null;
            }

            set {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return;
                }

                if ( this.Data.ContainsKey( key: section ) ) {

                    //TODO merge, not overwrite
                    this.Data[ key: section ] = value;

                    return;
                }

                this.Data[ key: section ] = value;
            }
        }

        [CanBeNull]
        public String this[ [CanBeNull] String section, [CanBeNull] String key ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return default;
                }

                if ( String.IsNullOrEmpty( value: key ) ) {
                    return default;
                }

                if ( !this.Data.ContainsKey( key: section ) ) {
                    return default;
                }

                return this.Data[ key: section ].FirstOrDefault( predicate: line => line.Key.Like( right: key ) )?.Value;
            }

            [DebuggerStepThrough]
            set {
                if ( String.IsNullOrEmpty( value: section ) ) {
                    return;
                }

                if ( String.IsNullOrEmpty( value: key ) ) {
                    return;
                }

                this.Add( section: section, kvp: new KeyValuePair<String, String>( key: key, value: value ) );
            }
        }

        public const String SectionBegin = "[";

        public const String SectionEnd = "]";

        [DebuggerStepThrough]
        public IniFile( [NotNull] IDocument document ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            this.Add( document: document );
        }

        public IniFile( [NotNull] String data ) {
            if ( String.IsNullOrWhiteSpace( value: data ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( data ) );
            }

            //cheat: write out to temp file, read in, then delete temp file
            var document = Document.GetTempDocument();

            try {
                document.AppendText( text: data );
                this.Add( document: document );
            }
            finally {
                document.Delete();
            }
        }

        public IniFile() { }

        [NotNull]
        [DebuggerStepThrough]
        private static String Encode( [NotNull] IniLine line ) => $"{line ?? throw new ArgumentNullException( paramName: nameof( line ) )}";

        [NotNull]
        [DebuggerStepThrough]
        private static String Encode( [NotNull] String section ) => $"{SectionBegin}{section.TrimStart()}{SectionEnd}";

        [CanBeNull]
        private IniSection EnsureDataSection( [NotNull] String section ) {
            if ( String.IsNullOrEmpty( value: section ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( section ) );
            }

            lock ( this.Data ) {
                if ( !this.Data.ContainsKey( key: section ) ) {
                    this.Data[ key: section ] = new IniSection();
                }

                return this.Data[ key: section ];
            }
        }

        private Boolean FindComment( [NotNull] String line, [NotNull] String section, ref Int32 counter ) {
            if ( String.IsNullOrWhiteSpace( value: line ) ) {
                return default;
            }

            if ( String.IsNullOrWhiteSpace( value: section ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( section ) );
            }

            if ( line.StartsWith( value: IniLine.CommentHeader ) && this.Add( section: section, kvp: new KeyValuePair<String, String>( key: line, value: default ) ) ) {
                counter++;

                return true;
            }

            return default;
        }

        private Int32 FindKVLine( [NotNull] String line, [NotNull] String section, Int32 counter ) {
            if ( line is null ) {
                throw new ArgumentNullException( paramName: nameof( line ) );
            }

            if ( String.IsNullOrWhiteSpace( value: section ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( section ) );
            }

            if ( line.Contains( value: IniLine.PairSeparator ) ) {
                var pos = line.IndexOf( value: IniLine.PairSeparator, comparisonType: StringComparison.Ordinal );
                var key = line.Substring( startIndex: 0, length: pos ).Trimmed();

                if ( !String.IsNullOrEmpty( value: key ) ) {
                    var value = line.Substring( startIndex: pos + IniLine.PairSeparator.Length ).Trimmed();

                    if ( this.Add( section: section, key: key, value: value ) ) {
                        counter++;
                    }
                }
            }

            return counter;
        }

        private Boolean FindSection( [NotNull] String line, [CanBeNull] out String section ) {
            line = line.Trimmed();

            if ( String.IsNullOrWhiteSpace( value: line ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( line ) );
            }

            if ( line.StartsWith( value: SectionBegin ) && line.EndsWith( value: SectionEnd ) ) {
                section = line.Substring( startIndex: SectionBegin.Length, length: line.Length - ( SectionBegin.Length + SectionEnd.Length ) ).Trimmed();

                if ( !String.IsNullOrEmpty( value: section ) ) {
                    return true;
                }
            }

            section = default;

            return default;
        }

        private Boolean WriteSection( [NotNull] IDocument document, [NotNull] String section ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            if ( section is null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            if ( !this.Data.TryGetValue( key: section, value: out var dict ) ) {
                return default; //section not found
            }

            try {
                using ( var writer = File.AppendText( path: document.FullPath ) ) {
                    writer.WriteLine( value: Encode( section: section ) );

                    foreach ( var pair in dict.OrderBy( keySelector: pair => pair.Key ) ) {
                        writer.WriteLine( value: Encode( line: pair ) );
                    }

                    writer.WriteLine( value: String.Empty );
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        private async Task<Boolean> WriteSectionAsync( [NotNull] IDocument document, [NotNull] String section ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            if ( section is null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            try {
                if ( !this.Data.TryGetValue( key: section, value: out var dict ) ) {
                    return default; //section not found
                }

                await using ( var writer = File.AppendText( path: document.FullPath ) ) {
                    await writer.WriteLineAsync( value: Encode( section: section ) ).ConfigureAwait( continueOnCapturedContext: false );

                    foreach ( var pair in dict.OrderBy( keySelector: pair => pair.Key ) ) {
                        await writer.WriteLineAsync( value: Encode( line: pair ) ).ConfigureAwait( continueOnCapturedContext: false );
                    }
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        public Boolean Add( [NotNull] String section, [NotNull] String key, [CanBeNull] String value ) {
            section = section.Trimmed();

            if ( String.IsNullOrEmpty( value: section ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( section ) );
            }

            key = key.Trimmed();

            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( key ) );
            }

            var retries = 10;
            TryAgain:

            try {
                var dataSection = this.EnsureDataSection( section: section );

                var found = dataSection.FirstOrDefault( predicate: line => line?.Key.Like( right: key ) == true );

                if ( found == default ) {
                    dataSection.Add( key: key, value: value );
                }
                else {
                    found.Value = value;
                }

                return true;

            }
            catch ( KeyNotFoundException exception ) {
                exception.Log();
                retries--;

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }

            return default;
        }

        public Boolean Add( [NotNull] String section, (String key, String value) tuple ) => this.Add( section: section, key: tuple.key, value: tuple.value );

        public Boolean Add( String section, KeyValuePair<String, String> kvp ) {
            if ( String.IsNullOrEmpty( value: section ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( section ) );
            }

            if ( String.IsNullOrEmpty( value: kvp.Key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( section ) );
            }

            return this.Add( section: section, key: kvp.Key, value: kvp.Value );
        }

        [DebuggerStepThrough]
        public Boolean Add( [NotNull] IDocument document ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            if ( document.Exists() == false ) {
                return default;
            }

            try {
                var lines = File.ReadLines( path: document.FullPath ).Where( predicate: line => !String.IsNullOrWhiteSpace( value: line ) );

                return this.Add( lines: lines );
            }
            catch ( IOException exception ) {

                //file in use by another app
                exception.Log();

                return default;
            }
            catch ( OutOfMemoryException exception ) {

                //file is big-huge! As my daughter would say.
                exception.Log();

                return default;
            }
        }

        public Boolean Add( [NotNull] String text ) {
            if ( text is null ) {
                throw new ArgumentNullException( paramName: nameof( text ) );
            }

            text = text.Replace( oldValue: Environment.NewLine, newValue: "\n" );

            var lines = text.Split( separator: new[] {
                '\n'
            }, options: StringSplitOptions.RemoveEmptyEntries );

            return this.Add( lines: lines );
        }

        public Boolean Add( [NotNull] IEnumerable<String> lines ) {
            if ( lines is null ) {
                throw new ArgumentNullException( paramName: nameof( lines ) );
            }

            var counter = 0;

            foreach ( var line in lines.Select( selector: s => s?.Trimmed() ).Where( predicate: line => !line.IsNullOrEmpty() ) ) {

                if ( this.FindSection( line: line, section: out var section ) ) {
                    if ( section != default ) {
                        continue;
                    }
                }

                if ( section != null && this.FindComment( line: line, section: section, counter: ref counter ) ) {
                    continue;
                }

                if ( section != null ) {
                    counter = this.FindKVLine( line: line, section: section, counter: counter );
                }
            }

            return counter.Any();
        }

        /// <summary>Return the entire structure as a JSON formatted String.</summary>
        /// <returns></returns>
        [NotNull]
        public String AsJSON() {
            var tempDocument = Document.GetTempDocument();

            var writer = File.CreateText( path: tempDocument.FullPath );

            using ( JsonWriter jw = new JsonTextWriter( textWriter: writer ) ) {
                jw.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize( jsonWriter: jw, value: this.Data );
            }

            var text = File.ReadAllText( path: tempDocument.FullPath );

            return text;
        }

        /// <summary>Removes all data from all sections.</summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( source: this.Data.Keys, body: section => this.TryRemove( section: section ) );

            return !this.Data.Keys.Any();
        }

        /// <summary>Save the data to the specified document, overwriting it by default.</summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Boolean Save( [NotNull] IDocument document, Boolean overwrite = true ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return default;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( keySelector: section => section ) ) {
                this.WriteSection( document: document, section: section );
            }

            return true;
        }

        /// <summary>Save the data to the specified document, overwriting it by default.</summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task<Boolean> SaveAsync( [NotNull] IDocument document, Boolean overwrite = true ) {
            if ( document is null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return default;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( keySelector: section => section ) ) {
                await this.WriteSectionAsync( document: document, section: section ).ConfigureAwait( continueOnCapturedContext: false );
            }

            return default;
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( [NotNull] String section ) {
            if ( section is null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            return this.Data.TryRemove( key: section, value: out var dict );
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( [NotNull] String section, [NotNull] String key ) {

            if ( String.IsNullOrWhiteSpace( value: section ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( section ) );
            }

            if ( String.IsNullOrWhiteSpace( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( key ) );
            }

            if ( !this.Data.ContainsKey( key: section ) ) {
                return default;
            }

            return this.Data[ key: section ].Remove( key: key );
        }

    }

}