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
// "Librainian/BigDecimal2.cs" was last cleaned by Rick on 2014/08/16 at 2:37 PM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Linq;
    using System.Numerics;
    using Annotations;
    using FluentAssertions;
    using Parsing;

    /// <summary>
    ///     Another <see cref="BigDecimal" /> class found on GitHub.
    /// </summary>
    /// <seealso cref="http://gist.github.com/nberardi/2667136" />
    public struct BigDecimal2 :  {















       

        [Pure]
        public override string ToString() {
            var number = this.Significand.ToString( "G" );

            if ( this.Exponent > 0 ) {
                return number.Insert( number.Length - this.Exponent, "." );
            }

            return number;
        }

        public Byte[] ToByteArray() {
            var unscaledValue = this.Significand.ToByteArray();
            var scale = BitConverter.GetBytes( this.Exponent );

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
            bytes[ 2 ] = ( Byte )( lo >> 0x10 );
            bytes[ 3 ] = ( Byte )( lo >> 0x18 );
            bytes[ 4 ] = ( Byte )mid;
            bytes[ 5 ] = ( Byte )( mid >> 8 );
            bytes[ 6 ] = ( Byte )( mid >> 0x10 );
            bytes[ 7 ] = ( Byte )( mid >> 0x18 );
            bytes[ 8 ] = ( Byte )hi;
            bytes[ 9 ] = ( Byte )( hi >> 8 );
            bytes[ 10 ] = ( Byte )( hi >> 0x10 );
            bytes[ 11 ] = ( Byte )( hi >> 0x18 );
            bytes[ 12 ] = ( Byte )flags;
            bytes[ 13 ] = ( Byte )( flags >> 8 );
            bytes[ 14 ] = ( Byte )( flags >> 0x10 );
            bytes[ 15 ] = ( Byte )( flags >> 0x18 );

            return bytes;
        }

        public T ToType<T>() where T : struct {
            return ( T )( ( IConvertible )this ).ToType( typeof( T ), null );
        }

        public override Boolean Equals( object obj ) {
            return ( ( obj is BigDecimal2 ) && this.Equals( ( BigDecimal2 )obj ) );
        }

        [Pure]
        public override Int32 GetHashCode() {
            return this.Mantissa.GetHashMerge( this.Exponent );
        }

        /// <summary>
        ///     Static comparison test.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean Equals( BigDecimal2 left, BigDecimal2 right ) {
            return left.Exponent == right.Exponent && left.Significand == right.Significand;
        }

        #region Operators

        public static Boolean operator ==( BigDecimal2 left, BigDecimal2 right ) {
            return left.Equals( right );
        }

        public static Boolean operator !=( BigDecimal2 left, BigDecimal2 right ) {
            return !left.Equals( right );
        }

        public static Boolean operator >( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static Boolean operator >=( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static Boolean operator <( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static Boolean operator <=( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static Boolean operator ==( BigDecimal2 left, Decimal right ) {
            return left.Equals( right );
        }

        public static Boolean operator !=( BigDecimal2 left, Decimal right ) {
            return !left.Equals( right );
        }

        public static Boolean operator >( BigDecimal2 left, Decimal right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static Boolean operator >=( BigDecimal2 left, Decimal right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static Boolean operator <( BigDecimal2 left, Decimal right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static Boolean operator <=( BigDecimal2 left, Decimal right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static Boolean operator ==( Decimal left, BigDecimal2 right ) {
            return left.Equals( right );
        }

        public static Boolean operator !=( Decimal left, BigDecimal2 right ) {
            return !left.Equals( right );
        }

        public static Boolean operator >( Decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static Boolean operator >=( Decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static Boolean operator <( Decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static Boolean operator <=( Decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        #endregion Operators

        #region Explicity and Implicit Casts

        public static explicit operator Byte( BigDecimal2 value ) {
            return value.ToType<Byte>();
        }

        public static explicit operator sbyte( BigDecimal2 value ) {
            return value.ToType<sbyte>();
        }

        public static explicit operator short( BigDecimal2 value ) {
            return value.ToType<short>();
        }

        public static explicit operator Int32( BigDecimal2 value ) {
            return value.ToType<Int32>();
        }

        public static explicit operator Int64( BigDecimal2 value ) {
            return value.ToType<Int64>();
        }

        public static explicit operator ushort( BigDecimal2 value ) {
            return value.ToType<ushort>();
        }

        public static explicit operator UInt32( BigDecimal2 value ) {
            return value.ToType<UInt32>();
        }

        public static explicit operator UInt64( BigDecimal2 value ) {
            return value.ToType<UInt64>();
        }

        public static explicit operator float( BigDecimal2 value ) {
            return value.ToType<float>();
        }

        public static explicit operator Double( BigDecimal2 value ) {
            return value.ToType<Double>();
        }

        public static explicit operator Decimal( BigDecimal2 value ) {
            return value.ToType<Decimal>();
        }

        public static explicit operator BigInteger( BigDecimal2 value ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), value.Exponent );
            var scaledValue = BigInteger.Divide( value.Significand, scaleDivisor );
            return scaledValue;
        }

        public static implicit operator BigDecimal2( Byte value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( sbyte value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( short value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( Int32 value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( Int64 value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( ushort value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( UInt32 value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( UInt64 value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( float value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( Double value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( Decimal value ) {
            return new BigDecimal2( value );
        }

        public static implicit operator BigDecimal2( BigInteger value ) {
            return new BigDecimal2( value, 0 );
        }

        #endregion Explicity and Implicit Casts
    }
}