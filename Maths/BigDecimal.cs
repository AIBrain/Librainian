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
// "Librainian2/BigDecimal.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Numerics;
    using Annotations;
    using NUnit.Framework;
    using Parsing;

    /// <summary>
    ///     <para>Arbitrary precision Decimal.</para>
    ///     <para>
    ///         All operations are exact, except for division. Division never determines more digits than the given
    ///         precision.
    ///     </para>
    ///     <para>Based on http://stackoverflow.com/a/4524254</para>
    ///     <para>Author: Jan Christoph Bernack (contact: jc.bernack at googlemail.com)</para>
    /// </summary>
    /// <see cref="http://stackoverflow.com/a/13813535/956364" />
    [UsedImplicitly]
    public struct BigDecimal : IComparable, IComparable< BigDecimal > {
        /// <summary>
        ///     Sets the maximum precision of division operations.
        ///     If AlwaysTruncate is set to true all operations are affected.
        /// </summary>
        public const int Precision = 100;

        public static readonly BigDecimal One = new BigDecimal( number: 1 );

        public static readonly BigDecimal Zero = new BigDecimal( mantissa: 0, exponent: 0 );

        public readonly Int64 Exponent;

        public readonly BigInteger Mantissa;

        public BigDecimal( BigDecimal bigDecimal ) : this( mantissa: bigDecimal.Mantissa, exponent: bigDecimal.Exponent ) { }

        public BigDecimal( Decimal number ) : this( bigDecimal: number ) { }

        /// <summary>
        /// </summary>
        /// <param name="mantissa"></param>
        /// <param name="exponent"></param>
        public BigDecimal( BigInteger mantissa, Int64 exponent ) : this() {
            this.Mantissa = mantissa;
            this.Exponent = exponent;

            //BUG is this correct?
            if ( this.Mantissa.IsZero ) {
                this.Exponent = 0;
            }
            else {
                BigInteger remainder = 0;
                while ( remainder == 0 ) {
                    var shortened = BigInteger.DivRem( dividend: this.Mantissa, divisor: 10, remainder: out remainder );
                    if ( remainder != 0 ) {
                        continue;
                    }
                    this.Mantissa = shortened;
                    this.Exponent++;
                }
            }
        }

        public int CompareTo( [CanBeNull] object obj ) {
            if ( ReferenceEquals( obj, null ) || !( obj is BigDecimal ) ) {
                throw new ArgumentException();
            }
            return this.CompareTo( ( BigDecimal ) obj );
        }

        public int CompareTo( BigDecimal other ) {
            return this < other ? -1 : ( this > other ? 1 : 0 );
        }

        /// <summary>
        ///     Returns the mantissa of <paramref name="value" />, aligned to the exponent of reference.
        ///     Assumes the exponent of <paramref name="value" /> is larger than of value.
        /// </summary>
        public static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) {
            Assert.GreaterOrEqual( value.Exponent, reference.Exponent );
            return value.Mantissa*BigInteger.Pow( value: 10, exponent: ( Int32 ) ( value.Exponent - reference.Exponent ) ); //BUG cast.
        }

        /// <summary>
        ///     Static equality check for <paramref name="left" /> against <paramref name="right" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( BigDecimal left, BigDecimal right ) {
            return left.Mantissa.Equals( right.Mantissa ) && left.Exponent.Equals( right.Exponent );
        }

        public static BigDecimal Exp( Double exponent ) {
            var tmp = One;
            while ( Math.Abs( exponent ) > 100 ) {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Exp( diff );
                exponent -= diff;
            }
            return tmp*Math.Exp( d: exponent );
        }

        public static explicit operator Double( BigDecimal value ) {
            return ( Double ) value.Mantissa*Math.Pow( 10, value.Exponent );
        }

        public static implicit operator BigDecimal( Int64 number ) {
            return new BigDecimal( mantissa: number, exponent: 0 );
        }

        /// <summary>
        ///     Do not know if casting and math here is correct (bug free and overflow free)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static implicit operator BigDecimal( Double number ) {
            var mantissa = new BigInteger( value: number );
            var exponent = 0L;
            Double scaleFactor = 1;
            while ( Math.Abs( number*scaleFactor - ( Double ) mantissa ) > 0 ) {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = ( BigInteger ) ( number*scaleFactor );
            }
            return new BigDecimal( mantissa: mantissa, exponent: exponent );
        }

        /// <summary>
        ///     Do not know if casting and math here is correct (bug free and overflow free)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BigDecimal( Decimal value ) {
            var mantissa = ( BigInteger ) value;
            var exponent = 0L;
            Decimal scaleFactor = 1;
            while ( ( Decimal ) mantissa != value*scaleFactor ) {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = new BigInteger( value: value*scaleFactor );
            }
            return new BigDecimal( mantissa: mantissa, exponent: exponent );
        }

        public static BigDecimal operator -( BigDecimal number ) {
            //value.Mantissa *= -1;
            return new BigDecimal( mantissa: number.Mantissa*-1, exponent: number.Exponent ); //BUG is this correct?
        }

        public static BigDecimal operator -( BigDecimal left, BigDecimal right ) {
            return Add( left: left, right: -right );
        }

        public static BigDecimal operator --( BigDecimal number ) {
            return number - 1;
        }

        public static Boolean operator !=( BigDecimal left, BigDecimal right ) {
            return !Equals( left, right );
        }

        public static BigDecimal operator *( BigDecimal left, BigDecimal right ) {
            return new BigDecimal( mantissa: left.Mantissa*right.Mantissa, exponent: left.Exponent + right.Exponent );
        }

        public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) {
            var exponentChange = Precision - ( dividend.Mantissa.NumberOfDigits() - divisor.Mantissa.NumberOfDigits() );
            if ( exponentChange < 0 ) {
                exponentChange = 0;
            }
            //dividend.Mantissa *= BigInteger.Pow( 10, exponentChange );
            var newdividend = new BigDecimal( mantissa: dividend.Mantissa*BigInteger.Pow( 10, ( int ) exponentChange ), exponent: dividend.Exponent ); //BUG is this correct? I don't like that cast..
            return new BigDecimal( newdividend.Mantissa/divisor.Mantissa, newdividend.Exponent - divisor.Exponent - exponentChange );
        }

        public static BigDecimal operator +( BigDecimal number ) {
            return number;
        }

        public static BigDecimal operator +( BigDecimal left, BigDecimal right ) {
            return Add( left, right );
        }

        public static BigDecimal operator ++( BigDecimal number ) {
            return Add( number, 1 );
        }

        public static Boolean operator <( BigDecimal left, BigDecimal right ) {
            return left.Exponent > right.Exponent ? AlignExponent( left, right ) < right.Mantissa : left.Mantissa < AlignExponent( right, left );
        }

        public static Boolean operator <=( BigDecimal left, BigDecimal right ) {
            return left.Exponent > right.Exponent ? AlignExponent( left, right ) <= right.Mantissa : left.Mantissa <= AlignExponent( right, left );
        }

        public static Boolean operator ==( BigDecimal left, BigDecimal right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( BigDecimal left, BigDecimal right ) {
            return left.Exponent > right.Exponent ? AlignExponent( left, right ) > right.Mantissa : left.Mantissa > AlignExponent( right, left );
        }

        public static Boolean operator >=( BigDecimal left, BigDecimal right ) {
            return left.Exponent > right.Exponent ? AlignExponent( left, right ) >= right.Mantissa : left.Mantissa >= AlignExponent( right, left );
        }

        public static BigDecimal Pow( Double basis, Double exponent ) {
            BigDecimal tmp = 1;
            while ( Math.Abs( exponent ) > 100 ) {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Pow( basis, diff );
                exponent -= diff;
            }
            return tmp*Math.Pow( basis, exponent );
        }

        ///// <summary>
        /////     Truncate the number to the given precision by removing the least significant digits.
        ///// </summary>
        ///// <returns>The truncated number</returns>
        //public static BigDecimal Truncate( BigDecimal bigDecimal, int precision = Precision ) {
        //    // copy this instance (remember its a struct)
        //    var shortened = bigDecimal;
        //    // save some time because the number of digits is not needed to remove trailing zeros
        //    Normalize( shortened );
        //    // remove the least significant digits, as long as the number of digits is higher than the given Precision
        //    while ( NumberOfDigits( shortened.Mantissa ) > precision ) {
        //        //shortened.Mantissa = shortened.Mantissa / 10;
        //        //shortened.Exponent++;
        //        shortened = new BigDecimal( mantissa: shortened.Mantissa / 10, exponent: shortened.Exponent++  );
        //    }
        //    return shortened;
        //}

        public Boolean Equals( BigDecimal other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is BigDecimal && Equals( this, ( BigDecimal ) obj );
        }

        public override int GetHashCode() {
            return this.Mantissa.GetHashMerge( this.Exponent );
        }

        public override string ToString() {
            return String.Concat( this.Mantissa.ToString(), "E", this.Exponent );
        }

        private static BigDecimal Add( BigDecimal left, BigDecimal right ) {
            return left.Exponent > right.Exponent ? new BigDecimal( mantissa: AlignExponent( left, right ) + right.Mantissa, exponent: right.Exponent ) : new BigDecimal( mantissa: AlignExponent( right, left ) + left.Mantissa, exponent: left.Exponent );
        }

        //public static explicit operator float( BigDecimal value ) {
        //    return Convert.ToSingle( ( Double )value );
        //}

        public static explicit operator Decimal( BigDecimal value ) {
            return ( Decimal ) value.Mantissa*( Decimal ) Math.Pow( 10, value.Exponent );
        }

        //public static explicit operator int( BigDecimal value ) {
        //    return ( int )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
        //}

        //public static explicit operator uint( BigDecimal value ) {
        //    return ( uint )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
        //}
    }
}
