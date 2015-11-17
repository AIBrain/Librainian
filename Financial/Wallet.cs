// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Wallet.cs" was last cleaned by Rick on 2015/10/05 at 6:21 PM

namespace Librainian.Financial {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks.Dataflow;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Currency;
    using NUnit.Framework;
    using Threading;

    /// <summary>
    ///     My go at a thread-safe Wallet class for US dollars and coins. It's more pseudocode for
    ///     learning than for production.. Use at your own risk. Any tips or ideas? Any dos or dont's?
    ///     Email me!
    /// </summary>
    /// <seealso cref="SimpleWallet" />
    /// <seealso cref="Measurement.Currency.BTC.SimpleBitcoinWallet" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{Formatted,nq}" )]
    public class Wallet : IEnumerable<KeyValuePair<IDenomination, UInt64>>, IWallet {

        /// <summary>Count of each <see cref="IBankNote" />.</summary>
        [DataMember]
        [NotNull]
        private readonly ConcurrentDictionary<IBankNote, UInt64> _bankNotes = new ConcurrentDictionary<IBankNote, UInt64>();

        /// <summary>Count of each <see cref="ICoin" />.</summary>
        [DataMember]
        [NotNull]
        private readonly ConcurrentDictionary<ICoin, UInt64> _coins = new ConcurrentDictionary<ICoin, UInt64>();

        [DataMember]
        public WalletStatistics WalletStatistics = new WalletStatistics();

        public Wallet( Guid id ) {
            this.ID = id;

            this.Actor = new ActionBlock<TransactionMessage>( message => {
                switch ( message.TransactionType ) {
                    case TransactionType.Deposit:
                        this.Deposit( message );
                        break;

                    case TransactionType.Withdraw:
                        this.TryWithdraw( message );
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, Blocks.ManyProducers.ConsumeSerial );
        }

        private ActionBlock<TransactionMessage> Actor {
            get;
        }

        public IEnumerator<KeyValuePair<IDenomination, UInt64>> GetEnumerator() => this.Groups.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>Return each <see cref="ICoin" /> in this <see cref="Wallet" />.</summary>
        public IEnumerable<ICoin> Coins {
            [NotNull]
            get {
                return this._coins.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );
            }
        }

        public IEnumerable<KeyValuePair<ICoin, UInt64>> CoinsGrouped {
            [NotNull]
            get {
                Assert.NotNull( this._coins );
                return this._coins;
            }
        }

        public String Formatted {
            get {
                var total = this.Total.ToString( "C4" );
                Assert.NotNull( this._bankNotes );
                var notes = this._bankNotes.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

                Assert.NotNull( this._coins );
                var coins = this._coins.Aggregate( 0UL, ( current, pair ) => current + pair.Value );
                return $"{total} in {notes:N0} notes and {coins:N0} coins.";
            }
        }

        /// <summary>Return the count of each type of <see cref="Notes" /> and <see cref="Coins" />.</summary>
        public IEnumerable<KeyValuePair<IDenomination, UInt64>> Groups {
            [NotNull]
            get {
                return this._bankNotes.Cast<KeyValuePair<IDenomination, UInt64>>()
                           .Concat( this._coins.Cast<KeyValuePair<IDenomination, UInt64>>() );
            }
        }

        public Guid ID {
            get;
        }

        /// <summary>Return each <see cref="IBankNote" /> in this <see cref="Wallet" />.</summary>
        public IEnumerable<IBankNote> Notes => this._bankNotes.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

        /// <summary>
        ///     Return an expanded list of the <see cref="Notes" /> and <see cref="Coins" /> in this <see cref="Wallet" />.
        /// </summary>
        public IEnumerable<IDenomination> NotesAndCoins {
            [NotNull]
            get {
                return this.Coins.Concat<IDenomination>( this.Notes );
            }
        }

        public IEnumerable<KeyValuePair<IBankNote, UInt64>> NotesGrouped {
            [NotNull]
            get {
                return this._bankNotes;
            }
        }

        /// <summary>Return the total amount of money contained in this <see cref="Wallet" />.</summary>
        public Decimal Total {
            get {
                var total = this._coins.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );
                total += this._bankNotes.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );
                return total;
            }
        }

        public Boolean Contains( IBankNote bankNote ) {
            if ( bankNote == null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }
            return this._bankNotes.ContainsKey( bankNote );
        }

        public Boolean Contains( ICoin coin ) {
            if ( coin == null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }
            return this._coins.ContainsKey( coin );
        }

        public UInt64 Count( IBankNote bankNote ) {
            if ( bankNote == null ) {
                throw new ArgumentNullException( nameof( bankNote ) );
            }
            UInt64 result;
            return this._bankNotes.TryGetValue( bankNote, out result ) ? result : UInt64.MinValue;
        }

        public UInt64 Count( ICoin coin ) {
            if ( coin == null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }
            UInt64 result;
            return this._coins.TryGetValue( coin, out result ) ? result : UInt64.MinValue;
        }

        /// <summary>Deposit one or more <paramref name="denomination" /> into this <see cref="Wallet" />.</summary>
        /// <param name="denomination"></param>
        /// <param name="quantity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean Deposit( IDenomination denomination, UInt64 quantity, Guid? id = null ) {
            if ( denomination == null ) {
                throw new ArgumentNullException( nameof( denomination ) );
            }
            if ( quantity <= 0 ) {
                return false;
            }

