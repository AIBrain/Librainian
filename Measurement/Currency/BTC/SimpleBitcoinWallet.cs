// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SimpleBitcoinWallet.cs" was last cleaned by Rick on 2014/10/16 at 2:32 AM

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Windows.Forms;
    using Annotations;
    using Controls;
    using Threading;
    using Time;

    public interface ISimpleBitcoinWallet {
    }

    /// <summary>
    /// A very simple, thread-safe, Decimal-based bitcoin wallet.
    /// </summary>
    /// <remarks>
    /// TODO add in support for automatic persisting TODO add in support for exploring the blockchain
    /// </remarks>
    [DebuggerDisplay( "{Formatted,nq}" )]
    [Serializable]
    [DataContract( IsReference = true )]
    public class SimpleBitcoinWallet : IEquatable<SimpleBitcoinWallet> {

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
        /// 0. 00000001
        /// </summary>
        public const Decimal Satoshi = 0.00000001M;

        /// <summary>
        /// 100,000,000 Satoshi are in 1 BTC
        /// </summary>
        public const UInt64 SatoshiInOneBTC = ( UInt64 )( BTC / Satoshi );

        /// <summary>
        /// 0. 0000001
        /// </summary>
        public const Decimal TenSatoshi = 0.0000001M;

        /// <summary>
        /// 0. 000001
        /// </summary>
        public const Decimal μBTC = 0.000001M;

        /// <summary>
        /// 1,000,000 μBTC are in 1 BTC
        /// </summary>
        public const UInt64 μBTCInOneBTC = ( UInt64 )( BTC / μBTC );

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private readonly int _hashcode;

        [DataMember]
        private Decimal _balance;

        /// <summary>
        /// Initialize the wallet with the specified amount of satoshi.
        /// </summary>
        /// <param name="satoshi"></param>
        public SimpleBitcoinWallet( long satoshi ) : this( satoshi.ToBTC() ) {
        }

        public SimpleBitcoinWallet( ISimpleWallet wallet ) : this( wallet.Balance ) {
        }

        /// <summary>
        /// Initialize the wallet with the specified <paramref name="balance" /> .
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

        public String Formatted {
            get {
                return this.ToString();
            }
        }

        public Label LabelToFlashOnChanges {
            get;
            set;
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

        /// <summary>
        /// <para>Defaults to <see cref="Seconds.Thirty" /> in the ctor.</para>
        /// </summary>
        public TimeSpan Timeout {
            get;
            set;
        }

        /// <summary>
        /// <para>Static comparison.</para>
        /// <para>Returns true if the wallets are the same instance.</para>
        /// <para>Returns true if the wallets have the same balance.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] SimpleBitcoinWallet left, [CanBeNull] SimpleBitcoinWallet right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }
            if ( null == left || null == right ) {
                return false;
            }

            return left.Balance == right.Balance;
        }

        /// <summary>
        /// Indicates whether the current wallet has the same balance as the
        /// <paramref name="other" /> wallet.
        /// </summary>
        /// <param name="other">Annother to compare with this wallet.</param>
        public bool Equals( SimpleBitcoinWallet other ) {
            return Equals( this, other );
        }

        public override int GetHashCode() {
            return this._hashcode;
        }

        public override String ToString() {
            return String.Format( "฿ {0:f8}", this.Balance );
        }

        /// <summary>
        /// Add any (+-)amount directly to the balance.
        /// </summary>
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
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( amount );
                }
            }
        }

        public Boolean TryAdd( [NotNull] SimpleBitcoinWallet wallet, Boolean sanitize = true ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryAdd( wallet.Balance, sanitize );
        }

        /// <summary>
        /// Attempt to deposit amoount (larger than zero) to the <see cref="Balance" /> .
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
                    intoWallet.TryDeposit( amount: withdrewAmount.Value, sanitize: false );
                }

                var onWithdraw = this.OnAfterWithdraw;
                if ( onWithdraw != null ) {
                    onWithdraw( amount );
                }
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( amount );
                }
            }
        }

        /// <summary>
        /// <para>Directly sets the <see cref="Balance" /> of this wallet.</para>
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
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( amount );
                }
            }
        }

        public void TryUpdateBalance( SimpleBitcoinWallet simpleBitcoinWallet ) {
            this.TryUpdateBalance( simpleBitcoinWallet.Balance );
        }

        /// <summary>
        /// <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
        /// <para>If the amount is not available, then nothing is withdrawn.</para>
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
                var onWithdraw = this.OnAfterWithdraw;
                if ( onWithdraw != null ) {
                    onWithdraw( amount );
                }
                var onAnyUpdate = this.OnAnyUpdate;
                if ( null != onAnyUpdate ) {
                    onAnyUpdate( amount );
                }
            }
        }

        /// <summary>
        /// Attempt to withdraw an amount (must be larger than Zero) from the wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( [NotNull] SimpleBitcoinWallet wallet ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryWithdraw( wallet.Balance );
        }
    }
}