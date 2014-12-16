namespace Librainian.Maths {
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Struct for combining two <see cref="ushort" /> (or <see cref="short"/>) to and from a <see cref="uint" /> (or <see cref="int"/>) as easily as possible.
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

        public Translate32( Int16 signedHigh, Int16 signedLow )
            : this() {
            UnsignedValue = UInt32.MaxValue;
            SignedHigh = signedHigh;
            SignedLow = signedLow;
        }

        public Translate32( UInt32 unsignedValue )
            : this() {
            this.UnsignedValue = unsignedValue;
        }

        public Translate32( Int32 signedValue )
            : this() {
            this.SignedValue = signedValue;
        }
    }
}