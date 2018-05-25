// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "BPM.cs",
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
// "Librainian/Librainian/BPM.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Frequency {

    using System;
    using Newtonsoft.Json;
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
}