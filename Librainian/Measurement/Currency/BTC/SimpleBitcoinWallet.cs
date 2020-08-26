// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "SimpleBitcoinWallet.cs" last formatted on 2020-08-14 at 8:36 PM.

namespace Librainian.Measurement.Currency.BTC {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Time;
	using Utilities;

	/// <summary>A very simple, thread-safe, Decimal-based bitcoin wallet.</summary>
	/// <remarks>TODO add in support for automatic persisting TODO add in support for exploring the blockchain</remarks>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject]
	public class SimpleBitcoinWallet : ABetterClassDispose, IEquatable<SimpleBitcoinWallet>, IComparable<SimpleBitcoinWallet> {

		/// <summary>1</summary>
		public const Decimal BTC = 1M;

		/// <summary>0. 001</summary>
		public const Decimal mBTC = 0.001M;

		/// <summary>1000 mBTC are in 1 BTC</summary>
		public const UInt16 mBTCInOneBTC = ( UInt16 )( BTC / mBTC );

		/// <summary>0.00000001</summary>
		public const Decimal Satoshi = 0.00000001M;

		/// <summary>100,000,000 Satoshi are in 1 BTC</summary>
		public const UInt64 SatoshiInOneBtc = ( UInt64 )( BTC / Satoshi );

		/// <summary>0.0000001</summary>
		public const Decimal TenSatoshi = 0.0000001M;

		/// <summary>0. 000001</summary>
		public const Decimal ΜBtc = 0.000001M;

		/// <summary>1,000,000 μBTC are in 1 BTC</summary>
		public const UInt64 ΜBtcInOneBtc = ( UInt64 )( BTC / ΜBtc );

		[NonSerialized]
		[NotNull]
		private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

		private readonly Int32 _hashcode;

		[JsonProperty]
		private Decimal _balance;

		/// <summary>Initialize the wallet with the specified amount of satoshi.</summary>
		/// <param name="satoshi"></param>
		public SimpleBitcoinWallet( Int64 satoshi ) : this( satoshi.ToBTC() ) { }

		public SimpleBitcoinWallet( [NotNull] ISimpleWallet wallet ) : this( wallet.Balance ) { }

		/// <summary>Initialize the wallet with the specified <paramref name="btcbalance" /> .</summary>
		/// <param name="btcbalance"></param>
		public SimpleBitcoinWallet( Decimal btcbalance ) {
			this._balance = btcbalance;
			this.Timeout = Minutes.One;
			this._hashcode = Randem.NextInt32();
		}

		public SimpleBitcoinWallet() : this( 0.0m ) { }

		public Decimal Balance {
			get {
				try {
					return this._access.TryEnterReadLock( this.Timeout ) ? this._balance : Decimal.Zero;
				}
				finally {
					if ( this._access.IsReadLockHeld ) {
						this._access.ExitReadLock();
					}
				}
			}
		}

		[CanBeNull]
		public Action<Decimal>? OnAfterDeposit { get; set; }

		[CanBeNull]
		public Action<Decimal>? OnAfterWithdraw { get; set; }

		[CanBeNull]
		public Action<Decimal>? OnAnyUpdate { get; set; }

		[CanBeNull]
		public Action<Decimal>? OnBeforeDeposit { get; set; }

		[CanBeNull]
		public Action<Decimal>? OnBeforeWithdraw { get; set; }

		/// <summary>
		///     <para>Defaults to <see cref="Seconds.Thirty" /> in the ctor.</para>
		/// </summary>
		public TimeSpan Timeout { get; set; }

		public Int32 CompareTo( [NotNull] SimpleBitcoinWallet otherWallet ) {
			if ( otherWallet is null ) {
				throw new ArgumentNullException( nameof( otherWallet ) );
			}

			return this.Balance.CompareTo( otherWallet.Balance );
		}

		/// <summary>Indicates whether the current wallet is the same as the <paramref name="otherWallet" /> wallet.</summary>
		/// <param name="otherWallet">Annother to compare with this wallet.</param>
		public Boolean Equals( SimpleBitcoinWallet otherWallet ) => Equals( this, otherWallet );

