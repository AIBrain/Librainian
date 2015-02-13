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
// "Librainian/BiggerRational.cs" was last cleaned by Rick on 2014/08/22 at 8:23 PM

#endregion License & Information

namespace Librainian.Maths {
    using System;
    using System.Globalization;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    ///     <para>Decompiled from BigRationalLibrary. Where BigRationalLibrary came from, I don't know. But thanks!</para>
    ///     <para>Type: Numerics.BiggerRational</para>
    ///     <para>Assembly: BigRationalLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</para>
    ///     <para>MVID: 9E1BC183-333C-405D-BAC4-4796801DDDA5</para>
    /// </summary>
    [ComVisible( false )]
    [Serializable]
    public struct BiggerRational : IComparable, IComparable<BiggerRational>, IDeserializationCallback, IEquatable<BiggerRational>, ISerializable {
        public static readonly BiggerRational MinusOne = new BiggerRational( BigInteger.MinusOne );
        public static readonly BiggerRational One = new BiggerRational( BigInteger.One );
        public static readonly BiggerRational Zero = new BiggerRational( BigInteger.Zero );
        public BigInteger Denominator;
        public BigInteger Numerator;
        private const String CSolidus = "/";
        private const int DecimalMaxScale = 28;
        private const int DecimalScaleMask = 16711680;
        private const int DecimalSignMask = -2147483648;
        private const int DoubleMaxScale = 308;
        private static readonly BigInteger SBnDecimalMaxValue = ( BigInteger )new Decimal( -1, -1, -1, false, 0 );
        private static readonly BigInteger SBnDecimalMinValue = ( BigInteger )new Decimal( -1, -1, -1, true, 0 );
        private static readonly BigInteger SBnDecimalPrecision = BigInteger.Pow( 10, DecimalMaxScale );
        private static readonly BigInteger SBnDoubleMaxValue = new BigInteger( Double.MaxValue );
        private static readonly BigInteger SBnDoubleMinValue = new BigInteger( Double.MinValue );
        private static readonly BigInteger SBnDoublePrecision = BigInteger.Pow( 10, DoubleMaxScale );

        static BiggerRational() {
        }

        public BiggerRational( BigInteger numerator ) {
            this.Numerator = numerator;
            this.Denominator = BigInteger.One;
        }

        public BiggerRational( Double value ) {
            if ( Double.IsNaN( value ) ) {
                throw new ArgumentException( "Argument is not a number", nameof( value ) );
            }
            if ( Double.IsInfinity( value ) ) {
                throw new ArgumentException( "Argument is infinity", nameof( value ) );
            }
            int sign;
            int exp;
            ulong man;
            bool isFinite;
            SplitDoubleIntoParts( value, out sign, out exp, out man, out isFinite );
            if ( ( long )man == 0L ) {
                this = Zero;
            }
            else {
                this.Numerator = man;
                this.Denominator = 1048576;
                if ( exp > 0 ) {
                    this.Numerator = BigInteger.Pow( this.Numerator, exp );
                }
                else if ( exp < 0 ) {
                    this.Denominator = BigInteger.Pow( this.Denominator, -exp );
                }
                if ( sign < 0 ) {
                    this.Numerator = BigInteger.Negate( this.Numerator );
                }
                this.Simplify();
            }
        }

        public BiggerRational( Decimal value ) {
            var bits = Decimal.GetBits( value );
            if ( bits == null || bits.Length != 4 || ( ( bits[ 3 ] & 2130771967 ) != 0 || ( bits[ 3 ] & DecimalScaleMask ) > 1835008 ) ) {
                throw new ArgumentException( "invalid System.Decimal", nameof( value ) );
            }
            if ( value == new Decimal( 0 ) ) {
                this = Zero;
            }
            else {
                this.Numerator = new BigInteger( ( ulong )( uint )bits[ 2 ] << 32 | ( uint )bits[ 1 ] ) << 32 | ( uint )bits[ 0 ];
                if ( ( bits[ 3 ] & int.MinValue ) != 0 ) {
                    this.Numerator = BigInteger.Negate( this.Numerator );
                }
                this.Denominator = BigInteger.Pow( 10, ( bits[ 3 ] & DecimalScaleMask ) >> 16 );
                this.Simplify();
            }
        }

