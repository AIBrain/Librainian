// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Translate128.cs",
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
// "Librainian/Librainian/Translate128.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Maths {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Struct for combining four <see cref="int" /> (or <see cref="uint" />) to and from a <see cref="ulong" /> (or
    ///     <see cref="long" />) as easily as possible.
    /// </summary>
    [StructLayout( layoutKind: LayoutKind.Explicit )]
    [SuppressMessage( category: "ReSharper", checkId: "FieldCanBeMadeReadOnly.Global" )]
    [SuppressMessage( category: "ReSharper", checkId: "MemberCanBePrivate.Global" )]

    //TODO
    public struct Translate128 {

        [FieldOffset( offset: 0 )]
        public Translate64 Lower;

        [FieldOffset( offset: 0 )]
        public Guid Guid;

        [FieldOffset( offset: sizeof( UInt64 ) )]
        public Translate64 Higher;

        //public Translate128( Translate64 lower, Translate64 higher ) {
        //    this.Lower = lower;
        //    this.Higher = higher;
        //}
        ////TODO

        //public Translate128( Int32 lowLow, Int32 highLow, Int32 lowHigh, Int32 highHigh ) {
        //    this.SignedValue = 0;
        //    this.UnsignedLow = 0;
        //    this.UnsignedHigh = 0;
        //    this.UnsignedValue = UInt64.MaxValue;
        //    this.SignedHigh = signedHigh;
        //    this.SignedLow = signedLow;
        //}

        //public Translate128( UInt64 unsignedValue ) {
        //    this.SignedHigh = default;
        //    this.SignedLow = default;
        //    this.SignedValue = default;
        //    this.UnsignedLow = default;
        //    this.UnsignedHigh = default;
        //    this.UnsignedValue = unsignedValue;
        //}

        //public Translate128( Int64 signedValue ) {
        //    this.UnsignedValue = default;
        //    this.UnsignedLow = default;
        //    this.UnsignedHigh = default;
        //    this.SignedLow = default;
        //    this.SignedHigh = default;
        //    this.SignedValue = signedValue;
        //}
    }
}