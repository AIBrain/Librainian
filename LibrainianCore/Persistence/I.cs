// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "I.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "I.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Persistence {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using OperatingSystem.FileSystem;

    /// <summary>[K]ey and a [U]nique location. (an [I]ndexer of storage locations)</summary>
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [Serializable]
    [JsonObject( memberSerialization: MemberSerialization.OptIn, IsReference = false, ItemIsReference = false, ItemNullValueHandling = NullValueHandling.Ignore,
        ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore )]
    public class I {

        [JsonProperty]
        [NotNull]
        public String K { get; }

        [JsonProperty]
        [NotNull]
        public Unique U { get; }

        public I( [NotNull] String key, [NotNull] Uri pointer ) {
            this.K = key ?? throw new ArgumentNullException( paramName: nameof( key ) );

            if ( pointer is null ) {
                throw new ArgumentNullException( paramName: nameof( pointer ) );
            }

            if ( !pointer.IsAbsoluteUri ) {
                throw new ArgumentException( message: $"Uri pointer must be absolute. K={Strings.Left( str: this.K, Length: 20 )}" );
            }

            this.U = pointer.ToUnique();
        }

        /// <summary></summary>
        /// <param name="key"></param>
        /// <param name="pointer"></param>
        /// <exception cref="ArgumentException"></exception>
        public I( [NotNull] String key, [CanBeNull] String pointer ) {
            this.K = key ?? throw new ArgumentNullException( paramName: nameof( key ) );

            if ( !Uri.TryCreate( uriString: pointer, uriKind: UriKind.Absolute, result: out var uri ) ) {
                throw new ArgumentException();
            }

            if ( !uri.IsAbsoluteUri ) {
                throw new ArgumentException( message: $"Uri pointer must be absolute for key {Strings.Left( str: this.K, Length: 20 )}" );
            }

            Unique.TryCreate( uri: uri, unique: out var u );
            this.U = u;
        }

        [NotNull]
        public override String ToString() =>
            this.K.Length > 42 ? $"{Strings.Left( str: this.K, Length: 20 )}..{Strings.Right( str: this.K, Length: 20 )}={this.U}" : $"{this.K}";

    }

}