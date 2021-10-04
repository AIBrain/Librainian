// Copyright © Protiguous. All Rights Reserved.
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
// File "$FILENAME$" last touched on $CURRENT_YEAR$-$CURRENT_MONTH$-$CURRENT_DAY$ at $CURRENT_TIME$ by Protiguous.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Exceptions;
	using ExtendedNumerics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>
	///         Under the International System of Units, since 1967 the second has been defined as the duration of 9192631770
	///         periods of the radiation corresponding to the transition
	///         between the two hyperfine levels of the ground state of the caesium 133 atom.
	///     </para>
	/// </summary>
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public record Seconds( BigDecimal Value ) : IQuantityOfTime {

		/// <summary>31536000</summary>
		public const Int32 InOneCommonYear = 31536000;

		/// <summary>86400</summary>
		public const Int32 InOneDay = 86400;

		/// <summary>3600</summary>
		public const UInt16 InOneHour = 3600;

		/// <summary>60</summary>
		public const Byte InOneMinute = 60;

		/// <summary>2635200 (30.5 days)</summary>
		public const Int32 InOneMonth = 2635200;

		/// <summary>604800</summary>
		public const Int32 InOneWeek = 604800;

		public Seconds( Double value ) : this( ( BigDecimal )value ) { }

		/// <summary><see cref="Five" /><see cref="Seconds" />.</summary>
		public static Seconds Five { get; } = new( ( BigDecimal )5 );

		/// <summary><see cref="One" /><see cref="Seconds" />.</summary>
		public static Seconds One { get; } = new( ( BigDecimal )1 );

		/// <summary><see cref="Seven" /><see cref="Seconds" />.</summary>
		public static Seconds Seven { get; } = new( ( BigDecimal )7 );

		/// <summary><see cref="Ten" /><see cref="Seconds" />.</summary>
		public static Seconds Ten { get; } = new( ( BigDecimal )10 );

		/// <summary><see cref="Thirteen" /><see cref="Seconds" />.</summary>
		public static Seconds Thirteen { get; } = new( ( BigDecimal )13 );

		/// <summary><see cref="Thirty" /><see cref="Seconds" />.</summary>
		public static Seconds Thirty { get; } = new( ( BigDecimal )30 );

		/// <summary><see cref="Three" /><see cref="Seconds" />.</summary>
		public static Seconds Three { get; } = new( ( BigDecimal )3 );

		/// <summary><see cref="Twenty" /><see cref="Seconds" />.</summary>
		public static Seconds Twenty { get; } = new( ( BigDecimal )20 );

		/// <summary><see cref="Two" /><see cref="Seconds" />.</summary>
		public static Seconds Two { get; } = new( ( BigDecimal )2 );

		public static Seconds Zero { get; } = new( ( BigDecimal )0 );

		public IQuantityOfTime ToFinerGranularity() => this.ToMilliseconds();

		public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneSecond );

		public Seconds ToSeconds() => new( this.Value );

		public IQuantityOfTime ToCoarserGranularity() => this.ToMinutes();

		public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds( ( Double )this.Value );

		/// <summary>
		///     Compares the current instance with another object of the same type and returns an integer that indicates whether
		///     the current instance precedes, follows, or occurs in the
		///     same position in the sort order as the other object.
		/// </summary>
		/// <param name="other">An object to compare with this instance. </param>
		/// <returns>
		///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
		///     Meaning Less than zero This instance precedes
		///     <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as
		///     <paramref name="other" />. Greater than zero This instance
		///     follows <paramref name="other" /> in the sort order.
		/// </returns>
		public Int32 CompareTo( IQuantityOfTime other ) {
			if ( other is null ) {
				throw new ArgumentEmptyException( nameof( other ) );
			}

			return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );
		}

		[Pure]
		public static Seconds Combine( Seconds left, Seconds right ) => Combine( left, right.Value );

		public static Seconds Combine( Seconds left, BigDecimal seconds ) => new( left.Value + seconds );

		public static Seconds Combine( Seconds left, BigInteger seconds ) => new( left.Value + seconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		public static Boolean Equals( Seconds left, Seconds right ) => left.Value.Equals( right.Value );

		/// <summary>Implicitly convert the number of <paramref name="seconds" /> to <see cref="Milliseconds" />.</summary>
		/// <param name="seconds"></param>
		public static implicit operator Milliseconds( Seconds seconds ) => seconds.ToMilliseconds();

		/// <summary>Implicitly convert the number of <paramref name="seconds" /> to <see cref="Minutes" />.</summary>
		/// <param name="seconds"></param>
		public static implicit operator Minutes( Seconds seconds ) => seconds.ToMinutes();

		public static implicit operator SpanOfTime( Seconds seconds ) => new( seconds );

		/// <summary>Returns a <see cref="TimeSpan" /></summary>
		/// <param name="seconds"></param>
		public static implicit operator TimeSpan( Seconds seconds ) => TimeSpan.FromSeconds( ( Double )seconds.Value );

		public static Seconds operator -( Seconds seconds ) => new( seconds.Value * -1 );

		public static Seconds operator -( Seconds left, Seconds right ) => Combine( left, -right );

		public static Seconds operator -( Seconds left, BigDecimal seconds ) => Combine( left, -seconds );

		public static Seconds operator +( Seconds left, Seconds right ) => Combine( left, right );

		public static Seconds operator +( Seconds left, BigDecimal seconds ) => Combine( left, seconds );

		public static Seconds operator +( Seconds left, BigInteger seconds ) => Combine( left, seconds );

		public static Boolean operator <( Seconds left, Seconds right ) => left.Value < right.Value;

		public static Boolean operator <( Seconds left, Milliseconds right ) => left < ( Seconds )right;

		public static Boolean operator <( Seconds left, Minutes right ) => ( Minutes )left < right;

		public static Boolean operator >( Seconds left, Minutes right ) => ( Minutes )left > right;

		public static Boolean operator >( Seconds left, Seconds right ) => left.Value > right.Value;

		public static Boolean operator >( Seconds left, Milliseconds right ) => left > ( Seconds )right;

		public Milliseconds ToMilliseconds() => new( this.Value * Milliseconds.InOneSecond );

		public Minutes ToMinutes() => new( this.Value / InOneMinute );

		public override String ToString() => $"{this.Value} seconds";

		public Weeks ToWeeks() => new( this.Value / InOneWeek );

		public Years ToYears() => new( this.Value / InOneCommonYear );

	}

}