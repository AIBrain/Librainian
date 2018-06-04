// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Page.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Page.cs" was last formatted by Protiguous on 2018/06/04 at 4:02 PM.

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A <see cref="Page" /> is a sequence of <see cref="Paragraph" /> .</para>
	/// </summary>
	/// <seealso cref="Book"></seealso>
	[JsonObject]
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public sealed class Page : IEquatable<Page>, IEnumerable<Paragraph> {

		public IEnumerator<Paragraph> GetEnumerator() => this.Paragraphs.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public Boolean Equals( [CanBeNull] Page other ) {
			if ( other is null ) { return false; }

			return ReferenceEquals( this, other ) || this.Paragraphs.SequenceEqual( other.Paragraphs );
		}

		public static Page Empty { get; } = new Page();

		[NotNull]
		[JsonProperty]
		private List<Paragraph> Paragraphs { get; } = new List<Paragraph>();

		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override Int32 GetHashCode() => this.Paragraphs.GetHashCode();

		private Page() { }

		public Page( [NotNull] IEnumerable<Paragraph> paragraphs ) {
			if ( paragraphs is null ) { throw new ArgumentNullException( nameof( paragraphs ) ); }

			this.Paragraphs.AddRange( paragraphs.Where( paragraph => paragraph != null ) );
		}

	}

}