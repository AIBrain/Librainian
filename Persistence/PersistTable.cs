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
// "Librainian/PersistTable.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

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
    using Measurement.Time;
    using Microsoft.Database.Isam.Config;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Microsoft.Isam.Esent.Interop.Windows81;
    using Newtonsoft.Json;
    using OperatingSystem.Compression;
    using Parsing;
    using Threading;

    /// <summary>
    ///     <para>
    ///         Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist almost any object by using
    ///         Newtonsoft.Json.
    ///     </para>
    /// </summary>
    /// <seealso cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
    [DebuggerDisplay( "{ToString(),nq}" )]
    [JsonObject]
    public sealed class PersistTable<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable where TKey : IComparable<TKey> {

        /// <summary>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="tableName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Environment.SpecialFolder specialFolder, String tableName ) : this( new Folder( specialFolder, null, tableName ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="tableName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Folder folder, String tableName ) : this( Path.Combine( folder.FullName, tableName ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="testForReadWriteAccess"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [CanBeNull] Folder folder, Boolean testForReadWriteAccess = false ) {
            try {
                if ( folder == null ) {
                    throw new ArgumentNullException( nameof( folder ) );
                }
                this.Folder = folder;

                if ( !this.Folder.Create() ) {
                    throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." );
                }

                var customConfig = new DatabaseConfig { CreatePathIfNotExist = true, EnableShrinkDatabase = ShrinkDatabaseGrbit.On, DefragmentSequentialBTrees = true };
                this.Dictionary = new PersistentDictionary<TKey, String>( this.Folder.FullName, customConfig );

                if ( testForReadWriteAccess && !this.TestForReadWriteAccess() ) {
                    throw new IOException( $"Read/write permissions denied in folder {folder.FullName}" );
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fullpath"></param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] String fullpath ) : this( new Folder( fullpath ) ) { }

        /// <summary>
        ///     No path given?
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        private PersistTable() {
            throw new NotImplementedException();

            //var name = Types.Name( () => this );

            //TODO Use the programdata\thisapp.exe type of path.
        }

        /// <summary>
        ///     Gets the number of elements contained in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </summary>
        /// <returns>
        ///     The number of elements contained in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </returns>
        public Int32 Count => this.Dictionary.Count;

        [NotNull]
        public Folder Folder {
            get;
        }

        public Boolean IsDisposed {
            get; private set;
        }

        /// <summary>
        ///     Gets a value indicating whether the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only;
        ///     otherwise, false.
        /// </returns>
        public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of
        ///     the <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        ///     object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </returns>
        public ICollection<TKey> Keys => this.Dictionary.Keys;

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values
        ///     in the <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in
        ///     the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </returns>
        public ICollection<TValue> Values => this.Dictionary.Values.Select( value => value.FromCompressedBase64().FromJSON<TValue>() ) as ICollection<TValue> ?? new Collection<TValue>();

        [JsonProperty]
        [NotNull]
        private PersistentDictionary<TKey, String> Dictionary {
            get;
        }

        /// <summary>
        ///     <para>
        ///         Here is where we interject NetDataContractSerializer to serialize to and from a
        ///         <see cref="String" /> so the <see cref="PersistentDictionary{TKey,TValue}" /> has no
        ///         trouble with it.
        ///     </para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public TValue this[ [CanBeNull] TKey key ] {
            [CanBeNull]
            get {
                if ( Equals( default( TKey ), key ) ) {
                    return default( TValue );
                }

                String storedValue;

                // ReSharper disable once AssignNullToNotNullAttribute
                if ( !this.Dictionary.TryGetValue( key, out storedValue ) ) {
                    return default( TValue );
                }
                var valueFromStore = storedValue.FromCompressedBase64().FromJSON<TValue>();
                return valueFromStore;
            }

            set {
                if ( Equals( default( TKey ), value ) ) {
                    return;
                }

                var valueToStore = value.ToJSON().ToCompressedBase64();
                this.Dictionary[ key ] = valueToStore;
            }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     An element with the same key already exists in the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public void Add( TKey key, TValue value ) => this[ key ] = value;

        /// <summary>
        ///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </summary>
        /// <param name="item">
        ///     The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add( KeyValuePair<TKey, TValue> item ) => this[ item.Key ] = item.Value;

        /// <summary>
        ///     Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Clear() => this.Dictionary.Clear();

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///     contains a specific value.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="item" /> is found in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ; otherwise, false.
        /// </returns>
        /// <param name="item">
        ///     The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </param>
        public Boolean Contains( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON().ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( item.Key, value );
            return this.Dictionary.Contains( asItem );
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" />
        ///     contains an element with the specified key.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
        ///     element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">
        ///     The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public Boolean ContainsKey( TKey key ) => this.Dictionary.ContainsKey( key );

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to
        ///     an <see cref="T:System.Array" /> , starting at a particular
        ///     <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the
        ///     elements copied from <see cref="T:System.Collections.Generic.ICollection`1" /> . The
        ///     <see cref="T:System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     The number of elements in the source
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available
        ///     space from <paramref name="arrayIndex" /> to the end of the destination
        ///     <paramref name="array" /> .
        /// </exception>
        public void CopyTo( KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex ) {
            throw new NotImplementedException();

            //this.Dictionary.CopyTo( array, arrayIndex ); ??
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose() {
            using ( Dictionary ) {
                this.Dictionary.Dispose();
            }
            this.IsDisposed = true;
        }

        public void Flush() => this.Dictionary.Flush();

        /// <summary>
        ///     Returns an enumerator that iterates through the deserialized collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.Items().GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Initialize() {
            Log.Enter();
            this.Dictionary.Database.Should().NotBeNull();
            if ( this.Dictionary.Database.ToString().IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." );
            }
            Log.Exit();
        }

        /// <summary>
        ///     All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="TValue" /> deserialized.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> Items() {
            return this.Dictionary.Select( pair => new KeyValuePair<TKey, TValue>( pair.Key, pair.Value.FromCompressedBase64().FromJSON<TValue>() ) );
        }

        /// <summary>
        ///     Removes the element with the specified key from the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </summary>
        /// <returns>
        ///     true if the element is successfully removed; otherwise, false. This method also returns
        ///     false if <paramref name="key" /> was not found in the original
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> .
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public Boolean Remove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </summary>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ; otherwise, false. This
        ///     method also returns false if <paramref name="item" /> is not found in the original
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </returns>
        /// <param name="item">
        ///     The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public Boolean Remove( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON().ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( item.Key, value );
            return this.Dictionary.Remove( asItem );
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override String ToString() {
            return $"{this.Count} items";
        }

        public void TryAdd( TKey key, TValue value ) {
            if ( !this.Dictionary.ContainsKey( key ) ) {
                this[ key ] = value;
            }
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        ///     true if the object that implements
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the
        ///     specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is
        ///     found; otherwise, the default value for the type of the <paramref name="value" />
        ///     parameter. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public Boolean TryGetValue( TKey key, out TValue value ) {
            value = default( TValue );

            String storedValue;
            if ( !this.Dictionary.TryGetValue( key, out storedValue ) ) {
                return false;
            }
            value = storedValue.FromCompressedBase64().FromJSON<TValue>();
            return true;
        }

        public Boolean TryRemove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

        /// <summary>
        ///     Return true if we can read/write in the <see cref="Folder" /> .
        /// </summary>
        /// <returns></returns>
        private Boolean TestForReadWriteAccess() {
            try {
                Document document;
                if ( this.Folder.TryGetTempDocument( out document ) ) {
                    var text = Randem.NextString( length: 64, lowers: true, uppers: true, numbers: true, symbols: true );
                    document.AppendText( text );
                    document.TryDeleting( Seconds.Seven );
                    return true;
                }
            }
            catch ( Exception ) { }
            return false;
        }
    }
}