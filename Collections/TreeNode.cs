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
// "Librainian/TreeNode.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

namespace Librainian.Collections {

    using System;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>
    /// http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : ABetterClassDispose {

        private TreeNode<T> _parent;

        private T _value;

        public TreeNode( T value ) {
            this.Value = value;
            this.Parent = null;
            this.Children = new TreeNodeList<T>( parent: this );
        }

        public TreeNode( T value, [NotNull] TreeNode<T> parent ) {
            this.Value = value;
            this.Parent = parent ?? throw new ArgumentNullException( nameof( parent ) );
            this.Children = new TreeNodeList<T>( parent: this );
        }

        //public event EventHandler Disposing;

        public TreeNodeList<T> Children { get; }

        public TreeTraversalType DisposeTraversal { get; } = TreeTraversalType.BottomUp;

        public Boolean IsDisposed { get; private set; }

        public TreeNode<T> Parent {
            get => this._parent;

            set {
                if ( value == this._parent ) {
                    return;
                }

                this._parent?.Children.Remove( item: this );

                if ( value?.Children.Contains( item: this ) == false ) {
                    value.Children.Add( node: this );
                }

                this._parent = value;
            }
        }

        public TreeNode<T> Root {
            get {

                //return (Parent == null) ? this : Parent.Root;

                var node = this;

                while ( node.Parent != null ) {
                    node = node.Parent;
                }

                return node;
            }
        }

        public T Value {
            get => this._value;

            set {
                this._value = value;

                if ( this._value is ITreeNodeAware<T> aware ) {
                    aware.Node = this;
                }
            }
        }

        //protected void OnDisposing() => this.Disposing?.Invoke( sender: this, e: EventArgs.Empty );

        public void CheckDisposed() {
            if ( this.IsDisposed ) {
                throw new ObjectDisposedException( objectName: GetType().Name );
            }
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            this.CheckDisposed();

            //this.OnDisposing();

            // clean up contained objects (in Value property)
            if ( this.Value is IDisposable disposable ) {
                if ( this.DisposeTraversal == TreeTraversalType.BottomUp ) {
                    foreach ( var node in this.Children ) {
                        node.Dispose();
                    }
                }

                disposable.Dispose();

                if ( this.DisposeTraversal == TreeTraversalType.TopDown ) {
                    foreach ( var node in this.Children ) {
                        node.Dispose();
                    }
                }
            }

            this.IsDisposed = true;
        }
    }
}