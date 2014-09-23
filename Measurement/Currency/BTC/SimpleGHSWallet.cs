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

    /// <summary>
    ///     A very simple, thread-safe,  Decimal-based wallet.
    /// </summary>
    /// <remarks>
    ///     TODO add in support for automatic persisting
    ///     TODO add in support for exploring the blockchain
    /// </remarks>
    [DebuggerDisplay( "{Formatted,nq}" )]
    [Serializable]
    [DataContract(IsReference = true)]
    public class SimpleGHSWallet : ISimpleWallet, IEquatable<SimpleGHSWallet> {
        

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        [ CanBeNull ]
        public Label LabelToFlashOnChanges { get; set; }

        [DataMember]
        private Decimal _balance;
        private readonly int _hashcode;

        public SimpleGHSWallet() {
            this.Timeout = Minutes.One;
            this._hashcode = Randem.NextInt32();
        }

        public SimpleGHSWallet( Label labelToFlashOnChanges ) : this() {
            this.LabelToFlashOnChanges = labelToFlashOnChanges;
        }


        /// <summary>
        ///     Initialize the wallet with the specified <paramref name="balance" />.
        /// </summary>
        /// <param name="balance"></param>
        public SimpleGHSWallet( Decimal balance )
            : this() {
            this._balance = balance.Sanitize();
        }

        public decimal Balance {
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

        /// <summary>
        /// <para>Static comparison.</para>
        /// <para>Returns true if the wallets are the same instance.</para>
        /// <para>Returns true if the wallets have the same balance.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [ CanBeNull ] SimpleGHSWallet left, [ CanBeNull ] SimpleGHSWallet right ) {

            if ( ReferenceEquals(left, right) ) {
                return true;
            }
            if ( null == left || null == right ) {
                return false;
            }

            return left.Balance == right.Balance;
        }

        public override int GetHashCode() {
            return this._hashcode;
        }

        /// <summary>
        /// Indicates whether the current wallet has the same balance as the <paramref name="other"/> wallet.
        /// </summary>
        /// <param name="other">Annother to compare with this wallet.</param>
        public bool Equals( SimpleGHSWallet other ) {
            return Equals( this, other );
        }

        public override String ToString() {
            return String.Format( "{0:f8} GHS", this.Balance );
        }

        /// <summary>
        ///     Add any (+-)amount directly to the balance.
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

        public Boolean TryAdd( [NotNull] SimpleGHSWallet wallet, Boolean sanitize = true ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryAdd( wallet.Balance, sanitize );
        }

        /// <summary>
        ///     Attempt to deposit btc (larger than zero) to the <see cref="Balance" />.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryDeposit( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }
            if ( amount < Decimal.Zero ) {
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

        /// <summary>
        /// <para>Directly sets the <see cref="Balance"/> of this wallet.</para>
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

        public void TryUpdateBalance( SimpleGHSWallet simpleBitcoinWallet ) {
            this.TryUpdateBalance( simpleBitcoinWallet.Balance );
        }

        /// <summary>
        ///     Attempt to withdraw an amount (must be larger than Zero) from the wallet.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( Decimal amount, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }
            if ( amount < Decimal.Zero ) {
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

        public Boolean TryWithdraw( [NotNull] SimpleGHSWallet wallet ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryWithdraw( wallet.Balance );
        }

        public Boolean TryWithdraw( Decimal amount, ref SimpleGHSWallet intoWallet, Boolean sanitize = true ) {
            if ( sanitize ) {
                amount = amount.Sanitize();
            }
            if ( amount < Decimal.Zero ) {
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
    }
}