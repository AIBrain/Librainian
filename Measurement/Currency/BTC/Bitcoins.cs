// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Bitcoins.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

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