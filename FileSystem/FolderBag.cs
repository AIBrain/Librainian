// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FolderBag.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A bag of folders, stored somewhat efficiently memory-wise than a list.</para>
    /// </summary>
    [JsonObject]
    public class FolderBag : IEnumerable<Folder> {

        [JsonProperty]
        public List<Node> Endings { get; } = new List<Node>();

        [JsonProperty]
        public List<Node> Roots { get; } = new List<Node>();

        public Boolean Add( [CanBeNull] String folderpath ) {
            if ( null == folderpath ) {
                return false;
            }
            this.FoundAnotherFolder( new Folder( folderpath ) );
            return true;
        }

        public UInt64 AddRange( [CanBeNull] IEnumerable<String> folderpaths ) {
            var counter = 0UL;

            if ( null == folderpaths ) {
                return counter;
            }

            foreach ( var folderpath in folderpaths ) {
                this.FoundAnotherFolder( new Folder( folderpath ) );
                counter++;
            }

            return counter;
        }

        public void FoundAnotherFolder( [NotNull] Folder folder ) {
            if ( folder is null ) {
                throw new ArgumentNullException( nameof( folder ) );
            }

            var pathParts = FolderExtensions.SplitPath( folder ).ToList();
            if ( !pathParts.Any() ) {
                return;
            }

            var currentNode = new Node( pathParts[ 0 ] );

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
                var nextNode = new Node( pathPart ) { Parent = currentNode };
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
            if ( !currentNode.Data.EndsWith( ":" ) ) {
                currentNode.Parent.Should().NotBeNull();
            }

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
                path = String.Concat( node.Data, path );
                this.Roots.Should().Contain( node );
                yield return new Folder( path );
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	    [JsonObject]
        [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
        public class Node : IEquatable<Node>, IComparable<Node> {

            public Node( String data ) => this.Data = data;

	        [JsonProperty]
            public String Data {
                get;
            }

            public Boolean IsEmpty => !this.SubFolders.Any();

            [JsonProperty]
            public Node Parent {
                get; set;
            }

            [JsonProperty]
            public List<Node> SubFolders { get; } = new List<Node>();

            /// <summary>
            ///     Static equality check
            /// </summary>
            /// <param name="lhs"></param>
            /// <param name="rhs"></param>
            /// <returns></returns>
            public static Boolean Equals( Node lhs, Node rhs ) {
                if ( ReferenceEquals( lhs, rhs ) ) {
                    return true;
                }
                if ( lhs is null || rhs is null ) {
                    return false;
                }
                return String.Equals( lhs.Data, rhs.Data, StringComparison.Ordinal );
            }

            public Int32 CompareTo( Node other ) => String.Compare( this.Data, other.Data, StringComparison.Ordinal );

		    public Boolean Equals( Node other ) => Equals( this, other );

		    //public override Boolean Equals( Object obj ) {
            //    var bob = obj as Node;
            //    if ( null == bob ) {
            //        return false;
            //    }
            //    return Equals( this, bob );
            //}

            public override Int32 GetHashCode() => this.Data.GetHashCode();

		    public override String ToString() => this.Data;

        }
    }
}