		/// <summary>
		///     <para>Static comparison.</para>
		///     <para>Returns true if the wallets are the same instance.</para>
		///     <para>Returns true if the balances match! (Even if different wallets)</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] SimpleBitcoinWallet left, [CanBeNull] SimpleBitcoinWallet right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return default;
			}

			return left.Balance == right.Balance;
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() {
			using ( this._access ) { }
		}

		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
		///     <see langword="false" />.
		/// </returns>
		public override Boolean Equals( [CanBeNull] Object? obj ) => Equals( this, obj as SimpleBitcoinWallet );

		public override Int32 GetHashCode() => this._hashcode;

		[NotNull]
		public override String ToString() => $"฿ {this.Balance:F8}";

		/// <summary>Add any (+-)amount directly to the balance.</summary>
		/// <param name="amount">  </param>
		/// <returns></returns>
		public Boolean TryAdd( Decimal amount ) {
			try {
				if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
					try {
						this._balance += amount;
					}
					finally {
						this._access.ExitWriteLock();
					}

					return true;
				}
				else {
					return default;
				}
			}
			finally {
				this.OnAnyUpdate?.Invoke( amount );
			}
		}

		public Boolean TryAdd( [NotNull] SimpleBitcoinWallet wallet ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			return this.TryAdd( wallet.Balance );
		}

		/// <summary>Attempt to deposit amount (larger than zero) to the <see cref="Balance" /> .</summary>
		/// <param name="amount">  </param>
		/// <returns></returns>
		public Boolean TryDeposit( Decimal amount ) {
			if ( amount <= Decimal.Zero ) {
				return default;
			}

			this.OnBeforeDeposit?.Invoke( amount );

			if ( !this.TryAdd( amount ) ) {
				return default;
			}

			this.OnAfterDeposit?.Invoke( amount );

			return true;
		}

		public Boolean TryTransfer( Decimal amount, [CanBeNull] ref SimpleBitcoinWallet intoWallet ) {
			if ( amount <= Decimal.Zero ) {
				return default;
			}

			try {
				if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
					return default;
				}

				if ( this._balance < amount ) {
					return default;
				}

				this._balance -= amount;

				intoWallet?.TryDeposit( amount ); //shouldn't this be outside of lock?

				return true;
			}
			finally {
				if ( this._access.IsWriteLockHeld ) {
					this._access.ExitWriteLock();
				}

				this.OnAfterWithdraw?.Invoke( amount );
				this.OnAnyUpdate?.Invoke( amount );
			}
		}

		/// <summary>
		///     <para>Directly sets the <see cref="Balance" /> of this wallet.</para>
		/// </summary>
		/// <param name="amount">  </param>
		/// <param name="sanitize"></param>
		/// <returns></returns>
		public Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true ) {
			try {
				if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
					return default;
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

		public void TryUpdateBalance( [NotNull] SimpleBitcoinWallet simpleBitcoinWallet ) => this.TryUpdateBalance( simpleBitcoinWallet.Balance );

		/// <summary>
		///     <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
		///     <para>If the amount is not available, then nothing is withdrawn.</para>
		/// </summary>
		/// <param name="amount">  </param>
		/// <param name="sanitize"></param>
		/// <returns></returns>
		public Boolean TryWithdraw( Decimal amount, Boolean sanitize = true ) {
			if ( sanitize ) {
				amount = amount.Sanitize();
			}

			if ( amount <= Decimal.Zero ) {
				return default;
			}

			try {
				if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
					return default;
				}

				if ( this._balance < amount ) {
					return default;
				}

				this._balance -= amount;

				return true;
			}
			finally {
				if ( this._access.IsWriteLockHeld ) {
					this._access.ExitWriteLock();
				}

				this.OnAfterWithdraw?.Invoke( amount );
				this.OnAnyUpdate?.Invoke( amount );
			}
		}

		/// <summary>Attempt to withdraw an amount (must be larger than Zero) from the wallet.</summary>
		/// <param name="wallet"></param>
		/// <returns></returns>
		public Boolean TryWithdraw( [NotNull] SimpleBitcoinWallet wallet ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			return this.TryWithdraw( wallet.Balance );
		}

	}

}