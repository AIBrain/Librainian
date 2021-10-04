// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Zeptoseconds.cs" last formatted on 2021-02-03 at 3:36 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using ExtendedNumerics;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	/// </summary>
	/// <see cref="http://wikipedia.org/wiki/Zeptosecond" />
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public record Zeptoseconds( BigDecimal Value ) : IQuantityOfTime, IComparable<Zeptoseconds> {

		/// <summary>
		///     1000
		/// </summary>
		public const UInt16 InOneAttosecond = 1000;

		/// <summary>
		///     <see cref="Five" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Five = new( 5 );

		/// <summary>
		///     <see cref="One" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds One = new( 1 );

		/// <summary>
		///     <see cref="Seven" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Seven = new( 7 );

		/// <summary>
		///     <see cref="Ten" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Ten = new( 10 );

		/// <summary>
		///     <see cref="Thirteen" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Thirteen = new( 13 );

		/// <summary>
		///     <see cref="Thirty" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Thirty = new( 30 );

		/// <summary>
		///     <see cref="Three" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Three = new( 3 );

		/// <summary>
		///     <see cref="Two" /><see cref="Zeptoseconds" />.
		/// </summary>
		public static readonly Zeptoseconds Two = new( 2 );

		/// <summary>
		/// </summary>
		public static readonly Zeptoseconds Zero = new( 0 );

		public Zeptoseconds( Int32 value ) : this( ( BigDecimal )value ) { }
		public Zeptoseconds( Int64 value ) : this( ( BigInteger )value ) { }

		public Zeptoseconds( BigInteger value ) : this( ( BigDecimal )value ) { }

		public static BigDecimal InOneSecond { get; } = new( 10E21 );

		public Int32 CompareTo( Zeptoseconds? other ) {
			if ( other is null ) {
				return SortOrder.Before;
			}

			if ( ReferenceEquals( this, other ) ) {
				return SortOrder.Same;
			}

			return this.Value.CompareTo( other.Value );
		}

		/// <summary>
		/// Return this value in <see cref="Yoctoseconds"/>.
		/// </summary>
		public IQuantityOfTime ToFinerGranularity() => this.ToYoctoseconds();

		public PlanckTimes ToPlanckTimes() => new( this.Value * PlanckTimes.InOneZeptosecond );

		public Seconds ToSeconds() => new( this.Value * InOneSecond );

		/// <summary>
		/// Return this value in <see cref="Attoseconds"/>.
		/// </summary>
		public IQuantityOfTime ToCoarserGranularity() => this.ToAttoseconds();

		TimeSpan IQuantityOfTime.ToTimeSpan() => this.ToSeconds();

		public static Zeptoseconds Combine( Zeptoseconds left, Zeptoseconds right ) => Combine( left, right.Value );

		public static Zeptoseconds Combine( Zeptoseconds left, BigDecimal zeptoseconds ) => new( left.Value + zeptoseconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		public static Boolean Equals( Zeptoseconds left, Zeptoseconds right ) => left.Value == right.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Milliseconds" />.
		/// </summary>
		/// <param name="zeptoseconds"></param>
		public static implicit operator Attoseconds( Zeptoseconds zeptoseconds ) => zeptoseconds.ToAttoseconds();

		public static implicit operator SpanOfTime( Zeptoseconds zeptoseconds ) => new( zeptoseconds );

		public static implicit operator TimeSpan( Zeptoseconds zeptoseconds ) => TimeSpan.FromSeconds( ( Double )zeptoseconds.Value );

		/// <summary>
		///     Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Yoctoseconds" />.
		/// </summary>
		/// <param name="zeptoseconds"></param>
		public static implicit operator Yoctoseconds( Zeptoseconds zeptoseconds ) => zeptoseconds.ToYoctoseconds();

		public static Zeptoseconds operator -( Zeptoseconds zeptoseconds ) => new( zeptoseconds.Value * -1 );

		public static Zeptoseconds operator -( Zeptoseconds left, Zeptoseconds right ) => Combine( left, -right );

		public static Zeptoseconds operator -( Zeptoseconds left, BigDecimal zeptoseconds ) => Combine( left, -zeptoseconds );

		public static Zeptoseconds operator +( Zeptoseconds left, Zeptoseconds right ) => Combine( left, right );

		public static Zeptoseconds operator +( Zeptoseconds left, BigDecimal zeptoseconds ) => Combine( left, zeptoseconds );

		public static Boolean operator <( Zeptoseconds left, Zeptoseconds right ) => left.Value < right.Value;

		public static Boolean operator <( Zeptoseconds left, Yoctoseconds right ) => left < ( Zeptoseconds )right;

		public static Boolean operator >( Zeptoseconds left, Yoctoseconds right ) => left > ( Zeptoseconds )right;

		public static Boolean operator >( Zeptoseconds left, Zeptoseconds right ) => left.Value > right.Value;

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		/// <summary>
		///     <para>Convert to a larger unit.</para>
		/// </summary>
		public Attoseconds ToAttoseconds() => new( this.Value / InOneAttosecond );

		public override String ToString() => $"{this.Value:N}";

		public TimeSpan? ToTimeSpan() => this.ToSeconds();

		/// <summary>
		///     <para>Convert to a smaller unit.</para>
		/// </summary>
		public Yoctoseconds ToYoctoseconds() => new( this.Value * Yoctoseconds.InOneZeptosecond );
	}
}