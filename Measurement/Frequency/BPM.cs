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
// "Librainian/BPM.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Frequency {

    using System;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Time;

    /// <summary>BPM. Beats Per Minute</summary>
    [JsonObject]
    public struct Bpm : IComparable<Bpm> {

        //TODO BPM and WPM

        /// <summary>Ten <see cref="Bpm" /> s.</summary>
        public static readonly Bpm Fifteen = new Bpm( 15 );

        /// <summary>Five <see cref="Bpm" /> s.</summary>
        public static readonly Bpm Five = new Bpm( 5 );

        /// <summary>Five Hundred <see cref="Bpm" /> s.</summary>
        public static readonly Bpm FiveHundred = new Bpm( 500 );

        /// <summary>111. 1 Hertz <see cref="Bpm" />.</summary>
        public static readonly Bpm Hertz111 = new Bpm( 9 );

        /// <summary></summary>
        public static readonly Bpm MaxValue = new Bpm( UInt64.MaxValue );

        /// <summary>About zero. :P</summary>
        public static readonly Bpm MinValue = new Bpm( Decimal.MinValue );

        /// <summary>One <see cref="Bpm" />.</summary>
        public static readonly Bpm One = new Bpm( 1 );

        /// <summary>One Thousand Nine <see cref="Bpm" /> (Prime).</summary>
        public static readonly Bpm OneThousandNine = new Bpm( 1009 );

        /// <summary>Ten <see cref="Bpm" /> s.</summary>
        public static readonly Bpm Ten = new Bpm( 10 );

        /// <summary>Three <see cref="Bpm" /> s.</summary>
        public static readonly Bpm Three = new Bpm( 3 );

        /// <summary>Three Three Three <see cref="Bpm" />.</summary>
        public static readonly Bpm ThreeHundredThirtyThree = new Bpm( 333 );

        /// <summary>Two <see cref="Bpm" /> s.</summary>
        public static readonly Bpm Two = new Bpm( 2 );

        /// <summary>Two Hundred <see cref="Bpm" />.</summary>
        public static readonly Bpm TwoHundred = new Bpm( 200 );

        /// <summary>Two Hundred Eleven <see cref="Bpm" /> (Prime).</summary>
        public static readonly Bpm TwoHundredEleven = new Bpm( 211 );

        /// <summary>Two Thousand Three <see cref="Bpm" /> (Prime).</summary>
        public static readonly Bpm TwoThousandThree = new Bpm( 2003 );

        [JsonProperty]
        public readonly Decimal Value;

        public Bpm( Decimal bpm ) => this.Value = bpm;

	    public static implicit operator TimeSpan( Bpm bpm ) => TimeSpan.FromMilliseconds( ( Double )bpm.Value / Seconds.InOneMinute );

        public Int32 CompareTo( Bpm other ) => this.Value.CompareTo( other.Value );

        public override Int32 GetHashCode() => this.Value.GetHashCode();
    }

    [TestFixture]
    public static class BPMTests {

        [Test]
        public static void TestBPM() {
            Bpm.One.Should().BeLessThan( Bpm.Two );
            Bpm.Two.Should().BeGreaterThan( Bpm.One );
        }

    }

}