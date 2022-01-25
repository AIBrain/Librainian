// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "SimpleBitcoinWallet.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

#nullable enable

namespace Librainian.Financial.Currency.Bitcoin;

using System;
using System.Diagnostics;
using System.Threading;
using Exceptions;
using Measurement.Time;
using Newtonsoft.Json;
using Utilities.Disposables;

/// <summary>A very simple, thread-safe, Decimal-based bitcoin wallet.</summary>
/// <remarks>TODO add in support for automatic persisting TODO add in support for exploring the blockchain. lol</remarks>
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Serializable]
[JsonObject]
public record SimpleBitcoinWallet : ABetterRecordDispose, IComparable<SimpleBitcoinWallet> {

	/// <summary>1</summary>
	public const Decimal BTC = 1M;

	/// <summary>0.001</summary>
	public const Decimal mBTC = 0.001M;

	/// <summary>1000 mBTC are in 1 BTC</summary>
	public const UInt16 mBTCInOneBTC = ( UInt16 )( BTC / mBTC );

	/// <summary>0.00000001</summary>
	public const Decimal Satoshi = 0.00000001M;

	/// <summary>100,000,000 Satoshi are in 1 BTC</summary>
	public const UInt32 SatoshiInOneBtc = ( UInt32 )( BTC / Satoshi );

	/// <summary>0.0000001</summary>
	public const Decimal TenSatoshi = 0.0000001M;

	/// <summary>0.000001</summary>
	public const Decimal ΜBtc = 0.000001M;

	/// <summary>1,000,000 μBTC are in 1 BTC</summary>
	public const UInt64 ΜBtcInOneBtc = ( UInt64 )( BTC / ΜBtc );

	[NonSerialized]
	private readonly ReaderWriterLockSlim _access = new( LockRecursionPolicy.SupportsRecursion );

	[JsonProperty]
	private Decimal _balance;

	/// <summary>Initialize the wallet with the specified amount of satoshi.</summary>
	/// <param name="satoshi"></param>
	public SimpleBitcoinWallet( Int64 satoshi ) : this( satoshi.ToBTC() ) { }

	public SimpleBitcoinWallet( ISimpleWallet wallet, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) : this( wallet.Balance, readTimeout, writeTimeout ) { }

	/// <summary>Initialize the wallet with the specified <paramref name="btcbalance" /> .</summary>
	/// <param name="btcbalance"></param>
	/// <param name="readTimeout"></param>
	/// <param name="writeTimeout"></param>
	public SimpleBitcoinWallet( Decimal btcbalance, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) : base( nameof( SimpleBitcoinWallet ) ) {
		this._balance = btcbalance;
		this.ReadTimeout = readTimeout ?? Minutes.One;
		this.WriteTimeout = writeTimeout ?? Minutes.One;
	}

	public SimpleBitcoinWallet( TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) : this( 0.0m, readTimeout, writeTimeout ) { }

	public Decimal? Balance {
		get {
			try {
				return this._access.TryEnterReadLock( this.ReadTimeout ) ? this._balance : throw new NoReadAccessException( nameof( this.Balance ) );
			}
			finally {
				if ( this._access.IsReadLockHeld ) {
					this._access.ExitReadLock();
				}
			}
		}
	}

	public Action<Decimal>? OnAfterDeposit { get; set; }

	public Action<Decimal>? OnAfterWithdraw { get; set; }

	public Action<Decimal>? OnAnyUpdate { get; set; }

	public Action<Decimal>? OnBeforeDeposit { get; set; }

	public Action<Decimal>? OnBeforeWithdraw { get; set; }

	/// <summary>
	///     <para>Defaults to <see cref="Minutes.One" />.</para>
	/// </summary>
	public TimeSpan ReadTimeout { get; set; }

	/// <summary>
	///     <para>Defaults to <see cref="Minutes.One" />.</para>
	/// </summary>
	public TimeSpan WriteTimeout { get; set; }

	public Int32 CompareTo( SimpleBitcoinWallet? otherWallet ) {
		if ( otherWallet is null ) {
			throw new ArgumentEmptyException( nameof( otherWallet ) );
		}

		return this.Balance.GetValueOrDefault( Decimal.Zero ).CompareTo( otherWallet.Balance );
	}

	public virtual Boolean Equals( SimpleBitcoinWallet? other ) => Equals( this, other );

