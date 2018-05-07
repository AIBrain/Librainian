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
// "Librainian/Constants.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement {

    using System;
    using System.Numerics;
    using JetBrains.Annotations;
    using Numerics;

    public static class Constants {

        /// <summary>
        ///     <para>This value is too high. like really high.</para>
        /// </summary>
        [NotNull]
        public const String ValueIsTooHigh = "this value is too high. like really high.";

        /// <summary>
        ///     <para>This value is too low. like really low.</para>
        /// </summary>
        [NotNull]
        public const String ValueIsTooLow = "this value is too low. like really low.";

        /// <summary><see cref="BigInteger" /> copy of System.Decimal.MaxValue (79228162514264337593543950335M)</summary>
        public static readonly BigInteger MaximumUsefulDecimal = new BigInteger( Decimal.MaxValue );

        /// <summary><see cref="BigInteger" /> copy of System.Decimal.MinValue (-79228162514264337593543950335M)</summary>
        public static readonly BigInteger MinimumUsefulDecimal = new BigInteger( Decimal.MinValue );

        public static readonly BigRational OneOverTwo = new BigRational( 1, 2 ); //TODO overkill. But correct. lol.
    }
}