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
// "Librainian2/UBigInteger.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Globalization;
    using System.Numerics;
    using Annotations;
    using FluentAssertions;
    using Extensions;
    using Numerics;

    /// <summary>
    ///     Unsigned biginteger class.
    /// </summary>
    [UsedImplicitly]
    [Immutable]
    public struct UBigInteger : IComparable, IComparable< UBigInteger > {
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

        private readonly BigInteger _internalNumber;

        public UBigInteger( UInt64 number ) {
            this._internalNumber = number;
        }

        public UBigInteger( [NotNull] byte[] bytes ) {
            // http: //stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger
            if ( bytes == null ) {
                throw new ArgumentNullException( "bytes" );
            }
            var bytesWith00attheendnd = new byte[bytes.Length + 1];
            bytes.CopyTo( bytesWith00attheendnd, 0 );
            bytesWith00attheendnd[ bytes.Length ] = 0;
            this._internalNumber = new BigInteger( bytesWith00attheendnd );

            this._internalNumber.Should().BeGreaterOrEqualTo( 0 );
            if ( this._internalNumber < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UBigInteger( Int64 number ) {
            number.Should().BeGreaterOrEqualTo( 0 );
            if ( number < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
            this._internalNumber = number;
        }

        private UBigInteger( BigInteger number ) {
            number.Should().BeGreaterOrEqualTo( BigInteger.Zero );
            if ( number < BigInteger.Zero ) {
                throw new ArgumentOutOfRangeException();
            }
            this._internalNumber = number;
        }

        [Pure]
        public int CompareTo( [NotNull] object obj ) {
            if ( obj == null ) {
                throw new ArgumentNullException( "obj" );
            }
            if ( !( obj is UBigInteger ) ) {
                throw new InvalidCastException();
            }
            return this._internalNumber.CompareTo( ( UBigInteger ) obj );
        }

        [Pure]
        public int CompareTo( UBigInteger number ) {
            return this._internalNumber.CompareTo( number._internalNumber );
        }

        /*
         * wtf was this prop doing here?
                public UBigInteger Number {
                    get { return new UBigInteger( this._internalNumber ); }

                    set {
                        value.Should().BeGreaterOrEqualTo( 0 );
                        this._internalNumber = new BigInteger( value.ToByteArray() );
                    }
                }
        */

        public static UBigInteger Add( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( BigInteger.Add( left._internalNumber, right._internalNumber ) );
        }

        //public static explicit operator Int32( UBigInteger number ) {
        //    return ( Int32 )number._internalNumber;
        //}

        //public static explicit operator Int64( UBigInteger number ) {
        //    return ( Int64 )number._internalNumber;
        //}

        //public static explicit operator UInt64( UBigInteger number ) {
        //    return ( UInt64 )number._internalNumber;
        //}

        public static implicit operator Decimal( UBigInteger number ) {
            return ( Decimal ) number._internalNumber;
        }

        public static implicit operator BigInteger( UBigInteger number ) {
            return number._internalNumber;
        }

        public static implicit operator UBigInteger( long number ) {
            return new UBigInteger( number );
        }

        public static UBigInteger Multiply( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( BigInteger.Multiply( left._internalNumber, right._internalNumber ) );
        }

        public static UBigInteger operator -( UBigInteger number ) {
            return new UBigInteger( -number._internalNumber );
        }

        public static UBigInteger operator -( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalNumber - right._internalNumber );
        }

        public static UBigInteger operator %( UBigInteger dividend, UBigInteger divisor ) {
            return new UBigInteger( dividend._internalNumber%divisor._internalNumber );
        }

        public static UBigInteger operator &( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalNumber & right._internalNumber );
        }

        public static UBigInteger operator *( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalNumber*right._internalNumber );
        }

        public static UBigInteger operator /( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalNumber/right._internalNumber );
        }

        public static Double operator /( Double left, UBigInteger right ) {
            right.Should().BeGreaterThan( Zero );
            var rational = new BigRational( numerator: new BigInteger( left ), denominator: right._internalNumber );
            return ( Double ) rational;
        }

        public static UBigInteger operator +( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalNumber + right._internalNumber );
        }

        public static Boolean operator <( UBigInteger left, long right ) {
            return left._internalNumber < right;
        }

        public static Boolean operator <( UBigInteger left, UBigInteger right ) {
            return left._internalNumber < right._internalNumber;
        }

        public static Boolean operator <( UBigInteger left, ulong right ) {
            return left._internalNumber < right;
        }

        public static Boolean operator <( ulong left, UBigInteger right ) {
            return left < right._internalNumber;
        }

        public static UBigInteger operator <<( UBigInteger number, int shift ) {
            return new UBigInteger( number._internalNumber << shift );
        }

        public static Boolean operator <=( UBigInteger left, ulong right ) {
            return left._internalNumber <= right;
        }

        public static Boolean operator <=( UBigInteger left, UBigInteger right ) {
            return left._internalNumber <= right._internalNumber;
        }

        public static Boolean operator >( UBigInteger left, long right ) {
            return left._internalNumber > right;
        }

        public static Boolean operator >( UBigInteger left, UInt64 right ) {
            return left._internalNumber > right;
        }

        public static Boolean operator >( UInt64 left, UBigInteger right ) {
            return left > right._internalNumber;
        }

        public static Boolean operator >( UBigInteger left, UBigInteger right ) {
            return left._internalNumber > right._internalNumber;
        }

        public static Boolean operator >=( UBigInteger left, UInt64 right ) {
            return left._internalNumber >= right;
        }

        public static Boolean operator >=( UBigInteger left, UBigInteger right ) {
            return left._internalNumber >= right._internalNumber;
        }

        public static UBigInteger Parse( [NotNull] string number, NumberStyles style ) {
            if ( number == null ) {
                throw new ArgumentNullException( "number" );
            }
            return new UBigInteger( number: BigInteger.Parse( number, style ) );
        }

        public static UBigInteger Pow( UBigInteger number, int exponent ) {
            return new UBigInteger( BigInteger.Pow( number._internalNumber, exponent ) );
        }

        [Pure]
        public int CompareTo( long other ) {
            return this._internalNumber.CompareTo( other );
        }

        [Pure]
        public byte[] ToByteArray() {
            return this._internalNumber.ToByteArray();
        }

        [Pure]
        public override string ToString() {
            return this._internalNumber.ToString();
        }

        [Pure]
        public string ToString( string format ) {
            return this._internalNumber.ToString( format );
        }

        //public static BigInteger Parse(string value)
        //{
        //    return new BigInteger(System.Numerics.BigInteger.Parse(value));
        //}
    }
}