            return this.Actor.Post( new TransactionMessage { Date = DateTime.Now, Denomination = denomination, ID = id ?? Guid.NewGuid(), Quantity = quantity, TransactionType = TransactionType.Deposit } );
        }

        /// <summary>
        ///     Attempt to <see cref="TryWithdraw(IBankNote,UInt64)" /> one or more
        ///     <see cref="IBankNote" /> from this <see cref="Wallet" />.
        /// </summary>
        /// <param name="bankNote"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean TryWithdraw( IBankNote bankNote, UInt64 quantity ) {
            if ( bankNote == null ) {
                return false;
            }
            if ( quantity <= 0 ) {
                return false;
            }
            lock ( this._bankNotes ) {
                if ( !this._bankNotes.ContainsKey( bankNote ) || ( this._bankNotes[ bankNote ] < quantity ) ) {
                    return false; //no bills to withdraw!
                }
                this._bankNotes[ bankNote ] -= quantity;
                return true;
            }
        }

        /// <summary>
        ///     Attempt to <see cref="TryWithdraw(ICoin,UInt64)" /> one or more <see cref="ICoin" />
        ///     from this <see cref="Wallet" /> .
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <remarks>Locks the wallet.</remarks>
        public Boolean TryWithdraw( ICoin coin, UInt64 quantity ) {
            if ( coin == null ) {
                throw new ArgumentNullException( nameof( coin ) );
            }
            if ( quantity <= 0 ) {
                return false;
            }
            lock ( this._coins ) {
                if ( !this._coins.ContainsKey( coin ) || ( this._coins[ coin ] < quantity ) ) {
                    return false; //no coins to withdraw!
                }
                this._coins[ coin ] -= quantity;
                return true;
            }
        }

        /// <summary></summary>
        /// <param name="denomination"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public Boolean TryWithdraw( IDenomination denomination, UInt64 quantity ) {
            var asBankNote = denomination as IBankNote;
            if ( null != asBankNote ) {
                return this.TryWithdraw( asBankNote, quantity );
            }

            var asCoin = denomination as ICoin;
            if ( null != asCoin ) {
                return this.TryWithdraw( asCoin, quantity );
            }

            throw new WalletException( $"Unknown denomination {denomination}" );
        }

        /// <summary>Create an empty wallet with a new random id.</summary>
        /// <returns></returns>
        [NotNull]
        public static Wallet Create() => new Wallet( id: Guid.NewGuid() );

        /// <summary>Create an empty wallet with the given <paramref name="id" />.</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NotNull]
        public static Wallet Create( Guid id ) => new Wallet( id: id );

        /// <summary></summary>
        /// <param name="coin"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public UInt64 Deposit( ICoin coin, UInt64 quantity ) {
            if ( null == coin ) {
                return 0;
            }
            try {
                lock ( this._coins ) {
                    if ( this._coins.ContainsKey( coin ) ) {
                        this._coins[ coin ] += quantity;
                    }
                    else {
                        if ( !this._coins.TryAdd( coin, quantity ) ) {
                            throw new WalletException( $"Unable to add {quantity} of {coin}" );
                        }
                    }
                }
                return this._coins[ coin ];
            }
            finally {
                this.WalletStatistics.AllTimeDeposited += coin.FaceValue * quantity;
            }
        }

        /// <summary></summary>
        /// <param name="message"></param>
        /// <exception cref="WalletException"></exception>
        public void Deposit( TransactionMessage message ) {
            var asBankNote = message.Denomination as IBankNote;
            if ( null != asBankNote ) {
                this.Deposit( asBankNote, message.Quantity );
            }

            var asCoin = message.Denomination as ICoin;
            if ( null != asCoin ) {
                this.TryWithdraw( asCoin, message.Quantity );
            }

            throw new WalletException( $"Unknown denomination {message.Denomination}" );
        }

        /// <summary></summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="WalletException"></exception>
        public Boolean TryWithdraw( TransactionMessage message ) {
            var asBankNote = message.Denomination as IBankNote;
            if ( null != asBankNote ) {
                return this.TryWithdraw( asBankNote, message.Quantity );
            }

            var asCoin = message.Denomination as ICoin;
            if ( null != asCoin ) {
                return this.TryWithdraw( asCoin, message.Quantity );
            }

            throw new WalletException( $"Unknown denomination {message.Denomination}" );
        }

        private UInt64 Deposit( IBankNote bankNote, UInt64 quantity ) {
            if ( null == bankNote ) {
                return 0;
            }
            try {
                lock ( this._bankNotes ) {
                    if ( this._bankNotes.ContainsKey( bankNote ) ) {
                        this._bankNotes[ bankNote ] += quantity;
                    }
                    else {
                        if ( this._bankNotes.TryAdd( bankNote, quantity ) ) {
                        }
                    }
                }
                return this._bankNotes[ bankNote ]; //outside of lock... is this okay?
            }
            finally {
                this.WalletStatistics.AllTimeDeposited += bankNote.FaceValue * quantity;
            }
        }

    }

}
