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
// "Librainian/Centimeters.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Length {

    using System;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [JsonObject]
    public struct Centimeters {

        //public static readonly Centimeters MaxValue = new Centimeters( centimeters: Decimal.MaxValue );

        //public static readonly Centimeters MinValue = new Centimeters( centimeters: Decimal.MinValue );

        /// <summary>One <see cref="Centimeters" /> .</summary>
        public static readonly Centimeters One = new Centimeters( centimeters: 1 );

        /// <summary>Two <see cref="Centimeters" /> .</summary>
        public static readonly Centimeters Two = new Centimeters( centimeters: 2 );

        [JsonProperty]
        public readonly Decimal Value;

        public Centimeters( Decimal centimeters ) => this.Value = centimeters;

	    //public Centimeters( Millimeters millimeters ) {
        //    var val = millimeters.Value / Extensions.MillimetersInSingleCentimeter;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        //public Centimeters( Meters meters ) {
        //    var val = meters.Value / Extensions.CentimetersinSingleMeter;
        //    this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        //}

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }

    [TestFixture]
    public static class CentimeterTests {

        [Test]
        public static void TestCentimeters() {
            Centimeters.One.Value.Should().BeLessOrEqualTo( Centimeters.Two.Value );
            Centimeters.Two.Should().Be( Centimeters.One );
        }

    }
}