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
// "Librainian/FPS.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Frequency {

    using System;
    using System.Diagnostics;
    using Maths;
    using Newtonsoft.Json;
    using Time;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://en.wikipedia.org/wiki/Frame_rate" />
    [JsonObject]
    [DebuggerDisplay( "{ToString(),nq}" )]
    public struct Fps {

        [JsonProperty]
        private readonly Decimal _value;

        public Fps( Decimal fps ) {
            if ( fps <= 0m.Epsilon() ) {
                this._value = 0m.Epsilon();
            }
            else {
                this._value = fps >= Decimal.MaxValue ? Decimal.MaxValue : fps;
            }
        }

        /// <summary>
        ///     Frames per second.
        /// </summary>
        /// <param name="fps"></param>
        public Fps( UInt64 fps ) : this( ( Decimal )fps ) { }

        /// <summary>
        ///     Frames per second.
        /// </summary>
        /// <param name="fps"></param>
        public Fps( Double fps ) : this( ( Decimal )fps ) { }

        /// <summary>
        ///     Fifteen <see cref="Fps" /> s.
        /// </summary>
        public static Fps Fifteen { get; } = new Fps( 15 );

        /// <summary>
        ///     59. 9 <see cref="Fps" />.
        /// </summary>
        public static Fps FiftyNinePointNine { get; } = new Fps( 59.9 );

        /// <summary>
        ///     Five <see cref="Fps" /> s.
        /// </summary>
        public static Fps Five { get; } = new Fps( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Fps" /> s.
        /// </summary>
        public static Fps FiveHundred { get; } = new Fps( 500 );

        /// <summary>
        ///     111. 1 <see cref="Fps" />.
        /// </summary>
        public static Fps Hertz111 { get; } = new Fps( 111.1 );

        /// <summary>
        ///     One <see cref="Fps" />.
        /// </summary>
        public static Fps One { get; } = new Fps( 1 );

        /// <summary>
        ///     120 <see cref="Fps" />.
        /// </summary>
        public static Fps OneHundredTwenty { get; } = new Fps( 120 );

        /// <summary>
        ///     One Thousand Nine <see cref="Fps" /> (Prime).
        /// </summary>
        public static Fps OneThousandNine { get; } = new Fps( 1009 );

        /// <summary>
        ///     Sixty <see cref="Fps" />.
        /// </summary>
        public static Fps Sixty { get; } = new Fps( 60 );

        /// <summary>
        ///     Ten <see cref="Fps" /> s.
        /// </summary>
        public static Fps Ten { get; } = new Fps( 10 );

        public static TimeSpan Thirty { get; } = new Fps( 30 );

        /// <summary>
        ///     Three <see cref="Fps" /> s.
        /// </summary>
        public static Fps Three { get; } = new Fps( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Fps" />.
        /// </summary>
        public static Fps ThreeHundredThirtyThree { get; } = new Fps( 333 );

        /// <summary>
        ///     Two <see cref="Fps" /> s.
        /// </summary>
        public static Fps Two { get; } = new Fps( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Fps" />.
        /// </summary>
        public static Fps TwoHundred { get; } = new Fps( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Fps" /> (Prime).
        /// </summary>
        public static Fps TwoHundredEleven { get; } = new Fps( 211 );

        /// <summary>
        ///     Two.Five <see cref="Fps" /> s.
        /// </summary>
        public static Fps TwoPointFive { get; } = new Fps( 2.5 );

        //faster WPM than a female (~240wpm)

        //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Thousand Three <see cref="Fps" /> (Prime).
        /// </summary>
        public static Fps TwoThousandThree { get; } = new Fps( 2003 );

        /// <summary>
        ///     One <see cref="Fps" />.
        /// </summary>
        public static Fps Zero { get; } = new Fps( 0 );

        public Decimal Value => this._value;

        public static implicit operator Span( Fps fps ) => new Seconds( 1.0m / fps.Value );

        public static implicit operator TimeSpan( Fps fps ) => TimeSpan.FromSeconds( ( Double )( 1.0m / fps.Value ) );

        public static Boolean operator <( Fps lhs, Fps rhs ) => lhs.Value.CompareTo( rhs.Value ) < 0;

        public static Boolean operator >( Fps lhs, Fps rhs ) => lhs.Value.CompareTo( rhs.Value ) > 0;

        public override String ToString() => $"{this.Value} FPS ({( ( TimeSpan )this ).Simpler()})";
    }
}