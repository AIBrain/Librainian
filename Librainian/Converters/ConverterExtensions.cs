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
// Project: "Librainian", "ConverterExtensions.cs" was last formatted by Protiguous on 2018/07/10 at 8:57 PM.

namespace Librainian.Converters
{

    using ComputerSystem.FileSystem;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Maths;
    using Maths.Numbers;
    using NUnit.Framework;
    using Security;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Management;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using Logging;

    public static class ConverterExtensions
    {

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static BigInteger ToBigInteger(this Guid guid) => new BigInteger(guid.ToByteArray());

        /// <summary>
        ///     <para>
        ///         Converts the <paramref name="guid" /> to a <see cref="DateTime" />. Returns <see cref="DateTime.MinValue" />
        ///         if any error occurs.
        ///     </para>
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <see cref="ToGuid(DateTime)" />
        public static DateTime ToDateTime(this Guid guid)
        {
            try
            {
                var bytes = guid.ToByteArray();
                var year = BitConverter.ToInt32(bytes, startIndex: 0);
                var dayofYear = BitConverter.ToUInt16(bytes, startIndex: 4); //not used in constructing the datetime
                var millisecond = BitConverter.ToUInt16(bytes, startIndex: 6);
                var dayofweek = (DayOfWeek)bytes[8]; //not used in constructing the datetime
                var day = bytes[9];
                var hour = bytes[10];
                var minute = bytes[11];
                var second = bytes[12];
                var month = bytes[13];
                var kind = (DateTimeKind)bytes[15];
                var result = new DateTime(year: year, month: month, day: day, hour: hour, minute: minute, second: second, millisecond: millisecond, kind: kind);

                Assert.AreEqual(expected: result.DayOfYear, actual: dayofYear);
                Assert.AreEqual(expected: result.DayOfWeek, actual: dayofweek);

                return result;
            }
            catch (Exception exception) { exception.Log(); }

            return DateTime.MinValue;
        }

        public static Decimal ToDecimal(this Guid guid)
        {
            TranslateDecimalGuid converter;
            converter.Decimal = Decimal.Zero;
            converter.Guid = guid;

            return converter.Decimal;
        }

        [NotNull]
        public static Folder ToFolder(this Guid guid, Boolean reversed = false) => new Folder(fullPath: guid.ToPath(reversed: reversed));

        public static Guid ToGuid(this Decimal number)
        {
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
        public static Guid ToGuid(this String word)
        {
            var hashedBytes = word.Sha256();
            Array.Resize(array: ref hashedBytes, newSize: 16);

            return new Guid(b: hashedBytes);
        }

        /// <summary>
        ///     Converts a datetime to a guid. Returns Guid.Empty if any error occurs.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// <see cref="ToDateTime" />
        public static Guid ToGuid(this DateTime dateTime)
        {
            try
            {
                unchecked
                {
                    var guid = new Guid(a: (UInt32)dateTime.Year //0,1,2,3
                        , b: (UInt16)dateTime.DayOfYear //4,5
                        , c: (UInt16)dateTime.Millisecond //6,7
                        , d: (Byte)dateTime.DayOfWeek //8
                        , e: (Byte)dateTime.Day //9
                        , f: (Byte)dateTime.Hour //10
                        , g: (Byte)dateTime.Minute //11
                        , h: (Byte)dateTime.Second //12
                        , i: (Byte)dateTime.Month //13
                        , j: Convert.ToByte(dateTime.IsDaylightSavingTime()) //14
                        , k: (Byte)dateTime.Kind); //15

                    guid.Should().NotBeEmpty();

                    return guid;
                }
            }
            catch (Exception) { return Guid.Empty; }
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
        public static Guid ToGuid(this UInt64 mostImportantbits, UInt64 somewhatImportantbits, UInt64 leastImportantbits)
        {
            var guidMerger = new TranslateGuidUInt64(high: mostImportantbits, low: leastImportantbits);

            return guidMerger.guid;
        }

        public static Guid ToGuid([NotNull] this Tuple<UInt64, UInt64, UInt64> tuple)
        {
            var guidMerger = new TranslateGuidUInt64(high: tuple.Item1, low: tuple.Item3);

            return guidMerger.guid;
        }

        [NotNull]
        public static ManagementPath ToManagementPath([NotNull] this DirectoryInfo systemPath)
        {
            var fullPath = systemPath.FullName;

            while (fullPath.EndsWith(@"\", StringComparison.Ordinal)) { fullPath = fullPath.Substring(0, fullPath.Length - 1); }

            fullPath = "Win32_Directory.Name=\"" + fullPath.Replace("\\", "\\\\") + "\"";
            var managed = new ManagementPath(fullPath);

            return managed;
        }

        /// <summary>
        ///     Convert string to Guid
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the Guid value</returns>
        public static Guid ToMD5HashedGUID(this String value)
        {
            if (value == null) { value = String.Empty; }

            var bytes = Encoding.Unicode.GetBytes(value);
            var data = MD5.Create().ComputeHash(bytes);

            return new Guid(data);
        }

        /// <summary>
        ///     Return the characters of the guid as a path structure.
        /// </summary>
        /// <example>1/a/b/2/c/d/e/f/</example>
        /// <param name="guid">    </param>
        /// <param name="reversed">Return the reversed order of the <see cref="Guid" />.</param>
        /// <returns></returns>
        /// <see cref="GuidExtensions.FromPath" />
        [NotNull]
        public static String ToPath(this Guid guid, Boolean reversed = false)
        {
            var a = guid.ToByteArray();

            if (reversed)
            {
                return Path.Combine(a[15].ToString(), a[14].ToString(), a[13].ToString(), a[12].ToString(), a[11].ToString(), a[10].ToString(), a[9].ToString(), a[8].ToString(), a[7].ToString(),
                    a[6].ToString(), a[5].ToString(), a[4].ToString(), a[3].ToString(), a[2].ToString(), a[1].ToString(), a[0].ToString());
            }

            var pathNormal = Path.Combine(a[0].ToString(), a[1].ToString(), a[2].ToString(), a[3].ToString(), a[4].ToString(), a[5].ToString(), a[6].ToString(), a[7].ToString(), a[8].ToString(),
                a[9].ToString(), a[10].ToString(), a[11].ToString(), a[12].ToString(), a[13].ToString(), a[14].ToString(), a[15].ToString());

            return pathNormal;
        }

        [NotNull]
        public static IEnumerable<String> ToPaths([NotNull] this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) { throw new ArgumentNullException(nameof(directoryInfo)); }

            return directoryInfo.FullName.Split(new[] {
                Path.DirectorySeparatorChar
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static UBigInteger ToUBigInteger(this Guid guid)
        {
            var bigInteger = new UBigInteger(bytes: guid.ToByteArray());

            return bigInteger;
        }

        [StructLayout(layoutKind: LayoutKind.Explicit)]
        public struct DecimalTo
        {

            [FieldOffset(offset: 0)]

            // ReSharper disable once FieldCanBeMadeReadOnly.Global
            public Decimal Decimal;

            [FieldOffset(offset: 0)]
            public Guid Guid;

            /// <summary>
            ///     Access the first four bytes.
            /// </summary>
            [FieldOffset(offset: 0)]
            public FourBytes Bytes4;

            /// <summary>
            ///     Access all eight bytes.
            /// </summary>
            [FieldOffset(offset: 0)]
            public EightBytes Bytes8;
        }
    }
}