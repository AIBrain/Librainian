﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "CoinWallet.cs",
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
// "Librainian/Librainian/CoinWallet.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;
    using Collections;
    using Financial;
    using Financial.Containers.Wallets;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    ///     My first go at a thread-safe CoinWallet class for bitcoin coins. It's more pseudocode for learning than for
    ///     production.. Use at your own risk. Any tips or ideas? Any dos or dont's? Email me!
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class CoinWallet : ABetterClassDispose, IEnumerable<KeyValuePair<ICoin, UInt64>>, ICoinWallet {

        /// <summary>
        ///     Count of each <see cref="ICoin" />.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<ICoin, UInt64> _coins = new ConcurrentDictionary<ICoin, UInt64>();

        private ActionBlock<BitcoinTransactionMessage> Actor { get; set; }

        /// <summary>
        ///     Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.
        /// </summary>
        [NotNull]
        public IEnumerable<ICoin> Coins => this._coins.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

        [NotNull]
        public IEnumerable<KeyValuePair<ICoin, UInt64>> CoinsGrouped => this._coins;

        public Guid ID { get; }

        public Action<KeyValuePair<ICoin, UInt64>> OnDeposit { get; set; }

        public Action<KeyValuePair<ICoin, UInt64>> OnWithdraw { get; set; }

        [JsonProperty]
        public WalletStatistics Statistics { get; } = new WalletStatistics();

        /// <summary>
        ///     Return the total amount of money contained in this <see cref="CoinWallet" />.
        /// </summary>
        public Decimal Total => this._coins.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );

        private CoinWallet( Guid id ) {
            this.ID = id;

            this.Actor = new ActionBlock<BitcoinTransactionMessage>( message => {
                switch ( message.TransactionType ) {
                    case TransactionType.Deposit:
                        this.Deposit( message.Coin, message.Quantity );

                        break;

                    case TransactionType.Withdraw:
                        this.TryWithdraw( message.Coin, message.Quantity );

                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }, Blocks.ManyProducers.ConsumeSerial );
        }

        /// <summary>
        ///     Create an empty wallet with the given <paramref name="id" />. If the given <paramref name="id" /> is null or
        ///     <see cref="Guid.Empty" />, a new random <paramref name="id" /> is generated.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NotNull]
        public static CoinWallet Create( Guid? id = null ) {
            if ( !id.HasValue || id.Value == Guid.Empty ) { id = Guid.NewGuid(); }

            return new CoinWallet( id: id.Value );
        }

        public Boolean Contains( ICoin coin ) {
            if ( coin is null ) { throw new ArgumentNullException( nameof( coin ) ); }

            return this._coins.ContainsKey( coin );
        }

        public UInt64 Count( ICoin coin ) {
            if ( coin is null ) { throw new ArgumentNullException( nameof( coin ) ); }

            return this._coins.TryGetValue( coin, out var result ) ? result : UInt64.MinValue;
        }

        public UInt64 Deposit( [CanBeNull] ICoin coin, UInt64 quantity, Boolean updateStatistics = true ) {
            if ( null == coin ) { return 0; }

            try {
                lock ( this._coins ) {
                    UInt64 newQuantity = 0;

                    if ( !this._coins.ContainsKey( coin ) ) {
                        if ( this._coins.TryAdd( coin, quantity ) ) { newQuantity = quantity; }
                    }
                    else { newQuantity = this._coins[coin] += quantity; }

                    return newQuantity;
                }
            }
            finally {
                if ( updateStatistics ) { this.Statistics.AllTimeDeposited += coin.FaceValue * quantity; }

                var onDeposit = this.OnDeposit;
                onDeposit?.Invoke( new KeyValuePair<ICoin, UInt64>( coin, quantity ) );
            }
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Statistics.Dispose();

        public IEnumerator<KeyValuePair<ICoin, UInt64>> GetEnumerator() => this._coins.GetEnumerator();

        public override String ToString() {

            var coins = this._coins.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

            return $"฿{this.Total:F8} (in {coins:N0} coins)";
        }

        /// <summary>
        ///     Attempt to <see cref="TryWithdraw(ICoin,UInt64)" /> one or more <see cref="ICoin" /> from this
        ///     <see cref="CoinWallet" /> .
        /// </summary>
        /// <param name="coin">    </param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean TryWithdraw( ICoin coin, UInt64 quantity ) {
            if ( coin is null ) { throw new ArgumentNullException( nameof( coin ) ); }

            if ( quantity <= 0 ) { return false; }

            lock ( this._coins ) {
                if ( !this._coins.ContainsKey( coin ) || this._coins[coin] < quantity ) {
                    return false; //no coins to withdraw!
                }

                this._coins[coin] -= quantity;
            }

            var onWithdraw = this.OnWithdraw;
            onWithdraw?.Invoke( new KeyValuePair<ICoin, UInt64>( coin, quantity ) );

            return true;
        }

        [CanBeNull]
        public ICoin TryWithdrawAnyCoin() {
            var possibleCoins = this._coins.Where( pair => pair.Value > 0 ).ToList();

            if ( !possibleCoins.Any() ) { return default; }

            possibleCoins.Shuffle();
            var coin = possibleCoins.First();

            return this.TryWithdraw( coin.Key, 1 ) ? coin.Key : default;
        }

        [CanBeNull]
        public ICoin TryWithdrawSmallestCoin() {
            var coin = this._coins.Where( pair => pair.Value > 0 ).Select( pair => pair.Key ).OrderBy( coin1 => coin1.FaceValue ).FirstOrDefault();

            if ( coin == default( ICoin ) ) { return default; }

            return this.TryWithdraw( coin, 1 ) ? coin : default;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}