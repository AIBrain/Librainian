// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ShufflingType.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;

    public enum ShufflingType {

        /// <summary>This one is works best with smaller lists and more iterations.</summary>
        ByHarker,

        /// <summary>
        ///     Uses OrderBy( Random.Next ).ThenBy( Random.Next ). This is _might_ be the fastest for
        ///     larger sets.
        /// </summary>
        ByRandom,

        /// <summary>
        ///     Uses <see cref="ConcurrentBag{T}" />, which can introduce /some/ randomness, but is
        ///     horribly nondeterministic and unrandom. This method is so horrible, that please: Do NOT
        ///     use it. If you *have* to, do many many iterations.
        /// </summary>
        ByBags,

        /// <summary>Uses <see cref="Guid.NewGuid" /> to introduce randomness.</summary>
        ByGuid,

        /// <summary>Let the algorithm choose the optimal.</summary>
        AutoChoice
    }
}