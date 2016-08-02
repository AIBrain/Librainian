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
// "Librainian/Inches.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

    using System;
    using Newtonsoft.Json;

    [JsonObject]
    public struct Inches {

        /// <summary>About 584.9 million years.</summary>
        public static readonly Inches MaxValue = new Inches( inches: Decimal.MaxValue );

        /// <summary>About zero. :P</summary>
        public static readonly Inches MinValue = new Inches( inches: Decimal.MinValue );

        /// <summary>One <see cref="Inches" /> .</summary>
        public static readonly Inches One = new Inches( inches: 1 );

        /// <summary>Two <see cref="Inches" /> .</summary>
        public static readonly Inches Two = new Inches( inches: 2 );

        [JsonProperty]
        public readonly Decimal Value;

        static Inches() {

            //Assert.That( One < Inch.One );
            //Assert.That( One < Feet.One );
        }

        public Inches( Decimal inches ) {
            this.Value = inches;
        }

        public Inches( Millimeters millimeters ) {
            var val = millimeters.Value / Extensions.MillimetersInSingleInch;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public Inches( Centimeters centimeters ) {
            var val = centimeters.Value / Extensions.CentimetersinSingleMeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }
}