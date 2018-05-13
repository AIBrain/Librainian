// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CollectionDebugView.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom"/>
    public class CollectionDebugView<T> {

        private readonly ICollection<T> _collection;

        public CollectionDebugView( ICollection<T> collection ) => this._collection = collection ?? throw new ArgumentNullException( nameof( collection ) );

        [DebuggerBrowsable( state: DebuggerBrowsableState.RootHidden )]
        public T[] Items {
            get {
                var array = new T[this._collection.Count];
                this._collection.CopyTo( array: array, arrayIndex: 0 );

                return array;
            }
        }
    }
}