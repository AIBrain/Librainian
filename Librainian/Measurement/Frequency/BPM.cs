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
// File "BPM.cs" last formatted on 2020-08-14 at 8:37 PM.

namespace Librainian.Measurement.Frequency {

	using System;
	using Newtonsoft.Json;
	using Time;

	/// <summary>BPM. Beats Per Minute</summary>
	[JsonObject]
	public struct Bpm : IComparable<Bpm> {

		//TODO BPM and WPM

		/// <summary>Ten <see cref="Bpm" /> s.</summary>
		public static readonly Bpm Fifteen = new( 15 );

		/// <summary>Five <see cref="Bpm" /> s.</summary>
		public static readonly Bpm Five = new( 5 );

		/// <summary>Five Hundred <see cref="Bpm" /> s.</summary>
		public static readonly Bpm FiveHundred = new( 500 );

		/// <summary>111. 1 Hertz <see cref="Bpm" />.</summary>
		public static readonly Bpm Hertz111 = new( 9 );

		/// <summary></summary>
		public static readonly Bpm MaxValue = new( UInt64.MaxValue );

		/// <summary>About zero. :P</summary>
		public static readonly Bpm MinValue = new( Decimal.MinValue );

		/// <summary>One <see cref="Bpm" />.</summary>
		public static readonly Bpm One = new( 1 );

		/// <summary>One Thousand Nine <see cref="Bpm" /> (Prime).</summary>
		public static readonly Bpm OneThousandNine = new( 1009 );

		/// <summary>Ten <see cref="Bpm" /> s.</summary>
		public static readonly Bpm Ten = new( 10 );

		/// <summary>Three <see cref="Bpm" /> s.</summary>
		public static readonly Bpm Three = new( 3 );

		/// <summary>Three Three Three <see cref="Bpm" />.</summary>
		public static readonly Bpm ThreeHundredThirtyThree = new( 333 );

		/// <summary>Two <see cref="Bpm" /> s.</summary>
		public static readonly Bpm Two = new( 2 );

		/// <summary>Two Hundred <see cref="Bpm" />.</summary>
		public static readonly Bpm TwoHundred = new( 200 );

		/// <summary>Two Hundred Eleven <see cref="Bpm" /> (Prime).</summary>
		public static readonly Bpm TwoHundredEleven = new( 211 );

		/// <summary>Two Thousand Three <see cref="Bpm" /> (Prime).</summary>
		public static readonly Bpm TwoThousandThree = new( 2003 );

		[JsonProperty]
		public readonly Decimal Value;

		public Bpm( Decimal bpm ) => this.Value = bpm;

		public static implicit operator TimeSpan( Bpm bpm ) => TimeSpan.FromMilliseconds( ( Double )bpm.Value / Seconds.InOneMinute );

		public Int32 CompareTo( Bpm other ) => this.Value.CompareTo( other.Value );

		public override Int32 GetHashCode() => this.Value.GetHashCode();

	}

}