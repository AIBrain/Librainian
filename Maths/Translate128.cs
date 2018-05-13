namespace Librainian.Maths {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Struct for combining four <see cref="int"/> (or <see cref="uint"/>) to and from a <see cref="ulong"/> (or <see cref="long"/>) as easily as possible.
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