// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Collections.Extensions;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Parsing;

	/// <summary>A <see cref="Sentence" /> is an ordered sequence of <see cref="Word" /> .</summary>
	/// <see cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></see>
	/// <see cref="Paragraph"></see>
	[JsonObject( MemberSerialization.Fields )]
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public sealed class Sentence : IEquatable<Sentence>, IEnumerable<Word>, IComparable<Sentence> {

		/// <summary></summary>
		[NotNull]
		[JsonProperty]
		private List<Word> Words { get; } = new List<Word>();

		[NotNull]
		public static Sentence Empty { get; }

		[NotNull]
		public static String EndOfSentence { get; } = new String( Char.MaxValue, 2 );

		[NotNull]
		public static String StartOfSentence { get; } = new String( Char.MinValue, 2 );

		private Sentence() => throw new InvalidOperationException( "No." );

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="sentence"></param>
		private Sentence( [NotNull] String sentence ) : this( sentence.ToWords().Select( word => new Word( word ) ) ) { }

		static Sentence() => Empty = Parse( $"{StartOfSentence}{EndOfSentence}" );

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="words"></param>
		public Sentence( [NotNull] IEnumerable<Word> words ) {
			if ( words is null ) {
				throw new ArgumentNullException( nameof( words ) );
			}

			foreach ( var word in words.Where( word => word != null ) ) {
				this.Words.Add( word );
			}
		}

		public static Int32 Compare( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) {
			if ( ReferenceEquals( left, right ) ) {
				return 0;
			}

			if ( left is null ) {
				return 1; //TODO needs tested
			}

			if ( right is null ) {
				return -1; //TODO needs tested
			}

			return left.CompareTo( right );
		}

		public static Boolean Equals( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return default;
			}

			return left.Words.SequenceEqual( right.Words );
		}

		public static Boolean operator !=( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) => !Equals( left, right );

		public static Boolean operator <( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) => Compare( left, right ) < 0;

		public static Boolean operator ==( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) => Equals( left, right );

		public static Boolean operator >( [CanBeNull] Sentence left, [CanBeNull] Sentence right ) => Compare( left, right ) > 0;

		[NotNull]
		public static Sentence Parse( String sentence ) => new Sentence( sentence ?? throw new ArgumentNullException( nameof( sentence ) ) );

		public Int32 CompareTo( [CanBeNull] Sentence other ) => String.Compare( this.ToString(), other?.ToString(), StringComparison.Ordinal );

		public Boolean Equals( [CanBeNull] Sentence other ) => Equals( this, other );

		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override Boolean Equals( Object? obj ) => ReferenceEquals( this, obj ) || ( obj is Sentence other && this.Equals( other ) );

		public IEnumerator<Word> GetEnumerator() => this.Words.GetEnumerator();

		public override Int32 GetHashCode() => this.Words.GetHashCode();

		[NotNull]
		public override String ToString() => this.Words.ToStrings( ParsingConstants.Singlespace );

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		//[NotNull]public IEnumerable<Sentence> Possibles() => this.Words.ToArray().FastPowerSet().Select( words => new Sentence( words ) ).Where( sentence => !sentence.ToString().IsNullOrEmpty() );
	}
}