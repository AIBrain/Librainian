#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Premise.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Knowledge.Logic {
    using System;

    /// <summary>
    ///     In logic, an argument is a set of one or more declarative sentences (or "propositions")
    ///     known as the premises along with another declarative sentence (or "proposition") known as the conclusion.
    ///     Aristotle held that any logical argument could be reduced to two premises and a conclusion.
    /// </summary>
    /// <remarks>
    ///     http://wikipedia.org/wiki/Premise
    /// </remarks>
    /// <example>
    ///     Premises are sometimes left unstated in which case they are called missing premises, for example:
    ///     Socrates is mortal, since all men are mortal.
    ///     It is evident that a tacitly understood claim is that Socrates is a man. The fully expressed reasoning is thus:
    ///     Since all men are mortal and Socrates is a man, it follows that Socrates is mortal.
    ///     In this example, the first two independent clauses preceding the comma (namely, "all men are mortal" and "Socrates
    ///     is a man") are the premises, while "Socrates is mortal" is the conclusion.
    ///     The proof of a conclusion depends on both the truth of the premises and the validity of the argument
    /// </example>
    public class Premise {
        public Double ProbabilityOfBeingTrue;

        public Statement FirstStatement { get; private set; }

        public Statement SecondStatement { get; private set; }

        //public BackingStatements
        //public Related
    }
}
