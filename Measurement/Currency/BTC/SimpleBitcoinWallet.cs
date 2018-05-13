// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SimpleBitcoinWallet.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Controls;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Newtonsoft.Json;
    using Time;

    /// <summary>
    /// A very simple, thread-safe, Decimal-based bitcoin wallet.
    /// </summary>
    /// <remarks>TODO add in support for automatic persisting TODO add in support for exploring the blockchain</remarks>
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [Serializable]
    [JsonObject]
    public class SimpleBitcoinWallet : ABetterClassDispose, IEquatable<SimpleBitcoinWallet>, IComparable<SimpleBitcoinWallet> {

        [NonSerialized]
        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );

        private readonly Int32 _hashcode;

        [JsonProperty]
        private Decimal _balance;

        /// <summary>
        /// 1
        /// </summary>
        public const Decimal BTC = 1M;

        /// <summary>
        /// 0. 001
        /// </summary>
        public const Decimal mBTC = 0.001M;

        /// <summary>
        /// 1000 mBTC are in 1 BTC
        /// </summary>
        public const UInt64 mBTCInOneBTC = ( UInt64 )( BTC / mBTC );

        /// <summary>
        /// 0.00000001
        /// </summary>
        public const Decimal Satoshi = 0.00000001M;

        /// <summary>
        /// 100,000,000 Satoshi are in 1 BTC
        /// </summary>
        public const UInt64 SatoshiInOneBtc = ( UInt64 )( BTC / Satoshi );

        /// <summary>
        /// 0.0000001
        /// </summary>
        public const Decimal TenSatoshi = 0.0000001M;

        /// <summary>
        /// 0. 000001
        /// </summary>
        public const Decimal ΜBtc = 0.000001M;

        /// <summary>
        /// 1,000,000 μBTC are in 1 BTC
        /// </summary>
        public const UInt64 ΜBtcInOneBtc = ( UInt64 )( BTC / ΜBtc );

        /// <summary>
        /// Initialize the wallet with the specified amount of satoshi.
        /// </summary>
        /// <param name="satoshi"></param>
        public SimpleBitcoinWallet( Int64 satoshi ) : this( balance: satoshi.ToBTC() ) { }

        public SimpleBitcoinWallet( ISimpleWallet wallet ) : this( balance: wallet.Balance ) {
        }

        /// <summary>
        /// Initialize the wallet with the specified <paramref name="balance"/> .
        /// </summary>
        /// <param name="balance"></param>
        public SimpleBitcoinWallet( Decimal balance ) {
            this._balance = balance.Sanitize();
            this.Timeout = Minutes.One;
            this._hashcode = Randem.NextInt32();
        }

        public SimpleBitcoinWallet() : this( balance: 0.0m ) {
        }

        public Decimal Balance {
            get {
                try {
                    return this._access.TryEnterReadLock( timeout: this.Timeout ) ? this._balance : Decimal.Zero;
                }
                finally {
                    if ( this._access.IsReadLockHeld ) {
                        this._access.ExitReadLock();
                    }
                }
            }
        }

        public Label LabelToFlashOnChanges { get; set; }

        public Action<Decimal> OnAfterDeposit { get; set; }

        public Action<Decimal> OnAfterWithdraw { get; set; }

        public Action<Decimal> OnAnyUpdate { get; set; }

        public Action<Decimal> OnBeforeDeposit { get; set; }

        public Action<Decimal> OnBeforeWithdraw { get; set; }

        /// <summary>
        /// <para>Defaults to <see cref="Seconds.Thirty"/> in the ctor.</para>
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// <para>Static comparison.</para>
        /// <para>Returns true if the wallets are the same instance.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] SimpleBitcoinWallet left, [CanBeNull] SimpleBitcoinWallet right ) => ReferenceEquals( left, right );

        public Int32 CompareTo( [NotNull] SimpleBitcoinWallet otherWallet ) {
            if ( otherWallet is null ) {
                throw new ArgumentNullException(nameof( otherWallet ) );
            }

            return this.Balance.CompareTo( otherWallet.Balance );
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this._access.Dispose();

        /// <summary>
        /// Indicates whether the current wallet is the same as the <paramref name="otherWallet"/> wallet.
        /// </summary>
        /// <param name="otherWallet">Annother to compare with this wallet.</param>
        public Boolean Equals( SimpleBitcoinWallet otherWallet ) => Equals( left: this, right: otherWallet );

        public override Int32 GetHashCode() => this._hashcode;

        public override String ToString() => $"฿ {this.Balance:F8}";

        /// <summary>
        /// Add any (+-)amount directly to the balance.
        /// </summary>
        /// <param name="amount">  </param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryAdd( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }

            try {
                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) ) {
                    return false;
                }

                this._balance += amount;
                this.LabelToFlashOnChanges.Flash();
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }

                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        public Boolean TryAdd( [NotNull] SimpleBitcoinWallet wallet, Boolean sanitize = true ) {
            if ( wallet is null ) {
                throw new ArgumentNullException(nameof( wallet ) );
            }

            return this.TryAdd( amount: wallet.Balance, sanitize: sanitize );
        }

        /// <summary>
        /// Attempt to deposit amoount (larger than zero) to the <see cref="Balance"/> .
        /// </summary>
        /// <param name="amount">  </param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryDeposit( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }

            if ( amount <= Decimal.Zero ) {
                return false;
            }

            this.OnBeforeDeposit?.Invoke( amount );

            if ( !this.TryAdd( amount: amount ) ) {
                return false;
            }

            this.OnAfterDeposit?.Invoke( amount );
            return true;
        }

        public Boolean TryTransfer( Decimal amount, ref SimpleBitcoinWallet intoWallet, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }

            if ( amount <= Decimal.Zero ) {
                return false;
            }

            Decimal? withdrewAmount = null;
            try {
                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) ) {
                    return false;
                }

                if ( this._balance < amount ) {
                    return false;
                }

                this._balance -= amount;
                this.LabelToFlashOnChanges.Flash();
                withdrewAmount = amount;
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }

                if ( withdrewAmount.HasValue ) {
                    intoWallet.TryDeposit( amount: withdrewAmount.Value, sanitize: false );
                }

                this.OnAfterWithdraw?.Invoke( amount );
                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        /// <summary>
        /// <para>Directly sets the <see cref="Balance"/> of this wallet.</para>
        /// </summary>
        /// <param name="amount">  </param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true ) {
            try {
                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) ) {
                    return false;
                }

                this._balance = sanitize ? amount.Sanitize() : amount;

                this.LabelToFlashOnChanges.Flash();
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }

                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        public void TryUpdateBalance( SimpleBitcoinWallet simpleBitcoinWallet ) => this.TryUpdateBalance( amount: simpleBitcoinWallet.Balance );

        /// <summary>
        /// <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
        /// <para>If the amount is not available, then nothing is withdrawn.</para>
        /// </summary>
        /// <param name="amount">  </param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }

            if ( amount <= Decimal.Zero ) {
                return false;
            }

            try {
                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) ) {
                    return false;
                }

                if ( this._balance < amount ) {
                    return false;
                }

                this._balance -= amount;
                this.LabelToFlashOnChanges.Flash();
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
        /// Attempt to withdraw an amount (must be larger than Zero) from the wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( [NotNull] SimpleBitcoinWallet wallet ) {
            if ( wallet is null ) {
                throw new ArgumentNullException(nameof( wallet ) );
            }

            return this.TryWithdraw( amount: wallet.Balance );
        }
    }
}