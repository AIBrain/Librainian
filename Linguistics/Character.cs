// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Character.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Linguistics {

    using System;
    using System.Diagnostics;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A <see cref="Character" /> is a typographic character.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Character_(computing)#char"></seealso>
    /// <seealso cref="Word"></seealso>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( "{ToString(),nq}" )]
    public sealed class Character : IEquatable<Character>, IEquatable<Char>, IComparable<Character>, IComparable<Char> {
        public const UInt64 Level = 1;

        /// <summary>
        ///     <para>Represents a character as a UTF-16 code unit.</para>
        /// </summary>
        [JsonProperty]
        public readonly Char Token;

        static Character() {
            Level.Should().BeLessThan( Word.Level );
        }

        public Character( Char character ) {
            this.Token = character;
        }

        public static implicit operator Char( Character character ) => character.Token;

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
        public Int32 CompareTo( Char other ) {
            return this.Token.CompareTo( other );
        }

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
        public Int32 CompareTo( Character other ) {
            return this.Token.CompareTo( other.Token );
        }

        public Boolean Equals( Char other ) => Equals( this.Token, other );

        public Boolean Equals( [CanBeNull] Character other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || Equals( this.Token, other.Token );
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>
        ///     A hash code for the current object.
        /// </returns>
        public override Int32 GetHashCode() {
            return this.Token.GetHashCode();
        }

        public override String ToString() => $"{this.Token}";
    }
}