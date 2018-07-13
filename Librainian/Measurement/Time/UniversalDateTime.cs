// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "UniversalDateTime.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "UniversalDateTime.cs" was last formatted by Protiguous on 2018/07/13 at 1:31 AM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Absolute universal date and time.</para>
	///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
	/// </summary>
	/// <seealso cref="http://wikipedia.org/wiki/Lol" />
	[Immutable]
	[JsonObject]
	[DebuggerDisplay( "ToString()" )]
	public struct UniversalDateTime : IComparable<UniversalDateTime> {

		public static UniversalDateTime Now => new UniversalDateTime( DateTime.UtcNow );

		/// <summary>
		///     <para>1 planck times</para>
		/// </summary>
		public static UniversalDateTime One { get; } = new UniversalDateTime( BigInteger.One );

		/// <summary>
		///     <para>The value of this constant is equivalent to 00:00:00.0000000, January 1, 0001.</para>
		///     <para>430,000,000,000,000,000 seconds</para>
		/// </summary>
		public static PlanckTimes PlancksUpToMinDateTime { get; } = new PlanckTimes( new Seconds( 4.3E17m ) );

		/// <summary>
		///     <para>0 planck times</para>
		/// </summary>
		public static UniversalDateTime TheBeginning { get; } = new UniversalDateTime( BigInteger.Zero );

		public static UniversalDateTime Unix { get; } = new UniversalDateTime( Epochs.Unix );

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Date Date { get; }

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Time Time { get; }

		/// <summary>
		///     <para><see cref="PlanckTimes" /> since the big bang of <i>this</i> universe.</para>
		/// </summary>
		[JsonProperty]
		public BigInteger Value { get; }

		public UniversalDateTime( BigInteger planckTimesSinceBigBang ) {
			this.Value = planckTimesSinceBigBang;
			var span = new SpanOfTime( planckTimes: this.Value );

			//TODO
			this.Date = new Date( span );
			this.Time = new Time( span );
		}

		public UniversalDateTime( DateTime dateTime ) {
			var span = CalcSpanSince( dateTime );

			this.Value = span.TotalPlanckTimes.Value;
			this.Date = new Date( span ); //we can use span here because the values have been normalized. Should()Have()Been()?
			this.Time = new Time( span );

			//this.Time = new Time();
		}

		private static UniversalDateTime Combine( UniversalDateTime left, BigInteger value ) => new UniversalDateTime( left.Value + value );

		private static UniversalDateTime Combine( UniversalDateTime left, UniversalDateTime right ) => Combine( left, right.Value );

		/// <summary>
		///     Given a <see cref="DateTime" />, calculate the <see cref="SpanOfTime" />.
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		[NotNull]
		public static SpanOfTime CalcSpanSince( DateTime dateTime ) {
			var sinceThen = new SpanOfTime( dateTime - DateTime.MinValue );
			var plancksSinceThen = sinceThen.TotalPlanckTimes;
			var span = new SpanOfTime( planckTimes: PlancksUpToMinDateTime.Value + plancksSinceThen.Value );

			return span;
		}

		public static UniversalDateTime operator -( UniversalDateTime left, UniversalDateTime right ) => Combine( left, -right );

		public static UniversalDateTime operator -( UniversalDateTime universalDateTime ) => new UniversalDateTime( universalDateTime.Value * -1 );

		public static Boolean operator <( UniversalDateTime left, UniversalDateTime right ) => left.Value < right.Value;

		public static Boolean operator >( UniversalDateTime left, UniversalDateTime right ) => left.Value > right.Value;

		public Int32 CompareTo( UniversalDateTime other ) => this.Value.CompareTo( other.Value );

		/// <summary>
		///     Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		public override String ToString() => this.Value.ToString();
	}
}