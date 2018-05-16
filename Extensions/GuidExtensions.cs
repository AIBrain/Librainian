// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "GuidExtensions.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/GuidExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using FileSystem;
    using FluentAssertions;
    using Maths.Numbers;
    using NUnit.Framework;

    /// <summary>
    ///     A GUID is a 128-bit integer (16 bytes) that can be used across all computers and networks wherever a unique
    ///     identifier is required.
    /// </summary>
    /// <remarks>I just love guids!</remarks>
    public static class GuidExtensions {

        public static readonly Regex InGuidFormat =
            new Regex(
                pattern: "^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                         "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", options: RegexOptions.Compiled );

        /// <summary>
        ///     Convert string to Guid
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the Guid value</returns>
        public static Guid ConvertToMD5HashGUID( String value ) {

            // convert null to empty string - null can not be hashed
            if ( value == null ) { value = String.Empty; }

            // get the byte representation
            var bytes = Encoding.Unicode.GetBytes( value );

            // create the md5 hash
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash( bytes );

            // convert the hash to a Guid
            return new Guid( data );
        }

        /// <summary>
        ///     <seealso cref="ToPath" />
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Guid FromPath( this DirectoryInfo path ) {
            var s = path.ToPaths().ToList();
            s.RemoveAll( s1 => s1.Any( c => !Char.IsDigit( c ) ) );

            if ( s.Count < 16 ) { return Guid.Empty; }

            var b = new Byte[s.Count];

            for ( var i = 0; i < s.Count; i++ ) { b[i] = Convert.ToByte( s[i] ); }

            try {
                var result = new Guid( b );

                return result;
            }
            catch ( ArgumentException exception ) { exception.More(); }

            return Guid.Empty;
        }

        /// <summary>
        ///     Converts the string representation of a Guid to its Guid equivalent. A return value indicates whether the operation
        ///     succeeded.
        /// </summary>
        /// <param name="s">A string containing a Guid to convert.</param>
        /// When this method returns, contains the Guid value equivalent to the Guid contained in
        /// <paramref name="s" />
        /// , if the conversion succeeded, or
        /// <see cref="Guid.Empty" />
        /// if the conversion failed. The conversion fails if the
        /// <paramref name="s" />
        /// parameter is a
        /// <see langword="null" />
        /// reference (
        /// <see langword="Nothing" />
        /// in Visual Basic), or is not of the correct format.
        /// <value>
        ///     <see langword="true" /> if <paramref name="s" /> was converted successfully; otherwise, <see langword="false" />
        ///     .
        /// </value>
        /// <exception cref="ArgumentNullException">Thrown if <pararef name="s" /> is <see langword="null" />.</exception>
        /// <remarks>Original code at https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=94072</remarks>
        public static Boolean IsGuid( this String s ) {
            if ( s is null ) { throw new ArgumentNullException( nameof( s ) ); }

            var match = InGuidFormat.Match( input: s );

            return match.Success;
        }

        /// <summary>
        ///     merge two guids
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Guid Munge( this Guid left, Guid right ) {
            const Int32 bytecount = 16;
            var destByte = new Byte[bytecount];
            var lhsBytes = left.ToByteArray();
            var rhsBytes = right.ToByteArray();
            Assert.AreEqual( expected: lhsBytes.Length, actual: destByte.Length );
            Assert.AreEqual( expected: rhsBytes.Length, actual: destByte.Length );

            for ( var i = 0; i < bytecount; i++ ) {
                unchecked { destByte[i] = ( Byte )( lhsBytes[i] ^ rhsBytes[i] ); }
            }

            return new Guid( b: destByte );
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid">  </param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Guid Next( this Guid guid, Int64 amount = 1 ) {
            var bytes = guid.ToByteArray();
            var uBigInteger = new UBigInteger( bytes: bytes );
            uBigInteger += amount;
            var array = uBigInteger.ToByteArray();
            Array.Resize( array: ref array, newSize: 16 );
            var next = new Guid( b: array );

            return next;
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid">  </param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Guid Previous( this Guid guid, Int64 amount = 1 ) {
            var bytes = guid.ToByteArray();
            var uBigInteger = new UBigInteger( bytes: bytes );
            uBigInteger -= amount;
            var array = uBigInteger.ToByteArray();
            Array.Resize( array: ref array, newSize: 16 );
            var next = new Guid( b: array );

            return next;
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static BigInteger ToBigInteger( this Guid guid ) {
            var bigInteger = new BigInteger( guid.ToByteArray() );

            return bigInteger;
        }

        /// <summary>
        ///     (Kinda) converts a guid to a datetime. Returns DateTime.MinValue if any error occurs.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        /// <seealso cref="ToGuid(DateTime)" />
        public static DateTime ToDateTime( this Guid g ) {
            try {
                var bytes = g.ToByteArray();
                var year = BitConverter.ToInt32( bytes, startIndex: 0 );
                var dayofYear = BitConverter.ToUInt16( bytes, startIndex: 4 );
                var millisecond = BitConverter.ToUInt16( bytes, startIndex: 6 );
                var dayofweek = ( DayOfWeek )bytes[8];
                var day = bytes[9];
                var hour = bytes[10];
                var minute = bytes[11];
                var second = bytes[12];
                var month = bytes[13];
                var kind = ( DateTimeKind )bytes[15];
                var result = new DateTime( year: year, month: month, day: day, hour: hour, minute: minute, second: second, millisecond: millisecond, kind: kind );
                Assert.AreEqual( expected: result.DayOfYear, actual: dayofYear );
                Assert.AreEqual( expected: result.DayOfWeek, actual: dayofweek );

                return result;
            }
            catch ( Exception exception ) {
                exception.More();

                return DateTime.MinValue;
            }
        }

        public static Decimal ToDecimal( this Guid guid ) {
            DecimalGuidConverter converter;
            converter.Decimal = Decimal.Zero;
            converter.Guid = guid;

            return converter.Decimal;
        }

        public static Folder ToFolder( this Guid guid, Boolean reversed = false ) => new Folder( fullPath: guid.ToPath( reversed: reversed ) );

        public static Guid ToGuid( this Decimal number ) {
            DecimalGuidConverter converter;
            converter.Guid = Guid.Empty;
            converter.Decimal = number;

            return converter.Guid;
        }

        /// <summary>
        ///     Convert the first 16 bytes of the SHA256 hash of the <paramref name="word" /> into a <see cref="Guid" />.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static Guid ToGuid( this String word ) {
            var hashedBytes = word.Sha256();
            Array.Resize( array: ref hashedBytes, newSize: 16 );

            return new Guid( b: hashedBytes );
        }

        /// <summary>
        ///     Converts a datetime to a guid. Returns Guid.Empty if any error occurs.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// <seealso cref="ToDateTime" />
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

                    guid.Should().NotBeEmpty();

                    return guid;
                }
            }
            catch ( Exception ) { return Guid.Empty; }
        }

        /// <summary>
        ///     <para>
        ///         A GUID is a 128-bit integer (16 bytes) that can be used across all computers and networks wherever a unique
        ///         identifier is required.
        ///     </para>
        ///     <para>A GUID has a very low probability of being duplicated.</para>
        /// </summary>
        /// <param name="mostImportantbits">    </param>
        /// <param name="somewhatImportantbits"></param>
        /// <param name="leastImportantbits">   </param>
        /// <returns></returns>
        public static Guid ToGuid( this UInt64 mostImportantbits, UInt64 somewhatImportantbits, UInt64 leastImportantbits ) {
            var guidMerger = new GuidUnionUInt64( high: mostImportantbits, low: leastImportantbits );

            return guidMerger.guid;
        }

        public static Guid ToGuid( this Tuple<UInt64, UInt64, UInt64> tuple ) {
            var guidMerger = new GuidUnionUInt64( high: tuple.Item1, low: tuple.Item3 );

            return guidMerger.guid;
        }

        /// <summary>
        ///     Return the characters of the guid as a path structure.
        /// </summary>
        /// <example>1/a/b/2/c/d/e/f/</example>
        /// <param name="guid">    </param>
        /// <param name="reversed">Return the reversed order of the <see cref="Guid" />.</param>
        /// <returns></returns>
        /// <seealso cref="FromPath" />
        public static String ToPath( this Guid guid, Boolean reversed = false ) {
            var a = guid.ToByteArray();

            if ( reversed ) {
                return Path.Combine( a[15].ToString(), a[14].ToString(), a[13].ToString(), a[12].ToString(), a[11].ToString(), a[10].ToString(), a[9].ToString(), a[8].ToString(), a[7].ToString(),
                    a[6].ToString(), a[5].ToString(), a[4].ToString(), a[3].ToString(), a[2].ToString(), a[1].ToString(), a[0].ToString() );
            }

            var pathNormal = Path.Combine( a[0].ToString(), a[1].ToString(), a[2].ToString(), a[3].ToString(), a[4].ToString(), a[5].ToString(), a[6].ToString(), a[7].ToString(), a[8].ToString(),
                a[9].ToString(), a[10].ToString(), a[11].ToString(), a[12].ToString(), a[13].ToString(), a[14].ToString(), a[15].ToString() );

            return pathNormal;
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static UBigInteger ToUBigInteger( this Guid guid ) {
            var bigInteger = new UBigInteger( bytes: guid.ToByteArray() );

            return bigInteger;
        }

        [StructLayout( layoutKind: LayoutKind.Explicit )]
        public struct DecimalGuidConverter {

            [FieldOffset( 0 )]
            public Decimal Decimal;

            [FieldOffset( 0 )]
            public Guid Guid;
        }

        /// <summary>
        ///     I don't think this works the way I originally intended. Don't use it.
        /// </summary>
        [StructLayout( layoutKind: LayoutKind.Explicit )]
        public struct GuidUnionUInt64 {

            [FieldOffset( 0 )] // bytes 0..15 == 16 bytes
            public Guid guid;

            [FieldOffset( 0 )]
            public readonly UInt64 Low;

            [FieldOffset( sizeof( UInt64 ) )]
            public readonly UInt64 High;

            public GuidUnionUInt64( UInt64 high, UInt64 low ) {
                this.guid = Guid.Empty;
                this.Low = low;
                this.High = high;
            }
        }
    }
}