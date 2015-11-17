#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/UInt256.cs" was last cleaned by Rick on 2015/06/12 at 3:01 PM
#endregion License & Information

namespace Librainian.Maths {
    using System;
    using System.Globalization;
    using System.Net;
    using System.Numerics;
    using JetBrains.Annotations;

    /// <summary>
    /// <para>Pulled from the BitcoinSharp project.</para></summary>
    public struct UInt256 : IComparable<UInt256> {
        private readonly Int32 _hashCode;
        private readonly UInt64 _part1; // parts are big-endian
        private readonly UInt64 _part2; // parts are big-endian
        private readonly UInt64 _part3; // parts are big-endian
        private readonly UInt64 _part4; // parts are big-endian

        public UInt256(Byte[] value) {
            if ( ( value.Length > 32 ) && !( ( value.Length == 33 ) && ( value[ 32 ] == 0 ) ) ) {
                throw new ArgumentOutOfRangeException();
            }

            if ( value.Length < 32 ) {
                value = value.Concat( new Byte[ 32 - value.Length ] );
            }

            // convert parts and store
            this._part1 = Bits.ToUInt64( value, 24 );
            this._part2 = Bits.ToUInt64( value, 16 );
            this._part3 = Bits.ToUInt64( value, 8 );
            this._part4 = Bits.ToUInt64( value, 0 );

            this._hashCode = this._part1.GetHashMerge( this._part2.GetHashMerge( this._part3.GetHashMerge( this._part4 ) ) );
        }

