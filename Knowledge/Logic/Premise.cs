// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Premise.cs",
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
// "Librainian/Librainian/Premise.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Knowledge.Logic {

    using System;

    /// <summary>
    ///     In logic, an argument is a set of one or more declarative sentences (or "propositions")
    ///     known as the premises along with another declarative sentence (or "proposition") known as
    ///     the conclusion. Aristotle held that any logical argument could be reduced to two premises
    ///     and a conclusion.
    /// </summary>
    /// <remarks>http: //wikipedia.org/wiki/Premise</remarks>
    /// <example>
    ///     Premises are sometimes left unstated in which case they are called missing premises, for
    ///     example: Socrates is mortal, since all men are mortal. It is evident that a tacitly
    ///     understood claim is that Socrates is a man. The fully expressed reasoning is thus:
    ///     Since all men are mortal and Socrates is a man, it follows that Socrates is mortal.
    ///     In this example, the first two independent clauses preceding the comma (namely,
    ///     "all men are mortal" and "Socrates is a man") are the premises, while "Socrates is
    ///     mortal" is the conclusion. The proof of a conclusion depends on both the truth of
    ///     the premises and the validity of the argument
    /// </example>
    public class Premise {

        public Double ProbabilityOfBeingTrue;

        public Statement FirstStatement { get; private set; }

        public Statement SecondStatement { get; private set; }
    }
}