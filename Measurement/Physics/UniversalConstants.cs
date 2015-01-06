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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian 2015/UniversalConstants.cs" was last cleaned by Rick on 2015/01/02 at 8:16 AM

namespace Librainian.Measurement.Physics {

    using System;
    using Numerics;

    public static class UniversalConstants {

        /// <summary>
        /// </summary>
        /// <seealso cref="http://wikipedia.org/wiki/Elementary_charge"/>
        public static readonly ElectronVolts ElementaryCharge = new ElectronVolts( 0.0000000000000000001602176565m );

        public static readonly Decimal HalfSpin = ( Decimal )new BigRational( 1, 2 );

        /// <summary>
        /// Precalculated -1/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static readonly ElectronVolts NegativeOneThirdElementaryCharge = ( -1m * ElementaryCharge ) / 3m;

        /// <summary>
        /// Precalculated -2/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static readonly ElectronVolts NegativeTwoThirdsElementaryCharge = ( -2m * ElementaryCharge ) / 3m;

        /// <summary>
        /// Precalculated 1/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static readonly ElectronVolts PositiveOneThirdElementaryCharge = ( 1m * ElementaryCharge ) / 3m;

        /// <summary>
        /// Precalculated +2/3 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static readonly ElectronVolts PositiveTwoThirdsElementaryCharge = ( 2m * ElementaryCharge ) / 3m;

        /// <summary>
        /// Precalculated +0 of <see cref="ElementaryCharge"/>.
        /// </summary>
        public static readonly ElectronVolts ZeroElementaryCharge = 0.0M * ElementaryCharge;
    }
}