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
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Windows.Forms;
    using Annotations;
    using Controls;
    using Threading;
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
    [DataContract(IsReference = true)]
    public class SimpleBitcoinWallet : SimpleWallet, IEquatable<SimpleBitcoinWallet> {
        public const Decimal BTC = mBTC * 1000.0M;

        public const Decimal mBTC = μBTC * 1000.0M;

        public const Decimal mBTC1 = mBTC * 1.0M;

        public const Decimal mBTC2 = mBTC * 2.0M;

        public const Decimal mBTC3 = mBTC * 3.0M;

        public const Decimal mBTC4 = mBTC * 4.0M;

        public const Decimal mBTC5 = mBTC * 5.0M;

        public const Decimal mBTCPerBTC = BTC / mBTC;

        public const Decimal OneSatoshi = 1.0m * BTCInOneSatoshi;

        public const Decimal BTCInOneSatoshi = 0.00000001M;

        public const Decimal SatoshiInOneBTC = BTC / BTCInOneSatoshi;

        public const Decimal TenSatoshi = 10.0m * BTCInOneSatoshi;

        public const Decimal ZeroSatoshi = 0.0m * BTCInOneSatoshi;

        public const Decimal μBTC = BTCInOneSatoshi * 100.0M;

        public const Decimal μBTCPerBTC = BTC / μBTC;

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

      

        [DataMember]
        private Decimal _balance;
        private readonly int _hashcode;

        public SimpleBitcoinWallet() {
            this.Timeout = Minutes.One;
            this._hashcode = Randem.NextInt32();
        }

        public SimpleBitcoinWallet( Label labelToFlashOnChanges ) : this() {
            this.LabelToFlashOnChanges = labelToFlashOnChanges;
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




        /// <summary>
        /// <para>Static comparison.</para>
        /// <para>Returns true if the wallets are the same instance.</para>
        /// <para>Returns true if the wallets have the same balance.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [ CanBeNull ] SimpleBitcoinWallet left, [ CanBeNull ] SimpleBitcoinWallet right ) {

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
        public bool Equals( SimpleBitcoinWallet other ) {
            return Equals( this, other );
        }

        public override String ToString() {
            return String.Format( "฿ {0:f8}", this.Balance );
        }

     

      

        public Boolean TryAdd( [NotNull] SimpleBitcoinWallet wallet, Boolean sanitize = true ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryAdd( wallet.Balance, sanitize );
        }

      
      

        public void TryUpdateBalance( SimpleBitcoinWallet simpleBitcoinWallet ) {
            this.TryUpdateBalance( simpleBitcoinWallet.Balance );
        }

        /// <summary>
        ///     Attempt to withdraw an amount (must be larger than Zero) from the wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public Boolean TryWithdraw( [NotNull] SimpleBitcoinWallet wallet ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            return this.TryWithdraw( wallet.Balance );
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
    }
}