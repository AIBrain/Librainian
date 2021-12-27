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

namespace Librainian.Financial.Currency.BTC;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Collections.Extensions;
using Containers.Wallets;
using Exceptions;
using Maths;
using Newtonsoft.Json;
using Threading;
using Utilities.Disposables;

/// <summary>
///     My first go at a thread-safe CoinWallet class for bitcoin coins. It's more pseudocode for learning than for
///     production.. Use at your own risk. Any tips or ideas? Any dos
///     or dont's? Email me!
/// </summary>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
public class CoinWallet : ABetterClassDispose, IEnumerable<KeyValuePair<ICoin, UInt64>>, ICoinWallet {

	/// <summary>Count of each <see cref="ICoin" />.</summary>
	private readonly ConcurrentDictionary<ICoin, UInt64> _coins = new();

	private ActionBlock<BitcoinTransactionMessage>? Actor { get; set; }

	/// <summary>Return each <see cref="ICoin" /> in this <see cref="CoinWallet" />.</summary>
	public IEnumerable<ICoin> Coins => this._coins.SelectMany( pair => 1.To( pair.Value ), ( pair, valuePair ) => pair.Key );

	public IEnumerable<KeyValuePair<ICoin, UInt64>> CoinsGrouped => this._coins;

	public Guid ID { get; }

	public Action<KeyValuePair<ICoin, UInt64>>? OnDeposit { get; set; }

	public Action<KeyValuePair<ICoin, UInt64>>? OnWithdraw { get; set; }

	[JsonProperty]
	public WalletStatistics? Statistics { get; } = new();

	/// <summary>Return the total amount of money contained in this <see cref="CoinWallet" />.</summary>
	public Decimal Total => this._coins.Aggregate( Decimal.Zero, ( current, pair ) => current + pair.Key.FaceValue * pair.Value );

	private CoinWallet( Guid id ) : base( nameof( CoinWallet ) ) {
		this.ID = id;

		this.Actor = new ActionBlock<BitcoinTransactionMessage>( message => {
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
		}, Blocks.ManyProducers.ConsumeSerial( default( CancellationToken? ) ) );
	}

	/// <summary>
	///     Create an empty wallet with the given <paramref name="id" />. If the given <paramref name="id" /> is null or
	///     <see cref="Guid.Empty" />, a new random
	///     <paramref name="id" /> is generated.
	/// </summary>
	/// <param name="id"></param>
	public static CoinWallet Create( Guid? id = null ) {
		if ( !id.HasValue || id.Value == Guid.Empty ) {
			id = Guid.NewGuid();
		}

		return new CoinWallet( id.Value );
	}

	public Boolean Contains( ICoin coin ) {
		if ( coin is null ) {
			throw new ArgumentEmptyException( nameof( coin ) );
		}

		return this._coins.ContainsKey( coin );
	}

	public UInt64 Count( ICoin coin ) {
		if ( coin is null ) {
			throw new ArgumentEmptyException( nameof( coin ) );
		}

		return this._coins.TryGetValue( coin, out var result ) ? result : UInt64.MinValue;
	}

	public UInt64 Deposit( ICoin coin, UInt64 quantity, Boolean updateStatistics = true ) {
		try {
			lock ( this._coins ) {
				UInt64 newQuantity = 0;

				if ( !this._coins.ContainsKey( coin ) ) {
					if ( this._coins.TryAdd( coin, quantity ) ) {
						newQuantity = quantity;
					}
				}
				else {
					newQuantity = this._coins[coin] += quantity;
				}

				return newQuantity;
			}
		}
		finally {
			if ( updateStatistics ) {
				this.Statistics.AllTimeDeposited += coin.FaceValue * quantity;
			}

			this.OnDeposit?.Invoke( new KeyValuePair<ICoin, UInt64>( coin, quantity ) );
		}
	}

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		using ( this.Statistics ) { }
	}

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
	/// <remarks>Locks the wallet.</remarks>
	public Boolean TryWithdraw( ICoin coin, UInt64 quantity ) {
		if ( coin is null ) {
			throw new ArgumentEmptyException( nameof( coin ) );
		}

		if ( quantity <= 0 ) {
			return false;
		}

		lock ( this._coins ) {
			if ( !this._coins.ContainsKey( coin ) || this._coins[coin] < quantity ) {
				return false; //no coins to withdraw!
			}

			this._coins[coin] -= quantity;
		}

		this.OnWithdraw?.Invoke( new KeyValuePair<ICoin, UInt64>( coin, quantity ) );

		return true;
	}

	public ICoin? TryWithdrawAnyCoin() {
		var possibleCoins = this._coins.Where( pair => pair.Value > 0 ).Select( pair => pair.Key ).ToList();

		if ( !possibleCoins.Any() ) {
			return default( ICoin );
		}

		possibleCoins.Shuffle();
		var key = possibleCoins.First();

		return this.TryWithdraw( key, 1 ) ? key : default( ICoin );
	}

	public ICoin? TryWithdrawSmallestCoin() {
		var coin = this._coins.Where( pair => pair.Value > 0 ).Select( pair => pair.Key ).OrderBy( coin1 => coin1.FaceValue ).FirstOrDefault();

		return coin != null && this.TryWithdraw( coin, 1 ) ? coin : default( ICoin? );
	}

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}