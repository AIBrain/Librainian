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
// "Librainian2/Belief.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Knowledge {
    using System;
    using System.Collections.Concurrent;

    // an ontology for a domain that cannot or should not be reduced to anything more basic.

    /// <summary>
    ///     Belief is the psychological state in which an individual holds a proposition or premise to be true.
    /// </summary>
    /// <remarks>
    ///     http://wikipedia.org/wiki/Belief
    /// </remarks>
    public class Belief {
        public readonly ConcurrentBag< BasicBelief > BasedUponTheseBeliefs = new ConcurrentBag< BasicBelief >();

        private Double _strengthInBelief;

        public Boolean IsBasicBelief {
            get {
                this._strengthInBelief += 1;
                return 1 == this.BasedUponTheseBeliefs.Count;
            }
        }
    }
}
