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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "LargeSizeFormatProvider.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

#nullable enable

namespace Librainian.Extensions;

using System;
using Exceptions;
using Maths;
using Utilities;

public class LargeSizeFormatProvider : IFormatProvider, ICustomFormatter {

	private const String FileSizeFormat = "fs";

	private static String? DefaultFormat( String format, Object arg, IFormatProvider? formatProvider ) {
		var formattableArg = arg as IFormattable;

		var s = formattableArg?.ToString( format, formatProvider );

		return s ?? arg.ToString();
	}

	public String Format( String? format, Object? arg, IFormatProvider? formatProvider ) {
		if ( arg == null ) {
			throw new ArgumentEmptyException( nameof( arg ) );
		}

		if ( String.IsNullOrWhiteSpace( format ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( format ) );
		}

		if ( format.StartsWith( FileSizeFormat, StringComparison.CurrentCultureIgnoreCase ) != true ) {
			return DefaultFormat( format, arg, formatProvider ) ?? String.Empty;
		}

		if ( arg is String ) {
			return DefaultFormat( format, arg, formatProvider ) ?? String.Empty;
		}

		Single size;

		try {
			size = Convert.ToUInt64( arg );
		}
		catch ( InvalidCastException ) {
			return DefaultFormat( format, arg, formatProvider ) ?? String.Empty;
		}

		var suffix = "n/a";

		//TODO add larger sizes

		if ( size.Between( MathConstants.Sizes.OneTeraByte, UInt64.MaxValue ) ) {
			size /= MathConstants.Sizes.OneTeraByte;
			suffix = "trillion";
		}
		else if ( size.Between( MathConstants.Sizes.OneGigaByte, MathConstants.Sizes.OneTeraByte ) ) {
			size /= MathConstants.Sizes.OneGigaByte;
			suffix = "billion";
		}
		else if ( size.Between( MathConstants.Sizes.OneMegaByte, MathConstants.Sizes.OneGigaByte ) ) {
			size /= MathConstants.Sizes.OneMegaByte;
			suffix = "million";
		}
		else if ( size.Between( MathConstants.Sizes.OneKiloByte, MathConstants.Sizes.OneMegaByte ) ) {
			size /= MathConstants.Sizes.OneKiloByte;
			suffix = "thousand";
		}
		else if ( size.Between( UInt64.MinValue, MathConstants.Sizes.OneKiloByte ) ) {
			suffix = "";
		}

		return $"{size:N3} {suffix}";
	}

	public Object? GetFormat( Type? formatType ) => formatType != null && formatType == typeof( ICustomFormatter ) ? this : null;
}