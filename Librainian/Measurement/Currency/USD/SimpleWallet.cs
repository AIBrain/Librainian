﻿// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "SimpleWallet.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "SimpleWallet.cs" was last formatted by Protiguous on 2020/03/16 at 2:56 PM.

namespace Librainian.Measurement.Currency.USD {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Utilities;

    /// <summary>A simple, thread-safe,  Decimal-based wallet.</summary>
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    public class SimpleWallet : ABetterClassDispose, ISimpleWallet {

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );

        private Decimal _balance;

        /// <summary></summary>
        /// <exception cref="TimeoutException"></exception>
        public Decimal Balance {
            get {
                try {
                    if ( !this._access.TryEnterReadLock( timeout: this.Timeout ) ) {
                        throw new TimeoutException( message: "Unable to get balance" );
                    }

                    return this._balance;
                }
                finally {
                    if ( this._access.IsReadLockHeld ) {
                        this._access.ExitReadLock();
                    }
                }
            }

            private set {
                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) ) {
                    throw new TimeoutException( message: "Unable to set the balance" );
                }

                this._balance = value;
                this._access.ExitWriteLock();
                this.OnAnyUpdate?.Invoke( obj: value );
            }
        }

        [UsedImplicitly]
        [NotNull]
        public String Formatted => this.ToString();

        [CanBeNull]
        public Label LabelToFlashOnChanges { get; set; }

        [CanBeNull]
        public Action<Decimal>? OnAfterDeposit { get; set; }

        [CanBeNull]
        public Action<Decimal>? OnAfterWithdraw { get; set; }

        [CanBeNull]
        public Action<Decimal>? OnAnyUpdate { get; set; }

        [CanBeNull]
        public Action<Decimal>? OnBeforeDeposit { get; set; }

        [CanBeNull]
        public Action<Decimal>? OnBeforeWithdraw { get; set; }

        /// <summary>
        ///     <para>Timeout went reading or writing to the b<see cref="Balance" />.</para>
        ///     <para>Defaults to one minute.</para>
        /// </summary>
        public TimeSpan Timeout { get; set; }

        public SimpleWallet() => this.Timeout = TimeSpan.FromMinutes( value: 1 );

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() {
            using ( this._access ) { }
        }

        public override String ToString() => $"{this.Balance:C}";

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Boolean TryAdd( Decimal amount ) => throw new NotImplementedException();

        public Boolean TryAdd( Currency.SimpleWallet wallet ) => throw new NotImplementedException();

        public Boolean TryDeposit( Decimal amount ) {
            if ( amount < Decimal.Zero ) {
                return default;
            }

            try {
                this.OnBeforeDeposit?.Invoke( obj: amount );
                this.Balance += amount;

                return true;
            }
            finally {
                this.OnAfterDeposit?.Invoke( obj: amount );
            }
        }

        public Boolean TryTransfer( Decimal amount, ref Currency.SimpleWallet intoWallet ) => throw new NotImplementedException();

        public void TryUpdateBalance( Currency.SimpleWallet simpleWallet ) => throw new NotImplementedException();

        public Boolean TryUpdateBalance( Decimal amount ) {
            try {
                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) ) {
                    return default;
                }

                this._balance = amount;

                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }

                var onAnyUpdate = this.OnAnyUpdate;
                onAnyUpdate?.Invoke( obj: amount );
            }
        }

        public Boolean TryWithdraw( Decimal amount ) {
            if ( amount < Decimal.Zero ) {
                return default;
            }

            try {
                var onBeforeWithdraw = this.OnBeforeWithdraw;
                onBeforeWithdraw?.Invoke( obj: amount );

                if ( !this._access.TryEnterWriteLock( timeout: this.Timeout ) || this._balance < amount ) {
                    return default;
                }

                this._balance -= amount;

                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }

                var onAfterWithdraw = this.OnAfterWithdraw;
                onAfterWithdraw?.Invoke( obj: amount );
            }
        }

        public Boolean TryWithdraw( Currency.SimpleWallet wallet ) => throw new NotImplementedException();
    }
}