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
// "Librainian/Meters.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

    using System;
    using Newtonsoft.Json;
    using Numerics;

    public struct Meters {

        public static readonly Meters MaxValue = new Meters( meters: Decimal.MaxValue );

        public static readonly Meters MinValue = new Meters( meters: Decimal.MinValue );

        public static readonly Meters One = new Meters( meters: 1 );

        public static readonly Meters Two = new Meters( meters: 2 );

        [JsonProperty]
        public readonly BigRational Value;

        public Meters( Decimal meters ) => this.Value = meters;

	    public Meters( Millimeters millimeters ) {
            var val = millimeters.Value / Extensions.MillimetersInSingleCentimeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public Meters( Centimeters centimeters ) {
            var val = centimeters.Value / Extensions.CentimetersinSingleMeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }
}