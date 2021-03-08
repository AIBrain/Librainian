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
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A <see cref="Page" /> is a sequence of <see cref="Paragraph" /> .</para>
    /// </summary>
    /// <see cref="Book"></see>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public class Chapter : IEquatable<Chapter>, IEnumerable<Page> {

        private Chapter() { }

        public Chapter( [NotNull] [ItemNotNull] IEnumerable<Page> pages ) {
            if ( pages is null ) {
                throw new ArgumentNullException( nameof( pages ) );
            }

            this.Add( pages );
        }

        [NotNull]
        [JsonProperty]
        private List<Page> Pages { get; } = new();

        public static Chapter Empty { get; } = new();

        public IEnumerator<Page> GetEnumerator() => this.Pages.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean Equals( [CanBeNull] Chapter? other ) {
            if ( other is null ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.Pages.SequenceEqual( other.Pages );
        }

        public void Add( Page page ) {
            this.Pages.Add( page );
        }

        public void Add( IEnumerable<Page> pages ) {
            this.Pages.AddRange( pages );
        }

        public override Int32 GetHashCode() => this.Pages.GetHashCode();

    }

}