// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ParsingExtensions.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "ParsingExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:28 AM.

namespace LibrainianCore.Parsing {

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
    using Collections.Extensions;
    using Extensions;
    using JetBrains.Annotations;
    using Linguistics;
    using Logging;
    using Maths.Numbers;
    using Measurement.Time;
    using Newtonsoft.Json;
    using Rationals;
    using Threading;

    public static class ParsingExtensions {

        [NotNull]
        private static String[] OrdinalSuffixes { get; } = {
            "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th"
        };

        /// <summary>this doesn't handle apostrophe well</summary>
        [NotNull]
        public static Lazy<Regex> RegexBySentenceNotworking { get; } = new Lazy<Regex>( () =>
            new Regex( @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])", RegexOptions.Compiled | RegexOptions.Multiline ) );

        [NotNull]
        public static Lazy<Regex> RegexBySentenceStackoverflow { get; } = new Lazy<Regex>( () => new Regex( "(?<Sentence>\\S.+?(?<Terminator>[.!?]|\\Z))(?=\\s+|\\Z)",
            RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled ) );

        [NotNull]
        public static Lazy<Regex> RegexByWordBreak { get; } = new Lazy<Regex>( () => new Regex( @"(?=\S*(?<=\w))\b", RegexOptions.Compiled | RegexOptions.Singleline ) );

        [NotNull]
        public static Lazy<Regex> RegexJustDigits { get; } = new Lazy<Regex>( () => new Regex( @"\D+", RegexOptions.Compiled | RegexOptions.Singleline ) );

        [NotNull]
        public static Char[] SplitBySpace { get; } = {
            Symbols.Singlespace[ 0 ]
        };

        /// <summary>Store the little array allocation for parsing so we don't have millions of GC on them.</summary>
        [NotNull]
        private static ConcurrentDictionary<String, String[]> Separators { get; } = new ConcurrentDictionary<String, String[]>();

        /// <summary>WHY?? For fun?</summary>
        [NotNull]
        public static Lazy<String> AllLowercaseLetters { get; } = new Lazy<String>( () =>
            new String( AllLetters().Where( Char.IsLower ).Distinct().OrderBy( c => c ).ToArray() ) );

        /// <summary>WHY?? For fun?</summary>
        [NotNull]
        public static Lazy<String> AllUppercaseLetters { get; } = new Lazy<String>( () =>
            new String( AllLetters().Where( Char.IsUpper ).Distinct().OrderBy( c => c ).ToArray() ) );

        [NotNull]
        private static Lazy<Regex> LowerUnderscore { get; } = new Lazy<Regex>( () => new Regex( @"([a-z\d])([A-Z])", RegexOptions.Compiled ) );

        [NotNull]
        private static Lazy<Regex> UpperToUnderscore { get; } = new Lazy<Regex>( () => new Regex( @"([A-Z]+)([A-Z][a-z])", RegexOptions.Compiled ) );

        [NotNull]
        private static Lazy<Regex> NoIdeaToUnderscore { get; } = new Lazy<Regex>( () => new Regex( @"[-\s]", RegexOptions.Compiled ) );

        private static Lazy<Regex> ForEnglishOnlyMethod { get; } = new Lazy<Regex>( () => new Regex( @"(\w+)|(\$\d+\.\d+)", RegexOptions.Compiled ) );

        [NotNull]
        [Pure]
        public static IEnumerable<Char> AllLetters() => ParallelEnumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Where( Char.IsLetter );

        /// <summary>Return <paramref name="self" />, up the <paramref name="maxlength" />.
        /// <para>Does not do any string trimming. Just truncate.</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="maxlength"></param>
        /// <returns></returns>
        [CanBeNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String Limit( [CanBeNull] this String self, Int32 maxlength ) => self?.Substring( 0, Math.Min( maxlength, self.Length ) );

        [CanBeNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String Left( [CanBeNull] this String self, Int32 count ) => self?.Substring( 0, Math.Min( count, self.Length ) );

        /// <summary>Does not Trim().</summary>
        /// <param name="self"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String Right( [CanBeNull] this String self, Int32 count ) {
            if ( String.IsNullOrEmpty( self ) || count <= 0 ) {
                return default;
            }

            var startIndex = self.Length - count;

            return startIndex > 0 ? self.Substring( startIndex, count ) : self;
        }

        /// <summary>Return <paramref name="self" />, up the <paramref name="maxlength" />.</summary>
        /// <param name="self"></param>
        /// <param name="maxlength"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String LimitAndTrim( [CanBeNull] this String self, Int32 maxlength ) => self?.Substring( 0, Math.Min( maxlength, self.Length ) ).TrimEnd();

        /// <summary>Add a single quote around <paramref name="self" /> string.</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String SingleQuote( [CanBeNull] this String self ) => $"'{self.Trimmed()}'";

        /// <summary>Add the left [ and right ] brackets if they're not already on the string.
        /// <para>An empty or whitepsace string returns an empty string.</para>
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String Bracket( [NotNull] this String self ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

            self = self.Trimmed();

            if ( String.IsNullOrEmpty( self ) ) {
                return String.Empty;
            }

            return
                $"{( self.StartsWith( "[", StringComparison.Ordinal ) ? String.Empty : "[" )}{self}{( self.EndsWith( "]", StringComparison.Ordinal ) ? String.Empty : "]" )}";
        }

        /// <summary>Trim the ToString() of the object; returning null if null, empty, or whitespace.</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String Trimmed<T>( [CanBeNull] this T self ) {
            switch ( self ) {
                case null: return default;
                case String s: return s.Trim().NullIfEmpty();
                default: return self.ToString().Trim().NullIfEmpty();
            }
        }

        [DebuggerStepThrough]
        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String DoubleQuote<T>( [CanBeNull] this T self ) => $"\"{self}\"";

        /// <summary>Return <paramref name="self" />, up the <paramref name="maxlength" />.
        /// <para>TODO faster? slower? Needs benchmarking.</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="maxlength"></param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String LimitAndTrimAlternate( [CanBeNull] this String self, Int32 maxlength ) =>
            self is null ?
                null :
                new StringBuilder( self ) {
                    Length = Math.Min( maxlength, self.Length )
                }.ToString().TrimEnd();