        public BiggerRational( BigInteger numerator, BigInteger denominator ) {
            if ( denominator.Sign == 0 ) {
                throw new DivideByZeroException();
            }
            if ( numerator.Sign == 0 ) {
                this.Numerator = BigInteger.Zero;
                this.Denominator = BigInteger.One;
            }
            else if ( denominator.Sign < 0 ) {
                this.Numerator = BigInteger.Negate( numerator );
                this.Denominator = BigInteger.Negate( denominator );
            }
            else {
                this.Numerator = numerator;
                this.Denominator = denominator;
            }
            this.Simplify();
        }

        public BiggerRational( BigInteger whole, BigInteger numerator, BigInteger denominator ) {
            if ( denominator.Sign == 0 ) {
                throw new DivideByZeroException();
            }
            if ( numerator.Sign == 0 && whole.Sign == 0 ) {
                this.Numerator = BigInteger.Zero;
                this.Denominator = BigInteger.One;
            }
            else if ( denominator.Sign < 0 ) {
                this.Denominator = BigInteger.Negate( denominator );
                this.Numerator = BigInteger.Negate( whole ) * this.Denominator + BigInteger.Negate( numerator );
            }
            else {
                this.Denominator = denominator;
                this.Numerator = whole * denominator + numerator;
            }
            this.Simplify();
        }

        private BiggerRational( SerializationInfo info, StreamingContext context ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }
            this.Numerator = ( BigInteger )info.GetValue( "Numerator", typeof( BigInteger ) );
            this.Denominator = ( BigInteger )info.GetValue( "Denominator", typeof( BigInteger ) );
        }

        public int Sign => this.Numerator.Sign;

        public static BiggerRational Abs( BiggerRational r ) => r.Numerator.Sign >= 0 ? r : new BiggerRational( BigInteger.Abs( r.Numerator ), r.Denominator );

        public static BiggerRational Add( BiggerRational r1, BiggerRational r2 ) => new BiggerRational( r1.Numerator * r2.Denominator + r1.Denominator * r2.Numerator, r1.Denominator * r2.Denominator );

        public static int Compare( BiggerRational r1, BiggerRational r2 ) => BigInteger.Compare( r1.Numerator * r2.Denominator, r2.Numerator * r1.Denominator );

        public static BiggerRational Divide( BiggerRational dividend, BiggerRational divisor ) => new BiggerRational( dividend.Numerator * divisor.Denominator, dividend.Denominator * divisor.Numerator );

        public static BiggerRational DivRem( BiggerRational dividend, BiggerRational divisor, out BiggerRational remainder ) {
            var numerator = dividend.Numerator * divisor.Denominator;
            var denominator1 = dividend.Denominator * divisor.Numerator;
            var denominator2 = dividend.Denominator * divisor.Denominator;
            remainder = new BiggerRational( numerator % denominator1, denominator2 );
            return new BiggerRational( numerator, denominator1 );
        }

        public static Boolean Equals( BiggerRational me, BiggerRational other ) {
            if ( me.Denominator == other.Denominator ) {
                return me.Numerator == other.Numerator;
            }
            return me.Numerator * other.Denominator == me.Denominator * other.Numerator;
        }


        public static explicit operator ulong( BiggerRational value ) => ( ulong )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator int( BiggerRational value ) => ( int )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator BigInteger( BiggerRational value ) => BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator uint( BiggerRational value ) => ( uint )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator float( BiggerRational value ) => ( float )( Double )value;

