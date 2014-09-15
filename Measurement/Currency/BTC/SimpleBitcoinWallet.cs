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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SimpleBitcoinWallet.cs" was last cleaned by Rick on 2014/09/09 at 5:29 PM

#endregion License & Information

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Annotations;
    using Controls;
    using Time;

    /// <summary>
    ///     A very simple, thread-safe,  Decimal-based bitcoin wallet.
    /// </summary>
    /// <remarks>
    ///     TODO add in support for automatic persisting
    ///     TODO add in support for exploring the blockchain
    /// </remarks>
    [DebuggerDisplay( "{Formatted,nq}" )]
    [Serializable]
    public class SimpleBitcoinWallet : ISimpleWallet {

        [CanBeNull]
        private readonly Label _flashLabelOnChanges;

        public const Decimal BTC = mBTC * 1000.0M;
        public const Decimal mBTC = μBTC * 1000.0M;
        public const Decimal mBTC1 = mBTC * 1.0M;
        public const Decimal mBTC2 = mBTC * 2.0M;
        public const Decimal mBTC3 = mBTC * 3.0M;
        public const Decimal mBTC4 = mBTC * 4.0M;
        public const Decimal mBTC5 = mBTC * 5.0M;
        public const Decimal mBTCPerBTC = BTC / mBTC;
        public const Decimal OneSatoshi = 1.0m * Satoshi;
        public const Decimal Satoshi = 0.00000001M;
        public const Decimal SatoshiPerBTC = BTC / Satoshi;
        public const Decimal TenSatoshi = 10.0m * Satoshi;
        public const Decimal ZeroSatoshi = 0.00000000M;
        public const Decimal μBTC = Satoshi * 100.0M;
        public const Decimal μBTCPerBTC = BTC / μBTC;

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private Decimal _balance;

        public SimpleBitcoinWallet() {
            this.Timeout = Minutes.One;
        }

        public SimpleBitcoinWallet( Label flashLabelOnChanges ) {
            this._flashLabelOnChanges = flashLabelOnChanges;
        }

        /// <summary>
        ///     Initialize the wallet with the specified amount of satoshi.
        /// </summary>
        /// <param name="satoshi"></param>
        public SimpleBitcoinWallet( long satoshi )
            : this() {
            this._balance = satoshi.ToBTC();
        }

        public SimpleBitcoinWallet( ISimpleWallet wallet )
            : this( wallet.Balance ) {
        }

        /// <summary>
        ///     Initialize the wallet with the specified <paramref name="balance" />.
        /// </summary>
        /// <param name="balance"></param>
        public SimpleBitcoinWallet( Decimal balance )
            : this() {
            this._balance = balance.Sanitize();
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

        [UsedImplicitly]
        public String Formatted {
            get {
                return this.ToString();
            }
        }

        public Action<Decimal> OnAfterDeposit {
            get;
            set;
        }

        public Action<Decimal> OnAfterWithdraw {
            get;
            set;
        }

        public Action<Decimal> OnAnyUpdate {
            get;
            set;
        }

        public Action<Decimal> OnBeforeDeposit {
            get;
            set;
        }

        public Action<Decimal> OnBeforeWithdraw {
            get;
            set;
        }

        public TimeSpan Timeout {
            get;
            set;
        }

        public override string ToString() {
            return String.Format( "฿ {0:f8}", this.Balance );
        }

        /// <summary>
        ///     Add any (+-)amount directly to the balance.
        /// </summary>
        /// <param name="btc"></param>
        /// <param name="sanitizeBtc"></param>
        /// <returns></returns>
        public Boolean TryAdd( Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                this._balance += btc;
                _flashLabelOnChanges.Flash();
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( btc );
                }
            }
        }

        public Boolean TryAdd( [NotNull] SimpleBitcoinWallet wallet, Boolean sanitizeBtc = true ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryAdd( wallet.Balance, sanitizeBtc );
        }

        /// <summary>
        ///     Attempt to deposit btc (larger than zero) to the <see cref="Balance" />.
        /// </summary>
        /// <param name="btc"></param>
        /// <param name="sanitizeBtc"></param>
        /// <returns></returns>
        public Boolean TryDeposit( Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            if ( btc < Decimal.Zero ) {
                return false;
            }
            var onBeforeDeposit = this.OnBeforeDeposit;
            if ( onBeforeDeposit != null ) {
                onBeforeDeposit( btc );
            }
            if ( !this.TryAdd( btc ) ) {
                return false;
            }
            var onAfterDeposit = this.OnAfterDeposit;
            if ( onAfterDeposit != null ) {
                onAfterDeposit( btc );
            }
            return true;
        }

        public Boolean TryUpdateBalance( Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }

            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                this._balance = btc;
                _flashLabelOnChanges.Flash();
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( btc );
                }
            }
        }

        /// <summary>
        ///     Attempt to withdraw an amount (must be larger than Zero) from the wallet.
        /// </summary>
        /// <param name="btc"></param>
        /// <param name="sanitizeBtc"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            if ( btc < Decimal.Zero ) {
                return false;
            }
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                if ( this._balance < btc ) {
                    return false;
                }
                this._balance -= btc;
                _flashLabelOnChanges.Flash();
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }
                var onWithdraw = this.OnAfterWithdraw;
                if ( onWithdraw != null ) {
                    onWithdraw( btc );
                }
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( btc );
                }
            }
        }

        public Boolean TryWithdraw( [NotNull] SimpleBitcoinWallet wallet ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryWithdraw( wallet.Balance );
        }

        public Boolean TryWithdraw( Decimal btc, ref SimpleBitcoinWallet intoWallet, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            if ( btc < Decimal.Zero ) {
                return false;
            }
            Decimal? withdrewAmount = null;
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                if ( this._balance < btc ) {
                    return false;
                }
                this._balance -= btc;
                _flashLabelOnChanges.Flash();
                withdrewAmount = btc;
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }
                if ( withdrewAmount.HasValue ) {
                    intoWallet.TryDeposit( btc: withdrewAmount.Value, sanitizeBtc: false );
                }

                var onWithdraw = this.OnAfterWithdraw;
                if ( onWithdraw != null ) {
                    onWithdraw( btc );
                }
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( btc );
                }
            }
        }
    }
}