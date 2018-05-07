// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Wallet.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

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
	using NUnit.Framework;

	/// <summary>
	///     My go at a thread-safe Wallet class for US dollars and coins. It's more pseudocode for
	///     learning than for production.. Use at your own risk. Any tips or ideas? Any dos or don'ts?
	///     Email me!
	/// </summary>
	/// <seealso cref="SimpleWallet" />
	/// <seealso cref="Measurement.Currency.BTC.SimpleBitcoinWallet" />
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public class Wallet : ABetterClassDispose, IEnumerable<KeyValuePair<IDenomination, UInt64>> {

		public Wallet( Guid id ) => this.ID = id;

		[JsonProperty]
		public Guid ID {
			get;
		}

		[JsonProperty]
		public WalletStatistics Statistics { get; } = new WalletStatistics();

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

		/// <summary>
		///     Create an empty wallet with a new random id.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public static Wallet Create() => new Wallet( id: Guid.NewGuid() );

		//private ActionBlock<TransactionMessage> Messages {get;}

		/// <summary>
		///     Create an empty wallet with the given <paramref name="id" />.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
		public static Wallet Create( Guid id ) => new Wallet( id: id );

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

		public Boolean Contains( IBankNote bankNote ) {
			if ( bankNote is null ) {
				throw new ArgumentNullException( nameof( bankNote ) );
			}

			return this.BankNotes.ContainsKey( bankNote );
		}

		public Boolean Contains( ICoin coin ) {
			if ( coin is null ) {
				throw new ArgumentNullException( nameof( coin ) );
			}

			return this.Coins.ContainsKey( coin );
		}

		//    await this.Messages.Completion;
		public UInt64 Count( IBankNote bankNote ) {
			if ( bankNote is null ) {
				throw new ArgumentNullException( nameof( bankNote ) );
			}

			return this.BankNotes.TryGetValue( bankNote, out var result ) ? result : UInt64.MinValue;
		}

		///// <summary>
		///// Makes sure all <see cref="Messages" /> have been processed.
		///// </summary>
		//public async Task CatchUp() {
		public UInt64 Count( ICoin coin ) {
			if ( coin is null ) {
				throw new ArgumentNullException( nameof( coin ) );
			}

			return this.Coins.TryGetValue( coin, out var result ) ? result : UInt64.MinValue;
		}

		/// <summary>
		///     Deposit one or more <paramref name="denomination" /> into this <see cref="Wallet" />.
		/// </summary>
		/// <param name="denomination"></param>
		/// <param name="quantity"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <remarks>Locks the wallet.</remarks>
		public Boolean Deposit( IDenomination denomination, UInt64 quantity, Guid? id = null ) {
			if ( denomination is null ) {
				throw new ArgumentNullException( nameof( denomination ) );
			}
			if ( !quantity.Any() ) {
				return false;
			}

			var message = new TransactionMessage { Date = DateTime.Now, Denomination = denomination, ID = id ?? Guid.NewGuid(), Quantity = quantity, TransactionType = TransactionType.Deposit };
			this.Deposit( message );
			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="coin"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		/// <exception cref="WalletException"></exception>
		public UInt64 Deposit( ICoin coin, UInt64 quantity ) {
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
			if ( message.Denomination is IBankNote asBankNote ) {
				this.Deposit( asBankNote, message.Quantity );
				return;
			}

			if ( message.Denomination is ICoin asCoin ) {
				this.Deposit( asCoin, message.Quantity );
				return;
			}

			throw new WalletException( $"Unknown denomination {message.Denomination}" );
		}

		/// <summary>
		///     Return each <see cref="ICoin" /> in this <see cref="Wallet" />.
		/// </summary>
		public IEnumerable<ICoin> GetCoins() => this.Coins.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

		public IEnumerable<KeyValuePair<ICoin, UInt64>> GetCoinsGrouped() {
			Assert.NotNull( this.Coins );

			return this.Coins;
		}

		public IEnumerator<KeyValuePair<IDenomination, UInt64>> GetEnumerator() => this.GetGroups().GetEnumerator();

		/// <summary>
		///     Return the count of each type of <see cref="BankNotes" /> and <see cref="Coins" />.
		/// </summary>
		public IEnumerable<KeyValuePair<IDenomination, UInt64>> GetGroups() => this.BankNotes.Cast<KeyValuePair<IDenomination, UInt64>>().Concat( this.Coins.Cast<KeyValuePair<IDenomination, UInt64>>() );

		/// <summary>
		///     Return each <see cref="IBankNote" /> in this <see cref="Wallet" />.
		/// </summary>
		public IEnumerable<IBankNote> GetNotes() => this.BankNotes.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

		/// <summary>
		///     Return an expanded list of the <see cref="BankNotes" /> and <see cref="Coins" /> in this <see cref="Wallet" />.
		/// </summary>
		public IEnumerable<IDenomination> GetNotesAndCoins() => this.GetCoins().Concat<IDenomination>( this.GetNotes() );

		public IEnumerable<KeyValuePair<IBankNote, UInt64>> GetNotesGrouped() => this.BankNotes;

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public override String ToString() {
			var total = this.Total().ToString( "C2" );
			Assert.NotNull( this.BankNotes );
			var notes = this.BankNotes.Aggregate( 0UL, ( current, pair ) => current + pair.Value );

			Assert.NotNull( this.Coins );
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
			var total = this.BankNotes.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );
			return total;
		}

		/// <summary>
		///     Return the total amount of coins contained in this <see cref="Wallet" />.
		/// </summary>
		public Decimal TotalCoins() {
			var total = this.Coins.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );
			return total;
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
			if ( bankNote is null ) {
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
		///     Attempt to <see cref="TryWithdraw(ICoin,UInt64)" /> one or more <see cref="ICoin" />
		///     from this <see cref="Wallet" /> .
		/// </summary>
		/// <param name="coin"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		/// <remarks>Locks the wallet.</remarks>
		public Boolean TryWithdraw( ICoin coin, UInt64 quantity ) {
			if ( coin is null ) {
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
		/// <param name="quantity"></param>
		/// <returns></returns>
		/// <exception cref="WalletException"></exception>
		public Boolean TryWithdraw( IDenomination denomination, UInt64 quantity ) {
			if ( denomination is IBankNote asBankNote ) {
				return this.TryWithdraw( asBankNote, quantity );
			}

			if ( denomination is ICoin asCoin ) {
				return this.TryWithdraw( asCoin, quantity );
			}

			throw new WalletException( $"Unknown denomination {denomination}" );
		}

		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		/// <exception cref="WalletException"></exception>
		public Boolean TryWithdraw( TransactionMessage message ) {
			if ( message.Denomination is IBankNote asBankNote ) {
				return this.TryWithdraw( asBankNote, message.Quantity );
			}

			if ( message.Denomination is ICoin asCoin ) {
				return this.TryWithdraw( asCoin, message.Quantity );
			}

			throw new WalletException( $"Unknown denomination {message.Denomination}" );
		}

		private UInt64 Deposit( IBankNote bankNote, UInt64 quantity ) {
			if ( null == bankNote ) {
				return 0;
			}
			try {
				lock ( this.BankNotes ) {
					if ( this.BankNotes.ContainsKey( bankNote ) ) {
						this.BankNotes[ bankNote ] += quantity;
					}
					else {
						if ( this.BankNotes.TryAdd( bankNote, quantity ) ) {
						}
					}
				}
				return this.BankNotes[ bankNote ]; //outside of lock... is this okay?
			}
			finally {
				this.Statistics.AllTimeDeposited += bankNote.FaceValue * quantity;
			}
		}

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this.Statistics.Dispose();

	}
}