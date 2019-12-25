// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Digit.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Digit.cs" was last formatted by Protiguous on 2019/08/08 at 8:28 AM.

namespace LibrainianCore.Maths.Numbers {

    using System;
    using System.Diagnostics.CodeAnalysis;
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
            if ( value < Minimum || value > Maximum ) {
                throw new ArgumentOutOfRangeException( nameof( value ), "Out of range" );
            }

            this.Value = ( Byte ) value;
        }

        public Digit( Byte value ) : this( ( SByte ) value ) { }

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

        [NotNull]
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