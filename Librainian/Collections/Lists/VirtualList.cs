// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "VirtualList.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "VirtualList.cs" was last formatted by Protiguous on 2019/08/08 at 6:34 AM.

namespace Librainian.Collections.Lists {

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

        Int32 ICollection.Count => this._count;

        Boolean IList.IsFixedSize => true;

        Boolean IList.IsReadOnly => true;

        Boolean ICollection.IsSynchronized => false;

        Object ICollection.SyncRoot => this;

        Object IList.this[ Int32 index ] {
            get => this.GetValueForIndex( arg: index );

            set => throw new NotSupportedException();
        }

        Int32 IList.Add( Object value ) => throw new NotSupportedException();

        void IList.Clear() => throw new NotSupportedException();

        Boolean IList.Contains( Object value ) => throw new NotSupportedException();

        void ICollection.CopyTo( Array array, Int32 index ) => throw new NotSupportedException();

        Int32 IList.IndexOf( Object value ) => throw new NotSupportedException();

        void IList.Insert( Int32 index, Object value ) => throw new NotSupportedException();

        void IList.Remove( Object value ) => throw new NotSupportedException();

        void IList.RemoveAt( Int32 index ) => throw new NotSupportedException();

        Int32 ICollection<T>.Count => this._count;

        Boolean ICollection<T>.IsReadOnly => true;

        T IList<T>.this[ Int32 index ] {
            get => this.GetValueForIndex( arg: index );

            set => throw new NotSupportedException();
        }

        void ICollection<T>.Add( T item ) => throw new NotSupportedException();

        void ICollection<T>.Clear() => throw new NotSupportedException();

        Boolean ICollection<T>.Contains( T item ) => throw new NotSupportedException();

        void ICollection<T>.CopyTo( T[] array, Int32 arrayIndex ) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable<T> ) this ).GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            for ( var i = 0; i < this._count; i++ ) {
                yield return this.GetValueForIndex( arg: i );
            }
        }

        Int32 IList<T>.IndexOf( T item ) => throw new NotSupportedException();

        void IList<T>.Insert( Int32 index, T item ) => throw new NotSupportedException();

        Boolean ICollection<T>.Remove( T item ) => throw new NotSupportedException();

        void IList<T>.RemoveAt( Int32 index ) => throw new NotSupportedException();

        private Int32 _count { get; }

        private Func<Int32, T> GetValueForIndex { get; }

        public VirtualList( Int32 count, Func<Int32, T> getValueForIndex ) {
            this.GetValueForIndex = getValueForIndex;
            this._count = count;
        }
    }
}