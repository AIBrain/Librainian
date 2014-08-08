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
// "Librainian2/CollectionDebugView.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="http://www.codeproject.com/Articles/28405/Make-the-debugger-show-the-contents-of-your-custom" />
    public class CollectionDebugView< T > {
        private readonly ICollection< T > _collection;

        public CollectionDebugView( ICollection< T > collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            this._collection = collection;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public T[] Items {
            get {
                var array = new T[this._collection.Count];
                this._collection.CopyTo( array, 0 );
                return array;
            }
        }
    }
}
