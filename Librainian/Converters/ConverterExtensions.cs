// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConverterExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ConverterExtensions.cs" was last formatted by Protiguous on 2019/02/04 at 8:24 PM.

namespace Librainian.Converters {

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Collections.Extensions;
    using Controls;
    using Database;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Maths.Numbers;
    using OperatingSystem.FileSystem;
    using Parsing;
    using Security;

    public static class ConverterExtensions {

        /// <summary>
        ///     Converts strings that may contain "$" or "()" to a <see cref="Decimal" /> amount.
        ///     <para>Null or empty strings return <see cref="Decimal.Zero" />.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Decimal MoneyToDecimal<T>( [CanBeNull] this T value ) {
            if ( value == null ) {
                return Decimal.Zero;
            }

            var amount = value.ToString();

            if ( String.IsNullOrEmpty( amount ) ) {
                return Decimal.Zero;
            }

            amount = amount.Replace( "$", String.Empty );
            amount = amount.Replace( ")", String.Empty );
            amount = amount.Replace( "(", "-" );

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
        public static String StripLetters( [NotNull] this String s ) {
            if ( s == null ) {
                throw new ArgumentNullException( paramName: nameof( s ) );
            }

            return Regex.Replace( s, "[a-zA-Z]", String.Empty );
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static BigInteger ToBigInteger( this Guid guid ) => new BigInteger( guid.ToByteArray() );

        [DebuggerStepThrough]
        [Pure]
        public static Boolean? ToBooleanOrNull<T>( [CanBeNull] this T value ) {
            if ( value == null ) {
                return null;
            }

            if ( value is Boolean b ) {
                return b;
            }

            if ( value is Char c ) {
                return c.In( ParsingConstants.TrueChars );
            }

            if ( value is Int32 i ) {
                return i >= 1;
            }

            if ( value is String s ) {
                if ( String.IsNullOrWhiteSpace( s ) ) {
                    return null;
                }

                s = s.Trim();

                if ( s.In( ParsingConstants.TrueStrings ) ) {
                    return true;
                }

                if ( s.In( ParsingConstants.FalseStrings ) ) {
                    return false;
                }

                if ( Boolean.TryParse( s, out var result ) ) {
                    return result;
                }
            }

            var t = value.ToString();

            if ( String.IsNullOrWhiteSpace( t ) ) {
                return null;
            }

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

            return null; //don't know.
        }

        [DebuggerStepThrough]
        [Pure]
        public static Byte? ToByteOrNull<T>( [CanBeNull] this T value ) {
            try {
                if ( value == null ) {
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
        public static Byte ToByteOrThrow<T>( this T value ) => value.ToByteOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a byte." );

        [DebuggerStepThrough]
        [Pure]
        public static Byte ToByteOrZero<T>( this T value ) => value.ToByteOrNull() ?? 0;

        /// <summary>
        ///     <para>
        ///         Converts the <paramref name="guid" /> to a <see cref="DateTime" />. Returns <see cref="DateTime.MinValue" />
        ///         if any error occurs.
        ///     </para>
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <see cref="ToGuid(DateTime)" />
        [DebuggerStepThrough]
        [Pure]
        public static DateTime ToDateTime( this Guid guid ) {
            try {
                var bytes = guid.ToByteArray();
                var year = BitConverter.ToInt32( bytes, startIndex: 0 );
                var dayofYear = BitConverter.ToUInt16( bytes, startIndex: 4 ); //not used in constructing the datetime
                var millisecond = BitConverter.ToUInt16( bytes, startIndex: 6 );
                var dayofweek = ( DayOfWeek )bytes[ 8 ]; //not used in constructing the datetime
                var day = bytes[ 9 ];
                var hour = bytes[ 10 ];
                var minute = bytes[ 11 ];
                var second = bytes[ 12 ];
                var month = bytes[ 13 ];
                var kind = ( DateTimeKind )bytes[ 15 ];
                var result = new DateTime( year: year, month: month, day: day, hour: hour, minute: minute, second: second, millisecond: millisecond, kind: kind );

                
                return result;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return DateTime.MinValue;
        }

        [Pure]
        [DebuggerStepThrough]
        public static DateTime? ToDateTimeOrNull<T>( [CanBeNull] this T value ) {
            try {
                if ( value != null && DateTime.TryParse( value.ToString().Trim(), out var result ) ) {
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
        public static Decimal ToDecimal( this Guid guid ) {
            TranslateDecimalGuid converter;
            converter.Decimal = Decimal.Zero;
            converter.Guid = guid;

            return converter.Decimal;
        }

        /// <summary>
        ///     Tries to convert <paramref name="value" /> to a <see cref="Decimal" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Decimal? ToDecimalOrNull<T>( [CanBeNull] this T value ) {
            if ( value == null ) {
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
                s = s.Trim();

                if ( String.IsNullOrWhiteSpace( s ) ) {
                    return null;
                }

                if ( s.Contains( "$" ) || s.Contains( "(" ) ) {
                    return s.MoneyToDecimal();
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
        public static Decimal ToDecimalOrThrow<T>( this T value ) =>
            value.ToDecimalOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a decimal." );

        [DebuggerStepThrough]
        [Pure]
        public static Decimal ToDecimalOrZero<T>( this T value ) => value.ToDecimalOrNull() ?? Decimal.Zero;

        [NotNull]
        [DebuggerStepThrough]
        [Pure]
        public static Folder ToFolder( this Guid guid, Boolean reversed = false ) => new Folder( fullPath: guid.ToPath( reversed: reversed ) );

        [DebuggerStepThrough]
        [Pure]
        public static Guid ToGuid( this Decimal number ) {
            TranslateDecimalGuid converter;
            converter.Guid = Guid.Empty;
            converter.Decimal = number;

            return converter.Guid;
        }

        /// <summary>
        ///     Convert the first 16 bytes of the SHA256 hash of the <paramref name="word" /> into a <see cref="Guid" />.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Guid ToGuid( [NotNull] this String word ) {
            var hashedBytes = word.Sha256();
            Array.Resize( array: ref hashedBytes, newSize: 16 );

            return new Guid( b: hashedBytes );
        }

        /// <summary>
        ///     Converts a datetime to a guid. Returns Guid.Empty if any error occurs.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// <see cref="ToDateTime" />
        [DebuggerStepThrough]
        [Pure]
        public static Guid ToGuid( this DateTime dateTime ) {
            try {
                unchecked {
                    var guid = new Guid( a: ( UInt32 )dateTime.Year //0,1,2,3
                        , b: ( UInt16 )dateTime.DayOfYear //4,5
                        , c: ( UInt16 )dateTime.Millisecond //6,7
                        , d: ( Byte )dateTime.DayOfWeek //8
                        , e: ( Byte )dateTime.Day //9
                        , f: ( Byte )dateTime.Hour //10
                        , g: ( Byte )dateTime.Minute //11
                        , h: ( Byte )dateTime.Second //12
                        , i: ( Byte )dateTime.Month //13
                        , j: Convert.ToByte( dateTime.IsDaylightSavingTime() ) //14
                        , k: ( Byte )dateTime.Kind ); //15

                    
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
        public static Guid ToGuid( this UInt64 high, UInt64 low ) => new TranslateGuidUInt64( high: high, low: low ).guid;

        [DebuggerStepThrough]
        [Pure]
        public static Guid ToGuid( this (UInt64 high, UInt64 low) values ) => new TranslateGuidUInt64( high: values.high, low: values.low ).guid;

        /// <summary>
        ///     Returns the value converted to an <see cref="Int32" /> or null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Int32? ToIntOrNull<T>( [CanBeNull] this T value ) {
            if ( value == null ) {
                return null;
            }

            try {
                String s;

                if ( value is Control control ) {
                    s = control.Text();
                }
                else {
                    s = value.ToString();
                }

                s = s.StripLetters();
                s = s.Replace( "$", String.Empty );
                s = s.Replace( ")", String.Empty );
                s = s.Replace( "(", "-" );
                s = s.Replace( "..", "." );
                s = s.Replace( " ", "" );
                s = s.Trim();

                var pos = s.LastIndexOf( '.' );

                if ( pos.Any() ) {
                    s = s.Substring( 0, pos );
                }

                if ( String.IsNullOrEmpty( s ) ) {
                    return null;
                }

                if ( Int32.TryParse( s, out var result ) ) {
                    return result;
                }

                return Convert.ToInt32( s );
            }
            catch ( FormatException exception ) {
                exception.Log();
            }
            catch ( OverflowException exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>
        ///     Converts <paramref name="value" /> to an <see cref="Int32" /> or throws <see cref="FormatException" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        [DebuggerStepThrough]
        [Pure]
        public static Int32 ToIntOrThrow<T>( [NotNull] this T value ) {
            if ( value == null ) {
                throw new ArgumentNullException( nameof( value ), "Cannot convert null value to Int32." );
            }

            return value.ToIntOrNull() ?? throw new FormatException( $"Cannot convert value `{value}` to Int32." );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Int32 ToIntOrZero<T>( this T value ) => value.ToIntOrNull() ?? 0;

        [NotNull]
        public static ManagementPath ToManagementPath( [NotNull] this DirectoryInfo systemPath ) {
            var fullPath = systemPath.FullName;

            while ( fullPath.EndsWith( @"\", StringComparison.Ordinal ) ) {
                fullPath = fullPath.Substring( 0, fullPath.Length - 1 );
            }

            fullPath = "Win32_Directory.Name=\"" + fullPath.Replace( "\\", "\\\\" ) + "\"";
            var managed = new ManagementPath( fullPath );

            return managed;
        }

        /// <summary>
        ///     Convert string to Guid
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the Guid value</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Guid ToMD5HashedGUID( this String value ) {
            if ( value == null ) {
                value = String.Empty;
            }

            var bytes = Encoding.Unicode.GetBytes( value );
            var data = MD5.Create().ComputeHash( bytes );

            return new Guid( data );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Decimal? ToMoneyOrNull( this SqlDataReader bob, String columnName ) {
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

        /// <summary>
        ///     Return the characters of the guid as a path structure.
        /// </summary>
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
                return Path.Combine( a[ 15 ].ToString(), a[ 14 ].ToString(), a[ 13 ].ToString(), a[ 12 ].ToString(), a[ 11 ].ToString(), a[ 10 ].ToString(), a[ 9 ].ToString(),
                    a[ 8 ].ToString(), a[ 7 ].ToString(), a[ 6 ].ToString(), a[ 5 ].ToString(), a[ 4 ].ToString(), a[ 3 ].ToString(), a[ 2 ].ToString(), a[ 1 ].ToString(),
                    a[ 0 ].ToString() );
            }

            var pathNormal = Path.Combine( a[ 0 ].ToString(), a[ 1 ].ToString(), a[ 2 ].ToString(), a[ 3 ].ToString(), a[ 4 ].ToString(), a[ 5 ].ToString(), a[ 6 ].ToString(),
                a[ 7 ].ToString(), a[ 8 ].ToString(), a[ 9 ].ToString(), a[ 10 ].ToString(), a[ 11 ].ToString(), a[ 12 ].ToString(), a[ 13 ].ToString(), a[ 14 ].ToString(),
                a[ 15 ].ToString() );

            return pathNormal;
        }

        [DebuggerStepThrough]
        [Pure]
        [NotNull]
        public static IEnumerable<String> ToPaths( [NotNull] this DirectoryInfo directoryInfo ) {
            if ( directoryInfo == null ) {
                throw new ArgumentNullException( nameof( directoryInfo ) );
            }

            return directoryInfo.FullName.Split( new[] {
                Path.DirectorySeparatorChar
            }, StringSplitOptions.RemoveEmptyEntries );
        }

        /// <summary>
        ///     Returns the trimmed <paramref name="obj" /> ToString() or null.
        ///     <para>If <paramref name="obj" /> is null, empty, or whitespace then return null, else return obj.ToString().Trim().</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public static String ToStringOrNull<T>( [CanBeNull] this T obj ) {
            if ( obj == null ) {
                return null;
            }

            if ( obj is String s ) {
                return String.IsNullOrWhiteSpace( s ) ? null : s.Trim();
            }

            if ( obj is DBNull ) {
                return null;
            }

            if ( Equals( obj, DBNull.Value ) ) {
                return null;
            }

            var value = obj.ToString().Trim();

            return value.IsEmpty() ? null : value;
        }

        /// <summary>
        ///     Returns a trimmed string from <paramref name="value" />, or throws <see cref="FormatException" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        [Pure]
        [NotNull]
        public static String ToStringOrThrow<T>( this T value ) =>
            value.ToStringOrNull() ?? throw new FormatException( $"Unable to convert value '{nameof( value )}' to a string." );

        [DebuggerStepThrough]
        [Pure]
        [NotNull]
        public static String ToStringTrimmed<T>( [CanBeNull] this T obj ) => obj.ToStringOrNull()?.Trim() ?? String.Empty;

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static UBigInteger ToUBigInteger( this Guid guid ) {
            var bigInteger = new UBigInteger( bytes: guid.ToByteArray() );

            return bigInteger;
        }
    }
}