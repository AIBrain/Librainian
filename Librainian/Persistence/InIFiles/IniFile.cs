// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IniFile.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "IniFile.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

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

        public const String SectionBegin = "[";

        public const String SectionEnd = "]";

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, IniSection> Data { [DebuggerStepThrough] get; } = new ConcurrentDictionary<String, IniSection>();

        [NotNull]
        public IEnumerable<String> Sections => this.Data.Keys;

        [CanBeNull]
        public IniSection this[ [CanBeNull] String? section ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( section ) ) {
                    return null;
                }

                if ( !this.Data.ContainsKey( section ) ) {
                    return null;
                }

                if ( this.Data.TryGetValue( section, out var result ) ) {
                    return result;
                }

                return null;
            }

            set {
                if ( String.IsNullOrEmpty( section ) ) {
                    return;
                }

                if ( this.Data.ContainsKey( section ) ) {

                    //TODO merge, not overwrite
                    this.Data[ section ] = value;

                    return;
                }

                this.Data[ section ] = value;
            }
        }

        [CanBeNull]
        public String this[ [CanBeNull] String? section, [CanBeNull] String? key ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( section ) ) {
                    return default;
                }

                if ( String.IsNullOrEmpty( key ) ) {
                    return default;
                }

                if ( !this.Data.ContainsKey( section ) ) {
                    return default;
                }

                return this.Data[ section ].FirstOrDefault( line => line.Key.Like( key ) )?.Value;
            }

            [DebuggerStepThrough]
            set {
                if ( String.IsNullOrEmpty( section ) ) {
                    return;
                }

                if ( String.IsNullOrEmpty( key ) ) {
                    return;
                }

                this.Add( section, new KeyValuePair<String, String>( key, value ) );
            }
        }

        [DebuggerStepThrough]
        public IniFile( [NotNull] IDocument document ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            this.Add( document );
        }

        public IniFile( [NotNull] String data ) {
            if ( String.IsNullOrWhiteSpace( data ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( data ) );
            }

            //cheat: write out to temp file, read in, then delete temp file
            var document = Document.GetTempDocument();

            try {
                document.AppendText( data );
                this.Add( document );
            }
            finally {
                document.Delete();
            }
        }

        public IniFile() { }

        [NotNull]
        [DebuggerStepThrough]
        private static String Encode( [NotNull] IniLine line ) => $"{line ?? throw new ArgumentNullException( nameof( line ) )}";

        [NotNull]
        [DebuggerStepThrough]
        private static String Encode( [NotNull] String section ) => $"{SectionBegin}{section.TrimStart()}{SectionEnd}";

        [CanBeNull]
        private IniSection? EnsureDataSection( [NotNull] String section ) {
            if ( String.IsNullOrEmpty( section ) ) {
                throw new ArgumentException( "Value cannot be null or empty.", nameof( section ) );
            }

            lock ( this.Data ) {
                if ( !this.Data.ContainsKey( section ) ) {
                    this.Data[ section ] = new IniSection();
                }

                return this.Data[ section ];
            }
        }

        private Boolean FindComment( [NotNull] String line, [NotNull] String section, ref Int32 counter ) {
            if ( String.IsNullOrWhiteSpace( line ) ) {
                return default;
            }

            if ( String.IsNullOrWhiteSpace( section ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( section ) );
            }

            if ( line.StartsWith( IniLine.CommentHeader ) && this.Add( section, new KeyValuePair<String, String>( line, default ) ) ) {
                counter++;

                return true;
            }

            return default;
        }

        private Int32 FindKVLine( [NotNull] String line, [NotNull] String section, Int32 counter ) {
            if ( line is null ) {
                throw new ArgumentNullException( nameof( line ) );
            }

            if ( String.IsNullOrWhiteSpace( section ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( section ) );
            }

            if ( line.Contains( IniLine.PairSeparator ) ) {
                var pos = line.IndexOf( IniLine.PairSeparator, StringComparison.Ordinal );
                var key = line.Substring( 0, pos ).Trimmed();

                if ( !String.IsNullOrEmpty( key ) ) {
                    var value = line.Substring( pos + IniLine.PairSeparator.Length ).Trimmed();

                    if ( this.Add( section, key, value ) ) {
                        counter++;
                    }
                }
            }

            return counter;
        }

        private Boolean FindSection( [NotNull] String line, [CanBeNull] out String section ) {
            line = line.Trimmed();

            if ( String.IsNullOrWhiteSpace( line ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( line ) );
            }

            if ( line.StartsWith( SectionBegin ) && line.EndsWith( SectionEnd ) ) {
                section = line.Substring( SectionBegin.Length, line.Length - ( SectionBegin.Length + SectionEnd.Length ) ).Trimmed();

                if ( !String.IsNullOrEmpty( section ) ) {
                    return true;
                }
            }

            section = default;

            return default;
        }

        private Boolean WriteSection( [NotNull] IDocument document, [NotNull] String section ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            if ( !this.Data.TryGetValue( section, out var dict ) || dict is null ) {
                return default; //section not found
            }

            try {
                using ( var writer = File.AppendText( document.FullPath ) ) {
                    writer.WriteLine( Encode( section ) );

                    foreach ( var pair in dict.OrderBy( pair => pair?.Key ) ) {
                        if ( pair != null ) {
                            writer.WriteLine( Encode( pair ) );
                        }
                    }

                    writer.WriteLine( String.Empty );
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
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            try {
                if ( !this.Data.TryGetValue( section, out var dict ) ) {
                    return default; //section not found
                }

                using ( var writer = File.AppendText( document.FullPath ) ) {
                    await writer.WriteLineAsync( Encode( section ) ).ConfigureAwait( false );

                    foreach ( var pair in dict.OrderBy( pair => pair.Key ) ) {
                        await writer.WriteLineAsync( Encode( pair ) ).ConfigureAwait( false );
                    }
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        public Boolean Add( [NotNull] String section, [NotNull] String key, [CanBeNull] String? value ) {
            section = section.Trimmed();

            if ( String.IsNullOrEmpty( section ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( section ) );
            }

            key = key.Trimmed();

            if ( String.IsNullOrEmpty( key ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
            }

            var retries = 10;
            TryAgain:

            try {
                var dataSection = this.EnsureDataSection( section );

                if ( dataSection != default ) {
                    var found = dataSection.FirstOrDefault( line => line?.Key.Like( key ) == true );

                    if ( found == default ) {
                        dataSection.Add( key, value );
                    }
                    else {
                        found.Value = value;
                    }

                    return true;
                }
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

        public Boolean Add( [NotNull] String section, (String key, String value) tuple ) => this.Add( section, tuple.key, tuple.value );

        public Boolean Add( String section, KeyValuePair<String, String> kvp ) {
            if ( String.IsNullOrEmpty( section ) ) {
                throw new ArgumentException( "Value cannot be null or empty.", nameof( section ) );
            }

            if ( String.IsNullOrEmpty( kvp.Key ) ) {
                throw new ArgumentException( "Value cannot be null or empty.", nameof( section ) );
            }

            return this.Add( section, kvp.Key, kvp.Value );
        }

        [DebuggerStepThrough]
        public Boolean Add( [NotNull] IDocument document ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() == false ) {
                return default;
            }

            try {
                var lines = File.ReadLines( document.FullPath ).Where( line => !String.IsNullOrWhiteSpace( line ) );

                return this.Add( lines );
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
                throw new ArgumentNullException( nameof( text ) );
            }

            text = text.Replace( Environment.NewLine, "\n" );

            var lines = text.Split( new[] {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries );

            return this.Add( lines );
        }

        public Boolean Add( [NotNull] IEnumerable<String> lines ) {
            if ( lines is null ) {
                throw new ArgumentNullException( nameof( lines ) );
            }

            var counter = 0;

            foreach ( var line in lines.Select( s => s?.Trimmed() ).Where( line => !line.IsNullOrEmpty() ) ) {

                if ( this.FindSection( line, out var section ) ) {
                    if ( section != default ) {
                        continue;
                    }
                }

                if ( section != null && this.FindComment( line, section, ref counter ) ) {
                    continue;
                }

                if ( section != null ) {
                    counter = this.FindKVLine( line, section, counter );
                }
            }

            return counter.Any();
        }

        /// <summary>Return the entire structure as a JSON formatted String.</summary>
        /// <returns></returns>
        [NotNull]
        public String AsJSON() {
            var tempDocument = Document.GetTempDocument();

            var writer = File.CreateText( tempDocument.FullPath );

            using ( JsonWriter jw = new JsonTextWriter( writer ) ) {
                jw.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize( jw, this.Data );
            }

            var text = File.ReadAllText( tempDocument.FullPath );

            return text;
        }

        /// <summary>Removes all data from all sections.</summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( this.Data.Keys, section => this.TryRemove( section ) );

            return !this.Data.Keys.Any();
        }

        /// <summary>Save the data to the specified document, overwriting it by default.</summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Boolean Save( [NotNull] IDocument document, Boolean overwrite = true ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return default;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( section => section ) ) {
                this.WriteSection( document, section );
            }

            return true;
        }

        /// <summary>Save the data to the specified document, overwriting it by default.</summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task<Boolean> SaveAsync( [NotNull] IDocument document, Boolean overwrite = true ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return default;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( section => section ) ) {
                await this.WriteSectionAsync( document, section ).ConfigureAwait( false );
            }

            return default;
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( [NotNull] String section ) {
            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            return this.Data.TryRemove( section, out var dict );
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( [NotNull] String section, [NotNull] String key ) {

            if ( String.IsNullOrWhiteSpace( section ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( section ) );
            }

            if ( String.IsNullOrWhiteSpace( key ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
            }

            if ( !this.Data.ContainsKey( section ) ) {
                return default;
            }

            return this.Data[ section ].Remove( key );
        }
    }
}