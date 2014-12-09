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
// "Librainian/BigDecimal.cs" was last cleaned by Rick on 2014/08/17 at 9:06 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Annotations;
    using Extensions;
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
    ///     <para>Rewritten into an immutable struct by Rick@aibrain.org in August 2014</para>
    ///     <para>Added the parsing ability from the 'clojure' project.</para>
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/a/13813535/956364" />
    /// <seealso cref="http://gist.github.com/nberardi/2667136" />
    [UsedImplicitly]
    [Immutable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct BigDecimal : IComparable, IComparable<BigDecimal>, IConvertible, IFormattable, IEquatable<BigDecimal> {

        /// <summary>
        ///     -1
        /// </summary>
        public static readonly BigDecimal MinusOne = new BigDecimal( Decimal.MinusOne );

        /// <summary>
        ///     1
        /// </summary>
        public static readonly BigDecimal One = new BigDecimal( Decimal.One );

        /// <summary>
        ///     0
        /// </summary>
        public static readonly BigDecimal Zero = new BigDecimal( Decimal.Zero );

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

        public BigDecimal( BigDecimal bigDecimal ) : this( bigDecimal.Mantissa, bigDecimal.Exponent ) {
        }

        public BigDecimal( Decimal value ) : this( bigDecimal: value ) {
        }

        public BigDecimal( Double value ) : this( bigDecimal: value ) {
        }

        public BigDecimal( float value ) : this( bigDecimal: value ) {
        }

        public BigDecimal( BigInteger significand, Int32 exponent ) {
            this.Significand = significand;
            this.Exponent = exponent;

            //BUG is this correct?

            while ( exponent > 0 && this.Significand % 10 == 0 ) {
                if ( this.Significand == 0 ) {
                    break;
                }
                this.Significand /= 10;
                this.Exponent += 1;
            }
        }

        public BigDecimal( Int32 value ) : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( Int64 value ) : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( UInt32 value ) : this( new BigInteger( value ), 0 ) {
        }

        public BigDecimal( UInt64 value ) : this( new BigInteger( value ), 0 ) {
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

        public BigDecimal( String value ) {
            var number = value.ToBigDecimal();
            this.Significand = number.Significand;
            this.Exponent = number.Exponent;
        }

        public Boolean IsEven {
            get {
                return this.Significand.IsEven;
            }
        }

        public Boolean IsOne {
            get {
                return this.Significand.IsOne;
            }
        }

        public Boolean IsPowerOfTwo {
            get {
                return this.Significand.IsPowerOfTwo;
            }
        }

        public Boolean IsZero {
            get {
                return this.Significand.IsZero;
            }
        }

        /// <summary>
        ///     The significand (aka mantissa) is part of a number consisting of its significant digits.
        /// </summary>
        /// <seealso cref="Significand" />
        public BigInteger Mantissa {
            get {
                return this.Significand;
            }
        }

        public Int32 Sign {
            get {
                return this.Significand.Sign;
            }
        }

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

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

        /// <summary>
        ///     Returns the mantissa of <paramref name="value" />, aligned to the exponent of reference.
        ///     Assumes the exponent of <paramref name="value" /> is larger than of value.
        /// </summary>
        public static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) {
            Assert.GreaterOrEqual( value.Exponent, reference.Exponent );
            return value.Mantissa * BigInteger.Pow( value: 10, exponent: value.Exponent - reference.Exponent );
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

        [Pure]
        public static BigDecimal Divide( BigDecimal left, BigDecimal right ) {
            var ratio = left.Mantissa.NumberOfDigits() + right.Mantissa.NumberOfDigits();
            var power = BigInteger.Pow( 10, ratio );
            var templeft = left.Mantissa * power;
            var tempright = right.Mantissa * power;

            var tempmantissa = templeft / tempright;

            //tempmantissa /= power;

            var realexponent = left.Exponent - right.Exponent;

            var result = new BigDecimal( tempmantissa, realexponent );
            return result;
        }

        /// <summary>
        ///     Static equality check for <paramref name="left" /> against <paramref name="right" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( BigDecimal left, BigDecimal right ) => left.Mantissa.Equals( right.Mantissa ) && left.Exponent.Equals( right.Exponent );

        public static BigDecimal Exp( Double exponent ) {
            var tmp = One;
            while ( Math.Abs( exponent ) > 100 ) {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Exp( diff );
                exponent -= diff;
            }
            return tmp * Math.Exp( d: exponent );
        }

        public static explicit operator Double( BigDecimal value ) => ( Double )value.Mantissa * Math.Pow( 10, value.Exponent );

        /// <summary>
        ///     TODO this needs unit tested.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator BigInteger( BigDecimal value ) => value.ToBigInteger();

        public static implicit operator BigDecimal( Int64 number ) => new BigDecimal( number, 0 );

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

        [Pure]
        public static BigDecimal Multiply( BigDecimal left, BigDecimal right ) => new BigDecimal( left.Mantissa * right.Mantissa, left.Exponent + right.Exponent );

        public static BigDecimal operator -( BigDecimal number ) => new BigDecimal( number.Mantissa * -1, number.Exponent );

        public static BigDecimal operator -( BigDecimal left, BigDecimal right ) => Add( left: left, right: -right );

        public static BigDecimal operator --( BigDecimal number ) => number - 1;

        public static Boolean operator !=( BigDecimal left, BigDecimal right ) => !Equals( left, right );

        public static BigDecimal operator *( BigDecimal left, BigDecimal right ) => Multiply( left, right );

        public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) {

            //var exponentChange = 100 - ( dividend.Mantissa.NumberOfDigits() - divisor.Mantissa.NumberOfDigits() );
            //if ( exponentChange < 0 ) {
            //    exponentChange = 0;
            //}

            ////TODO this needs unit tested.
            //var newdividend = new BigDecimal( dividend.Mantissa * BigInteger.Pow( 10, exponentChange ), dividend.Exponent ); //BUG is this correct?

            //return new BigDecimal( newdividend.Mantissa / divisor.Mantissa, newdividend.Exponent - divisor.Exponent - exponentChange );
            //if ( dividend.Exponent < divisor.Exponent ) {
            //    dividend = new BigDecimal( dividend.Mantissa, );
            //}

            //var newmantissa = AlignExponent( divisor, dividend );
            //var newexponent = dividend.Exponent - divisor.Exponent;
            //var newmantissa = dividend.Mantissa * BigInteger.Pow(10, newexponent);
            //newmantissa /=  divisor.Mantissa;
            //var result = new BigDecimal( newmantissa, newexponent );
            var result = Divide( dividend, divisor );
            return result;
        }

        public static BigDecimal operator +( BigDecimal number ) => number;

        public static BigDecimal operator +( BigDecimal left, BigDecimal right ) => Add( left, right );

        public static BigDecimal operator ++( BigDecimal number ) => Add( number, 1 );

        public static Boolean operator <( BigDecimal left, BigDecimal right ) => left.Exponent > right.Exponent ? AlignExponent( left, right ) < right.Mantissa : left.Mantissa < AlignExponent( right, left );

        public static Boolean operator <=( BigDecimal left, BigDecimal right ) => left.Exponent > right.Exponent ? AlignExponent( left, right ) <= right.Mantissa : left.Mantissa <= AlignExponent( right, left );

        public static Boolean operator ==( BigDecimal left, BigDecimal right ) => Equals( left, right );

        public static Boolean operator >( BigDecimal left, BigDecimal right ) => left.Exponent > right.Exponent ? AlignExponent( left, right ) > right.Mantissa : left.Mantissa > AlignExponent( right, left );

        public static Boolean operator >=( BigDecimal left, BigDecimal right ) => left.Exponent > right.Exponent ? AlignExponent( left, right ) >= right.Mantissa : left.Mantissa >= AlignExponent( right, left );

        //public static explicit operator uint( BigDecimal value ) {
        //    return ( uint )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
        //}
        /// <summary>
        ///     <para>Create a BigDecimal from a String representation.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigDecimal Parse( [NotNull] String value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }
            return value.ToBigDecimal();
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

        //public static explicit operator Int32( BigDecimal value ) {
        //    return ( Int32 )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
        //}
        /// <summary>
        ///     <para>Create a BigDecimal from a String representation.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public static Boolean TryParse( [CanBeNull] String value, out BigDecimal? answer ) {
            answer = null;
            if ( String.IsNullOrWhiteSpace( value ) ) {
                return false;
            }
            try {
                answer = value.ToBigDecimal();
                return true;
            }
            catch ( FormatException ) {
            }
            catch ( ArithmeticException ) {
            }
            return false;
        }

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
            return this > other ? 1 : 0;
        }

        public Boolean Equals( BigDecimal other ) => Equals( this, other );

        [Pure]
        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is BigDecimal && Equals( this, ( BigDecimal )obj );
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
        [Pure]
        public override Int32 GetHashCode() => this.Mantissa.GetHashMerge( this.Exponent );

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

        Boolean IConvertible.ToBoolean( IFormatProvider provider ) => Convert.ToBoolean( this );

        Byte IConvertible.ToByte( IFormatProvider provider ) => Convert.ToByte( this );

        char IConvertible.ToChar( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal to Char" );
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {

            //TODO use a span -> plancks -> UDT (universaldatetime)
            throw new InvalidCastException( "Cannot cast BigDecimal to DateTime" );
        }

        Decimal IConvertible.ToDecimal( IFormatProvider provider ) => Convert.ToDecimal( this );

        Double IConvertible.ToDouble( IFormatProvider provider ) => Convert.ToDouble( this );

        short IConvertible.ToInt16( IFormatProvider provider ) => Convert.ToInt16( this );

        Int32 IConvertible.ToInt32( IFormatProvider provider ) => Convert.ToInt32( this );

        Int64 IConvertible.ToInt64( IFormatProvider provider ) => Convert.ToInt64( this );

        sbyte IConvertible.ToSByte( IFormatProvider provider ) => Convert.ToSByte( this );

        float IConvertible.ToSingle( IFormatProvider provider ) => Convert.ToSingle( this );

        String IConvertible.ToString( IFormatProvider provider ) => Convert.ToString( this );

        object IConvertible.ToType( Type conversionType, IFormatProvider provider ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), this.Exponent );
            var remainder = BigInteger.Remainder( this.Significand, scaleDivisor );
            var scaledValue = BigInteger.Divide( this.Significand, scaleDivisor );

            if ( scaledValue > new BigInteger( Decimal.MaxValue ) ) {
                throw new ArgumentOutOfRangeException( "provider", String.Format( "The value {0} cannot fit into {1}.", this.Significand, conversionType.Name ) );
            }

            var leftOfDecimal = ( Decimal )scaledValue;
            var rightOfDecimal = ( ( Decimal )remainder ) / ( ( Decimal )scaleDivisor );

            var value = leftOfDecimal + rightOfDecimal;
            return Convert.ChangeType( value, conversionType );
        }

        UInt16 IConvertible.ToUInt16( IFormatProvider provider ) => Convert.ToUInt16( this );

        UInt32 IConvertible.ToUInt32( IFormatProvider provider ) => Convert.ToUInt32( this );

        UInt64 IConvertible.ToUInt64( IFormatProvider provider ) => Convert.ToUInt64( this );

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

        //    return ( Decimal )value.Mantissa * ( Decimal )Math.Pow( 10, value.Exponent );
        //}
        public String ToScientificString() => MathExtensions.ToScientificString( this );

        public String ToString( String format, IFormatProvider formatProvider ) {
            throw new NotImplementedException();
        }

        [Pure]
        public override String ToString() {
            var result = BigInteger.Abs( this.Significand ).ToString(); //get the digits.

            if ( this.Exponent < 0 ) {
                var amountOfZeros = Math.Abs( this.Exponent ) - result.Length;
                if ( amountOfZeros > 0 ) {
                    var leadingZeros = new String( '0', amountOfZeros );
                    result = result.Prepend( leadingZeros );
                    result = result.Prepend( "0." );
                }
                else {
                    var at = result.Length + this.Exponent;
                    result = result.Insert( at, at == 0 ? "0." : "." );
                }
            }
            else if ( this.Exponent == 0 ) {
                if ( this.Significand.IsZero ) {

                    // do nothing?
                }
            }
            else if ( this.Exponent > 0 ) {
                var trailingZeros = new String( '0', this.Exponent );
                result = result.Append( trailingZeros ); //big number, add Exponent zeros on the right
            }

            if ( this.Sign == -1 ) {
                result = result.Prepend( "-" );
            }

            return result;

            //if ( this.Exponent < 0 ) {
            //    result = result.Insert( 0, leadingZeros );

            //}
            //else if ( this.Exponent > 0 ) {
            //    var at = result.Length + this.Exponent;

            //    var padLeft = at == 0;

            //    result = result.Insert( at, "." );

            //    if ( padLeft ) {
            //        result = result.Insert( at, "0" );
            //    }
            //}
        }

        public String ToStringWithE() => String.Concat( this.Mantissa.ToString(), "E", this.Exponent );

        //    return true; //whew.
        //}
        public T ToType<T>() where T : struct => ( T )( ( IConvertible )this ).ToType( typeof( T ), null );

        private static BigDecimal Add( BigDecimal left, BigDecimal right ) {
            if ( left.Exponent > right.Exponent ) {
                return new BigDecimal( AlignExponent( left, right ) + right.Mantissa, exponent: right.Exponent );
            }
            return new BigDecimal( AlignExponent( right, left ) + left.Mantissa, exponent: left.Exponent );
        }

        public static explicit operator float( BigDecimal value ) => Convert.ToSingle( ( Double )value );

        public static explicit operator Decimal( BigDecimal value ) => ( Decimal )value.Mantissa * ( Decimal )Math.Pow( 10, value.Exponent );

        //public static explicit operator BigInteger( BigDecimal value ) {
        //    var man = (BigDecimal)value.Mantissa;
        //    new BigDecimal(
        //    man *= BigDecimal.Pow( 10, value.Exponent );
        /*
                private static bool CheckExponent( long candidate, bool isZero, out int exponent ) {
                    exponent = ( int )candidate;
                    if ( exponent == candidate ) {
                        return true;
                    }
                    if ( !isZero ) {
                        return false;
                    }
                    exponent = candidate > ( long )int.MaxValue ? int.MaxValue : int.MinValue;
                    return true;
                }
        */

        /*
                private static int CheckExponent( long candidate, bool isZero ) {
                    int exponent;
                    if ( CheckExponent( candidate, isZero, out exponent ) ) {
                        return exponent;
                    }
                    if ( candidate > int.MaxValue ) {
                        throw new ArithmeticException( "Overflow in scale" );
                    }
                    throw new ArithmeticException( "Underflow in scale" );
                }
        */

        /*

                /// <summary>
                ///     Parse a substring of a character array as a BigDecimal.
                /// </summary>
                /// <param name="buf">The character array to parse</param>
                /// <param name="offset">Start index for parsing</param>
                /// <param name="len">Number of chars to parse.</param>
                /// <param name="throwOnError">If true, an error causes an exception to be thrown. If false, false is returned.</param>
                /// <param name="v">The BigDecimal corresponding to the characters.</param>
                /// <returns>True if successful, false if not (or throws if throwOnError is true).</returns>
                /// <remarks>
                ///     Ugly. We could use a RegEx, but trying to avoid unnecessary allocation, I guess.
                ///     [+-]?\d*(\.\d*)?([Ee][+-]?\d+)?  with additional constraint that one of the two d* must have at least one char.
                /// </remarks>
                private static Boolean DoParse( char[] buf, int offset, int len, bool throwOnError, out BigDecimal v ) {
                    v = default( BigDecimal );
                    if ( len == 0 ) {
                        if ( throwOnError ) {
                            throw new FormatException( "Empty String" );
                        }
                        return false;
                    }
                    if ( offset + len > buf.Length ) {
                        if ( throwOnError ) {
                            throw new FormatException( "offset+len past the end of the char array" );
                        }
                        return false;
                    }
                    var sourceIndex1 = offset;
                    var flag = false;
                    switch ( buf[ offset ] ) {
                        case '-':
                        case '+':
                            flag = true;
                            ++offset;
                            --len;
                            break;
                    }
                    for ( ; len > 0 && char.IsDigit( buf[ offset ] ); --len ) {
                        ++offset;
                    }
                    var num1 = offset - sourceIndex1;
                    var num2 = offset - sourceIndex1 - ( flag ? 1 : 0 );
                    var sourceIndex2 = offset;
                    var length1 = 0;
                    if ( len > 0 && buf[ offset ] == 46 ) {
                        ++offset;
                        --len;
                        sourceIndex2 = offset;
                        for ( ; len > 0 && char.IsDigit( buf[ offset ] ); --len ) {
                            ++offset;
                        }
                        length1 = offset - sourceIndex2;
                    }
                    var sourceIndex3 = -1;
                    var length2 = 0;
                    if ( len > 0 && ( buf[ offset ] == 101 || buf[ offset ] == 69 ) ) {
                        ++offset;
                        --len;
                        sourceIndex3 = offset;
                        if ( len == 0 ) {
                            if ( throwOnError ) {
                                throw new FormatException( "Missing exponent" );
                            }
                            return false;
                        }
                        switch ( buf[ offset ] ) {
                            case '-':
                            case '+':
                                ++offset;
                                --len;
                                break;
                        }
                        if ( len == 0 ) {
                            if ( throwOnError ) {
                                throw new FormatException( "Missing exponent" );
                            }
                            return false;
                        }
                        for ( ; len > 0 && char.IsDigit( buf[ offset ] ); --len ) {
                            ++offset;
                        }
                        length2 = offset - sourceIndex3;
                        if ( length2 == 0 ) {
                            if ( throwOnError ) {
                                throw new FormatException( "Missing exponent" );
                            }
                            return false;
                        }
                    }
                    if ( len != 0 ) {
                        if ( throwOnError ) {
                            throw new FormatException( "Unused characters at end" );
                        }
                        return false;
                    }
                    var num3 = num2 + length1;
                    if ( num3 == 0 ) {
                        if ( throwOnError ) {
                            throw new FormatException( "No digits in coefficient" );
                        }
                        return false;
                    }
                    var chArray1 = new char[ num1 + length1 ];
                    Array.Copy( buf, sourceIndex1, chArray1, 0, num1 );
                    if ( length1 > 0 ) {
                        Array.Copy( buf, sourceIndex2, chArray1, num1, length1 );
                    }
                    var coeff = BigInteger.Parse( new String( chArray1 ) );
                    var result = 0;
                    if ( length2 > 0 ) {
                        var chArray2 = new char[ length2 ];
                        Array.Copy( buf, sourceIndex3, chArray2, 0, length2 );
                        if ( throwOnError ) {
                            result = int.Parse( new String( chArray2 ), CultureInfo.InvariantCulture );
                        }
                        else if ( !int.TryParse( new String( chArray2 ), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result ) ) {
                            return false;
                        }
                    }
                    var exp = num2 - num3;
                    if ( result != 0 ) {
                        try {
                            exp = CheckExponent( exp + result, coeff.IsZero );
                        }
                        catch ( ArithmeticException ex ) {
                            if ( !throwOnError ) {
                                return false;
                            }
                            throw;
                        }
                    }
                    for ( var index = flag ? 1 : 0; index < num1 + length1 && num3 > 1 && ( int )chArray1[ index ] == 48; ++index ) {
                        --num3;
                    }
                    v = new BigDecimal( coeff, exp );
                    return true;
                }
        */

        ///// <summary>
        /////     Attempt to parse a huge Decimal from a String.
        ///// </summary>
        ///// <param name="value"></param>
        ///// <param name="number"></param>
        ///// <param name="whyParseFailed"></param>
        ///// <returns></returns>
        //public static Boolean TryParse( [CanBeNull] String value, out BigDecimal? number, out String whyParseFailed ) {
        //    whyParseFailed = String.Empty;
        //    number = null;

        //    //BigDecimal bob = Parse( value );
        //    //number = bob;

        //    clojure.lang.BigDecimal bigDecimal;
        //    if ( clojure.lang.BigDecimal.TryParse( value, out bigDecimal ) ) {
        //        number = new BigDecimal( bigDecimal.Coefficient, bigDecimal.Exponent );
        //        return true;
        //    }

        //    // all whitespace or none?
        //    if ( String.IsNullOrWhiteSpace( value ) ) {
        //        whyParseFailed = "'value' is null or contained only whitespace";
        //        return false;
        //    }

        //    value = value.Trim();
        //    if ( String.IsNullOrWhiteSpace( value ) ) {
        //        whyParseFailed = "'value' is null or contained only whitespace";
        //        return false;
        //    }

        //    if ( value.Contains( "E" ) ) {
        //        whyParseFailed = "not implemented yet";
        //        //TODO add in subset for parsing numbers like "3.14E15" (scientific notation?)
        //        throw new NotImplementedException();
        //        return false;
        //    }

        //    if ( value.Contains( "^" ) ) {
        //        whyParseFailed = "not implemented yet";
        //        //TODO add in subset for parsing numbers like "3.14^15"? (exponential notation?)

        //        //TODO add in subset for parsing numbers like "3.14X10^15"? (exponential notation?)
        //        throw new NotImplementedException();
        //        return false;
        //    }

        //    if ( !value.Contains( "." ) ) {
        //        value += ".0"; //for parsing large decimals
        //    }

        //    if ( value.Count( '.' ) > 1 ) {
        //        whyParseFailed = "'value' contained too many Decimal places";
        //        return false;
        //    }

        //    if ( value.Count( '-' ) > 1 ) {
        //        whyParseFailed = "'value' contained too many minus signs";
        //        return false;
        //    }

        //    if ( value.Any( c => !Char.IsDigit( c ) && c != '.' && c != '-' ) ) {
        //        whyParseFailed = "all chars must be a digit, a period, or a negative sign";
        //        return false;
        //    }

        //    var split = value.Split( '.' );
        //    split.Should().HaveCount( expected: 2, because: "otherwise invalid" );
        //    if ( split.Length != 2 ) {
        //        whyParseFailed = "";
        //        return false;
        //    }
        //    var wholeSide = split[ 0 ];

        //    var wholeSideLength = wholeSide.Length;
        //    if ( !wholeSideLength.CanAllocateMemory() ) {
        //        return false;
        //    }
        //    BigInteger leftOfDecimalPoint;
        //    if ( !BigInteger.TryParse( wholeSide, out leftOfDecimalPoint ) ) {
        //        //we were unable to parse the first String (all to the left of the Decimal point)
        //        return false;
        //    }

        //    var fractionSide = split[ 1 ];
        //    var fractionSideLength = fractionSide.Length;
        //    if ( !fractionSideLength.CanAllocateMemory() ) {
        //        return false;
        //    }

        //    BigInteger fractionInteger;

        //    var needToPadFractionSide = fractionSide[ 0 ] == '0';

        //    if ( needToPadFractionSide ) {
        //        //fractionSide = '1' + fractionSide;  //fake out BigInteger by replacing the leading zero with a 1
        //    }
        //    //BUG if the String split[1] had a bunch of leading zeros, they are getting trimmed out here.
        //    //TODO do some sort of multiplier. Or add a 1.0 in front, with a multiplier. then take off that after the recombine?
        //    //but it messes with the ratio

        //    if ( !BigInteger.TryParse( fractionSide, out fractionInteger ) ) {
        //        //we were unable to parse the second String (all to the right of the Decimal point)
        //        return false;
        //    }

        //    var fractionLength = fractionInteger.ToString().Length;

        //    var multiplier = BigInteger.Pow( 10, fractionLength ); //we want the ratio of top/bottom to scale up past the Decimal point and back down later

        //    leftOfDecimalPoint *= multiplier; //append a whole lot of zeroes "1000000000"
        //    leftOfDecimalPoint += fractionInteger; //reconstruct the part that was after the Decimal point "123456789"
        //    // so now it looks like "1123456789"

        //    if ( needToPadFractionSide ) {
        //        var zeros = new String( '0', fractionSideLength - fractionLength );
        //        var bside = leftOfDecimalPoint.ToString();
        //        bside = bside.Insert( wholeSideLength - 1, zeros );
        //        leftOfDecimalPoint = BigInteger.Parse( bside, NumberStyles.AllowDecimalPoint );
        //    }

        //    number = leftOfDecimalPoint;

        //    number /= multiplier;

        #region Explicity and Implicit Casts

        public static explicit operator Byte( BigDecimal value ) => value.ToType<Byte>();

        public static explicit operator Int64( BigDecimal value ) => value.ToType<Int64>();

        public static explicit operator Int32( BigDecimal value ) => value.ToType<Int32>();

        public static explicit operator sbyte( BigDecimal value ) => value.ToType<sbyte>();

        public static explicit operator short( BigDecimal value ) => value.ToType<short>();

        public static explicit operator UInt64( BigDecimal value ) => value.ToType<UInt64>();

        public static explicit operator UInt32( BigDecimal value ) => value.ToType<UInt32>();

        public static explicit operator UInt16( BigDecimal value ) => value.ToType<UInt16>();

        public static implicit operator BigDecimal( Byte value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( sbyte value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( short value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( Int32 value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( UInt16 value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( UInt32 value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( UInt64 value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( float value ) => new BigDecimal( value );

        public static implicit operator BigDecimal( BigInteger value ) => new BigDecimal( value, 0 );
        #endregion Explicity and Implicit Casts
    }
}