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
// File "GuidExtensions.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

namespace Librainian.Extensions;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Exceptions;
using FileSystem;
using Logging;
using Maths.Numbers;

/// <summary>
/// A GUID is a 128-bit integer (16 bytes) that can be used across all computers and networks wherever a unique identifier is required.
/// </summary>
/// <remarks>I just love guids!</remarks>
public static class GuidExtensions {

	public static readonly Regex InGuidFormat =
		new(
			"^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
			"^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled );

	/// <summary><see cref="Converters.ConverterExtensions.ToPath" /></summary>
	/// <param name="path"></param>
	public static Guid FromPath( this DirectoryInfo path ) {
		if ( path == null ) {
			throw new NullException( nameof( path ) );
		}

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
		catch ( NullException exception ) {
			exception.Log();
		}

		return Guid.Empty;
	}

	/// <summary>
	/// Converts the string representation of a Guid to its Guid equivalent. A return value indicates whether the operation succeeded.
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
	/// <see langword="true" /> if <paramref name="s" /> was converted successfully; otherwise, <see langword="false" /> .
	/// </value>
	/// <exception cref="NullException">Thrown if <pararef name="s" /> is <see langword="null" />.</exception>
	/// <remarks>Original code at https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=94072</remarks>
	public static Boolean IsGuid( this String s ) {
		if ( s is null ) {
			throw new NullException( nameof( s ) );
		}

		var match = InGuidFormat.Match( s );

		return match.Success;
	}

	/// <summary>merge two guids</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Guid Munge( this Guid left, Guid right ) {
		const Int32 bytecount = 16;
		var destByte = new Byte[ bytecount ];
		var lhsBytes = left.ToByteArray();
		var rhsBytes = right.ToByteArray();

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
	public static Guid Previous( this Guid guid, Int64 amount = 1 ) {
		var bytes = guid.ToByteArray();
		var uBigInteger = new UBigInteger( bytes );
		uBigInteger -= amount;
		var array = uBigInteger.ToByteArray();
		Array.Resize( ref array, 16 );
		var next = new Guid( array );

		return next;
	}
}