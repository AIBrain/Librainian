// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Forces.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

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