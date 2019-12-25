// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PersistTable.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "PersistTable.cs" was last formatted by Protiguous on 2019/11/20 at 6:44 AM.

namespace LibrainianCore.Persistence {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Logging;
    using Maths;
    using Measurement.Time;
    using OperatingSystem.Compression;
    using OperatingSystem.FileSystem;
    using Utilities;

    /// <summary>
    ///     <para>Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist almost any object by using Newtonsoft.Json.</para>
    /// </summary>
    /// <see cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public sealed class PersistTable<TKey, TValue> : ABetterClassDispose, IDictionary<TKey, TValue> where TKey : IComparable<TKey> {

        [JsonProperty]
        [NotNull]
        private PersistentDictionary<TKey, String> Dictionary { get; }

        /// <summary>No path given?</summary>
        [NotNull]
        public Folder Folder { get; }

        public Int32 Count => this.Dictionary.Count;

        public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

        public ICollection<TKey> Keys => this.Dictionary.Keys;

        /// <summary>This deserializes the list of values.. I have a feeling this cannot be very fast.</summary>
        public ICollection<TValue> Values => ( ICollection<TValue> )this.Dictionary.Values.Select( selector: value => value.FromCompressedBase64().FromJSON<TValue>() );

        /// <summary></summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public TValue this[ [NotNull] TKey key ] {
            [CanBeNull]
            get {
                if ( key is null ) {
                    throw new ArgumentNullException( paramName: nameof( key ) );
                }

                if ( !this.Dictionary.TryGetValue( key, out var storedValue ) ) {
                    return default;
                }

                return storedValue.FromCompressedBase64().FromJSON<TValue>();
            }

            set {
                if ( key is null ) {
                    throw new ArgumentNullException( paramName: nameof( key ) );
                }

                if ( value is null ) {
                    this.Dictionary.Remove( key );

                    return;
                }

                this.Dictionary[ key ] = value.ToJSON().ToCompressedBase64();
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private PersistTable() => throw new NotImplementedException();

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Environment.SpecialFolder specialFolder, [NotNull] String tableName ) : this( folder: new Folder( specialFolder: specialFolder,
            applicationName: null, subFolder: tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Environment.SpecialFolder specialFolder, String subFolder, [NotNull] String tableName ) : this( folder: new Folder( specialFolder, subFolder,
            tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] Folder folder, [NotNull] String tableName ) : this( fullpath: Path.Combine( path1: folder.FullName, path2: tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] Folder folder, [NotNull] String subFolder, [NotNull] String tableName ) : this( fullpath: Path.Combine( folder.FullName, subFolder,
            tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [CanBeNull] Folder folder, Boolean testForReadWriteAccess = false ) {
            try {
                this.Folder = folder ?? throw new ArgumentNullException( nameof( folder ) );

                if ( !this.Folder.Create() ) {
                    throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." );
                }

                var customConfig = new DatabaseConfig {
                    CreatePathIfNotExist = true,
                    EnableShrinkDatabase = ShrinkDatabaseGrbit.On,
                    DefragmentSequentialBTrees = true
                };

                this.Dictionary = new PersistentDictionary<TKey, String>( directory: this.Folder.FullName, customConfig: customConfig );

                if ( testForReadWriteAccess && !this.TestForReadWriteAccess() ) {
                    throw new IOException( $"Read/write permissions denied in folder {this.Folder.FullName}." );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] String fullpath ) : this( folder: new Folder( fullPath: fullpath ) ) { }

        /// <summary>Return true if we can read/write in the <see cref="Folder" /> .</summary>
        /// <returns></returns>
        private Boolean TestForReadWriteAccess() {
            try {
                if ( this.Folder.TryGetTempDocument( document: out var document ) ) {
                    var text = Randem.NextString( 64, lowers: true, uppers: true, numbers: true, symbols: true );
                    document.AppendText( text: text );
                    document.TryDeleting( tryFor: Seconds.Five );

                    return true;
                }
            }
            catch { }

            return false;
        }

        public void Add( TKey key, TValue value ) => this[ key ] = value;

        public void Add( KeyValuePair<TKey, TValue> item ) => this[ item.Key ] = item.Value;

        public void Clear() => this.Dictionary.Clear();

        public Boolean Contains( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON().ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( item.Key, value );

            return this.Dictionary.Contains( asItem );
        }

        public Boolean ContainsKey( TKey key ) => this.Dictionary.ContainsKey( key );

        public void CopyTo( KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

        /// <summary>Dispose any disposable managed fields or properties.</summary>
        public override void DisposeManaged() {
            Trace.Write( $"Disposing of {nameof( this.Dictionary )}..." );

            using ( this.Dictionary ) { }

            Trace.WriteLine( "done." );
        }

        public void Flush() => this.Dictionary.Flush();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.Items().GetEnumerator();

        public void Initialize() {

            if ( this.Dictionary.Database.ToString().IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." );
            }
        }

        /// <summary>All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="TValue" /> deserialized.</summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<KeyValuePair<TKey, TValue>> Items() =>
            this.Dictionary.Select( selector: pair => new KeyValuePair<TKey, TValue>( pair.Key, pair.Value.FromCompressedBase64().FromJSON<TValue>() ) );

        /// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" /> .</summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false. This method also returns false if <paramref name="key" /> was not found in the original
        /// <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
        public Boolean Remove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" /> .</summary>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" /> ; otherwise, false. This method also returns
        /// false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" /> .</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public Boolean Remove( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON().ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( item.Key, value );

            return this.Dictionary.Remove( item: asItem );
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.Count} items";

        public void TryAdd( [NotNull] TKey key, TValue value ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            if ( !this.Dictionary.ContainsKey( key ) ) {
                this[ key ] = value;
            }
        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <returns>true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.</returns>
        /// <param name="key">  The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the
        /// <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public Boolean TryGetValue( [NotNull] TKey key, out TValue value ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            value = default;

            if ( !this.Dictionary.TryGetValue( key, out var storedValue ) ) {
                return false;
            }

            value = storedValue.FromCompressedBase64().FromJSON<TValue>();

            return true;
        }

        public Boolean TryRemove( [NotNull] TKey key ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            return this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}