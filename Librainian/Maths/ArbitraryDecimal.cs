// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ArbitraryDecimal.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "ArbitraryDecimal.cs" was last formatted by Protiguous on 2020/03/18 at 10:24 AM.

namespace Librainian.Maths {

    using System;
    using System.Globalization;
    using JetBrains.Annotations;

    /// <summary>Private class used for manipulating</summary>
    public class ArbitraryDecimal {

        /// <summary>How many digits are *after* the System.Decimal point</summary>
        private Int32 _decimalPoint;

        /// <summary>Digits in the System.Decimal expansion, one byte per digit</summary>
        private Byte[] _digits;

        /// <summary>Constructs an arbitrary System.Decimal expansion from the given long. The long must not be negative.</summary>
        public ArbitraryDecimal( Int64 x ) {
            var tmp = x.ToString( CultureInfo.CurrentCulture );
            this._digits = new Byte[ tmp.Length ];

            for ( var i = 0; i < tmp.Length; i++ ) {
                this._digits[ i ] = ( Byte ) ( tmp[ i ] - '0' );
            }

            this.Normalize();
        }

        /// <summary>Multiplies the current expansion by the given amount, which should only be 2 or 5.</summary>
        public void MultiplyBy( Int32 amount ) {
            var result = new Byte[ this._digits.Length + 1 ];

            for ( var i = this._digits.Length - 1; i >= 0; i-- ) {
                var resultDigit = ( this._digits[ i ] * amount ) + result[ i + 1 ];
                result[ i ] = ( Byte ) ( resultDigit / 10 );
                result[ i + 1 ] = ( Byte ) ( resultDigit % 10 );
            }

            if ( result[ 0 ] != 0 ) {
                this._digits = result;
            }
            else {
                Buffer.BlockCopy( result, 1, this._digits, 0, this._digits.Length );
            }

            this.Normalize();
        }

        /// <summary>Removes leading/trailing zeroes from the expansion.</summary>
        public void Normalize() {
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

            if ( ( first == 0 ) && ( last == ( this._digits.Length - 1 ) ) ) {
                return;
            }

            var tmp = new Byte[ ( last - first ) + 1 ];

            for ( var i = 0; i < tmp.Length; i++ ) {
                tmp[ i ] = this._digits[ i + first ];
            }

            this._decimalPoint -= this._digits.Length - ( last + 1 );
            this._digits = tmp;
        }

        /// <summary>
        /// Shifts the System.Decimal point; a negative value makes the System.Decimal expansion bigger (as fewer digits come after the System.Decimal place) and a positive value
        /// makes the System.Decimal expansion smaller.
        /// </summary>
        public void Shift( Int32 amount ) => this._decimalPoint += amount;

        /// <summary>Converts the value to a proper System.Decimal String representation.</summary>
        [NotNull]
        public override String ToString() {
            var digitString = new Char[ this._digits.Length ];

            for ( var i = 0; i < this._digits.Length; i++ ) {
                digitString[ i ] = ( Char ) ( this._digits[ i ] + '0' );
            }

            // Simplest case - nothing after the System.Decimal point, and last real digit is
            // non-zero, eg value=35
            if ( this._decimalPoint == 0 ) {
                return new String( digitString );
            }

            // Fairly simple case - nothing after the System.Decimal point, but some 0s to add,
            // eg value=350
            if ( this._decimalPoint < 0 ) {
                return $"{new String( digitString )}{new String( '0', -this._decimalPoint )}";
            }

            // Nothing before the System.Decimal point, eg 0.035
            if ( this._decimalPoint >= digitString.Length ) {
                return $"0.{new String( '0', this._decimalPoint - digitString.Length )}{new String( digitString )}";
            }

            // Most complicated case - part of the String comes before the System.Decimal point,
            // part comes after it, eg 3.5
            return
                $"{new String( digitString, 0, digitString.Length - this._decimalPoint )}.{new String( digitString, digitString.Length - this._decimalPoint, this._decimalPoint )}";
        }

    }

}