// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TaggedSentence.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "TaggedSentence.cs" was last formatted by Protiguous on 2018/07/10 at 9:14 PM.

namespace Librainian.Linguistics.PoS {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Collections;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	[JsonObject]
	public sealed class TaggedSentence : IEquatable<TaggedSentence>, IEnumerable<ITaggedWord> {

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>
		///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
		///     through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<ITaggedWord> GetEnumerator() => this.Tokens.GetEnumerator();

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>
		///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
		///     through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///     true if the current object is equal to the <paramref name="other" /> parameter;
		///     otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		[Pure]
		public Boolean Equals( TaggedSentence other ) {
			if ( other is null ) { return false; }

			return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
		}

		[JsonProperty]
		public readonly List<ITaggedWord> Tokens = new List<ITaggedWord>();

		public TaggedSentence( [NotNull] IEnumerable<ITaggedWord> words ) {
			if ( words is null ) { throw new ArgumentNullException( nameof( words ) ); }

			this.Tokens.AddRange( words.Where( word => null != word ).Select( word => word ) );
		}

		[Pure]
		public static implicit operator String( [NotNull] TaggedSentence sentence ) => sentence.Tokens.ToStrings( " " );

		[Pure]
		public override String ToString() => this.Tokens.ToStrings( " " );
	}
}