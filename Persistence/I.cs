// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "I.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/I.cs" was last formatted by Protiguous on 2018/05/27 at 11:28 PM.

namespace Librainian.Persistence {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;

    /// <summary>
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Serializable]
    [JsonObject( MemberSerialization.OptIn, IsReference = false, ItemIsReference = false, ItemNullValueHandling = NullValueHandling.Ignore, ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore )]
    public class I {

        [NotNull]
        public String K { get; }

        [NotNull]
        public Uri P { get; }

        public I( [NotNull] String key, [NotNull] Uri pointer ) {
            this.K = key ?? throw new ArgumentNullException( nameof( key ) );

            if ( pointer is null ) { throw new ArgumentNullException( paramName: nameof( pointer ) ); }

            if ( !pointer.IsAbsoluteUri ) { throw new ArgumentException( $"Uri pointer must be absolute. K={Strings.Left( this.K, 20 )}" ); }

            this.P = pointer;
        }

        public I( [NotNull] String key, String pointer ) {
            this.K = key ?? throw new ArgumentNullException( nameof( key ) );

            if ( !Uri.TryCreate( pointer, UriKind.Absolute, out var uri ) ) { throw new ArgumentException(); }

            if ( !uri.IsAbsoluteUri ) { throw new ArgumentException( $"Uri pointer must be absolute for key {Strings.Left( this.K, 20 )}" ); }

            this.P = uri;
        }

        public override String ToString() {
            var keypart = String.Empty;

            if ( this.K.Length > 42 ) {
                var left = Strings.Left( this.K, 20 );
                var right = Strings.Right( this.K, 20 );

                keypart = $"{left}..{right}";
            }

            return $"{keypart}={this.P}";
        }
    }
}