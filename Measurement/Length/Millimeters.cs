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
// "Librainian/Millimeters.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

    using System;
    using Newtonsoft.Json;
    using Numerics;

    [JsonObject]
    public struct Millimeters {

        ///// <summary>.</summary>
        //public static readonly Millimeters MaxValue = new Millimeters( Decimal.MaxValue );

        //public static readonly Millimeters MinValue = new Millimeters( Decimal.MinValue );

        /// <summary>One <see cref="Millimeters" /> .</summary>
        public static readonly Millimeters One = new Millimeters( 1 );

        /// <summary>Two <see cref="Millimeters" /> .</summary>
        public static readonly Millimeters Two = new Millimeters( 2 );

        [JsonProperty]
        public readonly BigRational Value;

        public Millimeters( Decimal millimeters ) => this.Value = millimeters;

	    //public Millimeters( Centimeters centimeters ) {
        //    var val = centimeters.Value * Extensions.MillimetersInSingleCentimeter;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        //public Millimeters( Meters meters ) {
        //    var val = meters.Value * Extensions.MillimetersInSingleMeter;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }
}