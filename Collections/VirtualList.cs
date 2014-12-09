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
// "Librainian/VirtualList.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <see
    ///     cref="http://www.deanchalk.me.uk/post/2010/11/01/LINQ-Dynamically-Levereage-IList3cT3e-Functionality-With-VirtualList3cT3e.aspx" />
    /// <example>
    ///     var pi = new VirtualList(Double)(int.MaxValue, i => Math.Pow(-1d, i) / (2 * i + 1) * 4).AsParallel().Aggregate(()
    ///     => 0d, (tot, next) => tot + next, (maint,localt) => maint + localt, final => final);
    /// </example>
    public class VirtualList< T > : IList< T >, IList {
        private readonly int _count;

        private readonly Func< int, T > _getValueForIndex;

        public VirtualList( int count, Func< int, T > getValueForIndex ) {
            this._getValueForIndex = getValueForIndex;
            this._count = count;
        }

        int ICollection.Count => this._count;

        Boolean ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        Boolean IList.IsFixedSize => true;

        Boolean IList.IsReadOnly => true;

        object IList.this[ int index ] { get { return this._getValueForIndex( index ); } set { throw new NotSupportedException(); } }

        void ICollection.CopyTo( Array array, int index ) {
            throw new NotSupportedException();
        }

        int IList.Add( object value ) {
            throw new NotSupportedException();
        }

        void IList.Clear() {
            throw new NotSupportedException();
        }

        Boolean IList.Contains( object value ) {
            throw new NotSupportedException();
        }

        int IList.IndexOf( object value ) {
            throw new NotSupportedException();
        }

        void IList.Insert( int index, object value ) {
            throw new NotSupportedException();
        }

        void IList.Remove( object value ) {
            throw new NotSupportedException();
        }

        void IList.RemoveAt( int index ) {
            throw new NotSupportedException();
        }

        int ICollection< T >.Count => this._count;

        Boolean ICollection< T >.IsReadOnly => true;
        T IList< T >.this[ int index ] { get { return this._getValueForIndex( index ); } set { throw new NotSupportedException(); } }

        void ICollection< T >.Add( T item ) {
            throw new NotSupportedException();
        }

        void ICollection< T >.Clear() {
            throw new NotSupportedException();
        }

        Boolean ICollection< T >.Contains( T item ) {
            throw new NotSupportedException();
        }

        void ICollection< T >.CopyTo( T[] array, int arrayIndex ) {
            throw new NotSupportedException();
        }

        Boolean ICollection< T >.Remove( T item ) {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable< T > ) this ).GetEnumerator();

        IEnumerator< T > IEnumerable< T >.GetEnumerator() {
            for ( var i = 0; i < this._count; i++ ) {
                yield return this._getValueForIndex( i );
            }
        }

        int IList< T >.IndexOf( T item ) {
            throw new NotSupportedException();
        }

        void IList< T >.Insert( int index, T item ) {
            throw new NotSupportedException();
        }

        void IList< T >.RemoveAt( int index ) {
            throw new NotSupportedException();
        }
    }
}
