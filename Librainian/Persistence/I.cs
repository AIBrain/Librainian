// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "I.cs" last formatted on 2020-08-14 at 8:44 PM.

#nullable enable
namespace Librainian.Persistence {

	using System;
	using System.Diagnostics;
	using FileSystem;
	using JetBrains.Annotations;
	using Microsoft.VisualBasic;
	using Newtonsoft.Json;

	/// <summary>[K]ey and a [U]nique location. (an [I]ndexer of storage locations)</summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject( MemberSerialization.OptIn, IsReference = false, ItemIsReference = false, /*ItemNullValueHandling = NullValueHandling.Ignore,*/
				 ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore )]
	public class I {

		public I( [NotNull] String key, [NotNull] Uri pointer ) {
			this.K = key ?? throw new ArgumentNullException( nameof( key ) );

			if ( pointer is null ) {
				throw new ArgumentNullException( nameof( pointer ) );
			}

			if ( !pointer.IsAbsoluteUri ) {
				throw new ArgumentException( $"Uri pointer must be absolute. K={Strings.Left( this.K, 20 )}" );
			}

			this.U = pointer.ToUnique();
		}

		/// <summary></summary>
		/// <param name="key"></param>
		/// <param name="pointer"></param>
		/// <exception cref="ArgumentException"></exception>
		public I( [NotNull] String key, [CanBeNull] String? pointer ) {
			this.K = key ?? throw new ArgumentNullException( nameof( key ) );

			if ( !Uri.TryCreate( pointer, UriKind.Absolute, out var uri ) ) {
				throw new ArgumentException();
			}

			if ( !uri.IsAbsoluteUri ) {
				throw new ArgumentException( $"Uri pointer must be absolute for key {Strings.Left( this.K, 20 )}" );
			}

			Unique.TryCreate( uri, out var u );
			this.U = u;
		}

		[JsonProperty]
		[NotNull]
		public String K { get; }

		[JsonProperty]
		[NotNull]
		public Unique U { get; }

		[NotNull]
		public override String ToString() => this.K.Length > 42 ? $"{Strings.Left( this.K, 20 )}..{Strings.Right( this.K, 20 )}={this.U}" : $"{this.K}";

	}

}