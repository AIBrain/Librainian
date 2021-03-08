// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FolderBag.cs" last formatted on 2020-08-14 at 8:40 PM.

#nullable enable

namespace Librainian.FileSystem {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Pri.LongPath;

	/// <summary>
	///     <para>A bag of folders, stored somewhat efficiently ?memory-wise? than a list.</para>
	/// </summary>
	[JsonObject]
	public class FolderBag : IEnumerable<Folder> {

		[JsonProperty]
		public List<FolderBagNode> Endings { get; } = new();

		[JsonProperty]
		public List<FolderBagNode> Roots { get; } = new();

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<Folder> GetEnumerator() {
			foreach ( var ending in this.Endings ) {
				var node = ending;
				var path = String.Empty;

				while ( node.Parent != null ) {
					path = $"{Path.DirectorySeparatorChar}{node.Data}{path}";
					node = node.Parent;
				}

				//this.Roots.Should().Contain( node );
				path = String.Concat( node.Data, path );

				yield return new Folder( path );
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public Boolean Add( [NotNull] String folderpath ) {
            if ( folderpath == null ) {
                throw new ArgumentNullException( nameof( folderpath ) );
            }

            this.FoundAnotherFolder( new Folder( folderpath ) );

			return true;
		}

		public UInt64 AddRange( [NotNull] IEnumerable<String> folderpaths ) {
            if ( folderpaths is null ) {
                throw new ArgumentNullException( nameof( folderpaths ) );
            }

            var counter = 0UL;

            foreach ( var folderpath in folderpaths ) {
				this.FoundAnotherFolder( new Folder( folderpath ) );
				counter++;
			}

			return counter;
		}

		public void FoundAnotherFolder( [NotNull] IFolder folder ) {
			if ( folder is null ) {
				throw new ArgumentNullException( nameof( folder ) );
			}

			var pathParts = folder.Info.SplitPath().ToList();

			if ( !pathParts.Any() ) {
				return;
			}

			var currentNode = new FolderBagNode( pathParts[0] );

			var existingNode = this.Roots.Find( node => FolderBagNode.Equals( node, currentNode ) ); // look for an existing root node

			if ( !FolderBagNode.Equals( existingNode, default( FolderBagNode? ) ) ) {
				// use existing node
				currentNode = existingNode;
			}
			else {
				// didn't find one, add it
				this.Roots.Add( currentNode );
			}

			// ReSharper disable once LoopCanBePartlyConvertedToQuery
			foreach ( var pathPart in pathParts.Skip( 1 ) ) {
				var nextNode = new FolderBagNode( pathPart, currentNode );
				existingNode = currentNode?.SubFolders.Find( node => FolderBagNode.Equals( node, nextNode ) );

				if ( !FolderBagNode.Equals( existingNode, default( FolderBagNode? ) ) ) {
					nextNode = existingNode; // already there? don't need to add it.
				}
				else {
					currentNode?.SubFolders.Add( nextNode ); // didn't find one, add it
				}

				currentNode = nextNode;
			}

			//currentNode.IsEmpty.Should().BeTrue();

			//if ( !currentNode.Data.EndsWith( ":" ) ) { currentNode.Parent.Should().NotBeNull(); }

            if ( currentNode != null ) {
                this.Endings.Add( currentNode );
            }
        }

	}

}