// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Centimeters.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/Centimeters.cs" was last formatted by Protiguous on 2018/05/24 at 7:26 PM.

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