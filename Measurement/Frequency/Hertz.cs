// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Hertz.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/Hertz.cs" was last formatted by Protiguous on 2018/05/24 at 7:26 PM.

namespace Librainian.Measurement.Frequency {

    using System;
    using System.Diagnostics;
    using Maths;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Time;

    /// <summary>
    ///     http: //wikipedia.org/wiki/Frequency
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Hertz {

        [JsonProperty]
        private readonly Decimal _value;

        /// <summary>
        ///     Fifteen <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz Fifteen = new Hertz( 15 );

        /// <summary>
        ///     59. 9 <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz FiftyNinePointNine = new Hertz( 59.9 );

        /// <summary>
        ///     Five <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz Five = new Hertz( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz FiveHundred = new Hertz( 500 );

        /// <summary>
        ///     111. 1 Hertz <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz Hertz111 = new Hertz( 111.1 );

        /// <summary>
        ///     One <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz One = new Hertz( 1 );

        /// <summary>
        ///     120 <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz OneHundredTwenty = new Hertz( 120 );

        /// <summary>
        ///     One Thousand Nine <see cref="Hertz" /> (Prime).
        /// </summary>
        public static readonly Hertz OneThousandNine = new Hertz( 1009 );

        /// <summary>
        ///     Sixty <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz Sixty = new Hertz( 60 );

        /// <summary>
        ///     Ten <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz Ten = new Hertz( 10 );

        /// <summary>
        ///     Three <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz Three = new Hertz( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz ThreeHundredThirtyThree = new Hertz( 333 );

        /// <summary>
        ///     Two <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz Two = new Hertz( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz TwoHundred = new Hertz( 200 );

        /// <summary>
        ///     211 <see cref="Hertz" /> (Prime).
        /// </summary>
        public static readonly Hertz TwoHundredEleven = new Hertz( 211 );

        /// <summary>
        ///     Two.Five <see cref="Hertz" /> s.
        /// </summary>
        public static readonly Hertz TwoPointFive = new Hertz( 2.5 );

        //faster WPM than a female (~240wpm)

        //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Thousand Three <see cref="Hertz" /> (Prime).
        /// </summary>
        public static readonly Hertz TwoThousandThree = new Hertz( 2003 );

        /// <summary>
        ///     One <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz Zero = new Hertz( 0 );

        public Decimal Value => this._value;

        static Hertz() {

            //Assert.AreSame( Zero, MinValue );
            Assert.That( One < Two );
            Assert.That( Ten > One );

            //Assert.AreEqual( new Hertz( 4.7 ), new Milliseconds( 213 ) );
        }

        public Hertz( Decimal frequency ) {
            if ( frequency <= 0m.Epsilon() ) { this._value = 0m.Epsilon(); }
            else { this._value = frequency >= Decimal.MaxValue ? Decimal.MaxValue : frequency; }
        }

        public Hertz( UInt64 frequency ) : this( ( Decimal )frequency ) { }

        public Hertz( Double frequency ) : this( ( Decimal )frequency ) { }

        public static implicit operator Span( Hertz hertz ) => new Seconds( 1.0m / hertz.Value );

        public static implicit operator TimeSpan( Hertz hertz ) => TimeSpan.FromSeconds( ( Double )( 1.0m / hertz.Value ) );

        public static Boolean operator <( Hertz left, Hertz rhs ) => left.Value.CompareTo( rhs.Value ) < 0;

        public static Boolean operator >( Hertz left, Hertz rhs ) => left.Value.CompareTo( rhs.Value ) > 0;

        public override String ToString() => $"{this.Value} hertz ({( ( TimeSpan )this ).Simpler()})";
    }
}