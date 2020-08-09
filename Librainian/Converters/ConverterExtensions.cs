#nullable enable
namespace Librainian.Converters {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Numerics;
	using System.Security.Cryptography;
	using System.Text;
	using System.Text.RegularExpressions;
	using Collections.Extensions;
	using Extensions;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Maths.Numbers;
	using Microsoft.Data.SqlClient;
	using OperatingSystem.FileSystem;
	using OperatingSystem.FileSystem.Pri.LongPath;
	using Parsing;
	using Security;

	public static class ConverterExtensions {

		[NotNull]
		private static readonly String[] FalseStrings = {
			"N", "0", "no", "false", Boolean.FalseString, "Fail", "failed", "Failure", "bad"
		};

		[NotNull]
		private static readonly Char[] TrueChars = {
			'Y', '1'
		};

		[NotNull]
		private static readonly String[] TrueStrings = {
			"Y", "1", "yes", "true", Boolean.TrueString, "Success", "good", "ok"
		};

		[CanBeNull]
		public static T Cast<T>( this Object? self ) {

			if ( self == null ) {
				return default!;
			}

			if ( self is { } cast ) {
				return ( T )cast;
			}

			if ( Convert.IsDBNull( self ) ) {
				return default;
			}

			return ( T )Convert.ChangeType( self, typeof( T ) );
		}

		/*
		[CanBeNull]
		public static T? Cast<T>( this Object? self ) where T : struct {
			if ( self == null ) {
				return null;
			}

			if ( Convert.IsDBNull( self ) ) {
				return null;
			}

			if ( self is { } cast ) {
				return cast as T?;
			}

			return ( T )Convert.ChangeType( self, typeof( T ) );
		}
		*/

		/// <summary>
		///     Converts strings that may contain "$" or "()" to a <see cref="Decimal" /> amount.
		///     <para>Null or empty strings return <see cref="Decimal.Zero" />.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static Decimal MoneyToDecimal<T>( [CanBeNull] this T value ) {
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

		[NotNull]
		[DebuggerStepThrough]
		[Pure]
		public static String StripLetters( [NotNull] this String? s ) => Regex.Replace( s, "[a-zA-Z]", String.Empty );

		/// <summary>Untested.</summary>
		/// <param name="self"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static BigInteger ToBigInteger( this Guid self ) => new BigInteger( self.ToByteArray() );

		/// <summary>
		///     <para>Returns true if <paramref name="value" /> is a true, 'Y', "yes", "true", "1", or '1'.</para>
		///     <para>Returns false if <paramref name="value" /> is a false, 'N', "no", "false", or '0'.</para>
		///     <para>A null will return false.</para>
		/// </summary>
		/// <param name="value"></param>
		[Pure]
		public static Boolean ToBoolean<T>( [CanBeNull] this T value ) {
			switch ( value ) {
				case null:
					return default;

				case Boolean b:
					return b;

				case Char c:
					return c.In( TrueChars );

				case Int32 i:
					return i >= 1;

				case String s when String.IsNullOrWhiteSpace( s ):
					return default;

				case String s: {
					s = s.Trimmed();

					if ( s.In( TrueStrings ) ) {
						return true;
					}

					if ( Boolean.TryParse( s, out var result ) ) {
						return result;
					}

					break;
				}
			}

			var t = value.ToString();

			if ( !String.IsNullOrWhiteSpace( t ) ) {
				t = t.Trim();

				if ( t.In( TrueStrings ) ) {
					return true;
				}

				if ( t.In( FalseStrings ) ) {
					return default;
				}

				if ( Boolean.TryParse( t, out var rest ) ) {
					return rest;
				}
			}

			return default;
		}

		[DebuggerStepThrough]
		[Pure]
		public static Boolean? ToBooleanOrNull<T>( [CanBeNull] this T value ) {
			switch ( value ) {
				case null:
					return default;

				case Boolean b:
					return b;

				case Char c:
					return c.In( ParsingConstants.TrueChars );

				case Int32 i:
					return i >= 1;

				case String s when String.IsNullOrWhiteSpace( s ):
					return null;

				case String s: {
					s = s.Trimmed();

					if ( s is null ) {
						return default;
					}

					if ( s.In( ParsingConstants.TrueStrings ) ) {
						return true;
					}

					if ( s.In( ParsingConstants.FalseStrings ) ) {
						return default;
					}

					if ( Boolean.TryParse( s, out var result ) ) {
						return result;
					}

					break;
				}
			}

			var t = value.ToString();

			if ( String.IsNullOrWhiteSpace( t ) ) {
				return default;
			}

			t = t.Trim();

			if ( t.In( ParsingConstants.TrueStrings ) ) {
				return true;
			}

			if ( t.In( ParsingConstants.FalseStrings ) ) {
				return default;
			}

			return Boolean.TryParse( t, out var rest ) ? rest : default( Boolean? );
		}

