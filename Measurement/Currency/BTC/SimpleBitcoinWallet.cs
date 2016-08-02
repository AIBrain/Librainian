// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SimpleBitcoinWallet.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Controls;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Threading;
    using Time;

    /// <summary>A very simple, thread-safe, Decimal-based bitcoin wallet.</summary>
    /// <remarks>
    ///     TODO add in support for automatic persisting TODO add in support for exploring the blockchain
    /// </remarks>
    [DebuggerDisplay( "{ToString(),nq}" )]
    [Serializable]
    [JsonObject]
    public class SimpleBitcoinWallet : IEquatable<SimpleBitcoinWallet> {

        /// <summary>1</summary>
        public const Decimal Btc = 1M;

        /// <summary>0. 001</summary>
        public const Decimal MBtc = 0.001M;

        /// <summary>1000 mBTC are in 1 BTC</summary>
        public const UInt64 MBtcInOneBtc = ( UInt64 )( Btc / MBtc );

        /// <summary>0.00000001</summary>
        public const Decimal Satoshi = 0.00000001M;

        /// <summary>100,000,000 Satoshi are in 1 BTC</summary>
        public const UInt64 SatoshiInOneBtc = ( UInt64 )( Btc / Satoshi );

        /// <summary>0.0000001</summary>
        public const Decimal TenSatoshi = 0.0000001M;

        /// <summary>0. 000001</summary>
        public const Decimal ΜBtc = 0.000001M;

        /// <summary>1,000,000 μBTC are in 1 BTC</summary>
        public const UInt64 ΜBtcInOneBtc = ( UInt64 )( Btc / ΜBtc );

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private readonly Int32 _hashcode;

        [JsonProperty]
        private Decimal _balance;

        /// <summary>Initialize the wallet with the specified amount of satoshi.</summary>
        /// <param name="satoshi"></param>
        public SimpleBitcoinWallet( Int64 satoshi ) : this( satoshi.ToBTC() ) { }

        public SimpleBitcoinWallet( SimpleWallet wallet ) : this( wallet.Balance ) {
        }

        /// <summary>
        ///     Initialize the wallet with the specified <paramref name="balance" /> .
        /// </summary>
        /// <param name="balance"></param>
        public SimpleBitcoinWallet( Decimal balance ) {
            this._balance = balance.Sanitize();
            this.Timeout = Minutes.One;
            this._hashcode = Randem.NextInt32();
        }

        public SimpleBitcoinWallet() : this( 0.0m ) {
        }

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

        public Label LabelToFlashOnChanges {
            get; set;
        }

        public Action<Decimal> OnAfterDeposit {
            get; set;
        }

        public Action<Decimal> OnAfterWithdraw {
            get; set;
        }

        public Action<Decimal> OnAnyUpdate {
            get; set;
        }

        public Action<Decimal> OnBeforeDeposit {
            get; set;
        }

        public Action<Decimal> OnBeforeWithdraw {
            get; set;
        }

        /// <summary>
        ///     <para>Defaults to <see cref="Seconds.Thirty" /> in the ctor.</para>
        /// </summary>
        public TimeSpan Timeout {
            get; set;
        }

        /// <summary>
        ///     <para>Static comparison.</para>
        ///     <para>Returns true if the wallets are the same instance.</para>
        ///     <para>Returns true if the wallets have the same balance.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] SimpleBitcoinWallet left, [CanBeNull] SimpleBitcoinWallet right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }
            if ( ( null == left ) || ( null == right ) ) {
                return false;
            }

            return left.Balance == right.Balance;
        }

        /// <summary>
        ///     Indicates whether the current wallet has the same balance as the
        ///     <paramref name="other" /> wallet.
        /// </summary>
        /// <param name="other">Annother to compare with this wallet.</param>
        public Boolean Equals( SimpleBitcoinWallet other ) => Equals( this, other );

        public override Int32 GetHashCode() => this._hashcode;

        public override String ToString() => $"฿ {this.Balance:f8}";

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryAdd( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
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
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            return this.TryAdd( wallet.Balance, sanitize );
        }

        /// <summary>
        ///     Attempt to deposit amoount (larger than zero) to the <see cref="Balance" /> .
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryDeposit( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }
            if ( amount <= Decimal.Zero ) {
                return false;
            }
            var onBeforeDeposit = this.OnBeforeDeposit;
            if ( onBeforeDeposit != null ) {
                onBeforeDeposit( amount );
            }
            if ( !this.TryAdd( amount ) ) {
                return false;
            }
            var onAfterDeposit = this.OnAfterDeposit;
            if ( onAfterDeposit != null ) {
                onAfterDeposit( amount );
            }
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
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
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
                    intoWallet.TryDeposit( withdrewAmount.Value, false );
                }

                this.OnAfterWithdraw?.Invoke( amount );
                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        /// <summary>
        ///     <para>Directly sets the <see cref="Balance" /> of this wallet.</para>
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true ) {
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
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

        public void TryUpdateBalance( SimpleBitcoinWallet simpleBitcoinWallet ) => this.TryUpdateBalance( simpleBitcoinWallet.Balance );

        /// <summary>
        ///     <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
        ///     <para>If the amount is not available, then nothing is withdrawn.</para>
        /// </summary>
        /// <param name="amount"></param>
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
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
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

        /// <summary>Attempt to withdraw an amount (must be larger than Zero) from the wallet.</summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( [NotNull] SimpleBitcoinWallet wallet ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }
            return this.TryWithdraw( wallet.Balance );
        }
    }
}