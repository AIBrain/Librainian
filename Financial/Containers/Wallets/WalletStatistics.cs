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
// "Librainian/WalletStatistics.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Financial.Containers.Wallets {

    using System;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public class WalletStatistics {

        [NotNull]
        private readonly ReaderWriterLockSlim _depositLock;

        [NotNull]
        private readonly ReaderWriterLockSlim _withwrawLock;

        [JsonProperty]
        private Decimal _allTimeDeposited;

        [JsonProperty]
        private Decimal _allTimeWithdrawn;

        public WalletStatistics() {
            this._depositLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this._withwrawLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

            this.Reset();
        }

        public Decimal AllTimeDeposited {
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

        [JsonProperty]
        public Decimal AllTimeWithdrawn {
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

        [JsonProperty]
        public DateTime InstanceCreationTime {
            get; private set;
        }

        public void Reset() {
            this.InstanceCreationTime = DateTime.UtcNow;
            this.AllTimeDeposited = Decimal.Zero;
            this.AllTimeWithdrawn = Decimal.Zero;
        }
    }
}