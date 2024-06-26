﻿// Copyright © Protiguous. All Rights Reserved.
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
// File "Cookies.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

#nullable enable

namespace Librainian.Internet.Servers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

public class Cookies {

	private SortedList<String, Cookie> CookieCollection { get; } = new();

	/// <summary>
	/// Returns a Cookies instance populated by parsing the specified String. The String should be the value of the "Cookie"
	/// header that was received from the remote client. If the String is null or empty, an empty cookies collection is returned.
	/// </summary>
	/// <param name="str">The value of the "Cookie" header sent by the remote client.</param>
	public static Cookies FromString( String str ) {
		var cookies = new Cookies();

		str = WebUtility.UrlDecode( str );

#if NETCORE || NETSTANDARD
			var parts = str.Split( ';', StringSplitOptions.RemoveEmptyEntries );
#else
		if ( String.IsNullOrEmpty( str ) ) {
			return cookies;
		}

		var parts = str.Split( ';' );
#endif

		foreach ( var s in parts ) {
			var idxEquals = s.IndexOf( '=' );

			if ( idxEquals < 1 ) {
				continue;
			}

			var name = s[ ..idxEquals ].Trim();
			var value = s[ ( idxEquals + 1 ).. ].Trim();
			cookies.Add( name, value );
		}

		return cookies;
	}

	/// <summary>
	/// Adds a cookie with the specified name and value. The cookie is set to expire immediately at the end of the browsing session.
	/// </summary>
	/// <param name="name">The cookie's name.</param>
	/// <param name="value">The cookie's value.</param>
	public void Add( String? name, String? value ) => this.Add( name, value, TimeSpan.Zero );

	/// <summary>Adds a cookie with the specified name, value, and lifespan.</summary>
	/// <param name="name">The cookie's name.</param>
	/// <param name="value">The cookie's value.</param>
	/// <param name="expireTime">The amount of time before the cookie should expire.</param>
	public void Add( String name, String? value, TimeSpan expireTime ) {
		name = name.ToLower( CultureInfo.CurrentCulture );
		this.CookieCollection[ name ] = new Cookie( name, value, expireTime );
	}

	/// <summary>Gets the cookie with the specified name. If the cookie is not found, null is returned;</summary>
	/// <param name="name">The name of the cookie.</param>
	public Cookie? Get( String name ) => this.CookieCollection.TryGetValue( name, out var cookie ) ? cookie : null;

	/// <summary>Gets the value of the cookie with the specified name. If the cookie is not found, an empty String is returned;</summary>
	/// <param name="name">The name of the cookie.</param>
	public String GetValue( String name ) {
		var cookie = this.Get( name );

		return cookie?.Value ?? String.Empty;
	}

	/// <summary>
	/// Returns a String of "Set-Cookie: ..." headers (one for each cookie in the collection) separated by "\r\n". There is no
	/// leading or trailing "\r\n".
	/// </summary>
	/// <returns>
	/// A String of "Set-Cookie: ..." headers (one for each cookie in the collection) separated by "\r\n". There is no leading
	/// or trailing "\r\n".
	/// </returns>
	public override String ToString() {
		var cookiesStr = this.CookieCollection.Values.Select( cookie =>
			$"Set-Cookie: {cookie.Name}={cookie.Value}{( cookie.Expire == TimeSpan.Zero ? "" : "; Max-Age=" + ( Int64 )cookie.Expire.TotalSeconds )}; Path=/" );

		return String.Join( "\r\n", cookiesStr );
	}
}