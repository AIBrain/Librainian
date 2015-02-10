#region License & Information

// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian 2015/Attoseconds.cs" was last cleaned by Rick on 2015/01/05 at 7:23 PM
#endregion License & Information

namespace Librainian.Measurement.Time {
	using System;
	using System.Diagnostics;
	using System.Numerics;
	using System.Runtime.Serialization;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Librainian.Extensions;
	using Maths;
	using NUnit.Framework;

	[DataContract(IsReference = true)]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[Serializable]
	[Immutable]
	public struct Attoseconds : IComparable<Attoseconds>, IQuantityOfTime {

		/// <summary>
		/// 1000
		/// </summary>
		/// <seealso cref="Femtoseconds"/>
		public const UInt16 InOneFemtosecond = 1000;

		/// <summary>
		/// Ten <see cref="Attoseconds"/> s.
		/// </summary>
		public static readonly Attoseconds Fifteen = new Attoseconds( 15 );

		/// <summary>
		/// Five <see cref="Attoseconds"/> s.
		/// </summary>
		public static readonly Attoseconds Five = new Attoseconds( 5 );

		/// <summary>
		/// Five Hundred <see cref="Attoseconds"/> s.
		/// </summary>
		public static readonly Attoseconds FiveHundred = new Attoseconds( 500 );

		/// <summary>
		/// 111. 1 Hertz <see cref="Attoseconds"/>.
		/// </summary>
		public static readonly Attoseconds Hertz111 = new Attoseconds( 9 );

		/// <summary>
		/// One <see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
		public static readonly Attoseconds One = new Attoseconds( 1 );

		/// <summary>
		/// <see cref="OneHundred"/><see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>fastest ever view of molecular motion</remarks>
		public static readonly Attoseconds OneHundred = new Attoseconds( 100 );

		/// <summary>
		/// One Thousand Nine <see cref="Attoseconds"/> (Prime).
		/// </summary>
		public static readonly Attoseconds OneThousandNine = new Attoseconds( 1009 );

		/// <summary>
		/// Sixteen <see cref="Attoseconds"/>.
		/// </summary>
		public static readonly Attoseconds Sixteen = new Attoseconds( 16 );

		/// <summary>
		/// <see cref="SixtySeven"/><see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>the shortest pulses of laser light yet created</remarks>
		public static readonly Attoseconds SixtySeven = new Attoseconds( 67 );

		/// <summary>
		/// Ten <see cref="Attoseconds"/> s.
		/// </summary>
		public static readonly Attoseconds Ten = new Attoseconds( 10 );

		/// <summary>
		/// Three <see cref="Attoseconds"/> s.
		/// </summary>
		public static readonly Attoseconds Three = new Attoseconds( 3 );

		/// <summary>
		/// Three Three Three <see cref="Attoseconds"/>.
		/// </summary>
		public static readonly Attoseconds ThreeHundredThirtyThree = new Attoseconds( 333 );

		/// <summary>
		/// <see cref="ThreeHundredTwenty"/><see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
		public static readonly Attoseconds ThreeHundredTwenty = new Attoseconds( 320 );

		/// <summary>
		/// <see cref="Twelve"/><see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
		public static readonly Attoseconds Twelve = new Attoseconds( 12 );

		/// <summary>
		/// <see cref="TwentyFour"/><see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>the atomic unit of time</remarks>
		public static readonly Attoseconds TwentyFour = new Attoseconds( 24 );

		/// <summary>
		/// Two <see cref="Attoseconds"/> s.
		/// </summary>
		public static readonly Attoseconds Two = new Attoseconds( 2 );

		/// <summary>
		/// <see cref="TwoHundred"/><see cref="Attoseconds"/>.
		/// </summary>
		/// <remarks>
		/// (approximately) – half-life of beryllium-8, maximum time available for the triple-alpha
		/// process for the synthesis of carbon and heavier elements in stars
		/// </remarks>
		public static readonly Attoseconds TwoHundred = new Attoseconds( 200 );

