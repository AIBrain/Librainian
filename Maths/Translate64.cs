// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Translate64.cs" was last cleaned by Rick on 2015/06/12 at 3:01 PM

namespace Librainian.Maths {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Struct for combining two <see cref="int" /> (or <see cref="uint" />) to and from a
    /// <see cref="ulong" /> (or <see cref="long" />) as easily as possible.
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Translate64 {

        [FieldOffset( 0 )]
        public readonly UInt64 UnsignedValue;

        [FieldOffset( 0 )]
        public readonly Int64 SignedValue;

        [FieldOffset( 0 )]
        public readonly Int32 SignedLow;

        [FieldOffset( 0 )]
        public readonly UInt32 UnsignedLow;

        [FieldOffset( sizeof(UInt32) )]
        public readonly UInt32 UnsignedHigh;

        [FieldOffset( sizeof(Int32) )]
        public readonly Int32 SignedHigh;

        public Translate64(Int32 signedHigh, Int32 signedLow) : this() {
            UnsignedValue = UInt64.MaxValue;
            SignedHigh = signedHigh;
            SignedLow = signedLow;
        }

        public Translate64(UInt64 unsignedValue) : this() {
            this.UnsignedValue = unsignedValue;
        }

        public Translate64(Int64 signedValue) : this() {
            this.SignedValue = signedValue;
        }
    }
}