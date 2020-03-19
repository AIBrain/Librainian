// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Hertz.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Hertz.cs" was last formatted by Protiguous on 2020/03/16 at 3:07 PM.

namespace Librainian.Measurement.Frequency {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Time;

    /// <summary>http: //wikipedia.org/wiki/Frequency</summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Hertz {

        [JsonProperty]
        public Decimal Value { get; }

        /// <summary>Fifteen <see cref="Hertz" /> s.</summary>
        public static readonly Hertz Fifteen = new Hertz( frequency: 15 );

        /// <summary>59. 9 <see cref="Hertz" />.</summary>
        public static readonly Hertz FiftyNinePointNine = new Hertz( frequency: 59.9 );

        /// <summary>Five <see cref="Hertz" /> s.</summary>
        public static readonly Hertz Five = new Hertz( frequency: 5 );

        /// <summary>Five Hundred <see cref="Hertz" /> s.</summary>
        public static readonly Hertz FiveHundred = new Hertz( frequency: 500 );

        /// <summary>111. 1 Hertz <see cref="Hertz" />.</summary>
        public static readonly Hertz Hertz111 = new Hertz( frequency: 111.1 );

        /// <summary>One <see cref="Hertz" />.</summary>
        public static readonly Hertz One = new Hertz( frequency: 1 );

        /// <summary>120 <see cref="Hertz" />.</summary>
        public static readonly Hertz OneHundredTwenty = new Hertz( frequency: 120 );

        /// <summary>One Thousand Nine <see cref="Hertz" /> (Prime).</summary>
        public static readonly Hertz OneThousandNine = new Hertz( frequency: 1009 );

        /// <summary>Sixty <see cref="Hertz" />.</summary>
        public static readonly Hertz Sixty = new Hertz( frequency: 60 );

        /// <summary>Ten <see cref="Hertz" /> s.</summary>
        public static readonly Hertz Ten = new Hertz( frequency: 10 );

        /// <summary>Three <see cref="Hertz" /> s.</summary>
        public static readonly Hertz Three = new Hertz( frequency: 3 );

        /// <summary>Three Three Three <see cref="Hertz" />.</summary>
        public static readonly Hertz ThreeHundredThirtyThree = new Hertz( frequency: 333 );

        /// <summary>Two <see cref="Hertz" /> s.</summary>
        public static readonly Hertz Two = new Hertz( frequency: 2 );

        /// <summary>Two Hundred <see cref="Hertz" />.</summary>
        public static readonly Hertz TwoHundred = new Hertz( frequency: 200 );

        /// <summary>211 <see cref="Hertz" /> (Prime).</summary>
        public static readonly Hertz TwoHundredEleven = new Hertz( frequency: 211 );

        /// <summary>Two.Five <see cref="Hertz" /> s.</summary>
        public static readonly Hertz TwoPointFive = new Hertz( frequency: 2.5 );

        /// <summary>Two Thousand Three <see cref="Hertz" /> (Prime).</summary>
        public static readonly Hertz TwoThousandThree = new Hertz( frequency: 2003 );

        //faster WPM than a female (~240wpm)
        /// <summary>One <see cref="Hertz" />.</summary>
        public static readonly Hertz Zero = new Hertz( frequency: 0 );

        public Hertz( Decimal frequency ) {
            if ( frequency <= 0m.Epsilon() ) {
                this.Value = 0m.Epsilon();
            }
            else {
                this.Value = frequency >= Decimal.MaxValue ? Decimal.MaxValue : frequency;
            }
        }

        public Hertz( UInt64 frequency ) : this( ( Decimal ) frequency ) { }

        public Hertz( Double frequency ) : this( ( Decimal ) frequency ) { }

        public static implicit operator SpanOfTime( [NotNull] Hertz hertz ) => new Seconds( 1.0m / hertz.Value );

        public static implicit operator TimeSpan( [NotNull] Hertz hertz ) => TimeSpan.FromSeconds( ( Double ) ( 1.0m / hertz.Value ) );

        public static Boolean operator <( [NotNull] Hertz left, [NotNull] Hertz right ) => left.Value.CompareTo( value: right.Value ) < 0;

        public static Boolean operator >( [NotNull] Hertz left, [NotNull] Hertz right ) => left.Value.CompareTo( value: right.Value ) > 0;

        [NotNull]
        public override String ToString() => $"{this.Value} hertz ({( ( TimeSpan ) this ).Simpler()})";

    }

}