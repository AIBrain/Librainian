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
// "Librainian/TreeNode.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/</summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : ABetterClassDispose {
        private TreeNode<T> _parent;
        private T _value;

        public TreeNode( T value ) {
            this.Value = value;
            this.Parent = null;
            this.Children = new TreeNodeList<T>( this );
        }

        public TreeNode( T value, [NotNull] TreeNode<T> parent ) {
	        this.Value = value;
            this.Parent = parent ?? throw new ArgumentNullException( nameof( parent ) );
            this.Children = new TreeNodeList<T>( this );
        }

        public event EventHandler Disposing;

        public TreeNodeList<T> Children {
            get;
        }

        public TreeTraversalType DisposeTraversal { get; } = TreeTraversalType.BottomUp;

        public Boolean IsDisposed {
            get; private set;
        }

        public TreeNode<T> Parent {
            get => this._parent;

	        set {
                if ( value == this._parent ) {
                    return;
                }

                this._parent?.Children.Remove( this );

                if ( value != null && !value.Children.Contains( this ) ) {
                    value.Children.Add( this );
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

                if ( this._value is ITreeNodeAware<T> aware) {
                    aware.Node = this;
                }
            }
        }

        public void CheckDisposed() {
            if ( this.IsDisposed ) {
                throw new ObjectDisposedException( this.GetType().Name );
            }
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            this.CheckDisposed();
            this.OnDisposing();

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

		protected void OnDisposing() => this.Disposing?.Invoke( this, EventArgs.Empty );
	}
}