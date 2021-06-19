// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

namespace Librainian.Converters {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Numerics;
	using System.Runtime.CompilerServices;
	using System.Security.Cryptography;
	using System.Text;
	using System.Text.RegularExpressions;
	using Collections.Extensions;
	using Exceptions;
	using Extensions;
	using FileSystem;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Maths.Numbers;
	using Microsoft.Data.SqlClient;
	using Parsing;
	using Security;

	public static class ConverterExtensions {

		/// <summary>
		///     Does nothing. Try <see cref="Cast{TIn,TOut}" /> instead.
		/// </summary>
		/// <param name="anything"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T AsType<T>( this T anything ) => anything;

		/// <summary>
		///     <para>Attempt to convert/cast
		///         <param name="value"> to given type.</param>
		///     </para>
		///     <para>If the value cannot be converted, null is returned for classes and default or structs.</para>
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TOut? Cast<TIn, TOut>( this TIn value ) {
			if ( value is null ) {
				return default( TOut? );
			}

			if ( Convert.IsDBNull( value ) ) {
				return default( TOut? );
			}

			if ( value is TOut cast ) {
				return cast;
			}

			if ( Convert.ChangeType( value, typeof( TOut ) ) is TOut become ) {
				return become;
			}

			return default( TOut? );
		}

		/// <summary>
		///     Converts strings that may contain "$" or "()" to a <see cref="Decimal" /> amount.
		///     <para>Null or empty strings return <see cref="Decimal.Zero" />.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static Decimal MoneyToDecimal<T>( this T? value ) {
			if ( value is null ) {
				return Decimal.Zero;
			}

			var amount = value.ToString();

			if ( String.IsNullOrEmpty( amount ) ) {
				return Decimal.Zero;
			}

			amount = amount.Replace( "$", String.Empty );
			amount = amount.Replace( ")", String.Empty );
			amount = amount.Replace( "(", "-" );
			amount = amount.Replace( "--", "-" );

			try {
				if ( Decimal.TryParse( amount, out var v ) ) {
					return v;
				}
			}
			catch ( FormatException exception ) {
				exception.Log();
			}
			catch ( OverflowException exception ) {
				exception.Log();
			}

			return Decimal.Zero;
		}

		public static Int32? Ordinal( this SqlDataReader bob, String columnName ) {
			try {
				return bob.GetOrdinal( columnName );
			}
			catch ( IndexOutOfRangeException exception ) {
				exception.Log();
			}

			return default( Int32? );
		}

		[DebuggerStepThrough]
		[Pure]
		public static String StripLetters( this String s ) => Regex.Replace( s, "[a-zA-Z]", String.Empty );

		[DebuggerStepThrough]
		[Pure]
		public static BigInteger ToBigInteger( this Guid self ) => new( self.ToByteArray() );

		/// <summary>
		///     <para>Returns true if <paramref name="value" /> is a true, 'Y', "yes", "true", "1", or '1'.</para>
		///     <para>Returns false if <paramref name="value" /> is a false, 'N', "no", "false", or '0'.</para>
		///     <para>A null will return false.</para>
		/// </summary>
		/// <param name="value"></param>
		[Pure]
		public static Boolean ToBoolean<T>( this T? value ) {
			switch ( value ) {
				case null:
					return false;

				case Boolean b:
					return b;

				case Char c:
					return c.In( ParsingConstants.TrueChars );

				case Int32 i:
					return i >= 1;

				case String s when String.IsNullOrWhiteSpace( s ):
					return false;

				case String s: {
					var clean = s.Trimmed();

					if ( clean is null ) {
						return false;
					}

					if ( clean.In( ParsingConstants.TrueStrings ) ) {
						return true;
					}

					if ( Boolean.TryParse( clean, out var result ) ) {
						return result;
					}

					break;
				}
			}

			var t = value.ToString();

			if ( !String.IsNullOrWhiteSpace( t ) ) {
				t = t.Trim();

				if ( t.In( ParsingConstants.TrueStrings ) ) {
					return true;
				}

				if ( t.In( ParsingConstants.FalseStrings ) ) {
					return false;
				}

				if ( Boolean.TryParse( t, out var rest ) ) {
					return rest;
				}
			}

			return false;
		}

