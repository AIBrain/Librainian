// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Bitcoins.cs" belongs to Rick@AIBrain.org and
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Bitcoins.cs" was last formatted by Protiguous on 2018/05/24 at 7:25 PM.

namespace Librainian.Measurement.Currency.BTC {

    using System;

    /// <summary>
    ///     Bitcoins
    /// </summary>
    /// <see cref="http://en.bitcoin.it/wiki/FAQ" />
    //TODO
    public class Bitcoins : ICurrency {

        /// <summary>Example new <see cref="Bitcoins" />(123.4567).BTC == 123.0000</summary>
        public Decimal Btc { get; }

        /// <summary>
        ///     Example new <see cref="Bitcoins" />(123.45674564645634). <see cref="Satoshis" /> == 0.4567
        /// </summary>
        /// <remarks>lemOn91: "100 satoshis = 1uBTC. 1000 uBTC = 1mBTC. 1000 mBTC = 1BTC"</remarks>
        /// <remarks>The amount is in satoshis! 1 BTC = 100000000 satoshis.</remarks>
        public Decimal Satoshis { get; }

        public Bitcoins( Decimal btc ) {
            this.Btc = btc;
            this.Satoshis = this.Btc * SimpleBitcoinWallet.SatoshiInOneBtc;
        }
    }
}