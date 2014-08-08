#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/ICoinWallet.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Measurement.Currency.BTC {
    using System;
    using System.Collections.Generic;
    using Annotations;

    public interface ICoinWallet {
        IEnumerable< KeyValuePair< ICoin, ulong > > CoinsGrouped { [NotNull] get; }

        Action< KeyValuePair< ICoin, UInt64 > > OnDeposit { get; set; }
        Action< KeyValuePair< ICoin, UInt64 > > OnWithdraw { get; set; }

        [UsedImplicitly]
        String Formatted { get; }

        /// <summary>
        ///     Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.
        /// </summary>
        IEnumerable< ICoin > Coins { [NotNull] get; }

        Guid ID { get; }

        /// <summary>
        ///     Return the total amount of money contained in this <see cref="CoinWallet" />.
        /// </summary>
        Decimal Total { get; }

        /// <summary>
        ///     Attempt to <see cref="CoinWallet.TryWithdraw(ICoin,ulong)" /> one or more <see cref="ICoin" /> from this
        ///     <see cref="CoinWallet" />
        ///     .
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        Boolean TryWithdraw( [NotNull] ICoin coin, UInt64 quantity );

        IEnumerator< KeyValuePair< ICoin, UInt64 > > GetEnumerator();

        Boolean Contains( [NotNull] ICoin coin );

        UInt64 Count( [NotNull] ICoin coin );

        ///// <summary>
        /////     Deposit one or more <paramref name="coin" /> into this <see cref="Wallet" />.
        ///// </summary>
        ///// <param name="coin"></param>
        ///// <param name="quantity"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        ///// <remarks>Locks the wallet.</remarks>
        //Boolean Deposit( [NotNull] ICoin coin, UInt64 quantity, Guid? id = null );
    }
}
