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
// File "ParsingExtensions.cs" last touched on 2021-12-31 at 2:43 AM by Protiguous.

#nullable enable

namespace Librainian.Parsing;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Exceptions;
using ExtendedNumerics;
using LazyCache;
using Linguistics;
using Logging;
using Maths;
using Maths.Numbers;
using Microsoft.IO;
using Pluralize.NET;
using Rationals;
using Threading;
using Utilities;

public static class ParsingExtensions {

	[Flags]
	public enum CompareOptions {

		NullsAreNotEqual = 0b1,

		NullAreEqual = 0b10,

		IgnoreLeadingWhitespace = 0b100,

		IgnoreTrailingWhitespace = 0b1000,

		IgnoreAllWhitespace = 0b10000

	}

	public static readonly Lazy<Regex> RegexJustNumbers = new( () => new Regex( "[0-9]", RegexOptions.Compiled | RegexOptions.Singleline ) );

	private static Lazy<ParallelQuery<Char>> AllPossibleLetters { get; } =
		new( () => ParallelEnumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Where( Char.IsLetter ) );

	private static Lazy<Regex> ForEnglishOnlyMethod { get; } = new( () => new Regex( @"(\w+)|(\$\d+\.\d+)", RegexOptions.Compiled ) );

	internal static IAppCache? LazyCache { get; set; }

	private static Lazy<Regex> LowerUnderscore { get; } = new( () => new Regex( @"([a-z\d])([A-Z])", RegexOptions.Compiled ) );

	private static Lazy<Regex> NoIdeaToUnderscore { get; } = new( () => new Regex( @"[-\s]", RegexOptions.Compiled ) );

	private static String[] OrdinalSuffixes { get; } = {
		"th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th"
	};

	private static Lazy<Regex> SoundExPart1 { get; } = new( () => new Regex( @"(\d)\1*D?\1+", RegexOptions.Compiled ), true );

	private static Lazy<Regex> SoundExPart2 { get; } = new( () => new Regex( "^D+", RegexOptions.Compiled ), true );

	private static Lazy<Regex> SoundExPart3 { get; } = new( () => new Regex( "[D0]", RegexOptions.Compiled ), true );

	private static Lazy<Regex> UpperToUnderscore { get; } = new( () => new Regex( @"([A-Z]+)([A-Z][a-z])", RegexOptions.Compiled ) );

	private static Lazy<Pluralizer> WordPluralizer { get; } = new( () => new Pluralizer() );

	/// <summary>
	///     WHY?? For fun?
	/// </summary>
	public static Lazy<String> AllLowercaseLetters { get; } = new( () => new String( AllLetters().Where( Char.IsLower ).Distinct().OrderBy( c => c ).ToArray() ) );

	/// <summary>
	///     WHY?? For fun?
	/// </summary>
	public static Lazy<String> AllUppercaseLetters { get; } = new( () => new String( AllLetters().Where( Char.IsUpper ).Distinct().OrderBy( c => c ).ToArray() ) );

	/// <summary>
	///     this doesn't handle apostrophe well
	/// </summary>
	public static Lazy<Regex> RegexBySentenceNotworking { get; } =
		new( () => new Regex( @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])", RegexOptions.Compiled | RegexOptions.Multiline ) );

	public static Lazy<Regex> RegexBySentenceStackoverflow { get; } = new( () => new Regex( "(?<Sentence>\\S.+?(?<Terminator>[.!?]|\\Z))(?=\\s+|\\Z)",
		 RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline ) );

	public static Lazy<Regex> RegexByWordBreak { get; } = new( () => new Regex( @"(?=\S*(?<=\w))\b", RegexOptions.Compiled | RegexOptions.Singleline ) );

	public static Lazy<Regex> RegexJustDigits { get; } = new( () => new Regex( @"\D+", RegexOptions.Compiled | RegexOptions.Singleline ) );

	public static Char[] SplitBySpace { get; } = {
		ParsingConstants.Strings.Singlespace[ 0 ]
	};

	/// <summary>
	///     Add dashes to a pascal-cased String
	/// </summary>
	/// <param name="value">String to convert</param>
	/// <returns>String</returns>
	[NeedsTesting]
	public static String AddDashes( this String value ) =>
		Regex.Replace( Regex.Replace( Regex.Replace( value, @"([A-Z]+)([A-Z][a-z])", "$1-$2" ), @"([a-z\d])([A-Z])", "$1-$2" ), @"[\s]", "-" );

	[NeedsTesting]
	public static String AddSpacesBeforeUppercase( this String word ) {
		var l = word.Length * 2;

		var sb = new StringBuilder( l, l );

		foreach ( var c in word ) {
			if ( Char.IsUpper( c ) ) {
				sb.Append( ParsingConstants.Spaces.Space );
			}

			sb.Append( c );
		}

		return sb.ToString().Trim();
	}

	[NeedsTesting]
	public static String AddUnderscores( this String self ) =>
		NoIdeaToUnderscore.Value.Replace( LowerUnderscore.Value.Replace( UpperToUnderscore.Value.Replace( self, "$1_$2" ), "$1_$2" ), Symbols.Underscore );

	/// <summary>
	///     <para>Returns the rest of the string after <paramref name="splitter" />.</para>
	///     <para>If <paramref name="self" /> is null then null is returned.</para>
	///     <para>If <paramref name="self" /> is <see cref="String.Empty" />, then <see cref="String.Empty" /> is returned.</para>
	///     <para>
	///         If <paramref name="splitter" /> is null or <see cref="String.Empty" />, then <paramref name="self" /> is
	///         returned.
	///     </para>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="splitter"></param>
	/// <param name="comparison"></param>
	[NeedsTesting]
	public static String? After( this String? self, String? splitter, StringComparison comparison = StringComparison.Ordinal ) {
		if ( self is null ) {
			return default( String? );
		}

		if ( self.Length == 0 ) {
			return String.Empty;
		}

		if ( String.IsNullOrEmpty( splitter ) ) {
			return self;
		}

		return self[ ( self.IndexOf( splitter, comparison ) + splitter.Length ).. ];
	}

	/// <summary>
	///     Return list of all <see cref="Char" /> that are <see cref="Char.IsLetter(Char)" />
	/// </summary>
	[NeedsTesting]
	public static IEnumerable<Char> AllLetters() => AllPossibleLetters.Value;

	[NeedsTesting]
	public static String Append( this String? self, String? appendThis ) => $"{self ?? String.Empty}{appendThis ?? String.Empty}";

	/// <summary>
	///     Return an integer formatted as 1st, 2nd, 3rd, etc...
	/// </summary>
	/// <param name="number"></param>
	[NeedsTesting]
	public static String AsOrdinal( this Int32 number ) =>
		( number % 100 ) switch {
			13 => $"{number}th",
			12 => $"{number}th",
			11 => $"{number}th",
			var _ => ( number % 10 ) switch {
				1 => $"{number}st",
				2 => $"{number}nd",
				3 => $"{number}rd",
				var _ => $"{number}th"
			}
		};

	/// <summary>
	///     <para>Return the substring from 0 to the index of the splitter.</para>
	///     <para>If <paramref name="self" /> is null then null is returned.</para>
	///     <para>If <paramref name="self" /> is <see cref="String.Empty" />, then <see cref="String.Empty" /> is returned.</para>
	///     <para>
	///         If <paramref name="splitter" /> is null or <see cref="String.Empty" />, then <paramref name="self" /> is
	///         returned.
	///     </para>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="splitter"></param>
	/// <param name="comparison"></param>
	[NeedsTesting]
	public static String? Before( this String? self, String? splitter, StringComparison comparison = StringComparison.Ordinal ) {
		if ( self is null ) {
			return default( String? );
		}

		if ( self.Length == 0 ) {
			return String.Empty;
		}

		if ( splitter is null || splitter.Length == 0 ) {
			return self;
		}

		return self[ ..self.IndexOf( splitter, comparison ) ];
	}

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String Between( this String source, String left, String right ) {
		if ( source is null ) {
			throw new ArgumentEmptyException( nameof( source ) );
		}

		if ( left is null ) {
			throw new ArgumentEmptyException( nameof( left ) );
		}

		if ( right is null ) {
			throw new ArgumentEmptyException( nameof( right ) );
		}

		return Regex.Match( source, $"{left}(.*){right}" ).Groups[ 1 ].Value;
	}

	/// <summary>
	///     Add the left [ and right ] brackets if they're not already on the string.
	///     <para>An empty or whitepsace string returns an empty string.</para>
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String Bracket( this String? self ) {
		self = self.Trimmed();

		return String.IsNullOrEmpty( self ) ? String.Empty :
			$"{( self.StartsWith( ParsingConstants.Brackets.LeftBracket ) ? String.Empty : ParsingConstants.Brackets.LeftBracket )}{self}{( self.EndsWith( ParsingConstants.Brackets.RightBracket ) ? String.Empty : ParsingConstants.Brackets.RightBracket )}";
	}

	/// <summary>
	///     captalize the return word if the parameter is capitalized if word is "Table", then return "Tables"
	/// </summary>
	/// <param name="word"></param>
	/// <param name="action"></param>
	public static String Capitalize( this String word, Func<String, String> action ) {
		var result = action( word );

		if ( word.IsCapitalized() ) {
			if ( !result.Any() ) {
				return result;
			}

			var sb = new StringBuilder( result.Length );

			sb.Append( Char.ToUpperInvariant( result[ 0 ] ) );
			sb.Append( result[ 1.. ] );

			return sb.ToString();
		}

		return result;
	}

	/// <summary>
	///     Modifies the <paramref name="memory" /> and capitalizes the first letter.
	/// </summary>
	/// <param name="memory"></param>
	/// <param name="cultureInfo"></param>
	[DebuggerStepThrough]
	public static void Capitialize( this Memory<Char> memory, CultureInfo? cultureInfo = null ) {
		if ( memory.IsEmpty ) {
			return;
		}

		ref var first = ref memory.Span[ 0 ];
		first = Char.ToUpper( first, cultureInfo ?? CultureInfo.CurrentCulture );
	}

