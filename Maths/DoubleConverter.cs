#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/DoubleConverter.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Globalization;

    /// <summary>
    ///     A class to allow the conversion of doubles to string representations of
    ///     their exact decimal values. The implementation aims for readability over
    ///     efficiency.
    /// </summary>
    /// <see cref="http://yoda.arachsys.com/csharp/DoubleConverter.cs" />
    public class DoubleConverter {
        /// <summary>
        ///     Converts the given Double to a string representation of its exact decimal value.
        /// </summary>
        /// <param name="d">The Double to convert.</param>
        /// <returns>A string representation of the Double's exact decimal value.</returns>
        public static string ToExactString( Double d ) {
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
            var negative = ( bits < 0 );
            var exponent = ( int ) ( ( bits >> 52 ) & 0x7ffL );
            var mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if ( exponent == 0 ) {
                exponent++;
            }
                // Normal numbers; leave exponent as it is but add extra
                // bit to the front of the mantissa
            else {
                mantissa = mantissa | ( 1L << 52 );
            }

            // Bias the exponent. It's actually biased by 1023, but we're
            // treating the mantissa as m.0 rather than 0.m, so we need
            // to subtract another 52 from it.
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

            /// Construct a new decimal expansion with the mantissa
            var ad = new ArbitraryDecimal( mantissa );

            // If the exponent is less than 0, we need to repeatedly
            // divide by 2 - which is the equivalent of multiplying
            // by 5 and dividing by 10.
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

            // Finally, return the string with an appropriate sign
            if ( negative ) {
                return "-" + ad;
            }
            return ad.ToString();
        }

        /// <summary>Private class used for manipulating
        private class ArbitraryDecimal {
            /// <summary>
            ///     How many digits are *after* the decimal point
            /// </summary>
            private int decimalPoint;

            /// <summary>Digits in the decimal expansion, one byte per digit
            private byte[] digits;

            /// <summary>
            ///     Constructs an arbitrary decimal expansion from the given long.
            ///     The long must not be negative.
            /// </summary>
            internal ArbitraryDecimal( long x ) {
                var tmp = x.ToString( CultureInfo.InvariantCulture );
                this.digits = new byte[tmp.Length];
                for ( var i = 0; i < tmp.Length; i++ ) {
                    this.digits[ i ] = ( byte ) ( tmp[ i ] - '0' );
                }
                this.Normalize();
            }

            /// <summary>
            ///     Converts the value to a proper decimal string representation.
            /// </summary>
            public override String ToString() {
                var digitString = new char[this.digits.Length];
                for ( var i = 0; i < this.digits.Length; i++ ) {
                    digitString[ i ] = ( char ) ( this.digits[ i ] + '0' );
                }

                // Simplest case - nothing after the decimal point,
                // and last real digit is non-zero, eg value=35
                if ( this.decimalPoint == 0 ) {
                    return new string( digitString );
                }

                // Fairly simple case - nothing after the decimal
                // point, but some 0s to add, eg value=350
                if ( this.decimalPoint < 0 ) {
                    return new string( digitString ) + new string( '0', -this.decimalPoint );
                }

                // Nothing before the decimal point, eg 0.035
                if ( this.decimalPoint >= digitString.Length ) {
                    return "0." + new string( '0', ( this.decimalPoint - digitString.Length ) ) + new string( digitString );
                }

                // Most complicated case - part of the string comes
                // before the decimal point, part comes after it,
                // eg 3.5
                return new string( digitString, 0, digitString.Length - this.decimalPoint ) + "." + new string( digitString, digitString.Length - this.decimalPoint, this.decimalPoint );
            }

            /// <summary>
            ///     Multiplies the current expansion by the given amount, which should
            ///     only be 2 or 5.
            /// </summary>
            internal void MultiplyBy( int amount ) {
                var result = new byte[this.digits.Length + 1];
                for ( var i = this.digits.Length - 1; i >= 0; i-- ) {
                    var resultDigit = this.digits[ i ]*amount + result[ i + 1 ];
                    result[ i ] = ( byte ) ( resultDigit/10 );
                    result[ i + 1 ] = ( byte ) ( resultDigit%10 );
                }
                if ( result[ 0 ] != 0 ) {
                    this.digits = result;
                }
                else {
                    Array.Copy( result, 1, this.digits, 0, this.digits.Length );
                }
                this.Normalize();
            }

            /// <summary>
            ///     Removes leading/trailing zeroes from the expansion.
            /// </summary>
            internal void Normalize() {
                int first;
                for ( first = 0; first < this.digits.Length; first++ ) {
                    if ( this.digits[ first ] != 0 ) {
                        break;
                    }
                }
                int last;
                for ( last = this.digits.Length - 1; last >= 0; last-- ) {
                    if ( this.digits[ last ] != 0 ) {
                        break;
                    }
                }

                if ( first == 0 && last == this.digits.Length - 1 ) {
                    return;
                }

                var tmp = new byte[last - first + 1];
                for ( var i = 0; i < tmp.Length; i++ ) {
                    tmp[ i ] = this.digits[ i + first ];
                }

                this.decimalPoint -= this.digits.Length - ( last + 1 );
                this.digits = tmp;
            }

            /// <summary>
            ///     Shifts the decimal point; a negative value makes
            ///     the decimal expansion bigger (as fewer digits come after the
            ///     decimal place) and a positive value makes the decimal
            ///     expansion smaller.
            /// </summary>
            internal void Shift( int amount ) {
                this.decimalPoint += amount;
            }
        }
    }
}
