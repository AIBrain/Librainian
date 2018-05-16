// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FolderBag.Node.cs",
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
// "Librainian/Librainian/FolderBag.Node.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Newtonsoft.Json;

    public partial class FolderBag {

        [JsonObject]
        [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
        public class Node : IEquatable<Node>, IComparable<Node> {

            public Node( String data ) => this.Data = data;

            public Node( String data, Node parent ) {
                this.Data = data;
                this.Parent = parent;
            }

            [JsonProperty]
            public String Data { get; }

            public Boolean IsEmpty => !this.SubFolders.Any();

            [JsonProperty]
            public Node Parent { get; }

            [JsonProperty]
            public List<Node> SubFolders { get; } = new List<Node>();

            /// <summary>
            ///     Static equality check
            /// </summary>
            /// <param name="left"></param>
            /// <param name="rhs"> </param>
            /// <returns></returns>
            public static Boolean Equals( Node left, Node rhs ) {
                if ( ReferenceEquals( left, rhs ) ) { return true; }

                if ( left is null || rhs is null ) { return false; }

                return String.Equals( left.Data, rhs.Data, StringComparison.Ordinal );
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