// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Digit.cs",
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
// "Librainian/Librainian/Digit.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using Extensions;

    /// <summary>
    ///     Valid numbers are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
    /// </summary>
    /// <remarks>All functions should be atomic.</remarks>
    [Immutable]
    public struct Digit : IComparable<Digit> {

        public const Byte Maximum = 9;

        public const Byte Minimum = 0;

        public static Digit Eight { get; } = new Digit( 8 );

        public static Digit Five { get; } = new Digit( 5 );

        public static Digit Four { get; } = new Digit( 4 );

        public static Digit Nine { get; } = new Digit( 9 );

        public static Digit One { get; } = new Digit( 1 );

        public static Digit Seven { get; } = new Digit( 7 );

        public static Digit Six { get; } = new Digit( 6 );

        public static Digit Three { get; } = new Digit( 3 );

        public static Digit Two { get; } = new Digit( 2 );

        public static Digit Zero { get; } = new Digit( 0 );

        public Byte Value { get; }

        public Digit( SByte value ) {
            if ( value < Minimum || value > Maximum ) { throw new ArgumentOutOfRangeException( nameof( value ), "Out of range" ); }

            this.Value = ( Byte )value;
        }

        public Digit( Byte value ) : this( ( SByte )value ) { }

        public static implicit operator Byte( Digit digit ) => digit.Value;

        public static Boolean operator <( Digit left, Digit right ) => left.Value < right.Value;

        public static Boolean operator <( Digit left, SByte right ) => left.Value < right;

        public static Boolean operator <( Digit left, Byte right ) => left.Value < right;

        public static Boolean operator <( Byte left, Digit right ) => left < right.Value;

        public static Boolean operator <( SByte left, Digit right ) => left < right.Value;

        public static Boolean operator <=( Digit left, Digit right ) => left.Value <= right.Value;

        public static Boolean operator <=( Digit left, SByte right ) => left.Value <= right;

        public static Boolean operator <=( Digit left, Byte right ) => left.Value <= right;

        public static Boolean operator <=( Byte left, Digit right ) => left <= right.Value;

        public static Boolean operator <=( SByte left, Digit right ) => left <= right.Value;

        public static Boolean operator >( Digit left, Digit right ) => left.Value > right.Value;

        public static Boolean operator >( Digit left, SByte right ) => left.Value > right;

        public static Boolean operator >( Digit left, Byte right ) => left.Value > right;

        public static Boolean operator >( Byte left, Digit right ) => left > right.Value;

        public static Boolean operator >( SByte left, Digit right ) => left > right.Value;

        public static Boolean operator >=( Digit left, Digit right ) => left.Value >= right.Value;

        public static Boolean operator >=( Digit left, SByte right ) => left.Value >= right;

        public static Boolean operator >=( Digit left, Byte right ) => left.Value >= right;

        public static Boolean operator >=( Byte left, Digit right ) => left >= right.Value;

        public static Boolean operator >=( SByte left, Digit right ) => left >= right.Value;

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( Digit other ) => this.Value.CompareTo( other.Value );

        public String Number() => this.Value.ToString();

        public override String ToString() {
            switch ( this.Value ) {
                case 0: return nameof( Zero );

                case 1: return nameof( One );

                case 2: return nameof( Two );

                case 3: return nameof( Three );

                case 4: return nameof( Four );

                case 5: return nameof( Five );

                case 6: return nameof( Six );

                case 7: return nameof( Seven );

                case 8: return nameof( Eight );

                case 9: return nameof( Nine );

                default: return String.Empty;
            }
        }
    }
}