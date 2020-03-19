// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Digit.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Digit.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Runtime.CompilerServices;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>Valid numbers are 0, 1, 2, 3, 4, 5, 6, 7, 8, 9</summary>
    [Immutable]
    public struct Digit : IComparable<Digit>, IEquatable<Digit>, IEquatable<Byte> {

        public const Byte Maximum = 9;

        public const Byte Minimum = 0;

        public static Digit Eight { get; } = new Digit( value: 8 );

        public static Digit Five { get; } = new Digit( value: 5 );

        public static Digit Four { get; } = new Digit( value: 4 );

        public static Digit Nine { get; } = new Digit( value: 9 );

        public static Digit One { get; } = new Digit( value: 1 );

        public static Digit Seven { get; } = new Digit( value: 7 );

        public static Digit Six { get; } = new Digit( value: 6 );

        public static Digit Three { get; } = new Digit( value: 3 );

        public static Digit Two { get; } = new Digit( value: 2 );

        public static Digit Zero { get; } = new Digit( value: 0 );

        public Byte Value { get; }

        public Digit( Byte value ) {
            if ( value > Maximum ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( value ), message: "Out of range" );
            }

            this.Value = value;
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Equals( Digit left, Digit right ) => left.Value == right.Value;

        public static implicit operator Byte( Digit digit ) => digit.Value;

        /// <summary>Returns a value that indicates whether two <see cref="Digit" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( Digit left, Digit right ) => !Equals( left: left, right: right );

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

        /// <summary>Returns a value that indicates whether the values of two <see cref="Digit" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( Digit left, Digit right ) => Equals( left: left, right: right );

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

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less
        /// than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( Digit other ) => this.Value.CompareTo( value: other.Value );

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public Boolean Equals( Digit other ) => Equals( left: this, right: other );

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public Boolean Equals( Byte other ) => this.Value.Equals( obj: other );

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object? obj ) => obj is Digit other && Equals( left: this, right: other );

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [NotNull]
        public String Number() => this.Value.ToString();

        [NotNull]
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