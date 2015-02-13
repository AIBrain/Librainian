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
// "Librainian/UBigInteger.cs" was last cleaned by Rick on 2014/08/23 at 12:47 AM

#endregion License & Information

namespace Librainian.Maths {
    using System;
    using System.Globalization;
    using System.Numerics;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Numerics;

    /// <summary>
    ///     <para>Unsigned biginteger class.</para>
    /// </summary>
    [Immutable]
    public struct UBigInteger : IComparable, IComparable<UBigInteger> {

        /// <summary>
        ///     <para>
        ///         The lowest <see cref="UBigInteger" /> that is higher than <see cref="Zero" />.
        ///     </para>
        ///     <para>Should be "1".</para>
        /// </summary>
        public static readonly UBigInteger Epsilon = new UBigInteger( 1 );

        /// <summary>
        ///     1
        /// </summary>
        public static readonly UBigInteger One = new UBigInteger( 1 );

        /// <summary>
        ///     2
        /// </summary>
        public static readonly UBigInteger Two = new UBigInteger( 2 );

        /// <summary>
        ///     0
        /// </summary>
        public static readonly UBigInteger Zero = new UBigInteger( 0 );

        private readonly BigInteger _internalValue;

        public UBigInteger( UInt64 value ) {
            this._internalValue = value;
        }

        public UBigInteger( [NotNull] byte[] bytes ) {

            // http: //stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger
            if ( bytes == null ) {
                throw new ArgumentNullException( nameof( bytes ) );
            }
            var bytesWith00Attheendnd = new byte[ bytes.Length + 1 ];
            bytes.CopyTo( bytesWith00Attheendnd, 0 );
            bytesWith00Attheendnd[ bytes.Length ] = 0;
            this._internalValue = new BigInteger( bytesWith00Attheendnd );

            this._internalValue.Should().BeGreaterOrEqualTo( 0 );
            if ( this._internalValue < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UBigInteger( Int64 value ) {
            value.Should().BeGreaterOrEqualTo( 0 );
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
            this._internalValue = value;
        }

        private UBigInteger( BigInteger value ) {
            value.Should().BeGreaterOrEqualTo( BigInteger.Zero );
            if ( value < BigInteger.Zero ) {
                throw new ArgumentOutOfRangeException();
            }
            this._internalValue = value;
        }

        public static UBigInteger Add( UBigInteger left, UBigInteger right ) => new UBigInteger( BigInteger.Add( left._internalValue, right._internalValue ) );

        public static explicit operator Decimal( UBigInteger number ) => ( Decimal )number._internalValue;

        public static explicit operator Int32( UBigInteger number ) => ( Int32 )number._internalValue;

        public static explicit operator Int64( UBigInteger number ) => ( Int64 )number._internalValue;

        public static implicit operator BigInteger( UBigInteger number ) => number._internalValue;

        public static implicit operator UBigInteger( long number ) => new UBigInteger( number );

        public static UBigInteger Multiply( UBigInteger left, UBigInteger right ) => new UBigInteger( BigInteger.Multiply( left._internalValue, right._internalValue ) );

        public static UBigInteger operator -( UBigInteger number ) => new UBigInteger( -number._internalValue );

        public static UBigInteger operator -( UBigInteger left, UBigInteger right ) => new UBigInteger( left._internalValue - right._internalValue );

        public static UBigInteger operator %( UBigInteger dividend, UBigInteger divisor ) => new UBigInteger( dividend._internalValue % divisor._internalValue );

        public static UBigInteger operator &( UBigInteger left, UBigInteger right ) => new UBigInteger( left._internalValue & right._internalValue );

        public static UBigInteger operator *( UBigInteger left, UBigInteger right ) => new UBigInteger( left._internalValue * right._internalValue );

        public static UBigInteger operator /( UBigInteger left, UBigInteger right ) => new UBigInteger( left._internalValue / right._internalValue );

        public static Double operator /( Double left, UBigInteger right ) {
            right.Should().BeGreaterThan( Zero );
            var rational = new BigRational( numerator: new BigInteger( left ), denominator: right._internalValue );
            return ( Double )rational;
        }

        public static UBigInteger operator +( UBigInteger left, UBigInteger right ) => new UBigInteger( left._internalValue + right._internalValue );

        public static Boolean operator <( UBigInteger left, long right ) => left._internalValue < right;

        public static Boolean operator <( UBigInteger left, UBigInteger right ) => left._internalValue < right._internalValue;

        public static Boolean operator <( UBigInteger left, ulong right ) => left._internalValue < right;

        public static Boolean operator <( ulong left, UBigInteger right ) => left < right._internalValue;

        public static UBigInteger operator <<( UBigInteger number, int shift ) => new UBigInteger( number._internalValue << shift );

        public static Boolean operator <=( UBigInteger left, ulong right ) => left._internalValue <= right;

        public static Boolean operator <=( UBigInteger left, UBigInteger right ) => left._internalValue <= right._internalValue;

        public static Boolean operator >( UBigInteger left, long right ) => left._internalValue > right;

        public static Boolean operator >( UBigInteger left, UInt64 right ) => left._internalValue > right;

        public static Boolean operator >( UInt64 left, UBigInteger right ) => left > right._internalValue;

        public static Boolean operator >( UBigInteger left, UBigInteger right ) => left._internalValue > right._internalValue;

        public static Boolean operator >=( UBigInteger left, UInt64 right ) => left._internalValue >= right;

        public static Boolean operator >=( UBigInteger left, UBigInteger right ) => left._internalValue >= right._internalValue;

        public static UBigInteger Parse( [NotNull] String number, NumberStyles style ) {
            if ( number == null ) {
                throw new ArgumentNullException( nameof( number ) );
            }
            return new UBigInteger( value: BigInteger.Parse( number, style ) );
        }

        public static UBigInteger Pow( UBigInteger number, int exponent ) => new UBigInteger( BigInteger.Pow( number._internalValue, exponent ) );

        [Pure]
        public int CompareTo( [NotNull] object obj ) {
            if ( obj == null ) {
                throw new ArgumentNullException( nameof( obj ) );
            }
            if ( !( obj is UBigInteger ) ) {
                throw new InvalidCastException();
            }
            return this._internalValue.CompareTo( ( UBigInteger )obj );
        }

        [Pure]
        public int CompareTo( UBigInteger number ) => this._internalValue.CompareTo( number._internalValue );

        public static explicit operator UInt64( UBigInteger number ) => ( UInt64 )number._internalValue;

        [Pure]
        public int CompareTo( long other ) => this._internalValue.CompareTo( other );

        [Pure]
        public int CompareTo( ulong other ) => this._internalValue.CompareTo( other );

        [Pure]
        public byte[] ToByteArray() => this._internalValue.ToByteArray();

        [Pure]
        public override String ToString() => this._internalValue.ToString();

        [Pure]
        public String ToString( String format ) => this._internalValue.ToString( format );

        //public static BigInteger Parse(String value)
        //{
        //    return new BigInteger(System.Numerics.BigInteger.Parse(value));
        //}
    }
}