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
// File "ParsingConstants.cs" last formatted on 2021-11-30 at 7:21 PM by Protiguous.

#nullable enable

namespace Librainian.Parsing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Measurement.Time;

public static partial class ParsingConstants {

	/// <summary>N, 0, no, false, fail, failed, failure, bad</summary>
	public static readonly String[] FalseStrings = {
		"N", "0", "false", Boolean.FalseString, "fail", "failed", "stop", nameof( Status.Bad ), nameof( Status.Failure ), nameof( Status.No ),
		nameof( Status.Negative )
	};

	/// <summary>Y, 1</summary>
	public static readonly Char[] TrueChars = {
		'Y', '1'
	};

	/// <summary>Y, 1, yes, true, Success, good, Go, Positive, Continue</summary>
	public static readonly String[] TrueStrings = {
		"Y", "1", "yes", "true", Boolean.TrueString, nameof( Status.Success ), "good", nameof( Status.Go ), nameof( Status.Positive ), nameof( Status.Continue ),
		nameof( Status.Okay )
	};

	public static readonly Regex UpperCaseRegeEx = new( @"^[A-Z]+$", RegexOptions.Compiled, Minutes.One );

	public static Lazy<String> AllLetters { get; } = new( () =>
		   new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Where( Char.IsLetter ).Distinct().OrderBy( c => c ).ToArray() ) );

	/// <summary>The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.</summary>
	public static IEnumerable<String> UriRfc3986CharsToEscape { get; } = new[] {
		"!", "*", "'", "(", ")"
	};

	public static class English {

		[NotNull]
		public const String Numbers = "0123456789";

		public static String[] Consonants { get; } = "b,c,ch,cl,d,f,ff,g,gh,gl,j,k,l,ll,m,mn,n,p,ph,ps,r,rh,s,sc,sh,sk,st,t,th,v,w,x,y,z".Split( ',' );

		/// <summary></summary>
		public static String[] TensMap { get; } = {
			"zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"
		};

		/// <summary></summary>
		public static String[] UnitsMap { get; } = {
			"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
			"ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
		};

		public static String[] Vowels { get; } = "a,ai,au,e,ea,ee,i,ia,io,o,oa,oi,oo,ou,u".Split( ',' );

		public static class Alphabet {

			[NotNull]
			public const String Lowercase = "abcdefghijklmnopqrstuvwxyz";

			/// <summary>ABCDEFGHIJKLMNOPQRSTUVWXYZ</summary>
			[NotNull]
			public const String Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		}
	}

	public static class RegexPatterns {

		[NotNull]
		public const String MatchBlankLine = @"^\s*$";

		[NotNull]
		public const String MatchGitignoreCommentLine = @"^[#].*";

		[NotNull]
		public const String MatchMoney = @"//\$\s*[-+]?([0-9]{0,3}(,[0-9]{3})*(\.[0-9]+)?)";

		[NotNull]
		public const String SplitByEnglish = @"(?:\p{Lu}(?:\.\p{Lu})+)(?:,\s*\p{Lu}(?:\.\p{Lu})+)*";

		/// <summary>Regex pattern for words that don't start with a number</summary>
		[NotNull]
		public const String SplitByWordNotNumber = @"([a-zA-Z]\w+)\W*";
	}
}