// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Wallet.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Wallet.cs" was last formatted by Protiguous on 2019/08/08 at 7:27 AM.

#pragma warning disable RCS1138 // Add summary to documentation comment.

namespace Librainian.Financial.Containers.Wallets {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Currency;
    using Currency.BankNotes;
    using Currency.Coins;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Measurement.Currency;
    using Newtonsoft.Json;

    /// <summary>
    ///     My go at a thread-safe Wallet class for US dollars and coins. It's more pseudocode for learning than for
    ///     production.. Use at your own risk. Any tips or ideas? Any dos or don'ts? Email me!
    /// </summary>
    /// <see cref="SimpleWallet" />
    /// <see cref="Measurement.Currency.BTC.SimpleBitcoinWallet" />
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Wallet : ABetterClassDispose, IEnumerable<KeyValuePair<IDenomination, UInt64>> {

        public IEnumerator<KeyValuePair<IDenomination, UInt64>> GetEnumerator() => this.GetGroups().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     Count of each <see cref="IBankNote" />.
        /// </summary>
        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<IBankNote, UInt64> BankNotes { get; } = new ConcurrentDictionary<IBankNote, UInt64>();

        /// <summary>
        ///     Count of each <see cref="ICoin" />.
        /// </summary>
        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<ICoin, UInt64> Coins { get; } = new ConcurrentDictionary<ICoin, UInt64>();

        [JsonProperty]
        public Guid ID { get; }

        [JsonProperty]
        public WalletStatistics Statistics { get; } = new WalletStatistics();

        public Wallet( Guid id ) => this.ID = id;

        private UInt64 Deposit( [CanBeNull] IBankNote bankNote, UInt64 quantity ) {
            if ( null == bankNote ) {
                return 0;
            }

            try {
                lock ( this.BankNotes ) {
                    if ( this.BankNotes.ContainsKey( bankNote ) ) {
                        this.BankNotes[ bankNote ] += quantity;
                    }
                    else {
                        if ( this.BankNotes.TryAdd( bankNote, quantity ) ) { }
                    }
                }

                return this.BankNotes[ bankNote ]; //outside of lock... is this okay?
            }
            finally {
                this.Statistics.AllTimeDeposited += bankNote.FaceValue * quantity;
            }
        }

        public void Deposit( Decimal amount, out Decimal leftOver ) {
            var money = amount.ToOptimal( out leftOver );

            foreach ( var kvp in money ) {
                this.Deposit( kvp.Key, kvp.Value );
            }
        }

        /// <summary>
        ///     Create an empty wallet with a new random id.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static Wallet Create() => new Wallet( id: Guid.NewGuid() );

        /// <summary>
        ///     Create an empty wallet with the given <paramref name="id" />.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NotNull]
        public static Wallet Create( Guid id ) => new Wallet( id: id );

        //private ActionBlock<TransactionMessage> Messages {get;}
        public void ClearBankNotes() {
            lock ( this.BankNotes ) {
                this.BankNotes.Clear();
            }
        }

        public void ClearCoins() {
            lock ( this.Coins ) {
                this.Coins.Clear();
            }
        }

        public void ClearEverything() {
            this.ClearBankNotes();
            this.ClearCoins();
        }

        public Boolean Contains( [NotNull] IBankNote bankNote ) {
            if ( bankNote == null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }

            return this.BankNotes.ContainsKey( bankNote );
        }

        public Boolean Contains( [NotNull] ICoin coin ) {
            if ( coin == null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }

            return this.Coins.ContainsKey( coin );
        }

        // await this.Messages.Completion;
        public UInt64 Count( [NotNull] IBankNote bankNote ) {
            if ( bankNote == null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }

            return this.BankNotes.TryGetValue( bankNote, out var result ) ? result : UInt64.MinValue;
        }

        ///// <summary>
        ///// Makes sure all <see cref="Messages" /> have been processed.
        ///// </summary>
        //public async Task CatchUp() {
        public UInt64 Count( [NotNull] ICoin coin ) {
            if ( coin == null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }

            return this.Coins.TryGetValue( coin, out var result ) ? result : UInt64.MinValue;
        }

        /// <summary>
        ///     Deposit one or more <paramref name="denomination" /> into this <see cref="Wallet" />.
        /// </summary>
        /// <param name="denomination"></param>
        /// <param name="quantity">    </param>
        /// <param name="id">          </param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean Deposit( [NotNull] IDenomination denomination, UInt64 quantity, Guid? id = null ) {
            if ( denomination == null ) {
                throw new ArgumentNullException( nameof( denomination ) );
            }

            if ( !quantity.Any() ) {
                return false;
            }

            var message = new TransactionMessage {
                Date = DateTime.Now, Denomination = denomination, ID = id ?? Guid.NewGuid(), Quantity = quantity, TransactionType = TransactionType.Deposit
            };

            this.Deposit( message );

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="coin">    </param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public UInt64 Deposit( [CanBeNull] ICoin coin, UInt64 quantity ) {
            if ( null == coin ) {
                return 0;
            }

            try {
                lock ( this.Coins ) {
                    if ( this.Coins.ContainsKey( coin ) ) {
                        this.Coins[ coin ] += quantity;
                    }
                    else {
                        if ( !this.Coins.TryAdd( coin, quantity ) ) {
                            throw new WalletException( $"Unable to add {quantity} of {coin}" );
                        }
                    }
                }

                return this.Coins[ coin ];
            }
            finally {
                this.Statistics.AllTimeDeposited += coin.FaceValue * quantity;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="WalletException"></exception>
        public void Deposit( TransactionMessage message ) {
            switch ( message.Denomination ) {
                case IBankNote asBankNote:
                    this.Deposit( asBankNote, message.Quantity );

                    return;

                case ICoin asCoin:
                    this.Deposit( asCoin, message.Quantity );

                    return;
            }

            throw new WalletException( $"Unknown denomination {message.Denomination}" );
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Statistics.Dispose();

        /// <summary>
        ///     Return each <see cref="ICoin" /> in this <see cref="Wallet" />.
        /// </summary>
        [NotNull]
        public IEnumerable<ICoin> GetCoins() => this.Coins.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

        [NotNull]
        public IEnumerable<KeyValuePair<ICoin, UInt64>> GetCoinsGrouped() => this.Coins;

        /// <summary>
        ///     Return the count of each type of <see cref="BankNotes" /> and <see cref="Coins" />.
        /// </summary>
        [NotNull]
        public IEnumerable<KeyValuePair<IDenomination, UInt64>> GetGroups() =>
            this.BankNotes.Cast<KeyValuePair<IDenomination, UInt64>>().Concat( this.Coins.Cast<KeyValuePair<IDenomination, UInt64>>() );

        /// <summary>
        ///     Return each <see cref="IBankNote" /> in this <see cref="Wallet" />.
        /// </summary>
        [NotNull]
        public IEnumerable<IBankNote> GetNotes() => this.BankNotes.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

        /// <summary>
        ///     Return an expanded list of the <see cref="BankNotes" /> and <see cref="Coins" /> in this <see cref="Wallet" />.
        /// </summary>
        [NotNull]
        public IEnumerable<IDenomination> GetNotesAndCoins() => this.GetCoins().Concat<IDenomination>( this.GetNotes() );

        [NotNull]
        public IEnumerable<KeyValuePair<IBankNote, UInt64>> GetNotesGrouped() => this.BankNotes;

        public override String ToString() {
            var total = this.Total().ToString( "C2" );

            var notes = this.BankNotes.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

            var coins = this.Coins.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

            return $"{total} in {notes:N0} notes and {coins:N0} coins.";
        }

        /// <summary>
        ///     Return the total amount of money contained in this <see cref="Wallet" />.
        /// </summary>
        public Decimal Total() => this.TotalCoins() + this.TotalBankNotes();

        /// <summary>
        ///     Return the total amount of banknotes contained in this <see cref="Wallet" />.
        /// </summary>
        public Decimal TotalBankNotes() {
            var total = this.BankNotes.Aggregate( Decimal.Zero, ( current, pair ) => current + (pair.Key.FaceValue * pair.Value) );

            return total;
        }

        /// <summary>
        ///     Return the total amount of coins contained in this <see cref="Wallet" />.
        /// </summary>
        public Decimal TotalCoins() {
            var total = this.Coins.Aggregate( Decimal.Zero, ( current, pair ) => current + (pair.Key.FaceValue * pair.Value) );

            return total;
        }

        /// <summary>
        ///     Attempt to <see cref="TryWithdraw(IBankNote,UInt64)" /> one or more <see cref="IBankNote" /> from this
        ///     <see cref="Wallet" />.
        /// </summary>
        /// <param name="bankNote"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean TryWithdraw( [CanBeNull] IBankNote bankNote, UInt64 quantity ) {
            if ( bankNote == null ) {
                return false;
            }

            if ( quantity <= 0 ) {
                return false;
            }

            lock ( this.BankNotes ) {
                if ( !this.BankNotes.ContainsKey( bankNote ) || this.BankNotes[ bankNote ] < quantity ) {
                    return false; //no bills to withdraw!
                }

                this.BankNotes[ bankNote ] -= quantity;

                return true;
            }
        }

        /// <summary>
        ///     Attempt to <see cref="TryWithdraw(ICoin,UInt64)" /> one or more <see cref="ICoin" /> from this
        ///     <see cref="Wallet" /> .
        /// </summary>
        /// <param name="coin">    </param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean TryWithdraw( [NotNull] ICoin coin, UInt64 quantity ) {
            if ( coin == null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }

            if ( quantity <= 0 ) {
                return false;
            }

            lock ( this.Coins ) {
                if ( !this.Coins.ContainsKey( coin ) || this.Coins[ coin ] < quantity ) {
                    return false; //no coins to withdraw!
                }

                this.Coins[ coin ] -= quantity;

                return true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="denomination"></param>
        /// <param name="quantity">    </param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public Boolean TryWithdraw( [NotNull] IDenomination denomination, UInt64 quantity ) {
            switch ( denomination ) {
                case IBankNote asBankNote: return this.TryWithdraw( asBankNote, quantity );
                case ICoin asCoin: return this.TryWithdraw( asCoin, quantity );
            }

            throw new WalletException( $"Unknown denomination {denomination}" );
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public Boolean TryWithdraw( TransactionMessage message ) {
            switch ( message.Denomination ) {
                case IBankNote asBankNote: return this.TryWithdraw( asBankNote, message.Quantity );
                case ICoin asCoin: return this.TryWithdraw( asCoin, message.Quantity );
            }

            throw new WalletException( $"Unknown denomination {message.Denomination}" );
        }
    }
}