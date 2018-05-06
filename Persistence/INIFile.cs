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
// "Librainian/INIFile.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FileSystem;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;

    /// <summary>
    /// </summary>
    [JsonObject]
    public class INIFile {
        public const String PairSeparator = "=";
        public const String SectionBegin = "[";

        public const String SectionEnd = "]";

        public INIFile( String data ) {

            //cheat: write out to temp file, read in, then delete temp file
            var document = Document.GetTempDocument();
            document.AppendText( data );
            this.Add( document );
            this.AutoSaveDocument = null;
            document.Delete();
        }

        /// <summary>
        ///     Entire document dictionary is saved on any change.
        /// </summary>
        /// <param name="autoSaveDocument"></param>
        public INIFile( Document autoSaveDocument ) : this() {
            this.AutoSaveDocument = autoSaveDocument;
            this.Add( autoSaveDocument );
        }

        public INIFile() {
        }

        /// <summary>
        ///     <para>WARNING: Set this value AFTER <see cref="Add(Document)" />.</para>
        ///     <para>If <see cref="AutoSaveDocument" /> is set, the entire dictionary/text is saved on each change.</para>
        /// </summary>
        [JsonProperty]
        public Document AutoSaveDocument {
            get; set;
        }

        public IEnumerable<String> Sections => this.Data.Keys;

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, ConcurrentDictionary<String, String>> Data { [DebuggerStepThrough] get; } = new ConcurrentDictionary<String, ConcurrentDictionary<String, String>>();

        public IReadOnlyDictionary<String, String> this[ [CanBeNull] String section ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( section ) ) {
                    return null;
                }
                if ( !this.Data.ContainsKey( section ) ) {
                    return null;
                }
				return this.Data.TryGetValue( section, out var result ) ? result : null;
			}
        }

        /// <summary>
        ///     If <see cref="AutoSaveDocument" /> is set, the entire dictionary/text is saved on each change.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[ [CanBeNull] String section, [CanBeNull] String key ] {
            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( String.IsNullOrEmpty( section ) ) {
                    return null;
                }
                if ( String.IsNullOrEmpty( key ) ) {
                    return null;
                }

                if ( !this.Data.ContainsKey( section ) ) {
                    return null;
                }


				return this.Data[ section ].TryGetValue( key, out var value ) ? value : null;
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

                if ( null != this.AutoSaveDocument ) {
                    this.Save( this.AutoSaveDocument );
                }
            }
        }

        [DebuggerStepThrough]
        public static String EncodePair( KeyValuePair<String, String> pair ) => $"{pair.Key}{PairSeparator}{pair.Value ?? String.Empty}{Environment.NewLine}";

	    [DebuggerStepThrough]
        public static String EncodeSection( String section ) {
            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }
            return $"{SectionBegin}{section.Trim()}{SectionEnd}{Environment.NewLine}";
        }

        /// <summary>
        ///     (Trims whitespaces from section, key, and value.)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public Boolean Add( String section, KeyValuePair<String, String> kvp ) {
            if ( String.IsNullOrWhiteSpace( section ) ) {
                throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
            }
            section = section.Trim();

            var retries = 10;
            TryAgain:
            lock ( this.Data ) {
                if ( !this.Data.ContainsKey( section ) ) {
                    this.Data[ section ] = new ConcurrentDictionary<String, String>();
                }
            }
            try {
                this.Data[ section ][ kvp.Key.Trim() ] = kvp.Value.Trim();
                return null == this.AutoSaveDocument || this.Save( this.AutoSaveDocument );
            }
            catch ( KeyNotFoundException exception ) {
                retries--;
                if ( retries.Any() ) {
                    goto TryAgain;
                }
                exception.More();
            }
            return false;
        }

        public Boolean Add( Document document ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            if ( !document.Exists() ) {
                return false;
            }

            try {
                var lines = File.ReadLines( document.FullPathWithFileName ).Where( line => !String.IsNullOrWhiteSpace( line ) );

                //.ToList();

                return this.Add( lines );
            }
            catch ( IOException exception ) {

                //file in use by another app
                exception.More();
                return false;
            }
            catch ( OutOfMemoryException exception ) {

                //file is huge
                exception.More();
                return false;
            }
        }

        public Boolean Add(  String text ) {
	        text = text.Replace( Environment.NewLine, "\n" );

            var lines = text.Split( new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries );

            return Add( lines );
        }

        public Boolean Add( [NotNull] IEnumerable<String> lines ) {
            if ( lines is null ) {
                throw new ArgumentNullException( nameof( lines ) );
            }
            var counter = 0;
            var section = String.Empty;

            foreach ( var line in lines.Where( s => !s.IsNullOrEmpty() ).Select( aline => aline.Trim() ).Where( line => !line.IsNullOrWhiteSpace() ) ) {
                if ( line.StartsWith( SectionBegin ) && line.EndsWith( SectionEnd ) ) {
                    section = line.Substring( SectionBegin.Length, line.Length - ( SectionBegin.Length + SectionEnd.Length ) ).Trim();
                    continue;
                }

                if ( line.Contains( PairSeparator ) ) {
                    var pos = line.IndexOf( PairSeparator, StringComparison.Ordinal );
                    var key = line.Substring( 0, pos ).Trim();
                    var value = line.Substring( pos + PairSeparator.Length );
                    if ( this.Add( section, new KeyValuePair<String, String>( key, value ) ) ) {
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
        public String AsJSON() {
            var tempDocument = Document.GetTempDocument();

            var writer = File.CreateText( tempDocument.FullPathWithFileName );

            using ( JsonWriter jw = new JsonTextWriter( writer ) ) {
                jw.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize( jw, this.Data );
            }

            var text = File.ReadAllText( tempDocument.FullPathWithFileName );

            return text;
        }

        /// <summary>
        ///     Removes all data from all sections.
        /// </summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( this.Data.Keys, section => {
                TryRemove( section );
            } );
            return !this.Data.Keys.Any();
        }

        /// <summary>
        ///     Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public Boolean Save( Document document, Boolean overwrite = true ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return false;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( section => section ) ) {
                WriteSection( document, section );
            }

            return true;
        }

        /// <summary>
        ///     Save the data to the specified document, overwriting it by default.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public async Task<Boolean> SaveAsync( Document document, Boolean overwrite = true ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            if ( document.Exists() ) {
                if ( overwrite ) {
                    document.Delete();
                }
                else {
                    return false;
                }
            }

            foreach ( var section in this.Data.Keys.OrderBy( section => section ) ) {
                await this.WriteSectionAsync( document, section );
            }

            return false;
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( String section ) {
            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }
			return this.Data.TryRemove( section, out var dict );
		}

        [DebuggerStepThrough]
        public Boolean TryRemove( String section, String key ) {
            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }
            if ( !this.Data.ContainsKey( section ) ) {
                return false;
            }
			return this.Data[ section ].TryRemove( key, out var value );
		}

        private Boolean WriteSection( Document document, String section ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

			if ( !this.Data.TryGetValue( section, out var dict ) ) {
				return false; //section not found
			}

			try {
                using ( var writer = File.AppendText( document.FullPathWithFileName ) ) {
                    writer.Write( EncodeSection( section ) );
                    foreach ( var pair in dict.OrderBy( pair => pair.Key ) ) {
                        writer.Write( EncodePair( pair ) );
                    }
                    writer.Write( Environment.NewLine );
                    writer.Flush();
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return false;
        }

        private async Task<Boolean> WriteSectionAsync( Document document, String section ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            if ( section is null ) {
                throw new ArgumentNullException( nameof( section ) );
            }

            try {
				if ( !this.Data.TryGetValue( section, out var dict ) ) {
					return false; //section not found
				}

				using ( var writer = File.AppendText( document.FullPathWithFileName ) ) {
                    writer.Write( EncodeSection( section ) );
                    foreach ( var pair in dict.OrderBy( pair => pair.Key ) ) {
                        await writer.WriteAsync( EncodePair( pair ) );
                    }
                    await writer.WriteLineAsync();
                    await writer.FlushAsync();
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return false;
        }

        private Boolean WriteSectionJSON( Document document, String section ) {
			if ( !this.Data.TryGetValue( section, out var dict ) ) {
				return false; //section not found
			}

			try {
                return true;
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return false;
        }
    }
}