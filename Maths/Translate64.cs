// Copyright 2018 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Translate64.cs" was last cleaned by Rick on 2018/02/08 at 3:01 AM

namespace Librainian.Maths {
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Struct for combining two <see cref="Int32" /> (or <see cref="UInt32" />) to and from a
	///     <see cref="UInt64" /> (or <see cref="Int64" />) as easily as possible.
	/// </summary>
	[StructLayout( layoutKind: LayoutKind.Explicit )]
	public struct Translate64 {
		[FieldOffset( offset: 0 )]
		public UInt64 UnsignedValue;

		[FieldOffset( offset: 0 )]
		public Int64 SignedValue;

		[FieldOffset( offset: 0 )]
		public Int32 SignedLow;

		[FieldOffset( offset: 0 )]
		public UInt32 UnsignedLow;

		[FieldOffset( offset: sizeof( UInt32 ) )]
		public UInt32 UnsignedHigh;

		[FieldOffset( offset: sizeof( Int32 ) )]
		public Int32 SignedHigh;

		public Translate64( Int32 signedHigh, Int32 signedLow ) : this() {
			this.UnsignedValue = UInt64.MaxValue;
			this.SignedHigh = signedHigh;
			this.SignedLow = signedLow;
		}

		public Translate64( UInt64 unsignedValue ) : this() => this.UnsignedValue = unsignedValue;

		public Translate64( Int64 signedValue ) : this() => this.SignedValue = signedValue;
	}
}