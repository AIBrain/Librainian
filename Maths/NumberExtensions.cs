namespace Librainian.Maths {
    using System;
    using System.Linq;
    using System.Numerics;
    using FluentAssertions;
    using Numerics;
    using NUnit.Framework;
    using Parsing;
    using Threading;

    public static class NumberExtensions {

        /// <summary>
        /// Converts the given string into a <see cref="BigRational"/> or null.
        /// </summary>
        /// <param name="longDecimalString"></param>
        /// <returns></returns>
        public static BigRational? ToBigRational( this String longDecimalString ) {

            // all whitespace or none?
            if ( String.IsNullOrWhiteSpace( longDecimalString ) ) {
                return null;
            }

            longDecimalString = longDecimalString.Trim();
            if ( String.IsNullOrWhiteSpace( longDecimalString ) ) {
                return null;
            }

            if ( longDecimalString.Contains( "E" ) ) {
                //TODO add in subset for parsing numbers like "3.14E15" (scientific notation?)
                throw new NotImplementedException();
                return null;
            }

            if ( longDecimalString.Contains( "^" ) ) {

                //TODO add in subset for parsing numbers like "3.14^15"? (exponential notation?)

                //TODO add in subset for parsing numbers like "3.14X10^15"? (exponential notation?)
                throw new NotImplementedException();
                return null;
            }

            //for parsing large decimals
            if ( !longDecimalString.Contains( "." ) ) {
                longDecimalString += ".0";
            }

            // too many of the allowed symbols
            if ( longDecimalString.Count( '.' ) > 1 || longDecimalString.Count( '-' ) > 1 ) {
                return null;
            }

            // all chars must be a digit, a period, or a negative sign.
            if ( longDecimalString.Any( c => !Char.IsDigit( c ) && c != '.' && c != '-' ) ) {
                return null;
            }

            var split = longDecimalString.Split( '.' );
            split.Should().HaveCount( expected: 2, because: "otherwise invalid" );

            BigInteger whole;
            BigInteger fraction;

            if ( !BigInteger.TryParse( split[ 0 ], out whole ) ) {
                //we were unable to parse the first string (all to the left of the decimal point)
                return null;
            }

            if ( !BigInteger.TryParse( split[ 1 ], out fraction ) ) {
                //we were unable to parse the second string (all to the right of the decimal point)
                return null;
            }

            var fractionLength = fraction.ToString().Length;

            var ratio = BigInteger.Pow( 10, fractionLength ); //we want the ratio of top/bottom to scale up past the decimal

            whole *= ratio;     //append a lot of zeroes

            whole += fraction;  //reconstruct the part that was after the decimal point

            var rational = new BigRational( whole, ratio );

            return rational;

            //TODO how to losslessly convert to a bigdecimal? the exponent..
            //we have a rational number here. 4 over 5. or 4/5
            //... how to convert it back to a BigDecimal ?
            //BigDecimal bob = new BigDecimal( rational. );

            //TODO does BigRational already reduce?
            //var leastCommonDenominator = this._value.LeastCommonDenominator( bob.Numerator, bob.Denominator );
            //bob /= leastCommonDenominator;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Number"/>
        [Test]
        public static Boolean TestNumberParsings() {

            //var bob = "18913489007071346701367013467767613401616136.136301590214084662236232265343672235925607263623468709823672366";
            var bob = String.Format( "{0}.{1}", Randem.NextString( length: 31, numbers: true ), Randem.NextString( length: 31, numbers: true ) );
            bob = "-18913489007071346701367013467767613401616136.136301590214084662236232265343672235925607263623468709823672366";


            BigInteger beforeDecimalPoint;
            BigInteger afterDecimalPoint;
            Number? sdgasdgd;
            var result = Number.TryParseNumber( bob, out sdgasdgd );
            return result;
        }
    }
}