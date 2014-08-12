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
// "Librainian/Number.cs" was last cleaned by Rick on 2014/08/12 at 8:51 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Linq;
    using System.Numerics;
    using Annotations;
    using Extensions;
    using FluentAssertions;
    using Numerics;
    using NUnit.Framework;
    using Threading;

    public static class NumberExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Number"/>
        [Test]
        public static Boolean TestNumberParsings() {

            //var bob = "18913489007071346701367013467767613401616136.136301590214084662236232265343672235925607263623468709823672366";
            var bob = String.Format( "{0}.{1}", Randem.NextString( length: 31, numbers: true ), Randem.NextString( length: 31, numbers: true ) );


            BigInteger beforeDecimalPoint;
            BigInteger afterDecimalPoint;
            BigRational sdgasdgd;
            var result = Number.TryParseNumber( bob, out sdgasdgd );
            return result;
        }
    }

    /// <summary>
    /// A number struct that can hold any real number.
    /// Decimal / Decimal
    /// </summary>
    /// <seealso cref="NumberExtensions.TestNumberParsings"/>
    [Immutable]
    public struct Number {
        public readonly BigDecimal Answer;
        public readonly BigDecimal Denominator;
        public readonly BigDecimal Numerator;

        internal readonly BigRational BigRational;



        public Number( [NotNull] String bigHugeDecimalNumber ) {
            if ( bigHugeDecimalNumber == null ) {
                throw new ArgumentNullException( "bigHugeDecimalNumber" );
            }

            BigInteger wholePart;
            BigInteger fractionalPart;
            BigRational sdgsdfgsdgfds;

            if ( !TryParseNumber( bigHugeDecimalNumber, out this.BigRational ) ) {
                throw new ArgumentOutOfRangeException( "bigHugeDecimalNumber", String.Format("Unable to parse a number from the string {0}", bigHugeDecimalNumber) );
            }
            //we now have a fraction, divvyed into the whole part and fraction part.
            //var bobW = new Fraction( wholePart );
            //var bobF = new Fraction( fractionalPart );
            //Fraction.

            //var jane = bobW + bobF;

            this.Answer = new BigDecimal();
            this.Denominator = new BigDecimal();
            this.Numerator = new BigDecimal();
        }

        public Number( BigDecimal numerator, BigDecimal denominator ) {

            BigInteger numeratorMultiplier = BigInteger.One;
            while ( Math.Truncate( ( Decimal )numerator ) > ( Decimal )numerator ) {
                numerator *= 10;
                numeratorMultiplier *= 10;
            }

            this.Answer = numerator/denominator;

            this.BigRational = new BigRational( ( Decimal ) this.Answer );

            this.Numerator = new BigDecimal( this.BigRational.Numerator, 0 );

            this.Denominator = new BigDecimal( this.BigRational.GetWholePart(), 0 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rational"></param>
        /// <returns></returns>
        public static Boolean TryParseNumber( [NotNull] String value, out BigRational rational ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }


            var theString = value.Trim();

            if ( !theString.Contains( "." ) ) {
                theString += ".0";
            }

            if ( theString.Any( c => !Char.IsDigit( c ) && c != '.' ) ) {
                rational = new BigRational();
                return false;
            }

            var split = theString.Split( '.' );
            split.Should().HaveCount( expected: 2, because: "otherwise invalid" );

            BigInteger wholePart;
            BigInteger fractionalPart;

            BigInteger.TryParse( split[ 0 ], out wholePart );

            BigInteger.TryParse( split[ 1 ], out fractionalPart );

            var fractionLength = fractionalPart.ToString().Length;

            var ratio = BigInteger.Pow( 10, fractionLength); //we want the ratio of top/bottom to scale up past the decimal.. right?

            wholePart *= ratio;

            var newNumber = wholePart + fractionalPart;
            newNumber.ToString().Length.Should().Be( theString.Length - 1 );    //because we took out the period

            rational = new BigRational( newNumber, ratio );
            var leastCommonDenominator = BigRational.LeastCommonDenominator( rational.Numerator, rational.Denominator );

            rational /= leastCommonDenominator;

            return true;
        }
    }
}
