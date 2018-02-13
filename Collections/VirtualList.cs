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
// "Librainian/VirtualList.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary></summary>
    /// <typeparam name="T"></typeparam>
    /// <see
    ///     cref="http://www.deanchalk.me.uk/post/2010/11/01/LINQ-Dynamically-Levereage-IList3cT3e-Functionality-With-VirtualList3cT3e.aspx" />
    /// <example>
    ///     var pi = new VirtualList(Double)(int.MaxValue, i =&gt; Math.Pow(-1d, i) / (2 * i + 1) *
    ///     4) .AsParallel().Aggregate(()
    ///     = &gt; 0d, (tot, next) =&gt; tot + next, (maint,localt) =&gt; maint + localt, final =&gt; final);
    /// </example>
    public sealed class VirtualList<T> : IList<T>, IList {
        private readonly Int32 _count;
        private readonly Func<Int32, T> _getValueForIndex;

        public VirtualList( Int32 count, Func<Int32, T> getValueForIndex ) {
            this._getValueForIndex = getValueForIndex;
            this._count = count;
        }

        Int32 ICollection.Count => this._count;

        Boolean ICollection.IsSynchronized => false;

        Object ICollection.SyncRoot => this;

        Int32 ICollection<T>.Count => this._count;

        Boolean ICollection<T>.IsReadOnly => true;

        Boolean IList.IsFixedSize => true;

        Boolean IList.IsReadOnly => true;

        Object IList.this[ Int32 index ] {
            get {
                return this._getValueForIndex( index );
            }

            set {
                throw new NotSupportedException();
            }
        }

        T IList<T>.this[ Int32 index ] {
            get {
                return this._getValueForIndex( index );
            }

            set {
                throw new NotSupportedException();
            }
        }

        void ICollection.CopyTo( Array array, Int32 index ) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add( T item ) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        Boolean ICollection<T>.Contains( T item ) {
            throw new NotSupportedException();
        }

        void ICollection<T>.CopyTo( T[] array, Int32 arrayIndex ) {
            throw new NotSupportedException();
        }

        Boolean ICollection<T>.Remove( T item ) {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable<T> )this ).GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            for ( var i = 0; i < this._count; i++ ) {
                yield return this._getValueForIndex( i );
            }
        }

        Int32 IList.Add( Object value ) {
            throw new NotSupportedException();
        }

        void IList.Clear() {
            throw new NotSupportedException();
        }

        Boolean IList.Contains( Object value ) {
            throw new NotSupportedException();
        }

        Int32 IList.IndexOf( Object value ) {
            throw new NotSupportedException();
        }

        void IList.Insert( Int32 index, Object value ) {
            throw new NotSupportedException();
        }

        void IList.Remove( Object value ) {
            throw new NotSupportedException();
        }

        void IList.RemoveAt( Int32 index ) {
            throw new NotSupportedException();
        }

        Int32 IList<T>.IndexOf( T item ) {
            throw new NotSupportedException();
        }

        void IList<T>.Insert( Int32 index, T item ) {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt( Int32 index ) {
            throw new NotSupportedException();
        }
    }
}