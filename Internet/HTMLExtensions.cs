namespace Librainian.Internet {
    using System;

    public class HTMLExtensions {
        public const String EmptyHTML5 = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"utf-8\" /><title id=\"title1\"></title></head><body><header id=\"header1\"></header><article id=\"article1\"></article><footer id=\"footer1\"></footer></body></html>";

        /// <summary>
        ///     an empty document.
        /// </summary>
        public static String EmptyHTMLDocument { get { return String.Format( "<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>{0}<html>{1}<head>{2}<title>AIBrain</title>{3}<style>{4}body {{ background-color: gainsboro; font-family: Arial; font-size: 10pt; }}{5}div {{ margin-bottom: 3pt; }}{6}div.Critical {{ color: crimson; font-weight: bolder; }}{7}div.Error {{ color: firebrick; }}{8}div.Warning {{ color: purple; }}{9}div.Information {{ color: green; }}{10}div.Write {{ color: green; }}{11}div.WriteLine {{ color: green; }}{12}div.Verbose {{ color: dimgray; }}{13}div span {{ margin-right: 2px; vertical-align: top; }}{14}div span.Dingbat {{ display: none; }}{15}div span.DateTime {{ display: inline; Single : left; width: 3em; height: auto }}{16}div span.Source {{ display: none; Single : left; width: 8em; height: auto; }}{17}div span.ThreadId {{ display: inline; Single: left; width: 2em; height: auto; text-align: right; }}{18}div span.MessageType {{ display: none; Single: left; width: 6em; height: auto; text-align: left; }}{19}div span.MessageText {{ display: inline; width: 100%; position:relative; }}{20}div.Critical span.MessageText {{ font-weight: bold; }}{21}div span.CallStack {{ display: none; margin-left: 1em; }}{22}</style>{23}</head>{24}<body>{25}</body>{26}</html>{27}", Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine ); } }
    }
}