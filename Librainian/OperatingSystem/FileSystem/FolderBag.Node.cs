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

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    public partial class FolderBag {

        [JsonObject]
        [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
        public class Node : IEquatable<Node>, IComparable<Node> {

            public Int32 CompareTo( [NotNull] Node other ) => String.Compare( this.Data, other.Data, StringComparison.Ordinal );

            public Boolean Equals( Node other ) => Equals( this, other );

            [JsonProperty]
            [CanBeNull]
            public String? Data { get; }

            public Boolean IsEmpty => !this.SubFolders.Any();

            [JsonProperty]
            public Node Parent { get; }

            [JsonProperty]
            [NotNull]
            public List<Node> SubFolders { get; } = new List<Node>();

            public Node( [CanBeNull] String? data ) => this.Data = data;

            public Node( [CanBeNull] String? data, [CanBeNull] Node parent ) {
                this.Data = data;
                this.Parent = parent;
            }

            /// <summary>Static equality check</summary>
            /// <param name="left"></param>
            /// <param name="right"> </param>
            /// <returns></returns>
            public static Boolean Equals( [CanBeNull] Node left, [CanBeNull] Node right ) {
                if ( ReferenceEquals( left, right ) ) {
                    return true;
                }

                if ( left is null || right is null ) {
                    return default;
                }

                return String.Equals( left.Data, right.Data, StringComparison.Ordinal );
            }

            /// <summary>Determines whether the specified object is equal to the current object.</summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
            public override Boolean Equals( Object? obj ) => Equals( this, obj as Node );

            public override Int32 GetHashCode() => this.Data?.GetHashCode() ?? 0;

            [NotNull]
            public override String ToString() => this.Data ?? Symbols.Null;

        }

    }

}