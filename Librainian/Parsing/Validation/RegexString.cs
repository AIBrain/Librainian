// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "RegexString.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "RegexString.cs" was last formatted by Protiguous on 2018/12/01 at 4:45 PM.

namespace Librainian.Parsing.Validation {

	using System;
	using System.Text.RegularExpressions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     Based from https://codereview.stackexchange.com/questions/208291/enforcing-string-validity-with-the-c-type-system
	/// </summary>
	[Serializable]
	[JsonObject]
	public abstract class RegexString : ValidatedString {

		private Regex _regex;

		protected abstract Boolean AllowNull { get; }

		protected abstract String RegexValidation { get; }

		[NotNull]
		public override String Requirements => $"match the Regular Expression: {this.RegexValidation}";

		public RegexString( String value ) : base( value ) { }

		protected override Boolean IsValid( [CanBeNull] String value ) {
			if ( this._regex == null ) {
				this._regex = new Regex( this.RegexValidation );
			}

			if ( value == null ) {
				return this.AllowNull;
			}

			return this._regex.IsMatch( value );
		}
	}
}