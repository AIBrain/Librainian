// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ByteSizeFormatter.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "ByteSizeFormatter.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.Parsing {

    using System;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;

    /// <summary>A custom formatter for byte sizes (things like files, network bandwidth, etc.) that will automatically determine the best abbreviation.</summary>
    /// <remarks>From the Vanara.PInvoke project @ https://github.com/dahall/Vanara </remarks>
    public class ByteSizeFormatter : Formatter {

        private static readonly String[] Suffixes = {
            " B", " KB", " MB", " GB", " TB", " PB", " EB"
        };

        /// <summary>Converts the string representation of a byte size to its 64-bit signed integer equivalent. A return value indicates whether the conversion succeeded.</summary>
        /// <param name="input">A string containing a byte size to convert.</param>
        /// <param name="bytes">
        /// When this method returns, contains the 64-bit signed integer value equivalent of the value contained in <paramref name="input" />, if the conversion succeeded,
        /// or zero if the conversion failed. The conversion fails if the <paramref name="input" /> parameter is null or Empty, or is not of the correct format. This parameter is passed
        /// uninitialized; any value originally supplied in result will be overwritten.
        /// </param>
        /// <returns><see langword="true" /> if <paramref name="input" /> was converted successfully; otherwise, <see langword="false" />.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Boolean TryParse( [NotNull] String input, out Int64 bytes ) {
            const String expr = @"^\s*(?<num>\d+(?:\.\d+)?)\s*(?<mod>[kKmMgGtTpPeEyY]?[bB])?\s*$";
            var match = Regex.Match( input: input, pattern: expr );
            bytes = 0;

            if ( !match.Success ) {
                return default;
            }

            Int64 mult = 1;

            switch ( match.Groups[ groupname: "mod" ].Value.ToUpper() ) {
                case "B":
                case "":
                    break;

                case "KB":
                    mult = 1024;

                    break;

                case "MB":
                    mult = ( Int64 ) Math.Pow( x: 1024, y: 2 );

                    break;

                case "GB":
                    mult = ( Int64 ) Math.Pow( x: 1024, y: 3 );

                    break;

                case "TB":
                    mult = ( Int64 ) Math.Pow( x: 1024, y: 4 );

                    break;

                case "PB":
                    mult = ( Int64 ) Math.Pow( x: 1024, y: 5 );

                    break;

                case "EB":
                    mult = ( Int64 ) Math.Pow( x: 1024, y: 6 );

                    break;

                case "YB":
                    mult = ( Int64 ) Math.Pow( x: 1024, y: 7 );

                    break;

                default: throw new InvalidOperationException();
            }

            bytes = ( Int64 ) Math.Round( a: Single.Parse( s: match.Groups[ groupname: "num" ].Value ) * mult );

            return true;
        }

        /// <summary>Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.</summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
        /// <returns>The string representation of the value of <paramref name="arg" />, formatted as specified by <paramref name="format" /> and <paramref name="formatProvider" />.</returns>
        [NotNull]
        public override String Format( [NotNull] String format, [CanBeNull] Object? arg, [CanBeNull] IFormatProvider formatProvider ) {
            if ( format == null ) {
                throw new ArgumentNullException( paramName: nameof( format ) );
            }

            Int64 bytes;

            try {
                bytes = Convert.ToInt64( value: arg );
            }
            catch {
                return this.HandleOtherFormats( format: format, arg: arg );
            }

            if ( bytes == 0 ) {
                return "0" + Suffixes[ 0 ];
            }

            var m = Regex.Match( input: format, pattern: @"^[B|b](?<prec>\d+)?$" );

            if ( !m.Success ) {
                return this.HandleOtherFormats( format: format, arg: arg );
            }

            var prec = m.Groups[ groupname: "prec" ].Success ? Byte.Parse( s: m.Groups[ groupname: "prec" ].Value ) : 0;
            var place = Convert.ToInt32( value: Math.Floor( d: Math.Log( a: bytes, newBase: 1024 ) ) );

            if ( place >= Suffixes.Length ) {
                place = Suffixes.Length - 1;
            }

            var num = Math.Round( value: bytes / Math.Pow( x: 1024, y: place ), digits: 1 );

            return $"{num.ToString( format: "F" + prec )}{Suffixes[ place ]}";
        }

    }

}