// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "IniLine.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "IniLine.cs" was last formatted by Protiguous on 2019/03/22 at 1:16 PM.

namespace Librainian.Persistence.InIFiles {

    using System;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>
    ///         <example>
    ///             <code>var comment=new IniLine(";Comment");</code>
    ///         </example>
    ///     </para>
    ///     <para>
    ///         <example>
    ///             <code>var commentwithvalue=new IniLine(";Comment","something");</code>
    ///         </example>
    ///     </para>
    ///     <para>
    ///         <example>
    ///             <code>var kvp=new IniLine("Key","value");</code>
    ///         </example>
    ///     </para>
    ///     <para>
    ///         <example>
    ///             <code>var empty=new IniLine("");</code>
    ///         </example>
    ///     </para>
    ///     <para>
    ///         <example>
    ///             <code>var empty=new IniLine();</code>
    ///         </example>
    ///     </para>
    /// </summary>
    [JsonObject]
    public class IniLine {

        public enum LineTipe {
            Empty,
            Text,
            Comment
        }

        [JsonProperty]
        public String Key { get; }

        [JsonProperty]
        public String Value { get; set; }

        [JsonProperty]
        public LineTipe LineType { get; }

        public const String CommentHeader = ";";

        public const String PairSeparator = "=";

        public IniLine( [CanBeNull] String key = default, [CanBeNull] String value = default ) {

            this.Key = key;
            this.Value = value;

            if ( key?.StartsWith( CommentHeader ) == true ) {
                this.LineType = LineTipe.Comment;
            }
            else {
                if ( String.IsNullOrEmpty( key ) || String.IsNullOrEmpty( value ) ) {
                    this.LineType = LineTipe.Empty;
                    this.Value = default;
                }
                else {
                    this.LineType = LineTipe.Text;
                }
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() {
            switch ( this.LineType ) {
                case LineTipe.Text: {
                    return $"{this.Key}{PairSeparator}{this.Value}";
                }
                case LineTipe.Comment: {
                    return $"{this.Key}";
                }
                case LineTipe.Empty: {
                    return $"{String.Empty}";
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

    }

}