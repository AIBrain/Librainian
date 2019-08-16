// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ICoinWallet.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "ICoinWallet.cs" was last formatted by Protiguous on 2019/08/08 at 8:42 AM.

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