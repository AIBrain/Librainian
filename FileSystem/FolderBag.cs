// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FolderBag.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/FolderBag.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A bag of folders, stored somewhat efficiently ?memory-wise> than a list.</para>
    /// </summary>
    [JsonObject]
    public partial class FolderBag : IEnumerable<Folder> {

        [JsonProperty]
        public List<Node> Endings { get; } = new List<Node>();

        [JsonProperty]
        public List<Node> Roots { get; } = new List<Node>();

        public Boolean Add( [CanBeNull] String folderpath ) {
            if ( null == folderpath ) { return false; }

            this.FoundAnotherFolder( new Folder( folderpath ) );

            return true;
        }

        public UInt64 AddRange( [CanBeNull] IEnumerable<String> folderpaths ) {
            var counter = 0UL;

            if ( null == folderpaths ) { return counter; }

            foreach ( var folderpath in folderpaths ) {
                this.FoundAnotherFolder( new Folder( folderpath ) );
                counter++;
            }

            return counter;
        }

        public void FoundAnotherFolder( [NotNull] Folder folder ) {
            if ( folder is null ) { throw new ArgumentNullException( nameof( folder ) ); }

            var pathParts = folder.Info.SplitPath().ToList();

            if ( !pathParts.Any() ) { return; }

            var currentNode = new Node( pathParts[0] );

            var existingNode = this.Roots.Find( node => Node.Equals( node, currentNode ) ); // look for an existing root node

            if ( !Node.Equals( existingNode, default ) ) {

                // use existing node
                currentNode = existingNode;
            }
            else {

                // didn't find one, add it
                this.Roots.Add( currentNode );
            }

            foreach ( var pathPart in pathParts.Skip( 1 ) ) {
                var nextNode = new Node( pathPart, currentNode );
                existingNode = currentNode.SubFolders.Find( node => Node.Equals( node, nextNode ) );

                if ( !Node.Equals( existingNode, default ) ) {
                    nextNode = existingNode; // already there? don't need to add it.
                }
                else {
                    currentNode.SubFolders.Add( nextNode ); // didn't find one, add it
                }

                currentNode = nextNode;
            }

            currentNode.IsEmpty.Should().BeTrue();

            if ( !currentNode.Data.EndsWith( ":" ) ) { currentNode.Parent.Should().NotBeNull(); }

            this.Endings.Add( currentNode );
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Folder> GetEnumerator() {
            foreach ( var ending in this.Endings ) {
                var node = ending;
                var path = String.Empty;

                while ( node.Parent != null ) {
                    path = $"{Path.DirectorySeparatorChar}{node.Data}{path}";
                    node = node.Parent;
                }

                this.Roots.Should().Contain( node );
                path = String.Concat( node.Data, path );

                yield return new Folder( path );
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}