#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian 2015/PersistTable.cs" was last cleaned by RICK on 2015/02/25 at 3:59 PM

#endregion License & Information

namespace Librainian.Persistence {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using IO;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Parsing;
    using Threading;

    /// <summary>
    ///     <para>
    ///         A little wrapper over the <see cref="PersistentDictionary{TKey,TValue}" /> class for
    ///         <see cref="DataContract" /> classes.
    ///     </para>
    /// </summary>
    [DataContract(IsReference = true)]

    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [Serializable]
    public class PersistTable<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable where TKey : IComparable<TKey>, IComparable {

        /// <summary>
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <param name="tableName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public PersistTable( Environment.SpecialFolder specialFolder, String tableName ) : this( new Folder( specialFolder, null, null, tableName ) ) {
        }

        public PersistTable( Folder folder, string tableName ) : this( Path.Combine( folder.FullName, tableName ) ) {
        }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="testForReadWriteAccess"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public PersistTable( [CanBeNull] Folder folder, Boolean testForReadWriteAccess = false ) {
            try {
                Log.Enter();

                if ( folder == null ) {
                    throw new ArgumentNullException( nameof( folder ) );
                }
                this.Folder = folder;
                var directory = this.Folder.FullName;

                this.Folder.Create();

                if ( !this.Folder.Exists() ) {
                    throw new DirectoryNotFoundException( String.Format( "Unable to find or create the folder `{0}`.", this.Folder.FullName ) );
                }

                this.Dictionary = new PersistentDictionary<TKey, String>( directory );

                if ( testForReadWriteAccess ) {
                    this.TestForReadWriteAccess();
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            finally {
                Log.Exit();
            }
        }

        public PersistTable( [NotNull] String fullpath ) : this( new Folder( fullpath ) ) {
        }

        /// <summary>
        ///     No path given?
        /// </summary>
        private PersistTable() {
            throw new NotImplementedException();

            //var name = Types.Name( () => this );

            //TODO Use the programdata\thisapp.exe type of path.
        }

        [NotNull]
        public Folder Folder { get; }

        [UsedImplicitly]
        private String DebuggerDisplay => this.Dictionary.ToString();

        [NotNull]
        private PersistentDictionary<TKey, String> Dictionary { get; }

        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Gets the number of elements contained in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </summary>
        /// <returns>
        ///     The number of elements contained in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
        /// </returns>
        public int Count => this.Dictionary.Count;

        /// <summary>
        ///     Gets a value indicating whether the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only;
        ///     otherwise, false.
        /// </returns>
        public bool IsReadOnly => this.Dictionary.IsReadOnly;

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
        public ICollection<TValue> Values => this.Dictionary.Values.Select( Value ) as ICollection<TValue> ?? new Collection<TValue>();

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
        public TValue this[ TKey key ] {
            get {
                String storedValue;
                if ( !this.Dictionary.TryGetValue( key, out storedValue ) ) {
                    return default(TValue);
                }
                var deSerialized = Value( storedValue );
                return deSerialized;
            }

            set {
                var valueToStore = Value( value );
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
        public bool Contains( KeyValuePair<TKey, TValue> item ) {
            var asItem = new KeyValuePair<TKey, String>( item.Key, Value( item.Value ) );
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
        public bool ContainsKey( TKey key ) => this.Dictionary.ContainsKey( key );

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
        public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex ) {

            //this.Dictionary.CopyTo( array, arrayIndex ); ??
        }

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
        public bool Remove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

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
        [Obsolete("haven't fixed this one yet")]
        public bool Remove( KeyValuePair<TKey, TValue> item ) {
            var asItem = new KeyValuePair<TKey, String>( item.Key, Value( item.Value ) ); //TODO ??
            throw new NotImplementedException();
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
        public bool TryGetValue( TKey key, out TValue value ) {
            value = default(TValue);

            String storedValue;
            if ( !this.Dictionary.TryGetValue( key, out storedValue ) ) {
                return false;
            }
            value = Value( storedValue );
            return true;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose() {
            using (Dictionary) {
                this.Dictionary.Dispose();
            }
            this.IsDisposed = true;
        }

        public void Flush() => this.Dictionary.Flush();

        public void Initialize() {
            Log.Enter();
            this.Dictionary.Database.Should().NotBeNullOrWhiteSpace();
            if ( this.Dictionary.Database.IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( String.Format( "Unable to find or create the folder `{0}`.", this.Folder.FullName ) );
            }
            Log.Exit();
        }

        /// <summary>
        ///     All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="TValue" /> deserialized.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> Items() => this.Dictionary.Select( pair => new KeyValuePair<TKey, TValue>( pair.Key, Value( pair.Value ) ) );

        public void TryAdd( TKey key, TValue value ) {
            if ( !this.Dictionary.ContainsKey( key ) ) {
                this[ key ] = value;
            }
        }

        public bool TryRemove( TKey key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

        /*
                private dynamic ToExpando( IEnumerable<KeyValuePair<TKey, TValue>> dictionary ) {
                    var expandoObject = new ExpandoObject() as IDictionary<TKey, TValue>;

                    if ( null != expandoObject ) {
                        foreach ( var keyValuePair in dictionary ) {
                            expandoObject[ keyValuePair.Key ] = keyValuePair.Value;
                        }
                    }

                    return expandoObject;
                }
        */

        /*

                /// <summary>
                /// check if we have a storage folder. if we don't, popup a dialog to ask. Settings.
                /// </summary>
                /// <returns></returns>
                public void ValidateStorageFolder() {
                    try {
                    Again:
                        if ( null == this.MainStoragePath ) {
                            this.AskUserForStorageFolder();
                            if ( null == this.MainStoragePath ) {
                                goto Again;
                            }
                        }

                        this.MainStoragePath.Refresh();
                        if ( !this.MainStoragePath.Exists ) {
                            this.AskUserForStorageFolder();
                        }

                        if ( null == this.MainStoragePath ) {
                            return;
                        }

                        if ( null == this.MainStoragePath.Ensure( requestReadAccess: true, requestWriteAccess: true ) ) {
                            goto Again;
                        }

                        if ( !this.MainStoragePath.Exists ) {
                            var dialogResult = MessageBox.Show( String.Format( "Unable to access storage folder [{0}]. Retry?", this.MainStoragePath.FullName ), "Folder Not Found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error );
                            switch ( dialogResult ) {
                                case DialogResult.Retry:
                                    goto Again;
                                case DialogResult.Cancel:
                                    return;
                            }
                        }

                        try {
                            this.TestForReadWriteAccess();
                        }
                        catch ( Exception ) {
                            var dialogResult = MessageBox.Show( String.Format( "Unable to write to storage folder [{0}]. Retry?", this.MainStoragePath ), "No Access", MessageBoxButtons.RetryCancel );
                            switch ( dialogResult ) {
                                case DialogResult.Retry:
                                    goto Again;
                                case DialogResult.Cancel:
                                    return;
                            }
                        }
                    }
                    finally {
                        String.Format( "Using storage folder `{0}`.", this.MainStoragePath ).TimeDebug();
                    }
                }
        */

        [NotNull]
        private static String Value( TValue value ) => value.Serialize() ?? String.Empty;

        private static TValue Value( [NotNull] String value ) {
            var deserialize = value.Deserialize<TValue>();
            return deserialize;
        }

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
                    Document.TryDeleting( document, Seconds.Seven );
                    return true;
                }
            }
            catch ( Exception) { }
            return false;
        }
    }
}