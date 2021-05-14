// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "BigDecimal.cs" last touched on 2021-05-14 at 7:58 AM by Protiguous.

#nullable enable

namespace Librainian.Maths.Bigger {

	using System;
	using System.Globalization;
	using System.Linq;
	using System.Numerics;

	/// <summary>
	///     Arbitrary precision decimal.
	///     All operations are exact, except for division. Division never determines more digits than the given precision.
	///     Based on code by Jan Christoph Bernack (http://stackoverflow.com/a/4524254 or jc.bernack at googlemail.com)
	///     Modified and extended by Adam White https://csharpcodewhisperer.blogspot.com
	/// </summary>
	public record BigDecimal : IComparable, IComparable<BigDecimal>, ICloneable<BigDecimal> {

		private const String NullString = "(␀)";

		private const String NumericCharacters = "-0.1234567890";

		public BigDecimal( Decimal value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( Int32 value ) : this( new BigInteger( value ), 0 ) { }

		public BigDecimal( Int64 value ) : this( ( BigInteger )value ) { }

		public BigDecimal( UInt64 value ) : this( ( BigInteger )value ) { }

		public BigDecimal( BigInteger mantissa, Int32 exponent = 0 ) {
			this.Mantissa = mantissa; //TODO Why was .Clone() tacked on here?
			this.Exponent = exponent;
		}

		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="OverflowException"></exception>
		/// <exception cref="NotFiniteNumberException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FormatException"></exception>
		public BigDecimal( Double value ) {
			if ( Double.IsInfinity( value ) ) {
				throw new OverflowException( "BigDecimal cannot represent infinity." );
			}

			if ( Double.IsNaN( value ) ) {
				throw new NotFiniteNumberException( $"{nameof( value )} is not a number (double.NaN)." );
			}

			var mantissa = new BigInteger( value );
			var exponent = 0;
			Double scaleFactor = 1;
			while ( Math.Abs( value * scaleFactor - Double.Parse( mantissa.ToString() ) ) > 0 ) {
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = new BigInteger( value * scaleFactor );
			}

			this.Mantissa = mantissa;
			this.Exponent = exponent;
		}

		private static NumberFormatInfo BigDecimalNumberFormatInfo => CultureInfo.InvariantCulture.NumberFormat;

		private static BigInteger TenInt => new(10);

		public static BigDecimal One => new(1);

		public static BigDecimal OneHalf => 0.5d;

		public static BigDecimal Pi =>
			new(BigInteger.Parse(
					"314159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196" )
				, 1);

		public static BigDecimal Ten => new(10, 0);

		public static BigDecimal Zero => new(0);

		public Int32 DecimalPlaces => this.SignifigantDigits + this.Exponent;

		public Int32 Exponent { get; }

		/// <summary>
		///     This method returns true if the BigDecimal is less than zero, false otherwise.
		/// </summary>
		public Boolean IsNegative => this.Mantissa.Sign < 0;

		/// <summary>
		///     This method returns true if the BigDecimal is greater than zero, false otherwise.
		/// </summary>
		public Boolean IsPositve => !this.IsZero && !this.IsNegative;

		public Int32 Length => GetSignifigantDigits( this.Mantissa ) + this.Exponent;

		public BigInteger Mantissa { get; }

		public Int32 SignifigantDigits => GetSignifigantDigits( this.Mantissa );

		public BigInteger WholeValue => this.GetWholePart();

		public static BigDecimal E =>
			new(BigInteger.Parse(
					"271828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642749193200305992181741359662904357290033429526059563073813232862794349076323382988075319525101901157383" )
				, 1);

		public static BigDecimal MinusOne => new(BigInteger.MinusOne, 0);

		/// <summary>
		///     This method returns true if the BigDecimal is equal to zero, false otherwise.
		/// </summary>
		public Boolean IsZero => this.Mantissa.IsZero;

		public Int32 Sign => this.GetSign();

		public BigDecimal Duplicate() => new(this.Mantissa, this.Exponent);

		/// <summary>
		///     Compares two BigDecimal values, returning an integer that indicates their relationship.
		/// </summary>
		public Int32 CompareTo( Object? obj ) => obj is BigDecimal @decimal ? this.CompareTo( @decimal ) : throw new ArgumentNullException( nameof( obj ) );

		/// <summary>
		///     Compares two BigDecimal values, returning an integer that indicates their relationship.
		/// </summary>
		public Int32 CompareTo( BigDecimal? other ) =>
			this < other ? -1 :
			this > other ? 1 : 0;

		/// <summary>
		///     Returns the mantissa of value, aligned to the exponent of reference. Assumes the exponent of value is larger than
		///     of reference.
		/// </summary>
		private static BigInteger AlignExponent( BigDecimal value, BigDecimal reference ) => value.Mantissa * BigInteger.Pow( TenInt, value.Exponent - reference.Exponent );

		private static Int32 GetDecimalIndex( BigInteger mantissa, Int32 exponent ) {
			var mantissaLength = mantissa.GetLength();
			if ( mantissa.Sign < 0 ) {
				mantissaLength += 1;
			}

			return mantissaLength + exponent;
		}

		private static Int32 GetSignifigantDigits( BigInteger value ) {
			if ( value.IsZero ) {
				return 0;
			}

			var valueString = value.ToString();
			if ( String.IsNullOrWhiteSpace( valueString ) ) {
				return 0;
			}

			valueString = new String( valueString.Trim().Where( c => NumericCharacters.Contains( c ) ).ToArray() );
			valueString = valueString.Replace( BigDecimalNumberFormatInfo.NegativeSign, String.Empty );
			valueString = valueString.Replace( BigDecimalNumberFormatInfo.PositiveSign, String.Empty );
			valueString = valueString.TrimEnd( '0' );
			valueString = valueString.Replace( BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty );

			return valueString.Length;
		}

		private static String ToString( BigInteger mantissa, Int32 exponent, IFormatProvider provider ) {
			if ( provider == null ) {
				throw new ArgumentNullException( nameof( provider ) );
			}

			var formatProvider = NumberFormatInfo.GetInstance( provider );

			var negativeValue = mantissa.Sign == -1;
			var negativeExponent = Math.Sign( exponent ) == -1;

			var result = BigInteger.Abs( mantissa ).ToString();
			var absExp = Math.Abs( exponent );

			if ( negativeExponent ) {
				if ( absExp > result.Length ) {
					var zerosToAdd = Math.Abs( absExp - result.Length );
					var zeroString = String.Join( String.Empty, Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], zerosToAdd ) );
					result = zeroString + result;
					result = result.Insert( 0, formatProvider.NumberDecimalSeparator );
					result = result.Insert( 0, formatProvider.NativeDigits[ 0 ] ?? throw new InvalidOperationException() );
				}
				else {
					var indexOfRadixPoint = Math.Abs( absExp - result.Length );
					result = result.Insert( indexOfRadixPoint, formatProvider.NumberDecimalSeparator );
					if ( indexOfRadixPoint == 0 ) {
						result = result.Insert( 0, formatProvider.NativeDigits[ 0 ] ?? throw new InvalidOperationException() );
					}
				}

				result = result.TrimEnd( '0' );
				if ( result.Last().ToString() == formatProvider.NumberDecimalSeparator ) {
					result = result.Substring( 0, result.Length - 1 );
				}
			}
			else {
				var zeroString = String.Join( String.Empty, Enumerable.Repeat( formatProvider.NativeDigits[ 0 ], absExp ) );
				result += zeroString;
			}

