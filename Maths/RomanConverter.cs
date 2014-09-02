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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/RomanConverter.cs" was last cleaned by Rick on 2014/08/13 at 10:35 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Based on the idea from lavz24.
    /// </summary>
    /// <seealso cref="http://github.com/lavz24/DecimalToRoman/blob/master/DecimalToRoman/Converter.cs" />
    public static class RomanConverter {

        /// <summary>
        /// 
        /// </summary>
        public static readonly RomanNumber[] RomanValues = { RomanNumber.I, RomanNumber.IV, RomanNumber.V, RomanNumber.IX, RomanNumber.X, RomanNumber.XL, RomanNumber.L, RomanNumber.XC, RomanNumber.C, RomanNumber.CD, RomanNumber.D, RomanNumber.CM, RomanNumber.M };

        /// <summary>
        /// <para>Returns the roman numeral for a <paramref name="number"/> between 1 and 3999.</para>
        /// <para>Or String.Empty in case of failure.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToRoman( this short number ) {
            if ( !number.Between( ( short )1, ( short )3999 ) ) {
                // per https://en.wikipedia.org/wiki/Roman_numerals#Large_numbers
                //throw new ArgumentOutOfRangeException( "number" );
                return String.Empty;
            }

            var result = new Queue<RomanNumber>();

            var currentRoman = RomanValues.Length - 1;

            for ( var i = ( int )number; i > 0; ) {
                if ( i < ( int )RomanValues[ currentRoman ] ) {
                    --currentRoman;
                } else {
                    result.Enqueue( RomanValues[ currentRoman ] );
                    i -= ( int )RomanValues[ currentRoman ];
                }
            }

            return result.Aggregate( String.Empty, ( current, romanNumber ) => current + romanNumber.ToString() );
        }
    }
}