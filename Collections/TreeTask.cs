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
// "Librainian/TreeTask.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;

    /// <summary>
    ///     http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
    /// </summary>
    public class TreeTask : ITreeNodeAware< TreeTask > {
        public Boolean Complete;

        public TreeNode< TreeTask > Node { get; set; }

        // recursive
        public void MarkComplete() {
            // mark all children, and their children, etc., complete
            foreach ( var childTreeNode in this.Node.Children ) {
                childTreeNode.Value.MarkComplete();
            }

            // now that all decendents are complete, mark this task complete
            this.Complete = true;
        }
    }
}
