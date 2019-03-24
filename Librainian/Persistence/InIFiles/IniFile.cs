// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IniFile.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "IniFile.cs" was last formatted by Protiguous on 2019/01/29 at 10:46 PM.

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

    /// <summary>
    ///     A human readable/editable text <see cref="Document" /> with <see cref="KeyValuePair{TKey,TValue}" /> under common Sections.
    /// </summary>
    [JsonObject]
    public class IniFile {

        /// <summary>
        /// If true, the section names have whitespace removed when added.
        /// </summary>
        public Boolean TrimSections { get; set; }

        /// <summary>
        /// If true, the keys have whitespace removed when added.
        /// </summary>
        public Boolean TrimKeys { get; set; }

        /// <summary>
        /// If true, the value have whitespace removed when added.
        /// </summary>
        public Boolean TrimValues { get; set; }

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, IniSection> Data { [DebuggerStepThrough] get; } = new ConcurrentDictionary<String, IniSection>();

        public const String SectionBegin = "[";

        public const String SectionEnd = "]";

        [NotNull]
        public IEnumerable<String> Sections => this.Data.Keys;

        [CanBeNull]
        public IniSection this[ [CanBeNull] String section ] {
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
        public String this[ [CanBeNull] String section, [CanBeNull] String key ] {
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
        public IniFile( [NotNull] IDocument document, Boolean trimSections=true, Boolean trimKeys=true, Boolean trimValues=true ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            this.TrimSections = trimSections;
            this.TrimKeys = trimKeys;
            this.TrimValues = trimValues;

            this.Add( document );
        }

        public IniFile( String data ) {
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

        private Boolean WriteSection( [NotNull] IDocument document, [NotNull] String section ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            if ( !this.Data.TryGetValue( section, out var dict ) ) {
                return false; //section not found
            }

            try {
                using ( var writer = File.AppendText( document.FullPath ) ) {
                    writer.WriteLine( Encode( section ) );

                    foreach ( var pair in dict.OrderBy( pair => pair.Key ) ) {
                        writer.WriteLine( Encode( pair ) );
                    }
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        private async Task<Boolean> WriteSectionAsync( [NotNull] Document document, [NotNull] String section ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            try {
                if ( !this.Data.TryGetValue( section, out var dict ) ) {
                    return false; //section not found
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

            return false;
        }

        private Boolean WriteSectionJSON( Document document, [NotNull] String section ) {

            throw new NotImplementedException();

            if ( !this.Data.TryGetValue( section, out var dict ) ) {
                return false; //section not found
            }

            try {
                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        [NotNull]
        [DebuggerStepThrough]
        public static String Encode( [NotNull] IniLine line ) {
            if ( line == null ) {
                throw new ArgumentNullException( paramName: nameof( line ) );
            }

            return $"{line}";
        }

        [NotNull]
        [DebuggerStepThrough]
        public static String Encode( [NotNull] String section ) {
            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            return $"{SectionBegin}{section.TrimStart()}{SectionEnd}";
        }

        public Boolean Add( String section, String key, String value ) => this.Add( section, new KeyValuePair<String, String>( key, value ) );

        public Boolean Add( String section, KeyValuePair<String, String> kvp ) {
            if ( String.IsNullOrWhiteSpace( section ) ) {
                throw new ArgumentException( "Argument == null or whitespace", nameof( section ) );
            }

            section = section.Trim();

            var retries = 10;
            TryAgain:

            lock ( this.Data ) {
                if ( !this.Data.ContainsKey( section ) ) {
                    this.Data[ section ] = new IniSection();
                }
            }

            try {
                var found = this.Data[ section ].FirstOrDefault(line => line.Key.Like(kvp.Key));

                if ( found == default ) {
                    this.Data[ section ].Add( kvp.Key, this.TrimValues ? kvp.Value?.Trim() : kvp.Value );
                }
                else {
                    found.Value = this.TrimValues ? kvp.Value?.Trim() : kvp.Value;
                }

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

        [DebuggerStepThrough]
        public Boolean Add( [NotNull] IDocument document ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() == false ) {
                return false;
            }

            try {
                var lines = File.ReadLines( document.FullPath ).Where( line => !String.IsNullOrWhiteSpace( line ) );

                return this.Add( lines );
            }
            catch ( IOException exception ) {

                //file in use by another app
                exception.Log();

                return false;
            }
            catch ( OutOfMemoryException exception ) {

                //file is big-huge!
                exception.Log();

                return false;
            }
        }

        public Boolean Add( String text ) {
            text = text.Replace( Environment.NewLine, "\n" );

            var lines = text.Split( new[] {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries );

            return this.Add( lines );
        }

        public Boolean Add( [NotNull] IEnumerable<String> lines ) {
            if ( lines == null ) {
                throw new ArgumentNullException( nameof( lines ) );
            }

            var counter = 0;
            var section = String.Empty;

            foreach ( var line in lines.Where( s => !s.IsNullOrEmpty() ).Select( aline => aline.Trim() ).Where( line => !line.IsNullOrWhiteSpace() ) ) {
                if ( line.StartsWith( SectionBegin ) && line.EndsWith( SectionEnd ) ) {
                    section = line.Substring( SectionBegin.Length, line.Length - ( SectionBegin.Length + SectionEnd.Length ) );

                    if ( this.TrimSections ) {
                        section = section.Trim();
                    }
                    continue;
                }

                if ( line.Contains( IniLine.PairSeparator ) ) {
                    var pos = line.IndexOf( IniLine.PairSeparator, StringComparison.Ordinal );
                    var key = line.Substring( 0, pos );

                    if ( this.TrimKeys ) {
                        key = key.Trim();
                    }

                    var value = line.Substring( pos + IniLine.PairSeparator.Length );

                    if ( this.TrimValues ) {
                        value = value.Trim();
                    }

                    if ( this.Add( section, new KeyValuePair<String, String>( key, value ) ) ) {
                        counter++;
                    }
                }
                else if ( line.StartsWith( IniLine.CommentHeader ) ) {
                    if ( this.Add( section, new KeyValuePair<String, String>( line, default ) ) ) {
                        counter++;
                    }
                }
            }

            return counter.Any();
        }

        /// <summary>
        ///     Return the entire structure as a JSON formatted String.
        /// </summary>
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

        /// <summary>
        ///     Removes all data from all sections.
        /// </summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( this.Data.Keys, section => this.TryRemove( section ) );

            return !this.Data.Keys.Any();
        }

        /// <summary>
        ///     Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Boolean Save( [NotNull] IDocument document, Boolean overwrite = true ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() == true ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return false;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( section => section ) ) {
                this.WriteSection( document, section );
            }

            return true;
        }

        /// <summary>
        ///     Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"> </param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task<Boolean> SaveAsync( [NotNull] Document document, Boolean overwrite = true ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() == true ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return false;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( section => section ) ) {
                await this.WriteSectionAsync( document, section ).ConfigureAwait( false );
            }

            return false;
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( [NotNull] String section ) {
            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            return this.Data.TryRemove( section, out var dict );
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( [NotNull] String section, String key ) {
            if ( section == null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            if ( !this.Data.ContainsKey( section ) ) {
                return false;
            }

            return this.Data[ section ].Remove( key );
        }

    }

}