	/// <summary>
	///     Returns the <paramref name="text" /> with the first letter capitalized.
	/// </summary>
	/// <param name="text"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? Capitialize( this String? text ) {
		if ( String.IsNullOrEmpty( text ) ) {
			return default( String? );
		}

		if ( text.Length == 1 ) {
			return Char.ToUpper( text[ 0 ], CultureInfo.CurrentCulture ).ToString();
		}

		return Char.ToUpper( text[ 0 ], CultureInfo.CurrentCulture ) + text[ 1.. ];
	}

	/// <summary>
	///     Replaces all CR, LF, and tabs with spaces. And then replaces all double spaces with a single space.
	/// </summary>
	/// <param name="s"></param>
	/// <param name="comparison"></param>
	public static String Cleanup( this String? s, StringComparison comparison = StringComparison.OrdinalIgnoreCase ) {
		if ( s is null ) {
			return String.Empty;
		}

		while ( s.Contains( ParsingConstants.Chars.CR, comparison ) ) {
			s = s.Replace( ParsingConstants.Strings.CarriageReturn, ParsingConstants.Strings.Singlespace, comparison );
		}

		while ( s.Contains( ParsingConstants.Chars.LF, comparison ) ) {
			s = s.Replace( ParsingConstants.Strings.LineFeed, ParsingConstants.Strings.Singlespace, comparison );
		}

		while ( s.Contains( ParsingConstants.Strings.Tab, comparison ) ) {
			s = s.Replace( ParsingConstants.Strings.Tab, ParsingConstants.Strings.Singlespace, comparison );
		}

		while ( s.Contains( ParsingConstants.Strings.Doublespace, comparison ) ) {
			s = s.Replace( ParsingConstants.Strings.Doublespace, ParsingConstants.Strings.Singlespace, comparison );
		}

		return s;
	}

	/// <summary>
	///     Appends <paramref name="element" /> after <paramref name="sequence" />.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sequence"></param>
	/// <param name="element"></param>
	[NeedsTesting]
	public static IEnumerable<T?> Concat<T>( this IEnumerable<T> sequence, T? element ) {
		foreach ( var item in sequence ) {
			yield return item;
		}

		yield return element;
	}

	public static Boolean Contains( this String value, Char find, StringComparison comparisonType = StringComparison.Ordinal ) => value.Contains( find, comparisonType );

	public static Boolean Contains( this String value, String find, StringComparison comparisonType = StringComparison.Ordinal ) => value.Contains( find, comparisonType );

	/// <summary>
	///     Returns the count of each letter in <paramref name="text" />.
	/// </summary>
	/// <param name="text"></param>
	[NeedsTesting]
	public static IDictionary<Char, UInt64> Count( this String text ) {
		var dict = new ConcurrentDictionary<Char, UInt64>();
		text.AsParallel().WithMergeOptions( ParallelMergeOptions.AutoBuffered ).ForAll( c => dict.AddOrUpdate( c, 1, ( _, arg ) => arg + 1 ) );

		return dict;
	}

	/// <summary>
	///     Returns the count of char <paramref name="character" /> in <paramref name="text" />.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="character"></param>
	[NeedsTesting]
	public static UInt32 Count( this String text, Char character ) => ( UInt32 )text.Count( c => c == character );

	/// <summary>
	///     Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of integers, where each
	///     integer
	///     represents the code point of a character in the source String. Includes an optional threshhold which can be used to
	///     indicate the maximum allowable distance.
	/// </summary>
	/// <param name="source">An array of the code points of the first String</param>
	/// <param name="target">An array of the code points of the second String</param>
	/// <param name="threshold">Maximum allowable distance</param>
	/// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
	[NeedsTesting]
	public static Int32 DamerauLevenshteinDistance( this String source, String target, Int32 threshold ) {
		if ( source is null ) {
			throw new ArgumentEmptyException( nameof( source ) );
		}

		if ( target is null ) {
			throw new ArgumentEmptyException( nameof( target ) );
		}

		var length1 = source.Length;
		var length2 = target.Length;

		// Return trivial case - difference in String lengths exceeds threshhold
		if ( Math.Abs( length1 - length2 ) > threshold ) {
			return Int32.MaxValue;
		}

		// Ensure arrays [i] / length1 use shorter length
		if ( length1 > length2 ) {
			Common.Swap( ref target, ref source );
			Common.Swap( ref length1, ref length2 );
		}

		var maxi = length1;
		var maxj = length2;

		var dCurrent = new Int32[ maxi + 1 ];
		var dMinus1 = new Int32[ maxi + 1 ];
		var dMinus2 = new Int32[ maxi + 1 ];

		for ( var i = 0; i <= maxi; i++ ) {
			dCurrent[ i ] = i;
		}

		var jm1 = 0;

		for ( var j = 1; j <= maxj; j++ ) {
			// Rotate
			var dSwap = dMinus2;
			dMinus2 = dMinus1;
			dMinus1 = dCurrent;
			dCurrent = dSwap;

			// Initialize
			var minDistance = Int32.MaxValue;
			dCurrent[ 0 ] = j;
			var im1 = 0;
			var im2 = -1;

			for ( var i = 1; i <= maxi; i++ ) {
				var cost = source[ im1 ] == target[ jm1 ] ? 0 : 1;

				var del = dCurrent[ im1 ] + 1;
				var ins = dMinus1[ i ] + 1;
				var sub = dMinus1[ im1 ] + cost;

				//Fastest execution for min value of 3 integers
				var min = del > ins ? ins > sub ? sub : ins :
					del > sub ? sub : del;

				if ( i > 1 && j > 1 && source[ im2 ] == target[ jm1 ] && source[ im1 ] == target[ j - 2 ] ) {
					min = Math.Min( min, dMinus2[ im2 ] + cost );
				}

				dCurrent[ i ] = min;

				if ( min < minDistance ) {
					minDistance = min;
				}

				im1++;
				im2++;
			}

			jm1++;

			if ( minDistance > threshold ) {
				return Int32.MaxValue;
			}
		}

		var result = dCurrent[ maxi ];

		return result > threshold ? Int32.MaxValue : result;
	}

	/// <summary>
	///     Returns a double quoted (") prefix and suffix (if the <paramref name="self" /> is not already double quoted).
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String DoubleQuote<T>( this T? self ) {
		var value = self.Trimmed();
		if ( value?.StartsWith( ParsingConstants.Strings.DoubleQuote ) == true && value.EndsWith( ParsingConstants.Strings.DoubleQuote ) ) {
			return value;
		}

		return $"{ParsingConstants.Chars.DoubleQuote}{value}{ParsingConstants.Chars.DoubleQuote}";
	}

	[NeedsTesting]
	public static Int32 EditDistanceParallel( this String s1, String s2 ) {
		var dist = new Int32[ s1.Length + 1, s2.Length + 1 ];

		for ( var i = 0; i <= s1.Length; i++ ) {
			dist[ i, 0 ] = i;
		}

		for ( var j = 0; j <= s2.Length; j++ ) {
			dist[ 0, j ] = j;
		}

		var numBlocks = Environment.ProcessorCount * 4;

		ParallelAlgorithms.Wavefront( ( startI, endI, startJ, endJ ) => {
			for ( var i = startI + 1; i <= endI; i++ ) {
				for ( var j = startJ + 1; j <= endJ; j++ ) {
					dist[ i, j ] = s1[ i - 1 ] == s2[ j - 1 ] ? dist[ i - 1, j - 1 ] : 1 + Math.Min( dist[ i - 1, j ], Math.Min( dist[ i, j - 1 ], dist[ i - 1, j - 1 ] ) );
				}
			}
		}, s1.Length, s2.Length, numBlocks, numBlocks );

		return dist[ s1.Length, s2.Length ];
	}

	/// <summary>
	///     <para>Case insensitive String-end comparison.</para>
	///     <para>( true example: cAt == CaT )</para>
	///     <para>
	///         <see cref="StringComparison.OrdinalIgnoreCase" />
	///     </para>
	/// </summary>
	/// <param name="source"></param>
	/// <param name="compare"></param>
	[NeedsTesting]
	public static Boolean EndsLike( this String source, String compare ) => source.EndsWith( compare, StringComparison.OrdinalIgnoreCase );

	public static (Status status, String? end) EndsWith( this String value, IEnumerable<String> ofThese, StringComparison comparison = StringComparison.CurrentCulture ) {
		foreach ( var end in ofThese.Where( s => value.EndsWith( s, comparison ) ) ) {
			return (true.ToStatus(), end);
		}

		return (false.ToStatus(), default( String? ));
	}

	public static Boolean EndsWith( this String? text, Char value ) => !String.IsNullOrEmpty( text ) && text[ ^1 ] == value;

	[NeedsTesting]
	public static IEnumerable<Char>? EnglishOnly( this String input ) {
		try {
			var sb = new StringBuilder();
			var matches = ForEnglishOnlyMethod.Value.Matches( input );

			foreach ( var m in matches ) {
				if ( m is Match match ) {
					sb.Append( match.Value );
				}
			}

			return sb.ToString().Trimmed();
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return input;
	}

	/// <summary>
	///     <para>Escapes a String according to the URI data String rules given in RFC 3986.</para>
	/// </summary>
	/// <param name="value">The value to escape.</param>
	/// <returns>The escaped value.</returns>
	/// <see cref="http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" />
	/// <see cref="http://meyerweb.com/eric/tools/dencoder/" />
	/// <see cref="http://www.ietf.org/rfc/rfc2396.txt" />
	/// <see cref="http://msdn.microsoft.com/en-us/Library/vstudio/bb968786(v=vs.100).aspx" />
	/// <remarks>
	///     <para>
	///         The <see cref="Uri.EscapeDataString" /> method is <i>supposed</i> to take on RFC 3986 behavior if certain
	///         elements are
	///         present in a .config file. Even if this actually worked (which in my experiments it <i>doesn't</i>), we can't
	///         rely on
	///         every host actually having this configuration element present.
	///     </para>
	/// </remarks>
	[NeedsTesting]
	public static Uri EscapeUriDataStringRfc3986( this String value ) {
		// Start with RFC 2396 escaping by calling the .NET method to do the work. This MAY sometimes exhibit RFC 3986 behavior
		// (according to the documentation). If it does, the escaping we do that follows it will be a no-op since the
		// characters we search for to replace can't possibly exist in the String.
		var escaped = new StringBuilder( Uri.EscapeDataString( value ) );

		// Upgrade the escaping to RFC 3986, if necessary.
		foreach ( var t in ParsingConstants.UriRfc3986CharsToEscape ) {
			escaped.Replace( t, Uri.HexEscape( t[ 0 ] ) );
		}

		// Return the fully-RFC3986-escaped String.

		return new Uri( escaped.ToString() );
	}

	[NeedsTesting]
	public static Boolean ExactMatch( this String source, String compare ) {
		if ( source is null ) {
			throw new ArgumentEmptyException( nameof( source ) );
		}

		if ( compare is null ) {
			throw new ArgumentEmptyException( nameof( compare ) );
		}

		if ( source.Length == 0 ) {
			return false;
		}

		if ( compare.Length == 0 ) {
			return false;
		}

		return source.SequenceEqual( compare );
	}

	[NeedsTesting]
	public static Sentence? FirstSentence( this String? text ) {
		if ( String.IsNullOrWhiteSpace( text ) ) {
			return Sentence.Empty;
		}

		return text.ToSentences().FirstOrDefault();
	}

	[NeedsTesting]
	public static Word? FirstWord( this String? sentence ) => sentence.ToWords().FirstOrDefault();

	/// <param name="rational"></param>
	/// <param name="numberOfDigits"></param>
	/// <seealso
	///     cref="http://kashfarooq.wordpress.com/2011/08/01/calculating-pi-in-c-part-3-using-the-net-4-bigrational-class/" />
	[NeedsTesting]
	public static String Format( this Rational rational, Int32 numberOfDigits ) {
		var numeratorShiftedToEnoughDigits = rational.Numerator * BigInteger.Pow( new BigInteger( 10 ), numberOfDigits );
		var bigInteger = numeratorShiftedToEnoughDigits / rational.Denominator;
		var toBeFormatted = bigInteger.ToString();
		var builder = new StringBuilder();
		builder.Append( toBeFormatted[ 0 ] );
		builder.Append( '.' );
		builder.Append( toBeFormatted[ 1..numberOfDigits ] );

		return builder.ToString();
	}

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String FormattedNice( this DateTime now ) => $"{now.Year}{now.Month:00}{now.Day:00}  {now.ToShortTimeString().Replace( ':', ';' )}";

	/// <summary>
	///     YearMonthDay HH;MM;ss
	/// </summary>
	/// <param name="now"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String FormattedNiceLong( this DateTime now ) => $"{now.Year}{now.Month:00}{now.Day:00}  {now.ToLongTimeString().Replace( ':', ';' )}";

	/// <summary>
	///     Returns the decoded string, or <paramref name="text" /> if unable to convert.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="encoding">Defaults to <see cref="Encoding.Unicode" /> and then <see cref="Encoding.UTF8" /></param>
	/// <seealso cref="ToBase64" />
	[NeedsTesting]
	public static String FromBase64( this String text, Encoding? encoding = null ) {
		Byte[] from64;

		try {
			from64 = Convert.FromBase64String( text );
		}
		catch ( Exception ) {
			return text;
		}

		try {
			return ( encoding ?? Common.DefaultEncoding ).GetString( from64 );
		}
		catch ( Exception ) {
			if ( Equals( Encoding.Unicode, encoding ?? Common.DefaultEncoding ) ) {
				try {
					return Encoding.UTF8.GetString( Convert.FromBase64String( text ) );
				}
				catch ( Exception exception) {
					exception.Log( BreakOrDontBreak.DontBreak );
				}
			}

			return text; //couldn't convert
		}
	}

	/// <summary>
	///     Where did this function come from?
	/// </summary>
	/// <param name="s"></param>
	[NeedsTesting]
	public static String FullSoundex( this String s ) {
		// the encoding information
		//const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		const String codes = "0123012D02245501262301D202";

		// i need a capitalized String
		s = s.ToUpper( CultureInfo.CurrentCulture );

		// i'm building the coded String using a String builder because i think this is probably the fastest and least
		// intensive way
		var coded = new StringBuilder( s.Length * 2, s.Length * 2 );

		// do the encoding
		foreach ( var index in s.Select( t => ParsingConstants.English.Alphabet.Uppercase.IndexOf( t ) ).Where( index => index >= 0 ) ) {
			coded.Append( codes[ index ] );
		}

		// okay, so here's how this goes . . . the first thing I do is assign the coded String so that i can regex replace on it

		// then i remove repeating characters
		//result = repeating.Replace(result, "$1");
		//var result = new Regex( @"(\d)\1*D?\1+" ).Replace( coded.ToString(), "$1" ).Substring( 1 );
		var result = SoundExPart1.Value.Replace( coded.ToString(), "$1" )[ 1.. ];

		// now i need to remove any characters coded as D from the front of the String because they're not really valid as the first code because they don't have an actual soundex code value
		//result = new Regex( "^D+" ).Replace( result, Empty );
		result = SoundExPart2.Value.Replace( result, String.Empty );

		// i used the char D to indicate that an h or w existed so that if to similar sounds were separated by an h or a w that I could remove one of them. if the h or w does not separate two similar sounds, then i
		// need to remove it now
		//result = new Regex( "[D0]" ).Replace( result, Empty );
		result = SoundExPart3.Value.Replace( result, String.Empty );

		// return the first character followed by the coded String
		return $"{s[ 0 ]}{result}";
	}

	/// <summary>
	///     Return possible variants of a name for name matching.
	/// </summary>
	/// <param name="input">String to convert</param>
	/// <param name="culture">The culture to use for conversion</param>
	/// <returns>IEnumerable&lt;String&gt;</returns>
	[NeedsTesting]
	public static IEnumerable<String?> GetNameVariants( this String? input, CultureInfo? culture = null ) {
		culture ??= CultureInfo.CurrentCulture;

		if ( String.IsNullOrEmpty( input = input.Trimmed() ) ) {
			yield break;
		}

		yield return input;

		// try camel cased name
		yield return input.ToCamelCase();

		// try lower cased name
		yield return input.ToLower( culture );

		// try name with underscores
		yield return input.AddUnderscores();

		// try name with underscores with lower case
		yield return input.AddUnderscores().ToLower( culture );

		// try name with dashes
		yield return input.AddDashes();

		// try name with dashes with lower case
		yield return input.AddDashes().ToLower( culture );

		// try name with underscore prefix
		yield return input.PrefixWithUnderscore();

		// try name with underscore prefix, using camel case
		yield return input.ToCamelCase().PrefixWithUnderscore();
	}

	/// <summary>
	///     separate one combine word in to two parts, prefix word and the last word(suffix word)
	/// </summary>
	/// <param name="word"></param>
	/// <param name="prefixWord"></param>
	public static String GetSuffixWord( this String word, out String prefixWord ) {
		var lastSpaceIndex = word.LastIndexOf( ' ' );
		prefixWord = word[ ..( lastSpaceIndex + 1 ) ];

		return word[ ( lastSpaceIndex + 1 ).. ];
	}

	public static Boolean HasSomeWhiteSpace( this String? text ) =>
		text is not null && ( from c in text select c.ToString() ).Any( s => !String.IsNullOrEmpty( s ) && String.IsNullOrWhiteSpace( s ) );

	/// <summary>
	///     Add a space Before Each Capital Letter. then lowercase the whole string.
	///     <para>See also: <see cref="AddSpacesBeforeUppercase" /></para>
	/// </summary>
	/// <param name="word"></param>
	[NeedsTesting]
	public static String Humanize( this String word ) {
		if ( word is null ) {
			throw new ArgumentEmptyException( nameof( word ) );
		}

		return word.AddSpacesBeforeUppercase().ToLower( CultureInfo.CurrentUICulture );
	}

	/// <summary>
	///     Case sensitive ( <see cref="StringComparison.Ordinal" />) string comparison.
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[NeedsTesting]
	public static Boolean Is( this String? left, String? right ) => ( left ?? String.Empty ).Equals( right ?? String.Empty, StringComparison.Ordinal );

	public static Boolean IsAllWhiteSpace( this String? text ) => !String.IsNullOrEmpty( text ) && String.IsNullOrWhiteSpace( text );

	public static Boolean IsCapitalized( this String word ) => !String.IsNullOrEmpty( word ) && Char.IsUpper( word, 0 );

	/// <summary>
	///     .NET Char struct already provides an static IsDigit method however it behaves differently depending on if char is a
	///     Latin or not.
	/// </summary>
	/// <param name="c"></param>
	[NeedsTesting]
	public static Boolean IsDigit( this Char c ) => c is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9';

	[NeedsTesting]
	public static Boolean IsJustNumbers( this String? text ) =>
		text is not null && ( text.All( Char.IsNumber ) || Decimal.TryParse( text, out var _ ) || Double.TryParse( text, out var _ ) );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static Boolean IsNullOrEmpty( this String? value ) => String.IsNullOrEmpty( value );

	/// <summary>
	///     Checks to see if a String is all uppper case
	/// </summary>
	/// <param name="inputString">String to check</param>
	/// <returns>Boolean</returns>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static Boolean IsUpperCase( this String inputString ) => ParsingConstants.UpperCaseRegeEx.IsMatch( inputString );

	/// <summary>
	///     <para>String sentence = "10 cats, 20 dogs, 40 fish and 1 programmer.";</para>
	///     <para>
	///         Should return:
	///         <list type="">
	///             <item>10</item>
	///             <item>20</item>
	///             <item>40</item>
	///             <item>1</item>
	///         </list>
	///     </para>
	/// </summary>
	/// <param name="sentence"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static IEnumerable<String> JustDigits( this String sentence ) => RegexJustDigits.Value.Split( sentence );

	/// <summary>
	///     Example: String s = "123-123-1234".JustNumbers();
	/// </summary>
	/// <param name="s"></param>
	[NeedsTesting]
	public static String? JustNumbers( this String? s ) {
		if ( s is null ) {
			return default( String? );
		}

		var sb = new StringBuilder( s.Length, s.Length );

		foreach ( Match m in RegexJustNumbers.Value.Matches( s ) ) {
			if ( m != null ) {
				sb.Append( m.Value );
			}
		}

		return sb.ToString();
	}

	/// <summary>
	///     Returns the <paramref name="count" /> from the beginning of the string.
	///     <para>
	///         <code>"abc123".Left(3) == "abc"</code>
	///     </para>
	///     <para>Does not Trim().</para>
	///     <para>Null returns <see cref="String.Empty" /></para>
	/// </summary>
	/// <remarks>
	///     <seealso cref="LimitLength" />
	/// </remarks>
	[NeedsTesting]
	public static String? Left( this String? self, UInt32 count ) => self?[ ..( Int32 )Math.Min( count, ( UInt32 )self.Length ) ];

	/// <summary>
	///     <para>Case insensitive string comparison.</para>
	///     <para>( for example: cAt == CaT is true )</para>
	///     <para>( for example: CaT == CaT is true )</para>
	///     <para>( Like( null, null ) is false )</para>
	///     <para>Default comparison is <see cref="StringComparison.OrdinalIgnoreCase" /></para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="compareOptions"></param>
	/// <param name="comparison"></param>
	[NeedsTesting]
	public static Boolean Like(
		this String? left,
		String? right,
		CompareOptions compareOptions = CompareOptions.NullsAreNotEqual,
		StringComparison comparison = StringComparison.OrdinalIgnoreCase
	) {
		if ( compareOptions.HasFlag( CompareOptions.NullsAreNotEqual ) && left is null && right is null ) {
			return false;
		}

		if ( compareOptions.HasFlag( CompareOptions.NullAreEqual ) && left is null && right is null ) {
			return true;
		}

		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( compareOptions.HasFlag( CompareOptions.IgnoreLeadingWhitespace ) ) {
			left = left?.TrimStart();
			right = right?.TrimStart();
		}

		if ( compareOptions.HasFlag( CompareOptions.IgnoreTrailingWhitespace ) ) {
			left = left?.TrimEnd();
			right = right?.TrimEnd();
		}

		if ( compareOptions.HasFlag( CompareOptions.IgnoreAllWhitespace ) ) {
			left = left.StripAllWhitespace();
			right = right.StripAllWhitespace();
		}

		return String.Equals( left, right, comparison );
	}

	/// <summary>
	///     Return <paramref name="self" />, up the <paramref name="maxlength" />.
	/// </summary>
	/// <param name="self"></param>
	/// <param name="maxlength"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? LimitAndTrim( this String? self, Int32 maxlength ) => self?[ ..Math.Min( maxlength, self.Length ) ]?.TrimEnd();

	/// <summary>
	///     Return <paramref name="self" />, up the <paramref name="maxlength" />.
	///     <para>TODO faster? slower? Needs benchmarking compared to <see cref="LimitAndTrim" />.</para>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="maxlength"></param>
	[NeedsTesting]
	public static String? LimitAndTrimAlternate( this String? self, Int32 maxlength ) {
		if ( self is null ) {
			return default( String? );
		}

		var length = Math.Min( maxlength, self.Length );
		return new StringBuilder( self, length ) {
			Length = length
		}.ToString()
			 .TrimEnd();
	}

	/// <summary>
	///     Return <paramref name="self" />, up the <paramref name="maxlength" />.
	///     <para>Does not do any string trimming. Just truncate.</para>
	/// </summary>
	/// <remarks>
	///     <seealso cref="Left" />
	/// </remarks>
	/// <param name="self"></param>
	/// <param name="maxlength"></param>
	[NeedsTesting]
	public static String? LimitLength( this String? self, UInt32 maxlength ) => self?[ ..( Int32 )Math.Min( maxlength, ( UInt32 )self.Length ) ];

	/// <summary>
	///     Convert the first letter of a String to lower case
	/// </summary>
	/// <param name="word">String to convert</param>
	/// <param name="cultureInfo"></param>
	/// <returns>String</returns>
	[NeedsTesting]
	public static String MakeInitialLowerCase( this String word, CultureInfo? cultureInfo = null ) =>
		word.Length switch {
			0 => String.Empty,
			1 => Char.ToLower( word[ 0 ], cultureInfo ?? CultureInfo.CurrentCulture ).ToString(),
			var _ => String.Concat( Char.ToLower( word[ 0 ], cultureInfo ?? CultureInfo.CurrentCulture ).ToString(), word[ 1.. ] )
		};

	/// <summary>
	///     Returns null if <paramref name="self" /> is <see cref="String.IsNullOrWhiteSpace" />.
	/// </summary>
	/// <param name="self"></param>
	[NeedsTesting]
	public static String? NullIfBlank( this String? self ) {
		self = self?.Trim();

		return String.IsNullOrEmpty( self ) ? default( String? ) : self;
	}

	/// <summary>
	///     Returns null if <paramref name="self" /> is <see cref="String.IsNullOrEmpty" />.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? NullIfEmpty( this String? self ) => String.IsNullOrEmpty( self ) ? null : self;

	/// <summary>
	///     Set <paramref name="self" /> to null if <see cref="String.IsNullOrEmpty" />.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	public static void NullIfEmpty( ref String? self ) {
		if ( String.IsNullOrEmpty( self ) ) {
			self = null;
		}
	}

	/// <summary>
	///     Set <paramref name="self" /> to null if <see cref="String.IsNullOrWhiteSpace" />.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	public static void NullIfEmptyOrWhiteSpace( ref String? self ) {
		if ( String.IsNullOrWhiteSpace( self ) ) {
			self = null;
		}
	}

	/// <summary>
	///     Returns null if <paramref name="self" /> is <see cref="String.IsNullOrWhiteSpace" />.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? NullIfEmptyOrWhiteSpace( this String? self ) => String.IsNullOrWhiteSpace( self ) ? default( String? ) : self;

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? NullIfJustNumbers( this String? self ) => self.IsJustNumbers() ? default( String? ) : self;

	[DebuggerStepThrough]
	[NeedsTesting]
	public static Int32 NumberOfDigits( this BigInteger number ) => number.ToString().Length;

	public static String? OnlyDigits( this String? input ) => input == default( String? ) ? default( String? ) : String.Concat( input.Where( Char.IsDigit ) );

	/// <summary>
	///     Return only English alphabet letters in <paramref name="value" />.
	///     <remarks>Strip all non-English alphabet letters.</remarks>
	/// </summary>
	/// <param name="value"></param>
	public static String OnlyEnglishLetters( this String value ) {
		const String both = ParsingConstants.English.Alphabet.Uppercase + ParsingConstants.English.Alphabet.Lowercase;
		var sb = new StringBuilder( value.Length, value.Length );

		foreach ( var c in value.Where( c => both.Contains( c, StringComparison.CurrentCulture ) ) ) {
			sb.Append( c );
		}

		return sb.ToString();
	}

	public static String? OnlyLetters( String? input ) => input == default( String? ) ? default( String? ) : String.Concat( input.Where( Char.IsLetter ) );

	public static String? OnlyLettersAndNumbers( String? input ) =>
		input == default( String? ) ? default( String? ) : String.Concat( input.Where( c => Char.IsDigit( c ) || Char.IsLetter( c ) ) );

	/// <summary>
	///     Combine <paramref name="left" />, <paramref name="middlePadding" /><paramref name="count" /> times, and the
	///     <paramref
	///         name="right" />
	///     strings.
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="middlePadding"></param>
	/// <param name="count"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String PadMiddle( this String left, String? right, Char middlePadding, Int32 count = 1 ) {
		if ( left is null ) {
			throw new ArgumentEmptyException( nameof( left ) );
		}

		if ( right is null ) {
			throw new ArgumentEmptyException( nameof( right ) );
		}

		return $"{left}{new String( middlePadding, count )}{right}";
	}

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this BigInteger number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == BigInteger.One ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this Decimal number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == Decimal.One ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this Rational number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this Byte number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this Int32 number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this BigDecimal number, String word ) {
		if ( String.IsNullOrEmpty( word ) ) {
			return default( String? );
		}

		String key;

		if ( number == 1 ) {
			if ( LazyCache is null ) {
				return WordPluralizer.Value.Singularize( word );
			}

			key = Common.ToKey( nameof( PluralOf ), word, nameof( WordPluralizer.Value.Singularize ) );
			return LazyCache.GetOrAdd( key, () => WordPluralizer.Value.Singularize( word ) );

		}

		if ( LazyCache is null ) {
			return WordPluralizer.Value.Pluralize( word );
		}

		key = Common.ToKey( nameof( PluralOf ), word, nameof( WordPluralizer.Value.Pluralize ) );
		return LazyCache.GetOrAdd( key, () => WordPluralizer.Value.Pluralize( word ) );

	}

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this Int64 number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this UInt32 number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this UInt64 number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? PluralOf( this UInt256 number, String word ) =>
		String.IsNullOrEmpty( word ) ? default( String? ) :
		number == 1 ? WordPluralizer.Value.Singularize( word ) : WordPluralizer.Value.Pluralize( word );

	[NeedsTesting]
	public static String PrefixWithUnderscore( this String? self ) => $"{Symbols.Underscore}{self}";

	[NeedsTesting]
	public static String Prepend( this String? self, String? prependThis ) => $"{prependThis}{self}";

	[NeedsTesting]
	public static String Quoted( this String? self ) => $"\"{self}\"";

	[NeedsTesting]
	public static String ReadToEnd( this RecyclableMemoryStream ms ) {
		if ( ms is null ) {
			throw new ArgumentEmptyException( nameof( ms ) );
		}

		ms.Seek( 0, SeekOrigin.Begin );

		using var reader = new StreamReader( ms );

		return reader.ReadToEnd();
	}

	public static String RemoveFirstCharacter( this String self ) {
		if ( String.IsNullOrEmpty( self ) ) {
			return String.Empty;
		}

		return self[ 1.. ];
	}

	public static String RemoveLastCharacter( this String self ) {
		if ( String.IsNullOrEmpty( self ) ) {
			return String.Empty;
		}

		return self[ ..^1 ];
	}

	[NeedsTesting]
	public static String? RemoveNullChars( this String text ) => text.ReplaceAll( "\0", String.Empty );

	/// <summary>
	///     Remove leading and trailing " from a string.
	/// </summary>
	/// <param name="input">String to parse</param>
	/// <returns>String</returns>
	[NeedsTesting]
	public static String RemoveSurroundingQuotes( this String input ) {
		if ( input.StartsWith( "\"", StringComparison.Ordinal ) && input.EndsWith( "\"", StringComparison.Ordinal ) ) {
			// remove leading/trailing quotes
			input = input[ 1..^1 ];
		}

		return input;
	}

	/// <summary>
	///     Repeats <paramref name="c" /><paramref name="count" /> times.
	/// </summary>
	/// <param name="c"></param>
	/// <param name="count"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static String Repeat( this Char c, Int32 count ) => new( c, count );

	/// <summary>
	///     Repeats the first char of the string <paramref name="self" /><paramref name="count" /> times.
	/// </summary>
	/// <param name="self"></param>
	/// <param name="count"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String Repeat( this String? self, Int32 count ) => Enumerable.Repeat( self, count ).ToStrings( null );

	/// <summary>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="count"></param>
	/// <param name="index"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String RepeatAt( this String self, Int32 count, Int32 index = 0 ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		if ( !index.Between( 0, self.Length ) ) {
			throw new IndexOutOfRangeException( $"The index {index} is out of range (0 to {self.Length})." );
		}

		return new String( self[ index ], count );
	}

	/// <summary>
	///     Repeats the supplied string the specified number of times, putting the separator string between each repetition.
	/// </summary>
	/// <param name="self">The extended string.</param>
	/// <param name="repetitions">The number of repetitions of the string to make. Must not be negative.</param>
	/// <param name="separator">The separator string to place between each repetition. Must not be null.</param>
	/// <returns>
	///     The subject string, repeated n times, where n = repetitions. Between each repetition will be the separator string.
	///     If n
	///     is 0, this method will return String.Empty.
	/// </returns>
	[NeedsTesting]
	public static String RepeatString( this String self, Int32 repetitions, String separator = "" ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		if ( separator is null ) {
			throw new ArgumentEmptyException( nameof( separator ) );
		}

		if ( repetitions < 0 ) {
			throw new ArgumentOutOfRangeException( nameof( repetitions ), "Value must not be negative." );
		}

		if ( repetitions == 0 ) {
			return String.Empty;
		}

		var builder = new StringBuilder( self.Length * repetitions + separator.Length * ( repetitions - 1 ) );

		for ( var i = 0; i < repetitions; ++i ) {
			if ( i > 0 ) {
				builder.Append( separator );
			}

			builder.Append( self );
		}

		return builder.ToString();
	}

	/// <summary>
	///     <para>
	///         Replace all instances of the string <paramref name="needle" /> in the string <paramref name="haystack" /> with
	///         the
	///         string <paramref name="replacement" />.
	///     </para>
	///     <see cref="StringComparison.OrdinalIgnoreCase" /> by default.
	/// </summary>
	/// <param name="haystack"></param>
	/// <param name="needle"></param>
	/// <param name="replacement"></param>
	/// <param name="comparison"></param>
	[NeedsTesting]
	public static String? ReplaceAll( this String? haystack, String? needle, String? replacement, StringComparison comparison = StringComparison.Ordinal ) {
		if ( String.IsNullOrEmpty( haystack ) || String.IsNullOrEmpty( needle ) || String.IsNullOrEmpty( replacement ) ) {
			return haystack;
		}

		replacement = replacement.NullIfEmpty() ?? String.Empty;

		// Avoid a possible infinite loop
		if ( String.Equals( needle, replacement, comparison ) ) {
			return haystack;
		}

		var needleLength = needle.Length; //Does this actually need optimized? oh well.
		Int32 indexOf;

		while ( ( indexOf = haystack.IndexOf( needle, comparison ) ) > 0 ) {
			haystack = $"{haystack[ ..indexOf ]}{replacement}{haystack[ ( indexOf + needleLength ).. ]}";
		}

		return haystack;
	}

	[NeedsTesting]
	public static String ReplaceFirst( this String haystack, String needle, String? replacement, StringComparison comparison = StringComparison.Ordinal ) {
		var indexOf = haystack.IndexOf( needle, comparison );

		return indexOf < 0 ? haystack : $"{haystack[ ..indexOf ]}{replacement}{haystack[ ( indexOf + needle.Length ).. ]}";
	}

	[NeedsTesting]
	public static String ReplaceHTML( this String s, String withwhat ) => Regex.Replace( s, @"<(.|\n)*?>", withwhat );

	/// <summary>
	///     Given a string, "a1s2d3f4g5hj6j7", return the string "1234567".
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static String ReturnOnlyDigits( this String text ) {
		var sb = new StringBuilder( text.Length, text.Length );
		foreach ( var c in text.Where( c => c.IsDigit() ) ) {
			sb.Append( c );
		}

		return sb.ToString();
	}

	/// <summary>
	///     Reverse a String
	/// </summary>
	/// <param name="s"></param>
	[NeedsTesting]
	public static String Reverse( this String s ) {
		var charArray = s.ToCharArray();
		Array.Reverse( charArray );

		return new String( charArray );
	}

	/// <param name="myString"></param>
	/// <see cref="http://codereview.stackexchange.com/questions/78065/reverse-a-sentence-quickly-without-pointers" />
	[NeedsTesting]
	public static String ReverseWords( this String myString ) {
		var length = myString.Length;
		var tokens = new Char[ length ];
		var position = 0;
		Int32 lastIndex;

		for ( var i = length - 1; i >= 0; i-- ) {
			if ( myString[ i ] != ' ' ) {
				continue;
			}

			lastIndex = length - position;

			for ( var k = i + 1; k < lastIndex; k++ ) {
				tokens[ position ] = myString[ k ];
				position++;
			}

			tokens[ position ] = ' ';
			position++;
		}

		lastIndex = myString.Length - position;

		for ( var i = 0; i < lastIndex; i++ ) {
			tokens[ position ] = myString[ i ];
			position++;
		}

		return new String( tokens );
	}

	/// <summary>
	///     Returns the <paramref name="count" /> from the end of the string.
	///     <para>
	///         <code>"abc123".Right(3) == "123"</code>
	///     </para>
	///     <para>
	///         <remarks>If <paramref name="count" /> is greater then the length, the full string is returned.</remarks>
	///     </para>
	///     <para>Does not Trim().</para>
	///     <para>Null returns <see cref="String.Empty" /></para>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="count"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String Right( this String? self, Int32 count ) {
		Debug.Assert( count >= 0 );

		if ( String.IsNullOrEmpty( self ) || count <= 0 ) {
			return String.Empty;
		}

		var startIndex = self.Length - count;

		return self[ startIndex.. ];
	}

	/// <summary>
	///     Returns the <paramref name="count" /> from the end of the string.
	///     <para>
	///         <code>"abc123".Right(3) == "123"</code>
	///     </para>
	///     <para>
	///         <remarks>If <paramref name="count" /> is greater then the length, the full string is returned.</remarks>
	///     </para>
	///     <para>Does not Trim().</para>
	///     <para>Null returns <see cref="String.Empty" /></para>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="count"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static ReadOnlySpan<Char> Right( this ReadOnlySpan<Char> self, Int32 count ) {
		if ( count <= 0 || self.IsEmpty ) {
			Debug.Assert( count > 0 );
			Debug.Assert( self.Length >= 0 );
			return String.Empty;
		}

		var startIndex = self.Length - count;

		return self[ startIndex.. ];
	}

	/// <summary>
	///     <para>Case (ordinal) sensitive comparison.</para>
	///     <para>( for example: cAt == cAt is true )</para>
	///     <para>( for example: cAt == CaT is false )</para>
	///     <para>( Same(null, null ) is true )</para>
	///     <para>
	///         <see cref="StringComparison.Ordinal" />
	///     </para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <param name="compareOptions"></param>
	/// <param name="comparison"></param>
	[NeedsTesting]
	public static Boolean Same(
		this String? left,
		String? right,
		CompareOptions compareOptions = CompareOptions.NullsAreNotEqual,
		StringComparison comparison = StringComparison.Ordinal
	) =>
		left.Like( right, compareOptions, comparison );

	/// <summary>
	///     Compute a Similarity between two strings. <br /> 1. 0 is a full, bit for bit match. <br />
	/// </summary>
	/// <param name="source"></param>
	/// <param name="compare"></param>
	/// <param name="timeout"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="matchReasons">preferably an empty queue</param>
	/// <remarks>The score is normalized such that 0 equates to no similarity and 1 is an exact match.</remarks>
	[NeedsTesting]
	public static Double Similarity( this String? source, String? compare, TimeSpan timeout, CancellationToken cancellationToken, out ConcurrentQueue<String> matchReasons ) {
		var similarity = new PotentialF( 0 );
		matchReasons = new ConcurrentQueue<String>();

		switch ( source ) {
			case null when compare is null:

				similarity.Add( 1 );

				return similarity;

			case null:

				return similarity;
		}

		if ( compare is null ) {
			return similarity;
		}

		var stopwatch = Stopwatch.StartNew();

		if ( !source.Any() || !compare.Any() ) {
			return similarity;
		}

		if ( source.ExactMatch( compare ) ) {
			matchReasons.Enqueue( "ExactMatch( source, compare )" );
			similarity.Add( 1 );

			return similarity;
		}

		if ( source.SequenceEqual( compare ) ) {
			return similarity; //exact match. no more comparisons needed.
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		var votes = new VotallyD();

		votes.ForA( source.Length );
		votes.ForB( compare.Length );

		var sourceIntoUtf32Encoding = new UTF32Encoding( true, true, false ).GetBytes( source );

		// ReSharper disable once UseCollectionCountProperty
		votes.ForA( sourceIntoUtf32Encoding.Length );

		var compareIntoUtf32Encoding = new UTF32Encoding( true, true, false ).GetBytes( compare );
		votes.ForB( compareIntoUtf32Encoding.Length );

		// Test for exact same sequence
		if ( sourceIntoUtf32Encoding.SequenceEqual( compareIntoUtf32Encoding ) ) {
			votes.ForA( sourceIntoUtf32Encoding.Length );
			votes.ForB( compareIntoUtf32Encoding.Length );
			matchReasons.Enqueue( "exact match as UTF32 encoded" );

			return similarity;
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		var compareReversed = compare.Reverse();

		if ( source.SequenceEqual( compareReversed ) ) {
			votes.ForA( source.Length );
			votes.ForB( compare.Length / 2.0 );
			matchReasons.Enqueue( "partial String reversal" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		var sourceDistinct = new String( source.Distinct().ToArray() );
		var compareDistinct = new String( compare.Distinct().ToArray() );
		var compareDistinctReverse = new String( compareDistinct.Reverse().ToArray() );

		if ( sourceDistinct.SequenceEqual( compareDistinct ) ) {
			votes.ForA( sourceDistinct.Length );
			votes.ForB( compareDistinct.Length );
			matchReasons.Enqueue( "exact match after Distinct()" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		if ( sourceDistinct.SequenceEqual( compareDistinctReverse ) ) {
			votes.ForA( sourceDistinct.Length * 2 );
			votes.ForB( compareDistinctReverse.Length );
			matchReasons.Enqueue( "exact match after Distinct()" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		var tempcounter = 0;

		foreach ( var c in source ) {
			votes.ForA();

			if ( !compare.Contains( c ) ) {
				continue;
			}

			votes.ForB();
			tempcounter++;
		}

		if ( tempcounter > 0 ) {
			matchReasons.Enqueue( $"{tempcounter} characters found in compare from source" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		votes.ForB( compare.Length );

		if ( ( tempcounter = ( Int32 )votes.ForA( compare.Count( c => Contains( source, c, StringComparison.Ordinal ) ) ) ).Any() ) {
			matchReasons.Enqueue( $"{tempcounter} characters found in compare from source" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		if ( source.Contains( compare ) ) {
			votes.ForA( source.Length );
			votes.ForB( compare.Length );
			matchReasons.Enqueue( "found compare String inside source String" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		if ( compare.Contains( source ) ) {
			votes.ForA( source.Length );
			votes.ForB( compare.Length );
			matchReasons.Enqueue( "found source String inside compare String" );
		}

		if ( stopwatch.Elapsed > timeout ) {
			return similarity; //no more time for comparison
		}

		Single threshold = Math.Max( source.Length, compare.Length );
		var actualDamerauLevenshteinDistance = DamerauLevenshteinDistance( source, compare, ( Int32 )threshold );

		//TODO votes.ForB ???
		similarity.Add( threshold - actualDamerauLevenshteinDistance / threshold );

		if ( stopwatch.Elapsed > timeout ) {
			//TODO
		}

		//TODO more. letters near each other on keyboard

		return similarity;
	}

	/// <summary>
	///     Add a single quote around <paramref name="self" />.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String SingleQuote( this String? self ) => $"{ParsingConstants.Strings.SingleQuote}{self.Trimmed()}{ParsingConstants.Strings.SingleQuote}";

	/// <summary>
	///     Trim whitespace, and then add the left [ and right ] brackets if they're not already on the string.
	///     <para>An empty or whitepsace string throws <see cref="NullException" />.</para>
	/// </summary>
	/// <param name="self"></param>
	/// <exception cref="NullException"></exception>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String SmartBrackets( this String? self ) {
		self = self.Trimmed();

		if ( String.IsNullOrEmpty( self ) ) {
			throw new NullException( nameof( self ) );
		}

		return self.StartsWith( ParsingConstants.Brackets.LeftBracket ) || self.EndsWith( ParsingConstants.Brackets.RightBracket ) ? self :
			$"{ParsingConstants.Brackets.LeftBracket}{self}{ParsingConstants.Brackets.RightBracket}";
	}

	/// <summary>
	///     Add the left ` and/or right ” brackets if they're not already on the string.
	///     <para>An empty or whitepsace string throws <see cref="ArgumentEmptyException" />.</para>
	/// </summary>
	/// <param name="self"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String SmartQuote<T>( this T? self ) {
		var trimmed = self?.ToString().Trimmed();

		if ( String.IsNullOrEmpty( trimmed ) ) {
			trimmed = Symbols.Null;
		}

		if ( trimmed.Length < 2 ) {
			return $"{ParsingConstants.Chars.LeftDoubleQuote}{trimmed}{ParsingConstants.Chars.DoubleQuote}";
		}

		var starts = trimmed[ 0 ];
		var ends = trimmed[ ^1 ];

		if ( starts == ParsingConstants.Chars.SingleQuote && ends == ParsingConstants.Chars.SingleQuote ) {
			return $"{ParsingConstants.Chars.LeftDoubleQuote}{trimmed}{ParsingConstants.Chars.RightDoubleQuote}";
		}

		if ( starts == ParsingConstants.Chars.LeftSingleQuote && ends == ParsingConstants.Chars.RightSingleQuote ) {
			return $"{ParsingConstants.Chars.LeftDoubleQuote}{trimmed}{ParsingConstants.Chars.RightDoubleQuote}";
		}

		//TODO There are many more cases of how something needs to be properly quoted, such as quotes within quotes. And lefts or rights.

		return $"{ParsingConstants.Chars.LeftDoubleQuote}{trimmed}{ParsingConstants.Chars.RightDoubleQuote}";
	}

	[NeedsTesting]
	public static String Soundex( this String s, Int32 length = 4 ) {
		if ( s is null ) {
			throw new ArgumentEmptyException( nameof( s ) );
		}

		return FullSoundex( s ).PadRight( length, '0' )[ ..length ]; // and no longer than length
	}

	[NeedsTesting]
	public static IEnumerable<String> SplitToChunks( this String s, Int32 chunks ) {
		if ( s is null ) {
			throw new ArgumentEmptyException( nameof( s ) );
		}

		var res = Enumerable.Range( 0, s.Length )
							.Select( index => new {
								index,
								ch = s[ index ]
							} )
							.GroupBy( f => f.index / chunks )
							.Select( g => String.Join( "", g.Select( z => z.ch ) ) );

		return res;
	}

	/// <summary>
	///     Returns a <see cref="Status" /> if <paramref name="value" /> starts with any of <paramref name="ofThese" />.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="ofThese"></param>
	/// <param name="comparison"></param>
	public static (Status status, String? start) StartsWith( this String value, IEnumerable<String> ofThese, StringComparison comparison = StringComparison.CurrentCulture ) {
		foreach ( var start in ofThese.Where( s => value.StartsWith( s, comparison ) ) ) {
			return (true.ToStatus(), start);
		}

		return (false.ToStatus(), default( String? ));
	}

	[NeedsTesting]
	public static String StringFromResponse( this WebResponse response ) {
		using var restream = response.GetResponseStream();

		using var reader = new StreamReader( restream );

		return reader.ReadToEnd();
	}

	[NeedsTesting]
	public static Byte[] StringToUtf32ByteArray( this String pXmlString ) => new UTF32Encoding().GetBytes( pXmlString );

	/// <summary>
	///     Converts the String to UTF8 Byte array and is used in De serialization
	/// </summary>
	/// <param name="pXmlString"></param>
	[NeedsTesting]
	public static Byte[] StringToUtf8ByteArray( this String pXmlString ) => new UTF8Encoding().GetBytes( pXmlString );

	/// <summary>
	///     Given a string, "a1s2d3f4g5hj6j7", return the string "asdfghjj".
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	public static String StripAllDigits( this String text ) {
		if ( text is null ) {
			throw new ArgumentEmptyException( nameof( text ) );
		}

		var sb = new StringBuilder( text.Length, text.Length );
		foreach ( var c in text.Where( c => !c.IsDigit() ) ) {
			sb.Append( c );
		}

		return sb.ToString();
	}

	/// <summary>
	///     Strips ALL whitespace from <paramref name="value" />, returning <see cref="String.Empty" /> if needed.
	/// </summary>
	/// <param name="value"></param>
	[NeedsTesting]
	public static String StripAllWhitespace( this String? value ) {
		if ( value is null ) {
			return String.Empty;
		}

		StringBuilder sb = new( value.Length, value.Length );
		foreach ( var c in value.Where( c => !Char.IsWhiteSpace( c ) ) ) {
			sb.Append( c );
		}

		return sb.ToString();
	}

	[NeedsTesting]
	public static String StripHTML( this String s ) => Regex.Replace( s, @"<(.|\n)*?>", String.Empty ).Replace( "&nbsp;", " " );

	[NeedsTesting]
	public static String StripTags( this String input, String[] allowedTags ) {
		if ( allowedTags is null ) {
			throw new ArgumentEmptyException( nameof( allowedTags ) );
		}

		var stripHTMLExp = new Regex( @"(<\/?[^>]+>)" );
		var output = input;

		foreach ( Match tag in stripHTMLExp.Matches( input ) ) {
			if ( tag == null ) {
				continue;
			}

			var htmlTag = tag.Value.ToLower( CultureInfo.CurrentCulture );
			var isAllowed = false;

			foreach ( var allowedTag in allowedTags ) {
				// Determine if it is an allowed tag "<tag>" , "<tag " and "</tag"
				var offset = htmlTag.IndexOf( '<' + allowedTag + '>', StringComparison.Ordinal );

				if ( offset != 0 ) {
					offset = htmlTag.IndexOf( '<' + allowedTag + ' ', StringComparison.Ordinal );
				}

				if ( offset != 0 ) {
					offset = htmlTag.IndexOf( "</" + allowedTag, StringComparison.Ordinal );
				}

				// If it matched any of the above the tag is allowed
				if ( offset != 0 ) {
					continue;
				}

				isAllowed = true;

				break;
			}

			// Remove tags that are not allowed
			if ( !isAllowed ) {
				output = output.ReplaceFirst( tag.Value, "" );
			}
		}

		return output;
	}

	[NeedsTesting]
	public static String? StripTagsAndAttributes( this String input, String[] allowedTags ) {
		if ( allowedTags == null ) {
			throw new ArgumentEmptyException( nameof( allowedTags ) );
		}

		/* Remove all unwanted tags first */
		var output = input.StripTags( allowedTags );

		/* Lambda functions */
		static String HrefMatch( Match m ) => m.Groups[ 1 ].Value + "href..;,;.." + m.Groups[ 2 ].Value;

		static String ClassMatch( Match m ) => m.Groups[ 1 ].Value + "class..;,;.." + m.Groups[ 2 ].Value;

		static String UnsafeMatch( Match m ) => m.Groups[ 1 ].Value + m.Groups[ 4 ].Value;

		/* Allow the "href" attribute */
		output = new Regex( "(<a.*)href=(.*>)" ).Replace( output, HrefMatch );

		/* Allow the "class" attribute */
		output = new Regex( "(<a.*)class=(.*>)" ).Replace( output, ClassMatch );

		/* Remove unsafe attributes in any of the remaining tags */
		output = new Regex( @"(<.*) .*=(\'|\""|\w)[\w|.|(|)]*(\'|\""|\w)(.*>)" ).Replace( output, UnsafeMatch );

		/* Return the allowed tags to their proper form */
		output = output.ReplaceAll( "..;,;..", "=" );

		return output;
	}

	/// <summary>
	///     Just <see cref="String.Substring(Int32)" /> with a minimum length check.
	/// </summary>
	/// <param name="s"></param>
	/// <param name="count"></param>
	[NeedsTesting]
	public static String Sub( this String s, Int32 count ) => s[ ..Math.Min( count, s.Length ) ];

	/// <summary>
	///     Performs the same action as <see cref="String.Substring(Int32)" /> but counting from the end of the string (instead
	///     of
	///     the start).
	/// </summary>
	/// <param name="self">The extended string.</param>
	/// <param name="endIndex">The zero-based starting character position (from the end) of a substring in this instance.</param>
	/// <returns>Returns the original string with <paramref name="endIndex" /> characters removed from the end.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	///     Thrown if endIndex is greater than the length of the string (or negative).
	/// </exception>
	[NeedsTesting]
	public static String SubstringFromEnd( this String self, Int32 endIndex ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		if ( endIndex < 0 || endIndex > self.Length ) {
			throw new ArgumentOutOfRangeException( nameof( endIndex ) );
		}

		return self[ ..^endIndex ] ?? throw new NullException( nameof( self ) );
	}

	/// <summary>
	///     Performs the same action as <see cref="String.Substring(Int32, Int32)" /> but counting from the end of the string
	///     (instead of the start).
	/// </summary>
	/// <param name="self">The extended string.</param>
	/// <param name="endIndex">The zero-based starting character position (from the end) of a substring in this instance.</param>
	/// <param name="length">The number of characters in the substring.</param>
	/// <returns>
	///     Returns <paramref name="length" /> characters of the subject string, counting backwards from
	///     <paramref name="endIndex" />.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	///     Thrown if endIndex is greater than the length of the string (or negative).
	/// </exception>
	[NeedsTesting]
	public static String SubstringFromEnd( this String self, Int32 endIndex, Int32 length ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		if ( endIndex < 0 || endIndex > self.Length ) {
			throw new ArgumentOutOfRangeException( nameof( endIndex ) );
		}

		return self.Substring( self.Length - endIndex - length, self.Length - endIndex );
	}

	/// <summary>
	///     Returns <paramref name="text" /> converted to a base-64 string.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="encoding"></param>
	/// <seealso cref="FromBase64" />
	[NeedsTesting]
	public static String ToBase64( this String? text, Encoding? encoding = null ) =>
		Convert.ToBase64String( ( encoding ?? Common.DefaultEncoding ).GetBytes( text ?? String.Empty ) );

	/// <summary>
	///     Converts a String to camel case
	/// </summary>
	/// <param name="lowercaseAndUnderscoredWord">String to convert</param>
	/// <param name="cultureInfo"></param>
	/// <returns>String</returns>
	[NeedsTesting]
	public static String ToCamelCase( this String? lowercaseAndUnderscoredWord, CultureInfo? cultureInfo = null ) =>
		MakeInitialLowerCase( ToPascalCase( lowercaseAndUnderscoredWord, true, cultureInfo )!, cultureInfo ?? CultureInfo.CurrentCulture );

	/// <summary>
	///     Date plus Time
	/// </summary>
	/// <param name="when"></param>
	[NeedsTesting]
	public static String ToLongDateTime( this DateTime when ) => when.ToLongDateString() + ParsingConstants.Strings.Singlespace + when.ToLongTimeString();

	/// <summary>
	///     Same as <see cref="AsOrdinal" />, but might be slightly faster performance-wise.
	/// </summary>
	/// <param name="number"></param>
	[NeedsTesting]
	public static String ToOrdinal( this Int32 number ) {
		var n = Math.Abs( number );
		var lt = n % 100;

		return number + OrdinalSuffixes[ lt is >= 11 and <= 13 ? 0 : n % 10 ];
	}

	/// <summary>
	///     Converts a String to pascal case with the option to remove underscores
	/// </summary>
	/// <param name="text">String to convert</param>
	/// <param name="removeUnderscores">Option to remove underscores</param>
	/// <param name="cultureInfo"></param>
	[NeedsTesting]
	public static String? ToPascalCase( this String? text, Boolean removeUnderscores = true, CultureInfo? cultureInfo = null ) {
		if ( String.IsNullOrEmpty( text ) ) {
			return default( String? );
		}

		if ( removeUnderscores ) {
			text = text.Replace( Symbols.Underscore, ParsingConstants.Strings.Singlespace, false, cultureInfo ?? CultureInfo.CurrentCulture );
		}

		if ( String.IsNullOrEmpty( text = text.Trimmed() ) ) {
			return default( String? );
		}

		var sb = new StringBuilder( text.Length );

		foreach ( var word in text.Split( SplitBySpace, StringSplitOptions.RemoveEmptyEntries ) ) {
			var w = word.TrimEnd();
			var l = w.Length;

			if ( l > 0 ) {
				sb.Append( Char.ToUpper( w[ 0 ], cultureInfo ?? CultureInfo.CurrentCulture ) );

				if ( l > 1 ) {
					sb.Append( w[ 1.. ] );
				}
			}
		}

		return sb.ToString();
	}

	[NeedsTesting]
	public static IEnumerable<Sentence> ToSentences( this String? paragraph ) {
		if ( paragraph is null ) {
			foreach ( var _ in Enumerable.Empty<Sentence>() ) {
				yield break;
			}
		}

		var sentences = RegexBySentenceStackoverflow.Value.Split( paragraph! );

		foreach ( var sentence in sentences ) {
			var sen = Sentence.Parse( sentence.Cleanup() );
			if ( sen.Equals( Sentence.Empty ) ) {
				continue;
			}

			yield return sen;
		}

		/*
		paragraph = paragraph.Cleanup();
		paragraph = paragraph.Replace( ParsingConstants.Chars.CR.ToString(), Environment.NewLine );
		paragraph = paragraph.Replace( ParsingConstants.Chars.LF.ToString(), Environment.NewLine );

		return RegexBySentenceStackoverflow.Value.Split( paragraph ).Select( s => s.Replace( Environment.NewLine, String.Empty ).Trimmed() ?? String.Empty )
		                                   .Where( s => !String.IsNullOrEmpty( s ) && !s.Equals( ".", StringComparison.Ordinal ) ).Select( Sentence.Parse );
		*/
	}

	[NeedsTesting]
	public static IEnumerable<String> ToSplit( this String? sentence ) =>
		RegexByWordBreak.Value.Split( $"{ParsingConstants.Strings.Singlespace}{sentence}{ParsingConstants.Strings.Singlespace}" )
						.ToStrings( ParsingConstants.Strings.Singlespace )
						.Split( SplitBySpace, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

	[NeedsTesting]
	public static String ToStrings( this IEnumerable<Object?> list, Char separator, String? atTheEnd = null, Boolean? trimEnd = true ) {
		if ( list is null ) {
			throw new ArgumentEmptyException( nameof( list ) );
		}

		var joined = String.Join( separator, list.Where( o => o is not null ) );

		if ( trimEnd == true ) {
			return String.IsNullOrEmpty( atTheEnd ) ? joined.TrimEnd() : $"{joined.TrimEnd()}{separator}{atTheEnd}".TrimEnd();
		}

		return String.IsNullOrEmpty( atTheEnd ) ? joined : $"{joined}{separator}{atTheEnd}";
	}

	[NeedsTesting]
	public static String ToStrings( this IEnumerable<Object?> list, String separator, String? atTheEnd = null, Boolean? trimEnd = true ) {
		if ( list is null ) {
			throw new ArgumentEmptyException( nameof( list ) );
		}

		var joined = String.Join( separator, list.Where( o => o is not null ) );

		if ( trimEnd == true ) {
			return String.IsNullOrEmpty( atTheEnd ) ? joined.TrimEnd() : $"{joined.TrimEnd()}{separator}{atTheEnd}".TrimEnd();
		}

		return String.IsNullOrEmpty( atTheEnd ) ? joined : $"{joined}{separator}{atTheEnd}";
	}

	/// <summary>
	///     <para>Returns a String with the <paramref name="separator" /> between each item of an <paramref name="list" />.</para>
	///     <para>If no separator is given, it defaults to ", ".</para>
	///     <para>Additonally, <paramref name="atTheEnd" /> can optionally be added to the returned string.</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="separator">Defaults to ", ".</param>
	/// <param name="atTheEnd"></param>
	/// <param name="trimEnd"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String ToStrings<T>( this IEnumerable<T> list, String? separator = ", ", String? atTheEnd = null, Boolean? trimEnd = true ) {
		if ( list is null ) {
			throw new ArgumentEmptyException( nameof( list ) );
		}

		var joined = String.Join( separator, list.Where( o => o is not null ) );

		if ( trimEnd == true ) {
			return String.IsNullOrEmpty( atTheEnd ) ? joined.TrimEnd() : $"{joined.TrimEnd()}{separator}{atTheEnd}".TrimEnd();
		}

		return String.IsNullOrEmpty( atTheEnd ) ? joined : $"{joined}{separator}{atTheEnd}";
	}

	/// <summary>
	///     Returns the wording of a number.
	/// </summary>
	/// <param name="number"></param>
	/// <see cref="http://stackoverflow.com/a/2730393/956364" />
	[NeedsTesting]
	public static String ToVerbalWord( this Int64 number ) {
		if ( number == 0 ) {
			return "zero";
		}

		if ( number < 0 ) {
			return "minus " + Math.Abs( number ).ToVerbalWord();
		}

		var words = String.Empty;

		var test = number / 1_000_000_000_000;

		if ( test > 0 ) {
			words += test.ToVerbalWord() + " trillion ";
			number %= 1_000_000_000_000;
		}

		test = number / 1_000_000_000;

		if ( test > 0 ) {
			words += test.ToVerbalWord() + " billion ";
			number %= 1_000_000_000;
		}

		test = number / 1_000_000;

		if ( test > 0 ) {
			words += test.ToVerbalWord() + " million ";
			number %= 1_000_000;
		}

		test = number / 1_000;

		if ( test > 0 ) {
			words += test.ToVerbalWord() + " thousand ";
			number %= 1000;
		}

		test = number / 100;

		if ( test > 0 ) {
			words += test.ToVerbalWord() + " hundred ";
			number %= 100;
		}

		if ( number <= 0 ) {
			return words.Trim();
		}

		if ( !String.IsNullOrEmpty( words ) ) {
			words += "and ";
		}

		if ( number < 20 ) {
			words += ParsingConstants.English.UnitsMap[ number ];
		}
		else {
			words += ParsingConstants.English.TensMap[ number / 10 ];

			if ( number % 10 > 0 ) {
				words += "-" + ParsingConstants.English.UnitsMap[ number % 10 ];
			}
		}

		return words.Trim();
	}

	/// <param name="number"></param>
	/// <see cref="http://stackoverflow.com/a/7829529/956364" />
	[NeedsTesting]
	public static String ToVerbalWord( this Decimal number ) {
		switch ( number ) {
			case Decimal.Zero:

				return "zero";

			case < Decimal.Zero:

				return "minus " + ToVerbalWord( Math.Abs( number ) );
		}

		var intPortion = ( Int32 )number;
		var fraction = ( number - intPortion ) * 100;
		var decPortion = ( Int32 )fraction; //TODO eh?

		var words = ( ( Decimal )intPortion ).ToVerbalWord();

		if ( decPortion <= 0 ) {
			return words.Trim();
		}

		words += " and ";
		words += ( ( Decimal )decPortion ).ToVerbalWord();

		return words.Trim();
	}

	[NeedsTesting]
	public static IEnumerable<Word> ToWords( this String? sentence ) => sentence.ToSplit().Select( s => new Word( s ) );

	/// <summary>
	///     //TODO This function needs unit tests.
	/// </summary>
	/// <remarks>
	///     <code>const String test = "   Hello, World! ";</code>
	///     <code>Console.WriteLine( ParsingExtensionsToo.Trim( test.ToCharArray() ).ToArray() );</code>
	/// </remarks>
	/// <param name="source"></param>
	public static Span<Char> Trim( this Span<Char> source ) {
		if ( source.IsEmpty ) {
			return source;
		}

		Int32 start = 0, end = source.Length - 1;

		Char startChar = source[ start ], endChar = source[ end ];

		while ( start < end && ( startChar == ' ' || endChar == ' ' ) ) {
			if ( startChar == ' ' ) {
				start++;
			}

			if ( endChar == ' ' ) {
				--end;
			}

			startChar = source[ start ];
			endChar = source[ end ];
		}

		return source.Slice( start, 1 + ( end - start ) );
	}

	/// <summary>
	/// </summary>
	/// <param name="self"></param>
	/// <param name="startingChar"></param>
	/// <param name="comparison"></param>
	public static String? TrimLeading( this String? self, String startingChar = ".", StringComparison comparison = StringComparison.Ordinal ) {
		self = self.Trimmed();

		while ( self?.StartsWith( startingChar, comparison ) == true ) {
			self = self[ startingChar.Length.. ].TrimStart();
		}

		return self;
	}

	/// <summary>
	///     Remove the first char and the last char.
	///     <para>Does not perform any length or range checks.</para>
	///     <para>Will return null if empty.</para>
	/// </summary>
	/// <param name="self"></param>
	public static String? TrimLeftAndRightChar( this String self ) => self[ 1..^1 ].Trimmed();

	/// <summary>
	///     Trim the ToString() of the object; returning null if null, empty, or whitespace.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? Trimmed<T>( this T? self ) =>
		self switch {
			null => default( String? ),
			String s => s.Trim().NullIfEmpty(),
			var _ => self.ToString()?.Trim().NullIfEmpty()
		};

	/// <summary>
	///     Trim the ToString() of the object; returning null if null, empty, or whitespace.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	[NeedsTesting]
	public static String? TrimmedAlt<T>( this T? self ) =>
		self switch {
			String s => s.Trim().NullIfEmpty(),
			null => null,
			var _ => self.ToString()?.Trim().NullIfEmpty()
		};

	/// <summary>
	///     Set <paramref name="self" /> to null if null, empty, or whitespace.
	/// </summary>
	/// <param name="self"></param>
	[DebuggerStepThrough]
	public static void TrimString( ref String? self ) {
		self = self?.Trim();
		NullIfEmpty( ref self );
	}

	public static ReadOnlySpan<Char> TrimWhiteSpaceAndNull( this ReadOnlySpan<Char> value ) {
		var start = 0;

		while ( start < value.Length ) {
			if ( Char.IsWhiteSpace( value[ start ] ) || value[ start ] == ParsingConstants.Chars.NullChar ) {
				start++;
			}
			else {
				break;
			}
		}

		var end = value.Length - 1;

		while ( end >= start ) {
			if ( Char.IsWhiteSpace( value[ end ] ) || value[ end ] == ParsingConstants.Chars.NullChar ) {
				end--;
			}
			else {
				break;
			}
		}

		return value.Slice( start, end - start + 1 );
	}

	/// <summary>
	///     Needs unit tests.
	/// </summary>
	/// <param name="s"></param>
	/// <param name="maximumLength"></param>
	[NeedsTesting]
	public static String? Truncate( this String? s, UInt32 maximumLength ) {
		if ( String.IsNullOrEmpty( s ) ) {
			return s;
		}

		return ( UInt32 )s.Length <= maximumLength ? s : s.AsMemory()[ ..( Int32 )maximumLength ].ToString();
	}

	[DebuggerStepThrough]
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean TryGetDecimal( this String? text, out Decimal result ) => Decimal.TryParse( text, out result );

	/// <summary>
	///     To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
	/// </summary>
	/// <param name="characters">Unicode Byte Array to be converted to String</param>
	/// <returns>String converted from Unicode Byte Array</returns>
	[NeedsTesting]
	public static String Utf8ByteArrayToString( this Byte[] characters ) => new UTF8Encoding().GetString( characters );

	/// <summary>
	///     Returns <paramref name="self" /> but culled to a maximum length of <paramref name="maxLength" /> characters.
	/// </summary>
	/// <param name="self">The extended string.</param>
	/// <param name="maxLength">The maximum desired length of the string.</param>
	/// <returns>A string containing the first <c>Min(this.Length, maxLength)</c> characters from the extended string.</returns>
	[NeedsTesting]
	public static String WithMaxLength( this String self, Int32 maxLength ) {
		if ( self is null ) {
			throw new ArgumentEmptyException( nameof( self ) );
		}

		return self[ ..Math.Min( self.Length, maxLength ) ] ?? throw new NullException( nameof( self ) );
	}

	/// <summary>
	///     Uses a <see cref="Regex" /> to count the number of words.
	///     <para>Returns -1 upon any exception.</para>
	/// </summary>
	/// <param name="input"></param>
	[NeedsTesting]
	public static Int32 WordCount( this String input ) {
		if ( input is null ) {
			throw new ArgumentEmptyException( nameof( input ) );
		}

		try {
			return Regex.Matches( input, @"[^\ ^\t^\n]+" ).Count;
		}
		catch ( Exception ) {
			return -1;
		}
	}


}