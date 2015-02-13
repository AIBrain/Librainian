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
// "Librainian/TreeNodeList.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    ///     http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNodeList< T > : List< TreeNode< T > > {
        public readonly TreeNode< T > Parent;

        public TreeNodeList( [NotNull] TreeNode< T > parent ) {
            if ( parent == null ) {
                throw new ArgumentNullException( nameof( parent ) );
            }
            this.Parent = parent;
        }

        public TreeNode< T > Add( T value ) => this.Add( new TreeNode< T >( value ) );

        public new TreeNode< T > Add( [NotNull] TreeNode< T > node ) {
            if ( node == null ) {
                throw new ArgumentNullException( nameof( node ) );
            }
            base.Add( node );
            node.Parent = this.Parent;
            return node;
        }

        public override String ToString() => String.Format( "Count={0}", this.Count );
    }
}
