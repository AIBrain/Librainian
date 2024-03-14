// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Urls.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

namespace Librainian.Extensions;

using System;
using System.Net;
using System.Text;
using Exceptions;

public static class Urls {

	/// <summary>Check that a String is not null or empty</summary>
	/// <param name="input">String to check</param>
	/// <returns>Boolean</returns>
	public static Boolean HasValue( this String? input ) => !String.IsNullOrEmpty( input );

	public static String? HtmlDecode( this String? input ) => WebUtility.HtmlDecode( input );

	public static String HtmlEncode( this String input ) => WebUtility.HtmlEncode( input );

	public static Boolean IsNameOnlyQueryString( this String? res ) => !String.IsNullOrEmpty( res ) && res[ 0 ] == '?';

	public static Uri UrlDecode( this String input ) {
		if ( String.IsNullOrWhiteSpace( input ) ) {
			throw new NullException( nameof( input ) );
		}

		return new Uri( WebUtility.UrlDecode( input ) );
	}

	/// <summary>Uses Uri.EscapeDataString() based on recommendations on MSDN http: //blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx</summary>
	public static Uri UrlEncode( this String input ) {
		if ( input is null ) {
			throw new NullException( nameof( input ) );
		}

		const Int32 maxLength = 32766;

		if ( input.Length <= maxLength ) {
			return new Uri( Uri.EscapeDataString( input ) );
		}

		var sb = new StringBuilder( input.Length * 2 );
		var index = 0;

		while ( index < input.Length ) {
			var length = Math.Min( input.Length - index, maxLength );
			var subString = input.Substring( index, length );
			sb.Append( Uri.EscapeDataString( subString ) );
			index += subString.Length;
		}

		return new Uri( sb.ToString() );
	}
}