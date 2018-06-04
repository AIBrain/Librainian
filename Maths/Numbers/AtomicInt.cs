// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AtomicInt.cs" belongs to Rick@AIBrain.org and
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
// File "AtomicInt.cs" was last formatted by Protiguous on 2018/06/04 at 4:05 PM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Threading;
	using Newtonsoft.Json;

	/// <summary>An integer, thread-safe by <see cref="Interlocked" />.</summary>
	[JsonObject]
	public sealed class AtomicInt {

		public Int32 Value {
			get => ( Int32 ) Interlocked.Read( ref this._value );

			set => Interlocked.Exchange( ref this._value, value );
		}

		/// <summary>ONLY always somtimes used in the getter and setter.</summary>
		[JsonProperty]
		private Int64 _value;

		public static implicit operator Int32( AtomicInt special ) => special.Value;

		public static AtomicInt operator -( AtomicInt a1, AtomicInt a2 ) => new AtomicInt( a1.Value - a2.Value );

		public static AtomicInt operator *( AtomicInt a1, AtomicInt a2 ) => new AtomicInt( a1.Value * a2.Value );

		public static AtomicInt operator +( AtomicInt a1, AtomicInt a2 ) => new AtomicInt( a1.Value + a2.Value );

		public static AtomicInt operator ++( AtomicInt a1 ) {
			a1.Value++;

			return a1;
		}

		public static AtomicInt Parse( String value ) => new AtomicInt( Int32.Parse( value ) );

		/// <summary>Resets the value to zero if less than zero at this moment in time;</summary>
		public void CheckReset() {
			if ( this.Value < 0 ) { this.Value = 0; }
		}

		public override String ToString() => $"{this.Value}";

		//public long Increment( long byAmount ) {
		//    return Interlocked.Add( ref this._value, byAmount );
		//}

		//public long Decrement( long byAmount ) {
		//    return Interlocked.Add( ref this._value, -byAmount );
		//}

		public AtomicInt( Int32 value = 0 ) => this.Value = value;

	}

}