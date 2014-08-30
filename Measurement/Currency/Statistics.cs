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
// "Librainian/Statistics.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;

    [DataContract]
    [Serializable]
    public struct Statistics {
        [NotNull] private readonly ReaderWriterLockSlim _depositLock;

        [NotNull] private readonly ReaderWriterLockSlim _gainLock;

        [NotNull] private readonly ReaderWriterLockSlim _lossLock;

        [NotNull] private readonly ReaderWriterLockSlim _withwrawLock;
        [DataMember] public Guid WalletID;

        [DataMember] private  Decimal _allTimeDeposited;

        private  Decimal _allTimeWithdrawn;
        private  Decimal _gain;
        private  Decimal _loss;

        public Statistics( Guid walletID ) : this() {
            if ( walletID == Guid.Empty ) {
                throw new ArgumentNullException();
            }
            this.WalletID = walletID;
            this._depositLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this._withwrawLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this._gainLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this._lossLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this.InstanceCreationTime = DateTime.UtcNow;
            this.Reset();
        }

        public  Decimal AllTimeDeposited {
            get {
                try {
                    this._depositLock.EnterReadLock();
                    return this._allTimeDeposited;
                }
                finally {
                    this._depositLock.ExitReadLock();
                }
            }
            set {
                try {
                    this._depositLock.EnterWriteLock();
                    this._allTimeDeposited = value;
                }
                finally {
                    this._depositLock.ExitWriteLock();
                }
            }
        }

        [DataMember]
        public  Decimal AllTimeWithdrawn {
            get {
                try {
                    this._withwrawLock.EnterReadLock();
                    return this._allTimeWithdrawn;
                }
                finally {
                    this._withwrawLock.ExitReadLock();
                }
            }
            set {
                try {
                    this._withwrawLock.EnterWriteLock();
                    this._allTimeWithdrawn = value;
                }
                finally {
                    this._withwrawLock.ExitWriteLock();
                }
            }
        }

        public  Decimal Gains {
            get {
                try {
                    this._gainLock.EnterReadLock();
                    return this._gain;
                }
                finally {
                    this._gainLock.ExitReadLock();
                }
            }
            set {
                try {
                    this._gainLock.EnterWriteLock();
                    this._gain = value;
                }
                finally {
                    this._gainLock.ExitWriteLock();
                }
            }
        }

        [DataMember]
        public DateTime InstanceCreationTime { get; private set; }

        public  Decimal Losses {
            get {
                try {
                    this._lossLock.EnterReadLock();
                    return this._loss;
                }
                finally {
                    this._lossLock.ExitReadLock();
                }
            }
            set {
                try {
                    this._lossLock.EnterWriteLock();
                    this._loss = value;
                }
                finally {
                    this._lossLock.ExitWriteLock();
                }
            }
        }

        public void Reset() {
            this.AllTimeDeposited =Decimal.Zero;
            this.AllTimeWithdrawn =Decimal.Zero;
            this.Gains =Decimal.Zero;
            this.Losses =Decimal.Zero;
        }
    }
}