		[DebuggerStepThrough]
		[Pure]
		public static Boolean? ToBooleanOrNull<T>( this T? value ) {
			switch ( value ) {
				case null:
					return default( Boolean? );

				case Boolean b:
					return b;

				case Char c:
					return c.In( ParsingConstants.TrueChars );

				case Int32 i:
					return i >= 1;

				case String s when String.IsNullOrWhiteSpace( s ):
					return default( Boolean? );

				case String s: {
					var trimmed = s.Trimmed();

					if ( trimmed is null ) {
						return default( Boolean? );
					}

					if ( trimmed.In( ParsingConstants.TrueStrings ) ) {
						return true;
					}

					if ( trimmed.In( ParsingConstants.FalseStrings ) ) {
						return default( Boolean? );
					}

					if ( Boolean.TryParse( trimmed, out var result ) ) {
						return result;
					}

					break;
				}
			}

			var t = value.ToString();

			if ( String.IsNullOrWhiteSpace( t ) ) {
				return default( Boolean? );
			}

			t = t.Trim();

			if ( t.In( ParsingConstants.TrueStrings ) ) {
				return true;
			}

			if ( t.In( ParsingConstants.FalseStrings ) ) {
				return default( Boolean? );
			}

			return Boolean.TryParse( t, out var rest ) ? rest : default( Boolean? );
		}

		public static Boolean ToBooleanOrThrow<T>( this T? value ) =>
			value.ToBooleanOrNull() ?? throw new FormatException( $"Unable to convert {nameof( value ).SmartQuote()} [{value}] to a boolean value." );

		[DebuggerStepThrough]
		[Pure]
		public static Byte? ToByteOrNull<T>( this T? value ) {
			try {
				if ( value is null ) {
					return default( Byte? );
				}

				var s = value.ToString()?.Trim();

				if ( String.IsNullOrWhiteSpace( s ) ) {
					return default( Byte? );
				}

				if ( Byte.TryParse( s, out var result ) ) {
					return result;
				}

				return Convert.ToByte( s );
			}
			catch ( FormatException exception ) {
				exception.Log();
			}
			catch ( OverflowException exception ) {
				exception.Log();
			}

			return default( Byte? );
		}

		[DebuggerStepThrough]
		[Pure]
		public static Byte ToByteOrThrow<T>( this T? value ) =>
			value.ToByteOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a byte." );

		[DebuggerStepThrough]
		[Pure]
		public static Byte ToByteOrZero<T>( this T? value ) => value.ToByteOrNull() ?? 0;

		/// <summary>
		///     <para>
		///         Converts the <paramref name="self" /> to a <see cref="DateTime" />. Returns <see cref="DateTime.MinValue" />
		///         if any error occurs.
		///     </para>
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		/// <see cref="ToGuid(DateTime)" />
		[DebuggerStepThrough]
		[Pure]
		public static DateTime ToDateTime( this Guid self ) {
			var bytes = self.ToByteArray();

			//var dayofYear = BitConverter.ToUInt16( bytes, startIndex: 4 ); //not used in constructing the datetime
			//var dayofweek = ( DayOfWeek )bytes[ 8 ]; //not used in constructing the datetime

			return new DateTime( BitConverter.ToInt32( bytes, 0 ), bytes[ 13 ], bytes[ 9 ], bytes[ 10 ], bytes[ 11 ], bytes[ 12 ], BitConverter.ToUInt16( bytes, 6 ),
				( DateTimeKind )bytes[ 15 ] );
		}

		[Pure]
		[DebuggerStepThrough]
		public static DateTime? ToDateTimeOrNull<T>( this T? value ) {
			try {
				if ( DateTime.TryParse( value.Trimmed(), out var result ) ) {
					return result;
				}
			}
			catch ( Exception ) {
				return DateTime.MinValue;
			}

			return default( DateTime? );
		}

		[DebuggerStepThrough]
		[Pure]
		public static Decimal ToDecimal( this Guid self ) {
			TranslateDecimalGuid converter;
			converter.Decimal = Decimal.Zero;
			converter.Guid = self;

			return converter.Decimal;
		}

		/// <summary>Tries to convert <paramref name="value" /> to a <see cref="Decimal" />.</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static Decimal? ToDecimalOrNull<T>( this T? value ) {
			if ( value is null ) {
				return default( Decimal? );
			}

			try {

				//todo This really should look for BOTH "()" and replace with "-"
				var s = value.Trimmed()?.StripLetters().Replace( "$", String.Empty ).Replace( ")", String.Empty ).Replace( "(", "-" ).Replace( "..", "." )
					.Replace( " ", String.Empty ).Trimmed();

				if ( String.IsNullOrEmpty( s ) ) {
					return default( Decimal? );
				}

				if ( Decimal.TryParse( s, out var result ) ) {
					return result;
				}

				return Convert.ToDecimal( s );
			}
			catch ( FormatException exception ) {
				exception.Log();
			}
			catch ( OverflowException exception ) {
				exception.Log();
			}

