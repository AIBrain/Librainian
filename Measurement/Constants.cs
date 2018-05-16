// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Constants.cs",
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
// "Librainian/Librainian/Constants.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

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