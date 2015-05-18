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
// "Librainian/IWallet.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.USD {
    using System;
    using System.Collections.Generic;
    using Financial;
    using JetBrains.Annotations;

    public interface IWallet {
        IEnumerable< KeyValuePair< ICoin, UInt64 > > CoinsGrouped { [NotNull] get; }

        [UsedImplicitly]
        String Formatted { get; }

        /// <summary>
        ///     Return an expanded list of the <see cref="Notes" /> and <see cref="Coins" /> in this <see cref="Wallet" />.
        /// </summary>
        IEnumerable< IDenomination > NotesAndCoins { [NotNull] get; }

        IEnumerable< KeyValuePair< IBankNote, UInt64 > > NotesGrouped { [NotNull] get; }

        /// <summary>
        ///     Return each <see cref="ICoin" /> in this <see cref="Wallet" />.
        /// </summary>
        IEnumerable< ICoin > Coins { [NotNull] get; }

        /// <summary>
        ///     Return the count of each type of <see cref="Notes" /> and <see cref="Coins" />.
        /// </summary>
        IEnumerable< KeyValuePair< IDenomination, UInt64 > > Groups { [NotNull] get; }

        Guid ID { get; }

        /// <summary>
        ///     Return each <see cref="IBankNote" /> in this <see cref="Wallet" />.
        /// </summary>
        IEnumerable< IBankNote > Notes { get; }

        /// <summary>
        ///     Return the total amount of money contained in this <see cref="Wallet" />.
        /// </summary>
       Decimal Total { get; }

        /// <summary>
        ///     Attempt to <see cref="Wallet.TryWithdraw(IBankNote,UInt64)" /> one or more <see cref="IBankNote" /> from this
        ///     <see cref="Wallet" />.
        /// </summary>
        /// <param name="bankNote"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        Boolean TryWithdraw( [CanBeNull] IBankNote bankNote, UInt64 quantity );

        /// <summary>
        ///     Attempt to <see cref="Wallet.TryWithdraw(ICoin,UInt64)" /> one or more <see cref="ICoin" /> from this
        ///     <see cref="Wallet" />
        ///     .
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        Boolean TryWithdraw( [NotNull] ICoin coin, UInt64 quantity );

        IEnumerator< KeyValuePair< IDenomination, UInt64 > > GetEnumerator();

        Boolean Contains( [NotNull] IBankNote bankNote );

        Boolean Contains( [NotNull] ICoin coin );

        UInt64 Count( [NotNull] IBankNote bankNote );

        UInt64 Count( [NotNull] ICoin coin );

        Boolean TryWithdraw( [CanBeNull] IDenomination denomination, UInt64 quantity );

        /// <summary>
        ///     Deposit one or more <paramref name="denomination" /> into this <see cref="Wallet" />.
        /// </summary>
        /// <param name="denomination"></param>
        /// <param name="quantity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        Boolean Deposit( [NotNull] IDenomination denomination, UInt64 quantity, Guid? id = null );
    }
}
