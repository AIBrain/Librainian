// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Translate64.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Translate64.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Maths {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Struct for combining two <see cref="int" /> (or <see cref="uint" />) to and from a <see cref="ulong" /> (or
    ///     <see cref="long" />) as easily as possible.
    /// </summary>
    [StructLayout( layoutKind: LayoutKind.Explicit )]
    [SuppressMessage( category: "ReSharper", checkId: "FieldCanBeMadeReadOnly.Global" )]
    [SuppressMessage( category: "ReSharper", checkId: "MemberCanBePrivate.Global" )]
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

        public Translate64( Int32 signedHigh, Int32 signedLow ) {
            this.UnsignedValue = default;
            this.SignedValue = default;
            this.UnsignedLow = default;
            this.UnsignedHigh = default;
            this.SignedLow = signedLow;
            this.SignedHigh = signedHigh;
        }

        public Translate64( UInt64 unsignedValue ) {
            this.SignedValue = default;
            this.SignedHigh = default;
            this.SignedLow = default;
            this.UnsignedLow = default;
            this.UnsignedHigh = default;
            this.UnsignedValue = unsignedValue;
        }

        public Translate64( Int64 signedValue ) {
            this.UnsignedValue = default;
            this.UnsignedLow = default;
            this.UnsignedHigh = default;
            this.SignedLow = default;
            this.SignedHigh = default;
            this.SignedValue = signedValue;
        }
    }
}