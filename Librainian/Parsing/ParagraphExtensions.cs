// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Parsing {

    using System;
    using System.Linq;
    using Collections.Extensions;
    using JetBrains.Annotations;
    using Linguistics;
    using Maths;

    public static class ParagraphExtensions {

        public const String FakeLatin =
            "lorem ipsum dolor sit amet consectetuer adipiscing elit sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat ut wisi enim ad minim veniam quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi Ut wisi enim ad minim veniam quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi";

        [NotNull]
        public static String FakeLatinSentence( UInt16 wordCount ) {
            var words = Sentence.Parse( FakeLatin.Split( ' ' ).OrderBy( s => Randem.Next() ).Take( wordCount )
                                                           .ToStrings( " " ) );

            var sentence = words.ToString();

            return $"{sentence.Capitialize()?.TrimEnd()}.";
        }

        //TODO

        //public static String FakeLatinParagraphs( this HtmlHelper helper, UInt16 paragraphCount, UInt16 wordsPerParagraph, String beforeParagraph, String afterParagraph ) {
        //    var paragraphs = new StringBuilder();

        //    for ( var n = 0; n < paragraphCount; n++ ) {
        //        paragraphs.AppendFormat( "\n{0}\n", beforeParagraph );
        //        paragraphs.AppendFormat( "\t{0}", helper.FakeLatinSentence( wordsPerParagraph ) );
        //        paragraphs.AppendFormat( "\n{0}\n", afterParagraph );
        //    }

        //    return paragraphs.ToString();
        //}

        //public static String FakeLatinTitle( this HtmlHelper helper, UInt16 wordCount ) {
        //TODO
        //    var title = helper.FakeLatinSentence( wordCount );

        //    title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( title );
        //    title = title.Substring( 0, title.Length - 1 ); // kill period from paragraph
        //    return title;
        //}

    }

}