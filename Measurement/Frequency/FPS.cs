namespace Librainian.Measurement.Frequency {
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Maths;
    using NUnit.Framework;
    using Time;

	/// <summary>
	///     
	/// </summary>
	/// <seealso cref="http://en.wikipedia.org/wiki/Frame_rate"/>
	[DataContract( IsReference = true )]
	// ReSharper disable once UseNameofExpression
	[DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct FPS {

        /// <summary>
        ///     Fifteen <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS Fifteen = new FPS( 15 );

        /// <summary>
        ///     59.9 <see cref="FPS" />.
        /// </summary>
        public static readonly FPS FiftyNinePointNine = new FPS( 59.9 );

        /// <summary>
        ///     Five <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS Five = new FPS( 5 );

        /// <summary>
        ///     Five Hundred <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS FiveHundred = new FPS( 500 );

        /// <summary>
        ///     111.1 Hertz <see cref="FPS" />.
        /// </summary>
        public static readonly FPS Hertz111 = new FPS( 111.1 );

        /// <summary>
        ///     One <see cref="FPS" />.
        /// </summary>
        public static readonly FPS One = new FPS( 1 );

        /// <summary>
        ///     120 <see cref="FPS" />.
        /// </summary>
        public static readonly FPS OneHundredTwenty = new FPS( 120 );

        /// <summary>
        ///     One Thousand Nine <see cref="FPS" /> (Prime).
        /// </summary>
        public static readonly FPS OneThousandNine = new FPS( 1009 );

        /// <summary>
        ///     Sixty <see cref="FPS" />.
        /// </summary>
        public static readonly FPS Sixty = new FPS( 60 );

        /// <summary>
        ///     Ten <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS Ten = new FPS( 10 );

        /// <summary>
        ///     Three <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS Three = new FPS( 3 );

        /// <summary>
        ///     Three Three Three <see cref="FPS" />.
        /// </summary>
        public static readonly FPS ThreeHundredThirtyThree = new FPS( 333 );

        /// <summary>
        ///     Two <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS Two = new FPS( 2 ); 
        
        /// <summary>
        ///     Two.Five <see cref="FPS" />s.
        /// </summary>
        public static readonly FPS TwoPointFive = new FPS( 2.5 );

        /// <summary>
        ///     Two Hundred <see cref="FPS" />.
        /// </summary>
        public static readonly FPS TwoHundred = new FPS( 200 ); //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Hundred Eleven <see cref="FPS" /> (Prime).
        /// </summary>
        public static readonly FPS TwoHundredEleven = new FPS( 211 ); //faster WPM than a female (~240wpm)

        /// <summary>
        ///     Two Thousand Three <see cref="FPS" /> (Prime).
        /// </summary>
        public static readonly FPS TwoThousandThree = new FPS( 2003 );

        /// <summary>
        ///     One <see cref="FPS" />.
        /// </summary>
        public static readonly FPS Zero = new FPS( 0 );

        [DataMember]
        private readonly decimal _value;

        [Test]
        public void TestFPS() {
            Assert.That( One < Two );
            Assert.That( Ten > One );
        }

        public FPS( Decimal fps ) {
            if ( fps <= 0m.Epsilon() ) {
                this._value = 0m.Epsilon();
            }
            else {
                this._value = fps >= Decimal.MaxValue ? Decimal.MaxValue : fps;
            }
        }

        /// <summary>
        /// Frames per second.
        /// </summary>
        /// <param name="fps"></param>
        public FPS( UInt64 fps ) : this( ( Decimal )fps ) {
        }

        /// <summary>
        /// Frames per second.
        /// </summary>
        /// <param name="fps"></param>
        public FPS( Double fps ) : this( ( Decimal )fps ) {
        }

        public Decimal Value => this._value;

        [UsedImplicitly]
        private String DebuggerDisplay => String.Format( "{0} FPS ({1})", this.Value, ( ( TimeSpan )this ).Simpler() );

        public static implicit operator TimeSpan( FPS fps ) => TimeSpan.FromSeconds( ( Double )( 1.0m / fps.Value ) );

        public static implicit operator Span( FPS fps ) => new Seconds( 1.0m / fps.Value );

        public static Boolean operator <( FPS lhs, FPS rhs ) => lhs.Value.CompareTo( rhs.Value ) < 0;

        public static Boolean operator >( FPS lhs, FPS rhs ) => lhs.Value.CompareTo( rhs.Value ) > 0;
    }
}