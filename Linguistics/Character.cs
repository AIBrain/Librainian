// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Character.cs" was last cleaned by Rick on 2014/10/21 at 5:02 AM

namespace Librainian.Linguistics {

    using System;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;
    using FluentAssertions;

    /// <summary>
    /// <para>A <see cref="Character" /> is a typographic character.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Character_(computing)#char"></seealso>
    /// <seealso cref="Word"></seealso>
    [DataContract( IsReference = true )]
    [Immutable]
    public sealed class Character : IEquatable<Character>, IEquatable<Char> {
        public const UInt64 Level = 1;

        /// <summary>
        /// <para>Represents a character as a UTF-16 code unit.</para>
        /// </summary>
        [DataMember]
        public readonly Char Token;

        static Character() {
            Level.Should().BeLessThan( Word.Level );
        }

        public Character( Char character ) {
            this.Token = character;
        }

        public static implicit operator char( Character character ) => character.Token;

        public Boolean Equals( Char other ) => Equals( this.Token, other );

        public Boolean Equals( [CanBeNull] Character other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || Equals( this.Token, other.Token );
        }

        public override string ToString() => string.Format( "{0}", this.Token );
    }
}