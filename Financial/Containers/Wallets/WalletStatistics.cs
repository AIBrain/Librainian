// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WalletStatistics.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/WalletStatistics.cs" was last formatted by Protiguous on 2018/05/21 at 10:02 PM.

namespace Librainian.Financial.Containers.Wallets {

    using System;
    using System.Threading;
    using JetBrains.Annotations;
    using Magic;
    using Newtonsoft.Json;

    [JsonObject]
    public class WalletStatistics : ABetterClassDispose {

        [NotNull]
        private readonly ReaderWriterLockSlim _depositLock;

        [NotNull]
        private readonly ReaderWriterLockSlim _withwrawLock;

        [JsonProperty]
        private Decimal _allTimeDeposited;

        [JsonProperty]
        private Decimal _allTimeWithdrawn;

        public Decimal AllTimeDeposited {
            get {
                try {
                    this._depositLock.EnterReadLock();

                    return this._allTimeDeposited;
                }
                finally { this._depositLock.ExitReadLock(); }
            }

            set {
                try {
                    this._depositLock.EnterWriteLock();
                    this._allTimeDeposited = value;
                }
                finally { this._depositLock.ExitWriteLock(); }
            }
        }

        [JsonProperty]
        public Decimal AllTimeWithdrawn {
            get {
                try {
                    this._withwrawLock.EnterReadLock();

                    return this._allTimeWithdrawn;
                }
                finally { this._withwrawLock.ExitReadLock(); }
            }

            set {
                try {
                    this._withwrawLock.EnterWriteLock();
                    this._allTimeWithdrawn = value;
                }
                finally { this._withwrawLock.ExitWriteLock(); }
            }
        }

        [JsonProperty]
        public DateTime InstanceCreationTime { get; private set; }

        public WalletStatistics() {
            this._depositLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this._withwrawLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

            this.Reset();
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            this._depositLock.Dispose();
            this._withwrawLock.Dispose();
        }

        public void Reset() {
            this.InstanceCreationTime = DateTime.UtcNow;
            this.AllTimeDeposited = Decimal.Zero;
            this.AllTimeWithdrawn = Decimal.Zero;
        }
    }
}