// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ICoinWallet.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/ICoinWallet.cs" was last formatted by Protiguous on 2018/05/24 at 7:25 PM.

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