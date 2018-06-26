// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "AutoNumber.cs" belongs to Protiguous@Protiguous.com
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
// File "AutoNumber.cs" was last formatted by Protiguous on 2018/06/26 at 1:18 AM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Threading;
	using Newtonsoft.Json;

#pragma warning disable IDE0015 // Use framework type

	/// <summary>
	///     An automatically incrementing Identity class. ( <see cref="Identity" /> is
	///     <see cref="ulong" /> )
	/// </summary>
	[JsonObject]
#pragma warning restore IDE0015 // Use framework type
	public sealed class AutoNumber {

		/// <summary>The current value of the AutoNumber</summary>
		public UInt64 Identity => ( UInt64 ) Interlocked.Read( ref this._identity );

		[JsonProperty]
		private Int64 _identity;

		public void Ensure( UInt64 atLeast ) {
			if ( this.Identity < atLeast ) {

				//TODO make this an atomic operation
				this.Reseed( atLeast );
			}
		}

		/// <summary>Returns the incremented Identity</summary>
		/// <returns></returns>
		public UInt64 Next() => ( UInt64 ) Interlocked.Increment( ref this._identity );

		/// <summary>Resets the Identity to the specified seed value</summary>
		/// <param name="newIdentity"></param>
		public void Reseed( UInt64 newIdentity ) => Interlocked.Exchange( ref this._identity, ( Int64 ) newIdentity );

		public override String ToString() => $"{this.Identity}";

		/// <summary>Initialize the Identity with the specified seed value.</summary>
		/// <param name="seed"></param>
		public AutoNumber( UInt64 seed = UInt64.MinValue ) => this.Reseed( seed );
	}
}