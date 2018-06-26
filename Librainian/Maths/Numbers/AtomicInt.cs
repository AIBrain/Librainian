// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "AtomicInt.cs" belongs to Protiguous@Protiguous.com
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
// File "AtomicInt.cs" was last formatted by Protiguous on 2018/06/26 at 1:18 AM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>An integer, thread-safe by <see cref="Interlocked" />.</summary>
	[JsonObject]
	public sealed class AtomicInt {

		/// <summary>ONLY always somtimes used in the getter and setter.</summary>
		[JsonProperty]
		private Int64 _value;

		public Int32 Value {
			get => ( Int32 ) Interlocked.Read( ref this._value );

			set => Interlocked.Exchange( ref this._value, value );
		}

		public AtomicInt( Int32 value = 0 ) => this.Value = value;

		public static implicit operator Int32( [NotNull] AtomicInt special ) => special.Value;

		[NotNull]
		public static AtomicInt operator -( [NotNull] AtomicInt a1, [NotNull] AtomicInt a2 ) => new AtomicInt( a1.Value - a2.Value );

		[NotNull]
		public static AtomicInt operator *( [NotNull] AtomicInt a1, [NotNull] AtomicInt a2 ) => new AtomicInt( a1.Value * a2.Value );

		[NotNull]
		public static AtomicInt operator +( [NotNull] AtomicInt a1, [NotNull] AtomicInt a2 ) => new AtomicInt( a1.Value + a2.Value );

		[NotNull]
		public static AtomicInt operator ++( [NotNull] AtomicInt a1 ) {
			a1.Value++;

			return a1;
		}

		[NotNull]
		public static AtomicInt Parse( [NotNull] String value ) => new AtomicInt( Int32.Parse( value ) );

		/// <summary>Resets the value to zero if less than zero at this moment in time;</summary>
		public void CheckReset() {
			if ( this.Value < 0 ) {
				this.Value = 0;
			}
		}

		public override String ToString() => $"{this.Value}";

		//public long Increment( long byAmount ) {
		//    return Interlocked.Add( ref this._value, byAmount );
		//}

		//public long Decrement( long byAmount ) {
		//    return Interlocked.Add( ref this._value, -byAmount );
		//}
	}
}