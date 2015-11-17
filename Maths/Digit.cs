// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Digit.cs" was last cleaned by Rick on 2015/09/04 at 10:56 PM

namespace Librainian.Maths {

    using System;
    using Extensions;
    using Properties;

    /// <summary>Valid numbers are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9</summary>
    /// <remarks>All functions should be atomic.</remarks>
    [Immutable]
    public struct Digit : IComparable< Digit > {

        public const Byte Minimum = 0;

        public const Byte Maximum = 9;

        public Digit( SByte value ) {
            if ( ( value < Minimum ) || ( value > Maximum ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), Resources.Out_of_range );
            }
            this.Value = ( Byte ) value;
        }

        public Digit( Byte value ) : this( (SByte)value) {
            
            
        }

        public Byte Value { get; }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return
        /// value has the following
        /// meanings: Value Meaning Less than zero This object is less than the
        ///           <paramref name="other" /> parameter.Zero This object is equal to
        ///           <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( Digit other ) => this.Value.CompareTo( other.Value );

        public static implicit operator Byte( Digit digit ) => digit.Value;

        public override String ToString() {
            switch ( this.Value ) {
                case 0:
                    return nameof( Zero );

                case 1:
                    return nameof( One );

                case 2:
                    return nameof( Two );

                case 3:
                    return nameof( Three );

                case 4:
                    return nameof( Four );

                case 5:
                    return nameof( Five );

                case 6:
                    return nameof( Six );

                case 7:
                    return nameof( Seven );

                case 8:
                    return nameof( Eight );

                case 9:
                    return nameof( Nine );

                default:
                    return String.Empty;
            }
        }

        public String Number() => this.Value.ToString();

        public static Boolean operator <( Digit left, Digit right ) => left.Value < right.Value;

        public static Boolean operator >( Digit left, Digit right ) => left.Value > right.Value;

        public static Boolean operator <=( Digit left, Digit right ) => left.Value <= right.Value;

        public static Boolean operator >=( Digit left, Digit right ) => left.Value >= right.Value;

        public static Boolean operator <( Digit left, SByte right ) => left.Value < right;

        public static Boolean operator >( Digit left, SByte right ) => left.Value > right;

        public static Boolean operator <=( Digit left, SByte right ) => left.Value <= right;

        public static Boolean operator >=( Digit left, SByte right ) => left.Value >= right;

        public static Boolean operator <( Digit left, Byte right ) => left.Value < right;

        public static Boolean operator >( Digit left, Byte right ) => left.Value > right;

        public static Boolean operator <=( Digit left, Byte right ) => left.Value <= right;

        public static Boolean operator >=( Digit left, Byte right ) => left.Value >= right;

        public static Boolean operator <( Byte left, Digit right ) => left < right.Value;

        public static Boolean operator >( Byte left, Digit right ) => left > right.Value;

        public static Boolean operator <=( Byte left, Digit right ) => left <= right.Value;

        public static Boolean operator >=( Byte left, Digit right ) => left >= right.Value;

        public static Boolean operator <( SByte left, Digit right ) => left < right.Value;

        public static Boolean operator >( SByte left, Digit right ) => left > right.Value;

        public static Boolean operator <=( SByte left, Digit right ) => left <= right.Value;

        public static Boolean operator >=( SByte left, Digit right ) => left >= right.Value;

        public static Digit Zero { get; } = new Digit( 0 );

        public static Digit One { get; } = new Digit( 1 );

        public static Digit Two { get; } = new Digit( 2 );

        public static Digit Three { get; } = new Digit( 3 );

        public static Digit Four { get; } = new Digit( 4 );

        public static Digit Five { get; } = new Digit( 5 );

        public static Digit Six { get; } = new Digit( 6 );

        public static Digit Seven { get; } = new Digit( 7 );

        public static Digit Eight { get; } = new Digit( 8 );

        public static Digit Nine { get; } = new Digit( 9 );

    }

}
