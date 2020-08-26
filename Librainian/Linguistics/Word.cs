﻿// Copyright © Protiguous. All Rights Reserved.
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
// File "Word.cs" last formatted on 2020-08-14 at 8:35 PM.

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Parsing;

	/// <summary>One word. Case-sensitive <see cref="Equals(Word,Word)" /></summary>
	/// <see cref="Sentence"></see>
	[JsonObject]
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public class Word : IEquatable<Word>, IEnumerable<Char>, IComparable<Word> {

		private Word() => this.value = Empty;

		public Word( [NotNull] String word ) {
			word = word.Trimmed();
			this.value = String.IsNullOrEmpty( word ) ? String.Empty : word;
		}

		[NotNull]
		[JsonProperty]
		private String value { get; }

		[NotNull]
		public static Word Empty { get; } = new Word( String.Empty );

		public Int32 CompareTo( [NotNull] Word other ) => String.Compare( this.value, other.value, StringComparison.Ordinal );

		public IEnumerator<Char> GetEnumerator() => this.value.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public Boolean Equals( [CanBeNull] Word other ) => Equals( this, other );

		public static Boolean Equals( [CanBeNull] Word left, [CanBeNull] Word right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return default;
			}

			return left.value.Is( right.value );
		}

		[NotNull]
		public static implicit operator String( [NotNull] Word word ) => word.value;

		public override Int32 GetHashCode() => this.value.GetHashCode();

		[NotNull]
		public override String ToString() => this.value;

		//[NotNull]public Char[][] Possibles() => this.Chars.ToArray().FastPowerSet();

	}

}