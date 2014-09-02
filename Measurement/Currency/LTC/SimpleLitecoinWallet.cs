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
// "Librainian/SimpleLitecoinWallet.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.LTC {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Annotations;

    /// <summary>
    ///     A very simple, thread-safe,  System.Decimal-based wallet.
    /// </summary>
    /// <remarks>
    ///     TODO add in support for automatic persisting
    ///     TODO add in support for exploring the blockchain
    /// </remarks>
    [DebuggerDisplay( "{Formatted,nq}" )]
    [Serializable]
    public class SimpleLitecoinWallet : ISimpleWallet {
        public const  Decimal mBTC1 = mBTC*1.0M;
        public const  Decimal mBTC2 = mBTC*2.0M;
        public const  Decimal mBTC3 = mBTC*3.0M;
        public const  Decimal mBTC4 = mBTC*4.0M;
        public const  Decimal mBTC5 = mBTC*5.0M;
        public const  Decimal TenSatoshi = 10.0m*Satoshi;
        public const  Decimal SatoshiPerBTC = BTC/Satoshi;
        public const  Decimal μBTCPerBTC = BTC/μBTC;
        public const  Decimal mBTCPerBTC = BTC/mBTC;
        public const  Decimal BTC = mBTC*1000.0M;
        public const  Decimal mBTC = μBTC*1000.0M;
        public const  Decimal Satoshi = 0.00000001M;
        public const  Decimal OneSatoshi = 0.00000001M;
        public const  Decimal μBTC = Satoshi*100.0M;

        [NotNull] private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private  Decimal _balance;

        public SimpleLitecoinWallet() {
            this.Timeout = TimeSpan.FromMinutes( 1 );
        }

        /// <summary>
        ///     Initialize the wallet with the specified <paramref name="balance" />.
        /// </summary>
        /// <param name="balance"></param>
        public SimpleLitecoinWallet(Decimal balance ) : this() {
            this._balance = balance.Sanitize();
        }

        /// <summary>
        ///     Initialize the wallet with the specified amount of satoshi.
        /// </summary>
        /// <param name="satoshi"></param>
        public SimpleLitecoinWallet( long satoshi ) : this() {
            this._balance = satoshi.ToBTC();
        }

        [UsedImplicitly]
        public String Formatted { get { return this.ToString(); } }

        public TimeSpan Timeout { get; set; }

        public  Decimal Balance {
            get {
                try {
                    return this._access.TryEnterReadLock( this.Timeout ) ? this._balance :Decimal.Zero;
                }
                finally {
                    if ( this._access.IsReadLockHeld ) {
                        this._access.ExitReadLock();
                    }
                }
            }
        }

        public Action<Decimal > OnBeforeDeposit { get; set; }
        public Action<Decimal > OnAfterDeposit { get; set; }

        public Action<Decimal > OnBeforeWithdraw { get; set; }
        public Action<Decimal > OnAfterWithdraw { get; set; }

        public Action<Decimal > OnAnyUpdate { get; set; }

        /// <summary>
        ///     Attempt to deposit btc (larger than zero) to the <see cref="Balance" />.
        /// </summary>
        /// <param name="btc"></param>
        /// <param name="sanitizeBtc"></param>
        /// <returns></returns>
        public Boolean TryDeposit(Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            if ( btc <Decimal.Zero ) {
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

        /// <summary>
        ///     Attempt to withdraw an amount (must be larger than Zero) from the wallet.
        /// </summary>
        /// <param name="btc"></param>
        /// <param name="sanitizeBtc"></param>
        /// <returns></returns>
        public Boolean TryWithdraw(Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            if ( btc <Decimal.Zero ) {
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

        public Boolean TryUpdateBalance(Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }

            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                this._balance = btc;
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

        public override string ToString() {
            return String.Format( "฿ {0:f8}", this.Balance );
        }

        /// <summary>
        ///     Add any (+-)amount directly to the balance.
        /// </summary>
        /// <param name="btc"></param>
        /// <param name="sanitizeBtc"></param>
        /// <returns></returns>
        public Boolean TryAdd(Decimal btc, Boolean sanitizeBtc = true ) {
            if ( sanitizeBtc ) {
                btc = btc.Sanitize();
            }
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                this._balance += btc;
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

        public Boolean TryWithdraw( [NotNull] SimpleLitecoinWallet wallet ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryWithdraw( wallet.Balance );
        }

        public Boolean TryAdd( [NotNull] SimpleLitecoinWallet wallet, Boolean sanitizeBtc = true ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryAdd( wallet.Balance, sanitizeBtc );
        }
    }
}