        public static explicit operator UInt16( BiggerRational value ) => ( UInt16 )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator Decimal( BiggerRational value ) {
            if ( SafeCastToDecimal( value.Numerator ) && SafeCastToDecimal( value.Denominator ) ) {
                return ( Decimal )value.Numerator / ( Decimal )value.Denominator;
            }
            var bigInteger = value.Numerator * SBnDecimalPrecision / value.Denominator;
            if ( bigInteger.IsZero ) {
                return new Decimal( 0 );
            }
            for ( var index = 28 ; index >= 0 ; --index ) {
                if ( !SafeCastToDecimal( bigInteger ) ) {
                    bigInteger /= 10;
                }
                else {
                    var decimalUint32 = new DecimalUInt32 {
                        dec = ( Decimal )bigInteger
                    };
                    decimalUint32.flags = decimalUint32.flags & -16711681 | index << 16;
                    return decimalUint32.dec;
                }
            }
            throw new OverflowException( "Value was either too large or too small for a System.Decimal." );
        }


        public static implicit operator BiggerRational( sbyte value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( UInt16 value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( uint value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( ulong value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( byte value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( short value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( int value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( long value ) => new BiggerRational( ( BigInteger )value );

        public static implicit operator BiggerRational( BigInteger value ) => new BiggerRational( value );

        public static implicit operator BiggerRational( float value ) => new BiggerRational( value );

        public static implicit operator BiggerRational( Double value ) => new BiggerRational( value );

        public static implicit operator BiggerRational( Decimal value ) => new BiggerRational( value );

        public static BiggerRational Invert( BiggerRational r ) => new BiggerRational( r.Denominator, r.Numerator );

        [UsedImplicitly]
        public static BigInteger LeastCommonDenominator( BiggerRational r1, BiggerRational r2 ) => r1.Denominator * r2.Denominator / BigInteger.GreatestCommonDivisor( r1.Denominator, r2.Denominator );

        public static BiggerRational Multiply( BiggerRational r1, BiggerRational r2 ) => new BiggerRational( r1.Numerator * r2.Numerator, r1.Denominator * r2.Denominator );

        public static BiggerRational Negate( BiggerRational r ) => new BiggerRational( BigInteger.Negate( r.Numerator ), r.Denominator );

        public static BiggerRational operator -( BiggerRational r ) => new BiggerRational( -r.Numerator, r.Denominator );

        public static BiggerRational operator -( BiggerRational r1, BiggerRational r2 ) => Subtract( r1, r2 );

        public static BiggerRational operator --( BiggerRational r ) => r - One;

        public static bool operator !=( BiggerRational x, BiggerRational y ) => Compare( x, y ) != 0;

        public static BiggerRational operator %( BiggerRational r1, BiggerRational r2 ) => new BiggerRational( r1.Numerator * r2.Denominator % r1.Denominator * r2.Numerator, r1.Denominator * r2.Denominator );

        public static BiggerRational operator *( BiggerRational r1, BiggerRational r2 ) => Multiply( r1, r2 );

        public static BiggerRational operator /( BiggerRational dividend, BiggerRational divisor ) => Divide( dividend, divisor );

        public static BiggerRational operator +( BiggerRational r ) => r;

        public static BiggerRational operator +( BiggerRational r1, BiggerRational r2 ) => Add( r1, r2 );

        public static BiggerRational operator ++( BiggerRational r ) => r + One;

        public static bool operator <( BiggerRational x, BiggerRational y ) => Compare( x, y ) < 0;

        public static bool operator <=( BiggerRational x, BiggerRational y ) => Compare( x, y ) <= 0;

        public static bool operator ==( BiggerRational x, BiggerRational y ) => Compare( x, y ) == 0;

        public static bool operator >( BiggerRational x, BiggerRational y ) => Compare( x, y ) > 0;

        public static bool operator >=( BiggerRational x, BiggerRational y ) => Compare( x, y ) >= 0;

        public static BiggerRational Pow( BiggerRational baseValue, BigInteger exponent ) {
            if ( exponent.Sign == 0 ) {
                return One;
            }
            if ( exponent.Sign < 0 ) {
                if ( baseValue == Zero ) {
                    throw new ArgumentException( "cannot raise zero to a negative power", nameof( baseValue ) );
                }
                baseValue = Invert( baseValue );
                exponent = BigInteger.Negate( exponent );
            }
            var bigRational = baseValue;
            while ( exponent > BigInteger.One ) {
                bigRational *= baseValue;
                --exponent;
            }
            return bigRational;
        }

        public static BiggerRational Remainder( BiggerRational dividend, BiggerRational divisor ) => dividend % divisor;

        public static bool SafeCastToDecimal( BigInteger value ) {
            if ( SBnDecimalMinValue <= value ) {
                return value <= SBnDecimalMaxValue;
            }
            return false;
        }

        public static bool SafeCastToDouble( BigInteger value ) {
            if ( SBnDoubleMinValue <= value ) {
                return value <= SBnDoubleMaxValue;
            }
            return false;
        }

        public static void SplitDoubleIntoParts( Double dbl, out int sign, out int exp, out ulong man, out bool isFinite ) {
            DoubleVsUlong doubleVsUlong;
            doubleVsUlong.uu = 0UL;
            doubleVsUlong.dbl = dbl;
            sign = 1 - ( ( int )( doubleVsUlong.uu >> 62 ) & 2 );
            man = doubleVsUlong.uu & 4503599627370495UL;
            exp = ( int )( doubleVsUlong.uu >> 52 ) & 2047;
            switch ( exp ) {
                case 0:
                    isFinite = true;
                    if ( ( long )man == 0L ) {
                        return;
                    }
                    exp = -1074;
                    break;

                case 2047:
                    isFinite = false;
                    exp = int.MaxValue;
                    break;

                default:
                    isFinite = true;
                    man |= 4503599627370496UL;
                    exp -= 1075;
                    break;
            }
        }

        public static BiggerRational Subtract( BiggerRational r1, BiggerRational r2 ) => new BiggerRational( r1.Numerator * r2.Denominator - r1.Denominator * r2.Numerator, r1.Denominator * r2.Denominator );

        public int CompareTo( BiggerRational other ) => Compare( this, other );

        public bool Equals( BiggerRational other ) => Equals( this, other );

        public override bool Equals( object obj ) => obj is BiggerRational && Equals( this, ( BiggerRational )obj );

        public BiggerRational GetFractionPart() => new BiggerRational( BigInteger.Remainder( this.Numerator, this.Denominator ), this.Denominator );

        public override int GetHashCode() {

            // ReSharper disable once NonReadonlyFieldInGetHashCode
            var numerator = this.Numerator;

            // ReSharper disable once NonReadonlyFieldInGetHashCode
            var denominator = this.Denominator;
            return numerator.GetHashMerge( denominator );
        }

        public BigInteger GetWholePart() => BigInteger.Divide( this.Numerator, this.Denominator );

        int IComparable.CompareTo( object obj ) {
            if ( obj == null ) {
                return 1;
            }
            if ( !( obj is BiggerRational ) ) {
                throw new ArgumentException( "Argument must be of type BiggerRational", nameof( obj ) );
            }
            return Compare( this, ( BiggerRational )obj );
        }

        [SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }
            info.AddValue( "Numerator", this.Numerator );
            info.AddValue( "Denominator", this.Denominator );
        }

        /// <summary>
        ///     Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">
        ///     The object that initiated the callback. The functionality for this parameter is not currently
        ///     implemented.
        /// </param>
        public void OnDeserialization( object sender ) {
            try {
                if ( this.Denominator.Sign == 0 || this.Numerator.Sign == 0 ) {
                    this.Numerator = BigInteger.Zero;
                    this.Denominator = BigInteger.One;
                }
                else if ( this.Denominator.Sign < 0 ) {
                    this.Numerator = BigInteger.Negate( this.Numerator );
                    this.Denominator = BigInteger.Negate( this.Denominator );
                }
                this.Simplify();
            }
            catch ( ArgumentException ex ) {
                throw new SerializationException( "invalid serialization data", ex );
            }
        }


        public static explicit operator sbyte( BiggerRational value ) => ( sbyte )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator byte( BiggerRational value ) => ( byte )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator short( BiggerRational value ) => ( short )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator long( BiggerRational value ) => ( long )BigInteger.Divide( value.Numerator, value.Denominator );

        public static explicit operator Double( BiggerRational value ) {
            if ( SafeCastToDouble( value.Numerator ) && SafeCastToDouble( value.Denominator ) ) {
                return ( Double )value.Numerator / ( Double )value.Denominator;
            }
            var bigInteger = value.Numerator * SBnDoublePrecision / value.Denominator;
            if ( bigInteger.IsZero ) {
                return value.Sign >= 0 ? 0.0 : BitConverter.Int64BitsToDouble( long.MinValue );
            }
            var num = 0.0;
            var flag = false;
            for ( var index = 308 ; index > 0 ; --index ) {
                if ( !flag ) {
                    if ( SafeCastToDouble( bigInteger ) ) {
                        num = ( Double )bigInteger;
                        flag = true;
                    }
                    else {
                        bigInteger /= 10;
                    }
                }
                num /= 10.0;
            }
            if ( flag ) {
                return num;
            }
            return value.Sign >= 0 ? Double.PositiveInfinity : Double.NegativeInfinity;
        }

        public override String ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append( this.Numerator.ToString( "R", CultureInfo.InvariantCulture ) );
            stringBuilder.Append( "/" );
            stringBuilder.Append( this.Denominator.ToString( "R", CultureInfo.InvariantCulture ) );
            return stringBuilder.ToString();
        }

        private static int CbitHighZero( ulong uu ) {
            if ( ( ( long )uu & -4294967296L ) == 0L ) {
                return 32 + CbitHighZero( ( uint )uu );
            }
            return CbitHighZero( ( uint )( uu >> 32 ) );
        }

        private static int CbitHighZero( uint u ) {
            if ( ( int )u == 0 ) {
                return 32;
            }
            var num = 0;
            if ( ( ( int )u & -65536 ) == 0 ) {
                num += 16;
                u <<= 16;
            }
            if ( ( ( int )u & -16777216 ) == 0 ) {
                num += 8;
                u <<= 8;
            }
            if ( ( ( int )u & -268435456 ) == 0 ) {
                num += 4;
                u <<= 4;
            }
            if ( ( ( int )u & -1073741824 ) == 0 ) {
                num += 2;
                u <<= 2;
            }
            if ( ( ( int )u & int.MinValue ) == 0 ) {
                ++num;
            }
            return num;
        }

        private static Double GetDoubleFromParts( int sign, int exp, ulong man ) {
            DoubleVsUlong doubleVsUlong;
            doubleVsUlong.dbl = 0.0;
            if ( ( long )man == 0L ) {
                doubleVsUlong.uu = 0UL;
            }
            else {
                var num = CbitHighZero( man ) - 11;
                if ( num < 0 ) {
                    man >>= -num;
                }
                else {
                    man <<= num;
                }
                exp += 1075;
                if ( exp >= 2047 ) {
                    doubleVsUlong.uu = 9218868437227405312UL;
                }
                else if ( exp <= 0 ) {
                    --exp;
                    doubleVsUlong.uu = exp >= -52 ? man >> -exp : 0UL;
                }
                else {
                    doubleVsUlong.uu = ( ulong )( ( long )man & 4503599627370495L | ( long )exp << 52 );
                }
            }
            if ( sign < 0 ) {
                doubleVsUlong.uu |= 9223372036854775808UL;
            }
            return doubleVsUlong.dbl;
        }

        private void Simplify() {
            if ( this.Numerator == BigInteger.Zero ) {
                this.Denominator = BigInteger.One;
            }
            var bigInteger = BigInteger.GreatestCommonDivisor( this.Numerator, this.Denominator );
            if ( !( bigInteger > BigInteger.One ) ) {
                return;
            }
            this.Numerator = this.Numerator / bigInteger;
            this.Denominator = this.Denominator / bigInteger;
        }

        [StructLayout( LayoutKind.Explicit )]
        internal struct DecimalUInt32 {

            [FieldOffset( 0 )]
            public Decimal dec;

            [FieldOffset( 0 )]
            public int flags;
        }

        [StructLayout( LayoutKind.Explicit )]
        internal struct DoubleVsUlong {

            [FieldOffset( 0 )]
            public Double dbl;

            [FieldOffset( 0 )]
            public ulong uu;
        }
    }
}