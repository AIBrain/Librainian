// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Gate.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Gate.cs" was last formatted by Protiguous on 2018/07/13 at 1:40 AM.

namespace Librainian.Threading {

	using System;
	using System.Threading;

	/// <summary>
	///     A simple atomic gate
	/// </summary>

	// TODO 2013, unfinished TODO 2014, what was this class for? lol. TODO 2015, still no idea.
	public sealed class Gate {

		private Int32 _value;

		/// <summary>
		///     Returns true if the gate is closed
		/// </summary>
		public Boolean IsClosed => 0 == Interlocked.Add( ref this._value, 0 );

		/// <summary>
		///     Returns true if the gate is open
		/// </summary>
		public Boolean IsOpened => OpenOrClosed.Opened == ( OpenOrClosed ) Interlocked.Add( ref this._value, 0 );

		/// <summary>
		///     Initializes a new instance of the Gate class.
		/// </summary>
		/// <param name="openOrClosed">Defaults to <see cref="OpenOrClosed.Closed" />.</param>
		public Gate( OpenOrClosed openOrClosed = OpenOrClosed.Closed ) => this._value = ( Int32 ) openOrClosed;

		/// <summary>
		///     Initializes a new instance of the Gate class in the closed state.
		/// </summary>
		public Gate() {

			//
		}

		/// <summary>
		///     Closes the gate. The gate must be in the open state.
		/// </summary>
		/// <exception cref="InvalidOperationException">thrown if the gate is already closed</exception>
		public void Close() {
			if ( !this.TryClose() ) { throw new InvalidOperationException(); }
		}

		/// <summary>
		///     Opens the gate. The gate must be in the closed state.
		/// </summary>
		/// <exception cref="InvalidOperationException">thrown if the gate is already open</exception>
		public void Open() {
			if ( !this.TryOpen() ) { throw new InvalidOperationException(); }
		}

		/// <summary>
		///     Attempts to close the gate
		/// </summary>
		/// <returns>true if the operation was successful</returns>
		public Boolean TryClose() => 1 == Interlocked.CompareExchange( ref this._value, 0, 1 );

		/// <summary>
		///     Attempts to open the gate
		/// </summary>
		/// <returns>true if the operation was successful</returns>
		public Boolean TryOpen() => 0 == Interlocked.CompareExchange( ref this._value, 1, 0 );
	}
}