// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FPS.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "FPS.cs" was last formatted by Protiguous on 2018/07/13 at 1:22 AM.

namespace Librainian.Measurement.Frequency {

	using System;
	using System.Diagnostics;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Time;

	/// <summary>
	/// </summary>
	/// <seealso cref="http://en.wikipedia.org/wiki/Frame_rate" />
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public struct Fps {

		[JsonProperty]
		private readonly Decimal _value;

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

		/// <summary>
		///     Two Thousand Three <see cref="Fps" /> (Prime).
		/// </summary>
		public static Fps TwoThousandThree { get; } = new Fps( 2003 );

		//faster WPM than a female (~240wpm)
		/// <summary>
		///     One <see cref="Fps" />.
		/// </summary>
		public static Fps Zero { get; } = new Fps( 0 );

		//faster WPM than a female (~240wpm)
		public Decimal Value => this._value;

		public Fps( Decimal fps ) {
			if ( fps <= 0m.Epsilon() ) { this._value = 0m.Epsilon(); }
			else { this._value = fps >= Decimal.MaxValue ? Decimal.MaxValue : fps; }
		}

		/// <summary>
		///     Frames per second.
		/// </summary>
		/// <param name="fps"></param>
		public Fps( UInt64 fps ) : this( ( Decimal ) fps ) { }

		/// <summary>
		///     Frames per second.
		/// </summary>
		/// <param name="fps"></param>
		public Fps( Double fps ) : this( ( Decimal ) fps ) { }

		[NotNull]
		public static implicit operator SpanOfTime( Fps fps ) => new Seconds( 1.0m / fps.Value );

		public static implicit operator TimeSpan( Fps fps ) => TimeSpan.FromSeconds( ( Double ) ( 1.0m / fps.Value ) );

		public static Boolean operator <( Fps left, Fps rhs ) => left.Value.CompareTo( rhs.Value ) < 0;

		public static Boolean operator >( Fps left, Fps rhs ) => left.Value.CompareTo( rhs.Value ) > 0;

		public override String ToString() => $"{this.Value} FPS ({( ( TimeSpan ) this ).Simpler()})";
	}
}