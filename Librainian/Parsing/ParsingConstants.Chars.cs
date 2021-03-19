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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ParsingConstants.Chars.cs" last touched on 2021-03-18 at 4:19 AM by Protiguous.

#nullable enable

namespace Librainian.Parsing {

	using System;

	public static partial class ParsingConstants {

		public static class Chars {

			public const Char LeftBrace = '[';
			public const Char RightBrace = ']';

			/// <summary>
			///     Char x0D
			/// </summary>
			public const Char CR = '\r';

			public const Char BrailleBlank = '⠀';

			public const Char EmSpace = ' ';

			public const Char EnSpace = ' ';

			public const Char FigureSpace = ' ';

			public const Char FourPerEmSpace = ' ';

			public const Char HairSpace = ' ';

			/// <summary>
			///     Char x0A
			/// </summary>
			public const Char LF = '\n';

			public const Char NoBreakSpace = '\u00A0';

			public const Char Apostrophe = '\'';

			/// <summary>
			///     `
			/// </summary>
			public const Char GraveAccent = LeftSingleQuote;

			/// <summary>
			///     ´
			/// </summary>
			public const Char AcuteAccent = RightSingleQuote;

			/// <summary>
			///     `
			/// </summary>
			public const Char LeftSingleQuote = '`';

			/// <summary>
			///     ´
			/// </summary>
			public const Char RightSingleQuote = '´';

			/// <summary>
			///     ( <see cref="Char" />)0x0000
			/// </summary>
			public const Char NullChar = ( Char ) 0x0000;

			public const Char PunctuationSpace = ' ';

			/// <summary>
			///     The " char as a <see cref="Char" />.
			/// </summary>
			public const Char DoubleQuote = '\"';

			/// <summary>
			///     The ' char as a <see cref="Char" />.
			/// </summary>
			public const Char SingleQuote = '\'';

			public const Char SixPerEmSpace = '\u2006';

			/// <summary>
			/// “
			/// </summary>
			public const Char LeftDoubleQuote = '\u201C';

			/// <summary>
			/// ”
			/// </summary>
			public const Char RightDoubleQuote = '\u201D';

			/// <summary>
			///     A single space char.
			/// </summary>
			public const Char Space = ' ';

			/// <summary>
			///     The tab char as a <see cref="Char" />.
			/// </summary>
			public const Char Tab = '\t';

			public const Char ThinSpace = '\u2009';

			public const Char ThreePerEmSpace = ' ';

			public const Char ZeroWidthSpace = '\u200B';

		}

	}

}