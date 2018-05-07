// Copyright 2018 Protiguous.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Ini.cs" was last cleaned by Protiguous on 2018/05/06 at 6:19 PM

namespace Librainian.Persistence {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using FileSystem;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    ///     Persist <see cref="Section" /> to and from a JSON formatted text <see cref="Document" />.
    /// </summary>
    [JsonObject]
    public class Ini : IEquatable<Ini> {

        [JsonProperty]
        public Guid ID { get; }

        public Ini( Guid id ) => this.ID = id;

        public Ini() => this.ID = Guid.NewGuid();

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public Boolean Equals( Ini other ) => Equals( this, other );

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object obj ) => Equals( this, obj as Ini );

        /// <summary>
        /// static comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Ini left, Ini right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            if ( left.AllID().Except( right.AllID() ).Any() ) {
                return false;
            }

            //var keysame = ( left.Data.Keys as IList<String> ).ContainSameElements( right.Data.Keys as IList<String> );
            //if ( !keysame ) {
            //    return false;
            //}

            return true;
        }

        public IReadOnlyList<Guid> AllID() => this.Data.Values.Select( section => section.ID ) as IReadOnlyList<Guid>;

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Data.GetHashCode();

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Librainian.Persistence.Ini" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( Ini left, Ini right ) => Equals( left, right );

        /// <summary>Returns a value that indicates whether two <see cref="T:Librainian.Persistence.Ini" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( Ini left, Ini right ) => !Equals( left, right );

        /// <summary>
        ///     Persist a document to and from a JSON formatted text document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="cancellationSource"></param>
        public Ini( [NotNull] Document document, CancellationTokenSource cancellationSource = null ) {
            this.Document = document ?? throw new ArgumentNullException( nameof( document ) );
            if ( !this.Document.Folder.Exists() ) {
                this.Document.Folder.Create();
            }

            if ( cancellationSource is null ) {
                cancellationSource = new CancellationTokenSource( delay: Seconds.Ten );
            }

            this.Reload().Wait( cancellationToken: cancellationSource.Token );
        }

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<String, Section> Data { get; } = new ConcurrentDictionary<String, Section>();

        /// <summary>
        /// </summary>
        [JsonProperty]
        [CanBeNull]
        public Document Document { get; set; }

        public IEnumerable<Section> Sections => this.Data.Values;

        public Section this[ [NotNull] String key ] {
            [DebuggerStepThrough]
            [CanBeNull] get {
                if ( key is null ) {
                    throw new ArgumentNullException( paramName: nameof( key ) );
                }

                return this.Data.TryGetValue(key, value: out var section ) ? section : null;
            }

            [DebuggerStepThrough]
            set {
                if ( key == null ) {
                    throw new ArgumentNullException( paramName: nameof( key ) );
                }

                this.Data[ key ] = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sectionKey"></param>
        /// <param name="dataKey">    </param>
        /// <returns></returns>
        public String this[ [NotNull] String sectionKey, [NotNull] String dataKey ] {

            [DebuggerStepThrough]
            [CanBeNull]
            get {
                if ( sectionKey is null ) {
                    throw new ArgumentNullException( paramName: nameof( sectionKey ) );
                }

                if ( dataKey is null ) {
                    throw new ArgumentNullException( paramName: nameof( dataKey ) );
                }

                return this.Data[ sectionKey ]?[ dataKey ];
            }

            [DebuggerStepThrough]
            set {
                if ( sectionKey is null ) {
                    throw new ArgumentNullException( paramName: nameof( sectionKey ) );
                }

                if ( dataKey is null ) {
                    throw new ArgumentNullException( paramName: nameof( dataKey ) );
                }

                var data = new Section();
                data[

                this.Add( section: sectionKey, pair: new KeyValuePair<String, String>(dataKey, value: value ) );
            }
        }

        /// <summary>
        ///     (Trims whitespaces from section and key)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="pair">   </param>
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
            if ( !this.Data.ContainsKey(section ) ) {
                this.Data.TryAdd(section, value: new ConcurrentDictionary<String, String>() );
            }

            try {
                this.Data[section ][pair.Key.Trim() ] = pair.Value;
                return true;
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

        /// <summary>
        ///     <para>Add in all of the sections, and key-value-pairs from the <see cref="INIFile" />.</para>
        ///     <para>Performs a file save.</para>
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public Boolean Add( INIFile iniFile ) {
            if ( null == iniFile ) {
                return false;
            }

            Parallel.ForEach( source: iniFile.Sections.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive, body: section => {
                var dictionary = iniFile[ section: section ];
                if ( null != dictionary ) {
                    Parallel.ForEach( source: dictionary.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive, body: pair => { this.Add( section: section, pair: pair ); } );
                }
            } );

            return true;
        }

        /// <summary>
        ///     Removes all data from all sections.
        /// </summary>
        /// <returns></returns>
        public Boolean Clear() {
            Parallel.ForEach( source: this.Data.Keys, body: section => { this.TryRemove( section: section ); } );
            return !this.Data.Keys.Any();
        }

        public Task<Boolean> Reload( CancellationToken cancellationToken = default ) {
            var document = this.Document;

            return Task.Run( function: () => {
                if ( document is null ) {
                    return false;
                }

                if ( !document.Exists() ) {
                    return false;
                }

                try {
                    var data = document.LoadJSON<ConcurrentDictionary<String, ConcurrentDictionary<String, String>>>();
                    if ( data is null ) {
                        return false;
                    }

                    var result = Parallel.ForEach( source: data.Keys.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive,
                        body: section => {
                            Parallel.ForEach( source: data[section ].Keys.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive,
                                body: key => { this.Add( section: section, pair: new KeyValuePair<String, String>(key, value: data[section ][key ] ) ); } );
                        } );
                    return result.IsCompleted;
                }
                catch ( JsonException exception ) {
                    exception.More();
                }
                catch ( IOException exception ) {
                    //file in use by another app
                    exception.More();
                }
                catch ( OutOfMemoryException exception ) {
                    //file is huge
                    exception.More();
                }

                return false;
            }, cancellationToken: cancellationToken );
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.Sections.Count()} sections, {this.AllKeys.Count()} keys";

        [DebuggerStepThrough]
        public Boolean TryRemove( String section ) {
            if ( section is null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            return this.Data.TryRemove(section, value: out var dict );
        }

        [DebuggerStepThrough]
        public Boolean TryRemove( String section, String key ) {
            if ( section is null ) {
                throw new ArgumentNullException( paramName: nameof( section ) );
            }

            if ( !this.Data.ContainsKey(section ) ) {
                return false;
            }

            return this.Data[section ].TryRemove(key, value: out _ );
        }

        /// <summary>
        ///     Saves the <see cref="Data" /> to the <see cref="Document" />.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [NotNull]
        public Task<Boolean> Write( CancellationToken cancellationToken = default ) {
            var document = this.Document;

            return Task.Run( function: () => {
                if ( document is null ) {
                    return false;
                }

                if ( document.Exists() ) {
                    document.Delete();
                }

                return this.Data.Save( document: document, overwrite: true, formatting: Formatting.Indented );
            }, cancellationToken: cancellationToken );
        }
    }
}