// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "BigDecimal.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "BigDecimal.cs" was last formatted by Protiguous on 2018/07/13 at 1:18 AM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Numerics;
	using ComputerSystem;
	using Extensions;
	using Hashings;
	using JetBrains.Annotations;
	using Numerics;
	using NUnit.Framework;
	using Parsing;

	/// <summary>
	///     <para>Arbitrary precision Decimal.</para>
	///     <para>
	///         All operations are exact, except for division. Division never determines more digits than the given
	///         precision.
	///     </para>
	///     <para>Based on http://stackoverflow.com/a/4524254 spacer</para>
	///     <para>Author: Jan Christoph Bernack (contact: jc.bernack at googlemail.com)</para>
	///     <para>Joined with code from nberardi from gist 2667136</para>
	///     <para>Rewritten into an immutable struct by Rick@Protiguous.com in August 2014</para>
	///     <para>Added the parsing ability from the 'clojure' project.</para>
	/// </summary>
	/// <see cref="http://stackoverflow.com/a/13813535/956364" />
	/// <see cref="http://gist.github.com/nberardi/2667136" />
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Obsolete( "Use BigRational instead." )]
	public struct BigDecimal : IComparable, IComparable<BigDecimal>, IConvertible, /*IFormattable,*/ IEquatable<BigDecimal> {

		/// <summary>
		///     - 1
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
		/// <see cref="http://wikipedia.org/wiki/Exponent" />
		public readonly Int32 Exponent;

		/// <summary>
		///     The <see cref="Significand" /> (aka <see cref="Mantissa" />) is the part of a number consisting of its significant
		///     digits.
		/// </summary>
		/// <see cref="http://wikipedia.org/wiki/Significand" />
		/// <see cref="Mantissa" />
		public readonly BigInteger Significand;

		public Boolean IsEven => this.Significand.IsEven;

		//public BigDecimal(String value) {
		//    var number = value.ToBigDecimal();
		//    this.Significand = number.Significand;
		//    this.Exponent = number.Exponent;
		//}
		public Boolean IsOne => this.Significand.IsOne;

		public Boolean IsPowerOfTwo => this.Significand.IsPowerOfTwo;

		public Boolean IsZero => this.Significand.IsZero;

		/// <summary>
		///     The significand (aka mantissa) is part of a number consisting of its significant digits.
		/// </summary>
		/// <see cref="Significand" />
		public BigInteger Mantissa => this.Significand;

		public Int32 Sign => this.Significand.Sign;

		public BigDecimal( BigDecimal bigDecimal ) : this( bigDecimal.Mantissa, bigDecimal.Exponent ) { }

		public BigDecimal( Decimal value ) : this( bigDecimal: value ) { }

		public BigDecimal( Double value ) : this( bigDecimal: value ) { }

		public BigDecimal( Single value ) : this( bigDecimal: value ) { }

		/// <summary>
		/// </summary>
		/// <param name="significand"></param>
		/// <param name="exponent">   </param>
		public BigDecimal( BigInteger significand, Int32 exponent ) {
			this.Significand = significand;
			this.Exponent = exponent;

			//BUG is this correct?
			//TODO is this correct?

			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			while ( exponent > 0 && this.Significand % 10 == 0 ) {

				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
				if ( this.Significand == 0 ) { break; }

				this.Significand /= 10;
				this.Exponent += 1;
			}
		}

		public BigDecimal( Int32 value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( Int64 value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( UInt32 value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( UInt64 value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( [NotNull] Byte[] value ) {
			if ( value.Length < 5 ) { throw new ArgumentOutOfRangeException( nameof( value ), "Not enough bytes to construct the Significand" ); }

			if ( !value.Length.CanAllocateMemory() ) { throw new ArgumentOutOfRangeException( nameof( value ), "'value' is too large to allocate" ); }

			var number = new Byte[ value.Length - 4 ];
			var flags = new Byte[ 4 ];

			Buffer.BlockCopy( value, 0, number, 0, number.Length );
			Buffer.BlockCopy( value, value.Length - 4, flags, 0, 4 );

			this.Significand = new BigInteger( value );
			this.Exponent = BitConverter.ToInt32( flags, 0 );
		}

		private static BigDecimal Add( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ?
				new BigDecimal( AlignExponent( left, right ) + right.Mantissa, exponent: right.Exponent ) :
				new BigDecimal( AlignExponent( right, left ) + left.Mantissa, exponent: left.Exponent );

		/// <summary>
		///     Returns the mantissa of <paramref name="value" />, aligned to the exponent of reference. Assumes the exponent of
		///     <paramref name="value" /> is larger than of value.
		/// </summary>
		public static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) {
			Assert.GreaterOrEqual( value.Exponent, reference.Exponent );

			return value.Mantissa * BigInteger.Pow( 10, exponent: value.Exponent - reference.Exponent );
		}

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
		[NotNull]
		[Pure]
		public static Byte[] DecimalToByteArray( Decimal d ) {
			var bytes = new Byte[ 16 ];

			var bits = Decimal.GetBits( d );
			var lo = bits[ 0 ];
			var mid = bits[ 1 ];
			var hi = bits[ 2 ];
			var flags = bits[ 3 ];

			bytes[ 0 ] = ( Byte ) lo;
			bytes[ 1 ] = ( Byte ) ( lo >> 8 );
			bytes[ 2 ] = ( Byte ) ( lo >> 16 );
			bytes[ 3 ] = ( Byte ) ( lo >> 24 );

			bytes[ 4 ] = ( Byte ) mid;
			bytes[ 5 ] = ( Byte ) ( mid >> 8 );
			bytes[ 6 ] = ( Byte ) ( mid >> 16 );
			bytes[ 7 ] = ( Byte ) ( mid >> 24 );

			bytes[ 8 ] = ( Byte ) hi;
			bytes[ 9 ] = ( Byte ) ( hi >> 8 );
			bytes[ 10 ] = ( Byte ) ( hi >> 16 );
			bytes[ 11 ] = ( Byte ) ( hi >> 24 );

			bytes[ 12 ] = ( Byte ) flags;
			bytes[ 13 ] = ( Byte ) ( flags >> 8 );
			bytes[ 14 ] = ( Byte ) ( flags >> 16 );
			bytes[ 15 ] = ( Byte ) ( flags >> 24 );

			return bytes;
		}

		/// <summary>
		///     <see cref="Divide" /> is soooo broken.
		/// </summary>
		/// <param name="dividend"></param>
		/// <param name="divisor"> </param>
		/// <returns></returns>
		public static BigDecimal Divide( BigDecimal dividend, BigDecimal divisor ) {
			var dendp = BigInteger.Pow( 10, dividend.Exponent );
			var dend = new BigRational( numerator: dividend.Mantissa, denominator: -dendp );

			//var bob = BigRational.Divide( dend, divisor );

			var ratio = dividend.Mantissa.NumberOfDigits() + divisor.Mantissa.NumberOfDigits();

			var power = BigInteger.Pow( 10, ratio );

			var templeft = dividend.Mantissa * power;

			//var tempright = divisor.Mantissa * power;

			var tempmantissa = templeft / divisor.Mantissa;

			//tempmantissa /= power;

			//var realexponent = dividend.Exponent - divisor.Exponent - (int)power;

			var result = new BigDecimal( tempmantissa, -ratio );

			return result;
		}

		///// <summary>
		///// </summary>
		///// <param name="mantissa"></param>
		///// <param name="exponent"></param>
		//public BigDecimal( BigInteger mantissa, Int32 exponent )
		//    : this() {
		//    this.Significand = mantissa;
		//    this.Exponent = exponent;
		//public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) {
		//    var exponentChange = Precision - ( NumberOfDigits( dividend.Mantissa ) - NumberOfDigits( divisor.Mantissa ) );
		//    if ( exponentChange < 0 ) {
		//        exponentChange = 0;
		//    }
		//    dividend.Mantissa *= BigInteger.Pow( 10, exponentChange );
		//    return new BigDecimal( dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange );
		//}
		/// <summary>
		///     Static equality check for <paramref name="left" /> against <paramref name="right" />.
		/// </summary>
		/// <param name="left"> </param>
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

		public static explicit operator Byte( BigDecimal value ) => value.ToType<Byte>();

		public static explicit operator Decimal( BigDecimal value ) => ( Decimal ) value.Mantissa * ( Decimal ) Math.Pow( 10, value.Exponent );

		public static explicit operator Double( BigDecimal value ) => ( Double ) value.Mantissa * Math.Pow( 10, value.Exponent );

		public static explicit operator Int16( BigDecimal value ) => value.ToType<Int16>();

		public static explicit operator Int32( BigDecimal value ) => value.ToType<Int32>();

		// number /= multiplier;
		public static explicit operator Int64( BigDecimal value ) => value.ToType<Int64>();

		// number = leftOfDecimalPoint;
		public static explicit operator SByte( BigDecimal value ) => value.ToType<SByte>();

		public static explicit operator Single( BigDecimal value ) => Convert.ToSingle( ( Double ) value );

		public static explicit operator UInt16( BigDecimal value ) => value.ToType<UInt16>();

		public static explicit operator UInt32( BigDecimal value ) => value.ToType<UInt32>();

		// if ( needToPadFractionSide ) { var zeros = new String( '0', fractionSideLength - fractionLength ); var bside = leftOfDecimalPoint.ToString(); bside = bside.Insert( wholeSideLength - 1, zeros );
		// leftOfDecimalPoint = BigInteger.Parse( bside, NumberStyles.AllowDecimalPoint ); }
		public static explicit operator UInt64( BigDecimal value ) => value.ToType<UInt64>();

		public static implicit operator BigDecimal( Int64 number ) => new BigDecimal( number, 0 );

		///// <summary>TODO this needs unit tested.</summary>
		///// <param name="value"></param>
		///// <returns></returns>
		//public static explicit operator BigInteger(BigDecimal value) => value.ToBigInteger();
		/// <summary>
		///     Do not know if casting and math here is correct (bug free and overflow free)
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static implicit operator BigDecimal( Double number ) {
			var mantissa = new BigInteger( number );
			var exponent = 0;
			Double scaleFactor = 1;

			while ( Math.Abs( number * scaleFactor - ( Double ) mantissa ) > 0 ) {
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = ( BigInteger ) ( number * scaleFactor );
			}

			return new BigDecimal( mantissa, exponent );
		}

		/// <summary>
		///     Don't know if casting and math here is correct (bug free and overflow free)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static implicit operator BigDecimal( Decimal value ) {
			var mantissa = ( BigInteger ) value;
			var exponent = 0;
			Decimal scaleFactor = 1;

			while ( ( Decimal ) mantissa != value * scaleFactor ) {
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = new BigInteger( value * scaleFactor );
			}

			return new BigDecimal( mantissa, exponent );
		}

		// leftOfDecimalPoint *= multiplier; //append a whole lot of zeroes "1000000000" leftOfDecimalPoint += fractionInteger; //reconstruct the part that was after the Decimal point "123456789" // so now it looks like "1123456789"
		public static implicit operator BigDecimal( Byte value ) => new BigDecimal( value );

		// var multiplier = BigInteger.Pow( 10, fractionLength ); //we want the ratio of top/bottom to scale up past the Decimal point and back down later
		public static implicit operator BigDecimal( SByte value ) => new BigDecimal( value );

		// var fractionLength = fractionInteger.ToString().Length;
		public static implicit operator BigDecimal( Int16 value ) => new BigDecimal( value );

		// if ( !BigInteger.TryParse( fractionSide, out fractionInteger ) ) { //we were unable to parse the second String (all to the right of the Decimal point) return false; }
		public static implicit operator BigDecimal( Int32 value ) => new BigDecimal( value );

		// if ( needToPadFractionSide ) { //fractionSide = '1' + fractionSide; //fake out BigInteger by replacing the leading zero with a 1 } //BUG if the String split[1] had a bunch of leading zeros, they are getting
		// trimmed out here. //TODO do some sort of multiplier. Or add a 1.0 in front, with a multiplier. then take off that after the recombine? //but it messes with the ratio
		public static implicit operator BigDecimal( UInt16 value ) => new BigDecimal( value );

		// var needToPadFractionSide = fractionSide[ 0 ] == '0';
		public static implicit operator BigDecimal( UInt32 value ) => new BigDecimal( value );

		// BigInteger fractionInteger;
		public static implicit operator BigDecimal( UInt64 value ) => new BigDecimal( value );

		// var fractionSide = split[ 1 ]; var fractionSideLength = fractionSide.Length; if ( !fractionSideLength.CanAllocateMemory() ) { return false; }
		public static implicit operator BigDecimal( Single value ) => new BigDecimal( value );

		// var wholeSideLength = wholeSide.Length; if ( !wholeSideLength.CanAllocateMemory() ) { return false; } BigInteger leftOfDecimalPoint; if ( !BigInteger.TryParse( wholeSide, out leftOfDecimalPoint ) ) { //we were
		// unable to parse the first String (all to the left of the Decimal point) return false; }
		public static implicit operator BigDecimal( BigInteger value ) => new BigDecimal( value, 0 );

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

		public static BigDecimal Pow( Double basis, Double exponent ) {
			BigDecimal tmp = 1;

			while ( Math.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Pow( basis, diff );
				exponent -= diff;
			}

			return tmp * Math.Pow( basis, exponent );
		}

		public Int32 CompareTo( [CanBeNull] Object obj ) {
			if ( !( obj is BigDecimal ) ) { throw new ArgumentException(); }

			return this.CompareTo( ( BigDecimal ) obj );
		}

		public Int32 CompareTo( BigDecimal other ) {
			if ( this < other ) { return -1; }

			return this > other ? 1 : 0;
		}

		public Boolean Equals( BigDecimal other ) => Equals( this, other );

		[Pure]
		public override Boolean Equals( [CanBeNull] Object obj ) {
			if ( obj is null ) { return false; }

			return obj is BigDecimal @decimal && Equals( this, @decimal );
		}

		///// <summary>
		/////     <para>Create a BigDecimal from a String representation.</para>
		///// </summary>
		///// <param name="value"></param>
		///// <param name="answer"></param>
		///// <returns></returns>
		//public static Boolean TryParse([CanBeNull] String value, out BigDecimal? answer) {
		//    answer = null;
		//    if ( String.IsNullOrWhiteSpace( value ) ) {
		//        return false;
		//    }
		//    try {
		//        answer = value.ToBigDecimal();
		//        return true;
		//    }
		//    catch ( FormatException ) { }
		//    catch ( ArithmeticException ) { }
		//    return false;
		//}
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

		///// <summary>
		/////     <para>Create a BigDecimal from a String representation.</para>
		///// </summary>
		///// <param name="value"></param>
		///// <returns></returns>
		//public static BigDecimal Parse([NotNull] String value) {
		//    if ( value is null ) {
		//        throw new ArgumentNullException( nameof( value ) );
		//    }
		//    return value.ToBigDecimal();
		//}
		//public static explicit operator Int32( BigDecimal value ) {
		//    return ( Int32 )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
		//}
		[NotNull]
		public Byte[] ToByteArray() {

			var unscaledValue = this.Significand.ToByteArray();
			var scale = BitConverter.GetBytes( this.Exponent );

			if ( !( unscaledValue.Length + scale.Length ).CanAllocateMemory() ) { throw new OutOfMemoryException( "ToByteArray() is too large to allocate" ); }

			var bytes = new Byte[ unscaledValue.Length + scale.Length ];
			Buffer.BlockCopy( unscaledValue, 0, bytes, 0, unscaledValue.Length );
			Buffer.BlockCopy( scale, 0, bytes, unscaledValue.Length, scale.Length );

			return bytes;
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

			if ( this.Sign == -1 ) { result = result.Prepend( "-" ); }

			return result;

			//if ( this.Exponent < 0 ) {
			//    result = result.Insert( 0, leadingZeros );

			//}
			//else if ( this.Exponent > 0 ) {
			//    var at = result.Length + this.Exponent;

			// var padLeft = at == 0;

			// result = result.Insert( at, "." );

			//    if ( padLeft ) {
			//        result = result.Insert( at, "0" );
			//    }
			//}
		}

		//public String ToString( String format, IFormatProvider formatProvider ) {
		//    throw new NotImplementedException();
		//}
		[NotNull]
		public String ToStringWithE() => String.Concat( this.Mantissa.ToString(), "E", this.Exponent );

		//public static explicit operator uint( BigDecimal value ) {
		//    return ( uint )( value.Mantissa * BigInteger.Pow( 10, value.Exponent ) );
		//}
		//    return ( Decimal )value.Mantissa * ( Decimal )Math.Pow( 10, value.Exponent );
		//}
		//public String ToScientificString() => MathExtensions.ToScientificString( this );
		public T ToType<T>() where T : struct => ( T ) ( ( IConvertible ) this ).ToType( typeof( T ), null );

		TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

		Boolean IConvertible.ToBoolean( IFormatProvider provider ) => Convert.ToBoolean( this );

		Byte IConvertible.ToByte( IFormatProvider provider ) => Convert.ToByte( this );

		Char IConvertible.ToChar( IFormatProvider provider ) => throw new InvalidCastException( "Cannot cast BigDecimal to Char" );

		DateTime IConvertible.ToDateTime( IFormatProvider provider ) => throw new InvalidCastException( "Cannot cast BigDecimal to DateTime" );

		Decimal IConvertible.ToDecimal( IFormatProvider provider ) => Convert.ToDecimal( this );

		Double IConvertible.ToDouble( IFormatProvider provider ) => Convert.ToDouble( this );

		Int16 IConvertible.ToInt16( IFormatProvider provider ) => Convert.ToInt16( this );

		Int32 IConvertible.ToInt32( IFormatProvider provider ) => Convert.ToInt32( this );

		Int64 IConvertible.ToInt64( IFormatProvider provider ) => Convert.ToInt64( this );

		SByte IConvertible.ToSByte( IFormatProvider provider ) => Convert.ToSByte( this );

		Single IConvertible.ToSingle( IFormatProvider provider ) => Convert.ToSingle( this );

		String IConvertible.ToString( IFormatProvider provider ) => Convert.ToString( this, CultureInfo.CurrentCulture );

		Object IConvertible.ToType( Type conversionType, IFormatProvider provider ) {
			var scaleDivisor = BigInteger.Pow( new BigInteger( 10 ), this.Exponent );
			var remainder = BigInteger.Remainder( this.Significand, scaleDivisor );
			var scaledValue = BigInteger.Divide( this.Significand, scaleDivisor );

			if ( scaledValue > new BigInteger( Decimal.MaxValue ) ) { throw new ArgumentOutOfRangeException( nameof( provider ), $"The value {this.Significand} cannot fit into {conversionType.Name}." ); }

			var leftOfDecimal = ( Decimal ) scaledValue;
			var rightOfDecimal = ( Decimal ) remainder / ( Decimal ) scaleDivisor;

			var value = leftOfDecimal + rightOfDecimal;

			return Convert.ChangeType( value, conversionType ) ?? throw new InvalidOperationException();
		}

		UInt16 IConvertible.ToUInt16( IFormatProvider provider ) => Convert.ToUInt16( this );

		UInt32 IConvertible.ToUInt32( IFormatProvider provider ) => Convert.ToUInt32( this );

		UInt64 IConvertible.ToUInt64( IFormatProvider provider ) => Convert.ToUInt64( this );

		//public static explicit operator BigInteger( BigDecimal value ) {
		//    var man = (BigDecimal)value.Mantissa;
		//    new BigDecimal(
		//    man *= BigDecimal.Pow( 10, value.Exponent );
		/*
                private static Boolean CheckExponent( long candidate, Boolean isZero, out int exponent ) {
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
                private static int CheckExponent( long candidate, Boolean isZero ) {
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
                /// Parse a substring of a character array as a BigDecimal.
                /// </summary>
                /// <param name="buf">         The character array to parse</param>
                /// <param name="offset">      Start index for parsing</param>
                /// <param name="len">         Number of chars to parse.</param>
                /// <param name="throwOnError">If true, an error causes an exception to be thrown. If false, false is returned.</param>
                /// <param name="v">           The BigDecimal corresponding to the characters.</param>
                /// <returns>True if successful, false if not (or throws if throwOnError is true).</returns>
                /// <remarks>
                /// Ugly. We could use a RegEx, but trying to avoid unnecessary allocation, I guess. [+-]?\d*(\.\d*)?([Ee][+-]?\d+)? with additional constraint that one of the two d* must have at least one char.
                /// </remarks>
                private static Boolean DoParse( char[] buf, int offset, int len, Boolean throwOnError, out BigDecimal v ) {
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
                    Buffer.BlockCopy( buf, sourceIndex1, chArray1, 0, num1 );
                    if ( length1 > 0 ) {
                        Buffer.BlockCopy( buf, sourceIndex2, chArray1, num1, length1 );
                    }
                    var coeff = BigInteger.Parse( new String( chArray1 ) );
                    var result = 0;
                    if ( length2 > 0 ) {
                        var chArray2 = new char[ length2 ];
                        Buffer.BlockCopy( buf, sourceIndex3, chArray2, 0, length2 );
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

		// //BigDecimal bob = Parse( value ); //number = bob;

		// clojure.lang.BigDecimal bigDecimal; if ( clojure.lang.BigDecimal.TryParse( value, out bigDecimal ) ) { number = new BigDecimal( bigDecimal.Coefficient, bigDecimal.Exponent ); return true; }

		// // all whitespace or none? if ( String.IsNullOrWhiteSpace( value ) ) { whyParseFailed = "'value' is null or contained only whitespace"; return false; }

		// value = value.Trim(); if ( String.IsNullOrWhiteSpace( value ) ) { whyParseFailed = "'value' is null or contained only whitespace"; return false; }

		// if ( value.Contains( "E" ) ) { whyParseFailed = "not implemented yet"; //TODO add in subset for parsing numbers like "3.14E15" (scientific notation?) throw new NotImplementedException(); return false; }

		// if ( value.Contains( "^" ) ) { whyParseFailed = "not implemented yet"; //TODO add in subset for parsing numbers like "3.14^15"? (exponential notation?)

		// //TODO add in subset for parsing numbers like "3.14X10^15"? (exponential notation?) throw new NotImplementedException(); return false; }

		// if ( !value.Contains( "." ) ) { value += ".0"; //for parsing large decimals }

		// if ( value.Count( '.' ) > 1 ) { whyParseFailed = "'value' contained too many Decimal places"; return false; }

		// if ( value.Count( '-' ) > 1 ) { whyParseFailed = "'value' contained too many minus signs"; return false; }

		// if ( value.Any( c => !Char.IsDigit( c ) && c != '.' && c != '-' ) ) { whyParseFailed = "all chars must be a digit, a period, or a negative sign"; return false; }

		// var split = value.Split( '.' ); split.Should().HaveCount( expected: 2, because: "otherwise invalid" ); if ( split.Length != 2 ) { whyParseFailed = ""; return false; } var wholeSide = split[ 0 ];
	}
}