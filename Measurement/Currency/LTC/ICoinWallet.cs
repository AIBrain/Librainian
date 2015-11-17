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
// "Librainian/ICoinWallet.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Currency.LTC {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    public interface ICoinWallet {

        /// <summary>Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.</summary>
        IEnumerable<ICoin> Coins {
            [NotNull] get;
        }

        IEnumerable<KeyValuePair<ICoin, UInt64>> CoinsGrouped {
            [NotNull] get;
        }

        String Formatted {
            get;
        }

        Guid ID {
            get;
        }

        Action<KeyValuePair<ICoin, UInt64>> OnDeposit {
            get; set;
        }

        Action<KeyValuePair<ICoin, UInt64>> OnWithdraw {
            get; set;
        }

        /// <summary>Return the total amount of money contained in this <see cref="CoinWallet" />.</summary>
        Decimal Total {
            get;
        }

        Boolean Contains([NotNull] ICoin coin);

        UInt64 Count([NotNull] ICoin coin);

        IEnumerator<KeyValuePair<ICoin, UInt64>> GetEnumerator();

        /// <summary>
        /// Attempt to
        /// <see cref="CoinWallet.TryWithdraw(Librainian.Measurement.Currency.BTC.ICoin,UInt64)" />
        /// one or more <see cref="ICoin" /> from this <see cref="CoinWallet" /> .
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        Boolean TryWithdraw([NotNull] ICoin coin, UInt64 quantity);
    }
}