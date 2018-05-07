// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TreeNodeList.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNodeList<T> : List<TreeNode<T>> {
        public readonly TreeNode<T> Parent;

        public TreeNodeList( [NotNull] TreeNode<T> parent ) => this.Parent = parent ?? throw new ArgumentNullException( paramName: nameof( parent ) );

        public TreeNode<T> Add( T value ) => this.Add( node: new TreeNode<T>( value: value ) );

        public new TreeNode<T> Add( [NotNull] TreeNode<T> node ) {
            if ( node is null ) {
                throw new ArgumentNullException( paramName: nameof( node ) );
            }

            base.Add( item: node );
            node.Parent = this.Parent;
            return node;
        }

        public override String ToString() => $"Count={Count}";
    }
}