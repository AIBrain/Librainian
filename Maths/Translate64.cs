// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Translate64.cs" was last cleaned by Protiguous on 2018/05/04 at 9:47 PM

namespace Librainian.Maths {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Struct for combining two <see cref="int"/> (or <see cref="uint"/>) to and from a <see cref="ulong"/> (or <see cref="long"/>) as easily as possible.
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