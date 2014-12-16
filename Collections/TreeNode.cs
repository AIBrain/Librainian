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
// "Librainian/TreeNode.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Collections {
    using System;
    using JetBrains.Annotations;

    /// <summary>
    ///     http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode< T > : IDisposable {
        private TreeNode< T > _parent;
        private T _value;

        public TreeNode( T value ) {
            this.Value = value;
            this.Parent = null;
            this.Children = new TreeNodeList< T >( this );
        }

        public TreeNode( T value, [NotNull] TreeNode< T > parent ) {
            if ( parent == null ) {
                throw new ArgumentNullException( "parent" );
            }
            this.Value = value;
            this.Parent = parent;
            this.Children = new TreeNodeList< T >( this );
        }

        public TreeNodeList< T > Children { get; private set; }

        public TreeTraversalType DisposeTraversal { get; } = TreeTraversalType.BottomUp;

        public Boolean IsDisposed { get; private set; }

        public TreeNode< T > Parent {
            get { return this._parent; }

            set {
                if ( value == this._parent ) {
                    return;
                }

                if ( this._parent != null ) {
                    this._parent.Children.Remove( this );
                }

                if ( value != null && !value.Children.Contains( this ) ) {
                    value.Children.Add( this );
                }

                this._parent = value;
            }
        }

        public TreeNode< T > Root {
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
            get { return this._value; }

            set {
                this._value = value;

                if ( this._value is ITreeNodeAware< T > ) {
                    ( this._value as ITreeNodeAware< T > ).Node = this;
                }
            }
        }

        public void Dispose() {
            this.CheckDisposed();
            this.OnDisposing();

            // clean up contained objects (in Value property)
            if ( this.Value is IDisposable ) {
                if ( this.DisposeTraversal == TreeTraversalType.BottomUp ) {
                    foreach ( var node in this.Children ) {
                        node.Dispose();
                    }
                }

                ( this.Value as IDisposable ).Dispose();

                if ( this.DisposeTraversal == TreeTraversalType.TopDown ) {
                    foreach ( var node in this.Children ) {
                        node.Dispose();
                    }
                }
            }

            this.IsDisposed = true;
        }

        public event EventHandler Disposing;

        public void CheckDisposed() {
            if ( this.IsDisposed ) {
                throw new ObjectDisposedException( this.GetType().Name );
            }
        }

        protected void OnDisposing() {
            if ( this.Disposing != null ) {
                this.Disposing( this, EventArgs.Empty );
            }
        }
    }
}
