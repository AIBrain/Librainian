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
// "Librainian/CollectionDebugView.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary></summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom" />
    public class CollectionDebugView<T> {
        private readonly ICollection<T> _collection;

        public CollectionDebugView( ICollection<T> collection ) => this._collection = collection ?? throw new ArgumentNullException( nameof( collection ) );

	    [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public T[] Items {
            get {
                var array = new T[ this._collection.Count ];
                this._collection.CopyTo( array, 0 );
                return array;
            }
        }
    }
}