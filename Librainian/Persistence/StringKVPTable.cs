// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Persistence {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using Microsoft.Database.Isam.Config;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Microsoft.Isam.Esent.Interop.Windows81;
    using Newtonsoft.Json;
    using OperatingSystem.Compression;
    using OperatingSystem.FileSystem;
    using Parsing;
    using Utilities;

    /// <summary>
    ///     <para>Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist a <see cref="KeyValuePair{TKey,TValue}" /> of base64 compressed strings.</para>
    /// </summary>
    /// <see cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public sealed class StringKVPTable : ABetterClassDispose, IDictionary<String, String> {

        public Int32 Count => this.Dictionary.Count;

        public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

        public ICollection<String> Keys => this.Dictionary.Keys;

        public ICollection<String> Values => ( ICollection<String> ) this.Dictionary.Values.Select( selector: value => value.FromCompressedBase64() );

        /// <summary></summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public String this[ [NotNull] String key ] {
            [CanBeNull]
            get {
                if ( key is null ) {
                    throw new ArgumentNullException( nameof( key ) );
                }

                return this.Dictionary.TryGetValue( key: key, value: out var storedValue ) ? storedValue.FromCompressedBase64() : default;
            }

            set {
                if ( key is null ) {
                    throw new ArgumentNullException( nameof( key ) );
                }

                if ( String.IsNullOrEmpty( value ) ) {
                    this.Dictionary.Remove( key: key );

                    return;
                }

                this.Dictionary[ key: key ] = value.ToCompressedBase64();
            }
        }

        public void Add( String key, [CanBeNull] String value ) => this[ key ] = value;

        public void Add( KeyValuePair<String, String> item ) {
            if ( item.Key != null ) {
                this[ item.Key ] = item.Value;
            }
        }

        public void Clear() => this.Dictionary.Clear();

        public Boolean Contains( KeyValuePair<String, String> item ) {
            if ( item.Value != null ) {
                var value = item.Value.ToJSON()?.ToCompressedBase64();
                var asItem = new KeyValuePair<String, String>( item.Key, value );

                return this.Dictionary.Contains( item: asItem );
            }

            return default;
        }

        public Boolean ContainsKey( String key ) => this.Dictionary.ContainsKey( key: key );

        public void CopyTo( KeyValuePair<String, String>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

        public IEnumerator<KeyValuePair<String, String>> GetEnumerator() => this.Items().GetEnumerator();

        /// <summary>Removes the element with the specified key from the <see cref="IDictionary" /> .</summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false. This method also returns false if <paramref name="key" /> was not found in the original
        /// <see cref="IDictionary" /> .
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="NotSupportedException">The <see cref="IDictionary" /> is read-only.</exception>
        public Boolean Remove( String key ) => this.Dictionary.ContainsKey( key: key ) && this.Dictionary.Remove( key: key );

        /// <summary>Removes the first occurrence of a specific object from the <see cref="ICollection" /> .</summary>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="ICollection" /> ; otherwise, false. This method also returns false if
        /// <paramref name="item" /> is not found in the original <see cref="ICollection" /> .
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="ICollection" /> .</param>
        /// <exception cref="NotSupportedException">The <see cref="ICollection" /> is read-only.</exception>
        public Boolean Remove( KeyValuePair<String, String> item ) {
            var value = item.Value.ToJSON()?.ToCompressedBase64();
            var asItem = new KeyValuePair<String, String>( item.Key, value );

            return this.Dictionary.Remove( item: asItem );
        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <returns>true if the object that implements <see cref="IDictionary" /> contains an element with the specified key; otherwise, false.</returns>
        /// <param name="key">  The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the
        /// <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public Boolean TryGetValue( [NotNull] String key, out String value ) {
            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            value = default;

            if ( this.Dictionary.TryGetValue( key: key, value: out var storedValue ) ) {
                value = storedValue.FromCompressedBase64();

                return true;
            }

            return default;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [JsonProperty]
        [NotNull]
        private PersistentDictionary<String, String> Dictionary { get; }

        /// <summary>No path given?</summary>
        [NotNull]
        public Folder Folder { get; }

        [CanBeNull]
#pragma warning disable CA1043 // Use Integral Or String Argument For Indexers
        public String this[ [NotNull] params String[] keys ] {
#pragma warning restore CA1043 // Use Integral Or String Argument For Indexers
            [CanBeNull]
            get {
                if ( keys is null ) {
                    throw new ArgumentNullException( nameof( keys ) );
                }

                var key = Cache.BuildKey( keys );

                return this.Dictionary.TryGetValue( key: key, value: out var storedValue ) ? storedValue.FromCompressedBase64() : default;
            }

            set {
                if ( keys is null ) {
                    throw new ArgumentNullException( nameof( keys ) );
                }

                var key = Cache.BuildKey( keys );

                if ( String.IsNullOrEmpty( value ) ) {
                    this.Dictionary.Remove( key: key );

                    return;
                }

                this.Dictionary[ key: key ] = value.ToCompressedBase64();
            }
        }

        private StringKVPTable() => throw new NotImplementedException();

        public StringKVPTable( Environment.SpecialFolder specialFolder, [NotNull] String tableName ) : this( new Folder( specialFolder,
            null, tableName ) ) { }

        public StringKVPTable( Environment.SpecialFolder specialFolder, [CanBeNull] String subFolder, [NotNull] String tableName ) : this(
            new Folder( specialFolder, subFolder, tableName ) ) { }

        public StringKVPTable( Byte specialFolder, [CanBeNull] String subFolder, [NotNull] String tableName ) : this(
            new Folder( ( Environment.SpecialFolder ) specialFolder, subFolder, tableName ) ) { }

        public StringKVPTable( [NotNull] Folder folder, [NotNull] String tableName ) : this( Path.Combine( folder.FullPath, tableName ) ) { }

        public StringKVPTable( [NotNull] Folder folder, [NotNull] String subFolder, [NotNull] String tableName ) : this( Path.Combine( folder.FullPath,
            subFolder, tableName ) ) { }

        public StringKVPTable( [NotNull] Folder folder, Boolean testForReadWriteAccess = false ) {
            if ( folder is null ) {
                throw new ArgumentNullException( nameof( folder ) );
            }

            try {
                this.Folder = folder;

                if ( !this.Folder.Create() ) {
                    throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullPath}`." );
                }

                var customConfig = new DatabaseConfig {
                    CreatePathIfNotExist = true, EnableShrinkDatabase = ShrinkDatabaseGrbit.On, DefragmentSequentialBTrees = true
                };

                this.Dictionary = new PersistentDictionary<String, String>( directory: this.Folder.FullPath, customConfig: customConfig );

                if ( testForReadWriteAccess && !this.TestForReadWriteAccess().Result ) {
                    throw new IOException( $"Read/write permissions denied in folder {this.Folder.FullPath}." );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        public StringKVPTable( [NotNull] String fullpath ) : this( new Folder( fullpath ) ) { }

        /// <summary>Return true if we can read/write in the <see cref="Folder" /> .</summary>
        /// <returns></returns>
        private async Task<Boolean> TestForReadWriteAccess() {
            try {
                var document = this.Folder.TryGetTempDocument();

                var text = Randem.NextString( 64, true, true, true, true );
                document.AppendText( text );

                using var cancel = new CancellationTokenSource( Seconds.Ten );

                await document.TryDeleting( Seconds.One, cancel.Token ).ConfigureAwait( continueOnCapturedContext: false );

                return !document.Exists();
            }
            catch { }

            return default;
        }

        public void Add( (String key, String value) kvp ) => this[ kvp.key ] = kvp.value;

        /// <summary>Dispose any disposable managed fields or properties.</summary>
        public override void DisposeManaged() {
            Trace.Write( $"Disposing of {nameof( this.Dictionary )}..." );

            using ( this.Dictionary ) { }

            Trace.WriteLine( "done." );
        }

        /// <summary>Force all changes to be written to disk.</summary>
        public void Flush() => this.Dictionary.Flush();

        public void Initialize() {

            if ( this.Dictionary.Database.ToString().IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullPath}`." );
            }
        }

        /// <summary>All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="String" /> deserialized.</summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<KeyValuePair<String, String>> Items() =>
            this.Dictionary.Select( selector: pair => new KeyValuePair<String, String>( key: pair.Key, value: pair.Value.FromCompressedBase64() ) );

        public void Save() => this.Flush();

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        [NotNull]
        public override String ToString() => $"{this.Count} items";

        //should be all that's needed..
        public void TryAdd( [NotNull] String key, [CanBeNull] String value ) {
            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            if ( !this.Dictionary.ContainsKey( key: key ) ) {
                this[ key ] = value;
            }
        }

        public Boolean TryRemove( [NotNull] String key ) {
            if ( key is null ) {
                throw new ArgumentNullException( nameof( key ) );
            }

            return this.Dictionary.ContainsKey( key: key ) && this.Dictionary.Remove( key: key );
        }

    }

}