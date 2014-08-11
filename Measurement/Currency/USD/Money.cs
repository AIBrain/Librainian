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
// "Librainian/Money.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.USD {
    using System;
    using Annotations;
    using NUnit.Framework;

    /// <summary>
    ///     USA Dollars and coins.
    /// </summary>
    public class Money : ICurrency {
        [NotNull] public readonly Wallet Wallet = Extensions.CreateWallet();

        public Money( Decimal amount ) {
            var leftOverAmount = Extensions.Fund( this.Wallet, amount ).Result;
            Assert.AreEqual( leftOverAmount, Decimal.Zero );
        }

        /// <summary>
        ///     Example new Money(123.4567).Dollars == 123.0000
        /// </summary>
        public Decimal Dollars { get { return Math.Truncate( this.Wallet.Total ); } }

        /// <summary>
        ///     Example new Money(123.4567).Cents == 0.4567
        /// </summary>
        public Decimal Cents {
            get {
                var total = this.Wallet.Total;
                return Math.Truncate( total ) - total;
            }
        }
    }
}
