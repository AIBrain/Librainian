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
// "Librainian/Bitcoins.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Currency.BTC {

    using System;

    /// <summary>
    ///     Bitcoins
    /// </summary>
    /// <see cref="https://en.bitcoin.it/wiki/FAQ" />
    //TODO
    public class Bitcoins : ICurrency {

        /// <summary>Example new <see cref="Bitcoins" />(123.4567).BTC == 123.0000</summary>
        public readonly Decimal Btc;

        /// <summary>
        ///     Example new <see cref="Bitcoins" />(123.45674564645634). <see cref="Satoshis" /> == 0.4567
        /// </summary>
        /// <remarks>lemOn91: "100 satoshis = 1uBTC. 1000 uBTC = 1mBTC. 1000 mBTC = 1BTC"</remarks>
        /// <remarks>The amount is in satoshis! 1 BTC = 100000000 satoshis.</remarks>
        public readonly Decimal Satoshis;

        public readonly Decimal Value;

        public Bitcoins( Decimal value ) {
            this.Value = value;
            this.Btc = Math.Truncate( value );

            //this.Satoshis = this.Dollars - value;
        }
    }
}