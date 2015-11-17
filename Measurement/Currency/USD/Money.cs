// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Money.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Currency.USD {

    using System;
    using Financial;
    using JetBrains.Annotations;
    using NUnit.Framework;

    /// <summary>USA Dollars and coins.</summary>
    public class Money : ICurrency {

        [NotNull]
        public readonly Wallet Wallet = Wallet.Create();

        /// <summary>Example new Money(123.4567).Cents == 0.4567</summary>
        public Decimal Cents {
            get {
                var total = this.Wallet.Total;
                return Math.Truncate( total ) - total;
            }
        }

        /// <summary>Example new Money(123.4567).Dollars == 123.0000</summary>
        public Decimal Dollars => Math.Truncate( this.Wallet.Total );

        public Money(Decimal amount) {
            var leftOverAmount = Extensions.Fund( this.Wallet, amount ).Result;
            Assert.AreEqual( leftOverAmount, Decimal.Zero );
        }
    }
}