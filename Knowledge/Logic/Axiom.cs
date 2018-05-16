// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Axiom.cs",
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
// "Librainian/Librainian/Axiom.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Knowledge.Logic {

    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     An <see cref="Axiom" /> or Postulate is a proposition that is not proved or
    ///     demonstrated but considered to be either self-evident, or subject to necessary decision.
    ///     That is to say, an axiom is a logical statement that is assumed to be true. Therefore, its
    ///     truth is taken for granted, and serves as a starting point for deducing and inferring other
    ///     (theory dependent) truths. <seealso cref="http://wikipedia.org/wiki/Postulate" />
    /// </summary>
    [JsonObject]
    public class Axiom {

        [JsonProperty]
        public String Description { get; set; }
    }
}