// Copyright � Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
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
// File "Milliseconds.cs" last touched on 2021-03-07 at 3:06 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Parsing;
	using Rationals;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	[Immutable]
	public record Milliseconds( Rational Value ) : IQuantityOfTime, IComparable<Milliseconds> {

		/// <summary>
		///     1000
		/// </summary>
		public const UInt16 InOneSecond = 1000;

		/// <summary>
		///     Optimization
		/// </summary>
		private readonly Lazy<Rational> _lazyPlancksInOneMillisecond = new(() => new Rational( new BigInteger( PlanckTimes.InOneMillisecond ) ), true);

		/// <summary>
		///     Ten <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds Fifteen { get; } = new(15);

		/// <summary>
		///     Five <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds Five { get; } = new(5);

		/// <summary>
		///     Five Hundred <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds FiveHundred { get; } = new(500);

		/// <summary>
		///     111. 1 Hertz (9 <see cref="Milliseconds" />).
		/// </summary>
		public static Milliseconds Hertz111 { get; } = new(9);

		/// <summary>
		///     97 <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds NinetySeven { get; } = new(97);

		/// <summary>
		///     One <see cref="Milliseconds" />.
		/// </summary>
		public static Milliseconds One { get; } = new(1);

		/// <summary>
		///     One <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds OneHundred { get; } = new(100);

		/// <summary>
		///     One Thousand Nine <see cref="Milliseconds" /> (Prime).
		/// </summary>
		public static Milliseconds OneThousandNine { get; } = new(1009);

		/// <summary>
		///     Sixteen <see cref="Milliseconds" />.
		/// </summary>
		public static Milliseconds Sixteen { get; } = new(16);

		/// <summary>
		///     Ten <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds Ten { get; } = new(10);

		/// <summary>
		///     Three <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds Three { get; } = new(3);

		/// <summary>
		///     Three Three Three <see cref="Milliseconds" />.
		/// </summary>
		public static Milliseconds ThreeHundredThirtyThree { get; } = new(333);

		/// <summary>
		///     Two <see cref="Milliseconds" /> s.
		/// </summary>
		public static Milliseconds Two { get; } = new(2);

		/// <summary>
		///     Two Hundred <see cref="Milliseconds" />.
		/// </summary>
		public static Milliseconds TwoHundred { get; } = new(200);

		/// <summary>
		///     Two Hundred Eleven <see cref="Milliseconds" /> (Prime).
		/// </summary>
		public static Milliseconds TwoHundredEleven { get; } = new(211);

		/// <summary>
		///     Two Thousand Three <see cref="Milliseconds" /> (Prime).
		/// </summary>
		public static Milliseconds TwoThousandThree { get; } = new(2003);

		//faster WPM than a female (~240wpm)
		/// <summary>
		///     Zero <see cref="Milliseconds" />.
		/// </summary>
		public static Milliseconds Zero { get; } = new(0);

		public Int32 CompareTo( [NotNull] Milliseconds? other ) {
			if ( other == null ) {
				throw new ArgumentNullException( nameof( other ) );
			}

			return this.Value.CompareTo( other.Value );
		}

		public IQuantityOfTime ToFinerGranularity() => this.ToMicroseconds();

		public PlanckTimes ToPlanckTimes() => new(( this.Value * this._lazyPlancksInOneMillisecond.Value ).WholePart);

		public Seconds ToSeconds() => new(this.Value / InOneSecond);

		public IQuantityOfTime ToCoarserGranularity() => this.ToSeconds();

		public TimeSpan ToTimeSpan() => this;

		public static Milliseconds Combine( Milliseconds left, Rational milliseconds ) => new(left.Value + milliseconds);

		public static Milliseconds Combine( Milliseconds left, BigInteger milliseconds ) => new(left.Value + milliseconds);

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Milliseconds left, Milliseconds right ) => left.Value == right.Value;

		public static explicit operator Double( Milliseconds milliseconds ) => ( Double )milliseconds.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="milliseconds" /> to <see cref="Microseconds" />.
		/// </summary>
		/// <param name="milliseconds"></param>
		/// <returns></returns>
		public static implicit operator Microseconds( Milliseconds milliseconds ) => milliseconds.ToMicroseconds();

		public static implicit operator Rational( Milliseconds milliseconds ) => milliseconds.Value;

		public static implicit operator Seconds( Milliseconds milliseconds ) => milliseconds.ToSeconds();

		public static implicit operator SpanOfTime( Milliseconds milliseconds ) => new(milliseconds);

		public static implicit operator TimeSpan( Milliseconds milliseconds ) => TimeSpan.FromMilliseconds( ( Double )milliseconds.Value );

		public static Milliseconds operator -( Milliseconds milliseconds ) => new(milliseconds.Value * -1);

		public static Milliseconds operator -( Milliseconds left, Milliseconds right ) => Combine( left, -right.Value );

		public static Milliseconds operator -( Milliseconds left, Decimal milliseconds ) => Combine( left, ( Rational )( -milliseconds ) );

		public static Milliseconds operator +( Milliseconds left, Milliseconds right ) => Combine( left, right.Value );

		public static Milliseconds operator +( Milliseconds left, Decimal milliseconds ) => Combine( left, ( Rational )milliseconds );

		public static Milliseconds operator +( Milliseconds left, BigInteger milliseconds ) => Combine( left, milliseconds );

		public static Boolean operator <( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) => left.Value < right.Value;

		public static Boolean operator <( [NotNull] Milliseconds left, [NotNull] Seconds right ) => ( Seconds )left < right;

		public static Boolean operator >( [NotNull] Milliseconds left, [NotNull] Milliseconds right ) => left.Value > right.Value;

		[Pure]
		public static Boolean operator >( [NotNull] Milliseconds left, [NotNull] Seconds right ) => ( Seconds )left > right;

		public Microseconds ToMicroseconds() => new(this.Value * Microseconds.InOneMillisecond);

		public override String ToString() {
			//TODO clean this up.
			if ( this.Value > MathConstants.MaxiumDecimalValue ) {
				var whole = this.Value.WholePart;

				return $"{whole} {whole.PluralOf( "millisecond" )}";
			}

			var dec = ( Decimal )this.Value;

			return $"{dec} {dec.PluralOf( "millisecond" )}";
		}

	}

}