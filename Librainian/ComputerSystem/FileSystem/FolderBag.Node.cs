// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FolderBag.Node.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "FolderBag.Node.cs" was last formatted by Protiguous on 2018/07/10 at 8:54 PM.

namespace Librainian.ComputerSystem.FileSystem
{

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public partial class FolderBag
    {

        [JsonObject]
        [DebuggerDisplay("{" + nameof(ToString) + "()}")]
        public class Node : IEquatable<Node>, IComparable<Node>
        {

            [JsonProperty]
            public String Data { get; }

            public Boolean IsEmpty => !this.SubFolders.Any();

            [JsonProperty]
            public Node Parent { get; }

            [JsonProperty]
            public List<Node> SubFolders { get; } = new List<Node>();

            public Node(String data) => this.Data = data;

            public Node(String data, Node parent)
            {
                this.Data = data;
                this.Parent = parent;
            }

            /// <summary>
            ///     Static equality check
            /// </summary>
            /// <param name="left"></param>
            /// <param name="rhs"> </param>
            /// <returns></returns>
            public static Boolean Equals(Node left, Node rhs)
            {
                if (ReferenceEquals(left, rhs)) { return true; }

                if (left == null || rhs == null) { return false; }

                return String.Equals(left.Data, rhs.Data, StringComparison.Ordinal);
            }

            public Int32 CompareTo([NotNull] Node other) => String.Compare(this.Data, other.Data, StringComparison.Ordinal);

            public Boolean Equals(Node other) => Equals(this, other);

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