		public static Boolean ToBooleanOrThrow<T>( [CanBeNull] this T value ) =>
			value.ToBooleanOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a boolean value." );

		[DebuggerStepThrough]
		[Pure]
		public static Byte? ToByteOrNull<T>( [CanBeNull] this T value ) {
			try {
				if ( value is null ) {
					return null;
				}

				var s = value.ToString().Trim();

				if ( String.IsNullOrWhiteSpace( s ) ) {
					return null;
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

			return null;
		}

		[DebuggerStepThrough]
		[Pure]
		public static Byte ToByteOrThrow<T>( [CanBeNull] this T value ) =>
			value.ToByteOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a byte." );

		[DebuggerStepThrough]
		[Pure]
		public static Byte ToByteOrZero<T>( [CanBeNull] this T value ) => value.ToByteOrNull() ?? 0;

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
		public static DateTime? ToDateTimeOrNull<T>( [CanBeNull] this T value ) {
			try {
				if ( DateTime.TryParse( value?.ToString().Trim(), out var result ) ) {
					return result;
				}
			}
			catch ( Exception ) {
				return DateTime.MinValue;
			}

			return null;
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
		public static Decimal? ToDecimalOrNull<T>( [CanBeNull] this T value ) {
			if ( value is null ) {
				return null;
			}

			try {
				//todo This really should look for BOTH "()" and replace with "-"
				var s = value.Trimmed()?.StripLetters().Replace( "$", String.Empty ).Replace( ")", String.Empty ).Replace( "(", "-" ).Replace( "..", "." )
							 .Replace( " ", String.Empty ).Trimmed();

				if ( String.IsNullOrEmpty( s ) ) {
					return null;
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

			return null;
		}

		[DebuggerStepThrough]
		[Pure]
		public static Decimal ToDecimalOrThrow<T>( [CanBeNull] this T value ) =>
			value.ToDecimalOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a decimal." );

		[DebuggerStepThrough]
		[Pure]
		public static Decimal ToDecimalOrZero<T>( [CanBeNull] this T value ) => value.ToDecimalOrNull() ?? Decimal.Zero;

		[NotNull]
		[DebuggerStepThrough]
		[Pure]
		public static Folder ToFolder( this Guid guid, Boolean reversed = false ) => new Folder( guid.ToPath( reversed ) );

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
		public static Guid ToGuid( [NotNull] this String word ) {
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
					var guid = new Guid( ( UInt32 )dateTime.Year                           //0,1,2,3
									   , ( UInt16 )dateTime.DayOfYear                      //4,5
									   , ( UInt16 )dateTime.Millisecond                    //6,7
									   , ( Byte )dateTime.DayOfWeek                        //8
									   , ( Byte )dateTime.Day                              //9
									   , ( Byte )dateTime.Hour                             //10
									   , ( Byte )dateTime.Minute                           //11
									   , ( Byte )dateTime.Second                           //12
									   , ( Byte )dateTime.Month                            //13
									   , Convert.ToByte( dateTime.IsDaylightSavingTime() ) //14
									   , ( Byte )dateTime.Kind );                          //15

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
		public static Int32? ToIntOrNull<T>( [CanBeNull] this T value ) {
			if ( value is null ) {
				return null;
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
					s = s.Substring( 0, pos );
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

			return null;
		}

		/// <summary>Converts <paramref name="value" /> to an <see cref="Int32" /> or throws <see cref="FormatException" />.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FormatException"></exception>
		[DebuggerStepThrough]
		[Pure]
		public static Int32 ToIntOrThrow<T>( [CanBeNull] this T value ) {
			if ( value is null ) {
				throw new ArgumentNullException( nameof( value ), "Cannot convert null value to Int32." );
			}

			return value.ToIntOrNull() ?? throw new FormatException( $"Cannot convert value `{value}` to Int32." );
		}

		[DebuggerStepThrough]
		[Pure]
		public static Int32 ToIntOrZero<T>( [CanBeNull] this T value ) => value.ToIntOrNull() ?? 0;

		/// <summary>Convert string to Guid</summary>
		/// <param name="value">the string value</param>
		/// <returns>the Guid value</returns>
		[DebuggerStepThrough]
		[Pure]
		public static Guid ToMD5HashedGUID( [CanBeNull] this String? value ) {
			value ??= String.Empty;

			var bytes = Encoding.Unicode.GetBytes( value );

			using var md5 = MD5.Create();

			var data = md5.ComputeHash( bytes );

			return new Guid( data );
		}

		public static Int32? Ordinal( [NotNull] this SqlDataReader bob, [NotNull] String columnName ) {
			try {
				return bob.GetOrdinal( columnName );
			}
			catch ( IndexOutOfRangeException exception ) {
				exception.Log();
			}

			return default;
		}

		[DebuggerStepThrough]
		[Pure]
		public static Decimal? ToMoneyOrNull( [NotNull] this SqlDataReader bob, [NotNull] String columnName ) {
			if ( bob == null ) {
				throw new ArgumentNullException( nameof( bob ) );
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

			return null;
		}

		/// <summary>Return the characters of the guid as a path structure.</summary>
		/// <example>1/a/b/2/c/d/e/f/</example>
		/// <param name="guid">    </param>
		/// <param name="reversed">Return the reversed order of the <see cref="Guid" />.</param>
		/// <returns></returns>
		/// <see cref="GuidExtensions.FromPath" />
		[DebuggerStepThrough]
		[Pure]
		[NotNull]
		public static String ToPath( this Guid guid, Boolean reversed = false ) {
			var a = guid.ToByteArray();

			if ( reversed ) {
				return Path.Combine( a[ 15 ].ToString()!, a[ 14 ].ToString()!, a[ 13 ].ToString()!, a[ 12 ].ToString()!, a[ 11 ].ToString()!, a[ 10 ].ToString()!, a[ 9 ].ToString()!,
									 a[ 8 ].ToString()!, a[ 7 ].ToString()!, a[ 6 ].ToString()!, a[ 5 ].ToString()!, a[ 4 ].ToString()!, a[ 3 ].ToString()!, a[ 2 ].ToString()!,
									 a[ 1 ].ToString()!, a[ 0 ].ToString()! );
			}

			return Path.Combine( a[ 0 ].ToString()!, a[ 1 ].ToString()!, a[ 2 ].ToString()!, a[ 3 ].ToString()!, a[ 4 ].ToString()!, a[ 5 ].ToString()!, a[ 6 ].ToString()!,
								 a[ 7 ].ToString()!, a[ 8 ].ToString()!, a[ 9 ].ToString()!, a[ 10 ].ToString()!, a[ 11 ].ToString()!, a[ 12 ].ToString()!, a[ 13 ].ToString()!,
								 a[ 14 ].ToString()!, a[ 15 ].ToString()! );
		}

		[DebuggerStepThrough]
		[Pure]
		[NotNull]
		public static IEnumerable<String> ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
			if ( directoryInfo is null ) {
				throw new ArgumentNullException( nameof( directoryInfo ) );
			}

			return directoryInfo.FullPath.Split( new[] {
				Path.DirectorySeparatorChar
			}, StringSplitOptions.RemoveEmptyEntries );
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
		[CanBeNull]
		[Pure]
		public static String? ToStringOrNull<T>( [CanBeNull] this T self ) =>
			self switch
			{
				null => default,
				DBNull _ => default,
				String s => s.Trimmed(),
				_ => Equals( self, DBNull.Value ) ? default : self.ToString().Trimmed()
			};

		/// <summary>Returns a trimmed string from <paramref name="value" />, or throws <see cref="FormatException" />.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		[DebuggerStepThrough]
		[Pure]
		[NotNull]
		public static String ToStringOrThrow<T>( [CanBeNull] this T value ) =>
			value.ToStringOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a string." );

		/// <summary>Untested.</summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[Pure]
		public static UBigInteger ToUBigInteger( this Guid guid ) => new UBigInteger( guid.ToByteArray() );

		/// <summary>Returns a 'Y' for true, or an 'N' for false.</summary>
		/// <param name="value"></param>
		[Pure]
		[DebuggerStepThrough]
		public static Char ToYN( this Boolean value ) => value ? 'Y' : 'N';

	}

}