        /// <summary>Add dashes to a pascal-cased String</summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>String</returns>
        [NotNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String AddDashes( [NotNull] this String pascalCasedWord ) =>
            Regex.Replace( Regex.Replace( Regex.Replace( pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1-$2" ), @"([a-z\d])([A-Z])", "$1-$2" ), @"[\s]", "-" );

        [NotNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String AddSpacesBeforeUppercase( [NotNull] this String word ) {
            var sb = new StringBuilder( word.Length * 2 );

            foreach ( var c in word ) {
                if ( Char.IsUpper( c ) ) {
                    sb.Append( Symbols.Singlespace );
                }

                sb.Append( c );
            }

            return sb.ToString().Trim();
        }

        /// <summary>Add an undescore prefix to a pascasl-cased String</summary>
        /// <param name="pascalCasedWord"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static String AddUnderscorePrefix( [CanBeNull] this String pascalCasedWord ) => $"_{pascalCasedWord}";

        /// <summary>Add underscores to a pascal-cased String</summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>String</returns>
        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String AddUnderscores( [NotNull] this String pascalCasedWord ) =>
            NoIdeaToUnderscore.Value.Replace( LowerUnderscore.Value.Replace( UpperToUnderscore.Value.Replace( pascalCasedWord, "$1_$2" ), "$1_$2" ), Symbols.Underscore );

        [NotNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static String After( [NotNull] this String s, [NotNull] String splitter ) {
            if ( s is null ) {
                throw new ArgumentNullException( nameof( s ) );
            }

            if ( splitter is null ) {
                throw new ArgumentNullException( nameof( splitter ) );
            }

            return s.Substring( s.IndexOf( splitter, StringComparison.Ordinal ) + 1 ).TrimStart();
        }

        [NotNull]
        [Pure]
        public static String Append( [CanBeNull] this String result, [CanBeNull] String appendThis ) => $"{result ?? String.Empty}{appendThis ?? String.Empty}";

        /// <summary>Return an integer formatted as 1st, 2nd, 3rd, etc...</summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String AsOrdinal( this Int32 number ) {
            switch ( number % 100 ) {
                case 13:
                case 12:
                case 11:
                    return $"{number}th";
            }

#pragma warning disable IDE0066 // Convert switch statement to expression
            switch ( number % 10 ) {
#pragma warning restore IDE0066 // Convert switch statement to expression
                case 1: return $"{number}st";

                case 2: return $"{number}nd";

                case 3: return $"{number}rd";

                default: return $"{number}th";
            }
        }

        /// <summary>Return the substring from 0 to the index of the splitter.</summary>
        /// <param name="s">       </param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String Before( [NotNull] this String s, [NotNull] String splitter ) {
            if ( s is null ) {
                throw new ArgumentNullException( nameof( s ) );
            }

            if ( splitter is null ) {
                throw new ArgumentNullException( nameof( splitter ) );
            }

            return s.Substring( 0, s.IndexOf( splitter, StringComparison.Ordinal ) ).TrimEnd();
        }

        /// <summary>Adds 1 <paramref name="element" /> after <paramref name="sequence" /> and returns the new result (IEnumerable&lt;T &gt;).</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        [ItemCanBeNull]
        [Pure]
        public static IEnumerable<T> ConcatSingle<T>( [NotNull] this IEnumerable<T> sequence, [CanBeNull] T element ) {
            if ( sequence is null ) {
                throw new ArgumentNullException( nameof( sequence ) );
            }

            foreach ( var item in sequence ) {
                yield return item;
            }

            yield return element;
        }

        /// <summary>Returns the count of each letter in <paramref name="text" />.</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static IDictionary<Char, UInt64> Count( [NotNull] this String text ) {
            var dict = new ConcurrentDictionary<Char, UInt64>();
            text.AsParallel().WithMergeOptions( ParallelMergeOptions.AutoBuffered ).ForAll( c => dict.AddOrUpdate( c, 1, ( c1, arg2 ) => arg2 + 1 ) );

            return dict;
        }

        /// <summary>Returns the count of char <paramref name="character" /> in <paramref name="text" />.</summary>
        /// <param name="text"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        [Pure]
        public static UInt32 Count( [NotNull] this String text, Char character ) => ( UInt32 )text.Count( c => c == character );

        /// <summary>
        /// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of integers, where each integer represents the code point of a character in the
        /// source String. Includes an optional threshhold which can be used to indicate the maximum allowable distance.
        /// </summary>
        /// <param name="source">   An array of the code points of the first String</param>
        /// <param name="target">   An array of the code points of the second String</param>
        /// <param name="threshold">Maximum allowable distance</param>
        /// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
        [Pure]
        public static Int32 DamerauLevenshteinDistance( [NotNull] this String source, [NotNull] String target, Int32 threshold ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }

            if ( target is null ) {
                throw new ArgumentNullException( nameof( target ) );
            }

            var length1 = source.Length;
            var length2 = target.Length;

            // Return trivial case - difference in String lengths exceeds threshhold
            if ( Math.Abs( length1 - length2 ) > threshold ) {
                return Int32.MaxValue;
            }

            // Ensure arrays [i] / length1 use shorter length
            if ( length1 > length2 ) {
                CommonExtensions.Swap( ref target, ref source );
                CommonExtensions.Swap( ref length1, ref length2 );
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
                    var min = del > ins ? ins > sub ? sub : ins : del > sub ? sub : del;

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

        [Pure]
        public static Int32 EditDistanceParallel( [NotNull] this String s1, [NotNull] String s2 ) {
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
                        dist[ i, j ] = s1[ i - 1 ] == s2[ j - 1 ] ?
                            dist[ i - 1, j - 1 ] :
                            1 + Math.Min( dist[ i - 1, j ], Math.Min( dist[ i, j - 1 ], dist[ i - 1, j - 1 ] ) );
                    }
                }
            }, s1.Length, s2.Length, numBlocks, numBlocks );

            return dist[ s1.Length, s2.Length ];
        }

        /// <summary>
        ///     <para>Case insensitive String-end comparison.</para>
        ///     <para>( true example: cAt == CaT )</para>
        ///     <para>
        ///         <see cref="StringComparison.InvariantCultureIgnoreCase" />
        ///     </para>
        /// </summary>
        /// <param name="source"> </param>
        /// <param name="compare"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean EndsLike( [NotNull] this String source, [NotNull] String compare ) => source.EndsWith( compare, StringComparison.InvariantCultureIgnoreCase );

