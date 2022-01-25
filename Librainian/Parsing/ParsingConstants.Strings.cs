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
// File "ParsingConstants.Strings.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

#nullable enable

namespace Librainian.Parsing;

using System;
using Utilities;

public static partial class ParsingConstants {

	public static class Strings {

		public const String Astronaut1 = "👨‍🚀";

		public const String Astronaut2 = "\U0001f468\u200D\U0001f680";

		/// <summary>
		///     Char x0D
		/// </summary>
		public const String CarriageReturn = "\r";

		public const String CRLF = "\r\n";

		/// <summary>
		///     The " char
		/// </summary>
		public const String DoubleQuote = "\"";

		/// <summary>
		///     Two spaces as a <see cref="String" />.
		/// </summary>
		[NeedsTesting]
		public const String Doublespace = Singlespace + Singlespace;

		/// <summary>
		///     Char x0A
		/// </summary>
		public const String LineFeed = "\n";

		/// <summary>
		///     The ' char as a <see cref="String" />.
		/// </summary>
		public const String SingleQuote = "'";

		/// <summary>
		///     A single space char as a <see cref="String" />.
		/// </summary>
		public const String Singlespace = " ";

		/// <summary> ~`!@#$%^&*()-_=+?:,./\[]{}|' </summary>
		[NeedsTesting]
		public const String Symbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

		/// <summary>
		///     The tab char as a <see cref="String" />.
		/// </summary>
		public const String Tab = "\t";
	}
}