#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/OrderedDictionary.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    /// Represents a generic collection of key/value pairs that are ordered independently of the key
    /// and value.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    [DataContract( IsReference = true )]
    public class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue> where TKey : class {

        [DataMember]
        [OptionalField]
        private const int DefaultInitialCapacity = 0;

        [DataMember]
        [OptionalField]
        private static readonly String _keyTypeName = typeof( TKey ).FullName;

        [DataMember]
        [OptionalField]
        private static readonly Boolean _valueTypeIsReferenceType = !typeof( ValueType ).IsAssignableFrom( typeof( TValue ) );

        [DataMember]
        [OptionalField]
        private static readonly String _valueTypeName = typeof( TValue ).FullName;

        [DataMember]
        [OptionalField]
        private IEqualityComparer<TKey> _comparer;

        [DataMember]
        [OptionalField]
        private Dictionary<TKey, TValue> _dictionary;

        [DataMember]
        [OptionalField]
        private int _initialCapacity;

        [DataMember]
        [OptionalField]
        private List<KeyValuePair<TKey, TValue>> _list;

        [DataMember]
        [OptionalField]
        private object _syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class.
        /// </summary>
        public OrderedDictionary()
            : this( DefaultInitialCapacity, null ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class
        /// using the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than 0
        /// </exception>
        public OrderedDictionary( int capacity )
            : this( capacity, null ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class
        /// using the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}">IEqualityComparer&lt;TKey&gt;</see> to use when
        /// comparing keys, or <null /> to use the default <see cref="EqualityComparer{TKey}">
        /// EqualityComparer&lt;TKey&gt; </see> for the type of the key.
        /// </param>
        public OrderedDictionary( IEqualityComparer<TKey> comparer )
            : this( DefaultInitialCapacity, comparer ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class
        /// using the specified initial capacity and comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection can contain.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}">IEqualityComparer&lt;TKey&gt;</see> to use when
        /// comparing keys, or <null /> to use the default <see cref="EqualityComparer{TKey}">
        /// EqualityComparer&lt;TKey&gt; </see> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than 0
        /// </exception>
        public OrderedDictionary( int capacity, IEqualityComparer<TKey> comparer ) {
            if ( 0 > capacity ) {
                throw new ArgumentOutOfRangeException( "capacity", "'capacity' must be non-negative" );
            }

            this._initialCapacity = capacity;
            this._comparer = comparer;
        }

        /// <summary>
        /// Gets the dictionary object that stores the keys and values
        /// </summary>
        /// <value>The dictionary object that stores the keys and values for the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></value>
        /// <remarks>Accessing this property will create the dictionary object if necessary</remarks>
        private Dictionary<TKey, TValue> Dictionary { get { return this._dictionary ?? ( this._dictionary = new Dictionary<TKey, TValue>( this._initialCapacity, this._comparer ) ); } }

        /// <summary>
        /// Gets the list object that stores the key/value pairs.
        /// </summary>
        /// <value>The list object that stores the key/value pairs for the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></value>
        /// <remarks>Accessing this property will create the list object if necessary.</remarks>
        private List<KeyValuePair<TKey, TValue>> List { get { return this._list ?? ( this._list = new List<KeyValuePair<TKey, TValue>>( this._initialCapacity ) ); } }

        #region IOrderedDictionary<TKey,TValue> Members

        /// <summary>
        /// Gets the number of key/values pairs contained in the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </value>
        public int Count { get { return this.List.Count; } }

        /// <summary>
        /// Gets a value indicating whether access to the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> object
        /// is synchronized (thread-safe).
        /// </summary>
        /// <value>This method always returns false.</value>
        Boolean ICollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> object.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> object.
        /// </value>
        object ICollection.SyncRoot {
            get {
                if ( this._syncRoot == null ) {
                    Interlocked.CompareExchange( ref this._syncRoot, new object(), null );
                }
                return this._syncRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> has a
        /// fixed size.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> has a
        /// fixed size; otherwise, <see langword="false" /> . The default is <see langword="false" />.
        /// </value>
        Boolean IDictionary.IsFixedSize { get { return false; } }

        /// <summary>
        /// Gets an <see cref="ICollection" /> object containing the keys in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.
        /// </summary>
        /// <value>An <see cref="ICollection" /> object containing the keys in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.</value>
        /// <remarks>
        /// The returned <see cref="ICollection" /> object is not a static copy; instead, the
        /// collection refers back to the keys in the original <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// Therefore, changes to the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> continue to be reflected in the key collection.
        /// </remarks>
        ICollection IDictionary.Keys { get { return ( ICollection )this.Keys; } }

        /// <summary>
        /// Gets an <see cref="ICollection" /> object containing the values in the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <value>
        /// An <see cref="ICollection" /> object containing the values in the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </value>
        /// <remarks>
        /// The returned <see cref="ICollection" /> object is not a static copy; instead, the <see
        /// cref="ICollection" /> refers back to the values in the original <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// collection. Therefore, changes to the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> continue to be reflected in the <see
        /// cref="ICollection" /> .
        /// </remarks>
        ICollection IDictionary.Values { get { return ( ICollection )this.Values; } }

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection is read-only.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is
        /// read-only; otherwise, <see langword="false" /> . The default is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// A collection that is read-only does not allow the addition, removal, or modification of
        /// elements after the collection is created. <para> A collection that is read-only is
        /// simply a collection with a wrapper that prevents modification of the collection;
        /// therefore, if changes are made to the underlying collection, the read-only collection
        /// reflects those changes. </para>
        /// </remarks>
        public Boolean IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets an <see cref="T:ICollection{TKey}">ICollection&lt;TKey&gt;</see> object containing
        /// the keys in the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </summary>
        /// <value>
        /// An <see cref="T:ICollection{TKey}">ICollection&lt;TKey&gt;</see> object containing the
        /// keys in the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </value>
        /// <remarks>
        /// The returned <see cref="T:ICollection{TKey}">ICollection&lt;TKey&gt;</see> object is not
        /// a static copy; instead, the collection refers back to the keys in the original <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// Therefore, changes to the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> continue to be reflected in the key collection.
        /// </remarks>
        public ICollection<TKey> Keys { get { return this.Dictionary.Keys; } }

        /// <summary>
        /// Gets an <see cref="T:ICollection{TValue}">ICollection&lt;TValue&gt;</see> object
        /// containing the values in the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </summary>
        /// <value>
        /// An <see cref="T:ICollection{TValue}">ICollection&lt;TValue&gt;</see> object containing
        /// the values in the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </value>
        /// <remarks>
        /// The returned <see cref="T:ICollection{TValue}">ICollection&lt;TKey&gt;</see> object is
        /// not a static copy; instead, the collection refers back to the values in the original
        /// <see cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// . Therefore, changes to the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> continue to be reflected in the value collection.
        /// </remarks>
        public ICollection<TValue> Values { get { return this.Dictionary.Values; } }

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>
        /// The value associated with the specified key. If the specified key is not found,
        /// attempting to get it returns <null />, and attempting to set it creates a new element
        /// using the specified key.
        /// </value>
        object IDictionary.this[ object key ] { get { return this[ ConvertToKeyType( key ) ]; } set { this[ ConvertToKeyType( key ) ] = ConvertToValueType( value ); } }

        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <value>The value of the item at the specified index.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.<br />
        /// - or-<br /> index is equal to or greater than <see cref="Count" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueObject" /> is a null reference, and the value type of the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> is a
        /// value type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The value type of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not
        /// in the inheritance hierarchy of <paramref name="valueObject" /> .
        /// </exception>
        object IOrderedDictionary.this[ int index ] { get { return this[ index ]; } set { this[ index ] = ConvertToValueType( value ); } }

        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <value>The value of the item at the specified index.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.<br />
        /// - or-<br /> index is equal to or greater than <see cref="Count" />.
        /// </exception>
        public TValue this[ int index ] {
            get { return this.List[ index ].Value; }

            set {
                if ( index >= this.Count || index < 0 ) {
                    throw new ArgumentOutOfRangeException( "index", "'index' must be non-negative and less than the size of the collection" );
                }

                var key = this.List[ index ].Key;

                this.List[ index ] = new KeyValuePair<TKey, TValue>( key, value );
                this.Dictionary[ key ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>
        /// The value associated with the specified key. If the specified key is not found,
        /// attempting to get it returns <null />, and attempting to set it creates a new element
        /// using the specified key.
        /// </value>
        public TValue this[ TKey key ] {
            get { return this.Dictionary[ key ]; }

            set {
                if ( this.Dictionary.ContainsKey( key ) ) {
                    this.Dictionary[ key ] = value;
                    this.List[ this.IndexOfKey( key ) ] = new KeyValuePair<TKey, TValue>( key, value );
                }
                else {
                    this.Add( key, value );
                }
            }
        }

        /// <summary>
        /// Adds an entry with the specified key and value into the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. This value can be <null />.</param>
        /// <returns>The index of the newly added entry</returns>
        /// <remarks>
        /// A key cannot be <null />, but a value can be. <para> You can also use the <see
        /// cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)" /> property to add new elements by
        /// setting the value of a key that does not exist in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// collection; however, if the specified key already exists in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> ,
        /// setting the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)" /> property
        /// overwrites the old value. In contrast, the <see cref="M:Add" /> method does not modify
        /// existing elements. </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <null /></exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// </exception>
        public int Add( TKey key, TValue value ) {
            this.Dictionary.Add( key, value );
            this.List.Add( new KeyValuePair<TKey, TValue>( key, value ) );
            return this.Count - 1;
        }

        /// <summary>
        /// Removes all elements from the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <remarks>The capacity is not changed as a result of calling this method.</remarks>
        public void Clear() {
            this.Dictionary.Clear();
            this.List.Clear();
        }

        /// <summary>
        /// Determines whether the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection contains a specific key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection contains an element with the specified key; otherwise, <see langword="false"
        /// /> .
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <null /></exception>
        public Boolean ContainsKey( TKey key ) {
            return this.Dictionary.ContainsKey( key );
        }

        /// <summary>
        /// Copies the elements of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// elements to a one-dimensional Array object at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> object that is the destination of the <see
        /// cref="T:KeyValuePair`2&gt;" /> objects copied from the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>. The
        /// <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        /// <remarks>
        /// The <see cref="M:CopyTo" /> method preserves the order of the elements in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// </remarks>
        void ICollection.CopyTo( Array array, int index ) {
            ( ( ICollection )this.List ).CopyTo( array, index );
        }

        /// <summary>
        /// Adds the specified value to the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> with
        /// the specified key.
        /// </summary>
        /// <param name="item">
        /// The <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see>
        /// structure representing the key and value to add to the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add( KeyValuePair<TKey, TValue> item ) {
            this.Add( item.Key, item.Value );
        }

        /// <summary>
        /// Determines whether the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// contains a specific key and value.
        /// </summary>
        /// <param name="item">
        /// The <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see>
        /// structure to locate in the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="keyValuePair" /> is found in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> ;
        /// otherwise, <see langword="false" />.
        /// </returns>
        Boolean ICollection<KeyValuePair<TKey, TValue>>.Contains( KeyValuePair<TKey, TValue> item ) {
            return ( ( ICollection<KeyValuePair<TKey, TValue>> )this.Dictionary ).Contains( item );
        }

        /// <summary>
        /// Copies the elements of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> to an
        /// array of type <see cref="T:KeyValuePair`2&gt;" /> , starting at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array of type <see
        /// cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see> that is the
        /// destination of the <see cref="T:KeyValuePair{TKey,TValue}">
        /// KeyValuePair&lt;TKey,TValue&gt; </see> elements copied from the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> . The
        /// array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array" /> at which copying begins.
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex ) {
            ( ( ICollection<KeyValuePair<TKey, TValue>> )this.Dictionary ).CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Removes a key and value from the dictionary.
        /// </summary>
        /// <param name="item">
        /// The <see cref="T:KeyValuePair{TKey,TValue}">KeyValuePair&lt;TKey,TValue&gt;</see>
        /// structure representing the key and value to remove from the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the key and value represented by <paramref
        /// name="keyValuePair" /> is successfully found and removed; otherwise, <see
        /// langword="false" /> . This method returns <see langword="false" /> if <paramref
        /// name="keyValuePair" /> is not found in the <see cref="OrderedDictionary{TKey,TValue}">
        /// OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </returns>
        Boolean ICollection<KeyValuePair<TKey, TValue>>.Remove( KeyValuePair<TKey, TValue> item ) {
            return this.Remove( item.Key );
        }

        /// <summary>
        /// Adds an entry with the specified key and value into the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. This value can be <null />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> is <null />.<br />
        /// - or-<br /><paramref name="value" /> is <null />, and the value type of the <see
        ///   cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is a
        ///   value type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The key type of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not
        /// in the inheritance hierarchy of <paramref name="key" /> .<br />
        /// - or-<br /> The value type of the <see
        ///   cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is
        ///   not in the inheritance hierarchy of <paramref name="value" /> .
        /// </exception>
        void IDictionary.Add( object key, object value ) {
            this.Add( ConvertToKeyType( key ), ConvertToValueType( value ) );
        }

        /// <summary>
        /// Determines whether the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection contains a specific key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection contains an element with the specified key; otherwise, <see langword="false"
        /// /> .
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <null /></exception>
        /// <exception cref="ArgumentException">
        /// The key type of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not
        /// in the inheritance hierarchy of <paramref name="key" /> .
        /// </exception>
        Boolean IDictionary.Contains( object key ) {
            return this.ContainsKey( ConvertToKeyType( key ) );
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return this.Dictionary.GetEnumerator();
        }

        /// <summary>
        /// Removes the entry with the specified key from the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        void IDictionary.Remove( object key ) {
            this.Remove( ConvertToKeyType( key ) );
        }

        /// <summary>
        /// Adds an entry with the specified key and value into the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. This value can be <null />.</param>
        /// <remarks>
        /// A key cannot be <null />, but a value can be. <para> You can also use the <see
        /// cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)" /> property to add new elements by
        /// setting the value of a key that does not exist in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// collection; however, if the specified key already exists in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> ,
        /// setting the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)" /> property
        /// overwrites the old value. In contrast, the <see cref="M:Add" /> method does not modify
        /// existing elements. </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <null /></exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see>
        /// </exception>
        void IDictionary<TKey, TValue>.Add( TKey key, TValue value ) {
            this.Add( key, value );
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.List.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return this.List.GetEnumerator();
        }

        /// <summary>
        /// Inserts a new entry into the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection with the specified key and value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">
        /// The value of the entry to add. The value can be <null /> if the type of the values in
        /// the dictionary is a reference type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.<br />
        /// - or-<br /><paramref name="index" /> is greater than <see cref="Count" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is <null />.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> .
        /// </exception>
        public void Insert( int index, TKey key, TValue value ) {
            if ( index > this.Count || index < 0 ) {
                throw new ArgumentOutOfRangeException( "index" );
            }

            this.Dictionary.Add( key, value );
            this.List.Insert( index, new KeyValuePair<TKey, TValue>( key, value ) );
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator() {
            return this.Dictionary.GetEnumerator();
        }

        /// <summary>
        /// Inserts a new entry into the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// collection with the specified key and value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">
        /// The value of the entry to add. The value can be <null /> if the type of the values in
        /// the dictionary is a reference type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.<br />
        /// - or-<br /><paramref name="index" /> is greater than <see cref="Count" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> is <null />.<br />
        /// - or-<br /><paramref name="value" /> is <null />, and the value type of the <see
        ///   cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is a
        ///   value type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The key type of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not
        /// in the inheritance hierarchy of <paramref name="key" /> .<br />
        /// - or-<br /> The value type of the <see
        ///   cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is
        ///   not in the inheritance hierarchy of <paramref name="value" /> .<br />
        /// - or-<br /> An element with the same key already exists in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.
        /// </exception>
        void IOrderedDictionary.Insert( int index, object key, object value ) {
            this.Insert( index, ConvertToKeyType( key ), ConvertToValueType( value ) );
        }

        /// <summary>
        /// Removes the entry with the specified key from the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <returns>
        /// <see langword="true" /> if the key was found and the corresponding element was removed;
        /// otherwise, <see langword="false" />
        /// </returns>
        public Boolean Remove( TKey key ) {
            if ( null == key ) {
                throw new ArgumentNullException( "key" );
            }

            var index = this.IndexOfKey( key );
            if ( index >= 0 ) {
                if ( this.Dictionary.Remove( key ) ) {
                    this.List.RemoveAt( index );
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the entry at the specified index from the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.<br />
        /// - or-<br /> index is equal to or greater than <see cref="Count" />.
        /// </exception>
        public void RemoveAt( int index ) {
            if ( index >= this.Count || index < 0 ) {
                throw new ArgumentOutOfRangeException( "index", "'index' must be non-negative and less than the size of the collection" );
            }

            var key = this.List[ index ].Key;

            this.List.RemoveAt( index );
            this.Dictionary.Remove( key );
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the
        /// key is found; otherwise, the default value for the type of <paramref name="value" /> .
        /// This parameter can be passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// contains an element with the specified key; otherwise, <see langword="false" /> .
        /// </returns>
        public Boolean TryGetValue( TKey key, out TValue value ) {
            return this.Dictionary.TryGetValue( key, out value );
        }

        #endregion IOrderedDictionary<TKey,TValue> Members

        /// <summary>
        /// Returns the zero-based index of the specified key in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></param>
        /// <returns>
        /// The zero-based index of <paramref name="key" />, if <paramref name="key" /> is found in
        /// the <see cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt;
        /// </see> ; otherwise, -1
        /// </returns>
        /// <remarks>
        /// This method performs a linear search; therefore it has a cost of O(n) at worst.
        /// </remarks>
        public int IndexOfKey( TKey key ) {
            if ( null == key ) {
                throw new ArgumentNullException( "key" );
            }

            for ( var index = 0; index < this.List.Count; index++ ) {
                var entry = this.List[ index ];
                var next = entry.Key;
                if ( null != this._comparer ) {
                    if ( this._comparer.Equals( next, key ) ) {
                        return index;
                    }
                }
                else if ( next.Equals( key ) ) {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Converts the object passed as a key to the key type of the dictionary
        /// </summary>
        /// <param name="keyObject">The key object to check</param>
        /// <returns>The key object, cast as the key type of the dictionary</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyObject" /> is <null />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The key type of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not
        /// in the inheritance hierarchy of <paramref name="keyObject" /> .
        /// </exception>
        private static TKey ConvertToKeyType( object keyObject ) {
            if ( null == keyObject ) {
                throw new ArgumentNullException( "keyObject" );
            }
            if ( keyObject is TKey ) {
                return ( TKey )keyObject;
            }
            throw new ArgumentException( "'key' must be of type " + _keyTypeName, "key" );
        }

        /// <summary>
        /// Converts the object passed as a value to the value type of the dictionary
        /// </summary>
        /// <param name="value">The object to convert to the value type of the dictionary</param>
        /// <returns>The value object, converted to the value type of the dictionary</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueObject" /> is <null />, and the value type of the <see
        /// cref="OrderedDictionary{TKey,TValue}"> OrderedDictionary&lt;TKey,TValue&gt; </see> is a
        /// value type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The value type of the <see
        /// cref="OrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not
        /// in the inheritance hierarchy of <paramref name="valueObject" /> .
        /// </exception>
        private static TValue ConvertToValueType( object value ) {
            if ( null == value ) {
                if ( _valueTypeIsReferenceType ) {
                    return default( TValue );
                }
                throw new ArgumentNullException( "value" );
            }
            if ( value is TValue ) {
                return ( TValue )value;
            }
            throw new ArgumentException( "'value' must be of type " + _valueTypeName, "value" );
        }
    }
}