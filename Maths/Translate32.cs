// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Translate32.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using System.Runtime.InteropServices;

	/// <summary>
	///     Struct for combining two <see cref="UInt16" /> (or <see cref="Int16" />) to and from a
	///     <see cref="UInt32" /> (or <see cref="Int32" />) as easily as possible.
	/// </summary>
	[StructLayout( LayoutKind.Explicit )]
    public struct Translate32 {

        [FieldOffset( 0 )]
        public readonly UInt32 UnsignedValue;

        [FieldOffset( 0 )]
        public readonly Int32 SignedValue;

        [FieldOffset( 0 )]
        public readonly Int16 SignedLow;

        [FieldOffset( 0 )]
        public readonly UInt16 UnsignedLow;

        [FieldOffset( sizeof( UInt16 ) )]
        public readonly UInt16 UnsignedHigh;

        [FieldOffset( sizeof( Int16 ) )]
        public readonly Int16 SignedHigh;

        public Translate32( Int16 signedHigh, Int16 signedLow ) : this() {
			this.UnsignedValue = UInt32.MaxValue;
			this.SignedHigh = signedHigh;
			this.SignedLow = signedLow;
        }

        public Translate32( UInt32 unsignedValue ) : this() => this.UnsignedValue = unsignedValue;

		public Translate32( Int32 signedValue ) : this() => this.SignedValue = signedValue;

	}
}