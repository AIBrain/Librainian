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
// File "Symbols.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

namespace Librainian.Parsing;

using System;

public static class Symbols {

	public const String BallotBox = "☐";

	public const String BallotBoxWithCheck = "☑";

	public const String BallotBoxWithX = "☒";

	public const String BlackStar = "★";

	public const String CheckMark = "✓";

	public const String CheckMarkHeavy = "✔";

	/// <summary>❕</summary>
	public const String Error = "❕";

	/// <summary>❌</summary>
	public const String Exception = FailBig;

	public const String Fail = "🗙";

	public const String FailBig = "❌";

	public const String Interobang1 = "‽";

	public const String Interobang2 = "⁈";

	/// <summary>Symbol for NAK</summary>
	public const String NegativeAcknowledge = "␕";

	public const String No = "🚫";

	/// <summary>N/A</summary>
	public const String NotApplicable = "ⁿ̸ₐ";

	/// <summary>N/A</summary>
	public const String NotApplicableHeavy = "ⁿ/ₐ";

	public const String Null = "␀";

	public const String Pipe = "|";

	public const String SkullAndCrossbones = "☠";

	public const String StopSign = "🛑";

	public const String Timeout = "⌛";

	public const String TriplePipes = "⦀";

	public const Char TripleTilde = '≋';

	public const String TwoPipes = "‖";

	public const String Underscore = "_";

	public const String Unknown = "�";

	public const String VerticalEllipsis = "⋮";

	public const String Warning = "⚠";

	public const String WhiteStar = "☆";

	public static class Dice {

		public const String Five = "⚄";

		public const String Four = "⚃";

		public const String One = "⚀";

		public const String Six = "⚅";

		public const String Three = "⚂";

		public const String Two = "⚁";
	}

	public static class Left {

		public const Char DoubleAngle = '«';

		public const Char DoubleParenthesis = '⸨';
	}

	public static class Right {

		public const Char DoubleAngle = '»';

		public const Char DoubleParenthesis = '⸩';
	}
}

/// <summary>Attempts at using text/emoji to make animations - display 1 char at a time from each string.</summary>
public static class TextAnimations {

	public const String BallotBoxCheck = Symbols.BallotBox + Symbols.BallotBoxWithCheck;

	public const String BallotBoxUnCheck = Symbols.BallotBoxWithCheck + Symbols.BallotBox;

	public const String Hearts = "🤍💖💓💗💛💙💚🧡💜🖤";

	public const String HorizontalDots = "‥…";

	public const String Pipes = Symbols.VerticalEllipsis + Symbols.Pipe + Symbols.TwoPipes + Symbols.TriplePipes;

	public const String StarBurst = "⭐⁕⁑⁂✨";

	public const String VerticalDots = "․⁞:⁚⁝";
}