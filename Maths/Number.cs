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

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Linq;
    using System.Numerics;
    using Annotations;
    using Extensions;
    using FluentAssertions;
    using MathNet.Numerics;
    using Numerics;
    using NUnit.Framework;
    using Parsing;

    /// <summary>
    /// A number struct that can hold any real number.
    /// Decimal / Decimal
    /// </summary>
    /// <seealso cref="NumberExtensions.TestNumberParsings"/>
    [Immutable]
    public struct Number {

        private readonly BigDecimal _answer;

        public BigDecimal Answer {
            get {
                return this._answer;
            }
        }

        //public readonly BigDecimal Numerator;

        //private readonly Number? _value;

        //public Number( [NotNull] String bigHugeDecimalNumber ) {
        //    if ( bigHugeDecimalNumber == null ) {
        //        throw new ArgumentNullException( "bigHugeDecimalNumber" );
        //    }

        //    if ( !TryParseNumber( bigHugeDecimalNumber, out this._value ) ) {
        //        throw new ArgumentOutOfRangeException( "bigHugeDecimalNumber", String.Format( "Unable to parse a number from the string {0}", bigHugeDecimalNumber ) );
        //    }
        //    //we now have a fraction, divvyed into the whole part and fraction part.
        //    //var bobW = new Fraction( wholePart );
        //    //var bobF = new Fraction( fractionalPart );
        //    //Fraction.

        //    //var jane = bobW + bobF;

        //    this.Answer = new BigDecimal();
        //    this.Denominator = new BigDecimal();
        //    this.Numerator = new BigDecimal();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Number( BigDecimal numerator, BigDecimal denominator ) {
            var numeratorMultiplier = BigInteger.One;
            while ( Math.Truncate( ( Decimal )numerator ) > ( Decimal )numerator ) {
                numerator *= 10;
                numeratorMultiplier *= 10;
            }

            this._answer = numerator / denominator;

            //this._value = new BigRational( ( Decimal )this.Answer );

            //this.Numerator = new BigDecimal( this._value.Numerator, 0 );

            //this.Denominator = new BigDecimal( this._value.GetWholePart(), 0 );
        }

        public Number( BigInteger numerator, BigInteger denominator ) {
            var bob = new BigRational( numerator, denominator );

            var bobAsString = String.Format("{0}", bob);

            MathNet.Numerics.Random.RandomExtensions.NextBytes

            this._answer = (Decimal)bob;
        }

        private Number( BigRational bigRational ) {
            throw new NotImplementedException();
        }


        //public Number( BigRational value ) {
        //    this._value = value;

        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static Boolean TryParseNumber( [CanBeNull] String value, out Number? number ) {
            number = null;

            // all whitespace or none?
            if ( String.IsNullOrWhiteSpace( value ) ) {
                return false;
            }

            value = value.Trim();
            if ( String.IsNullOrWhiteSpace( value ) ) {
                return false;
            }

            if ( value.Contains( "E" ) ) {
                //TODO add in subset for parsing numbers like "3.14E15" (scientific notation?)
                throw new NotImplementedException();
                return false;
            }

            if ( value.Contains( "^" ) ) {
                //TODO add in subset for parsing numbers like "3.14^15"? (exponential notation?)

                //TODO add in subset for parsing numbers like "3.14X10^15"? (exponential notation?)
                throw new NotImplementedException();
                return false;
            }

            //for parsing large decimals
            if ( !value.Contains( "." ) ) {
                value += ".0";
            }

            // too many of the allowed symbols
            if ( value.Count( '.' ) > 1 || value.Count( '-' ) > 1 ) {
                return false;
            }

            // all chars must be a digit, a period, or a negative sign.
            if ( value.Any( c => !Char.IsDigit( c ) && c != '.' && c != '-' ) ) {
                return false;
            }

            var split = value.Split( '.' );
            split.Should().HaveCount( expected: 2, because: "otherwise invalid" );

            BigInteger whole;
            BigInteger fraction;

            if ( !BigInteger.TryParse( split[ 0 ], out whole ) ) {
                //we were unable to parse the first string (all to the left of the decimal point)
                return false;
            }

            if ( !BigInteger.TryParse( split[ 1 ], out fraction ) ) {
                //we were unable to parse the second string (all to the right of the decimal point)
                return false;
            }

            var fractionLength = fraction.ToString().Length;

            var ratio = BigInteger.Pow( 10, fractionLength ); //we want the ratio of top/bottom to scale up past the decimal

            whole *= ratio;     //append a lot of zeroes

            whole += fraction;  //reconstruct the part that was after the decimal point

            var bob = new BigRational( whole, ratio );

            //TODO does BigRational already reduce?
            //var leastCommonDenominator = this._value.LeastCommonDenominator( bob.Numerator, bob.Denominator );
            //bob /= leastCommonDenominator;

            number = new Number( bob );

            return true;
        }
    }
}