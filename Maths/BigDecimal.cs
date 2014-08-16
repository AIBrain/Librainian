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
// "Librainian/BigDecimal.cs" was last cleaned by Rick on 2014/08/16 at 4:40 PM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Linq;
    using System.Numerics;
    using System.Runtime;
    using Annotations;
    using Extensions;
    using FluentAssertions;
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
    ///     <para>Joined with code from nberardi from gist 2667136</para>
    ///     <para>Rewritten into an immutable struct by Rick in August 2014</para>
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/a/13813535/956364" />
    /// <seealso cref="http://gist.github.com/nberardi/2667136" />
    [UsedImplicitly]
    [Immutable]
    public struct BigDecimal : IComparable, IComparable<BigDecimal>, IConvertible, IFormattable, IEquatable<BigDecimal> {

        /// <summary>
        ///     1
        /// </summary>
        public static readonly BigDecimal One = new BigDecimal( Decimal.One );

        /// <summary>
        ///     0
        /// </summary>
        public static readonly BigDecimal Zero = new BigDecimal( Decimal.Zero );

        /// <summary>
        ///     -1
        /// </summary>
        public static readonly BigDecimal MinusOne = new BigDecimal( Decimal.MinusOne );

        /// <summary>
        /// </summary>
        /// <seealso cref="http://wikipedia.org/wiki/Exponent" />
        public readonly Int32 Exponent;

        /// <summary>
        ///     The <see cref="Significand" /> (aka <see cref="Mantissa" />) is the part of a number consisting of its significant
        ///     digits.
        /// </summary>
        /// <seealso cref="http://wikipedia.org/wiki/Significand" />
        /// <seealso cref="Mantissa" />
        public readonly BigInteger Significand;

        public BigDecimal( BigDecimal bigDecimal )
            : this( bigDecimal.Mantissa, bigDecimal.Exponent ) {
        }

        public BigDecimal( Decimal value )
            : this( bigDecimal: value ) {
        }

        public BigDecimal( Double value )
            : this( ( Decimal )value ) {
        }

        public BigDecimal( float value )
            : this( ( Decimal )value ) {
        }

        public BigDecimal( BigInteger significand, Int32 exponent ) {
            this.Significand = significand;
            this.Exponent = exponent;
            //BUG is this correct?
        }

        public BigDecimal( Int32 value )
            : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( Int64 value )
            : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( UInt32 value )
            : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( UInt64 value )
            : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( Byte[] value ) {
            if ( value.Length < 5 ) {
                throw new ArgumentOutOfRangeException( "value", "Not enough bytes to construct the Significand" );
            }

            if ( !value.Length.CanAllocateMemory() ) {
                throw new ArgumentOutOfRangeException( "value", "'value' is too large to allocate" );
            }

            var number = new Byte[ value.Length - 4 ];
            var flags = new Byte[ 4 ];

            Buffer.BlockCopy( value, 0, number, 0, number.Length );
            Buffer.BlockCopy( value, value.Length - 4, flags, 0, 4 );

            this.Significand = new BigInteger( value );
            this.Exponent = BitConverter.ToInt32( flags, 0 );
        }

        /// <summary>
        ///     The significand (aka mantissa) is part of a number consisting of its significant digits.
        /// </summary>
        /// <seealso cref="Significand" />
        public BigInteger Mantissa { get { return this.Significand; } }

        public Boolean IsEven { get { return this.Significand.IsEven; } }

        public Boolean IsOne { get { return this.Significand.IsOne; } }

        public Boolean IsPowerOfTwo { get { return this.Significand.IsPowerOfTwo; } }

        public Boolean IsZero { get { return this.Significand.IsZero; } }

        public Int32 Sign { get { return this.Significand.Sign; } }

        ///// <summary>
        ///// </summary>
        ///// <param name="mantissa"></param>
        ///// <param name="exponent"></param>
        //public BigDecimal( BigInteger mantissa, Int32 exponent )
        //    : this() {
        //    this.Significand = mantissa;
        //    this.Exponent = exponent;

        //    //BUG is this correct?
        //    if ( this.Mantissa.IsZero ) {
        //        this.Exponent = 0;
        //    }
        //    else {
        //        BigInteger remainder = 0;
        //        while ( remainder == 0 ) {
        //            var shortened = BigInteger.DivRem( dividend: this.Mantissa, divisor: 10, remainder: out remainder );
        //            if ( remainder == 0 ) {
        //                this.Significand = shortened;
        //                this.Exponent++;
        //            }
        //        }
        //    }
        //}

        public Int32 CompareTo( [CanBeNull] object obj ) {
            if ( ReferenceEquals( obj, null ) || !( obj is BigDecimal ) ) {
                throw new ArgumentException();
            }
            return this.CompareTo( ( BigDecimal )obj );
        }

        public Int32 CompareTo( BigDecimal other ) {
            if ( this < other ) {
                return -1;
            }
            if ( this > other ) {
                return 1;
            }
            return 0;
        }

        object IConvertible.ToType( Type conversionType, IFormatProvider provider ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), this.Exponent );
            var remainder = BigInteger.Remainder( this.Significand, scaleDivisor );
            var scaledValue = BigInteger.Divide( this.Significand, scaleDivisor );

            if ( scaledValue > new BigInteger( Decimal.MaxValue ) ) {
                throw new ArgumentOutOfRangeException( "provider", string.Format( "The value {0} cannot fit into {1}.", this.Significand, conversionType.Name ) );
            }

            var leftOfDecimal = ( Decimal )scaledValue;
            var rightOfDecimal = ( ( Decimal )remainder ) / ( ( Decimal )scaleDivisor );

            var value = leftOfDecimal + rightOfDecimal;
            return Convert.ChangeType( value, conversionType );
        }

        TypeCode IConvertible.GetTypeCode() {
            return TypeCode.Object;
        }

        Boolean IConvertible.ToBoolean( IFormatProvider provider ) {
            return Convert.ToBoolean( this );
        }

        Byte IConvertible.ToByte( IFormatProvider provider ) {
            return Convert.ToByte( this );
        }

        char IConvertible.ToChar( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal2 to Char" );
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal2 to DateTime" );
        }

        Decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            return Convert.ToDecimal( this );
        }

        Double IConvertible.ToDouble( IFormatProvider provider ) {
            return Convert.ToDouble( this );
        }

        short IConvertible.ToInt16( IFormatProvider provider ) {
            return Convert.ToInt16( this );
        }

        Int32 IConvertible.ToInt32( IFormatProvider provider ) {
            return Convert.ToInt32( this );
        }

        Int64 IConvertible.ToInt64( IFormatProvider provider ) {
            return Convert.ToInt64( this );
        }

        sbyte IConvertible.ToSByte( IFormatProvider provider ) {
            return Convert.ToSByte( this );
        }

        float IConvertible.ToSingle( IFormatProvider provider ) {
            return Convert.ToSingle( this );
        }

        string IConvertible.ToString( IFormatProvider provider ) {
            return Convert.ToString( this );
        }

        ushort IConvertible.ToUInt16( IFormatProvider provider ) {
            return Convert.ToUInt16( this );
        }

        UInt32 IConvertible.ToUInt32( IFormatProvider provider ) {
            return Convert.ToUInt32( this );
        }

        UInt64 IConvertible.ToUInt64( IFormatProvider provider ) {
            return Convert.ToUInt64( this );
        }

        public Boolean Equals( BigDecimal other ) {
            return Equals( this, other );
        }

        public string ToString( string format, IFormatProvider formatProvider ) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns the mantissa of <paramref name="value" />, aligned to the exponent of reference.
        ///     Assumes the exponent of <paramref name="value" /> is larger than of value.
        /// </summary>
        public static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) {
            Assert.GreaterOrEqual( value.Exponent, reference.Exponent );
            return value.Mantissa * BigInteger.Pow( value: 10, exponent: value.Exponent - reference.Exponent );
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
            return tmp * Math.Exp( d: exponent );
        }

        public static explicit operator Double( BigDecimal value ) {
            return ( Double )value.Mantissa * Math.Pow( 10, value.Exponent );
        }

        public static implicit operator BigDecimal( Int64 number ) {
            return new BigDecimal( number, 0 );
        }

        /// <summary>
        ///     Do not know if casting and math here is correct (bug free and overflow free)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static implicit operator BigDecimal( Double number ) {
            var mantissa = new BigInteger( value: number );
            var exponent = 0;
            Double scaleFactor = 1;
            while ( Math.Abs( number * scaleFactor - ( Double )mantissa ) > 0 ) {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = ( BigInteger )( number * scaleFactor );
            }
            return new BigDecimal( mantissa, exponent );
        }

        /// <summary>
        ///     Do not know if casting and math here is correct (bug free and overflow free)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BigDecimal( Decimal value ) {
            var mantissa = ( BigInteger )value;
            var exponent = 0;
            Decimal scaleFactor = 1;
            while ( ( Decimal )mantissa != value * scaleFactor ) {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = new BigInteger( value: value * scaleFactor );
            }
            return new BigDecimal( mantissa, exponent );
        }

        public static BigDecimal operator -( BigDecimal number ) {
            //value.Mantissa *= -1;
            return new BigDecimal( number.Mantissa * -1, number.Exponent ); //BUG is this correct?
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
            return Multiply( left, right );
        }

        [Pure]
        public static BigDecimal Multiply( BigDecimal left, BigDecimal right ) {
            return new BigDecimal( left.Mantissa * right.Mantissa, left.Exponent + right.Exponent );
        }

        public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) {
            var exponentChange = 100 - ( dividend.Mantissa.NumberOfDigits() - divisor.Mantissa.NumberOfDigits() );
            if ( exponentChange < 0 ) {
                exponentChange = 0;
            }

            //TODO this needs unit tested.
            var newdividend = new BigDecimal( dividend.Mantissa * BigInteger.Pow( 10, exponentChange ), dividend.Exponent ); //BUG is this correct?

            return new BigDecimal( newdividend.Mantissa / divisor.Mantissa, newdividend.Exponent - divisor.Exponent - exponentChange );
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
            return tmp * Math.Pow( basis, exponent );
        }

        ///// <summary>
        /////     Truncate the number to the given precision by removing the least significant digits.
        ///// </summary>
        ///// <returns>The truncated number</returns>
        //public static BigDecimal Truncate( BigDecimal bigDecimal, Int32 precision = Precision ) {
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

        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is BigDecimal && Equals( this, ( BigDecimal )obj );
        }

        public override Int32 GetHashCode() {
            return this.Mantissa.GetHashMerge( this.Exponent );
        }

        public string ToStringWithE() {
            //TODO what is the proper name for this format? "123E45"?
            return String.Concat( this.Mantissa.ToString(), "E", this.Exponent );
        }

        [Pure]
        public override string ToString() {
            var number = this.Significand.ToString();

            return this.Exponent > 0 ? number.Insert( number.Length - this.Exponent, "." ) : number;
        }

        private static BigDecimal Add( BigDecimal left, BigDecimal right ) {
            if ( left.Exponent > right.Exponent ) {
                return new BigDecimal( AlignExponent( left, right ) + right.Mantissa, exponent: right.Exponent );
            }
            return new BigDecimal( AlignExponent( right, left ) + left.Mantissa, exponent: left.Exponent );
        }

        public static explicit operator float( BigDecimal value ) {
            return Convert.ToSingle( ( Double )value );
        }

        public static explicit operator Decimal( BigDecimal value ) {
            return ( Decimal )value.Mantissa * ( Decimal )Math.Pow( 10, value.Exponent );
        }

        //public static explicit operator BigInteger( BigDecimal value ) {
        //    var man = (BigDecimal)value.Mantissa;
        //    new BigDecimal(
        //    man *= BigDecimal.Pow( 10, value.Exponent );

        //    return ( Decimal )value.Mantissa * ( Decimal )Math.Pow( 10, value.Exponent );
        //}

        //public static explicit operator Int32( BigDecimal value ) {
        //    return ( Int32 )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
        //}

        //public static explicit operator uint( BigDecimal value ) {
        //    return ( uint )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
        //}

        /// <summary>
        ///     TODO this needs unit tested.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator BigInteger( BigDecimal value ) {
            var scaleDivisor = BigInteger.Pow( 10, value.Exponent );
            var scaledValue = BigInteger.Divide( value.Mantissa, scaleDivisor );
            return scaledValue;
        }

        /// <summary>
        ///     Attempt to parse a huge decimal from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static Boolean TryParse( [CanBeNull] String value, out BigDecimal? number, out String whyParseFailed ) {
            whyParseFailed = String.Empty;
            number = null;

            // all whitespace or none?
            if ( String.IsNullOrWhiteSpace( value ) ) {
                whyParseFailed = "'value' is null or contained only whitespace";
                return false;
            }

            value = value.Trim();
            if ( String.IsNullOrWhiteSpace( value ) ) {
                whyParseFailed = "'value' is null or contained only whitespace";
                return false;
            }

            if ( value.Contains( "E" ) ) {
                whyParseFailed = "not implemented yet";
                //TODO add in subset for parsing numbers like "3.14E15" (scientific notation?)
                throw new NotImplementedException();
                return false;
            }

            if ( value.Contains( "^" ) ) {
                whyParseFailed = "not implemented yet";
                //TODO add in subset for parsing numbers like "3.14^15"? (exponential notation?)

                //TODO add in subset for parsing numbers like "3.14X10^15"? (exponential notation?)
                throw new NotImplementedException();
                return false;
            }

            if ( !value.Contains( "." ) ) {
                value += ".0"; //for parsing large decimals
            }

            if ( value.Count( '.' ) > 1 ) {
                whyParseFailed = "'value' contained too many decimal places";
                return false;
            }

            if ( value.Count( '-' ) > 1 ) {
                whyParseFailed = "'value' contained too many minus signs";
                return false;
            }

            if ( value.Any( c => !Char.IsDigit( c ) && c != '.' && c != '-' ) ) {
                whyParseFailed = "all chars must be a digit, a period, or a negative sign";
                return false;
            }

            var split = value.Split( '.' );
            split.Should().HaveCount( expected: 2, because: "otherwise invalid" );
            if ( split.Length != 2 ) {
                whyParseFailed = "";
                return false;
            }

            try {
                using ( new MemoryFailPoint( split[ 0 ].Length / 1048576 ) ) { }
            }
            catch ( ArgumentOutOfRangeException ) {
                return false;
            }
            catch ( InsufficientMemoryException ) {
                return false;
            }

            try {
                using ( new MemoryFailPoint( split[ 1 ].Length / 1048576 ) ) { }
            }
            catch ( ArgumentOutOfRangeException ) {
                return false;
            }
            catch ( InsufficientMemoryException ) {
                return false;
            }

            BigInteger whole;

            if ( !BigInteger.TryParse( split[ 0 ], out whole ) ) {
                //we were unable to parse the first string (all to the left of the decimal point)
                return false;
            }

            BigInteger fraction;

            if ( !BigInteger.TryParse( split[ 1 ], out fraction ) ) {
                //we were unable to parse the second string (all to the right of the decimal point)
                return false;
            }

            var fractionLength = fraction.ToString().Length;

            var ratioInteger = BigInteger.Pow( 10, fractionLength ); //we want the ratio of top/bottom to scale up past the decimal point
            whole *= ratioInteger; //append a whole lot of zeroes

            //BigDecimal recombined = wholeInteger + fractionInteger;

            //var ratioDecimal = ( BigDecimal ) ratioInteger;

            //recombined /= ratioDecimal;

            whole += fraction; //reconstruct the part that was after the decimal point

            number = whole;

            number = number / ratioInteger;

            //TODO does BigRational already reduce?
            //var leastCommonDenominator = BigRational.LeastCommonDenominator( bob.Numerator, bob.Denominator );

            return true; //whew.
        }

        [Pure]
        public Byte[] ToByteArray() {
            var unscaledValue = this.Significand.ToByteArray();
            var scale = BitConverter.GetBytes( this.Exponent );

            if ( !( unscaledValue.Length + scale.Length ).CanAllocateMemory() ) {
                throw new OutOfMemoryException( "ToByteArray() is too large to allocate" );
            }

            var bytes = new Byte[ unscaledValue.Length + scale.Length ];
            Array.Copy( unscaledValue, 0, bytes, 0, unscaledValue.Length );
            Array.Copy( scale, 0, bytes, unscaledValue.Length, scale.Length );

            return bytes;
        }

        [Pure]
        public static Byte[] DecimalToByteArray( Decimal d ) {
            var bytes = new Byte[ 16 ];

            var bits = Decimal.GetBits( d );
            var lo = bits[ 0 ];
            var mid = bits[ 1 ];
            var hi = bits[ 2 ];
            var flags = bits[ 3 ];

            bytes[ 0 ] = ( Byte )lo;
            bytes[ 1 ] = ( Byte )( lo >> 8 );
            bytes[ 2 ] = ( Byte )( lo >> 16 );
            bytes[ 3 ] = ( Byte )( lo >> 24 );

            bytes[ 4 ] = ( Byte )mid;
            bytes[ 5 ] = ( Byte )( mid >> 8 );
            bytes[ 6 ] = ( Byte )( mid >> 16 );
            bytes[ 7 ] = ( Byte )( mid >> 24 );

            bytes[ 8 ] = ( Byte )hi;
            bytes[ 9 ] = ( Byte )( hi >> 8 );
            bytes[ 10 ] = ( Byte )( hi >> 16 );
            bytes[ 11 ] = ( Byte )( hi >> 24 );

            bytes[ 12 ] = ( Byte )flags;
            bytes[ 13 ] = ( Byte )( flags >> 8 );
            bytes[ 14 ] = ( Byte )( flags >> 16 );
            bytes[ 15 ] = ( Byte )( flags >> 24 );

            return bytes;
        }

        public T ToType<T>() where T : struct {
            return ( T )( ( IConvertible )this ).ToType( typeof( T ), null );
        }

        #region Explicity and Implicit Casts

        public static explicit operator Byte( BigDecimal value ) {
            return value.ToType<Byte>();
        }

        public static explicit operator sbyte( BigDecimal value ) {
            return value.ToType<sbyte>();
        }

        public static explicit operator short( BigDecimal value ) {
            return value.ToType<short>();
        }

        public static explicit operator Int32( BigDecimal value ) {
            return value.ToType<Int32>();
        }

        public static explicit operator Int64( BigDecimal value ) {
            return value.ToType<Int64>();
        }

        public static explicit operator ushort( BigDecimal value ) {
            return value.ToType<ushort>();
        }

        public static explicit operator UInt32( BigDecimal value ) {
            return value.ToType<UInt32>();
        }

        public static explicit operator UInt64( BigDecimal value ) {
            return value.ToType<UInt64>();
        }

        public static implicit operator BigDecimal( Byte value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( sbyte value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( short value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( Int32 value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( ushort value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( UInt32 value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( UInt64 value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( float value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( BigInteger value ) {
            return new BigDecimal( value, 0 );
        }

        #endregion Explicity and Implicit Casts
    }
}