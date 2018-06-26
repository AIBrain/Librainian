// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "PartsOfSpeech.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "PartsOfSpeech.cs" was last formatted by Protiguous on 2018/06/26 at 1:15 AM.

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