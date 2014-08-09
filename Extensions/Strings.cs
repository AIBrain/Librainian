#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/Strings.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Extensions {

    public static class Strings {

        //public static XmlDocument ToXHTML( this String content ) {
        //    try {
        //        var result = new XmlDocument();
        //        var tidy = new Tidy();
        //        tidy.Options.DocType = DocType.Omit;
        //        tidy.Options.DropEmptyParas = true;
        //        tidy.Options.DropFontTags = true;
        //        tidy.Options.FixBackslash = true;
        //        tidy.Options.HideEndTags = false;
        //        tidy.Options.IndentAttributes = false;
        //        tidy.Options.IndentContent = false;
        //        tidy.Options.LogicalEmphasis = false; //sort of normalize what we get...
        //        //tidy.Options.QuoteAmpersand ???
        //        //tidy.Options.QuoteMarks ???
        //        tidy.Options.QuoteNbsp = false;
        //        tidy.Options.RawOut = false;
        //        tidy.Options.SmartIndent = false;
        //        tidy.Options.UpperCaseAttrs = true;
        //        tidy.Options.UpperCaseTags = true;
        //        tidy.Options.MakeClean = true;
        //        tidy.Options.XmlOut = true;
        //        tidy.Options.Xhtml = true;

        //        using ( var output = new MemoryStream() ) {
        //            using ( var input = new MemoryStream() ) {
        //                var bytes = Encoding.UTF8.GetBytes( content );
        //                input.Write( bytes, 0, bytes.Length );
        //                input.Position = 0;
        //                var tmc = new TidyMessageCollection();
        //                tidy.Parse( input, output, tmc );
        //            }
        //            result.LoadXml( Encoding.UTF8.GetString( output.ToArray() ) );
        //        }
        //        return result;
        //    }
        //    catch ( DomException error ) {
        //        Utility.LogException( error );
        //        return new XmlDocument();
        //    }
        //}

        //public static String ToEnglishFromHTML( this String content ) {
        //    try {
        //        var tidy = new Tidy();
        //        tidy.Options.DocType = DocType.Omit;
        //        tidy.Options.DropEmptyParas = true;
        //        tidy.Options.DropFontTags = true;
        //        tidy.Options.FixBackslash = true;
        //        tidy.Options.HideEndTags = true;
        //        tidy.Options.IndentAttributes = false;
        //        tidy.Options.IndentContent = false;
        //        tidy.Options.LogicalEmphasis = false; //sort of normalize what we get...
        //        tidy.Options.QuoteAmpersand = true;
        //        tidy.Options.QuoteMarks = false;
        //        tidy.Options.QuoteNbsp = false;
        //        tidy.Options.RawOut = false;
        //        tidy.Options.SmartIndent = false;
        //        tidy.Options.UpperCaseAttrs = true;
        //        tidy.Options.UpperCaseTags = true;
        //        tidy.Options.MakeClean = true;
        //        tidy.Options.XmlOut = false;
        //        tidy.Options.Xhtml = false;

        // tidy.Options.AltText = String.Empty; tidy.Options.BreakBeforeBR = true;
        // tidy.Options.EncloseBlockText = true; tidy.Options.EncloseText = true;
        // tidy.Options.LiteralAttribs = false; tidy.Options.Word2000 = true; ////////////////////

        //        using ( var output = new MemoryStream() ) {
        //            using ( var input = new MemoryStream() ) {
        //                var bytes = Encoding.UTF8.GetBytes( content );
        //                input.Write( bytes, 0, bytes.Length );
        //                input.Position = 0;
        //                var tmc = new TidyMessageCollection();
        //                tidy.Parse( input, output, tmc );
        //            }
        //            var result = Encoding.UTF8.GetString( output.ToArray() );
        //            return result;
        //        }
        //    }
        //    catch ( DomException error ) {
        //        Utility.LogException( error );
        //        return String.Empty;
        //    }
        //}
    }
}