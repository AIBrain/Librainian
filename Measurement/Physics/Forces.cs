// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Forces.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Physics {

    using System;

    /// <summary></summary>
    /// <seealso cref="http://wikipedia.org/wiki/Fundamental_interaction" />
    [Flags]
    public enum Forces {

        /// <summary></summary>
        /// <seealso cref="http://wikipedia.org/wiki/Gravitation" />
        Gravitation = 0x1,

        /// <summary>http: //wikipedia.org/wiki/Electromagnetic_force</summary>
        ElectromagneticForce = 0x2,

        /// <summary>http: //wikipedia.org/wiki/Strong_interaction</summary>
        StrongInteraction = 0x4,

        /// <summary>http: //wikipedia.org/wiki/Weak_interaction</summary>
        WeakInteraction = 0x8
    }
}