        [CanBeNull]
        [Pure]
        public static IEnumerable<Char> EnglishOnly( [NotNull] this String input ) {
            if ( String.IsNullOrEmpty( value: input ) ) {
                return input;
            }

            try {
                var sb = new StringBuilder();

                foreach ( Match m in ForEnglishOnlyMethod.Value.Matches( input ) ) {

                    sb.Append( m.Value );
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
        ///     The <see cref="Uri.EscapeDataString" /> method is <i>supposed</i> to take on RFC 3986 behavior if certain elements are present in a .config file. Even if this actually
        ///     worked (which in my experiments it <i>doesn't</i>), we can't rely on every host actually having this configuration element present.
        ///     </para>
        /// </remarks>
        [NotNull]
        [Pure]
        public static Uri EscapeUriDataStringRfc3986( [NotNull] this String value ) {

            // Start with RFC 2396 escaping by calling the .NET method to do the work. This MAY sometimes exhibit RFC 3986 behavior (according to the documentation). If it does, the escaping we do that follows it will be
            // a no-op since the characters we search for to replace can't possibly exist in the String.
            var escaped = new StringBuilder( Uri.EscapeDataString( value ) );

            // Upgrade the escaping to RFC 3986, if necessary.
            foreach ( var t in ParsingConstants.UriRfc3986CharsToEscape ) {
                escaped.Replace( t, Uri.HexEscape( t[ 0 ] ) );
            }

            // Return the fully-RFC3986-escaped String.

            return new Uri( escaped.ToString() );
        }

        [Pure]
        public static Boolean ExactMatch( [NotNull] this String source, [NotNull] String compare ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }

            if ( compare is null ) {
                throw new ArgumentNullException( nameof( compare ) );
            }

            if ( source.Length == 0 ) {
                return default;
            }

            if ( compare.Length == 0 ) {
                return default;
            }

            return source.SequenceEqual( compare );
        }

        [NotNull]
        [Pure]
        public static String FirstSentence( [CanBeNull] this String text ) {
            if ( text.IsNullOrWhiteSpace() ) {
                return String.Empty;
            }

            var sentences = text.ToSentences().FirstOrDefault();

            return sentences?.ToString() ?? String.Empty;
        }

        [NotNull]
        [Pure]
        public static String FirstWord( [CanBeNull] this String sentence ) => sentence.ToWords().FirstOrDefault() ?? String.Empty;

        /// <summary></summary>
        /// <param name="rational">      </param>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <seealso cref="http://kashfarooq.wordpress.com/2011/08/01/calculating-pi-in-c-part-3-using-the-net-4-bigrational-class/" />
        [NotNull]
        [Pure]
        public static String Format( this Rational rational, Int32 numberOfDigits ) {
            var numeratorShiftedToEnoughDigits = rational.Numerator * BigInteger.Pow( new BigInteger( 10 ), numberOfDigits );
            var bigInteger = numeratorShiftedToEnoughDigits / rational.Denominator;
            var toBeFormatted = bigInteger.ToString();
            var builder = new StringBuilder();
            builder.Append( toBeFormatted[ 0 ] );
            builder.Append( "." );
            builder.Append( toBeFormatted.Substring( 1, numberOfDigits - 1 ) );

            return builder.ToString();
        }

        /// <summary>Returns the decoded string, or <paramref name="text" /> if unable to convert.</summary>
        /// <param name="text"></param>
        /// <param name="encoding">Defaults to <see cref="Encoding.Unicode" /> and then <see cref="Encoding.UTF8" /></param>
        /// <seealso cref="ToBase64" />
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String FromBase64( [NotNull] this String text, Encoding encoding = null ) {
            Byte[] from64;

            try {
                from64 = Convert.FromBase64String( text );
            }
            catch ( Exception ) {
                return text;
            }

            try {
                if ( encoding is null ) {
                    encoding = Encoding.Unicode;
                }

                return encoding.GetString( from64 );
            }
            catch ( Exception ) {
                if ( Equals( Encoding.Unicode, encoding ) ) {
                    try {
                        return Encoding.UTF8.GetString( Convert.FromBase64String( text ) );
                    }
                    catch ( Exception ) { }
                }

                return text; //couldn't convert
            }
        }

        [NotNull]
        [Pure]
        public static String FullSoundex( this String s ) {

            // the encoding information
            //const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const String codes = "0123012D02245501262301D202";

            // i need a capitalized String
            s = s.ToUpper( CultureInfo.CurrentCulture );

            // i'm building the coded String using a String builder because i think this is probably the fastest and least intensive way
            var coded = new StringBuilder();

            // do the encoding
            foreach ( var index in s.Select( t => ParsingConstants.EnglishAlphabetUppercase.IndexOf( t ) ).Where( index => index >= 0 ) ) {
                coded.Append( codes[ index ] );
            }

            // okay, so here's how this goes . . . the first thing I do is assign the coded String so that i can regex replace on it

            // then i remove repeating characters
            //result = repeating.Replace(result, "$1");
            var result = new Regex( @"(\d)\1*D?\1+" ).Replace( coded.ToString(), "$1" ).Substring( 1 );

            // now i need to remove any characters coded as D from the front of the String because they're not really valid as the first code because they don't have an actual soundex code value
            result = new Regex( "^D+" ).Replace( result, String.Empty );

            // i used the char D to indicate that an h or w existed so that if to similar sounds were separated by an h or a w that I could remove one of them. if the h or w does not separate two similar sounds, then i
            // need to remove it now
            result = new Regex( "[D0]" ).Replace( result, String.Empty );

            // return the first character followed by the coded String
            return $"{s[ 0 ]}{result}";
        }

        /// <summary>Return possible variants of a name for name matching.</summary>
        /// <param name="input">  String to convert</param>
        /// <param name="culture">The culture to use for conversion</param>
        /// <returns>IEnumerable&lt;String&gt;</returns>
        [ItemNotNull]
        [Pure]
        public static IEnumerable<String> GetNameVariants( [CanBeNull] this String input, [CanBeNull] CultureInfo culture = null ) {
            if ( culture is null ) {
                culture = CultureInfo.CurrentCulture;
            }

            if ( String.IsNullOrEmpty( input ) ) {
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
            yield return input.AddUnderscorePrefix();

            // try name with underscore prefix, using camel case
            yield return input.ToCamelCase().AddUnderscorePrefix();
        }

        /// <summary>Add a space Before Each Capital Letter. then lowercase the whole string.
        /// <para>See also: <see cref="AddSpacesBeforeUppercase" /></para>
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String Humanize( [NotNull] this String word ) {
            if ( word is null ) {
                throw new ArgumentNullException( nameof( word ) );
            }

            return word.AddSpacesBeforeUppercase().ToLower( CultureInfo.CurrentUICulture );
        }

        /// <summary>.NET Char class already provides an static IsDigit method however it behaves differently depending on if char is a Latin or not.</summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean IsDigit( this Char c ) => c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9';

        [Pure]
        public static Boolean IsJustNumbers( [CanBeNull] this String text ) =>
            !( text is null ) && ( text.All( Char.IsNumber ) || Decimal.TryParse( text, out _ ) || Double.TryParse( text, out _ ) );

        [DebuggerStepThrough]
        [Pure]
        public static Boolean TryGetDecimal( [CanBeNull] this String text, out Decimal result ) => Decimal.TryParse( text ?? String.Empty, out result );

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsNullOrEmpty( [CanBeNull] this String value ) => String.IsNullOrEmpty( value );

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsNullOrWhiteSpace( [CanBeNull] this String value ) => String.IsNullOrWhiteSpace( value );

        /// <summary>Checks to see if a String is all uppper case</summary>
        /// <param name="inputString">String to check</param>
        /// <returns>Boolean</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsUpperCase( [NotNull] this String inputString ) => ParsingConstants.UpperCaseRegeEx.IsMatch( inputString );

        /// <summary>
        ///     <para>String sentence = "10 cats, 20 dogs, 40 fish and 1 programmer.";</para>
        ///     <para>
        ///     Should return:
        ///     <list type="">
        ///         <item>10</item> <item>20</item> <item>40</item> <item>1</item>
        ///     </list>
        ///     </para>
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static IEnumerable<String> JustDigits( [NotNull] this String sentence ) => RegexJustDigits.Value.Split( sentence );

        /// <summary>Example: String s = "123-123-1234".JustNumbers();</summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public static String JustNumbers( [CanBeNull] this String s ) {
            try {
                if ( s != null ) {
                    var sb = new StringBuilder( s.Length );
#pragma warning disable IDE0007 // Use implicit type
                    foreach ( Match m in Regex.Matches( s, "[0-9]" ) ) {
#pragma warning restore IDE0007 // Use implicit type
                        sb.Append( m.Value );
                    }

                    return sb.ToString();
                }
            }
            catch ( Exception error ) {
                error.Log();
            }

            return default;
        }

        /// <summary></summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        /// <see cref="Word" />
        /// <see cref="Sentence" />
        [NotNull]
        [Pure]
        public static IEnumerable<String> JustWords( [CanBeNull] this String sentence ) => sentence.ToWords().Where( word => word.Any( Char.IsLetterOrDigit ) );

        /// <summary>
        ///     <para>Case insensitive String comparison.</para>
        ///     <para>( for example: cAt == CaT is true )</para>
        ///     <para>( for example: CaT == CaT is true )</para>
        ///     <para>( Like(null, null ) is true )</para>
        ///     <para>
        ///         <see cref="StringComparison.OrdinalIgnoreCase" />
        ///     </para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean Like( [CanBeNull] this String left, [CanBeNull] String right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.Equals( right, StringComparison.OrdinalIgnoreCase );
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
        /// <returns></returns>
        [Pure]
        public static Boolean Same( [CanBeNull] this String left, [CanBeNull] String right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.Equals( right, StringComparison.Ordinal );
        }

        /// <summary>Convert the first letter of a String to lower case</summary>
        /// <param name="word">String to convert</param>
        /// <returns>String</returns>
        [NotNull]
        [Pure]
        public static String MakeInitialLowerCase( [NotNull] this String word ) {
            switch ( word.Length ) {
                case 0: return String.Empty;
                case 1: return Char.ToLower( word[ 0 ], CultureInfo.CurrentCulture ).ToString();
                default: return String.Concat( Char.ToLower( word[ 0 ], CultureInfo.CurrentCulture ).ToString(), word.Substring( 1 ) );
            }
        }

        /// <summary>Gets a <b>horribly</b> ROUGH guesstimate of the memory consumed by an object by using <see cref="JsonConvert" /> .</summary>
        /// <param name="bob"></param>
        /// <returns></returns>
        [Pure]
        public static Int64 MemoryUsed<T>( [CanBeNull] this T bob ) {
            if ( bob is null ) {
                throw new ArgumentNullException( nameof( bob ) );
            }

            try {
                return JsonConvert.SerializeObject( bob, Formatting.None ).LongCount();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return 0;
        }

        /// <summary>Returns null if <paramref name="self" /> is <see cref="String.IsNullOrWhiteSpace" />.</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [CanBeNull]
        [Pure]
        public static String NullIfBlank( [CanBeNull] this String self ) {
            if ( String.IsNullOrWhiteSpace( self ) ) {
                return null;
            }

            self = self.Trim();

            return String.IsNullOrEmpty( self ) ? default : self;
        }

        /// <summary>Returns null if <paramref name="self" /> is <see cref="String.IsNullOrEmpty" />.</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [CanBeNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        [Pure]
        public static String NullIfEmpty( [CanBeNull] this String self ) => String.IsNullOrEmpty( self ) ? null : self;

        /// <summary>Returns null if <paramref name="self" /> is <see cref="String.IsNullOrWhiteSpace" />.</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        [CanBeNull]
        [DebuggerStepThrough]
        [Pure]
        public static String NullIfEmptyOrWhiteSpace( [CanBeNull] this String self ) => String.IsNullOrWhiteSpace( self ) ? default : self;

        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        public static String NullIfJustNumbers( [CanBeNull] this String self ) => self.IsJustNumbers() ? default : self;

        [DebuggerStepThrough]
        [Pure]
        public static Int32 NumberOfDigits( this BigInteger number ) => number.ToString().Length;

        /// <summary>Repeats <paramref name="c" /> <paramref name="count" /> times.</summary>
        /// <param name="c"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String Repeat( this Char c, Int32 count ) => new String( c, count );

        /// <summary>Repeats the first char of the string <paramref name="self" /> <paramref name="count" /> times.</summary>
        /// <param name="self"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String Repeat( [CanBeNull] this String self, Int32 count ) => Enumerable.Repeat( self, count ).ToStrings( separator: null );

        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String RepeatFirstChar( [NotNull] this String self, Int32 count ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ) );
            }

            return new String( self[ 0 ], count );
        }

