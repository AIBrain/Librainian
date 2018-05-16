// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "UniversalConstants.cs",
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
// "Librainian/Librainian/UniversalConstants.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Physics {

    using System;
    using Maths;

    public static class UniversalConstants {

        /// <summary>
        /// </summary>
        /// <seealso cref="http://wikipedia.org/wiki/Elementary_charge" />
        public static ElectronVolts ElementaryCharge { get; } = new ElectronVolts( 0.0000000000000000001602176565m );

        public static Decimal HalfSpin { get; } = ( Decimal )Constants.OneOverTwo;

        /// <summary>
        ///     Precalculated -1/3 of <see cref="ElementaryCharge" />.
        /// </summary>
        public static ElectronVolts NegativeOneThirdElementaryCharge { get; } = -1m * ElementaryCharge / 3m;

        /// <summary>
        ///     Precalculated -2/3 of <see cref="ElementaryCharge" />.
        /// </summary>
        public static ElectronVolts NegativeTwoThirdsElementaryCharge { get; } = -2m * ElementaryCharge / 3m;

        public static ElectronVolts PositiveOneElementaryCharge { get; } = 1m * ElementaryCharge;

        /// <summary>
        ///     Precalculated 1/3 of <see cref="ElementaryCharge" />.
        /// </summary>
        public static ElectronVolts PositiveOneThirdElementaryCharge { get; } = 1m * ElementaryCharge / 3m;

        public static ElectronVolts PositiveTwoElementaryCharge { get; } = 2m * ElementaryCharge;

        /// <summary>
        ///     Precalculated +2/3 of <see cref="ElementaryCharge" />.
        /// </summary>
        public static ElectronVolts PositiveTwoThirdsElementaryCharge { get; } = 2m * ElementaryCharge / 3m;

        /// <summary>
        ///     Precalculated +0 of <see cref="ElementaryCharge" />.
        /// </summary>
        public static ElectronVolts ZeroElementaryCharge { get; } = 0.0M * ElementaryCharge;
    }
}