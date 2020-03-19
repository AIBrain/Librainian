// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "UBigInteger.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "UBigInteger.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Globalization;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Rationals;

    /// <summary>
    ///     <para>Unsigned biginteger class.</para>
    /// </summary>
    [Immutable]
    public struct UBigInteger : IComparable, IComparable<UBigInteger> {

        private readonly BigInteger _internalValue;

        /// <summary>
        ///     <para>The lowest <see cref="UBigInteger" /> that is higher than <see cref="Zero" />.</para>
        ///     <para>Should be "1".</para>
        /// </summary>
        public static readonly UBigInteger Epsilon = new UBigInteger( value: 1 );

        /// <summary>1</summary>
        public static readonly UBigInteger One = new UBigInteger( value: 1 );

        /// <summary>2</summary>
        public static readonly UBigInteger Two = new UBigInteger( value: 2 );

        /// <summary>0</summary>
        public static readonly UBigInteger Zero = new UBigInteger( value: 0 );

        private UBigInteger( BigInteger value ) {

            //value.Should().BeGreaterOrEqualTo(expected: BigInteger.Zero);

            if ( value < BigInteger.Zero ) {
                throw new ArgumentOutOfRangeException();
            }

            this._internalValue = value;
        }

        public UBigInteger( UInt64 value ) => this._internalValue = value;

        public UBigInteger( [NotNull] Byte[] bytes ) {

            // http: //stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger
            if ( bytes is null ) {
                throw new ArgumentNullException( paramName: nameof( bytes ) );
            }

            var bytesWith00Attheendnd = new Byte[ bytes.Length + 1 ];
            bytes.CopyTo( array: bytesWith00Attheendnd, index: 0 );
            bytesWith00Attheendnd[ bytes.Length ] = 0;
            this._internalValue = new BigInteger( value: bytesWith00Attheendnd );

            //this._internalValue.Should().BeGreaterOrEqualTo(expected: 0);

            if ( this._internalValue < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UBigInteger( Int64 value ) {

            //value.Should().BeGreaterOrEqualTo(expected: 0);

            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }

            this._internalValue = value;
        }

        public static UBigInteger Add( UBigInteger left, UBigInteger right ) =>
            new UBigInteger( value: BigInteger.Add( left: left._internalValue, right: right._internalValue ) );

        public static explicit operator Decimal( UBigInteger number ) => ( Decimal ) number._internalValue;

        public static explicit operator Int32( UBigInteger number ) => ( Int32 ) number._internalValue;

        public static explicit operator Int64( UBigInteger number ) => ( Int64 ) number._internalValue;

        public static explicit operator UInt64( UBigInteger number ) => ( UInt64 ) number._internalValue;

        public static implicit operator BigInteger( UBigInteger number ) => number._internalValue;

        public static implicit operator UBigInteger( Int64 number ) => new UBigInteger( value: number );

        public static UBigInteger Multiply( UBigInteger left, UBigInteger right ) =>
            new UBigInteger( value: BigInteger.Multiply( left: left._internalValue, right: right._internalValue ) );

        public static UBigInteger operator -( UBigInteger number ) => new UBigInteger( value: -number._internalValue );

        public static UBigInteger operator -( UBigInteger left, UBigInteger right ) => new UBigInteger( value: left._internalValue - right._internalValue );

        public static UBigInteger operator %( UBigInteger dividend, UBigInteger divisor ) => new UBigInteger( value: dividend._internalValue % divisor._internalValue );

        public static UBigInteger operator &( UBigInteger left, UBigInteger right ) => new UBigInteger( value: left._internalValue & right._internalValue );

        public static UBigInteger operator *( UBigInteger left, UBigInteger right ) => new UBigInteger( value: left._internalValue * right._internalValue );

        public static UBigInteger operator /( UBigInteger left, UBigInteger right ) => new UBigInteger( value: left._internalValue / right._internalValue );

        public static Double operator /( Double left, UBigInteger right ) {

            //right.Should().BeGreaterThan(expected: Zero);
            var rational = new Rational( numerator: new BigInteger( value: left ), denominator: right._internalValue );

            return ( Double ) rational;
        }

        public static UBigInteger operator +( UBigInteger left, UBigInteger right ) => new UBigInteger( value: left._internalValue + right._internalValue );

        public static Boolean operator <( UBigInteger left, Int64 right ) => left._internalValue < right;

        public static Boolean operator <( UBigInteger left, UBigInteger right ) => left._internalValue < right._internalValue;

        public static Boolean operator <( UBigInteger left, UInt64 right ) => left._internalValue < right;

        public static Boolean operator <( UInt64 left, UBigInteger right ) => left < right._internalValue;

        public static UBigInteger operator <<( UBigInteger number, Int32 shift ) => new UBigInteger( value: number._internalValue << shift );

        public static Boolean operator <=( UBigInteger left, UInt64 right ) => left._internalValue <= right;

        public static Boolean operator <=( UBigInteger left, UBigInteger right ) => left._internalValue <= right._internalValue;

        public static Boolean operator >( UBigInteger left, Int64 right ) => left._internalValue > right;

        public static Boolean operator >( UBigInteger left, UInt64 right ) => left._internalValue > right;

        public static Boolean operator >( UInt64 left, UBigInteger right ) => left > right._internalValue;

        public static Boolean operator >( UBigInteger left, UBigInteger right ) => left._internalValue > right._internalValue;

        public static Boolean operator >=( UBigInteger left, UInt64 right ) => left._internalValue >= right;

        public static Boolean operator >=( UBigInteger left, UBigInteger right ) => left._internalValue >= right._internalValue;

        public static UBigInteger Parse( [NotNull] String number, NumberStyles style ) {
            if ( number is null ) {
                throw new ArgumentNullException( paramName: nameof( number ) );
            }

            return new UBigInteger( value: BigInteger.Parse( value: number, style: style ) );
        }

        public static UBigInteger Pow( UBigInteger number, Int32 exponent ) => new UBigInteger( value: BigInteger.Pow( value: number._internalValue, exponent: exponent ) );

        [Pure]
        public Int32 CompareTo( [NotNull] Object obj ) {
            if ( obj is null ) {
                throw new ArgumentNullException( paramName: nameof( obj ) );
            }

            if ( !( obj is UBigInteger ) ) {
                throw new InvalidCastException();
            }

            return this._internalValue.CompareTo( other: ( UBigInteger ) obj );
        }

        public Int32 CompareTo( UBigInteger number ) => this._internalValue.CompareTo( other: number._internalValue );

        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        public Int32 CompareTo( Int64 other ) => this._internalValue.CompareTo( other: other );

        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        public Int32 CompareTo( UInt64 other ) => this._internalValue.CompareTo( other: other );

        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        [NotNull]
        public Byte[] ToByteArray() => this._internalValue.ToByteArray();

        [CanBeNull]
        public override String ToString() => this._internalValue.ToString();

        [CanBeNull]
        public String ToString( [NotNull] String format ) => this._internalValue.ToString( format: format );

    }

}