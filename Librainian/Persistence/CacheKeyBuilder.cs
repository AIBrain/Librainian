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
// File "CacheKeyBuilder.cs" last touched on 2021-10-13 at 4:31 PM by Protiguous.

#nullable enable

namespace Librainian.Persistence;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exceptions;
using Microsoft.Data.SqlClient;
using Parsing;

public static class CacheKeyBuilder {

	/// <summary>
	///     Build a key from combining 1 or more <see cref="T" /> (converted to Strings).
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="things"></param>
	[DebuggerStepThrough]
	public static String BuildKey<T>( params T[] things ) {
		if ( things is null ) {
			throw new NullException( nameof( things ) );
		}

		if ( !things.Any() ) {
			throw new NullException( nameof( things ) );
		}

		var parts = things.Select( o => {
			if ( o is IEnumerable<SqlParameter> parameters ) {
				var kvp = parameters.Select( parameter => new {
					parameter.ParameterName, parameter.Value
				} );

				return $"{kvp.ToStrings( Symbols.TwoPipes )}".Trim();
			}

			var s = o.Trimmed().NullIfEmpty();

			if ( s != null ) {
				return s;
			}

			return $"{Symbols.VerticalEllipsis}null{Symbols.VerticalEllipsis}";
		} );

		return parts.ToStrings( Symbols.TwoPipes ).Trim();
	}

	/// <summary>
	///     Build a key from combining 1 or more Objects.
	/// </summary>
	/// <param name="things"></param>
	[DebuggerStepThrough]
	public static String BuildKey( params Object[] things ) {
		if ( things is null ) {
			throw new NullException( nameof( things ) );
		}

		if ( !things.Any() ) {
			throw new NullException( nameof( things ) );
		}

		var parts = things.Select( o => {
			if ( o is IEnumerable<SqlParameter> collection ) {
				var kvp = collection.Select( parameter => new {
					parameter.ParameterName, parameter.Value, parameter
				} );

				return $"{kvp.ToStrings( Symbols.TwoPipes )}".Trim();
			}

			return o.ToString();
		} );

		return parts.ToStrings( Symbols.TwoPipes ).Trim();
	}

}