        /// <summary>Combine <paramref name="left" />, <paramref name="middlePadding" /> <paramref name="count" /> times, and the <paramref name="right" /> strings.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="middlePadding"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String PadMiddle( [NotNull] this String left, [CanBeNull] String right, Char middlePadding, Int32 count = 1 ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return $"{left}{new String( middlePadding, count )}{right}";
        }

     

        [NotNull]
        [Pure]
        public static String Prepend( [CanBeNull] this String self, [CanBeNull] String prependThis ) => $"{prependThis}{self}";

        [NotNull]
        [Pure]
        public static String Quoted( [CanBeNull] this String self ) => $"\"{self}\"";

        [NotNull]
        [Pure]
        public static String ReadToEnd( [NotNull] this MemoryStream ms ) {
            if ( ms is null ) {
                throw new ArgumentNullException( nameof( ms ) );
            }

            ms.Seek( 0, SeekOrigin.Begin );

            using ( var reader = new StreamReader( ms ) ) {
                return reader.ReadToEnd();
            }
        }

        [Pure]
        public static UInt64 RealLength( [CanBeNull] this String s ) {
            if ( String.IsNullOrEmpty( s ) ) {
                return 0;
            }

            var stringInfo = new StringInfo( s );

            return ( UInt64 )stringInfo.LengthInTextElements;
        }

