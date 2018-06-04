// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Credits.cs" belongs to Rick@AIBrain.org and
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Credits.cs" was last formatted by Protiguous on 2018/06/04 at 4:05 PM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Keep count of credits, current and lifetime.</para>
	/// </summary>
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public class Credits {

		public UInt64 CurrentCredits {
			get => Thread.VolatileRead( ref this._currentCredits );

			private set => Thread.VolatileWrite( ref this._currentCredits, value );
		}

		public UInt64 LifetimeCredits {
			get => Thread.VolatileRead( ref this._lifetimeCredits );

			private set => Thread.VolatileWrite( ref this._lifetimeCredits, value );
		}

		/// <summary>ONLY used in the getter and setter.</summary>
		[JsonProperty]
		private UInt64 _currentCredits;

		/// <summary>ONLY used in the getter and setter.</summary>
		[JsonProperty]
		private UInt64 _lifetimeCredits;

		public static Credits Combine( [NotNull] Credits left, [NotNull] Credits right ) {
			if ( left is null ) { throw new ArgumentNullException( nameof( left ) ); }

			if ( right is null ) { throw new ArgumentNullException( nameof( right ) ); }

			return new Credits( left.CurrentCredits + right.CurrentCredits, left.LifetimeCredits + right.LifetimeCredits );
		}

		public void AddCredits( UInt64 credits = 1 ) {
			this.CurrentCredits += credits;
			this.LifetimeCredits += credits;
		}

		public Credits Clone() => new Credits( this.CurrentCredits, this.LifetimeCredits );

		public void SubtractCredits( UInt64 credits = 1 ) {
			var currentcredits = ( Int64 ) this.CurrentCredits;

			if ( currentcredits - ( Int64 ) credits < 0 ) { this.CurrentCredits = 0; }
			else { this.CurrentCredits -= credits; }
		}

		public override String ToString() => $"{this.CurrentCredits:N0} credits ({this.LifetimeCredits:N0} lifetime credits).";

		/// <summary>No credits.</summary>
		public static readonly Credits Zero = new Credits( currentCredits: 0, lifetimeCredits: 0 );

		public Credits( UInt64 currentCredits = 0, UInt64 lifetimeCredits = 0 ) {
			this.CurrentCredits = currentCredits;
			this.LifetimeCredits = lifetimeCredits;
		}

	}

}