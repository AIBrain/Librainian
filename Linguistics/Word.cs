// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Word.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Word.cs" was last formatted by Protiguous on 2018/05/24 at 7:19 PM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Collections;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public static class WordTests {

        [Test]
        public static void TestWordStuff() {

            //Word.Level.Should().BeGreaterThan( Character.Level );
        }
    }

    /// <summary>
    ///     A <see cref="Word" /> is a sequence of <see cref="Char" /> .
    /// </summary>
    /// <seealso cref="Sentence"></seealso>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public class Word : IEquatable<Word>, IEnumerable<Char>, IComparable<Word> {

        [NotNull]
        [JsonProperty]
        private List<Char> Chars { get; } = new List<Char>();

        public static Word Empty { get; } = new Word();

        private Word() { }

        public Word( [CanBeNull] String word ) {
            if ( String.IsNullOrEmpty( word ) ) { word = String.Empty; }

            this.Chars.AddRange( word.Select( character => character ) );
            this.Chars.Fix();
        }

        public static implicit operator String( Word word ) => word.Chars.ToStrings( "" );

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        ///     Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance
        ///     occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows
        ///     <paramref name="other" /> in the sort order.
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param>
        public Int32 CompareTo( Word other ) => String.Compare( this.ToString(), other.ToString(), StringComparison.Ordinal );

        public Boolean Equals( [CanBeNull] Word other ) {
            if ( other is null ) { return false; }

            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public IEnumerator<Char> GetEnumerator() => this.Chars.GetEnumerator();

        public override Int32 GetHashCode() => this.Chars.GetHashCode();

        public Char[][] Possibles() => this.Chars.ToArray().FastPowerSet();

        public override String ToString() => this.Chars.ToStrings( "" );

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}