        [NotNull]
        [Pure]
        public static String RemoveNullChars( [NotNull] this String text ) => text.Replace( "\0", String.Empty );

        /// <summary>Remove leading and trailing " from a string.</summary>
        /// <param name="input">String to parse</param>
        /// <returns>String</returns>
        [NotNull]
        [Pure]
        public static String RemoveSurroundingQuotes( this String input ) {
            if ( input.StartsWith( "\"", StringComparison.Ordinal ) && input.EndsWith( "\"", StringComparison.Ordinal ) ) {

                // remove leading/trailing quotes
                input = input.Substring( 1, input.Length - 2 );
            }

            return input;
        }

        /// <summary>Repeats the supplied string the specified number of times, putting the separator string between each repetition.</summary>
        /// <param name="self">       The extended string.</param>
        /// <param name="repetitions">The number of repetitions of the string to make. Must not be negative.</param>
        /// <param name="separator">  The separator string to place between each repetition. Must not be null.</param>
        /// <returns>The subject string, repeated n times, where n = repetitions. Between each repetition will be the separator string. If n is 0, this method will return String.Empty.</returns>
        [NotNull]
        [Pure]
        public static String Repeat( [NotNull] this String self, Int32 repetitions, [NotNull] String separator = "" ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "Repeat called on a null string." );
            }

            if ( separator is null ) {
                throw new ArgumentNullException( nameof( separator ) );
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

        [CanBeNull]
        [Pure]
        public static String ReplaceAll( this String haystack, [CanBeNull] String needle, [CanBeNull] String replacement ) {
            if ( String.IsNullOrEmpty( haystack ) || String.IsNullOrEmpty( needle ) ) {
                return haystack;
            }

            replacement = replacement.NullIfEmpty() ?? String.Empty;

            // Avoid a possible infinite loop
            if ( String.Equals( needle, replacement, StringComparison.Ordinal ) ) {
                return haystack;
            }

            Int32 pos;

            while ( ( pos = haystack.IndexOf( needle, StringComparison.Ordinal ) ) > 0 ) {
                haystack = $"{haystack.Substring( 0, pos )}{replacement}{haystack.Substring( pos + needle.Length )}";
            }

            return haystack;
        }

        [NotNull]
        [Pure]
        public static String ReplaceFirst( [NotNull] this String haystack, [NotNull] String needle, [CanBeNull] String replacement ) {
            var pos = haystack.IndexOf( needle, StringComparison.Ordinal );

            if ( pos < 0 ) {
                return haystack;
            }

            return haystack.Substring( 0, pos ) + replacement + haystack.Substring( pos + needle.Length );
        }

        [NotNull]
        [Pure]
        public static String ReplaceHTML( [NotNull] this String s, [NotNull] String withwhat ) => Regex.Replace( s, @"<(.|\n)*?>", withwhat );

        /// <summary>Reverse a String</summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String Reverse( [NotNull] this String s ) {
            var charArray = s.ToCharArray();
            Array.Reverse( charArray );

            return new String( charArray );
        }

        /// <summary></summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        /// <see cref="http://codereview.stackexchange.com/questions/78065/reverse-a-sentence-quickly-without-pointers" />
        [NotNull]
        [Pure]
        public static String ReverseWords( [NotNull] this String myString ) {
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

        /// <summary>Case sensitive ( <see cref="StringComparison.Ordinal" />) string comparison.</summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean Is( [CanBeNull] this String left, [CanBeNull] String right ) =>
            ( left ?? String.Empty ).Equals( right ?? String.Empty, StringComparison.Ordinal );

        /// <summary>Compute a Similarity between two strings. <br /> 1. 0 is a full, bit for bit match. <br /></summary>
        /// <param name="source">      </param>
        /// <param name="compare">     </param>
        /// <param name="timeout">     </param>
        /// <param name="matchReasons">preferably an empty queue</param>
        /// <returns></returns>
        /// <remarks>The score is normalized such that 0 equates to no similarity and 1 is an exact match.</remarks>
        [Pure]
        public static Double Similarity( [CanBeNull] this String source, [CanBeNull] String compare, [CanBeNull] ConcurrentQueue<String> matchReasons = null,
            TimeSpan? timeout = null ) {
            var similarity = new PotentialF( 0 );

            if ( source is null && compare is null ) {
                similarity.Add( 1 );

                return similarity;
            }

            if ( source is null ) {
                return similarity;
            }

            if ( compare is null ) {
                return similarity;
            }

            var stopwatch = Stopwatch.StartNew();

            if ( !timeout.HasValue ) {
                timeout = Minutes.One;
            }

            if ( !source.Any() || !compare.Any() ) {
                return similarity;
            }

            if ( source.ExactMatch( compare ) ) {
                matchReasons?.Add( "ExactMatch( source, compare )" );
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

            var sourceIntoUtf32Encoding = new UTF32Encoding( bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: false ).GetBytes( source );
            votes.ForA( sourceIntoUtf32Encoding.LongCount() );

            var compareIntoUtf32Encoding = new UTF32Encoding( bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: false ).GetBytes( compare );
            votes.ForB( compareIntoUtf32Encoding.LongCount() );

            // Test for exact same sequence
            if ( sourceIntoUtf32Encoding.SequenceEqual( compareIntoUtf32Encoding ) ) {
                votes.ForA( sourceIntoUtf32Encoding.Length );
                votes.ForB( compareIntoUtf32Encoding.Length );
                matchReasons?.Add( "exact match as UTF32 encoded" );

                return similarity;
            }

            if ( stopwatch.Elapsed > timeout ) {
                return similarity; //no more time for comparison
            }

            var compareReversed = compare.Reverse();

            if ( source.SequenceEqual( compareReversed ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length / 2.0 );
                matchReasons?.Add( "partial String reversal" );
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
                matchReasons?.Add( "exact match after Distinct()" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                return similarity; //no more time for comparison
            }

            if ( sourceDistinct.SequenceEqual( compareDistinctReverse ) ) {
                votes.ForA( sourceDistinct.Length * 2 );
                votes.ForB( compareDistinctReverse.Length );
                matchReasons?.Add( "exact match after Distinct()" );
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
                matchReasons?.Add( $"{tempcounter} characters found in compare from source" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                return similarity; //no more time for comparison
            }

            tempcounter = 0;

            foreach ( var c in compare ) {
                votes.ForB();

                if ( !source.Contains( c ) ) {
                    continue;
                }

                votes.ForA();
                tempcounter++;
            }

            if ( tempcounter > 0 ) {
                matchReasons?.Add( $"{tempcounter} characters found in compare from source" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                return similarity; //no more time for comparison
            }

            if ( source.Contains( compare ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length );
                matchReasons?.Add( "found compare String inside source String" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                return similarity; //no more time for comparison
            }

            if ( compare.Contains( source ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length );
                matchReasons?.Add( "found source String inside compare String" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                return similarity; //no more time for comparison
            }

            Single threshold = Math.Max( source.Length, compare.Length );
            var actualDamerauLevenshteinDistance = DamerauLevenshteinDistance( source: source, compare, threshold: ( Int32 )threshold );

            //TODO votes.ForB ???
            similarity.Add( threshold - actualDamerauLevenshteinDistance / threshold );

            if ( stopwatch.Elapsed > timeout ) {

                //TODO
            }

            //TODO

            return similarity;
        }

        [NotNull]
        [Pure]
        public static String Soundex( [NotNull] this String s, Int32 length = 4 ) {
            if ( s is null ) {
                throw new ArgumentNullException( nameof( s ) );
            }

            return FullSoundex( s ).PadRight( length, '0' ) // soundex is no shorter than
                .Substring( 0, length ); // and no longer than length
        }

        /// <summary>Same as calling <see cref="String.Split(String[], StringSplitOptions)" /> with an array of size 1 per <paramref name="separator" />.</summary>
        /// <param name="self">        The extended string.</param>
        /// <param name="separator">   The delimiter that splits substrings in the given string. Must not be null.</param>
        /// <param name="splitOptions">RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned.</param>
        /// <returns>See: <see cref="String.Split(String[], StringSplitOptions)" />.</returns>
        [NotNull]
        [Pure]
        public static IEnumerable<String> Split( [NotNull] this String self, [NotNull] String separator,
            StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "Split called on a null String." );
            }

            if ( separator is null ) {
                throw new ArgumentNullException( nameof( separator ) );
            }

            if ( !Separators.TryGetValue( separator, out var sep ) ) {
                sep = new[] {
                    separator
                };

                Separators[ separator ] = sep;
            }

            return self.Split( sep, splitOptions );
        }

        [NotNull]
        [Pure]
        public static IEnumerable<String> SplitToChunks( [NotNull] this String s, Int32 chunks ) {
            if ( s is null ) {
                throw new ArgumentNullException( nameof( s ) );
            }

            var res = Enumerable.Range( 0, s.Length ).Select( index => new {
                index,
                ch = s[ index ]
            } ).GroupBy( f => f.index / chunks ).Select( g => String.Join( "", g.Select( z => z.ch ) ) );

            return res;
        }

        [NotNull]
        [Pure]
        public static String StringFromResponse( [CanBeNull] this WebResponse response ) {
            using var restream = response?.GetResponseStream();

            if ( restream is null ) {
                return String.Empty;
            }

            using var reader = new StreamReader( restream );

            return reader.ReadToEnd();
        }

        [NotNull]
        [Pure]
        public static Byte[] StringToUtf32ByteArray( [NotNull] this String pXmlString ) => new UTF32Encoding().GetBytes( pXmlString );

        /// <summary>Converts the String to UTF8 Byte array and is used in De serialization</summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static Byte[] StringToUtf8ByteArray( [NotNull] this String pXmlString ) => new UTF8Encoding().GetBytes( pXmlString );

        [NotNull]
        [Pure]
        public static String StripHTML( [NotNull] this String s ) => Regex.Replace( s, @"<(.|\n)*?>", String.Empty ).Replace( "&nbsp;", " " );

        [NotNull]
        [Pure]
        public static String StripTags( [NotNull] this String input, [NotNull] String[] allowedTags ) {
            if ( allowedTags == null ) {
                throw new ArgumentNullException( nameof( allowedTags ) );
            }

            var stripHTMLExp = new Regex( @"(<\/?[^>]+>)" );
            var output = input;

            foreach ( Match tag in stripHTMLExp.Matches( input ) ) {
                var htmlTag = tag.Value.ToLower( CultureInfo.CurrentCulture );
                var isAllowed = false;

                foreach ( var allowedTag in allowedTags ) {
                    var offset = -1;

                    // Determine if it is an allowed tag "<tag>" , "<tag " and "</tag"
                    if ( offset != 0 ) {
                        offset = htmlTag.IndexOf( '<' + allowedTag + '>', StringComparison.Ordinal );
                    }

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

        [CanBeNull]
        [Pure]
        public static String StripTagsAndAttributes( [NotNull] this String input, String[] allowedTags ) {
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

        /// <summary>Just <see cref="String.Substring(Int32)" /> with a length check.</summary>
        /// <param name="s">    </param>
        /// <param name="count"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String Sub( [NotNull] this String s, Int32 count ) {
            var length = Math.Min( count, s.Length );

            return s.Substring( 0, length );
        }

        /// <summary>Performs the same action as <see cref="String.Substring(Int32)" /> but counting from the end of the string (instead of the start).</summary>
        /// <param name="self">    The extended string.</param>
        /// <param name="endIndex">The zero-based starting character position (from the end) of a substring in this instance.</param>
        /// <returns>Returns the original string with <paramref name="endIndex" /> characters removed from the end.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if endIndex is greater than the length of the string (or negative).</exception>
        [NotNull]
        [Pure]
        public static String SubstringFromEnd( [NotNull] this String self, Int32 endIndex ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "SubstringFromEnd called on a null string." );
            }

            if ( endIndex < 0 || endIndex > self.Length ) {
                throw new ArgumentOutOfRangeException( nameof( endIndex ) );
            }

            return self.Substring( 0, self.Length - endIndex );
        }

        /// <summary>Performs the same action as <see cref="String.Substring(Int32, Int32)" /> but counting from the end of the string (instead of the start).</summary>
        /// <param name="self">    The extended string.</param>
        /// <param name="endIndex">The zero-based starting character position (from the end) of a substring in this instance.</param>
        /// <param name="length">  The number of characters in the substring.</param>
        /// <returns>Returns <paramref name="length" /> characters of the subject string, counting backwards from <paramref name="endIndex" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if endIndex is greater than the length of the string (or negative).</exception>
        [NotNull]
        [Pure]
        public static String SubstringFromEnd( [NotNull] this String self, Int32 endIndex, Int32 length ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "SubstringFromEnd called on a null string." );
            }

            if ( endIndex < 0 || endIndex > self.Length ) {
                throw new ArgumentOutOfRangeException( nameof( endIndex ) );
            }

            return self.Substring( self.Length - endIndex - length, self.Length - endIndex );
        }

        /// <summary>Returns <paramref name="text" /> converted to a base-64 string.</summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <seealso cref="FromBase64" />
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String ToBase64( [CanBeNull] this String text, [CanBeNull] Encoding encoding = null ) {
            if ( encoding is null ) {
                encoding = Encoding.Unicode;
            }

            return Convert.ToBase64String( encoding.GetBytes( text ?? String.Empty ) );
        }

        /// <summary>Date plus Time</summary>
        /// <param name="when"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String ToLongDateTime( this DateTime when ) => when.ToLongDateString() + Symbols.Singlespace + when.ToLongTimeString();

        /// <summary>Converts a String to camel case</summary>
        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
        /// <returns>String</returns>
        [NotNull]
        [Pure]
        public static String ToCamelCase( [CanBeNull] this String lowercaseAndUnderscoredWord ) => MakeInitialLowerCase( ToPascalCase( lowercaseAndUnderscoredWord ) );

        /// <summary>Same as <see cref="AsOrdinal" />, but might be slightly faster performance-wise.</summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String ToOrdinal( this Int32 number ) {
            var n = Math.Abs( number );
            var lt = n % 100;

            return number + OrdinalSuffixes[ lt >= 11 && lt <= 13 ? 0 : n % 10 ];
        }

        /// <summary>Converts a String to pascal case with the option to remove underscores</summary>
        /// <param name="text">             String to convert</param>
        /// <param name="removeUnderscores">Option to remove underscores</param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String ToPascalCase( this String text, Boolean removeUnderscores = true ) {
            if ( String.IsNullOrEmpty( text ) ) {
                return String.Empty;
            }

            if ( removeUnderscores ) {
                text = text.Replace( Symbols.Underscore, Symbols.Singlespace );
            }

            text = text.Trimmed() ?? String.Empty;

            var sb = new StringBuilder( text.Length );

            var words = text.Split( SplitBySpace, StringSplitOptions.RemoveEmptyEntries );

            foreach ( var word in words ) {
                var w = word.TrimEnd();
                var l = w.Length;

                if ( l > 0 ) {
                    sb.Append( Char.ToUpper( w[ 0 ], CultureInfo.CurrentCulture ) );

                    if ( l > 1 ) {
                        sb.Append( w.Substring( 1 ) );
                    }
                }
            }

            return sb.ToString();
        }

        [NotNull]
        [Pure]
        public static IEnumerable<Sentence> ToSentences( [CanBeNull] this String paragraph ) {
            if ( paragraph is null ) {
                return Enumerable.Empty<Sentence>();
            }

            //clean it up some
            paragraph = paragraph.Replace( "\t", Symbols.Singlespace );

            do {
                paragraph = paragraph.Replace( ParsingConstants.Doublespace, Symbols.Singlespace );
            } while ( paragraph.Contains( ParsingConstants.Doublespace ) );

            paragraph = paragraph.Replace( "\n\n", Environment.NewLine );
            paragraph = paragraph.Replace( "\r\n", Environment.NewLine );
            paragraph = paragraph.Replace( "\r", Environment.NewLine );
            paragraph = paragraph.Replace( "\n", Environment.NewLine );

            //paragraph = paragraph.Replace( Environment.NewLine, Singlespace );

            while ( paragraph.Contains( ParsingConstants.Doublespace ) ) {
                paragraph = paragraph.Replace( oldValue: ParsingConstants.Doublespace, newValue: Symbols.Singlespace );
            }

            var results = RegexBySentenceStackoverflow.Value.Split( paragraph ).Select( s => s.Replace( Environment.NewLine, String.Empty ).Trimmed() )
                .Where( ts => !String.IsNullOrEmpty( ts ) && !ts.Equals( ".", StringComparison.Ordinal ) );

            return results.Select( Sentence.Parse );
        }

        /// <summary>Returns the wording of a number.</summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/a/2730393/956364" />
        [NotNull]
        [Pure]
        public static String ToVerbalWord( this Int32 number ) {
            if ( number == 0 ) {
                return "zero";
            }

            if ( number < 0 ) {
                return "minus " + ToVerbalWord( Math.Abs( number ) );
            }

            var words = String.Empty;

            if ( number / 1000000 > 0 ) {
                words += ToVerbalWord( number / 1000000 ) + " million ";
                number %= 1000000;
            }

            if ( number / 1000 > 0 ) {
                words += ToVerbalWord( number / 1000 ) + " thousand ";
                number %= 1000;
            }

            if ( number / 100 > 0 ) {
                words += ToVerbalWord( number / 100 ) + " hundred ";
                number %= 100;
            }

            if ( number <= 0 ) {
                return words;
            }

            if ( !String.IsNullOrEmpty( words ) ) {
                words += "and ";
            }

            if ( number < 20 ) {
                words += ParsingConstants.UnitsMap[ number ];
            }
            else {
                words += ParsingConstants.TensMap[ number / 10 ];

                if ( number % 10 > 0 ) {
                    words += "-" + ParsingConstants.UnitsMap[ number % 10 ];
                }
            }

            return words;
        }

        /// <summary></summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/a/7829529/956364" />
        [NotNull]
        [Pure]
        public static String ToVerbalWord( this Decimal number ) {
            if ( number == 0 ) {
                return "zero";
            }

            if ( number < 0 ) {
                return "minus " + ToVerbalWord( Math.Abs( number ) );
            }

            var intPortion = ( Int32 )number;
            var fraction = ( number - intPortion ) * 100;
            var decPortion = ( Int32 )fraction;

            var words = ToVerbalWord( intPortion );

            if ( decPortion <= 0 ) {
                return words;
            }

            words += " and ";
            words += ToVerbalWord( decPortion );

            return words;
        }

        [NotNull]
        [ItemNotNull]
        [Pure]
        public static String[] ToWords( [CanBeNull] this String sentence ) =>
            RegexByWordBreak.Value.Split( $"{Symbols.Singlespace}{sentence}{Symbols.Singlespace}" ).ToStrings( Symbols.Singlespace )
                .Split( SplitBySpace, StringSplitOptions.RemoveEmptyEntries );

        [CanBeNull]
        [Pure]
        public static String Truncate( [CanBeNull] this String s, Int32 maxLen ) {
            if ( maxLen < 0 ) {
                throw new ArgumentException( "Maximum length must be greater than 0.", nameof( maxLen ) );
            }

            if ( String.IsNullOrEmpty( s ) ) {
                return s;
            }

            return s.Length <= maxLen ? s : s.Substring( 0, maxLen );
        }

        /// <summary>To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.</summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        [NotNull]
        [Pure]
        public static String Utf8ByteArrayToString( [NotNull] this Byte[] characters ) => new UTF8Encoding().GetString( characters );

        /// <summary>Returns <paramref name="self" /> but culled to a maximum length of <paramref name="maxLength" /> characters.</summary>
        /// <param name="self">     The extended string.</param>
        /// <param name="maxLength">The maximum desired length of the string.</param>
        /// <returns>A string containing the first <c>Min(this.Length, maxLength)</c> characters from the extended string.</returns>
        [NotNull]
        [Pure]
        public static String WithMaxLength( [NotNull] this String self, Int32 maxLength ) {
            if ( self is null ) {
                throw new ArgumentNullException( nameof( self ), "WithMaxLength called on a null string." );
            }

            return self.Substring( 0, Math.Min( self.Length, maxLength ) );
        }

        /// <summary>
        ///     <para>Remove duplicate words ONLY if the previous word was the same word.</para>
        /// </summary>
        /// <example>Example: "My cat cat likes likes to to to eat food." Should become "My cat likes to eat food."</example>
        /// <param name="s"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        public static String RemoveDuplicateWords( [CanBeNull] String s ) {
            if ( String.IsNullOrEmpty( s ) ) {
                return String.Empty;
            }

            var words = s.ToWords();
            var result = new List<String>( words.Length );

            String previous = default;

            foreach ( var word in words ) {
                if ( !String.Equals( word, previous, StringComparison.CurrentCulture ) ) {
                    result.Add( word );
                }

                previous = word;
            }

            return result.ToStrings( Symbols.Singlespace );
        }

        /// <summary>Uses a <see cref="Regex" /> to count the number of words.</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Pure]
        public static Int32 WordCount( [NotNull] this String input ) {
            if ( input is null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            try {
                return Regex.Matches( input, @"[^\ ^\t^\n]+" ).Count;
            }
            catch ( Exception ) {
                return -1;
            }
        }

        [DebuggerStepThrough]
        [NotNull]
        [Pure]
        public static String Between( [NotNull] this String source, [NotNull] String left, [NotNull] String right ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }

            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }

            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }

            return Regex.Match( source, $"{left}(.*){right}" ).Groups[ 1 ].Value;
        }

        [NotNull]
        [DebuggerStepThrough]
        [Pure]
        public static String FormattedNice( this DateTime now ) => $"{now.Year}{now.Month:00}{now.Day:00}  {now.ToShortTimeString().Replace( ':', ';' )}";

        /// <summary>YearMonthDay HH;MM;ss</summary>
        /// <param name="now"></param>
        /// <returns></returns>
        [NotNull]
        [DebuggerStepThrough]
        [Pure]
        public static String FormattedNiceLong( this DateTime now ) => $"{now.Year}{now.Month:00}{now.Day:00}  {now.ToLongTimeString().Replace( ':', ';' )}";

        /// <summary>Modifies the <paramref name="memory" /> and makes the first letter capitalized.</summary>
        /// <param name="memory"></param>
        [DebuggerStepThrough]
        public static void Capitialize( this Memory<Char> memory ) {
            if ( memory.IsEmpty ) {
                return;
            }

            ref var first = ref memory.Span[ 0 ];
            first = Char.ToUpper( first, CultureInfo.CurrentCulture );
        }

        /// <summary>Returns the <paramref name="text" /> with the first letter capitalized.</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        public static String Capitialize( [CanBeNull] this String text ) {
            if ( String.IsNullOrEmpty( text ) ) {
                return default;
            }

            if ( text.Length == 1 ) {
                return Char.ToUpper( text[ 0 ], CultureInfo.CurrentCulture ).ToString();
            }

            return Char.ToUpper( text[ 0 ], CultureInfo.CurrentCulture ) + text.Substring( 1 );
        }

        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        public static String PluralOf( this BigInteger number, [CanBeNull] String singular ) {
            if ( String.IsNullOrEmpty( singular ) ) {
                return default;
            }

            if ( number == BigInteger.One ) {
                return singular;
            }

            return singular + "s";  //TODO find .NET Core plural nuget
        }

        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        public static String PluralOf( this Decimal number, [CanBeNull] String singular ) {
            if ( String.IsNullOrEmpty( singular ) ) {
                return default;
            }

            if ( number == 1 ) {
                return singular;
            }

            return singular + "s";  //TODO find .NET Core plural nuget
        }

        [DebuggerStepThrough]
        [CanBeNull]
        [Pure]
        public static String PluralOf( this Rational number, [CanBeNull] String singular ) {
            if ( String.IsNullOrEmpty( singular ) ) {
                return default;
            }

            if ( number == 1 ) {
                return singular;
            }

            return singular + "s";  //TODO find .NET Core plural nuget
        }


    }
}