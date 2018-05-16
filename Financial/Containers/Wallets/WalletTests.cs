// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WalletTests.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/WalletTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:42 PM.

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
        public static void Setup() { }

        //[OneTimeTearDown]
        public static void TearDown() { }

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