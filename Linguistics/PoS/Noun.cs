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
// "Librainian/Noun.cs" was last cleaned by Protiguous on 2016/08/26 at 10:14 AM

namespace Librainian.Linguistics.PoS {

    using System;
    using System.Linq;
    using Newtonsoft.Json;

    [JsonObject]
    public class Noun : Word, ITaggedWord {

        public Noun( String word ) : base( word ) {
        }

        public PartsOfSpeech PartOfSpeech => PartsOfSpeech.Noun;

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter;
        ///     otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals( Noun other ) {
            if ( other is null ) {
                return false;
            }

            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }
    }
}
