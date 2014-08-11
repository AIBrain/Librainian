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
// "Librainian/BigDecimal.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

// ReSharper disable once EmptyNamespace
namespace Librainian.Misc {
    //using System;
    //using System.Numerics;

/*
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://gist.github.com/nberardi/2667136"/>
    public struct BigDecimal : IConvertible, IFormattable, IComparable, IComparable<BigDecimal>, IEquatable<BigDecimal> {
        public static readonly BigDecimal MinusOne = new BigDecimal( BigInteger.MinusOne, 0 );
        public static readonly BigDecimal One = new BigDecimal( BigInteger.One, 0 );
        public static readonly BigDecimal Zero = new BigDecimal( BigInteger.Zero, 0 );
        private readonly int _scale;
        private readonly BigInteger _unscaledValue;

        public BigDecimal( double value )
            : this( ( decimal )value ) {
        }

        public BigDecimal( float value )
            : this( ( decimal )value ) {
        }

        public BigDecimal( decimal value ) {
            var bytes = FromDecimal( value );

            var unscaledValueBytes = new byte[ 12 ];
            Array.Copy( bytes, unscaledValueBytes, unscaledValueBytes.Length );

            var unscaledValue = new BigInteger( unscaledValueBytes );
            var scale = bytes[ 14 ];

            if ( bytes[ 15 ] == 128 ) {
                unscaledValue *= BigInteger.MinusOne;
            }

            this._unscaledValue = unscaledValue;
            this._scale = scale;
        }

        public BigDecimal( int value )
            : this( unscaledValue: new BigInteger( value ), scale: 0 ) {
        }

        public BigDecimal( long value )
            : this( unscaledValue: new BigInteger( value ), scale: 0 ) {
        }

        public BigDecimal( uint value )
            : this( unscaledValue: new BigInteger( value ), scale: 0 ) {
        }

        public BigDecimal( ulong value )
            : this( unscaledValue: new BigInteger( value ), scale: 0 ) {
        }

        public BigDecimal( BigInteger unscaledValue, int scale ) {
            this._unscaledValue = unscaledValue;
            this._scale = scale;
        }

        public BigDecimal( byte[] value ) {
            var number = new byte[ value.Length - 4 ];
            var flags = new byte[ 4 ];

            Array.Copy( value, 0, number, 0, number.Length );
            Array.Copy( value, value.Length - 4, flags, 0, 4 );

            this._unscaledValue = new BigInteger( number );
            this._scale = BitConverter.ToInt32( flags, 0 );
        }

        public Boolean IsEven { get { return this._unscaledValue.IsEven; } }

        public Boolean IsOne { get { return this._unscaledValue.IsOne; } }

        public Boolean IsPowerOfTwo { get { return this._unscaledValue.IsPowerOfTwo; } }

        public Boolean IsZero { get { return this._unscaledValue.IsZero; } }

        public int Sign { get { return this._unscaledValue.Sign; } }

        public override Boolean Equals( object obj ) {
            return ( ( obj is BigDecimal ) && this.Equals( ( BigDecimal )obj ) );
        }

        public override int GetHashCode() {
            return this._unscaledValue.GetHashCode() ^ this._scale.GetHashCode();
        }

        object IConvertible.ToType( Type conversionType, IFormatProvider provider ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), this._scale );
            var remainder = BigInteger.Remainder( this._unscaledValue, scaleDivisor );
            var scaledValue = BigInteger.Divide( this._unscaledValue, scaleDivisor );

            if ( scaledValue > new BigInteger( Decimal.MaxValue ) ) {
                throw new ArgumentOutOfRangeException( "value", "The value " + this._unscaledValue + " cannot fit into " + conversionType.Name + "." );
            }

            var leftOfDecimal = ( decimal )scaledValue;
            var rightOfDecimal = ( ( decimal )remainder ) / ( ( decimal )scaleDivisor );

            var value = leftOfDecimal + rightOfDecimal;
            return Convert.ChangeType( value, conversionType );
        }

        public byte[] ToByteArray() {
            var unscaledValue = this._unscaledValue.ToByteArray( );
            var scale = BitConverter.GetBytes( this._scale );

            var bytes = new byte[ unscaledValue.Length + scale.Length ];

            Array.Copy( sourceArray: unscaledValue, sourceIndex: 0, destinationArray: bytes, destinationIndex: 0, length: unscaledValue.Length );
            Array.Copy( sourceArray: scale, sourceIndex: 0, destinationArray: bytes, destinationIndex: unscaledValue.Length, length: scale.Length );

            return bytes;
        }

        public override string ToString() {
            var number = String.Format( "{0:G}", this._unscaledValue );

            if ( this._scale > 0 ) {
                return number.Insert( number.Length - this._scale, "." );
            }

            return number;
        }

        public T ToType<T>() where T : struct {
            return ( T )( this as IConvertible ).ToType( typeof( T ), null );
        }

        private static byte[] FromDecimal( decimal d ) {
            var bytes = new byte[ 16 ];

            var bits = decimal.GetBits( d );
            var lo = bits[ 0 ];
            var mid = bits[ 1 ];
            var hi = bits[ 2 ];
            var flags = bits[ 3 ];

            bytes[ 0 ] = ( byte )lo;
            bytes[ 1 ] = ( byte )( lo >> 8 );
            bytes[ 2 ] = ( byte )( lo >> 0x10 );
            bytes[ 3 ] = ( byte )( lo >> 0x18 );
            bytes[ 4 ] = ( byte )mid;
            bytes[ 5 ] = ( byte )( mid >> 8 );
            bytes[ 6 ] = ( byte )( mid >> 0x10 );
            bytes[ 7 ] = ( byte )( mid >> 0x18 );
            bytes[ 8 ] = ( byte )hi;
            bytes[ 9 ] = ( byte )( hi >> 8 );
            bytes[ 10 ] = ( byte )( hi >> 0x10 );
            bytes[ 11 ] = ( byte )( hi >> 0x18 );
            bytes[ 12 ] = ( byte )flags;
            bytes[ 13 ] = ( byte )( flags >> 8 );
            bytes[ 14 ] = ( byte )( flags >> 0x10 );
            bytes[ 15 ] = ( byte )( flags >> 0x18 );

            return bytes;
        }

        TypeCode IConvertible.GetTypeCode() {
            return TypeCode.Object;
        }

        Boolean IConvertible.ToBoolean( IFormatProvider provider ) {
            return Convert.ToBoolean( this );
        }

        byte IConvertible.ToByte( IFormatProvider provider ) {
            return Convert.ToByte( this );
        }

        char IConvertible.ToChar( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal to Char" );
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal to DateTime" );
        }

        decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            return Convert.ToDecimal( this );
        }

        double IConvertible.ToDouble( IFormatProvider provider ) {
            return Convert.ToDouble( this );
        }

        short IConvertible.ToInt16( IFormatProvider provider ) {
            return Convert.ToInt16( this );
        }

        int IConvertible.ToInt32( IFormatProvider provider ) {
            return Convert.ToInt32( this );
        }

        long IConvertible.ToInt64( IFormatProvider provider ) {
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

        uint IConvertible.ToUInt32( IFormatProvider provider ) {
            return Convert.ToUInt32( this );
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider ) {
            return Convert.ToUInt64( this );
        }

        public string ToString( string format, IFormatProvider formatProvider ) {
            throw new NotImplementedException();
        }

        public int CompareTo( object obj ) {
            if ( obj == null ) {
                return 1;
            }

            if ( !( obj is BigDecimal ) ) {
                throw new ArgumentException( "Compare to object must be a BigDecimal", "obj" );
            }

            return this.CompareTo( ( BigDecimal )obj );
        }

        public int CompareTo( BigDecimal other ) {
            var unscaledValueCompare = this._unscaledValue.CompareTo( other._unscaledValue );
            var scaleCompare = this._scale.CompareTo( other._scale );

            // if both are the same value, return the value
            if ( unscaledValueCompare == scaleCompare ) {
                return unscaledValueCompare;
            }

            // if the scales are both the same return unscaled value
            if ( scaleCompare == 0 ) {
                return unscaledValueCompare;
            }

            var scaledValue = BigInteger.Divide( this._unscaledValue, BigInteger.Pow( new BigInteger( 10 ), this._scale ) );
            var otherScaledValue = BigInteger.Divide( other._unscaledValue, BigInteger.Pow( new BigInteger( 10 ), other._scale ) );

            return scaledValue.CompareTo( otherScaledValue );
        }

        public Boolean Equals( BigDecimal other ) {
            return Equals( this, other );
        }

        /// <summary>
        /// static comparison test
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( BigDecimal left, BigDecimal right ) {
            return left._scale == right._scale && left._unscaledValue == right._unscaledValue;
        }

        public static Boolean operator !=( BigDecimal left, BigDecimal right ) {
            return !Equals( left, right );
        }

        public static Boolean operator !=( BigDecimal left, decimal right ) {
            return !Equals( left, right );
        }

        public static Boolean operator !=( decimal left, BigDecimal right ) {
            return !Equals( left, right );
        }

        public static Boolean operator <( BigDecimal left, BigDecimal right ) {
            return left.CompareTo( right ) < 0;
        }

        public static Boolean operator <( BigDecimal left, decimal right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static Boolean operator <( decimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static Boolean operator <=( BigDecimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static Boolean operator <=( BigDecimal left, decimal right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static Boolean operator <=( decimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static Boolean operator ==( BigDecimal left, BigDecimal right ) {
            return left.Equals( right );
        }

        public static Boolean operator ==( BigDecimal left, decimal right ) {
            return left.Equals( right );
        }

        public static Boolean operator ==( decimal left, BigDecimal right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( BigDecimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static Boolean operator >( BigDecimal left, decimal right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static Boolean operator >( decimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static Boolean operator >=( BigDecimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static Boolean operator >=( BigDecimal left, decimal right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static Boolean operator >=( decimal left, BigDecimal right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        [Obsolete( "possible loss of information here" )]
        public static explicit operator BigInteger( BigDecimal value ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), value._scale );
            var scaledValue = BigInteger.Divide( value._unscaledValue, scaleDivisor );
            return scaledValue;
        }

        public static explicit operator byte( BigDecimal value ) {
            return value.ToType<byte>();
        }

        public static explicit operator double( BigDecimal value ) {
            return value.ToType<double>();
        }

        public static explicit operator ushort( BigDecimal value ) {
            return value.ToType<ushort>();
        }

        public static explicit operator ulong( BigDecimal value ) {
            return value.ToType<ulong>();
        }

        public static explicit operator int( BigDecimal value ) {
            return value.ToType<int>();
        }

        public static explicit operator short( BigDecimal value ) {
            return value.ToType<short>();
        }

        public static explicit operator sbyte( BigDecimal value ) {
            return value.ToType<sbyte>();
        }

        public static explicit operator long( BigDecimal value ) {
            return value.ToType<long>();
        }

        public static explicit operator uint( BigDecimal value ) {
            return value.ToType<uint>();
        }

        public static explicit operator float( BigDecimal value ) {
            return value.ToType<float>();
        }

        public static explicit operator decimal( BigDecimal value ) {
            return value.ToType<decimal>();
        }

        public static implicit operator BigDecimal( byte value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( sbyte value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( short value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( int value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( long value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( ushort value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( uint value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( ulong value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( float value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( double value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( decimal value ) {
            return new BigDecimal( value );
        }

        public static implicit operator BigDecimal( BigInteger value ) {
            return new BigDecimal( value, 0 );
        }
    }
*/
}
