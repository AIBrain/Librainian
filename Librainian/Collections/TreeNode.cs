// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "TreeNode.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "TreeNode.cs" was last formatted by Protiguous on 2018/06/04 at 3:44 PM.

namespace Librainian.Collections {

	using System;
	using JetBrains.Annotations;
	using Magic;

	/// <summary>
	///     http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TreeNode<T> : ABetterClassDispose {

		public TreeNodeList<T> Children { get; }

		//public event EventHandler Disposing;
		public TreeTraversalType DisposeTraversal { get; } = TreeTraversalType.BottomUp;

		public TreeNode<T> Parent {
			get => this._parent;

			set {
				if ( value == this._parent ) { return; }

				this._parent?.Children.Remove( item: this );

				if ( value?.Children.Contains( item: this ) == false ) { value.Children.Add( node: this ); }

				this._parent = value;
			}
		}

		[NotNull]
		public TreeNode<T> Root {
			get {

				//return (Parent == null) ? this : Parent.Root;

				var node = this;

				while ( node.Parent != null ) { node = node.Parent; }

				return node;
			}
		}

		public T Value {
			get => this._value;

			set {
				this._value = value;

				if ( this._value is ITreeNodeAware<T> aware ) { aware.Node = this; }
			}
		}

		private TreeNode<T> _parent;

		private T _value;

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() {

			// clean up contained objects (in Value property)
			if ( this.Value is IDisposable disposable ) {
				if ( this.DisposeTraversal == TreeTraversalType.BottomUp ) {
					foreach ( var node in this.Children ) { node.Dispose(); }
				}

				disposable.Dispose();

				if ( this.DisposeTraversal == TreeTraversalType.TopDown ) {
					foreach ( var node in this.Children ) { node.Dispose(); }
				}
			}
		}

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

	}

}