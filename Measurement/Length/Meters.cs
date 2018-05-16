// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Meters.cs",
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
// "Librainian/Librainian/Meters.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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