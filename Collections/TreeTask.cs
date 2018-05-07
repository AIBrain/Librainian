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
// "Librainian/TreeTask.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

namespace Librainian.Collections {

    using System;

    /// <summary>
    /// http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
    /// </summary>
    public class TreeTask : ITreeNodeAware<TreeTask> {
        public Boolean Complete;

        public TreeNode<TreeTask> Node { get; set; }

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