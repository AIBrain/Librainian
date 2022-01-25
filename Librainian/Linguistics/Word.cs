// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Word.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Linguistics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Extensions;
using Newtonsoft.Json;
using Parsing;

/// <summary>One word. Should be case-sensitive. <see cref="Equals(Word,Word)" /></summary>
/// <see cref="Sentence"></see>
[JsonObject]
[Immutable]
[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
[Serializable]
public record Word : IEnumerable<Char>, IComparable<Word> {
	private Word() : this( String.Empty ) { }

	public Word( String word ) => this.Value = word.Trimmed() ?? String.Empty;

	[JsonProperty]
	public String Value { get; init; }

	public static Word Empty { get; } = new();

	public Int32 CompareTo( Word? other ) => String.Compare( this.Value, other?.Value, StringComparison.Ordinal );

	public IEnumerator<Char> GetEnumerator() => this.Value.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => this.Value.GetEnumerator();

	public virtual Boolean Equals( Word? other ) => Equals( this, other );

	public static Boolean Equals( Word? left, Word? right ) => String.Equals( left?.Value, right?.Value, StringComparison.Ordinal );

	public static implicit operator String( Word word ) => word.Value;

	public override String ToString() => this.Value;

	/// <summary>Return a jagged array of every possible combination.</summary>
	public Char[][] PowerSet() => this.Value.ToArray().PowerSet();

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public static Boolean operator <( Word left, Word right ) => left.CompareTo( right ) < 0;

	public static Boolean operator <=( Word left, Word right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >( Word left, Word right ) => left.CompareTo( right ) > 0;

	public static Boolean operator >=( Word left, Word right ) => left.CompareTo( right ) >= 0;
}