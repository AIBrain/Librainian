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
// File "Translate64.cs" last formatted on 2020-08-14 at 8:32 PM.

namespace Librainian.Converters {

	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Struct for combining two <see cref="Int32" /> (or <see cref="UInt32" />) to and from a <see cref="UInt64" /> (or
	///     <see cref="Int64" />) as easily as possible. Fast? No idea
	///     yet..
	/// </summary>
	[StructLayout( LayoutKind.Explicit, Pack = 0 )]
	public struct Translate64 {

		[FieldOffset( 0 )]
		public UInt64 UnsignedValue;

		[FieldOffset( 0 )]
		public Int64 SignedValue;

		[FieldOffset( 0 )]
		public Int32 SignedLow;

		[FieldOffset( 0 )]
		public readonly UInt32 UnsignedLow;

		[FieldOffset( sizeof( UInt32 ) )]
		public readonly UInt32 UnsignedHigh;

		[FieldOffset( sizeof( Int32 ) )]
		public Int32 SignedHigh;

		public Translate64( Int32 signedHigh, Int32 signedLow ) {
			this.UnsignedValue = default( UInt64 );
			this.SignedValue = default( Int64 );
			this.UnsignedLow = default( UInt32 );
			this.UnsignedHigh = default( UInt32 );
			this.SignedLow = signedLow;
			this.SignedHigh = signedHigh;
		}

		public Translate64( UInt64 unsignedValue ) {
			this.SignedValue = default( Int64 );
			this.SignedHigh = default( Int32 );
			this.SignedLow = default( Int32 );
			this.UnsignedLow = default( UInt32 );
			this.UnsignedHigh = default( UInt32 );
			this.UnsignedValue = unsignedValue;
		}

		public Translate64( Int64 signedValue ) {
			this.UnsignedValue = default( UInt64 );
			this.UnsignedLow = default( UInt32 );
			this.UnsignedHigh = default( UInt32 );
			this.SignedLow = default( Int32 );
			this.SignedHigh = default( Int32 );
			this.SignedValue = signedValue;
		}

		public Translate64( UInt32 unsignedLow, UInt32 unsignedHigh ) {
			this.UnsignedValue = default( UInt64 );
			this.SignedValue = default( Int64 );
			this.SignedLow = default( Int32 );
			this.SignedHigh = default( Int32 );
			this.UnsignedLow = unsignedLow;
			this.UnsignedHigh = unsignedHigh;
		}
	}
}