#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/ParsingExtensions.cs" was last cleaned by Rick on 2014/09/06 at 7:29 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using Annotations;
    using Collections;
    using Extensions;
    using IO;
    using Linguistics;
    using Maths;
    using Numerics;
    using Threading;

    public static class ParsingExtensions {
        public const String Doublespace = Singlespace + Singlespace;
        /*
                public const String LotsOfSpacesString = "                                                                                          ";
        */

        /// <summary>
        ///     abcdefghijklmnopqrstuvwxyz
        /// </summary>
        public const String Lowercase = "abcdefghijklmnopqrstuvwxyz";

        public const String MatchMoney = @"//\$\s*[-+]?([0-9]{0,3}(,[0-9]{3})*(\.[0-9]+)?)";

        /// <summary>
        ///     0123456789
        /// </summary>
        public const String Numbers = "0123456789";

        public const String Singlespace = @" ";
        public const String SplitByEnglish = @"(?:\p{Lu}(?:\.\p{Lu})+)(?:,\s*\p{Lu}(?:\.\p{Lu})+)*";

        /// <summary>
        ///     Regex pattern for words that don't start with a number
        /// </summary>
        public const String SplitByWordNotNumber = @"([a-zA-Z]\w+)\W*";

        /// <summary>
        ///     ~`!@#$%^&*()-_=+?:,./\[]{}|'
        /// </summary>
        public const String Symbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

        /// <summary>
        ///     ABCDEFGHIJKLMNOPQRSTUVWXYZ
        /// </summary>
        public const String Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static readonly String AllLetters = new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char ) i ).Distinct().Where( Char.IsLetter ).OrderBy( c => c ).ToArray() );

        [ NotNull ] public static readonly String AllLowercaseLetters = new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char ) i ).Distinct().Where( Char.IsLetter ).Where( Char.IsLower ).OrderBy( c => c ).ToArray() );

        [ NotNull ] public static readonly String AllUppercaseLetters = new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char ) i ).Distinct().Where( Char.IsLetter ).Where( Char.IsUpper ).OrderBy( c => c ).ToArray() );

        [ NotNull ] public static readonly String Alphabet = new String( value: Randem.NextString( 676, lowers: true, uppers: false, numbers: false, symbols: false ).Where( Char.IsLetter ).Distinct().OrderBy( c => c ).Aggregate( String.Empty, ( s, c1 ) => s + ' ' + c1 ).ToArray() ).Trim();

        [ NotNull ] public static readonly Lazy< PluralizationService > LazyPluralizationService = new Lazy< PluralizationService >( () => PluralizationService.CreateService( Thread.CurrentThread.CurrentCulture ) );

        public static readonly String[] OrdinalSuffixes = { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };

        /// <summary>
        ///     this doesn't handle apostrophe well
        /// </summary>
        public static readonly Regex RegexBySentenceNotworking = new Regex( pattern: @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])", options: RegexOptions.Compiled | RegexOptions.Multiline );

        public static readonly Regex RegexBySentenceStackoverflow = new Regex( "(?<Sentence>\\S.+?(?<Terminator>[.!?]|\\Z))(?=\\s+|\\Z)", RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled );

        public static readonly Regex RegexByWordBreak = new Regex( pattern: @"(?=\S*(?<=\w))\b", options: RegexOptions.Compiled | RegexOptions.Singleline );

        public static readonly Regex RegexJustDigits = new Regex( @"\D+", RegexOptions.Compiled );

        public static readonly char[] SpaceSplitBy = { Singlespace[ 0 ] };

        /// <summary>
        ///     The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        public static readonly string[] UriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        private static readonly String[] TensMap = { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        private static readonly String[] UnitsMap = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };

        /// <summary>
        ///     Add dashes to a pascal-cased string
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>string</returns>
        public static string AddDashes( this string pascalCasedWord ) {
            return Regex.Replace( Regex.Replace( Regex.Replace( pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1-$2" ), @"([a-z\d])([A-Z])", "$1-$2" ), @"[\s]", "-" );
        }

        /// <summary>
        ///     Add an undescore prefix to a pascasl-cased string
        /// </summary>
        /// <param name="pascalCasedWord"></param>
        /// <returns></returns>
        public static string AddUnderscorePrefix( this string pascalCasedWord ) {
            return string.Format( "_{0}", pascalCasedWord );
        }

        /// <summary>
        ///     Add underscores to a pascal-cased string
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>string</returns>
        public static string AddUnderscores( this string pascalCasedWord ) {
            return Regex.Replace( Regex.Replace( Regex.Replace( pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2" ), @"([a-z\d])([A-Z])", "$1_$2" ), @"[-\s]", "_" );
        }

        public static String After( [ NotNull ] this String s, [ NotNull ] String splitter ) {
            if ( s == null ) {
                throw new ArgumentNullException( "s" );
            }
            if ( splitter == null ) {
                throw new ArgumentNullException( "splitter" );
            }
            return s.Substring( s.IndexOf( splitter, StringComparison.InvariantCulture ) + 1 ).TrimStart();
        }

        public static String Append( [ CanBeNull ] this String result, [ CanBeNull ] String appendThis ) {
            return String.Format( "{0}{1}", result ?? String.Empty, appendThis ?? String.Empty );
        }

        /// <summary>
        ///     Return the <see cref="tuple" /> formatted with the index.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static String AsIndexed( this Tuple< string, int > tuple ) {
            return String.Format( "{0}.[{1}]", tuple.Item1, tuple.Item2 );
        }

        /// <summary>
        ///     Return the <see cref="word" /> formatted with the <see cref="index" />.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static String AsIndexed( [ NotNull ] this String word, int index ) {
            if ( word == null ) {
                throw new ArgumentNullException( "word" );
            }
            return String.Format( "{0}.[{1}]", word, index );
        }

        /// <summary>
        ///     Return an integer formatted as 1st, 2nd, 3rd, etc...
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String AsOrdinal( this int number ) {
            switch ( number % 100 ) {
                case 13:
                case 12:
                case 11:
                    return String.Format( "{0}th", number );
            }
            switch ( number % 10 ) {
                case 1:
                    return String.Format( "{0}st", number );

                case 2:
                    return String.Format( "{0}nd", number );

                case 3:
                    return String.Format( "{0}rd", number );

                default:
                    return String.Format( "{0}th", number );
            }
        }

        public static String Base64Decode( String base64EncodedData ) {
            var base64EncodedBytes = Convert.FromBase64String( base64EncodedData );
            return Encoding.UTF8.GetString( base64EncodedBytes );
        }

        public static String Base64Encode( this String plainText ) {
            if ( String.IsNullOrEmpty( plainText ) ) {
                plainText = String.Empty;
            }
            var plainTextBytes = Encoding.UTF8.GetBytes( plainText );
            return Convert.ToBase64String( plainTextBytes );
        }

        /// <summary>
        ///     Return the substring from 0 to the index of the splitter.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static String Before( [ NotNull ] this String s, [ NotNull ] String splitter ) {
            if ( s == null ) {
                throw new ArgumentNullException( "s" );
            }
            if ( splitter == null ) {
                throw new ArgumentNullException( "splitter" );
            }
            return s.Substring( 0, s.IndexOf( splitter, StringComparison.InvariantCulture ) ).TrimEnd();
        }

        /// <summary>
        ///     Pulled from https://bitbucket.org/jpbochi/jplabscode/src/e1bb20c8f273/Extensions/CompressionExt.cs
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] Compress( [ NotNull ] this String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            return Compress( text, Encoding.Default );
        }

        public static byte[] Compress( [ NotNull ] this String text, [ NotNull ] Encoding encoding ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            if ( encoding == null ) {
                throw new ArgumentNullException( "encoding" );
            }
            return encoding.GetBytes( text ).Compress();
        }

        public static IEnumerable< T > ConcatSingle< T >( [ NotNull ] this IEnumerable< T > sequence, T element ) {
            if ( sequence == null ) {
                throw new ArgumentNullException( "sequence" );
            }
            foreach ( var item in sequence ) {
                yield return item;
            }
            yield return element;
        }

        public static IDictionary< char, ulong > Count( this String text ) {
            var dict = new ConcurrentDictionary< char, ulong >();
            text.AsParallel().ForAll( c => dict.AddOrUpdate( c, 1, ( c1, arg2 ) => arg2 + 1 ) );
            return dict;
        }

        public static UInt64 Count( this String text, Char character ) {
            return ( UInt64 ) text.Where( c => c == character ).LongCount();
        }

        /// <summary>
        ///     Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
        ///     integers, where each integer represents the code point of a character in the source String.
        ///     Includes an optional threshhold which can be used to indicate the maximum allowable distance.
        /// </summary>
        /// <param name="source">An array of the code points of the first String</param>
        /// <param name="target">An array of the code points of the second String</param>
        /// <param name="threshold">Maximum allowable distance</param>
        /// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
        public static int DamerauLevenshteinDistance( this String source, String target, int threshold ) {
            var length1 = source.Length;
            var length2 = target.Length;

            // Return trivial case - difference in String lengths exceeds threshhold
            if ( Math.Abs( length1 - length2 ) > threshold ) {
                return Int32.MaxValue;
            }

            // Ensure arrays [i] / length1 use shorter length
            if ( length1 > length2 ) {
                Utility.Swap( ref target, ref source );
                Utility.Swap( ref length1, ref length2 );
            }

            var maxi = length1;
            var maxj = length2;

            var dCurrent = new int[maxi + 1];
            var dMinus1 = new int[maxi + 1];
            var dMinus2 = new int[maxi + 1];

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
                    var min = del > ins ? ( ins > sub ? sub : ins ) : ( del > sub ? sub : del );

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
            return ( result > threshold ) ? Int32.MaxValue : result;
        }

        public static byte[] Decompress( [ NotNull ] this byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            using ( var decompress = new GZipStream( new MemoryStream( data ), CompressionMode.Decompress ) ) {
                using ( var output = new MemoryStream() ) {
                    decompress.CopyTo( output );
                    return output.ToArray();
                }
            }
        }

        public static String DecompressToString( [ NotNull ] this byte[] data ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            return DecompressToString( data, Encoding.Default );
        }

        public static String DecompressToString( [ NotNull ] this byte[] data, [ NotNull ] Encoding encoding ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            if ( encoding == null ) {
                throw new ArgumentNullException( "encoding" );
            }
            return encoding.GetString( data.Decompress() );
        }

        /// <summary>
        ///     <para>Case insensitive string-end comparison. </para>
        ///     <para>( true example: cAt == CaT )</para>
        ///     <para>
        ///         <see cref="StringComparison.InvariantCultureIgnoreCase" />
        ///     </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Boolean EndsLike( this String source, String compare ) {
            return source.EndsWith( compare, StringComparison.InvariantCultureIgnoreCase );
            //( source ?? String.Empty ).Equals( compare ?? String.Empty,  );
        }

        public static IEnumerable< char > EnglishOnly( this String s ) {
            try {
                var sb = new StringBuilder();
                foreach ( Match m in Regex.Matches( s, @"(\w+)|(\$\d+\.\d+)" ) ) {
                    sb.Append( m.Value );
                }
                return sb.ToString().Trim();
            }
            catch ( Exception exception ) {
                exception.Error();
                return s;
            }
        }

        /// <summary>
        ///     <para>Escapes a string according to the URI data string rules given in RFC 3986.</para>
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>
        ///     The escaped value.
        /// </returns>
        /// <seealso cref="http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" />
        /// <seealso cref="http://meyerweb.com/eric/tools/dencoder/" />
        /// <seealso cref="http://www.ietf.org/rfc/rfc2396.txt" />
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/vstudio/bb968786(v=vs.100).aspx" />
        /// <remarks>
        ///     <para>
        ///         The <see cref="Uri.EscapeDataString" /> method is <i>supposed</i> to take on RFC 3986 behavior if certain
        ///         elements are present in a .config file.  Even if this actually worked (which in my experiments it
        ///         <i>doesn't</i>), we can't rely on every host actually having this configuration element present.
        ///     </para>
        /// </remarks>
        public static string EscapeUriDataStringRfc3986( string value ) {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            var escaped = new StringBuilder( Uri.EscapeDataString( value ) );

            // Upgrade the escaping to RFC 3986, if necessary.
            foreach ( var t in UriRfc3986CharsToEscape ) {
                escaped.Replace( t, Uri.HexEscape( t[ 0 ] ) );
            }

            // Return the fully-RFC3986-escaped string.

            return escaped.ToString();
        }

        public static Boolean ExactMatch( [ NotNull ] this String source, [ NotNull ] String compare ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( compare == null ) {
                throw new ArgumentNullException( "compare" );
            }
            if ( source.Length == 0 || compare.Length == 0 ) {
                return false;
            }
            return source.SequenceEqual( compare );
        }

        public static String FirstSentence( this String paragraph ) {
            return paragraph.ToSentences().FirstOrDefault();
        }

        public static String FirstWord( this String sentence ) {
            return sentence.ToWords().FirstOrDefault() ?? String.Empty;
        }

        /// <summary>
        /// </summary>
        /// <param name="rational"></param>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <seealso
        ///     cref="http://kashfarooq.wordpress.com/2011/08/01/calculating-pi-in-c-part-3-using-the-net-4-bigrational-class/" />
        public static String Format( this BigRational rational, int numberOfDigits ) {
            var numeratorShiftedToEnoughDigits = ( rational.Numerator * BigInteger.Pow( new BigInteger( 10 ), numberOfDigits ) );
            var bigInteger = numeratorShiftedToEnoughDigits / rational.Denominator;
            var toBeFormatted = bigInteger.ToString();
            var builder = new StringBuilder();
            builder.Append( toBeFormatted[ 0 ] );
            builder.Append( "." );
            builder.Append( toBeFormatted.Substring( 1, numberOfDigits - 1 ) );
            return builder.ToString();
        }

        public static String FullSoundex( this String s ) {
            // the encoding information
            //const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const String codes = "0123012D02245501262301D202";

            // some helpful regexes
            var hwBeginString = new Regex( "^D+" );
            var simplify = new Regex( @"(\d)\1*D?\1+" );
            var cleanup = new Regex( "[D0]" );

            // i need a capitalized String
            s = s.ToUpper();

            // i'm building the coded String using a String builder because i think this is probably the fastest and least intensive way
            var coded = new StringBuilder();

            // do the encoding
            foreach ( var index in s.Select( t => AllUppercaseLetters.IndexOf( t ) ).Where( index => index >= 0 ) ) {
                coded.Append( codes[ index ] );
            }

            // okay, so here's how this goes . . . the first thing I do is assign the coded String so that i can regex replace on it

            // then i remove repeating characters
            //result = repeating.Replace(result, "$1");
            var result = simplify.Replace( coded.ToString(), "$1" ).Substring( 1 );

            // now i need to remove any characters coded as D  from
            // the front of the String because they're not really
            // valid as the first code because they don't have an
            // actual soundex code value
            result = hwBeginString.Replace( result, String.Empty );

            // i used the char D to indicate that an h or w existed
            // so that if to similar sounds were separated by an h or
            // a w that I could remove one of them.  if the h or w does
            // not separate two similar sounds, then i need to remove it now
            result = cleanup.Replace( result, String.Empty );

            // return the first character followed by the coded String
            return String.Format( "{0}{1}", s[ 0 ], result );
        }

        /// <summary>
        ///     Return possible variants of a name for name matching.
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <param name="culture">The culture to use for conversion</param>
        /// <returns>IEnumerable&lt;string&gt;</returns>
        public static IEnumerable< string > GetNameVariants( this string input, CultureInfo culture ) {
            if ( String.IsNullOrEmpty( input ) ) {
                yield break;
            }

            yield return input;

            // try camel cased name
            yield return input.ToCamelCase( culture );

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
            yield return input.ToCamelCase( culture ).AddUnderscorePrefix();
        }

        public static String InOutputFormat( this String indexed ) {
            return String.Format( "{0}-|", indexed );
        }

        public static Boolean IsJustNumbers( [ CanBeNull ] this String text ) {
            if ( null == text ) {
                return false;
            }
            if ( text.All( Char.IsNumber ) ) {
                return true;
            }
            Double test;
            return Double.TryParse( text, out test );
        }

        public static Boolean IsJustNumbers( [ CanBeNull ] this String text, out Decimal result ) {
            return Decimal.TryParse( text ?? String.Empty, out result );
        }

        public static Boolean IsNullOrEmpty( [ CanBeNull ] this String value ) {
            return String.IsNullOrEmpty( value );
        }

        public static Boolean IsNullOrWhiteSpace( [ CanBeNull ] this String value ) {
            return String.IsNullOrWhiteSpace( value );
        }

        /// <summary>
        ///     Checks to see if a string is all uppper case
        /// </summary>
        /// <param name="inputString">String to check</param>
        /// <returns>bool</returns>
        public static bool IsUpperCase( this string inputString ) {
            return Regex.IsMatch( inputString, @"^[A-Z]+$" );
        }

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
        /// <returns></returns>
        public static IEnumerable< string > JustDigits( this String sentence ) {
            return RegexJustDigits.Split( sentence );
        }

        /// <summary>
        ///     Example: String s = "123-123-1234".JustNumbers();
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String JustNumbers( this String s ) {
            try {
                var sb = new StringBuilder();
                foreach ( Match m in Regex.Matches( s, "[0-9]" ) ) {
                    sb.Append( m.Value );
                }
                return sb.ToString();
            }
            catch ( Exception error ) {
                error.Error();
                return s;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        /// <seealso cref="Word" />
        /// <seealso cref="Sentence" />
        public static IEnumerable< string > JustWords( this String sentence ) {
            var result = sentence.ToWords().Where( word => word.Any( Char.IsLetterOrDigit ) );
            return result;
        }

        /// <summary>
        ///     <para>Case insensitive string comparison. </para>
        ///     <para>( true example: cAt == CaT )</para>
        ///     <para>
        ///         <see cref="StringComparison.InvariantCultureIgnoreCase" />
        ///     </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Boolean Like( this String source, String compare ) {
            return ( source ?? String.Empty ).Equals( compare ?? String.Empty, StringComparison.InvariantCultureIgnoreCase );
        }

        /// <summary>
        ///     Convert the first letter of a string to lower case
        /// </summary>
        /// <param name="word">String to convert</param>
        /// <returns>string</returns>
        public static string MakeInitialLowerCase( this string word ) {
            return String.Concat( word.Substring( 0, 1 ).ToLower(), word.Substring( 1 ) );
        }

        /// <summary>
        ///     Gets a <b>horrible</b> ROUGH guesstimate of the memory consumed by an object by using
        ///     <seealso
        ///         cref="NetDataContractSerializer" />
        ///     .
        /// </summary>
        /// <param name="bob"> </param>
        /// <returns> </returns>
        public static long MemoryUsed( [ NotNull ] this Object bob ) {
            if ( bob == null ) {
                throw new ArgumentNullException( "bob" );
            }
            try {
                using ( var s = new NullStream() ) {
                    var serializer = new NetDataContractSerializer {
                                                                       AssemblyFormat = FormatterAssemblyStyle.Full
                                                                   };
                    serializer.WriteObject( stream: s, graph: bob );
                    return s.Length;
                }
            }
            catch ( InvalidDataContractException exception ) {
                exception.Error();
            }
            catch ( SerializationException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }
            return 0;
        }

        public static long MinifyXML( this XmlDocument xml ) {
            //TODO todo.
            throw new NotImplementedException();
        }

        //        public const String Lowercase = "abcdefghijklmnopqrstuvwxyz";
        //    }
        //}
        [ Pure ]
        [ CanBeNull ]
        public static String NullIfBlank( [ CanBeNull ] this String theString ) {
            if ( null == theString || String.IsNullOrWhiteSpace( theString ) ) {
                return null;
            }
            theString = theString.Trim();
            return String.IsNullOrWhiteSpace( theString ) ? null : theString;
        }

        public static String NullIfEmpty( [ CanBeNull ] this String value ) {
            return String.IsNullOrEmpty( value ) ? null : value;
        }

        public static String NullIfEmptyOrWhiteSpace( [ CanBeNull ] this String value ) {
            return String.IsNullOrWhiteSpace( value ) ? null : value;
        }

        public static String NullIfJustNumbers( [ CanBeNull ] this String value ) {
            return value.IsJustNumbers() ? null : value;
        }

        public static Int32 NumberOfDigits( this BigInteger number ) {
            // do not count the sign
            return ( number * number.Sign ).ToString().Length;
        }

        public static String PadMiddle( int totalLength, String partA, String partB, char paddingChar ) {
            var result = partA + partB;
            while ( result.Length < totalLength ) {
                result = result.Insert( partA.Length, "_" );
            }
            while ( result.Length > totalLength ) {
                result = result.Remove( partA.Length, 1 );
            }

            return result;
        }

        public static String PadMiddle( int totalLength, String partA, String partB, String partC, char paddingChar ) {
            var padding = "".PadRight( ( totalLength - ( partA.Length + partB.Length + partC.Length ) ) / 2, '_' );
            return partA + padding + partB + "".PadRight( totalLength - ( partA.Length + padding.Length + partB.Length + partC.Length ), '_' ) + partC;
        }

        public static int ParallelEditDistance( this string s1, string s2 ) {
            var dist = new int[s1.Length + 1, s2.Length + 1];
            for ( var i = 0; i <= s1.Length; i++ ) {
                dist[ i, 0 ] = i;
            }
            for ( var j = 0; j <= s2.Length; j++ ) {
                dist[ 0, j ] = j;
            }
            var numBlocks = Environment.ProcessorCount * 4;

            ParallelAlgorithms.Wavefront( ( ( startI, endI, startJ, endJ ) => {
                                                for ( var i = startI + 1; i <= endI; i++ ) {
                                                    for ( var j = startJ + 1; j <= endJ; j++ ) {
                                                        dist[ i, j ] = ( s1[ i - 1 ] == s2[ j - 1 ] ) ? dist[ i - 1, j - 1 ] : 1 + Math.Min( dist[ i - 1, j ], Math.Min( dist[ i, j - 1 ], dist[ i - 1, j - 1 ] ) );
                                                    }
                                                }
                                            } ), s1.Length, s2.Length, numBlocks, numBlocks );

            return dist[ s1.Length, s2.Length ];
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String PluralOf( this UInt64 number, [ NotNull ] String word ) {
            if ( word == null ) {
                throw new ArgumentNullException( "word" );
            }
            if ( 1 == number ) {
                return word;
            }
            if ( LazyPluralizationService.Value.IsPlural( word ) ) {
                return word;
            }
            return LazyPluralizationService.Value.Pluralize( word );
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String PluralOf( this Double number, [ NotNull ] String word ) {
            if ( word == null ) {
                throw new ArgumentNullException( "word" );
            }
            if ( number.Near( 1 ) ) {
                return word;
            }

            return LazyPluralizationService.Value.Pluralize( word );
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String PluralOf( this Decimal number, [ NotNull ] String word ) {
            if ( word == null ) {
                throw new ArgumentNullException( "word" );
            }

            if ( Decimal.One == number ) {
                return word;
            }

            if ( LazyPluralizationService.Value.IsPlural( word ) ) {
                return word;
            }

            var result = LazyPluralizationService.Value.Pluralize( word );
            return result;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String PluralOf( this BigDecimal number, [ NotNull ] String word ) {
            if ( word == null ) {
                throw new ArgumentNullException( "word" );
            }
            if ( number == BigDecimal.One ) {
                return word;
            }
            if ( LazyPluralizationService.Value.IsPlural( word ) ) {
                return word;
            }
            return LazyPluralizationService.Value.Pluralize( word );
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String PluralOf( this BigInteger number, [ NotNull ] String word ) {
            if ( word == null ) {
                throw new ArgumentNullException( "word" );
            }
            if ( BigInteger.One == number ) {
                return word;
            }
            if ( LazyPluralizationService.Value.IsPlural( word ) ) {
                return word;
            }
            return LazyPluralizationService.Value.Pluralize( word );
        }

        public static String Prepend( [ CanBeNull ] this String result, [ CanBeNull ] String prependThis ) {
            return String.Format( "{0}{1}", prependThis ?? String.Empty, result ?? String.Empty );
        }

        public static String ReadToEnd( [ NotNull ] this MemoryStream ms ) {
            if ( ms == null ) {
                throw new ArgumentNullException( "ms" );
            }
            ms.Seek( 0, SeekOrigin.Begin );
            using ( var reader = new StreamReader( ms ) ) {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        ///     Remove leading and trailing " from a string
        /// </summary>
        /// <param name="input">String to parse</param>
        /// <returns>String</returns>
        public static string RemoveSurroundingQuotes( this string input ) {
            if ( input.StartsWith( "\"" ) && input.EndsWith( "\"" ) ) {
                // remove leading/trailing quotes
                input = input.Substring( 1, input.Length - 2 );
            }
            return input;
        }

        public static string ReplaceAll( string haystack, string needle, string replacement ) {
            int pos;
            // Avoid a possible infinite loop
            if ( needle == replacement ) {
                return haystack;
            }
            while ( ( pos = haystack.IndexOf( needle, StringComparison.Ordinal ) ) > 0 ) {
                haystack = haystack.Substring( 0, pos ) + replacement + haystack.Substring( pos + needle.Length );
            }
            return haystack;
        }

        public static string ReplaceFirst( string haystack, string needle, string replacement ) {
            var pos = haystack.IndexOf( needle, StringComparison.Ordinal );
            if ( pos < 0 ) {
                return haystack;
            }
            return haystack.Substring( 0, pos ) + replacement + haystack.Substring( pos + needle.Length );
        }

        public static String ReplaceHTML( this String s, String withwhat ) {
            return Regex.Replace( s, @"<(.|\n)*?>", withwhat );
        }

        /// <summary>
        ///     Reverse a String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Reverse( this String s ) {
            var charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new String( charArray );
        }

        public static String Right( this String s, int count ) {
            var newString = String.Empty;
            if ( !String.IsNullOrEmpty( s ) && count > 0 ) {
                var startIndex = s.Length - count;
                newString = startIndex > 0 ? s.Substring( startIndex, count ) : s;
            }
            return newString;
        }

        /// <summary>
        ///     Case sensitive
        ///     <see cref="StringComparison.InvariantCulture" />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Boolean Same( this String source, String compare ) {
            return ( source ?? String.Empty ).Equals( compare ?? String.Empty, StringComparison.InvariantCulture );
        }

        /// <summary>
        ///     Compute a Similarity between two strings.<br />
        ///     1.0 is a full, bit for bit match.<br />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <param name="timeout"></param>
        /// <param name="matchReasons">preferably an empty queue</param>
        /// <returns></returns>
        /// <remarks> The score is normalized such that 0 equates to no similarity and 1 is an exact match.</remarks>
        [ UsedImplicitly ]
        public static Double Similarity( [ NotNull ] this String source, [ NotNull ] String compare, [ NotNull ] ref ConcurrentQueue< string > matchReasons, TimeSpan? timeout = null ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( compare == null ) {
                throw new ArgumentNullException( "compare" );
            }
            if ( matchReasons == null ) {
                throw new ArgumentNullException( "matchReasons" );
            }

            if ( !timeout.HasValue ) {
                timeout = TimeSpan.MaxValue;
            }

            var stopwatch = Stopwatch.StartNew();
            var floater = new PotentialF( 0 );

            if ( source.Length <= 0 || compare.Length <= 0 ) {
                goto noMoreTests;
            }

            #region Test for an exact match
            if ( source.ExactMatch( compare ) ) {
                matchReasons.Add( "ExactMatch( source, compare )" );
                floater.Add( 1 );
                goto noMoreTests;
            }
            #endregion Test for an exact match

            if ( source.SequenceEqual( compare ) ) {
                goto noMoreTests; //exact match. no more comparisons needed.
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            var votes = new VotallyD();

            #region Test for any amount of characters
            votes.ForA( source.Length );
            votes.ForB( compare.Length );
            #endregion Test for any amount of characters

            #region Test for UTF32Encoding
            var sourceIntoUTF32Encoding = new UTF32Encoding( bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: false ).GetBytes( source );
            votes.ForA( sourceIntoUTF32Encoding.LongCount() );

            var compareIntoUTF32Encoding = new UTF32Encoding( bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: false ).GetBytes( compare );
            votes.ForB( compareIntoUTF32Encoding.LongCount() );

            // Test for exact same sequence
            if ( sourceIntoUTF32Encoding.SequenceEqual( compareIntoUTF32Encoding ) ) {
                votes.ForA( sourceIntoUTF32Encoding.Length );
                votes.ForB( compareIntoUTF32Encoding.Length );
                matchReasons.Add( "exact match as UTF32 encoded" );
                goto noMoreTests;
            }

            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion Test for UTF32Encoding

            #region Test for a string reversal.
            var compareReversed = Enumerable.Reverse( compare );
            if ( source.SequenceEqual( compareReversed ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length / 2.0 );
                matchReasons.Add( "partial string reversal" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion Test for a string reversal.

            #region Test for exact match after Distinct()
            var sourceDistinct = new String( source.Distinct().ToArray() );
            var compareDistinct = new String( compare.Distinct().ToArray() );
            var compareDistinctReverse = new String( Enumerable.Reverse( compareDistinct ).ToArray() );

            if ( sourceDistinct.SequenceEqual( compareDistinct ) ) {
                votes.ForA( sourceDistinct.Length );
                votes.ForB( compareDistinct.Length );
                matchReasons.Add( "exact match after Distinct()" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion Test for exact match after Distinct()

            #region Test for reversal match after distinct
            if ( sourceDistinct.SequenceEqual( compareDistinctReverse ) ) {
                votes.ForA( sourceDistinct.Length * 2 );
                votes.ForB( compareDistinctReverse.Length );
                matchReasons.Add( "exact match after Distinct()" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion Test for reversal match after distinct

            #region do any chars in source also show in compare
            var tempcounter = 0;
            foreach ( var c in source ) {
                votes.ForA();
                if ( compare.Contains( c ) ) {
                    votes.ForB();
                    tempcounter++;
                }
            }
            if ( tempcounter > 0 ) {
                matchReasons.Add( String.Format( "{0} characters found in compare from source", tempcounter ) );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion do any chars in source also show in compare

            #region do any chars in compare also show in source
            tempcounter = 0;
            foreach ( var c in compare ) {
                votes.ForB( 1 );
                if ( source.Contains( c ) ) {
                    votes.ForA( 1 );
                    tempcounter++;
                }
            }
            if ( tempcounter > 0 ) {
                matchReasons.Add( String.Format( "{0} characters found in compare from source", tempcounter ) );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion do any chars in compare also show in source

            #region check for substrings
            if ( source.Contains( compare ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length );
                matchReasons.Add( "found compare string inside source string" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            if ( compare.Contains( source ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length );
                matchReasons.Add( "found source string inside compare string" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }
            #endregion check for substrings

            #region DamerauLevenshteinDistance
            Single threshold = Math.Max( source.Length, compare.Length );
            var actualDamerauLevenshteinDistance = DamerauLevenshteinDistance( source: source, target: compare, threshold: ( int ) threshold );
            floater.Add( threshold - ( actualDamerauLevenshteinDistance / threshold ) );

            if ( stopwatch.Elapsed > timeout ) { }
            #endregion DamerauLevenshteinDistance

            noMoreTests:
            return floater;
        }

        public static String Soundex( [ NotNull ] this String s, int length = 4 ) {
            if ( s == null ) {
                throw new ArgumentNullException( "s" );
            }
            return FullSoundex( s ).PadRight( length, '0' ) // soundex is no shorter than
                                   .Substring( 0, length ); // and no longer than length
        }

        public static IEnumerable< string > SplitToChunks( [ NotNull ] this string s, int chunks ) {
            if ( s == null ) {
                throw new ArgumentNullException( "s" );
            }
            var res = Enumerable.Range( 0, s.Length ).Select( index => new {
                                                                               index = index,
                                                                               ch = s[ index ]
                                                                           } ).GroupBy( f => f.index / chunks ).Select( g => String.Join( "", g.Select( z => z.ch ) ) );

            return res;
        }

        public static String StringFromResponse( [ CanBeNull ] this WebResponse response ) {
            if ( null == response ) {
                return String.Empty;
            }
            var restream = response.GetResponseStream();
            if ( restream != null ) {
                return new StreamReader( restream ).ReadToEnd();
            }
            return String.Empty;
        }

        public static Byte[] StringToUTF32ByteArray( this String pXmlString ) {
            return new UTF32Encoding().GetBytes( pXmlString );
        }

        /// <summary>
        ///     Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        public static Byte[] StringToUTF8ByteArray( this String pXmlString ) {
            return new UTF8Encoding().GetBytes( pXmlString );
        }

        public static String StripHTML( this String s ) {
            return Regex.Replace( s, @"<(.|\n)*?>", String.Empty ).Replace( "&nbsp;", " " );
        }

        public static string StripTags( string input, string[] allowedTags ) {
            var StripHTMLExp = new Regex( @"(<\/?[^>]+>)" );
            var Output = input;

            foreach ( Match Tag in StripHTMLExp.Matches( input ) ) {
                var HTMLTag = Tag.Value.ToLower();
                var IsAllowed = false;

                foreach ( var AllowedTag in allowedTags ) {
                    var offset = -1;

                    // Determine if it is an allowed tag
                    // "<tag>" , "<tag " and "</tag"
                    if ( offset != 0 ) {
                        offset = HTMLTag.IndexOf( '<' + AllowedTag + '>', StringComparison.Ordinal );
                    }
                    if ( offset != 0 ) {
                        offset = HTMLTag.IndexOf( '<' + AllowedTag + ' ', StringComparison.Ordinal );
                    }
                    if ( offset != 0 ) {
                        offset = HTMLTag.IndexOf( "</" + AllowedTag, StringComparison.Ordinal );
                    }

                    // If it matched any of the above the tag is allowed
                    if ( offset != 0 ) {
                        continue;
                    }
                    IsAllowed = true;
                    break;
                }

                // Remove tags that are not allowed
                if ( !IsAllowed ) {
                    Output = ReplaceFirst( Output, Tag.Value, "" );
                }
            }

            return Output;
        }

        public static string StripTagsAndAttributes( string Input, string[] allowedTags ) {
            /* Remove all unwanted tags first */
            var Output = StripTags( Input, allowedTags );

            /* Lambda functions */
            MatchEvaluator HrefMatch = m => m.Groups[ 1 ].Value + "href..;,;.." + m.Groups[ 2 ].Value;
            MatchEvaluator ClassMatch = m => m.Groups[ 1 ].Value + "class..;,;.." + m.Groups[ 2 ].Value;
            MatchEvaluator UnsafeMatch = m => m.Groups[ 1 ].Value + m.Groups[ 4 ].Value;

            /* Allow the "href" attribute */
            Output = new Regex( "(<a.*)href=(.*>)" ).Replace( Output, HrefMatch );

            /* Allow the "class" attribute */
            Output = new Regex( "(<a.*)class=(.*>)" ).Replace( Output, ClassMatch );

            /* Remove unsafe attributes in any of the remaining tags */
            Output = new Regex( @"(<.*) .*=(\'|\""|\w)[\w|.|(|)]*(\'|\""|\w)(.*>)" ).Replace( Output, UnsafeMatch );

            /* Return the allowed tags to their proper form */
            Output = ReplaceAll( Output, "..;,;..", "=" );

            return Output;
        }

        public static void Test() {
            Console.WriteLine( StripTags( "<p>George</p><b>W</b><i>Bush</i>", new[] { "i", "b" } ) );
            Console.WriteLine( StripTags( "<p>George <img src='someimage.png' onmouseover='someFunction()'>W <i>Bush</i></p>", new[] { "p" } ) );
            Console.WriteLine( StripTags( "<a href='http://www.dijksterhuis.org'>Martijn <b>Dijksterhuis</b></a>", new[] { "a" } ) );

            const string test4 = "<a class=\"classof69\" onClick='crosssite.boom()' href='http://www.dijksterhuis.org'>Martijn Dijksterhuis</a>";
            Console.WriteLine( StripTagsAndAttributes( test4, new[] { "a" } ) );
        }

        /// <summary>
        ///     Converts a string to camel case
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
        /// <returns>String</returns>
        public static string ToCamelCase( this string lowercaseAndUnderscoredWord, CultureInfo culture ) {
            return MakeInitialLowerCase( ToPascalCase( lowercaseAndUnderscoredWord, culture ) );
        }

        /// <summary>
        ///     Same as <see cref="AsOrdinal" />, but might be slightly faster performance-wise.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String ToOrdinal( this int number ) {
            var n = Math.Abs( number );
            var lt = n % 100;
            return number + OrdinalSuffixes[ ( lt >= 11 && lt <= 13 ) ? 0 : n % 10 ];
        }

        /// <summary>
        ///     Converts a string to pascal case with the option to remove underscores
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="culture"></param>
        /// <param name="removeUnderscores">Option to remove underscores</param>
        /// <returns></returns>
        public static string ToPascalCase( this string text, CultureInfo culture, bool removeUnderscores = true ) {
            if ( String.IsNullOrEmpty( text ) ) {
                return text;
            }

            text = text.Replace( "_", " " );
            var joinString = removeUnderscores ? String.Empty : "_";
            var words = text.Split( ' ' );
            if ( words.Length <= 1 && !words[ 0 ].IsUpperCase() ) {
                return String.Concat( words[ 0 ].Substring( 0, 1 ).ToUpper( culture ), words[ 0 ].Substring( 1 ) );
            }

            for ( var i = 0; i < words.Length; i++ ) {
                if ( words[ i ].Length <= 0 ) {
                    continue;
                }
                var word = words[ i ];
                var restOfWord = word.Substring( 1 );

                if ( restOfWord.IsUpperCase() ) {
                    restOfWord = restOfWord.ToLower( culture );
                }

                var firstChar = char.ToUpper( word[ 0 ], culture );
                words[ i ] = String.Concat( firstChar, restOfWord );
            }
            return String.Join( joinString, words );
        }

        [ NotNull ]
        public static IEnumerable< Sentence > ToSentences( [ CanBeNull ] this String paragraph ) {
            if ( paragraph == null ) {
                return Enumerable.Empty< Sentence >();
            }
            //clean it up some
            paragraph = paragraph.Replace( "\t", Singlespace );
            paragraph = paragraph.Replace( "\r\n", Environment.NewLine );
            paragraph = paragraph.Replace( "\n\n", Environment.NewLine );
            paragraph = paragraph.Replace( "\r", Environment.NewLine );
            paragraph = paragraph.Replace( "\n", Environment.NewLine );
            //paragraph = paragraph.Replace( Environment.NewLine, Singlespace );

            while ( paragraph.Contains( Doublespace ) ) {
                paragraph = paragraph.Replace( oldValue: Doublespace, newValue: Singlespace );
            }

            var results = RegexBySentenceStackoverflow.Split( input: paragraph ).Select( s => s.Replace( Environment.NewLine, String.Empty ).Trim() ).Where( ts => !String.IsNullOrWhiteSpace( ts ) && !ts.Equals( "." ) );
            return results.Select( s => new Sentence( s ) );
        }

        /// <summary>
        ///     Returns the wording of a number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/a/2730393/956364" />
        public static string ToVerbalWord( this int number ) {
            if ( number == 0 ) {
                return "zero";
            }

            if ( number < 0 ) {
                return "minus " + ToVerbalWord( Math.Abs( number ) );
            }

            var words = String.Empty;

            if ( ( number / 1000000 ) > 0 ) {
                words += ToVerbalWord( number / 1000000 ) + " million ";
                number %= 1000000;
            }

            if ( ( number / 1000 ) > 0 ) {
                words += ToVerbalWord( number / 1000 ) + " thousand ";
                number %= 1000;
            }

            if ( ( number / 100 ) > 0 ) {
                words += ToVerbalWord( number / 100 ) + " hundred ";
                number %= 100;
            }

            if ( number <= 0 ) {
                return words;
            }

            if ( words != "" ) {
                words += "and ";
            }

            if ( number < 20 ) {
                words += UnitsMap[ number ];
            }
            else {
                words += TensMap[ number / 10 ];
                if ( ( number % 10 ) > 0 ) {
                    words += "-" + UnitsMap[ number % 10 ];
                }
            }

            return words;
        }

        /// <summary>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/a/7829529/956364" />
        public static string ToVerbalWord( this decimal number ) {
            if ( number == 0 ) {
                return "zero";
            }

            if ( number < 0 ) {
                return "minus " + ToVerbalWord( Math.Abs( number ) );
            }

            var intPortion = ( int ) number;
            var fraction = ( number - intPortion ) * 100;
            var decPortion = ( int ) fraction;

            var words = ToVerbalWord( intPortion );
            if ( decPortion <= 0 ) {
                return words;
            }
            words += " and ";
            words += ToVerbalWord( decPortion );
            return words;
        }

        public static IEnumerable< string > ToWords( [ NotNull ] this String sentence ) {
            //TODO try parsing with different splitters?
            // ...do we mabe want the most or least words or avg ?

            #region Testing

            ////Regex r = new Regex( _regsplit );
            ////Regex r = new Regex( @"\b(\w+)\s+\b" ); almost works..
            //Regex r = .Split(    (\$\s*[\d,]+\.\d{2})\b
            // (\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?))$
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(?=\b\$[\d]+\.\d{4}\s+\b)" ).Split( sentence ) );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"([\w(?=\.\W)]+)|(\b\b)|(\$\d+\.\d+)" ).Split( sentence ) );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"([\w]+)" ).Split( sentence ) );

            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"\b((([''/,&\:\(\)\$\+\-\*\w\000-\032])|(-*\d+\.\d+[%]*))+[\s]+)+\b[\w'',%\(\)]+[.!?]([''\s]|$)" ).Split( sentence ) );

            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(\s*\w+\W\s*)" ).Split( sentence ) );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(\s*\$\d+\.\d+\D)" ).Split( sentence ) );

            //Regex r = new Regex( @"(\$\d+\.\d+)" );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( r.Split( sentence ) );

            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(\b\b)|(\$\d+\.\d+)" ).Split( sentence ) );
            #endregion Testing

            if ( sentence == null ) {
                throw new ArgumentNullException( "sentence" );
            }
            var result = RegexByWordBreak.Split( sentence ).ToStrings( " " ).Split( SpaceSplitBy, StringSplitOptions.RemoveEmptyEntries );
            return result;

            //var sb = new StringBuilder( sentence.Length );
            //foreach ( var wrod in Regex_ByWordBreak.Split( sentence ) ) {
            //    sb.AppendFormat( " {0} ", wrod ?? String.Empty );
            //}
            //return sb.ToString().Split( SpaceSplitBy, StringSplitOptions.RemoveEmptyEntries );
        }

        /// <summary>
        ///     Attempt to conver the String into an XmlDocument.
        ///     An empty XmlDocument will be returned if the conversion throws an XmlException
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDoc( this String input ) {
            try {
                var doc = new XmlDocument();
                doc.LoadXml( input );
                return doc;
            }
            catch ( XmlException ) {
                return new XmlDocument();
            }
        }

        /// <summary>
        ///     To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        public static String UTF8ByteArrayToString( this Byte[] characters ) {
            return new UTF8Encoding().GetString( characters );
        }

        /// <summary>
        ///     <para>Remove duplicate words ONLY if the previous word was the same word.</para>
        /// </summary>
        /// <example>
        ///     Example: "My cat cat likes likes to to to eat food." Should become "My cat likes to eat food."
        /// </example>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String WithoutDuplicateWords( String s ) {
            if ( String.IsNullOrEmpty( s ) ) {
                return String.Empty;
            }

            var words = s.ToWords().ToList();
            //if ( 0 == words.Count() ) { return String.Empty; }
            //if ( 1 == words.Count() ) { return words.FirstOrDefault(); }

            var sb = new StringBuilder( words.Count() );
            var prevWord = words.FirstOrDefault();
            sb.Append( prevWord );
            foreach ( var cur in words.Where( cur => !cur.Equals( prevWord ) ) ) {
                sb.AppendFormat( " {0}", cur );
            }
            //for ( int idx = 1; idx < words.Count(); idx++ ) {
            //    String wordA = words[ idx - 1 ];
            //    String wordB = words[ idx ];
            //    if ( !wordB.Equals( wordA ) ) {
            //        sb.Append( " " );
            //        sb.Append( wordB );
            //    }
            //}

            return sb.ToString();
        }

        /// <summary>
        ///     Uses a <see cref="Regex" /> to count the number of words.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WordCount( [ NotNull ] this String input ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            try {
                return Regex.Matches( input, @"[^\ ^\t^\n]+" ).Count;
            }
            catch ( Exception ) {
                return -1;
            }
        }
    }
}
