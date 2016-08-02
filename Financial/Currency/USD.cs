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
// "Librainian/USD.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Financial.Currency {

    using System;
    using Containers.Wallets;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>USA dollars and coins.</summary>
    [JsonObject]
    public class USD : Wallet {

        public USD( Decimal amount ) : base( Guid.NewGuid() ) {
            var leftOverAmount = this.Fund( amount ).Result;
            Assert.AreEqual( leftOverAmount, Decimal.Zero );
        }

        /// <summary>Example new Money(123.4567).Cents == 0.4567</summary>
        public Decimal Cents => this.TotalCoins();

        /// <summary>Example new Money(123.4567).Dollars == 123.0000</summary>
        public Decimal Dollars => this.TotalBankNotes();
    }
}