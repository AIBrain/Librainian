// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WalletStatistics.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "WalletStatistics.cs" was last formatted by Protiguous on 2019/11/25 at 4:24 AM.

namespace LibrainianCore.Financial.Containers.Wallets {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Measurement.Time;
    using Utilities;

    [JsonObject]
    public class WalletStatistics : ABetterClassDispose {

        [NotNull]
        private readonly ReaderWriterLockSlim _depositLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        [NotNull]
        private readonly ReaderWriterLockSlim _withwrawLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        [JsonProperty]
        private Decimal _allTimeDeposited;

        [JsonProperty]
        private Decimal _allTimeWithdrawn;

        public Decimal AllTimeDeposited {
            get {
                if ( this._depositLock.TryEnterReadLock( Seconds.Ten ) ) {
                    try {
                        return this._allTimeDeposited;
                    }
                    finally {
                        this._depositLock.ExitReadLock();
                    }
                }

                return default;
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
        public DateTime InstanceCreationTime { get; private set; }

        public WalletStatistics() => this.Reset();

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() {
            using ( this._depositLock ) { }

            using ( this._withwrawLock ) { }
        }

        public void Reset() {
            this.InstanceCreationTime = DateTime.UtcNow;
            this.AllTimeDeposited = Decimal.Zero;
            this.AllTimeWithdrawn = Decimal.Zero;
        }
    }
}