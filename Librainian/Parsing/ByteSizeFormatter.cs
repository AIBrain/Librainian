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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ByteSizeFormatter.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

#nullable enable

namespace Librainian.Parsing;

using System;
using System.Text.RegularExpressions;
using Exceptions;

/// <summary>
///     A custom formatter for byte sizes (things like files, network bandwidth, etc.) that will automatically
///     determine the best abbreviation.
/// </summary>
/// <remarks>From the Vanara.PInvoke project @ https://github.com/dahall/Vanara </remarks>
public class ByteSizeFormatter : Formatter {

	private static readonly String[] Suffixes = {
		" B", " KB", " MB", " GB", " TB", " PB", " EB"
	};

	/// <summary>
	///     Converts the string representation of a byte size to its 64-bit signed integer equivalent. A return value
	///     indicates whether the conversion succeeded.
	/// </summary>
	/// <param name="input">A string containing a byte size to convert.</param>
	/// <param name="bytes">
	///     When this method returns, contains the 64-bit signed integer value equivalent of the value contained in
	///     <paramref name="input" />, if the conversion succeeded,
	///     or zero if the conversion failed. The conversion fails if the <paramref name="input" /> parameter is null or Empty,
	///     or is not of the correct format. This parameter is passed
	///     uninitialized; any value originally supplied in result will be overwritten.
	/// </param>
	/// <returns>
	///     <see langword="true" /> if <paramref name="input" /> was converted successfully; otherwise,
	///     <see langword="false" />.
	/// </returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static Boolean TryParse( String input, out Int64 bytes ) {
		const String expr = @"^\s*(?<num>\d+(?:\.\d+)?)\s*(?<mod>[kKmMgGtTpPeEyY]?[bB])?\s*$";
		var match = Regex.Match( input, expr );
		bytes = 0;

		if ( !match.Success ) {
			return false;
		}

		Int64 mult = 1;

		switch ( match.Groups[ "mod" ].Value.ToUpper() ) {
			case "B":
			case "":
				break;

			case "KB":
				mult = 1024;

				break;

			case "MB":
				mult = ( Int64 ) Math.Pow( 1024, 2 );

				break;

			case "GB":
				mult = ( Int64 ) Math.Pow( 1024, 3 );

				break;

			case "TB":
				mult = ( Int64 ) Math.Pow( 1024, 4 );

				break;

			case "PB":
				mult = ( Int64 ) Math.Pow( 1024, 5 );

				break;

			case "EB":
				mult = ( Int64 ) Math.Pow( 1024, 6 );

				break;

			case "YB":
				mult = ( Int64 ) Math.Pow( 1024, 7 );

				break;

			default:
				throw new InvalidOperationException();
		}

		bytes = ( Int64 ) Math.Round( Single.Parse( match.Groups[ "num" ].Value ) * mult );

		return true;
	}

	/// <summary>
	///     Converts the value of a specified object to an equivalent string representation using specified format and
	///     culture-specific formatting information.
	/// </summary>
	/// <param name="format">A format string containing formatting specifications.</param>
	/// <param name="arg">An object to format.</param>
	/// <param name="formatProvider">An object that supplies format information about the current instance.</param>
	/// <returns>
	///     The string representation of the value of <paramref name="arg" />, formatted as specified by
	///     <paramref name="format" /> and <paramref name="formatProvider" />.
	/// </returns>
	public override String Format( String? format, Object? arg, IFormatProvider? formatProvider ) {
		if ( format == null ) {
			throw new ArgumentEmptyException( nameof( format ) );
		}

		Int64 bytes;

		try {
			bytes = Convert.ToInt64( arg );
		}
		catch {
			return this.HandleOtherFormats( format, arg );
		}

		if ( bytes == 0 ) {
			return "0" + Suffixes[ 0 ];
		}

		var m = Regex.Match( format, @"^[B|b](?<prec>\d+)?$" );

		if ( !m.Success ) {
			return this.HandleOtherFormats( format, arg );
		}

		var prec = m.Groups[ "prec" ].Success ? Byte.Parse( m.Groups[ "prec" ].Value ) : 0;
		var place = Convert.ToInt32( Math.Floor( Math.Log( bytes, 1024 ) ) );

		if ( place >= Suffixes.Length ) {
			place = Suffixes.Length - 1;
		}

		var num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );

		return $"{num.ToString( "F" + prec )}{Suffixes[ place ]}";
	}

}