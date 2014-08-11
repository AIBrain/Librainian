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
// "Librainian/Axiom.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Knowledge.Logic {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     An <see cref="Axiom" /> or <see cref="Postulate" /> is a proposition that is not proved or demonstrated but
    ///     considered to be either self-evident, or subject to necessary decision. That is to say, an axiom is a logical
    ///     statement that is assumed to be true. Therefore, its truth is taken for granted, and serves as a starting point for
    ///     deducing and inferring other (theory dependent) truths.
    ///     <seealso cref="http://wikipedia.org/wiki/Postulate" />
    /// </summary>
    [DataContract( IsReference = true )]
    public class Axiom {
        [DataMember]
        public String Description { get; set; }
    }
}
