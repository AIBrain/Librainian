// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ICoinWallet.cs",
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
// "Librainian/Librainian/ICoinWallet.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Currency.LTC {

    using System;
    using System.Collections.Generic;
    using BTC;
    using JetBrains.Annotations;

    public interface ICoinWallet {

        /// <summary>Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.</summary>
        IEnumerable<ICoin> Coins { [NotNull] get; }

        IEnumerable<KeyValuePair<ICoin, UInt64>> CoinsGrouped { [NotNull] get; }

        String Formatted { get; }

        Guid ID { get; }

        Action<KeyValuePair<ICoin, UInt64>> OnDeposit { get; set; }

        Action<KeyValuePair<ICoin, UInt64>> OnWithdraw { get; set; }

        /// <summary>Return the total amount of money contained in this <see cref="CoinWallet" />.</summary>
        Decimal Total { get; }

        Boolean Contains( [NotNull] ICoin coin );

        UInt64 Count( [NotNull] ICoin coin );

        IEnumerator<KeyValuePair<ICoin, UInt64>> GetEnumerator();

        /// <summary>
        ///     Attempt to
        ///     <see cref="CoinWallet.TryWithdraw(Librainian.Measurement.Currency.BTC.ICoin,UInt64)" />
        ///     one or more <see cref="ICoin" /> from this <see cref="CoinWallet" /> .
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        Boolean TryWithdraw( [NotNull] ICoin coin, UInt64 quantity );
    }
}