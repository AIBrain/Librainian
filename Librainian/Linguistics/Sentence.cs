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
// File "Sentence.cs" last formatted on 2021-01-01 at 9:38 AM.

#nullable enable

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Measurement;
	using Newtonsoft.Json;
	using Parsing;

	/// <summary>A <see cref="Sentence" /> is an ordered sequence of <see cref="Word" /> .</summary>
	/// <see cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></see>
	/// <see cref="Paragraph"></see>
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public class Sentence : IEquatable<Sentence>, IEnumerable<Word>, IComparable<Sentence>, IHasCitations {

		/// <summary></summary>
		[JsonProperty]
		private List<Word> Words { get; } = new();

		public static Sentence Empty { get; }

		public static String EndOfSentence { get; } = new( Char.MaxValue, 2 );

		public static String StartOfSentence { get; } = new( Char.MinValue, 2 );

		public Lazy<HashSet<ICitation>?>? Citations { get; set; }

		private Sentence() => throw new InvalidOperationException();

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="sentence"></param>
		private Sentence( String sentence ) : this( sentence.ToWords() ) { }

		static Sentence() => Empty = Parse( $"{StartOfSentence}{String.Empty}{EndOfSentence}" );

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="words"></param>
		public Sentence( IEnumerable<Word> words ) {
			foreach ( var word in words ) {
				this.Words.Add( word );
			}
		}

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="words"></param>
		public Sentence( IEnumerable<String> words ) {
			var sentence = words.ToStrings( ParsingConstants.Strings.Singlespace );

			foreach ( var word in sentence.ToWords() ) {
				this.Words.Add( word );
			}
		}

		public static Int32 Compare( Sentence? left, Sentence? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return Order.Same;
			}

			if ( left is null || right is null ) {
				return Order.Before; //TODO needs tested
			}

			return left.CompareTo( right );
		}

		public static Boolean Equals( Sentence? left, Sentence? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.Words.SequenceEqual( right.Words );
		}

		public static Boolean operator !=( Sentence? left, Sentence? right ) => !Equals( left, right );

		public static Boolean operator <( Sentence? left, Sentence? right ) => Compare( left, right ) < 0;

		public static Boolean operator ==( Sentence? left, Sentence? right ) => Equals( left, right );

		public static Boolean operator >( Sentence? left, Sentence? right ) => Compare( left, right ) > 0;

		public static Sentence Parse( String sentence ) => new( sentence );

		public Int32 CompareTo( Sentence? other ) => String.Compare( this.ToString(), other?.ToString(), StringComparison.Ordinal );

		public Boolean Equals( Sentence? other ) => Equals( this, other );

		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
		///     <see langword="false" />.
		/// </returns>
		public override Boolean Equals( Object? obj ) => ReferenceEquals( this, obj ) || obj is Sentence other && this.Equals( other );

		public IEnumerable<ICitation>? GetCitations() => this.Citations?.Value;

		public IEnumerator<Word> GetEnumerator() => this.Words.GetEnumerator();

		//[NotNull]public IEnumerable<Sentence> Possibles() => this.Words.ToArray().FastPowerSet().Select( words => new Sentence( words ) ).Where( sentence => !sentence.ToString().IsNullOrEmpty() );
		public override Int32 GetHashCode() => this.Words.GetHashCode();

		public override String ToString() => this.Words.ToStrings( ParsingConstants.Strings.Singlespace );

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}