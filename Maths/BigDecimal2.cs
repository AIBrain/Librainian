namespace Librainian.Maths {
    using System;
    using System.Numerics;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://gist.github.com/nberardi/2667136"/>
    public struct BigDecimal2 : IConvertible, IFormattable, IComparable, IComparable<BigDecimal2>, IEquatable<BigDecimal2> {
        public static readonly BigDecimal2 MinusOne = new BigDecimal2( BigInteger.MinusOne, 0 );
        public static readonly BigDecimal2 Zero = new BigDecimal2( BigInteger.Zero, 0 );
        public static readonly BigDecimal2 One = new BigDecimal2( BigInteger.One, 0 );

        private readonly BigInteger _unscaledValue;
        private readonly int _scale;

        public BigDecimal2( double value )
            : this( ( decimal )value ) { }

        public BigDecimal2( float value )
            : this( ( decimal )value ) { }

        public BigDecimal2( decimal value ) {
            var bytes = FromDecimal( value );

            var unscaledValueBytes = new byte[ 12 ];
            Array.Copy( bytes, unscaledValueBytes, unscaledValueBytes.Length );

            var unscaledValue = new BigInteger( unscaledValueBytes );
            var scale = bytes[ 14 ];

            if ( bytes[ 15 ] == 128 )
                unscaledValue *= BigInteger.MinusOne;

            this._unscaledValue = unscaledValue;
            this._scale = scale;
        }

        public BigDecimal2( int value )
            : this( new BigInteger( value ), 0 ) { }

        public BigDecimal2( long value )
            : this( new BigInteger( value ), 0 ) { }

        public BigDecimal2( uint value )
            : this( new BigInteger( value ), 0 ) { }

        public BigDecimal2( ulong value )
            : this( new BigInteger( value ), 0 ) { }

        public BigDecimal2( BigInteger unscaledValue, int scale ) {
            this._unscaledValue = unscaledValue;
            this._scale = scale;
        }

        public BigDecimal2( byte[] value ) {
            var number = new byte[ value.Length - 4 ];
            var flags = new byte[ 4 ];

            Array.Copy( value, 0, number, 0, number.Length );
            Array.Copy( value, value.Length - 4, flags, 0, 4 );

            this._unscaledValue = new BigInteger( number );
            this._scale = BitConverter.ToInt32( flags, 0 );
        }

        public bool IsEven { get { return this._unscaledValue.IsEven; } }
        public bool IsOne { get { return this._unscaledValue.IsOne; } }
        public bool IsPowerOfTwo { get { return this._unscaledValue.IsPowerOfTwo; } }
        public bool IsZero { get { return this._unscaledValue.IsZero; } }
        public int Sign { get { return this._unscaledValue.Sign; } }

        public override string ToString() {
            var number = this._unscaledValue.ToString( "G" );

            if ( this._scale > 0 )
                return number.Insert( number.Length - this._scale, "." );

            return number;
        }

        public byte[] ToByteArray() {
            var unscaledValue = this._unscaledValue.ToByteArray();
            var scale = BitConverter.GetBytes( this._scale );

            var bytes = new byte[ unscaledValue.Length + scale.Length ];
            Array.Copy( unscaledValue, 0, bytes, 0, unscaledValue.Length );
            Array.Copy( scale, 0, bytes, unscaledValue.Length, scale.Length );

            return bytes;
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

        #region Operators

        public static bool operator ==( BigDecimal2 left, BigDecimal2 right ) {
            return left.Equals( right );
        }

        public static bool operator !=( BigDecimal2 left, BigDecimal2 right ) {
            return !left.Equals( right );
        }

        public static bool operator >( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static bool operator >=( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static bool operator <( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static bool operator <=( BigDecimal2 left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static bool operator ==( BigDecimal2 left, decimal right ) {
            return left.Equals( right );
        }

        public static bool operator !=( BigDecimal2 left, decimal right ) {
            return !left.Equals( right );
        }

        public static bool operator >( BigDecimal2 left, decimal right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static bool operator >=( BigDecimal2 left, decimal right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static bool operator <( BigDecimal2 left, decimal right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static bool operator <=( BigDecimal2 left, decimal right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        public static bool operator ==( decimal left, BigDecimal2 right ) {
            return left.Equals( right );
        }

        public static bool operator !=( decimal left, BigDecimal2 right ) {
            return !left.Equals( right );
        }

        public static bool operator >( decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) > 0 );
        }

        public static bool operator >=( decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) >= 0 );
        }

        public static bool operator <( decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) < 0 );
        }

        public static bool operator <=( decimal left, BigDecimal2 right ) {
            return ( left.CompareTo( right ) <= 0 );
        }

        #endregion

        #region Explicity and Implicit Casts

        public static explicit operator byte( BigDecimal2 value ) { return value.ToType<byte>(); }
        public static explicit operator sbyte( BigDecimal2 value ) { return value.ToType<sbyte>(); }
        public static explicit operator short( BigDecimal2 value ) { return value.ToType<short>(); }
        public static explicit operator int( BigDecimal2 value ) { return value.ToType<int>(); }
        public static explicit operator long( BigDecimal2 value ) { return value.ToType<long>(); }
        public static explicit operator ushort( BigDecimal2 value ) { return value.ToType<ushort>(); }
        public static explicit operator uint( BigDecimal2 value ) { return value.ToType<uint>(); }
        public static explicit operator ulong( BigDecimal2 value ) { return value.ToType<ulong>(); }
        public static explicit operator float( BigDecimal2 value ) { return value.ToType<float>(); }
        public static explicit operator double( BigDecimal2 value ) { return value.ToType<double>(); }
        public static explicit operator decimal( BigDecimal2 value ) { return value.ToType<decimal>(); }
        public static explicit operator BigInteger( BigDecimal2 value ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), value._scale );
            var scaledValue = BigInteger.Divide( value._unscaledValue, scaleDivisor );
            return scaledValue;
        }

        public static implicit operator BigDecimal2( byte value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( sbyte value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( short value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( int value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( long value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( ushort value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( uint value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( ulong value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( float value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( double value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( decimal value ) { return new BigDecimal2( value ); }
        public static implicit operator BigDecimal2( BigInteger value ) { return new BigDecimal2( value, 0 ); }

        #endregion

        public T ToType<T>() where T : struct {
            return ( T )( ( IConvertible )this ).ToType( typeof( T ), null );
        }

        object IConvertible.ToType( Type conversionType, IFormatProvider provider ) {
            var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), this._scale );
            var remainder = BigInteger.Remainder( this._unscaledValue, scaleDivisor );
            var scaledValue = BigInteger.Divide( this._unscaledValue, scaleDivisor );

            if ( scaledValue > new BigInteger( Decimal.MaxValue ) )
                throw new ArgumentOutOfRangeException( "value", "The value " + this._unscaledValue + " cannot fit into " + conversionType.Name + "." );

            var leftOfDecimal = ( decimal )scaledValue;
            var rightOfDecimal = ( ( decimal )remainder ) / ( ( decimal )scaleDivisor );

            var value = leftOfDecimal + rightOfDecimal;
            return Convert.ChangeType( value, conversionType );
        }

        public override bool Equals( object obj ) {
            return ( ( obj is BigDecimal2 ) && this.Equals( ( BigDecimal2 )obj ) );
        }

        public override int GetHashCode() {
            return this._unscaledValue.GetHashCode() ^ this._scale.GetHashCode();
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode() {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean( IFormatProvider provider ) {
            return Convert.ToBoolean( this );
        }

        byte IConvertible.ToByte( IFormatProvider provider ) {
            return Convert.ToByte( this );
        }

        char IConvertible.ToChar( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal2 to Char" );
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {
            throw new InvalidCastException( "Cannot cast BigDecimal2 to DateTime" );
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

        #endregion

        #region IFormattable Members

        public string ToString( string format, IFormatProvider formatProvider ) {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable Members

        public int CompareTo( object obj ) {
            if ( obj == null )
                return 1;

            if ( !( obj is BigDecimal2 ) )
                throw new ArgumentException( "Compare to object must be a BigDecimal2", "obj" );

            return this.CompareTo( ( BigDecimal2 )obj );
        }

        #endregion

        #region IComparable<BigDecimal2> Members

        public int CompareTo( BigDecimal2 other ) {
            var unscaledValueCompare = this._unscaledValue.CompareTo( other._unscaledValue );
            var scaleCompare = this._scale.CompareTo( other._scale );

            // if both are the same value, return the value
            if ( unscaledValueCompare == scaleCompare )
                return unscaledValueCompare;

            // if the scales are both the same return unscaled value
            if ( scaleCompare == 0 )
                return unscaledValueCompare;

            var scaledValue = BigInteger.Divide( this._unscaledValue, BigInteger.Pow( new BigInteger( 10 ), this._scale ) );
            var otherScaledValue = BigInteger.Divide( other._unscaledValue, BigInteger.Pow( new BigInteger( 10 ), other._scale ) );

            return scaledValue.CompareTo( otherScaledValue );
        }

        #endregion

        #region IEquatable<BigDecimal2> Members

        public bool Equals( BigDecimal2 other ) {
            return this._scale == other._scale && this._unscaledValue == other._unscaledValue;
        }

        #endregion
    }
}