		/// <summary>
		/// Two Hundred Eleven <see cref="Attoseconds"/> (Prime).
		/// </summary>
		public static readonly Attoseconds TwoHundredEleven = new Attoseconds( 211 );

		/// <summary>
		/// Two Thousand Three <see cref="Attoseconds"/> (Prime).
		/// </summary>
		public static readonly Attoseconds TwoThousandThree = new Attoseconds( 2003 );

		/// <summary>
		/// Zero <see cref="Attoseconds"/>.
		/// </summary>
		public static readonly Attoseconds Zero = new Attoseconds( 0 );

		[DataMember]
		public BigDecimal Value { get; }

		public Attoseconds( Decimal value ) {
			this.Value = value;
		}

		public Attoseconds( BigDecimal value ) {
			this.Value = value;
		}

		public Attoseconds( long value ) {
			this.Value = value;
		}

		public Attoseconds( BigInteger value ) {
			this.Value = value;
		}

		[UsedImplicitly]
		private String DebuggerDisplay => this.ToString();

		public int CompareTo( Attoseconds other ) => this.Value.CompareTo( other.Value );

		public override int GetHashCode() => this.Value.GetHashCode();

		[Pure]
		public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneAttosecond* this.Value ) ;

		[Pure]
		public override String ToString() => String.Format( "{0} as", this.Value );

		public static Attoseconds Combine( Attoseconds left, Attoseconds right ) => new Attoseconds( left.Value + right.Value );

		public static Attoseconds Combine( Attoseconds left, Decimal attoseconds ) => new Attoseconds( left.Value + attoseconds );

		/// <summary>
		/// <para>static equality test</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Attoseconds left, Attoseconds right ) => left.Value == right.Value;

		public static implicit operator Femtoseconds( Attoseconds attoseconds ) => attoseconds.ToFemtoseconds();

		public static implicit operator Zeptoseconds( Attoseconds attoseconds ) => attoseconds.ToZeptoseconds();

		public static implicit operator Span( Attoseconds attoseconds ) {
			var plancks = attoseconds.ToPlanckTimes();
			return new Span( plancks );
		}

		public static Attoseconds operator -( Attoseconds left, Decimal attoseconds ) => Combine( left, -attoseconds );

		public static Boolean operator !=( Attoseconds left, Attoseconds right ) => !Equals( left, right );

		public static Attoseconds operator +( Attoseconds left, Attoseconds right ) => Combine( left, right );

		public static Attoseconds operator +( Attoseconds left, Decimal attoseconds ) => Combine( left, attoseconds );

		public static Boolean operator <( Attoseconds left, Attoseconds right ) => left.Value < right.Value;

		public static Boolean operator ==( Attoseconds left, Attoseconds right ) => Equals( left, right );

		public static Boolean operator >( Attoseconds left, Attoseconds right ) => left.Value > right.Value;

		public Boolean Equals( Attoseconds other ) => Equals( this, other );

		public override Boolean Equals( [CanBeNull] object obj ) {
			if ( ReferenceEquals( null, obj ) ) {
				return false;
			}
			return obj is Attoseconds && this.Equals( ( Attoseconds )obj );
		}

		/// <summary>
		/// Convert to a larger unit.
		/// </summary>
		/// <returns></returns>
		[Pure]
		public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value / InOneFemtosecond );

		/// <summary>
		/// Convert to a smaller unit.
		/// </summary>
		/// <returns></returns>
		public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value * Zeptoseconds.InOneAttosecond );

		[Test]
		public static void Test() {
			Zero.Should().BeLessThan( One );
			One.Should().BeGreaterThan( Zero );
			One.Should().Be( One );
			One.Should().BeGreaterThan( Zeptoseconds.One );
			One.Should().BeLessThan( Femtoseconds.One );
		}
	}
}