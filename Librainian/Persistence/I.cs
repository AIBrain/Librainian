// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "I.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "I.cs" was last formatted by Protiguous on 2018/06/26 at 1:38 AM.

namespace Librainian.Persistence {

	using System;
	using System.Diagnostics;
	using ComputerSystem.FileSystem;
	using JetBrains.Annotations;
	using Microsoft.VisualBasic;
	using Newtonsoft.Json;

	/// <summary>
	///     [K]ey and a [U]nique location. (an [I]ndexer of storage locations)
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject( MemberSerialization.OptIn, IsReference = false, ItemIsReference = false, ItemNullValueHandling = NullValueHandling.Ignore, ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore )]
	public class I {

		[JsonProperty]
		[NotNull]
		public String K { get; }

		[JsonProperty]
		[NotNull]
		public Unique U { get; }

		public I( [NotNull] String key, [NotNull] Uri pointer ) {
			this.K = key ?? throw new ArgumentNullException( nameof( key ) );

			if ( pointer is null ) {
				throw new ArgumentNullException( paramName: nameof( pointer ) );
			}

			if ( !pointer.IsAbsoluteUri ) {
				throw new ArgumentException( $"Uri pointer must be absolute. K={Strings.Left( this.K, 20 )}" );
			}

			this.U = pointer.ToUnique();
		}

		/// <summary>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="pointer"></param>
		/// <exception cref="ArgumentException"></exception>
		public I( [NotNull] String key, String pointer ) {
			this.K = key ?? throw new ArgumentNullException( nameof( key ) );

			if ( !Uri.TryCreate( pointer, UriKind.Absolute, out var uri ) ) {
				throw new ArgumentException();
			}

			if ( !uri.IsAbsoluteUri ) {
				throw new ArgumentException( $"Uri pointer must be absolute for key {Strings.Left( this.K, 20 )}" );
			}

			if ( Unique.TryCreate( uri, out var u ) ) {
				this.U = u;
			}
		}

		public override String ToString() {
			var keypart = String.Empty;

			if ( this.K.Length > 42 ) {
				var left = Strings.Left( this.K, 20 );
				var right = Strings.Right( this.K, 20 );

				keypart = $"{left}..{right}";
			}

			return $"{keypart}={this.U}";
		}
	}
}