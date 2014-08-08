#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/BPM.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Measurement.Frequency {
    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Time;

    /// <summary>
    ///     BPM. Beats Per Minute
    /// </summary>
    [DataContract( IsReference = true )]
    public struct BPM : IComparable< BPM > {
        /// <summary>
        ///     Ten <see cref="BPM" />s.
        /// </summary>
        public static readonly BPM Fifteen = new BPM( 15 );

        /// <summary>
        ///     Five <see cref="BPM" />s.
        /// </summary>
        public static readonly BPM Five = new BPM( 5 );

        /// <summary>
        ///     Five Hundred <see cref="BPM" />s.
        /// </summary>
        public static readonly BPM FiveHundred = new BPM( 500 );

        /// <summary>
        ///     111. 1 Hertz <see cref="BPM" />.
        /// </summary>
        public static readonly BPM Hertz111 = new BPM( 9 );

        /// <summary>
        ///     About 584.9 million years.
        /// </summary>
        public static readonly BPM MaxValue = new BPM( UInt64.MaxValue );

        /// <summary>
        ///     About zero. :P
        /// </summary>
        public static readonly BPM MinValue = new BPM( Decimal.MinValue );

        /// <summary>
        ///     One <see cref="BPM" />.
        /// </summary>
        public static readonly BPM One = new BPM( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="BPM" /> (Prime).
        /// </summary>
        public static readonly BPM OneThousandNine = new BPM( 1009 );

        /// <summary>
        ///     Ten <see cref="BPM" />s.
        /// </summary>
        public static readonly BPM Ten = new BPM( 10 );

        /// <summary>
        ///     Three <see cref="BPM" />s.
        /// </summary>
        public static readonly BPM Three = new BPM( 3 );

        /// <summary>
        ///     Three Three Three <see cref="BPM" />.
        /// </summary>
        public static readonly BPM ThreeHundredThirtyThree = new BPM( 333 );

        /// <summary>
        ///     Two <see cref="BPM" />s.
        /// </summary>
        public static readonly BPM Two = new BPM( 2 );

        /// <summary>
        ///     Two Hundred <see cref="BPM" />.
        /// </summary>
        public static readonly BPM TwoHundred = new BPM( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="BPM" /> (Prime).
        /// </summary>
        public static readonly BPM TwoHundredEleven = new BPM( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="BPM" /> (Prime).
        /// </summary>
        public static readonly BPM TwoThousandThree = new BPM( 2003 );

        [DataMember] public readonly Decimal Value;

        static BPM() {
            One.Should().BeLessThan( Two );
            Two.Should().BeGreaterThan( One );
        }

        public BPM( Decimal bpm ) {
            this.Value = bpm;
        }

        public int CompareTo( BPM other ) {
            return this.Value.CompareTo( other.Value );
        }

        public static implicit operator TimeSpan( BPM bpm ) {
            return TimeSpan.FromMilliseconds( ( Double ) bpm.Value/Seconds.InOneMinute ); //BUG is this correct?
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }
    }
}
