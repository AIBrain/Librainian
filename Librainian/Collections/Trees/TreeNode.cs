// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Collections.Trees {

	using System;
	using JetBrains.Annotations;
	using Utilities;

	/// <summary>http: //dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/</summary>
	/// <typeparam name="T"></typeparam>
	public class TreeNode<T> : ABetterClassDispose {

		private TreeNode<T>? _parent;

		private T _value;

		public TreeNodeList<T> Children { get; }

		//public event EventHandler Disposing;

		public TreeTraversalType DisposeTraversal { get; } = TreeTraversalType.BottomUp;

		[CanBeNull]
		public TreeNode<T>? Parent {
			get => this._parent;

			set {
				if ( value == this._parent ) {
					return;
				}

				this._parent?.Children.Remove( this );

				if ( value?.Children.Contains( this ) == false ) {
					value.Children.Add( this );
				}

				this._parent = value;
			}
		}

		[NotNull]
		public TreeNode<T> Root {
			get {

				//return (Parent is null) ? this : Parent.Root;

				var node = this;

				while ( node.Parent != null ) {
					node = node.Parent;
				}

				return node;
			}
		}

		[CanBeNull]
		public T Value {
			get => this._value;

			set {
				this._value = value;

				if ( this._value is ITreeNodeAware<T> aware ) {
					aware.Node = this;
				}
			}
		}

		public TreeNode( [CanBeNull] T value ) {
			this.Value = value;
			this.Parent = null;
			this.Children = new TreeNodeList<T>( this );
		}

		public TreeNode( [CanBeNull] T value, [NotNull] TreeNode<T> parent ) {
			this.Value = value;
			this.Parent = parent ?? throw new ArgumentNullException( nameof( parent ) );
			this.Children = new TreeNodeList<T>( this );
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() {

			// clean up contained objects (in Value property)
			if ( this.Value is IDisposable disposable ) {
				if ( this.DisposeTraversal == TreeTraversalType.BottomUp ) {
					foreach ( var node in this.Children ) {
						using ( node ) { }
					}
				}

				
				using ( disposable ) { }


				if ( this.DisposeTraversal == TreeTraversalType.TopDown ) {
					foreach ( var node in this.Children ) {
						using ( node ) { }
					}
				}
			}
		}
	}
}