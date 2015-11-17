// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/IOrderedDictionary.cs" was last cleaned by Rick on 2015/06/12 at 2:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Represents a generic collection of key/value pairs that are ordered independently of the key
    /// and value.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
    public interface IOrderedDictionary<TKey, TValue> : IOrderedDictionary, IDictionary<TKey, TValue> {

        /// <summary>Gets or sets the value at the specified index.</summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <value>The value of the item at the specified index.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0. <br />
        /// - or- <br /><paramref name="index" /> is equal to or greater than
        ///   <see cref="System.Collections.ICollection.Count" /> .
        /// </exception>
        new TValue this[ Int32 index ] {
            get; set;
        }

        /// <summary>
        /// Adds an entry with the specified key and value into the
        /// <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add.</param>
        /// <returns>The index of the newly added entry</returns>
        /// <remarks>
        /// <para>
        /// You can also use the <see cref="P:IDictionary{TKey,TValue}.Item(TKey)" /> property to
        /// add new elements by setting the value of a key that does not exist in the
        /// <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> collection; however, if the specified key already exists in the
        /// <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> , setting the <see cref="P:Item(TKey)" /> property overwrites the old value. In
        /// contrast, the <see cref="M:Add" /> method does not modify existing elements.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="IOrderedDictionary{TKey,TValue}">IOrderedDictionary&lt;TKey,TValue&gt;</see>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> is read-only. <br />
        /// - or- <br /> The <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> has a fized size.
        /// </exception>
        new Int32 Add(TKey key, TValue value);

        /// <summary>
        /// Inserts a new entry into the <see cref="IOrderedDictionary{TKey,TValue}">
        /// IOrderedDictionary&lt;TKey,TValue&gt; </see> collection with the specified key and value
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">
        /// The value of the entry to add. The value can be <null /> if the type of the values in
        /// the dictionary is a reference type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0. <br />
        /// - or- <br /><paramref name="index" /> is greater than
        ///   <see cref="System.Collections.ICollection.Count" /> .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the
        /// <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> .
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> is read-only. <br />
        /// - or- <br /> The <see cref="IOrderedDictionary{TKey,TValue}"> IOrderedDictionary&lt;TKey,TValue&gt;
        /// </see> has a fized size.
        /// </exception>
        void Insert(Int32 index, TKey key, TValue value);
    }
}