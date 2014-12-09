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
// "Librainian/UInt256.cs" was last cleaned by Rick on 2014/09/20 at 11:15 PM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Globalization;
    using System.Net;
    using System.Numerics;
    using Annotations;

    /// <summary>
    ///     <para>Pulled from the BitcoinSharp project.</para>
    /// </summary>
    public struct UInt256 : IComparable<UInt256> {
        private static readonly UInt256 _zero = new UInt256( new byte[ 0 ] );

        private readonly int _hashCode;

        private readonly UInt64 _part1; // parts are big-endian
        private readonly UInt64 _part2; // parts are big-endian
        private readonly UInt64 _part3; // parts are big-endian
        private readonly UInt64 _part4; // parts are big-endian

        public UInt256( byte[] value ) {
            if ( value.Length > 32 && !( value.Length == 33 && value[ 32 ] == 0 ) ) {
                throw new ArgumentOutOfRangeException();
            }

            if ( value.Length < 32 ) {
                value = value.Concat( new byte[ 32 - value.Length ] );
            }

            // convert parts and store
            this._part1 = Bits.ToUInt64( value, 24 );
            this._part2 = Bits.ToUInt64( value, 16 );
            this._part3 = Bits.ToUInt64( value, 8 );
            this._part4 = Bits.ToUInt64( value, 0 );

            this._hashCode = this._part1.GetHashMerge( this._part2.GetHashMerge( this._part3.GetHashMerge( this._part4 ) ) );
        }

        public UInt256( int value )
            : this( Bits.GetBytes( value ) ) {
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UInt256( long value )
            : this( Bits.GetBytes( value ) ) {
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UInt256( uint value )
            : this( Bits.GetBytes( value ) ) {
        }

        public UInt256( ulong value )
            : this( Bits.GetBytes( value ) ) {
        }

        public UInt256( BigInteger value )
            : this( value.ToByteArray() ) {
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        private UInt256( UInt64 part1, UInt64 part2, UInt64 part3, UInt64 part4 ) {
            this._part1 = part1;
            this._part2 = part2;
            this._part3 = part3;
            this._part4 = part4;

            this._hashCode = this._part1.GetHashMerge( this._part2.GetHashMerge( this._part3.GetHashMerge( this._part4 ) ) );
        }

        public static UInt256 Zero {
            get {
                return _zero;
            }
        }

        public static UInt256 DivRem( UInt256 dividend, UInt256 divisor, out UInt256 remainder ) {
            BigInteger remainderBigInt;
            var result = new UInt256( BigInteger.DivRem( dividend.ToBigInteger(), divisor.ToBigInteger(), out remainderBigInt ) );
            remainder = new UInt256( remainderBigInt );
            return result;
        }

        public static explicit operator BigInteger( UInt256 value ) => value.ToBigInteger();

        public static explicit operator double( UInt256 value ) => ( double )value.ToBigInteger();

        //TODO properly taken into account host endianness
        public static UInt256 FromByteArrayBE( byte[] buffer ) {
            unchecked {
                if ( buffer.Length != 32 ) {
                    throw new ArgumentException();
                }

                var part1 = ( ulong )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 0 ) );
                var part2 = ( ulong )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 8 ) );
                var part3 = ( ulong )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 16 ) );
                var part4 = ( ulong )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 24 ) );

                return new UInt256( part1, part2, part3, part4 );
            }
        }

        public static implicit operator UInt256( byte value ) => new UInt256( value );

        public static implicit operator UInt256( int value ) => new UInt256( value );

        public static implicit operator UInt256( long value ) => new UInt256( value );

        public static implicit operator UInt256( sbyte value ) => new UInt256( value );

        public static implicit operator UInt256( short value ) => new UInt256( value );

        public static implicit operator UInt256( uint value ) => new UInt256( value );

        public static implicit operator UInt256( ulong value ) => new UInt256( value );

        public static implicit operator UInt256( ushort value ) => new UInt256( value );

        public static double Log( UInt256 value, double baseValue ) => BigInteger.Log( value.ToBigInteger(), baseValue );

        public static bool operator !=( UInt256 left, UInt256 right ) => !( left == right );

        public static UInt256 operator %( UInt256 dividend, UInt256 divisor ) => new UInt256( dividend.ToBigInteger() % divisor.ToBigInteger() );

        public static UInt256 operator *( UInt256 left, UInt256 right ) => new UInt256( left.ToBigInteger() * right.ToBigInteger() );

        public static UInt256 operator /( UInt256 dividend, UInt256 divisor ) => new UInt256( dividend.ToBigInteger() / divisor.ToBigInteger() );

        public static UInt256 operator ~( UInt256 value ) => new UInt256( ~value._part1, ~value._part2, ~value._part3, ~value._part4 );

        public static bool operator <( UInt256 left, UInt256 right ) {
            if ( left._part1 < right._part1 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 < right._part2 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 < right._part3 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 == right._part3 && left._part4 < right._part4 ) {
                return true;
            }

            return false;
        }

        public static bool operator <=( UInt256 left, UInt256 right ) {
            if ( left._part1 < right._part1 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 < right._part2 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 < right._part3 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 == right._part3 && left._part4 < right._part4 ) {
                return true;
            }

            return left == right;
        }

        public static bool operator ==( UInt256 left, UInt256 right ) => left._part1 == right._part1 && left._part2 == right._part2 && left._part3 == right._part3 && left._part4 == right._part4;

        public static bool operator >( UInt256 left, UInt256 right ) {
            if ( left._part1 > right._part1 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 > right._part2 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 > right._part3 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 == right._part3 && left._part4 > right._part4 ) {
                return true;
            }

            return false;
        }

        public static bool operator >=( UInt256 left, UInt256 right ) {
            if ( left._part1 > right._part1 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 > right._part2 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 > right._part3 ) {
                return true;
            }
            if ( left._part1 == right._part1 && left._part2 == right._part2 && left._part3 == right._part3 && left._part4 > right._part4 ) {
                return true;
            }

            return left == right;
        }

        public static UInt256 operator >>( UInt256 value, int shift ) => new UInt256( value.ToBigInteger() >> shift );

        public static UInt256 Parse( string value ) => new UInt256( BigInteger.Parse( "0" + value ).ToByteArray() );

        public static UInt256 Parse( string value, IFormatProvider provider ) => new UInt256( BigInteger.Parse( "0" + value, provider ).ToByteArray() );

        public static UInt256 Parse( string value, NumberStyles style ) => new UInt256( BigInteger.Parse( "0" + value, style ).ToByteArray() );

        public static UInt256 Parse( string value, NumberStyles style, IFormatProvider provider ) => new UInt256( BigInteger.Parse( "0" + value, style, provider ).ToByteArray() );

        public static UInt256 Pow( UInt256 value, int exponent ) => new UInt256( BigInteger.Pow( value.ToBigInteger(), exponent ) );

        public int CompareTo( UInt256 other ) {
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

        public override bool Equals( object obj ) {
            if ( !( obj is UInt256 ) ) {
                return false;
            }

            var other = ( UInt256 )obj;
            return other._part1 == this._part1 && other._part2 == this._part2 && other._part3 == this._part3 && other._part4 == this._part4;
        }

        [Pure]
        public override int GetHashCode() => this._hashCode;

        public BigInteger ToBigInteger() => new BigInteger( this.ToByteArray().Concat( 0 ) );

        public byte[] ToByteArray() {
            var buffer = new byte[ 32 ];
            Buffer.BlockCopy( Bits.GetBytes( this._part4 ), 0, buffer, 0, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part3 ), 0, buffer, 8, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part2 ), 0, buffer, 16, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part1 ), 0, buffer, 24, 8 );

            return buffer;
        }

        public void ToByteArray( byte[] buffer, int offset ) {
            Buffer.BlockCopy( Bits.GetBytes( this._part4 ), 0, buffer, 0 + offset, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part3 ), 0, buffer, 8 + offset, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part2 ), 0, buffer, 16 + offset, 8 );
            Buffer.BlockCopy( Bits.GetBytes( this._part1 ), 0, buffer, 24 + offset, 8 );
        }

        //TODO properly taken into account host endianness
        public byte[] ToByteArrayBE() {
            unchecked {
                var buffer = new byte[ 32 ];
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( long )this._part1 ) ), 0, buffer, 0, 8 );
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( long )this._part2 ) ), 0, buffer, 8, 8 );
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( long )this._part3 ) ), 0, buffer, 16, 8 );
                Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( long )this._part4 ) ), 0, buffer, 24, 8 );

                return buffer;
            }
        }

        public override string ToString() => this.ToHexNumberString();
    }
}