// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "HTMLExtensions.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/HTMLExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;

    public static class HTMLExtensions {

        public const String EmptyHTML5 =
            "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"utf-8\" /><title id=\"title1\"></title></head><body><header id=\"header1\"></header><article id=\"article1\"></article><footer id=\"footer1\"></footer></body></html>";

        /// <summary>an empty document.</summary>
        public static String EmptyHTMLDocument =>
            $"<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>{Environment.NewLine}<html>{Environment.NewLine}<head>{Environment.NewLine}<title>AIBrain</title>{Environment.NewLine}<style>{Environment.NewLine}body {{ background-color: gainsboro; font-family: Arial; font-size: 10pt; }}{Environment.NewLine}div {{ margin-bottom: 3pt; }}{Environment.NewLine}div.Critical {{ color: crimson; font-weight: bolder; }}{Environment.NewLine}div.Error {{ color: firebrick; }}{Environment.NewLine}div.Warning {{ color: purple; }}{Environment.NewLine}div.Information {{ color: green; }}{Environment.NewLine}div.Write {{ color: green; }}{Environment.NewLine}div.WriteLine {{ color: green; }}{Environment.NewLine}div.Verbose {{ color: dimgray; }}{Environment.NewLine}div span {{ margin-right: 2px; vertical-align: top; }}{Environment.NewLine}div span.Dingbat {{ display: none; }}{Environment.NewLine}div span.DateTime {{ display: inline; Single : left; width: 3em; height: auto }}{Environment.NewLine}div span.Source {{ display: none; Single : left; width: 8em; height: auto; }}{Environment.NewLine}div span.ThreadId {{ display: inline; Single: left; width: 2em; height: auto; text-align: right; }}{Environment.NewLine}div span.MessageType {{ display: none; Single: left; width: 6em; height: auto; text-align: left; }}{Environment.NewLine}div span.MessageText {{ display: inline; width: 100%; position:relative; }}{Environment.NewLine}div.Critical span.MessageText {{ font-weight: bold; }}{Environment.NewLine}div span.CallStack {{ display: none; margin-left: 1em; }}{Environment.NewLine}</style>{Environment.NewLine}</head>{Environment.NewLine}<body>{Environment.NewLine}</body>{Environment.NewLine}</html>{Environment.NewLine}";
    }
}