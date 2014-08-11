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
// "Librainian/CoinWallet.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.BTC {
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks.Dataflow;
    using Annotations;
    using Collections;
    using Maths;
    using NUnit.Framework;
    using Threading;

    /// <summary>
    ///     My first go at a thread-safe CoinWallet class for bitcoin coins.
    ///     It's more pseudocode for learning than for production..
    ///     Use at your own risk.
    ///     Any tips or ideas? Any dos or dont's? Email me!
    /// </summary>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{Formatted,nq}" )]
    public class CoinWallet : IEnumerable< KeyValuePair< ICoin, ulong > >, ICoinWallet {
        /// <summary>
        ///     Count of each <see cref="ICoin" />.
        /// </summary>
        [NotNull] private readonly ConcurrentDictionary< ICoin, UInt64 > _coins = new ConcurrentDictionary< ICoin, UInt64 >();

        [DataMember] public Statistics Statistics;

        private CoinWallet( Guid id ) {
            this.ID = id;
            this.Statistics = new Statistics( id );
            this.Actor = new ActionBlock< BitcoinTransactionMessage >( message => {
                                                                           switch ( message.TransactionType ) {
                                                                               case TransactionType.Deposit:
                                                                                   this.Deposit( message.Coin, message.Quantity );
                                                                                   break;
                                                                               case TransactionType.Withdraw:
                                                                                   this.TryWithdraw( message.Coin, message.Quantity );
                                                                                   break;
                                                                               default:
                                                                                   throw new ArgumentOutOfRangeException();
                                                                           }
                                                                       }, Blocks.ManyProducers.ConsumeSerial );
        }

        private ActionBlock< BitcoinTransactionMessage > Actor { get; set; }

        public IEnumerable< KeyValuePair< ICoin, UInt64 > > CoinsGrouped {
            [NotNull]
            get {
                Assert.NotNull( this._coins );
                return this._coins;
            }
        }

        public Action< KeyValuePair< ICoin, UInt64 > > OnDeposit { get; set; }
        public Action< KeyValuePair< ICoin, UInt64 > > OnWithdraw { get; set; }

        [UsedImplicitly]
        public String Formatted {
            get {
                Assert.NotNull( this._coins );
                var coins = this._coins.Aggregate( 0UL, ( current, pair ) => current + pair.Value );
                return String.Format( "฿{0:f8} (in {1:N0} coins)", this.Total, coins );
            }
        }

        /// <summary>
        ///     Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.
        /// </summary>
        public IEnumerable< ICoin > Coins {
            [NotNull] get { return this._coins.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key ); }
        }

        public Guid ID { get; private set; }

        /// <summary>
        ///     Return the total amount of money contained in this <see cref="CoinWallet" />.
        /// </summary>
        public Decimal Total {
            get {
                var total = this._coins.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue*pair.Value );
                return total;
            }
        }

        /// <summary>
        ///     Attempt to <see cref="TryWithdraw(ICoin,ulong)" /> one or more <see cref="ICoin" /> from this
        ///     <see cref="CoinWallet" />
        ///     .
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean TryWithdraw( [NotNull] ICoin coin, UInt64 quantity ) {
            if ( coin == null ) {
                throw new ArgumentNullException( "coin" );
            }
            if ( quantity <= 0 ) {
                return false;
            }
            lock ( this._coins ) {
                if ( !this._coins.ContainsKey( coin ) || this._coins[ coin ] < quantity ) {
                    return false; //no coins to withdraw!
                }
                this._coins[ coin ] -= quantity;
            }
            var onWithdraw = this.OnWithdraw;
            if ( onWithdraw != null ) {
                onWithdraw( new KeyValuePair< ICoin, ulong >( coin, quantity ) );
            }
            return true;
        }

        public Boolean Contains( [NotNull] ICoin coin ) {
            if ( coin == null ) {
                throw new ArgumentNullException( "coin" );
            }
            return this._coins.ContainsKey( coin );
        }

        public UInt64 Count( [NotNull] ICoin coin ) {
            if ( coin == null ) {
                throw new ArgumentNullException( "coin" );
            }
            ulong result;
            return this._coins.TryGetValue( coin, out result ) ? result : UInt64.MinValue;
        }

        public IEnumerator< KeyValuePair< ICoin, UInt64 > > GetEnumerator() {
            return this._coins.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public ulong Deposit( [CanBeNull] ICoin coin, UInt64 quantity, Boolean updateStatistics = true ) {
            if ( null == coin ) {
                return 0;
            }
            try {
                lock ( this._coins ) {
                    UInt64 newQuantity = 0;
                    if ( !this._coins.ContainsKey( coin ) ) {
                        if ( this._coins.TryAdd( coin, quantity ) ) {
                            newQuantity = quantity;
                        }
                    }
                    else {
                        newQuantity = this._coins[ coin ] += quantity;
                    }
                    return newQuantity;
                }
            }
            finally {
                if ( updateStatistics ) {
                    this.Statistics.AllTimeDeposited += coin.FaceValue*quantity;
                }
                var onDeposit = this.OnDeposit;
                if ( onDeposit != null ) {
                    onDeposit( new KeyValuePair< ICoin, ulong >( coin, quantity ) );
                }
            }
        }

        /// <summary>
        ///     Create an empty wallet with the given <paramref name="id" />.
        ///     If the given <paramref name="id" /> is null or <see cref="Guid.Empty" />, a new random <paramref name="id" /> is
        ///     generated.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NotNull]
        public static CoinWallet Create( Guid? id = null ) {
            if ( !id.HasValue || id.Value == Guid.Empty ) {
                id = Guid.NewGuid();
            }
            return new CoinWallet( id: id.Value );
        }

        [CanBeNull]
        public ICoin TryWithdrawAnyCoin() {
            var possibleCoins = this._coins.Where( pair => pair.Value > 0 ).ToArray();

            if ( !possibleCoins.Any() ) {
                return default( ICoin );
            }

            possibleCoins.Shuffle( Randem.Next );
            var coin = possibleCoins.First();

            return this.TryWithdraw( coin.Key, 1 ) ? coin.Key : default( ICoin );
        }

        [CanBeNull]
        public ICoin TryWithdrawSmallestCoin() {
            var coin = this._coins.Where( pair => pair.Value > 0 ).Select( pair => pair.Key ).OrderBy( coin1 => coin1.FaceValue ).FirstOrDefault();

            if ( coin == default( ICoin ) ) {
                return default( ICoin );
            }

            return this.TryWithdraw( coin, 1 ) ? coin : default( ICoin );
        }

        ///// <summary>
        /////     Deposit one or more <paramref name="denomination" /> into this <see cref="Wallet" />.
        ///// </summary>
        ///// <param name="coin"></param>
        ///// <param name="quantity"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        ///// <remarks>Locks the wallet.</remarks>
        //public Boolean Deposit( [NotNull] ICoin coin, UInt64 quantity, Guid? id = null ) {
        //    if ( coin == null ) {
        //        throw new ArgumentNullException( "coin" );
        //    }
        //    if ( quantity <= 0 ) {
        //        return false;
        //    }

        //    return this.Actor.Post( new TransactionMessage {
        //                                                       Date = DateTime.Now,
        //                                                       Coin = coin,
        //                                                       ID = id ?? Guid.NewGuid(),
        //                                                       Quantity = quantity,
        //                                                       TransactionType = TransactionType.Deposit
        //                                                   } );
        //}

        ///// <summary>
        /////     Create an empty wallet with a new random id.
        ///// </summary>
        ///// <returns></returns>
        //[NotNull]
        //public static Wallet Create() {
        //    return new Wallet( id: Guid.NewGuid() );
        //}
    }
}
