// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PersistTable.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/PersistTable.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using FileSystem;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Measurement.Time;
    using Microsoft.Database.Isam.Config;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Microsoft.Isam.Esent.Interop.Windows81;
    using Newtonsoft.Json;
    using OperatingSystem.Compression;
    using Parsing;

    /// <summary>
    ///     <para>
    ///         Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist almost any object by using
    ///         Newtonsoft.Json.
    ///     </para>
    /// </summary>
    /// <seealso cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public sealed class PersistTable<TKey, TValue> : ABetterClassDispose, IDictionary<TKey, TValue> where TKey : IComparable<TKey> {

        /// <summary>
        ///     No path given?
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private PersistTable() => throw new NotImplementedException();

        /// <summary>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="tableName">    </param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Environment.SpecialFolder specialFolder, String tableName ) : this( folder: new Folder( specialFolder: specialFolder, applicationName: null, subFolder: tableName ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="folder">   </param>
        /// <param name="tableName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Folder folder, String tableName ) : this( fullpath: Path.Combine( path1: folder.FullName, path2: tableName ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="folder">                </param>
        /// <param name="testForReadWriteAccess"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [CanBeNull] Folder folder, Boolean testForReadWriteAccess = false ) {
            try {
                this.Folder = folder ?? throw new ArgumentNullException( nameof( folder ) );

                if ( !this.Folder.Create() ) { throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." ); }

                var customConfig = new DatabaseConfig { CreatePathIfNotExist = true, EnableShrinkDatabase = ShrinkDatabaseGrbit.On, DefragmentSequentialBTrees = true };
                this.Dictionary = new PersistentDictionary<TKey, String>( directory: this.Folder.FullName, customConfig: customConfig );

                if ( testForReadWriteAccess && !this.TestForReadWriteAccess() ) { throw new IOException( $"Read/write permissions denied in folder {this.Folder.FullName}." ); }
            }
            catch ( Exception exception ) { exception.More(); }
        }

        /// <summary>
        /// </summary>
        /// <param name="fullpath"></param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] String fullpath ) : this( folder: new Folder( fullPath: fullpath ) ) { }

        [JsonProperty]
        [NotNull]
        private PersistentDictionary<TKey, String> Dictionary { get; }

        [NotNull]
        public Folder Folder { get; }

        public Int32 Count => this.Dictionary.Count;

        public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

        public ICollection<TKey> Keys => this.Dictionary.Keys;

        public ICollection<TValue> Values => this.Dictionary.Values.Select( selector: value => value.FromCompressedBase64().FromJSON<TValue>() ) as ICollection<TValue> ?? new Collection<TValue>();

        [CanBeNull]
        public TValue this[[NotNull] TKey key] {
            [CanBeNull]
            get {
                if ( key == null ) { throw new ArgumentNullException( paramName: nameof( key ) ); }

                if ( !this.Dictionary.TryGetValue( key, out var storedValue ) ) { return default; }

                return storedValue.FromCompressedBase64().FromJSON<TValue>();
            }

            set {
                if ( key == null ) { throw new ArgumentNullException( paramName: nameof( key ) ); }

                if ( value == null ) {
                    this.Dictionary.Remove( key );

                    return;
                }

                this.Dictionary[key] = value.ToJSON().ToCompressedBase64();
            }
        }

        /// <summary>
        ///     Return true if we can read/write in the <see cref="Folder" /> .
        /// </summary>
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

        public void Add( TKey key, TValue value ) => this[key] = value;

        public void Add( KeyValuePair<TKey, TValue> item ) => this[item.Key] = item.Value;

        public void Clear() => this.Dictionary.Clear();

        public Boolean Contains( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON().ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( item.Key, value );

            return this.Dictionary.Contains( item: asItem );
        }

        public Boolean ContainsKey( TKey key ) => this.Dictionary.ContainsKey( key );

        public void CopyTo( KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

        /// <summary>
        ///     Dispose any disposable managed fields or properties.
        /// </summary>
        public override void DisposeManaged() {
            using ( this.Dictionary ) { }

            base.DisposeManaged();
        }

        public void Flush() => this.Dictionary.Flush();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.Items().GetEnumerator();

        public void Initialize() {
            Logging.Enter();
            this.Dictionary.Database.Should().NotBeNull();

            if ( this.Dictionary.Database.ToString().IsNullOrWhiteSpace() ) { throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." ); }

            Logging.Exit();
        }

        /// <summary>
        ///     All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="TValue" /> deserialized.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> Items() => this.Dictionary.Select( selector: pair => new KeyValuePair<TKey, TValue>( pair.Key, pair.Value.FromCompressedBase64().FromJSON<TValue>() ) );

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </summary>
        /// <returns>
        ///     true if the element is successfully removed; otherwise, false. This method also returns false if
        ///     <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is
        ///     read-only.
        /// </exception>
        public Boolean Remove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </summary>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ; otherwise, false. This method also returns false if
        ///     <paramref name="item" /> is not
        ///     found in the original <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" /> .</param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        public Boolean Remove( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON().ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( item.Key, value );

            return this.Dictionary.Remove( item: asItem );
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{this.Count} items";

        public void TryAdd( TKey key, TValue value ) {
            if ( !this.Dictionary.ContainsKey( key ) ) { this[key] = value; }
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        ///     true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
        ///     element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">  The key whose value to get.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found; otherwise, the default
        ///     value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public Boolean TryGetValue( TKey key, out TValue value ) {
            value = default;

            if ( !this.Dictionary.TryGetValue( key, out var storedValue ) ) { return false; }

            value = storedValue.FromCompressedBase64().FromJSON<TValue>();

            return true;
        }

        public Boolean TryRemove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}