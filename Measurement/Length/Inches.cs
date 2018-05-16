// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Inches.cs",
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
// "Librainian/Librainian/Inches.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Length {

    using System;
    using Newtonsoft.Json;
    using Numerics;

    [JsonObject]
    public struct Inches {

        //public static readonly Inches MaxValue = new Inches( inches: Decimal.MaxValue );

        //public static readonly Inches MinValue = new Inches( inches: Decimal.MinValue );

        /// <summary>One <see cref="Inches" /> .</summary>
        public static readonly Inches One = new Inches( inches: 1 );

        /// <summary>Two <see cref="Inches" /> .</summary>
        public static readonly Inches Two = new Inches( inches: 2 );

        [JsonProperty]
        public readonly BigRational Value;

        public Inches( Decimal inches ) => this.Value = inches;

        //public Inches( Millimeters millimeters ) {
        //    var val = millimeters.Value / Extensions.MillimetersInSingleInch;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        //public Inches( Centimeters centimeters ) {
        //    var val = centimeters.Value * 2.54;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }
}