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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Hertz.cs" was last cleaned by Rick on 2014/09/29 at 1:56 PM

#endregion License & Information

namespace Librainian.Measurement.Frequency {

    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Annotations;
    using Maths;
    using NUnit.Framework;
    using Time;

    /// <summary>
    ///     http://wikipedia.org/wiki/Frequency
    /// </summary>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public class Hertz {

        /// <summary>
        ///     Ten <see cref="Hertz" />s.
        /// </summary>
        public static readonly Hertz Fifteen = new Hertz( 15 );

        /// <summary>
        ///     59.9 <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz FiftyNinePointNine = new Hertz( 59.9 );

        /// <summary>
        ///     Five <see cref="Hertz" />s.
        /// </summary>
        public static readonly Hertz Five = new Hertz( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Hertz" />s.
        /// </summary>
        public static readonly Hertz FiveHundred = new Hertz( 500 );

        /// <summary>
        ///     111.1 Hertz <see cref="Hertz" />.
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
        ///     Ten <see cref="Hertz" />s.
        /// </summary>
        public static readonly Hertz Ten = new Hertz( 10 );

        /// <summary>
        ///     Three <see cref="Hertz" />s.
        /// </summary>
        public static readonly Hertz Three = new Hertz( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz ThreeHundredThirtyThree = new Hertz( 333 );

        /// <summary>
        ///     Two <see cref="Hertz" />s.
        /// </summary>
        public static readonly Hertz Two = new Hertz( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz TwoHundred = new Hertz( 200 ); //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Hundred Eleven <see cref="Hertz" /> (Prime).
        /// </summary>
        public static readonly Hertz TwoHundredEleven = new Hertz( 211 ); //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Thousand Three <see cref="Hertz" /> (Prime).
        /// </summary>
        public static readonly Hertz TwoThousandThree = new Hertz( 2003 );

        /// <summary>
        ///     One <see cref="Hertz" />.
        /// </summary>
        public static readonly Hertz Zero = new Hertz( 0 );

        [DataMember]
        private readonly decimal _value;

        static Hertz() {

            //Assert.AreSame( Zero, MinValue );
            Assert.That( One < Two );
            Assert.That( Ten > One );

            //Assert.AreEqual( new Hertz( 4.7 ), new Milliseconds( 213 ) );
        }

        public Hertz( Decimal frequency ) {
            if ( frequency <= 0m.Epsilon() ) {
                this._value = 0m.Epsilon();
            }
            else {
                this._value = frequency >= Decimal.MaxValue ? Decimal.MaxValue : frequency;
            }
        }

        public Hertz( UInt64 frequency ) : this( ( Decimal )frequency ) {
        }

        public Hertz( Double frequency ) : this( ( Decimal )frequency ) {
        }

        public Decimal Value {
            get {
                return this._value;
            }
        }

        [UsedImplicitly]
        private String DebuggerDisplay {
            get {
                return String.Format( "{0} hertz ({1})", this.Value, ( ( TimeSpan )this ).Simpler() );
            }
        }

        public static implicit operator TimeSpan( Hertz hertz ) {
            return TimeSpan.FromSeconds( ( Double )( 1.0m / hertz.Value ) );
        }

        public static Boolean operator <( Hertz lhs, Hertz rhs ) {
            return lhs.Value.CompareTo( rhs.Value ) < 0;
        }

        public static Boolean operator >( Hertz lhs, Hertz rhs ) {
            return lhs.Value.CompareTo( rhs.Value ) > 0;
        }
    }
}