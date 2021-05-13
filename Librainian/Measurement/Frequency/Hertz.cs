// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Hertz.cs" last formatted on 2020-08-14 at 8:37 PM.

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
	public record Hertz {

		/// <summary>Fifteen <see cref="Hertz" /> s.</summary>
		public static readonly Hertz Fifteen = new( 15 );

		/// <summary>59. 9 <see cref="Hertz" />.</summary>
		public static readonly Hertz FiftyNinePointNine = new( 59.9 );

		/// <summary>Five <see cref="Hertz" /> s.</summary>
		public static readonly Hertz Five = new( 5 );

		/// <summary>Five Hundred <see cref="Hertz" /> s.</summary>
		public static readonly Hertz FiveHundred = new( 500 );

		/// <summary>111. 1 Hertz <see cref="Hertz" />.</summary>
		public static readonly Hertz Hertz111 = new( 111.1 );

		/// <summary>One <see cref="Hertz" />.</summary>
		public static readonly Hertz One = new( 1 );

		/// <summary>120 <see cref="Hertz" />.</summary>
		public static readonly Hertz OneHundredTwenty = new( 120 );

		/// <summary>One Thousand Nine <see cref="Hertz" /> (Prime).</summary>
		public static readonly Hertz OneThousandNine = new( 1009 );

		/// <summary>Sixty <see cref="Hertz" />.</summary>
		public static readonly Hertz Sixty = new( 60 );

		/// <summary>Ten <see cref="Hertz" /> s.</summary>
		public static readonly Hertz Ten = new( 10 );

		/// <summary>Three <see cref="Hertz" /> s.</summary>
		public static readonly Hertz Three = new( 3 );

		/// <summary>Three Three Three <see cref="Hertz" />.</summary>
		public static readonly Hertz ThreeHundredThirtyThree = new( 333 );

		/// <summary>Two <see cref="Hertz" /> s.</summary>
		public static readonly Hertz Two = new( 2 );

		/// <summary>Two Hundred <see cref="Hertz" />.</summary>
		public static readonly Hertz TwoHundred = new( 200 );

		/// <summary>211 <see cref="Hertz" /> (Prime).</summary>
		public static readonly Hertz TwoHundredEleven = new( 211 );

		/// <summary>Two.Five <see cref="Hertz" /> s.</summary>
		public static readonly Hertz TwoPointFive = new( 2.5 );

		/// <summary>Two Thousand Three <see cref="Hertz" /> (Prime).</summary>
		public static readonly Hertz TwoThousandThree = new( 2003 );

		//faster WPM than a female (~240wpm)
		/// <summary>One <see cref="Hertz" />.</summary>
		public static readonly Hertz Zero = new( 0 );

		public Hertz( Decimal frequency ) {
			if ( frequency <= 0m.Epsilon() ) {
				this.Value = 0m.Epsilon();
			}
			else {
				this.Value = frequency >= Decimal.MaxValue ? Decimal.MaxValue : frequency;
			}
		}

		public Hertz( UInt64 frequency ) : this( ( Decimal )frequency ) { }

		public Hertz( Double frequency ) : this( ( Decimal )frequency ) { }

		[JsonProperty]
		public Decimal Value { get; init; }

		public static implicit operator SpanOfTime( [NotNull] Hertz hertz ) => new Seconds( 1 / hertz.Value );

		public static implicit operator TimeSpan( [NotNull] Hertz hertz ) => TimeSpan.FromSeconds( ( Double )( 1 / hertz.Value ) );

		public static Boolean operator <( [NotNull] Hertz left, [NotNull] Hertz right ) => left.Value.CompareTo( right.Value ) < 0;

		public static Boolean operator >( [NotNull] Hertz left, [NotNull] Hertz right ) => left.Value.CompareTo( right.Value ) > 0;

		[NotNull]
		public override String ToString() => $"{this.Value} hertz ({( ( TimeSpan )this ).Simpler()})";
	}
}