			if ( negativeExponent ) // Prefix "0."
			{ }

			if ( negativeValue ) // Prefix "-"
			{
				result = result.Insert( 0, formatProvider.NegativeSign );
			}

			return result;
		}

		private Int32 GetSign() {
			if ( this.Mantissa.IsZero ) {
				return 0;
			}

			if ( this.Mantissa.Sign != -1 ) {
				return 1;
			}

			if ( this.Exponent >= 0 ) {
				return -1;
			}

			var mantissa = this.Mantissa.ToString();
			var length = mantissa.Length + this.Exponent;
			if ( length == 0 ) {
				return Int32.TryParse( mantissa[ 0 ].ToString(), out var tenthsPlace ) ? tenthsPlace < 5 ? 0 :
					1 : throw new InvalidOperationException( "Error parsing mantissa." );
			}

			return length > 0 ? 1 : 0;
		}

		/// <summary>
		///     Returns the absolute value of the BigDecimal
		/// </summary>
		public static BigDecimal Abs( BigDecimal value ) => value.IsNegative ? value * -1 : value;

		/// <summary>
		///     Adds two BigDecimal values.
		/// </summary>
		public static BigDecimal Add( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? new BigDecimal( AlignExponent( left, right ) + right.Mantissa, right.Exponent ) :
				new BigDecimal( AlignExponent( right, left ) + left.Mantissa, left.Exponent );

		/// <summary>
		///     Rounds a BigDecimal to an integer value. The BigDecimal argument is rounded towards positive infinity.
		/// </summary>
		public static BigDecimal Ceiling( BigDecimal value ) {
			BigDecimal result = value.WholeValue;

			if ( result != value.Mantissa && value >= 0 ) {
				result += 1;
			}

			return result;
		}

		/// <summary>
		///     Divides two BigDecimal values.
		/// </summary>
		public static BigDecimal Divide( BigDecimal dividend, BigDecimal divisor ) {
			if ( divisor.Equals( Zero ) ) {
				throw new DivideByZeroException();
			}

			if ( Abs( dividend ).Equals( 1 ) ) {
				var doubleDivisor = Double.Parse( divisor.ToString() );
				doubleDivisor = 1d / doubleDivisor;

				return Parse( doubleDivisor.ToString( CultureInfo.InvariantCulture ) );
			}

			//string remString = "";
			//string mantissaString = "";
			//string dividendMantissaString = dividend.Mantissa.ToString();
			//string divisorMantissaString = divisor.Mantissa.ToString();

			//var dividendMantissaLength = dividend.DecimalPlaces;
			//var divisorMantissaLength = divisor.DecimalPlaces;
			var exponentChange = dividend.Exponent - divisor.Exponent; //(dividendMantissaLength - divisorMantissaLength);

			var counter = 0;

			BigDecimal result = BigInteger.DivRem( dividend.Mantissa, divisor.Mantissa, out var remainder );
			while ( remainder != 0 && result.SignifigantDigits < divisor.SignifigantDigits ) {
				while ( BigInteger.Abs( remainder ) < BigInteger.Abs( divisor.Mantissa ) ) {
					remainder *= 10;
					result = new BigDecimal( result.Mantissa * 10, result.Exponent );
					counter++;
				}

				result = new BigDecimal( result.Mantissa + BigInteger.DivRem( remainder, divisor.Mantissa, out remainder ), result.Exponent );
			}

			result = new BigDecimal( result.Mantissa, exponentChange - counter );
			return result;
		}

		/// <summary>
		///     Returns e raised to the specified power
		/// </summary>
		public static BigDecimal Exp( Double exponent ) {
			var tmp = One;
			while ( Math.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Exp( diff );
				exponent -= diff;
			}

			return tmp * Math.Exp( exponent );
		}

		/// <summary>
		///     Returns e raised to the specified power
		/// </summary>
		public static BigDecimal Exp( BigInteger exponent ) {
			var tmp = ( BigDecimal )1;
			while ( BigInteger.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Exp( diff );
				exponent -= diff;
			}

			var exp = ( Double )exponent;
			return tmp * Math.Exp( exp );
		}

		public static explicit operator BigInteger( BigDecimal v ) {
			if ( v.Exponent >= 0 ) {
				return BigInteger.Multiply( v.Mantissa, BigInteger.Pow( TenInt, v.Exponent ) );
			}

			var mant = v.Mantissa.ToString();

			var length = v.GetDecimalIndex();
			if ( length > 0 ) {
				return BigInteger.Parse( mant.Substring( 0, length ) );
			}

			if ( length == 0 ) {
				var tenthsPlace = Int32.Parse( mant[ 0 ].ToString() );
				return tenthsPlace >= 5 ? new BigInteger( 1 ) : new BigInteger( 0 );
			}

			return BigInteger.Zero;
		}

		public static explicit operator Decimal( BigDecimal value ) {
			if ( !Decimal.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToDecimal( value.Mantissa.ToString() );
			}

			return mantissa * ( Decimal )Math.Pow( 10, value.Exponent );
		}

		public static explicit operator Double( BigDecimal value ) {
			if ( !Double.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToDouble( value.Mantissa.ToString() );
			}

			return mantissa * Math.Pow( 10, value.Exponent );
		}

		public static explicit operator Int32( BigDecimal value ) {
			if ( !Int32.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToInt32( value.Mantissa.ToString() );
			}

			return mantissa * ( Int32 )BigInteger.Pow( TenInt, value.Exponent );
		}

		public static explicit operator Single( BigDecimal value ) {
			if ( !Single.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToSingle( value.Mantissa.ToString() );
			}

			return mantissa * ( Single )Math.Pow( 10, value.Exponent );
		}

		public static explicit operator UInt32( BigDecimal value ) {
			if ( !UInt32.TryParse( value.Mantissa.ToString(), out var mantissa ) ) {
				mantissa = Convert.ToUInt32( value.Mantissa.ToString() );
			}

			return mantissa * ( UInt32 )BigInteger.Pow( TenInt, value.Exponent );
		}

		/// <summary>
		/// </summary>
		public static BigDecimal Floor( BigDecimal value ) {
			BigDecimal result = value.WholeValue;

			if ( result.Equals( value.Mantissa ) || value > 0 ) {
				return result;
			}

			result -= 1;

			return result;
		}

		public static implicit operator BigDecimal( Byte value ) => new(value);

		public static implicit operator BigDecimal( SByte value ) => new(value);

		public static implicit operator BigDecimal( Int16 value ) => new(value);

		public static implicit operator BigDecimal( Int32 value ) => new(value);

		public static implicit operator BigDecimal( Int64 value ) => new(value);

		public static implicit operator BigDecimal( UInt16 value ) => new(value);

		public static implicit operator BigDecimal( UInt32 value ) => new(value);

		public static implicit operator BigDecimal( UInt64 value ) => new(value);

		public static implicit operator BigDecimal( BigInteger value ) => new(value, 0);

		public static implicit operator BigDecimal( Single value ) => new(value);

		public static implicit operator BigDecimal( Double value ) => new(value);

		public static implicit operator BigDecimal( Decimal value ) {
			var mantissa = new BigInteger( value );
			var exponent = 0;
			Decimal scaleFactor = 1;
			while ( Decimal.Parse( mantissa.ToString() ) != value * scaleFactor ) {
				exponent -= 1;
				scaleFactor *= 10;
				mantissa = new BigInteger( value * scaleFactor );
			}

			return new BigDecimal( mantissa, exponent );
		}

		/// <summary>
		///     Divides two BigDecimal values, returning the remainder and discarding the quotient.
		/// </summary>
		public static BigDecimal Mod( BigDecimal value, BigDecimal mod ) {
			// x – q * y
			var quotient = Divide( value, mod );
			var floor = Floor( quotient );
			return Subtract( value, Multiply( floor, mod ) );
		}

		/// <summary>
		///     Multiplies two BigDecimal values.
		/// </summary>
		public static BigDecimal Multiply( BigDecimal left, BigDecimal right ) => new(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

		/// <summary>
		///     Returns the result of multiplying a BigDecimal by negative one.
		/// </summary>
		public static BigDecimal Negate( BigDecimal value ) => new(BigInteger.Negate( value.Mantissa ), value.Exponent);

		public static BigDecimal operator -( BigDecimal value ) => Negate( value );

		public static BigDecimal operator -( BigDecimal left, BigDecimal right ) => Subtract( left, right );

		public static BigDecimal operator --( BigDecimal value ) => Subtract( value, 1 );

		//public static Boolean operator !=( BigDecimal left, BigDecimal right ) => left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;

		public static BigDecimal operator *( BigDecimal left, BigDecimal right ) => Multiply( left, right );

		public static BigDecimal operator /( BigDecimal dividend, BigDecimal divisor ) => Divide( dividend, divisor );

		public static BigDecimal operator +( BigDecimal value ) => value;

		public static BigDecimal operator +( BigDecimal left, BigDecimal right ) => Add( left, right );

		public static BigDecimal operator ++( BigDecimal value ) => Add( value, 1 );

		public static Boolean operator <( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) < right.Mantissa : left.Mantissa < AlignExponent( right, left );

		public static Boolean operator <=( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) <= right.Mantissa : left.Mantissa <= AlignExponent( right, left );

		//public static Boolean operator ==( BigDecimal left, BigDecimal right ) => left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;

		public static Boolean operator >( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) > right.Mantissa : left.Mantissa > AlignExponent( right, left );

		public static Boolean operator >=( BigDecimal left, BigDecimal right ) =>
			left.Exponent > right.Exponent ? AlignExponent( left, right ) >= right.Mantissa : left.Mantissa >= AlignExponent( right, left );

		/// <summary>
		///     Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse( Double input ) => Parse( input.ToString( CultureInfo.InvariantCulture ) );

		/// <summary>
		///     Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		public static BigDecimal Parse( Decimal input ) => Parse( input.ToString( CultureInfo.InvariantCulture ) );

		/// <summary>
		///     Converts the string representation of a decimal to the BigDecimal equivalent.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static BigDecimal Parse( String input ) {
			if ( String.IsNullOrWhiteSpace( input ) ) {
				//return default( BigDecimal? );
				return Zero;
			}

			var exponent = 0;
			var isNegative = false;
			var localInput = new String( input.Trim().Where( c => NumericCharacters.Contains( c ) ).ToArray() );

			if ( localInput.StartsWith( BigDecimalNumberFormatInfo.NegativeSign ) ) {
				isNegative = true;
				localInput = localInput.Replace( BigDecimalNumberFormatInfo.NegativeSign, String.Empty );
			}

			if ( localInput.Contains( BigDecimalNumberFormatInfo.NumberDecimalSeparator ) ) {
				var decimalPlace = localInput.IndexOf( BigDecimalNumberFormatInfo.NumberDecimalSeparator, StringComparison.Ordinal );

				exponent = decimalPlace + 1 - localInput.Length;
				localInput = localInput.Replace( BigDecimalNumberFormatInfo.NumberDecimalSeparator, String.Empty );
			}

			var mantessa = BigInteger.Parse( localInput );
			if ( isNegative ) {
				mantessa = BigInteger.Negate( mantessa );
			}

			return new BigDecimal( mantessa, exponent );
		}

		public static BigDecimal Concat( String beforeDecimal, String afterDecimal ) =>
			Parse( $"{beforeDecimal}{BigDecimalNumberFormatInfo.NumberDecimalSeparator}{afterDecimal}" );

		public static Boolean TryParse( String input, out BigDecimal? result ) {
			try {
				result = Parse( input );
				return true;
			}
			catch ( Exception ) {
				result = default( BigDecimal? );
				return false;
			}
		}

		/// <summary>
		///     Returns a specified number raised to the specified power.
		/// </summary>
		public static BigDecimal Pow( BigDecimal baseValue, BigInteger exponent ) {
			if ( exponent.IsZero ) {
				return One;
			}

			if ( exponent.Sign < 0 ) {
				if ( baseValue == Zero ) {
					throw new NotSupportedException( "Cannot raise zero to a negative power" );
				}

				// n^(-e) -> (1/n)^e
				baseValue = One / baseValue;
				exponent = BigInteger.Negate( exponent );
			}

			var result = baseValue;
			while ( exponent > BigInteger.One ) {
				result *= baseValue;
				exponent--;
			}

			return result;
		}

		/// <summary>
		///     Returns a specified number raised to the specified power.
		/// </summary>
		public static BigDecimal Pow( Double basis, Double exponent ) {
			var tmp = ( BigDecimal )1;
			while ( Math.Abs( exponent ) > 100 ) {
				var diff = exponent > 0 ? 100 : -100;
				tmp *= Math.Pow( basis, diff );
				exponent -= diff;
			}

			return tmp * Math.Pow( basis, exponent );
		}

		/// <summary>
		///     Rounds a BigDecimal value to the nearest integral value.
		/// </summary>
		public static BigInteger Round( BigDecimal value ) => Round( value, MidpointRounding.AwayFromZero );

		/// <summary>
		///     Rounds a BigDecimal value to the nearest integral value. A parameter specifies how to round the value if it is
		///     midway between two numbers.
		/// </summary>
		public static BigInteger Round( BigDecimal value, MidpointRounding mode ) {
			var wholePart = value.WholeValue;
			var decimalPart = value.GetFractionalPart();

			BigInteger addOne = value.IsNegative ? -1 : 1;

			if ( decimalPart > OneHalf ) {
				wholePart += addOne;
			}
			else if ( decimalPart == OneHalf ) {
				if ( mode == MidpointRounding.AwayFromZero ) {
					wholePart += addOne;
				}
				else // MidpointRounding.ToEven
				{
					if ( !wholePart.IsEven ) {
						wholePart += addOne;
					}
				}
			}

			return wholePart;
		}

		/// <summary>
		///     Subtracts two BigDecimal values.
		/// </summary>
		public static BigDecimal Subtract( BigDecimal left, BigDecimal right ) => Add( left, Negate( right ) );

		//public override Boolean Equals( Object obj ) => obj is BigDecimal @decimal && this.Equals( @decimal );

		/*
		public Boolean Equals( BigDecimal other ) {

			//this.Normalize();
			//other.Normalize();

			var matchMantissa = this.Mantissa.Equals( other.Mantissa );
			var matchExponent = this.Exponent.Equals( other.Exponent );
			var matchSign = this.Sign.Equals( other.Sign );

			return matchMantissa && matchExponent && matchSign;
		}
		*/

		/// <summary>
		///     Returns the zero-based index of the decimal point, if the BigDecimal were rendered as a string.
		/// </summary>
		public Int32 GetDecimalIndex() => GetDecimalIndex( this.Mantissa, this.Exponent );

		/// <summary>
		///     Gets the fractional part of the BigDecimal, setting everything left of the decimal point to zero.
		/// </summary>
		public BigDecimal GetFractionalPart() {
			if ( this == null ) {
				throw new TypeInitializationException( nameof( BigDecimal ), new NullReferenceException() );
			}

			var decimalString = this.ToString();
			var valueSplit = decimalString.Split( '.' );

			if ( valueSplit.Length == 0 ) {
				return Zero;
			}

			if ( valueSplit.Length == 1 ) {
				return Zero;
			}

			if ( valueSplit.Length == 2 ) {
				var resultString = valueSplit[ 1 ];
				var part = Parse( "0." + resultString );
				return new BigDecimal( part.Mantissa );
			}

			//var newMantessa = BigInteger.Parse( resultString.TrimStart( '0' ) );
			//var result = new BigDecimal( newMantessa, 0 - resultString.Length );
			//return result;
			throw new InvalidOperationException( $"bad logic parsing in {nameof( this.GetFractionalPart )}." );
		}

		public override Int32 GetHashCode() => ( this.Mantissa, this.Exponent ).GetHashCode();

		/// <summary>
		///     Returns the whole number integer part of the BigDecimal, dropping anything right of the decimal point. Essentially
		///     behaves like Math.Truncate(). For example, GetWholePart() would return 3 for Math.PI.
		/// </summary>
		public BigInteger GetWholePart() {
			var resultString = String.Empty;
			var decimalString = ToString( this.Mantissa, this.Exponent, BigDecimalNumberFormatInfo );
			var valueSplit = decimalString.Split( new[] {
				BigDecimalNumberFormatInfo.NumberDecimalSeparator
			}, StringSplitOptions.RemoveEmptyEntries );
			if ( valueSplit.Length > 0 ) {
				resultString = valueSplit[ 0 ];
			}

			return BigInteger.Parse( resultString ?? throw new InvalidOperationException() );
		}

		public override String ToString() => this.ToString( BigDecimalNumberFormatInfo );

		public String ToString( IFormatProvider provider ) => ToString( this.Mantissa, this.Exponent, provider );

	}

	public static class BigDecimalExtensions { }

}