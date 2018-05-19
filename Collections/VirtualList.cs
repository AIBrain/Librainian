// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any
// source code used or derived from our binaries, libraries, projects, or solutions.
//
// This source code, "VirtualList.cs", belongs to Rick@AIBrain.org
// and Protiguous@Protiguous.com unless otherwise specified or
// the original license has been overwritten by this automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects
// still retain their original license and our thanks goes to those Authors.
//
// Donations, royalties from any software that uses any of our code,
// and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/VirtualList.cs" was last formatted by Protiguous on 2018/05/17 at 5:57 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>http://www.deanchalk.me.uk/post/2010/11/01/LINQ-Dynamically-Levereage-IList3cT3e-Functionality-With-VirtualList3cT3e.aspx</remarks>
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

        Int32 ICollection<T>.Count => this._count;

        Boolean IList.IsFixedSize => true;

        Boolean IList.IsReadOnly => true;

        Boolean ICollection<T>.IsReadOnly => true;

        Boolean ICollection.IsSynchronized => false;

        Object ICollection.SyncRoot => this;

        Object IList.this[Int32 index] {
            get => this._getValueForIndex( arg: index );

            set => throw new NotSupportedException();
        }

        T IList<T>.this[Int32 index] {
            get => this._getValueForIndex( arg: index );

            set => throw new NotSupportedException();
        }

        Int32 IList.Add( Object value ) => throw new NotSupportedException();

        void ICollection<T>.Add( T item ) => throw new NotSupportedException();

        void IList.Clear() => throw new NotSupportedException();

        void ICollection<T>.Clear() => throw new NotSupportedException();

        Boolean IList.Contains( Object value ) => throw new NotSupportedException();

        Boolean ICollection<T>.Contains( T item ) => throw new NotSupportedException();

        void ICollection.CopyTo( Array array, Int32 index ) => throw new NotSupportedException();

        void ICollection<T>.CopyTo( T[] array, Int32 arrayIndex ) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable<T> )this ).GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            for ( var i = 0; i < this._count; i++ ) { yield return this._getValueForIndex( arg: i ); }
        }

        Int32 IList.IndexOf( Object value ) => throw new NotSupportedException();

        Int32 IList<T>.IndexOf( T item ) => throw new NotSupportedException();

        void IList.Insert( Int32 index, Object value ) => throw new NotSupportedException();

        void IList<T>.Insert( Int32 index, T item ) => throw new NotSupportedException();

        void IList.Remove( Object value ) => throw new NotSupportedException();

        Boolean ICollection<T>.Remove( T item ) => throw new NotSupportedException();

        void IList.RemoveAt( Int32 index ) => throw new NotSupportedException();

        void IList<T>.RemoveAt( Int32 index ) => throw new NotSupportedException();
    }
}