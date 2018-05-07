// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Word.cs" was last cleaned by Protiguous on 2016/08/26 at 10:14 AM

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

        public Word( [CanBeNull] String word ) {
            if ( String.IsNullOrEmpty( word ) ) {
                word = String.Empty;
            }
            this.Chars.AddRange( word.Select( character => character ) );
            this.Chars.Fix();
        }

        private Word() {
        }

        public static Word Empty { get; } = new Word();

        [NotNull]
        [JsonProperty]
        private List<Char> Chars { get; } = new List<Char>();

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
            if ( other is null ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public IEnumerator<Char> GetEnumerator() => this.Chars.GetEnumerator();

        public override Int32 GetHashCode() => this.Chars.GetHashCode();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Char[][] Possibles() => this.Chars.ToArray().FastPowerSet();

		public override String ToString() => this.Chars.ToStrings( "" );
    }
}
