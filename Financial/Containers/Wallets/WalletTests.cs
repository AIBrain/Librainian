// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/WalletTests.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Financial.Containers.Wallets {

    using FluentAssertions;
    using Maths;
    using NUnit.Framework;

    [TestFixture]
    public static class WalletTests {

        public static Wallet WalletMain { get; } = Wallet.Create();

        public static Wallet WalletSecond { get; } = Wallet.Create();

        public static Wallet WalletThird { get; } = Wallet.Create();

        //[OneTimeSetUp]
        public static void Setup() {
        }

        //[OneTimeTearDown]
        public static void TearDown() {
        }

        [Test]
        public static void Test_funding_an_empty_wallet() {
            WalletMain.Should().NotBeNull();

            WalletMain.ClearEverything();

            var leftOver = WalletMain.Fund( 123.45678m ).Result;

            WalletMain.Total().Should().Be( 123.45m );

            leftOver.Should().Be( 0.00678m );
        }

        [Test]
        public static void Test_random_funding_wallet_to_wallet() {
            WalletMain.Should().NotBeNull();
            WalletMain.ClearEverything();

            var leftOver = WalletMain.Fund( 0.00001m.NextDecimal( 9999999m ) ).Result;
            var total = WalletMain.Total();

            WalletSecond.Should().NotBeNull();
            WalletSecond.ClearEverything();

            var bob = WalletMain.Transfer( WalletSecond );

            total.Should().Be( WalletSecond.Total() );
        }
    }
}