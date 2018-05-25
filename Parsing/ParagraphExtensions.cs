// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ParagraphExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/ParagraphExtensions.cs" was last formatted by Protiguous on 2018/05/24 at 7:30 PM.

namespace Librainian.Parsing {

    using System;
    using System.Linq;
    using Collections;
    using Linguistics;
    using Maths;

    public static class ParagraphExtensions {

        public const String FakeLatin =
            "lorem ipsum dolor sit amet consectetuer adipiscing elit sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat ut wisi enim ad minim veniam quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi Ut wisi enim ad minim veniam quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi";

        public static String FakeLatinSentence( UInt16 wordCount ) {
            var words = new Sentence( FakeLatin.Split( ' ' ).OrderBy( s => Randem.Next() ).Take( wordCount ).ToStrings( " " ) );
            var sentence = words.ToString();

            return $"{Char.ToUpperInvariant( sentence[0] )}{sentence.Substring( 1 ).TrimEnd()}.";
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

        //    title = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase( title );
        //    title = title.Substring( 0, title.Length - 1 ); // kill period from paragraph
        //    return title;
        //}
    }
}