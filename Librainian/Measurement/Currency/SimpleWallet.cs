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
// Project: "Librainian", File: "SimpleWallet.cs" was last formatted by Protiguous on 2020/03/18 at 10:25 AM.

namespace Librainian.Measurement.Currency {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Controls;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Time;
    using Utilities;

    /// <summary>A very simple, thread-safe, Decimal-based wallet.</summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class SimpleWallet : ABetterClassDispose, ISimpleWallet, IEquatable<SimpleWallet> {

        /// <summary>Indicates whether the current wallet has the same balance as the <paramref name="other" /> wallet.</summary>
        /// <param name="other">Annother to compare with this wallet.</param>
        public Boolean Equals( SimpleWallet other ) => Equals( this, other );

        public Decimal Balance {
            get {
                if ( this._access.TryEnterReadLock( this.Timeout ) ) {
                    try {
                        return this._balance;
                    }
                    finally {
                        this._access.ExitReadLock();
                    }
                }

                return Decimal.Zero;
            }
        }

        [CanBeNull]
        public Label? LabelToFlashOnChanges { get; set; }

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

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount">  </param>
        /// <returns></returns>
        public Boolean TryAdd( Decimal amount ) {

            try {
                if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
                    try {
                        this._balance += amount;
                        this.LabelToFlashOnChanges?.Flash();

                        return true;
                    }
                    finally {
                        this._access.ExitWriteLock();
                    }
                }
                else {
                    return default;
                }
            }
            finally {

                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        public Boolean TryAdd( SimpleWallet wallet ) {
            if ( wallet is null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }

            return this.TryAdd( wallet.Balance );
        }

        /// <summary>Attempt to deposit amount (larger than zero) to the <see cref="Balance" />.</summary>
        /// <param name="amount">  </param>
        /// <returns></returns>
        public Boolean TryDeposit( Decimal amount ) {

            if ( amount > Decimal.Zero ) {
                this.OnBeforeDeposit?.Invoke( amount );

                if ( this.TryAdd( amount ) ) {
                    this.OnAfterDeposit?.Invoke( amount );

                    return true;
                }
            }

            return default;
        }

        public Boolean TryTransfer( Decimal amount, [NotNull] ref SimpleWallet intoWallet ) {
            if ( Equals( this, intoWallet ) ) {
                return default;
            }

            if ( amount <= Decimal.Zero ) {
                return default;
            }

            try {
                if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
                    try {
                        if ( this._balance < amount ) {
                            return default;
                        }

                        this._balance -= amount;
                        this.LabelToFlashOnChanges?.Flash();

                        intoWallet?.TryDeposit( amount );

                        return true;
                    }
                    finally {
                        this._access.ExitWriteLock();
                    }
                }
                else {
                    return default;
                }
            }
            finally {

                this.OnAfterWithdraw?.Invoke( amount );
                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        /// <summary>
        ///     <para>Directly sets the <see cref="Balance" /> of this wallet.</para>
        /// </summary>
        /// <param name="amount">  </param>
        /// <returns></returns>
        public Boolean TryUpdateBalance( Decimal amount ) {
            try {
                if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
                    try {
                        this._balance = amount;

                        this.LabelToFlashOnChanges?.Flash();

                        return true;
                    }
                    finally {
                        this._access.ExitWriteLock();
                    }
                }
                else {
                    return default;
                }
            }
            finally {

                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        public void TryUpdateBalance( [NotNull] SimpleWallet simpleWallet ) => this.TryUpdateBalance( simpleWallet.Balance );

        /// <summary>
        ///     <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
        ///     <para>If the amount is not available, then nothing is withdrawn.</para>
        /// </summary>
        /// <param name="amount">  </param>
        /// <returns></returns>
        public Boolean TryWithdraw( Decimal amount ) {

            if ( amount <= Decimal.Zero ) {
                return default;
            }

            try {
                if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
                    try {
                        if ( this._balance < amount ) {
                            return default;
                        }

                        this._balance -= amount;
                        this.LabelToFlashOnChanges?.Flash();

                        return true;
                    }
                    finally {
                        this._access.ExitWriteLock();
                    }
                }
                else {
                    return default;
                }
            }
            finally {

                this.OnAfterWithdraw?.Invoke( amount );
                this.OnAnyUpdate?.Invoke( amount );
            }
        }

        public Boolean TryWithdraw( SimpleWallet wallet ) {
            if ( wallet is null ) {
                throw new ArgumentNullException( nameof( wallet ) );
            }

            return this.TryWithdraw( wallet.Balance );
        }

        [NotNull]
        private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        [JsonProperty]
        private Decimal _balance;

        /// <summary>
        ///     <para>Defaults to <see cref="Seconds.Thirty" /> in the ctor.</para>
        /// </summary>
        public TimeSpan Timeout { get; set; }

        //TODO TODO add in support for automatic persisting?
        public SimpleWallet() => this.Timeout = Minutes.One;

        /// <summary>Initialize the wallet with the specified <paramref name="balance" />.</summary>
        /// <param name="balance"></param>
        public SimpleWallet( Decimal balance ) : this() => this._balance = balance;

        /// <summary>
        ///     <para>Static comparison.</para>
        ///     <para>Returns true if the wallets ARE the same instance.</para>
        ///     <para>Returns true if the wallets HAVE the same balance.</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] SimpleWallet left, [CanBeNull] SimpleWallet right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            return left.Balance == right.Balance;
        }

        /// <summary>Returns a value that indicates whether two <see cref="SimpleWallet" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( [CanBeNull] SimpleWallet left, [CanBeNull] SimpleWallet right ) => !Equals( left, right );

        /// <summary>Returns a value that indicates whether the values of two <see cref="SimpleWallet" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( [CanBeNull] SimpleWallet left, [CanBeNull] SimpleWallet right ) => Equals( left, right );

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() {
            using ( this._access ) { }
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object obj ) => Equals( this, obj as SimpleWallet );

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this._access.GetHashCode();

        public override String ToString() => $"{this.Balance:F8}";

    }

}