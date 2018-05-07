// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/RomanConverter.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Numbers;

    /// <summary>Based on the idea from lavz24.</summary>
    /// <seealso cref="http://github.com/lavz24/DecimalToRoman/blob/master/DecimalToRoman/Converter.cs" />
    public static class RomanConverter {

        /// <summary></summary>
        public static readonly RomanNumber[] RomanValues = { RomanNumber.I, RomanNumber.Iv, RomanNumber.V, RomanNumber.Ix, RomanNumber.X, RomanNumber.Xl, RomanNumber.L, RomanNumber.Xc, RomanNumber.C, RomanNumber.Cd, RomanNumber.D, RomanNumber.Cm, RomanNumber.M };

        /// <summary>
        ///     <para>Returns the roman numeral for a <paramref name="number" /> between 1 and 3999.</para>
        ///     <para>Or String.Empty in case of failure.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static String ToRoman( this Int16 number ) {
            if ( !number.Between( ( Int16 )1, ( Int16 )3999 ) ) {

                // per https://en.wikipedia.org/wiki/Roman_numerals#Large_numbers
                //throw new ArgumentOutOfRangeException( "number" );
                return String.Empty;
            }

            var result = new Queue<RomanNumber>();

            var currentRoman = RomanValues.Length - 1;

            for ( var i = ( Int32 )number; i > 0; ) {
                if ( i < ( Int32 )RomanValues[ currentRoman ] ) {
                    --currentRoman;
                }
                else {
                    result.Enqueue( RomanValues[ currentRoman ] );
                    i -= ( Int32 )RomanValues[ currentRoman ];
                }
            }

            return result.Aggregate( String.Empty, ( current, romanNumber ) => current + romanNumber.ToString() );
        }
    }
}