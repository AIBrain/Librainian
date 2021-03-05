// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

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
	public record Attoseconds( Rational Value ) : IQuantityOfTime {

		/// <summary>1000</summary>
		/// <see cref="Femtoseconds" />
		public const UInt16 InOneFemtosecond = 1000;

		public Attoseconds( Int64 value ) : this( ( Rational )value ) { }
		public Attoseconds( UInt64 value ) : this( ( Rational )value ) { }

		public Attoseconds( BigInteger value ) : this( ( Rational )value ) { }

		/// <summary>Ten <see cref="Attoseconds" /> s.</summary>
		public static Attoseconds Fifteen { get; } = new( 15 );

		/// <summary>Five <see cref="Attoseconds" /> s.</summary>
		public static Attoseconds Five { get; } = new( 5 );

		/// <summary>Five Hundred <see cref="Attoseconds" /> s.</summary>
		public static Attoseconds FiveHundred { get; } = new( 500 );

		/// <summary>111. 1 Hertz <see cref="Attoseconds" />.</summary>
		public static Attoseconds Hertz111 { get; } = new( 9 );

		/// <summary>One <see cref="Attoseconds" />.</summary>
		/// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
		public static Attoseconds One { get; } = new( 1 );

		/// <summary><see cref="OneHundred" /><see cref="Attoseconds" />.</summary>
		/// <remarks>fastest ever view of molecular motion</remarks>
		public static Attoseconds OneHundred { get; } = new( 100 );

		/// <summary>One Thousand Nine <see cref="Attoseconds" /> (Prime).</summary>
		public static Attoseconds OneThousandNine { get; } = new( 1009 );

		/// <summary>Sixteen <see cref="Attoseconds" />.</summary>
		public static Attoseconds Sixteen { get; } = new( 16 );

		/// <summary><see cref="SixtySeven" /><see cref="Attoseconds" />.</summary>
		/// <remarks>the shortest pulses of laser light yet created</remarks>
		public static Attoseconds SixtySeven { get; } = new( 67 );

		/// <summary>Ten <see cref="Attoseconds" /> s.</summary>
		public static Attoseconds Ten { get; } = new( 10 );

		/// <summary>Three <see cref="Attoseconds" /> s.</summary>
		public static Attoseconds Three { get; } = new( 3 );

		/// <summary>Three Three Three <see cref="Attoseconds" />.</summary>
		public static Attoseconds ThreeHundredThirtyThree { get; } = new( 333 );

		/// <summary><see cref="ThreeHundredTwenty" /><see cref="Attoseconds" />.</summary>
		/// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
		public static Attoseconds ThreeHundredTwenty { get; } = new( 320 );

		/// <summary><see cref="Twelve" /><see cref="Attoseconds" />.</summary>
		/// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
		public static Attoseconds Twelve { get; } = new( 12 );

		/// <summary><see cref="TwentyFour" /><see cref="Attoseconds" />.</summary>
		/// <remarks>the atomic unit of time</remarks>
		public static Attoseconds TwentyFour { get; } = new( 24 );

		/// <summary>Two <see cref="Attoseconds" /> s.</summary>
		public static Attoseconds Two { get; } = new( 2 );

		/// <summary><see cref="TwoHundred" /><see cref="Attoseconds" />.</summary>
		/// <remarks>
		///     (approximately) – half-life of beryllium-8, maximum time available for the triple-alpha process for the
		///     synthesis of carbon and heavier elements in stars
		/// </remarks>
		public static Attoseconds TwoHundred { get; } = new( 200 );

		/// <summary>Two Hundred Eleven <see cref="Attoseconds" /> (Prime).</summary>
		public static Attoseconds TwoHundredEleven { get; } = new( 211 );

		/// <summary>Two Thousand Three <see cref="Attoseconds" /> (Prime).</summary>
		public static Attoseconds TwoThousandThree { get; } = new( 2003 );

		/// <summary>Zero <see cref="Attoseconds" />.</summary>
		public static Attoseconds Zero { get; } = new( 0 );

		

		public IQuantityOfTime ToFinerGranularity() => this.ToZeptoseconds();

		public PlanckTimes ToPlanckTimes() => new( this.Value * ( Rational )PlanckTimes.InOneAttosecond);

		public Seconds ToSeconds() => new( ( Rational )this.ToTimeSpan().TotalSeconds );

		public IQuantityOfTime ToCoarserGranularity() => this.ToFemtoseconds();

		public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds( ( Double )this.ToSeconds().Value );

		public static Attoseconds Combine( Attoseconds left, Attoseconds right ) => new( left.Value + right.Value );

		public static Attoseconds Combine( Attoseconds left, Decimal attoseconds ) => new( left.Value + ( Rational )attoseconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Attoseconds left, Attoseconds right ) => left.Value == right.Value;

		public static implicit operator Femtoseconds( Attoseconds attoseconds ) => attoseconds.ToFemtoseconds();
		public static implicit operator SpanOfTime( Attoseconds attoseconds ) => new( attoseconds.ToPlanckTimes().Value );
		public static implicit operator Zeptoseconds( Attoseconds attoseconds ) => attoseconds.ToZeptoseconds();
		public static Attoseconds operator -( Attoseconds left, Decimal attoseconds ) => Combine( left, -attoseconds );
		public static Attoseconds operator +( Attoseconds left, Attoseconds right ) => Combine( left, right );

		public static Attoseconds operator +( Attoseconds left, Decimal attoseconds ) => Combine( left, attoseconds );

		public static Boolean operator <( Attoseconds left, Attoseconds right ) => left.Value < right.Value;

		public static Boolean operator >( Attoseconds left, Attoseconds right ) => left.Value > right.Value;

		public Int32 CompareTo( Attoseconds other ) => this.Value.CompareTo( other.Value );

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
		public Int32 CompareTo( [NotNull] IQuantityOfTime other ) {
			if ( other is null ) {
				throw new ArgumentNullException( nameof( other ) );
			}

			return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );
		}

		/// <summary>
		///     Compares the current instance with another object of the same type and returns an integer that indicates whether
		///     the current instance precedes, follows, or occurs in the
		///     same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance. </param>
		/// <returns>
		///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
		///     Meaning Less than zero This instance precedes
		///     <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as
		///     <paramref name="obj" />. Greater than zero This instance follows
		///     <paramref name="obj" /> in the sort order.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="obj" /> is not the same type as this instance.</exception>
		public Int32 CompareTo( [CanBeNull] Object? obj ) {
			if ( obj is null ) {
				return 1;
			}

			return obj is Attoseconds other ? this.CompareTo( other ) : throw new ArgumentException( $"Object must be of type {nameof( Attoseconds )}" );
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		/// <summary>Convert to a larger unit.</summary>
		/// <returns></returns>
		public Femtoseconds ToFemtoseconds() => new( this.Value / InOneFemtosecond );

		public override String ToString() {
			if ( this.Value > MathConstants.MaxiumDecimalValue ) {
				var whole = this.Value.WholePart;

				return $"{whole} {whole.PluralOf( "as" )}";
			}

			var dec = this.Value;

			return $"{dec} {dec.PluralOf( "as" )}";
		}

		/// <summary>Convert to a smaller unit.</summary>
		/// <returns></returns>
		public Zeptoseconds ToZeptoseconds() => new( this.Value * Zeptoseconds.InOneAttosecond );

	}

}