        public UInt256(Int32 value) : this( Bits.GetBytes( value ) ) {
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UInt256(Int64 value) : this( Bits.GetBytes( value ) ) {
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UInt256(UInt32 value) : this( Bits.GetBytes( value ) ) {
        }

        public UInt256(UInt64 value) : this( Bits.GetBytes( value ) ) {
        }

        public UInt256(BigInteger value) : this( value.ToByteArray() ) {
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        private UInt256(UInt64 part1, UInt64 part2, UInt64 part3, UInt64 part4) {
            this._part1 = part1;
            this._part2 = part2;
            this._part3 = part3;
            this._part4 = part4;

            this._hashCode = this._part1.GetHashMerge( this._part2.GetHashMerge( this._part3.GetHashMerge( this._part4 ) ) );
        }

        public static UInt256 Zero {
            get;
        }
        = new UInt256( new Byte[ 0 ] );

        public Int32 CompareTo(UInt256 other) {
            if ( this == other ) {
                return 0;
            }
            if ( this < other ) {
                return -1;
            }
            if ( this > other ) {
                return +1;
            }

            throw new Exception();
        }

        public static UInt256 DivRem(UInt256 dividend, UInt256 divisor, out UInt256 remainder) {
            BigInteger remainderBigInt;
            var result = new UInt256( BigInteger.DivRem( dividend.ToBigInteger(), divisor.ToBigInteger(), out remainderBigInt ) );
            remainder = new UInt256( remainderBigInt );
            return result;
        }

        public static explicit operator BigInteger(UInt256 value) => value.ToBigInteger();

        public static explicit operator Double (UInt256 value) => ( Double )value.ToBigInteger();

        //TODO properly taken into account host endianness
        public static UInt256 FromByteArray(Byte[] buffer) {
            unchecked {
                if ( buffer.Length != 32 ) {
                    throw new ArgumentException();
                }

                var part1 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 0 ) );
                var part2 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 8 ) );
                var part3 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 16 ) );
                var part4 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 24 ) );

                return new UInt256( part1, part2, part3, part4 );
            }
        }

        public static implicit operator UInt256(Byte value) => new UInt256( value );

        public static implicit operator UInt256(Int32 value) => new UInt256( value );

        public static implicit operator UInt256(Int64 value) => new UInt256( value );

        public static implicit operator UInt256(SByte value) => new UInt256( value );

        public static implicit operator UInt256(Int16 value) => new UInt256( value );

        public static implicit operator UInt256(UInt32 value) => new UInt256( value );

        public static implicit operator UInt256(UInt64 value) => new UInt256( value );

        public static implicit operator UInt256(UInt16 value) => new UInt256( value );

        public static Double Log(UInt256 value, Double baseValue) => BigInteger.Log( value.ToBigInteger(), baseValue );

        public static Boolean operator !=(UInt256 left, UInt256 right) => !( left == right );

        public static UInt256 operator %(UInt256 dividend, UInt256 divisor) => new UInt256( dividend.ToBigInteger() % divisor.ToBigInteger() );

        public static UInt256 operator *(UInt256 left, UInt256 right) => new UInt256( left.ToBigInteger() * right.ToBigInteger() );

        public static UInt256 operator /(UInt256 dividend, UInt256 divisor) => new UInt256( dividend.ToBigInteger() / divisor.ToBigInteger() );

        public static UInt256 operator ~(UInt256 value) => new UInt256( ~value._part1, ~value._part2, ~value._part3, ~value._part4 );

        public static Boolean operator <(UInt256 left, UInt256 right) {
            if ( left._part1 < right._part1 ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 < right._part2 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 < right._part3 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 < right._part4 ) ) {
                return true;
            }

            return false;
        }

        public static Boolean operator <=(UInt256 left, UInt256 right) {
            if ( left._part1 < right._part1 ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 < right._part2 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 < right._part3 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 < right._part4 ) ) {
                return true;
            }

            return left == right;
        }

        public static Boolean operator ==(UInt256 left, UInt256 right) => ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 == right._part4 );

        public static Boolean operator >(UInt256 left, UInt256 right) {
            if ( left._part1 > right._part1 ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 > right._part2 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 > right._part3 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 > right._part4 ) ) {
                return true;
            }

            return false;
        }

        public static Boolean operator >=(UInt256 left, UInt256 right) {
            if ( left._part1 > right._part1 ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 > right._part2 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 > right._part3 ) ) {
                return true;
            }
            if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 > right._part4 ) ) {
                return true;
            }

            return left == right;
        }

        public static UInt256 operator >>(UInt256 value, Int32 shift) => new UInt256( value.ToBigInteger() >> shift );

        public static UInt256 Parse(String value) => new UInt256( BigInteger.Parse( "0" + value ).ToByteArray() );

        public static UInt256 Parse(String value, IFormatProvider provider) => new UInt256( BigInteger.Parse( "0" + value, provider ).ToByteArray() );

        public static UInt256 Parse(String value, NumberStyles style) => new UInt256( BigInteger.Parse( "0" + value, style ).ToByteArray() );

        public static UInt256 Parse(String value, NumberStyles style, IFormatProvider provider) => new UInt256( BigInteger.Parse( "0" + value, style, provider ).ToByteArray() );

        public static UInt256 Pow(UInt256 value, Int32 exponent) => new UInt256( BigInteger.Pow( value.ToBigInteger(), exponent ) );

        public override Boolean Equals(Object obj) {
            if ( !( obj is UInt256 ) ) {
                return false;
            }

            var other = ( UInt256 )obj;
            return ( other._part1 == this._part1 ) && ( other._part2 == this._part2 ) && ( other._part3 == this._part3 ) && ( other._part4 == this._part4 );
        }

        [Pure]
        public override Int32 GetHashCode() => this._hashCode;

        public BigInteger ToBigInteger() => new BigInteger( this.ToByteArray().Concat( 0 ) );

        public Byte[] ToByteArray() {
            var buffer = new Byte[ 32 ];
            Buffer.BlockCopy( Bits.GetBytes( this._part4 ), 0, buffer, 0, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part3 ), 0, buffer, 8, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part2 ), 0, buffer, 16, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part1 ), 0, buffer, 24, 8 );

            return buffer;
        }

        public void ToByteArray(Byte[] buffer, Int32 offset) {
            Buffer.BlockCopy( Bits.GetBytes( this._part4 ), 0, buffer, 0 + offset, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part3 ), 0, buffer, 8 + offset, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part2 ), 0, buffer, 16 + offset, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part1 ), 0, buffer, 24 + offset, 8 );
        }

        //TODO properly taken into account host endianness
        public Byte[] ToByteArrayBe() {
            unchecked {
                var buffer = new Byte[ 32 ];
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part1 ) ), 0, buffer, 0, 8 );
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part2 ) ), 0, buffer, 8, 8 );
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part3 ) ), 0, buffer, 16, 8 );
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part4 ) ), 0, buffer, 24, 8 );

                return buffer;
            }
        }

        public override String ToString() => this.ToHexNumberString();
    }
}