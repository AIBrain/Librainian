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
// File "PhoneNumber.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Parsing;

using System;
using System.Text.RegularExpressions;

/// <summary>
///     //TODO This whole concept needs tests.
///     <code>PhoneNumber phoneNumber = "555-867-5309";</code>
/// </summary>
public record PhoneNumber( Int32 AreaCode, Int32 ExchangeCode, Int32 StationCode ) {

	private static Regex PhoneNumberRegex { get; } = new(@"\(?(\d{3})\)?-? *(\d{3})-? *-?(\d{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public static implicit operator PhoneNumber( String value ) {
		var match = PhoneNumberRegex.Match( value );
		if ( match.Length < 4 ) {
			throw new FormatException( "Invalid phone number format" );
		}

		return new PhoneNumber( Int32.Parse( match.Groups[ 1 ].Value ), Int32.Parse( match.Groups[ 2 ].Value ), Int32.Parse( match.Groups[ 3 ].Value ) );
	}

}