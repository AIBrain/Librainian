// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PartsOfSpeech.cs" was last cleaned by Protiguous on 2016/08/26 at 10:14 AM

namespace Librainian.Linguistics.PoS {

    using System.ComponentModel;

    public enum PartsOfSpeech {

        [Description( "A noun is a naming word. It names a person, place, thing, idea, quality, or action." )]
        Noun,

        [Description( "A verb is a word which describes an action (doing something) or a state (being something)." )]
        Verb,

        [Description( "An adjective is a word that describes a noun." )]
        Adjective,

        [Description( "An adverb is a word which usually describes a verb." )]
        Adverb,

        [Description( "A pronoun is used instead of a noun, to avoid repeating the noun." )]
        Pronoun,

        [Description( "A conjunction joins two words, phrases or sentences together." )]
        Conjunction,

        [Description( "A preposition usually comes before a noun, pronoun or noun phrase and then joins the noun to some other part of the sentence." )]
        Preposition,

        [Description( "An interjection is an unusual kind of word, because it often stands alone." )]
        Interjection,

        [Description( "An article is used to introduce a noun." )]
        Article
    }
}
