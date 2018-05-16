// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PartsOfSpeech.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/PartsOfSpeech.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

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