// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "IniLine.cs" last formatted on 2020-08-28 at 1:45 AM.

#nullable enable

namespace Librainian.Persistence.InIFiles {

	using System;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Parsing;

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

		public enum LineTypes {

			Empty,
			Text,
			Comment

		}

		public const String PairSeparator = "=";

		public static readonly String[] CommentHeaders = {
			"#", ";", ":", "rem", "REM"
		};

		public IniLine( [CanBeNull]
		                String? key, [CanBeNull]
		                String? value = default ) {

			key = key.Trimmed() ?? String.Empty;

			var test = key.StartsWith( CommentHeaders, StringComparison.Ordinal );

			if ( test.status.IsGood() ) {
				this.Key = key;
				this.Value = value;
				this.LineType = LineTypes.Comment;
				this.LineHeader = test.start;

				return;
			}

			this.Key = key;
			this.Value = value;

			if ( String.IsNullOrEmpty( key ) || String.IsNullOrEmpty( value ) ) {
				this.LineType = LineTypes.Empty;
				this.Value = default;
			}
			else {
				this.LineType = LineTypes.Text;
			}
		}

		/// <summary>
		///     The prefix for lines.
		/// </summary>
		[JsonProperty]
		[CanBeNull]
		public String? LineHeader { get; set; }

		[JsonProperty]
		[CanBeNull]
		public String? Key { get; }

		[JsonProperty]
		public LineTypes LineType { get; }

		[JsonProperty]
		[CanBeNull]
		public String? Value { get; set; }

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() =>
			this.LineType switch {
				LineTypes.Text => $"{this.Key}{PairSeparator}{this.Value}",
				LineTypes.Comment => $"{this.LineHeader} {this.Key}",
				LineTypes.Empty => $"{String.Empty}",
				_ => throw new ArgumentOutOfRangeException()
			};

	}

}