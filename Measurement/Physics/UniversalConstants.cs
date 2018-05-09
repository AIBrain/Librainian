// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/UniversalConstants.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Physics {

    using System;
    using Maths;

    public static class UniversalConstants {

        /// <summary>
        /// </summary>
        /// <seealso cref="http://wikipedia.org/wiki/Elementary_charge"/>
        public static ElectronVolts ElementaryCharge { get; } = new ElectronVolts( 0.0000000000000000001602176565m );

        public static Decimal HalfSpin { get; } = ( Decimal )Constants.OneOverTwo;

        /// <summary>
        /// Precalculated -1/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static ElectronVolts NegativeOneThirdElementaryCharge { get; } = -1m * ElementaryCharge / 3m;

        /// <summary>
        /// Precalculated -2/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static ElectronVolts NegativeTwoThirdsElementaryCharge { get; } = -2m * ElementaryCharge / 3m;

        public static ElectronVolts PositiveOneElementaryCharge { get; } = 1m * ElementaryCharge;

        /// <summary>
        /// Precalculated 1/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static ElectronVolts PositiveOneThirdElementaryCharge { get; } = 1m * ElementaryCharge / 3m;

        public static ElectronVolts PositiveTwoElementaryCharge { get; } = 2m * ElementaryCharge;

        /// <summary>
        /// Precalculated +2/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static ElectronVolts PositiveTwoThirdsElementaryCharge { get; } = 2m * ElementaryCharge / 3m;

        /// <summary>
        /// Precalculated +0 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static ElectronVolts ZeroElementaryCharge { get; } = 0.0M * ElementaryCharge;
    }
}