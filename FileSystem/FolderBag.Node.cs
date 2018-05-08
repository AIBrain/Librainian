// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
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
    using System.Collections.Generic;
    using System.Diagnostics;
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
            public String Data {
                get;
            }

            public Boolean IsEmpty => !this.SubFolders.Any();

            [JsonProperty]
            public Node Parent { get; }

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