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
// "Librainian/TreeNodeList.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/</summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNodeList<T> : List<TreeNode<T>> {
        public readonly TreeNode<T> Parent;

        public TreeNodeList( [NotNull] TreeNode<T> parent ) => this.Parent = parent ?? throw new ArgumentNullException( nameof( parent ) );

	    public TreeNode<T> Add( T value ) => this.Add( new TreeNode<T>( value ) );

        public new TreeNode<T> Add( [NotNull] TreeNode<T> node ) {
            if ( node == null ) {
                throw new ArgumentNullException( nameof( node ) );
            }
            base.Add( node );
            node.Parent = this.Parent;
            return node;
        }

        public override String ToString() => $"Count={this.Count}";
    }
}