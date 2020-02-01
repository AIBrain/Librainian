// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Percentage.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "Percentage.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Diagnostics;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Rationals;

    /// <summary>
    ///     <para>Restricts the value to between 0.0 and 1.0.</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public class Percentage : IComparable<Percentage>, IComparable<Double>, IEquatable<Percentage>, IComparable<Decimal> {

        [Pure]
        public Int32 CompareTo( Decimal other ) => this.Quotient.CompareTo( other );

        [Pure]
        public Int32 CompareTo( Double other ) => this.Quotient.CompareTo( other );

        [Pure]
        public Int32 CompareTo( [NotNull] Percentage other ) {
            if ( other is null ) {
                throw new ArgumentNullException( nameof( other ) );
            }

            return this.Quotient.CompareTo( other.Quotient );
        }

        public Boolean Equals( Percentage other ) => Equals( this, other );

        /// <summary>The number resulting from the division of one number by another.</summary>
        [JsonProperty]
        public Rational Quotient { get; }

        /// <summary></summary>
        /// <param name="numerator">The part of a fraction that is above the line and signifies the number to be divided by the denominator.</param>
        /// <param name="denominator">The part of a fraction that is below the line and that functions as the divisor of the numerator.</param>
        public Percentage( Rational numerator, Rational denominator ) => this.Quotient = numerator / denominator;

        public Percentage( Rational value ) : this( value, Rational.One ) { }

        public Percentage( Single value ) : this( ( Rational ) value ) { }

        public Percentage( Double value ) : this( ( Rational ) value ) { }

        public Percentage( Decimal value ) : this( ( Rational ) value ) { }

        public Percentage( Int32 value ) : this( ( Rational ) value ) { }

        /// <summary>Lerp?</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [NotNull]
        public static Percentage Combine( [NotNull] Percentage left, [NotNull] Percentage right ) =>
            new Percentage( ( left.Quotient + right.Quotient ) / ( Rational.One + Rational.One ) );

        /// <summary>static comparison</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Percentage left, [CanBeNull] Percentage right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null ) {
                return default;
            }

            if ( right is null ) {
                return default;
            }

            return left.Quotient == right.Quotient;
        }

        public static explicit operator Double( [NotNull] Percentage special ) => ( Double ) special.Quotient;

        [NotNull]
        public static implicit operator Percentage( Single value ) => new Percentage( ( Rational ) value );

        [NotNull]
        public static implicit operator Percentage( Double value ) => new Percentage( ( Rational ) value );

        [NotNull]
        public static implicit operator Percentage( Decimal value ) => new Percentage( ( Rational ) value );

        [NotNull]
        public static implicit operator Percentage( Int32 value ) => new Percentage( value );

        [NotNull]
        public static Percentage operator +( [NotNull] Percentage left, [NotNull] Percentage right ) => Combine( left, right );

        [CanBeNull]
        public static Percentage Parse( [NotNull] String value ) {
            if ( value is null ) {
                throw new ArgumentNullException( nameof( value ) );
            }

            if ( Decimal.TryParse( value, out var dec ) ) {
                return new Percentage( ( Rational ) dec );
            }

            if ( Double.TryParse( value, out var dob ) ) {
                return new Percentage( ( Rational ) dob );
            }

            return null;
        }

        public static Boolean TryParse( [NotNull] String numberString, [CanBeNull] out Percentage result ) {

            if ( Decimal.TryParse( numberString, out var dec ) ) {
                result = new Percentage( ( Rational ) dec );

                return true;
            }

            if ( Double.TryParse( numberString, out var dob ) ) {
                result = new Percentage( ( Rational ) dob );

                return true;
            }

            result = default;

            return default;
        }

        public override String ToString() => $"{this.Quotient}";

    }

}