	/// <summary>
	///     <para>Static comparison.</para>
	///     <para>Returns true if the wallets are the same instance.</para>
	///     <para>Returns true if the balances match! (Even if different wallets)</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( SimpleBitcoinWallet? left, SimpleBitcoinWallet? right ) {
		if ( left is null || right is null ) {
			return false;
		}

		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		return left.Balance == right.Balance;
	}

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		using ( this._access ) { }
	}

	///// <summary>Indicates whether the current wallet is the same as the <paramref name="otherWallet" /> wallet.</summary>
	///// <param name="otherWallet">Annother to compare with this wallet.</param>
	//public Boolean Equals( SimpleBitcoinWallet? otherWallet ) => Equals( this, otherWallet );

	public override Int32 GetHashCode() => this.Balance.GetHashCode();

	public override String ToString() => $"฿ {this.Balance:F8}";

	/// <summary>Add any (+-)amount directly to the balance.</summary>
	/// <param name="amount"></param>
	public Boolean TryAdd( Decimal? amount ) {
		if ( amount is null ) {
			return false;
		}

		try {
			if ( this._access.TryEnterWriteLock( this.WriteTimeout ) ) {
				try {
					this._balance += amount.Value;
				}
				finally {
					this._access.ExitWriteLock();
				}

				return true;
			}
			else {
				return false;
			}
		}
		finally {
			this.OnAnyUpdate?.Invoke( amount.Value );
		}
	}

	public Boolean TryAdd( SimpleBitcoinWallet? wallet ) {
		if ( wallet is null ) {
			throw new ArgumentEmptyException( nameof( wallet ) );
		}

		return this.TryAdd( wallet.Balance );
	}

	/// <summary>Attempt to deposit amount (larger than zero) to the <see cref="Balance" /> .</summary>
	/// <param name="amount"></param>
	public Boolean TryDeposit( Decimal amount ) {
		if ( amount <= Decimal.Zero ) {
			return false;
		}

		this.OnBeforeDeposit?.Invoke( amount );

		if ( !this.TryAdd( amount ) ) {
			return false;
		}

		this.OnAfterDeposit?.Invoke( amount );

		return true;
	}

	/// <summary>
	///     Attempts to transfer the <paramref name="amount" /> from this wallet into the <paramref name="destinationWallet" />
	///     ;
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="destinationWallet"></param>
	/// <returns></returns>
	public Boolean TryTransfer( Decimal amount, ref SimpleBitcoinWallet? destinationWallet ) {
		var transferred = false;
		if ( amount <= Decimal.Zero || destinationWallet is null || Equals( this, destinationWallet ) ) {
			return false;
		}

		try {
			if ( !this._access.TryEnterWriteLock( this.WriteTimeout ) ) {
				return false;
			}

			if ( this._balance < amount ) {
				return false;
			}

			this._balance -= amount;
			transferred = destinationWallet.TryDeposit( amount ); //shouldn't this be outside of lock?

			return true;
		}
		finally {
			if ( this._access.IsWriteLockHeld ) {
				this._access.ExitWriteLock();
			}

			if ( transferred ) {
				this.OnAfterWithdraw?.Invoke( amount );
				this.OnAnyUpdate?.Invoke( amount );
			}
		}
	}

	/// <summary>
	///     <para>Directly sets the <see cref="Balance" /> of this wallet.</para>
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="sanitize"></param>
	public Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true ) {
		try {
			if ( !this._access.TryEnterWriteLock( this.WriteTimeout ) ) {
				return false;
			}

			this._balance = sanitize ? amount.Sanitize() : amount;

			return true;
		}
		finally {
			if ( this._access.IsWriteLockHeld ) {
				this._access.ExitWriteLock();
			}

			this.OnAnyUpdate?.Invoke( amount );
		}
	}

	public Boolean TryUpdateBalance( SimpleBitcoinWallet simpleBitcoinWallet ) =>
		simpleBitcoinWallet.Balance != null && this.TryUpdateBalance( simpleBitcoinWallet.Balance.Value );

	/// <summary>
	///     <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
	///     <para>If the amount is not available, then nothing is withdrawn.</para>
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="sanitize"></param>
	public Boolean TryWithdraw( Decimal amount, Boolean sanitize = true ) {
		if ( sanitize ) {
			amount = amount.Sanitize();
		}

		if ( amount <= Decimal.Zero ) {
			return false;
		}

		try {
			if ( !this._access.TryEnterWriteLock( this.WriteTimeout ) ) {
				return false;
			}

			if ( this._balance < amount ) {
				return false;
			}

			this._balance -= amount;

			return true;
		}
		finally {
			if ( this._access.IsWriteLockHeld ) {
				this._access.ExitWriteLock();
			}

			//TODO only if changed
			this.OnAfterWithdraw?.Invoke( amount );
			this.OnAnyUpdate?.Invoke( amount );
		}
	}

	/// <summary>Attempt to withdraw an amount (must be larger than Zero) from the wallet.</summary>
	/// <param name="wallet"></param>
	public Boolean TryWithdraw( SimpleBitcoinWallet wallet ) {
		if ( wallet is null ) {
			throw new ArgumentEmptyException( nameof( wallet ) );
		}

		return wallet.Balance != null && this.TryWithdraw( wallet.Balance.Value );
	}

	public static Boolean operator <( SimpleBitcoinWallet left, SimpleBitcoinWallet right ) => left.CompareTo( right ) < 0;

	public static Boolean operator <=( SimpleBitcoinWallet left, SimpleBitcoinWallet right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >( SimpleBitcoinWallet left, SimpleBitcoinWallet right ) => left.CompareTo( right ) > 0;

	public static Boolean operator >=( SimpleBitcoinWallet left, SimpleBitcoinWallet right ) => left.CompareTo( right ) >= 0;
}