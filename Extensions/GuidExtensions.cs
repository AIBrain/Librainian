// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/GuidExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using FileSystem;
    using FluentAssertions;
    using Maths.Numbers;
    using NUnit.Framework;
    using Security;

    public static class GuidExtensions {

        public static Guid FromPath( this DirectoryInfo path ) {
            var s = path.ToPaths().ToList();
            s.RemoveAll( s1 => s1.Any( c => !Char.IsDigit( c ) ) );

            if ( s.Count < 16 ) {
                return Guid.Empty;
            }

            var b = new Byte[ s.Count ];
            for ( var i = 0; i < s.Count; i++ ) {
                b[ i ] = Convert.ToByte( s[ i ] );
            }
            try {
                var result = new Guid( b );
                return result;
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }

            return Guid.Empty;
        }

        /// <summary>merge two guids</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Guid Munge( this Guid left, Guid right ) {
            const Int32 bytecount = 16;
            var destByte = new Byte[ bytecount ];
            var lhsBytes = left.ToByteArray();
            var rhsBytes = right.ToByteArray();
            Assert.AreEqual( lhsBytes.Length, destByte.Length );
            Assert.AreEqual( rhsBytes.Length, destByte.Length );

            for ( var i = 0; i < bytecount; i++ ) {
                unchecked {
                    destByte[ i ] = ( Byte )( lhsBytes[ i ] ^ rhsBytes[ i ] );
                }
            }
            return new Guid( destByte );
        }

        /// <summary>Untested.</summary>
        /// <param name="guid"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Guid Next( this Guid guid, Int64 amount = 1 ) {
            var bytes = guid.ToByteArray();
            var uBigInteger = new UBigInteger( bytes );
            uBigInteger += amount;
            var array = uBigInteger.ToByteArray();
            Array.Resize( ref array, 16 );
            var next = new Guid( array );
            return next;
        }

        /// <summary>Untested.</summary>
        /// <param name="guid"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Guid Previous( this Guid guid, Int64 amount = 1 ) {
            var bytes = guid.ToByteArray();
            var uBigInteger = new UBigInteger( bytes );
            uBigInteger -= amount;
            var array = uBigInteger.ToByteArray();
            Array.Resize( ref array, 16 );
            var next = new Guid( array );
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
        public static DateTime ToDateTime( this Guid g ) {
            try {
                var bytes = g.ToByteArray();
                var year = BitConverter.ToInt32( bytes, 0 );
                var dayofYear = BitConverter.ToUInt16( bytes, 4 );
                var millisecond = BitConverter.ToUInt16( bytes, 6 );
                var dayofweek = ( DayOfWeek )bytes[ 8 ];
                var day = bytes[ 9 ];
                var hour = bytes[ 10 ];
                var minute = bytes[ 11 ];
                var second = bytes[ 12 ];
                var month = bytes[ 13 ];
                var kind = ( DateTimeKind )bytes[ 15 ];
                var result = new DateTime( year: year, month: month, day: day, hour: hour, minute: minute, second: second, millisecond: millisecond, kind: kind );
                Assert.AreEqual( result.DayOfYear, dayofYear );
                Assert.AreEqual( result.DayOfWeek, dayofweek );
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

        public static Folder ToFolder( this Guid guid, Boolean reversed = false ) => new Folder( guid.ToPath( reversed ).FullName );

        public static Guid ToGuid( this Decimal number ) {
            DecimalGuidConverter converter;
            converter.Guid = Guid.Empty;
            converter.Decimal = number;
            return converter.Guid;
        }

        /// <summary>
        ///     Convert the first 16 bytes of the SHA256 hash of the <paramref name="word" /> into a <see cref="Guid"/>.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static Guid ToGuid( this String word ) {
            var hashedBytes = word.Sha256();
            Array.Resize( ref hashedBytes, 16 );
            return new Guid( hashedBytes );
        }

        /// <summary>Converts a datetime to a guid. Returns Guid.Empty if any error occurs.</summary>
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
            catch ( Exception ) {
                return Guid.Empty;
            }
        }

        /// <summary>
        ///     <para>
        ///         A GUID is a 128-bit integer (16 bytes) that can be used across all computers and
        ///         networks wherever a unique identifier is required.
        ///     </para>
        ///     <para>A GUID has a very low probability of being duplicated.</para>
        /// </summary>
        /// <param name="mostImportantbits"></param>
        /// <param name="somewhatImportantbits"></param>
        /// <param name="leastImportantbits"></param>
        /// <returns></returns>
        public static Guid ToGuid( this UInt64 mostImportantbits, UInt64 somewhatImportantbits, UInt64 leastImportantbits ) {
            var guidMerger = new GuidMergerUInt64( mostImportantbits, somewhatImportantbits, leastImportantbits );
            return guidMerger.guid;
        }

        public static Guid ToGuid( this Tuple<UInt64, UInt64, UInt64> tuple ) {
            var guidMerger = new GuidMergerUInt64( tuple.Item1, tuple.Item2, tuple.Item3 );
            return guidMerger.guid;
        }

        /// <summary>Return the characters of the guid as a path structure.</summary>
        /// <example>1/a/b/2/c/d/e/f/</example>
        /// <param name="guid"></param>
        /// <param name="reversed">Return the reversed order of the <see cref="Guid" />.</param>
        /// <returns></returns>
        public static DirectoryInfo ToPath( this Guid guid, Boolean reversed = false ) {
            var a = guid.ToByteArray();
            return new DirectoryInfo( reversed ? Path.Combine( a[ 15 ].ToString(), a[ 14 ].ToString(), a[ 13 ].ToString(), a[ 12 ].ToString(), a[ 11 ].ToString(), a[ 10 ].ToString(), a[ 9 ].ToString(), a[ 8 ].ToString(), a[ 7 ].ToString(), a[ 6 ].ToString(), a[ 5 ].ToString(), a[ 4 ].ToString(), a[ 3 ].ToString(), a[ 2 ].ToString(), a[ 1 ].ToString(), a[ 0 ].ToString() ) : Path.Combine( a[ 0 ].ToString(), a[ 1 ].ToString(), a[ 2 ].ToString(), a[ 3 ].ToString(), a[ 4 ].ToString(), a[ 5 ].ToString(), a[ 6 ].ToString(), a[ 7 ].ToString(), a[ 8 ].ToString(), a[ 9 ].ToString(), a[ 10 ].ToString(), a[ 11 ].ToString(), a[ 12 ].ToString(), a[ 13 ].ToString(), a[ 14 ].ToString(), a[ 15 ].ToString() ) );

            //return guid.ToString( "D" ).Substring( 0, 4 ).Aggregate( String.Empty, ( current, ch ) => current + ( ch + "/" ) );
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static UBigInteger ToUBigInteger( this Guid guid ) {
            var bigInteger = new UBigInteger( guid.ToByteArray() );
            return bigInteger;
        }

        [StructLayout( LayoutKind.Explicit )]
        public struct DecimalGuidConverter {

            [FieldOffset( 0 )]
            public Decimal Decimal;

            [FieldOffset( 0 )]
            public Guid Guid;
        }

        [StructLayout( LayoutKind.Explicit )]
        public struct GuidMergerUInt64 {

            [FieldOffset( 0 )] // bytes 0..15 == 16 bytes
            public Guid guid;

            [FieldOffset( 0 )]
            public readonly UInt64 lowest;

            [FieldOffset( sizeof(UInt64) )]
            public readonly UInt64 center;

            [FieldOffset( sizeof( UInt64 )+ sizeof( UInt64 ) )] //8+8=16 == sizeof(Guid)
            public readonly UInt64 highest;

            public GuidMergerUInt64( UInt64 most, UInt64 middle, UInt64 least ) {
                this.guid = Guid.Empty;
                this.lowest = least;
                this.center = middle;
                this.highest = most;
                if ( ( most != 0 ) || ( middle != 0 ) || ( least != 0 ) ) {
                    this.guid.Should().NotBeEmpty();
                }
            }
        }

        /// <summary>
        /// Converts the string representation of a Guid to its Guid 
        /// equivalent. A return value indicates whether the operation 
        /// succeeded. 
        /// </summary>
        /// <param name="s">A string containing a Guid to convert.</param>
        /// 
        /// When this method returns, contains the Guid value equivalent to 
        /// the Guid contained in <paramref name="s"/>, if the conversion 
        /// succeeded, or <see cref="Guid.Empty"/> if the conversion failed. 
        /// The conversion fails if the <paramref name="s"/> parameter is a 
        /// <see langword="null" /> reference (<see langword="Nothing" /> in 
        /// Visual Basic), or is not of the correct format. 
        /// 
        /// <value>
        /// <see langword="true" /> if <paramref name="s"/> was converted 
        /// successfully; otherwise, <see langword="false" />.
        /// </value>
        /// <exception cref="ArgumentNullException">
        ///        Thrown if <pararef name="s"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// Original code at https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=94072
        /// </remarks>
        public static Boolean IsGuid( this String s ) {
            if ( s == null ) {
                throw new ArgumentNullException( nameof( s ) );
            }

            var match = InGuidFormat.Match( s );

            return match.Success;
        }

        public static Regex InGuidFormat { get; } = new Regex( "^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" + "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled );

    }
}