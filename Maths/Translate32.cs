// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Translate32.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Translate32.cs" was last formatted by Protiguous on 2018/05/24 at 7:24 PM.

namespace Librainian.Maths {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Struct for combining two <see cref="ushort" /> (or <see cref="short" />) to and from a <see cref="uint" /> (or
    ///     <see cref="int" />) as easily as possible.
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
            this.SignedHigh = signedHigh;
            this.SignedLow = signedLow;
        }

        public Translate32( UInt32 unsignedValue ) : this() => this.UnsignedValue = unsignedValue;

        public Translate32( Int32 signedValue ) : this() => this.SignedValue = signedValue;

        public Translate32( UInt16 unsignedLow, UInt16 unsignedHigh ) : this() {
            this.UnsignedLow = unsignedLow;
            this.UnsignedHigh = unsignedHigh;
        }
    }
}