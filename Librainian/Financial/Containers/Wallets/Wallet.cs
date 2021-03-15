// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

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
    using Maths;
    using Newtonsoft.Json;
    using Utilities;

    /// <summary>
    ///     My go at a thread-safe Wallet class for US dollars and coins. It's more pseudocode for learning than for
    ///     production.. Use at your own risk. Any tips or ideas? Any dos or
    ///     don'ts? Email me!
    /// </summary>
    /// <remarks>A database with proper locking would be better than this, although not as fun to create.</remarks>
    /// <see cref="SimpleWallet" />
    /// <see cref="Currency.BTC.SimpleBitcoinWallet" />
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Wallet : ABetterClassDispose, IEnumerable<(IDenomination, UInt64)> {

        public Wallet( Guid id ) => this.ID = id;

        /// <summary>Count of each <see cref="IBankNote" />.</summary>
        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<IBankNote, UInt64> BankNotes { get; } = new();

        /// <summary>Count of each <see cref="Currency.Coins.ICoin" />.</summary>
        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<ICoin, UInt64> Coins { get; } = new();

        [JsonProperty]
        public Guid ID { get; }

        [JsonProperty]
        public WalletStatistics Statistics { get; } = new();

        public IEnumerator<(IDenomination, UInt64)> GetEnumerator() => this.GetGroups().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private UInt64 Deposit( [NotNull] IBankNote bankNote, UInt64 quantity ) {
            try {
                lock ( this.BankNotes ) {
                    if ( this.BankNotes.ContainsKey( bankNote ) ) {
                        this.BankNotes[bankNote] += quantity;
                    }
                    else {
                        if ( this.BankNotes.TryAdd( bankNote, quantity ) ) { }
                    }
                }

                return this.BankNotes[bankNote]; //outside of lock... is this okay?
            }
            finally {
                this.Statistics.AllTimeDeposited += bankNote.FaceValue * quantity;
            }
        }

        /// <summary>Create an empty wallet with a new random id.</summary>
        /// <returns></returns>
        [NotNull]
        public static Wallet Create() => new( Guid.NewGuid() );

        /// <summary>Create an empty wallet with the given <paramref name="id" />.</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NotNull]
        public static Wallet Create( Guid id ) => new( id );

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
            if ( bankNote is null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }

            return this.BankNotes.ContainsKey( bankNote );
        }

        public Boolean Contains( [NotNull] ICoin coin ) {
            if ( coin is null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }

            return this.Coins.ContainsKey( coin );
        }

        public UInt64 Count( [NotNull] IBankNote bankNote ) {
            if ( bankNote is null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }

            return this.BankNotes.TryGetValue( bankNote, out var result ) ? result : UInt64.MinValue;
        }

        public UInt64 Count( [NotNull] ICoin coin ) {
            if ( coin is null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }

            return this.Coins.TryGetValue( coin, out var result ) ? result : UInt64.MinValue;
        }

        public void Deposit( Decimal amount, out Decimal leftOver ) {
            var money = amount.ToOptimal( out leftOver );

            foreach ( var (key, value) in money ) {
                this.Deposit( key, value );
            }
        }

        /// <summary>Deposit one or more <paramref name="denomination" /> into this <see cref="Wallet" />.</summary>
        /// <param name="denomination"></param>
        /// <param name="quantity">    </param>
        /// <param name="id">          </param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean Deposit( [NotNull] IDenomination denomination, UInt64 quantity, Guid? id = null ) {
            if ( denomination is null ) {
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

        /// <summary></summary>
        /// <param name="coin">    </param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public UInt64 Deposit( [NotNull] ICoin coin, UInt64 quantity ) {
            try {
                lock ( this.Coins ) {
                    if ( this.Coins.ContainsKey( coin ) ) {
                        this.Coins[coin] += quantity;
                    }
                    else {
                        if ( !this.Coins.TryAdd( coin, quantity ) ) {
                            throw new WalletException( $"Unable to add {quantity} of {coin}" );
                        }
                    }
                }

                lock ( this.Coins ) {
                    return this.Coins[coin];
                }
            }
            finally {
                this.Statistics.AllTimeDeposited += coin.FaceValue * quantity;
            }
        }

        /// <summary></summary>
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

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() {
            using ( this.Statistics ) { }
        }

        /// <summary>Return each <see cref="ICoin" /> in this <see cref="Wallet" />.</summary>
        /// <remarks>Don't use this on large amounts.. it will return Count items.</remarks>
        [NotNull]
        public IEnumerable<ICoin> GetCoins() => this.Coins.SelectMany( pair => 1.To( pair.Value ), ( pair, _ ) => pair.Key );

        [NotNull]
        public IEnumerable<KeyValuePair<ICoin, UInt64>> GetCoinsGrouped() => this.Coins;

        /// <summary>Return the count of each type of <see cref="BankNotes" /> and <see cref="Coins" />.</summary>
        [NotNull]
        public IEnumerable<(IDenomination, UInt64)> GetGroups() {
            foreach ( var (key, value) in this.BankNotes ) {
                yield return ( key, value );
            }

            foreach ( var (key, value) in this.Coins ) {
                yield return ( key, value );
            }
        }

        /// <summary>Return each <see cref="IBankNote" /> in this <see cref="Wallet" />.</summary>
        [NotNull]
        public IEnumerable<IBankNote> GetNotes() => this.BankNotes.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

        /// <summary>
        ///     Return an expanded list of the <see cref="BankNotes" /> and <see cref="Coins" /> in this <see cref="Wallet" />
        ///     .
        /// </summary>
        [NotNull]
        public IEnumerable<IDenomination> GetNotesAndCoins() => this.GetCoins().Concat<IDenomination>( this.GetNotes() );

        [NotNull]
        public IEnumerable<KeyValuePair<IBankNote, UInt64>> GetNotesGrouped() => this.BankNotes;

        [NotNull]
        public override String ToString() {
            var total = this.Total().ToString( "C2" );

            var notes = this.BankNotes.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

            var coins = this.Coins.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

            return $"{total} in {notes:N0} notes and {coins:N0} coins.";
        }

        /// <summary>Return the total amount of money contained in this <see cref="Wallet" />.</summary>
        public Decimal Total() => this.TotalCoins() + this.TotalBankNotes();

        /// <summary>Return the total amount of banknotes contained in this <see cref="Wallet" />.</summary>
        public Decimal TotalBankNotes() {
            var total = this.BankNotes.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );

            return total;
        }

        /// <summary>Return the total amount of coins contained in this <see cref="Wallet" />.</summary>
        public Decimal TotalCoins() {
            var total = this.Coins.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );

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
        public Boolean TryWithdraw( [NotNull] IBankNote bankNote, UInt64 quantity ) {
            if ( bankNote == null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }

            if ( quantity <= 0 ) {
                return false;
            }

            lock ( this.BankNotes ) {
                if ( this.BankNotes.ContainsKey( bankNote ) && this.BankNotes[bankNote] >= quantity ) {
                    this.BankNotes[bankNote] -= quantity;

                    return true;
                }

                return false; //no bills to withdraw!
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
            if ( coin is null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }

            if ( quantity <= 0 ) {
                return false;
            }

            lock ( this.Coins ) {
                if ( this.Coins.ContainsKey( coin ) && this.Coins[coin] >= quantity ) {
                    this.Coins[coin] -= quantity;

                    return true;
                }

                return false; //no coins to withdraw!
            }
        }

        /// <summary></summary>
        /// <param name="denomination"></param>
        /// <param name="quantity">    </param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public Boolean TryWithdraw( [NotNull] IDenomination denomination, UInt64 quantity ) {
	        return denomination switch {
		        IBankNote asBankNote => this.TryWithdraw( asBankNote, quantity ),
		        ICoin asCoin => this.TryWithdraw( asCoin, quantity ),
		        var _ => throw new WalletException( $"Unknown denomination {denomination}" )
	        };
        }

        /// <summary></summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public Boolean TryWithdraw( TransactionMessage message ) {
	        return message.Denomination switch {
		        IBankNote asBankNote => this.TryWithdraw( asBankNote, message.Quantity ),
		        ICoin asCoin => this.TryWithdraw( asCoin, message.Quantity ),
		        var _ => throw new WalletException( $"Unknown denomination {message.Denomination}" )
	        };
        }

    }

}