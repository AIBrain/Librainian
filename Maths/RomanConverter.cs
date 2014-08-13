namespace Librainian.Maths {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://github.com/lavz24/DecimalToRoman/blob/master/DecimalToRoman/Converter.cs" />
    public static class RomanConverter {
        public static readonly RomanNumber[] RomanValues = {
                                                               RomanNumber.I, RomanNumber.IV, RomanNumber.V,
                                                               RomanNumber.IX, RomanNumber.X, RomanNumber.XL,
                                                               RomanNumber.L, RomanNumber.XC, RomanNumber.C, RomanNumber.CD,
                                                               RomanNumber.D, RomanNumber.CM, RomanNumber.M };

        public static string ToRoman( this short number ) {

            if ( !number.Between( ( short )1, ( short )3999 ) ) {
                throw new ArgumentOutOfRangeException( "number" );
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