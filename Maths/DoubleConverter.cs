// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DoubleConverter.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using System.Globalization;

    /// <summary>
    ///     A class to allow the conversion of doubles to String representations of their exact
    ///     System.Decimal values. The implementation aims for readability over efficiency.
    /// </summary>
    /// <see cref="http://yoda.arachsys.com/csharp/DoubleConverter.cs" />
    public class DoubleConverter {

        /// <summary>
        ///     Converts the given Double to a String representation of its exact System.Decimal value.
        /// </summary>
        /// <param name="d">The Double to convert.</param>
        /// <returns>A String representation of the Double's exact System.Decimal value.</returns>
        public static String ToExactString( Double d ) {
            if ( Double.IsPositiveInfinity( d ) ) {
                return "+Infinity";
            }
            if ( Double.IsNegativeInfinity( d ) ) {
                return "-Infinity";
            }
            if ( Double.IsNaN( d ) ) {
                return "NaN";
            }

            // Translate the Double into sign, exponent and mantissa.
            var bits = BitConverter.DoubleToInt64Bits( d );

            // Note that the shift is sign-extended, hence the test against -1 not 1
            var negative = bits < 0;
            var exponent = ( Int32 )( ( bits >> 52 ) & 0x7ffL );
            var mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher, but there's no extra
            // normalisation bit in the mantissa
            if ( exponent == 0 ) {
                exponent++;
            }

            // Normal numbers; leave exponent as it is but add extra bit to the front of the mantissa
            else {
                mantissa = mantissa | ( 1L << 52 );
            }

            // Bias the exponent. It's actually biased by 1023, but we're treating the mantissa as
            // m.0 rather than 0.m, so we need to subtract another 52 from it.
            exponent -= 1075;

            if ( mantissa == 0 ) {
                return "0";
            }

            /* Normalize */
            while ( ( mantissa & 1 ) == 0 ) {
                /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            // Construct a new System.Decimal expansion with the mantissa
            var ad = new ArbitraryDecimal( mantissa );

            // If the exponent is less than 0, we need to repeatedly divide by 2 - which is the
            // equivalent of multiplying by 5 and dividing by 10.
            if ( exponent < 0 ) {
                for ( var i = 0; i < -exponent; i++ ) {
                    ad.MultiplyBy( 5 );
                }
                ad.Shift( -exponent );
            }

            // Otherwise, we need to repeatedly multiply by 2
            else {
                for ( var i = 0; i < exponent; i++ ) {
                    ad.MultiplyBy( 2 );
                }
            }

            // Finally, return the String with an appropriate sign
            if ( negative ) {
                return "-" + ad;
            }
            return ad.ToString();
        }

        /// <summary>Private class used for manipulating</summary>
        private class ArbitraryDecimal {

            /// <summary>How many digits are *after* the System.Decimal point</summary>
            private Int32 _decimalPoint;

            /// <summary>Digits in the System.Decimal expansion, one byte per digit</summary>
            private Byte[] _digits;

            /// <summary>
            ///     Constructs an arbitrary System.Decimal expansion from the given long. The long must
            ///     not be negative.
            /// </summary>
            internal ArbitraryDecimal( Int64 x ) {
                var tmp = x.ToString( CultureInfo.InvariantCulture );
                this._digits = new Byte[ tmp.Length ];
                for ( var i = 0; i < tmp.Length; i++ ) {
                    this._digits[ i ] = ( Byte )( tmp[ i ] - '0' );
                }
                this.Normalize();
            }

            /// <summary>Converts the value to a proper System.Decimal String representation.</summary>
            public override String ToString() {
                var digitString = new Char[ this._digits.Length ];
                for ( var i = 0; i < this._digits.Length; i++ ) {
                    digitString[ i ] = ( Char )( this._digits[ i ] + '0' );
                }

                // Simplest case - nothing after the System.Decimal point, and last real digit is
                // non-zero, eg value=35
                if ( this._decimalPoint == 0 ) {
                    return new String( digitString );
                }

                // Fairly simple case - nothing after the System.Decimal point, but some 0s to add,
                // eg value=350
                if ( this._decimalPoint < 0 ) {
                    return new String( digitString ) + new String( '0', -this._decimalPoint );
                }

                // Nothing before the System.Decimal point, eg 0.035
                if ( this._decimalPoint >= digitString.Length ) {
                    return "0." + new String( '0', this._decimalPoint - digitString.Length ) + new String( digitString );
                }

                // Most complicated case - part of the String comes before the System.Decimal point,
                // part comes after it, eg 3.5
                return new String( digitString, 0, digitString.Length - this._decimalPoint ) + "." + new String( digitString, digitString.Length - this._decimalPoint, this._decimalPoint );
            }

            /// <summary>
            ///     Multiplies the current expansion by the given amount, which should only be 2 or 5.
            /// </summary>
            internal void MultiplyBy( Int32 amount ) {
                var result = new Byte[ this._digits.Length + 1 ];
                for ( var i = this._digits.Length - 1; i >= 0; i-- ) {
                    var resultDigit = this._digits[ i ] * amount + result[ i + 1 ];
                    result[ i ] = ( Byte )( resultDigit / 10 );
                    result[ i + 1 ] = ( Byte )( resultDigit % 10 );
                }
                if ( result[ 0 ] != 0 ) {
                    this._digits = result;
                }
                else {
                    Array.Copy( result, 1, this._digits, 0, this._digits.Length );
                }
                this.Normalize();
            }

            /// <summary>Removes leading/trailing zeroes from the expansion.</summary>
            internal void Normalize() {
                Int32 first;
                for ( first = 0; first < this._digits.Length; first++ ) {
                    if ( this._digits[ first ] != 0 ) {
                        break;
                    }
                }
                Int32 last;
                for ( last = this._digits.Length - 1; last >= 0; last-- ) {
                    if ( this._digits[ last ] != 0 ) {
                        break;
                    }
                }

                if ( first == 0 && last == this._digits.Length - 1 ) {
                    return;
                }

                var tmp = new Byte[ last - first + 1 ];
                for ( var i = 0; i < tmp.Length; i++ ) {
                    tmp[ i ] = this._digits[ i + first ];
                }

                this._decimalPoint -= this._digits.Length - ( last + 1 );
                this._digits = tmp;
            }

            /// <summary>
            ///     Shifts the System.Decimal point; a negative value makes the System.Decimal expansion
            ///     bigger (as fewer digits come after the System.Decimal place) and a positive value
            ///     makes the System.Decimal expansion smaller.
            /// </summary>
            internal void Shift( Int32 amount ) => this._decimalPoint += amount;
        }
    }
}