			return default( Decimal? );
		}

		[DebuggerStepThrough]
		[Pure]
		public static Decimal ToDecimalOrThrow<T>( this T? value ) =>
			value.ToDecimalOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a decimal." );

		[DebuggerStepThrough]
		[Pure]
		public static Decimal ToDecimalOrZero<T>( this T? value ) => value.ToDecimalOrNull() ?? Decimal.Zero;

		[DebuggerStepThrough]
		[Pure]
		public static Folder ToFolder( this Guid guid, Boolean reversed = false ) => new( guid.ToPath( reversed ) );

		[DebuggerStepThrough]
		[Pure]
		public static Guid ToGuid( this Decimal number ) {
			TranslateDecimalGuid converter;
			converter.Guid = Guid.Empty;
			converter.Decimal = number;

			return converter.Guid;
		}

		/// <summary>Convert the first 16 bytes of the SHA256 hash of the <paramref name="word" /> into a <see cref="Guid" />.</summary>
		/// <param name="word"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static Guid ToGuid( this String word ) {
			var hashedBytes = word.Sha256();
			Array.Resize( ref hashedBytes, 16 );

			return new Guid( hashedBytes );
		}

		/// <summary>Converts a datetime to a guid. Returns Guid.Empty if any error occurs.</summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		/// <see cref="ToDateTime" />
		[DebuggerStepThrough]
		[Pure]
		public static Guid ToGuid( this DateTime dateTime ) {
			try {
				unchecked {
					var guid = new Guid( ( UInt32 )dateTime.Year //0,1,2,3
						, ( UInt16 )dateTime.DayOfYear //4,5
						, ( UInt16 )dateTime.Millisecond //6,7
						, ( Byte )dateTime.DayOfWeek //8
						, ( Byte )dateTime.Day //9
						, ( Byte )dateTime.Hour //10
						, ( Byte )dateTime.Minute //11
						, ( Byte )dateTime.Second //12
						, ( Byte )dateTime.Month //13
						, Convert.ToByte( dateTime.IsDaylightSavingTime() ) //14
						, ( Byte )dateTime.Kind ); //15

					return guid;
				}
			}
			catch ( Exception ) {
				return Guid.Empty;
			}
		}

		/// <summary>
		///     <para>
		///         A GUID is a 128-bit integer (16 bytes) that can be used across all computers and networks wherever a unique
		///         identifier is required.
		///     </para>
		///     <para>A GUID has a very low probability of being duplicated.</para>
		/// </summary>
		/// <param name="high">    </param>
		/// <param name="low">   </param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static Guid ToGuid( this UInt64 high, UInt64 low ) => new TranslateGuidUInt64( high, low ).guid;

		[DebuggerStepThrough]
		[Pure]
		public static Guid ToGuid( this (UInt64 high, UInt64 low) values ) => new TranslateGuidUInt64( values.high, values.low ).guid;

		/// <summary>Returns the value converted to an <see cref="Int32" /> or null.</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static Int32? ToIntOrNull<T>( this T? value ) {
			if ( value is null ) {
				return default( Int32? );
			}

			try {
				var s = value.ToString();

				s = s.StripLetters();
				s = s.Replace( "$", String.Empty );
				s = s.Replace( ")", String.Empty );
				s = s.Replace( "(", "-" );
				s = s.Replace( "..", "." );
				s = s.Replace( " ", String.Empty );
				s = s.Trimmed() ?? String.Empty;

				var pos = s.LastIndexOf( '.' );

				if ( pos.Any() ) {
					s = s[ ..pos ];
				}

				if ( !String.IsNullOrEmpty( s ) ) {
					return Int32.TryParse( s, out var result ) ? result : Convert.ToInt32( s );
				}
			}
			catch ( FormatException exception ) {
				exception.Log();
			}
			catch ( OverflowException exception ) {
				exception.Log();
			}

			return default( Int32? );
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="Int32" /> or throws <see cref="FormatException" />.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentEmptyException"></exception>
		/// <exception cref="FormatException"></exception>
		[DebuggerStepThrough]
		[Pure]
		public static Int32 ToIntOrThrow<T>( this T? value ) {
			if ( value is null ) {
				throw new ArgumentEmptyException( nameof( value ) );
			}

			return value.ToIntOrNull() ?? throw new FormatException( $"Cannot convert value `{value}` to Int32." );
		}

		[DebuggerStepThrough]
		[Pure]
		public static Int32 ToIntOrZero<T>( this T? value ) => value.ToIntOrNull() ?? 0;

		/// <summary>Convert string to Guid</summary>
		/// <param name="value">the string value</param>
		/// <returns>the Guid value</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Guid ToMD5HashedGUID( this String? value ) {
			value ??= String.Empty;

			var bytes = Encoding.Unicode.GetBytes( value );

			using var md5 = MD5.Create();

			var data = md5.ComputeHash( bytes );

			return new Guid( data );
		}

		[DebuggerStepThrough]
		[Pure]
		public static Decimal? ToMoneyOrNull( this SqlDataReader bob, String columnName ) {
			if ( bob == null ) {
				throw new ArgumentEmptyException( nameof( bob ) );
			}

			if ( String.IsNullOrWhiteSpace( columnName ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( columnName ) );
			}

			try {
				var ordinal = bob.Ordinal( columnName );

				if ( !ordinal.HasValue ) {
					$"Warning column {columnName} not found in SqlDataReader.".Break();
				}
				else {
					if ( !bob.IsDBNull( ordinal.Value ) ) {
						$"{bob[ columnName ]}".Log(); //TODO

						return bob[ columnName ].ToDecimalOrNull();
					}
				}
			}
			catch ( FormatException exception ) {
				exception.Log();
			}
			catch ( OverflowException exception ) {
				exception.Log();
			}

			return default( Decimal? );
		}

		/// <summary>Return the characters of the guid as a path structure.</summary>
		/// <example>/1/a/b/2/c/d/e/f/</example>
		/// <param name="guid">    </param>
		/// <param name="reversed">Return the reversed order of the <see cref="Guid" />.</param>
		/// <returns></returns>
		/// <see cref="GuidExtensions.FromPath" />
		[DebuggerStepThrough]
		[Pure]
		public static String ToPath( this Guid guid, Boolean reversed = false ) {
			var a = guid.ToByteArray();

			if ( reversed ) {
				return Path.Combine( a[ 15 ].ToString(), a[ 14 ].ToString(), a[ 13 ].ToString(), a[ 12 ].ToString(), a[ 11 ].ToString(), a[ 10 ].ToString(), a[ 9 ].ToString(),
					a[ 8 ].ToString(), a[ 7 ].ToString(), a[ 6 ].ToString(), a[ 5 ].ToString(), a[ 4 ].ToString(), a[ 3 ].ToString(), a[ 2 ].ToString(), a[ 1 ].ToString(),
					a[ 0 ].ToString() );
			}

			return Path.Combine( a[ 0 ].ToString(), a[ 1 ].ToString(), a[ 2 ].ToString(), a[ 3 ].ToString(), a[ 4 ].ToString(), a[ 5 ].ToString(), a[ 6 ].ToString(),
				a[ 7 ].ToString(), a[ 8 ].ToString(), a[ 9 ].ToString(), a[ 10 ].ToString(), a[ 11 ].ToString(), a[ 12 ].ToString(), a[ 13 ].ToString(), a[ 14 ].ToString(),
				a[ 15 ].ToString() );
		}

		[DebuggerStepThrough]
		[Pure]
		public static IEnumerable<String> ToPaths( this DirectoryInfo directoryInfo ) {
			if ( directoryInfo is null ) {
				throw new ArgumentEmptyException( nameof( directoryInfo ) );
			}

			return directoryInfo.FullName.Split( Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
		}

		/// <summary>
		///     Returns the trimmed <paramref name="self" /> ToString() or null.
		///     <para>
		///         If <paramref name="self" /> is null, empty, or whitespace then return null, else return
		///         <paramref name="self" />.ToString().
		///     </para>
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static String? ToStringOrNull<T>( this T? self ) =>
			self switch {
				null => default( String? ),
				DBNull _ => default( String? ),
				String s => s.Trimmed(),
				var _ => Equals( self, DBNull.Value ) ? default( String? ) : self.ToString().Trimmed()
			};

		/// <summary>Returns a trimmed string from <paramref name="value" />, or throws <see cref="FormatException" />.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		[DebuggerStepThrough]
		[Pure]
		public static String ToStringOrThrow<T>( this T? value ) =>
			value.ToStringOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a string." );

		/// <summary>Untested.</summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static UBigInteger ToUBigInteger( this Guid guid ) => new( guid.ToByteArray() );

		/// <summary>Returns a 'Y' for true, or an 'N' for false.</summary>
		/// <param name="value"></param>
		[Pure]
		[DebuggerStepThrough]
		public static Char ToYN( this Boolean value ) => value ? 'Y' : 'N';
	}
}