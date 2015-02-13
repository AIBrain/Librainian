#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Seconds.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

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
	using Parsing;

	/// <summary>
	///     <para>
	///         Under the International System of Units, since 1967 the second has been defined as the duration of 9192631770
	///         periods of the radiation corresponding to the transition between the two hyperfine levels of the ground state
	///         of the caesium 133 atom.
	///     </para>
	/// </summary>
	[DataContract(IsReference = true)]
	// ReSharper disable once UseNameofExpression
	[DebuggerDisplay( "{DebuggerDisplay,nq}" )]
	[Immutable]
	public struct Seconds : IComparable<Seconds>, IQuantityOfTime {
		/// <summary>
		///     31536000
		/// </summary>
		public const UInt32 InOneCommonYear = 31536000;

		/// <summary>
		///     60
		/// </summary>
		public const Byte InOneMinute = 60;

		/// <summary>
		///     3600
		/// </summary>
		public const UInt16 InOneHour = 3600;

		/// <summary>
		/// 86400
		/// </summary>
		public const UInt32 InOneDay = 86400;

		/// <summary>
		///     2635200 (30.5 days)
		/// </summary>
		public const UInt32 InOneMonth = 2635200;

		/// <summary>
		///     604800
		/// </summary>
		public const UInt32 InOneWeek = 604800;

		/// <summary>
		///     <see cref="Five" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Five = new Seconds( 5 );

		/// <summary>
		///     <see cref="One" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds One = new Seconds( 1 );

		/// <summary>
		///     <see cref="Seven" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Seven = new Seconds( 7 );

		/// <summary>
		///     <see cref="Ten" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Ten = new Seconds( 10 );

		/// <summary>
		///     <see cref="Twenty" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Twenty = new Seconds( 20 );

		/// <summary>
		///     <see cref="Thirteen" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Thirteen = new Seconds( 13 );

		/// <summary>
		///     <see cref="Thirty" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Thirty = new Seconds( 30 );

		/// <summary>
		///     <see cref="Three" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Three = new Seconds( 3 );

		/// <summary>
		///     <see cref="Two" /> <see cref="Seconds" />.
		/// </summary>
		public static readonly Seconds Two = new Seconds( 2 );

		/// <summary>
		/// </summary>
		public static readonly Seconds Zero = new Seconds( 0 );

		[DataMember]
		public BigDecimal Value { get; }

		static Seconds() {
			Zero.Should().BeLessThan( One );
			One.Should().BeGreaterThan( Zero );
			One.Should().Be( One );
			One.Should().BeLessThan( Minutes.One );
			One.Should().BeGreaterThan( Milliseconds.One );
		}

		public Seconds( Decimal value )  {
			this.Value = value;
		}

		public Seconds( BigDecimal value )  {
			this.Value = value;
		}

		public Seconds( long value )  {
			this.Value = value;
		}

		public Seconds( BigInteger value )  {
			this.Value = value;
		}

		[UsedImplicitly]
		private String DebuggerDisplay => this.ToString();

		public static Seconds Combine( Seconds left, Seconds right ) => Combine( left, right.Value );

		public static Seconds Combine( Seconds left, BigDecimal seconds ) => new Seconds( left.Value + seconds );

		public static Seconds Combine( Seconds left, BigInteger seconds ) => new Seconds( ( BigInteger )left.Value + seconds );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Seconds left, Seconds right ) => left.Value == right.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="seconds" /> to <see cref="Milliseconds" />.
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public static implicit operator Milliseconds( Seconds seconds ) => seconds.ToMilliseconds();

		/// <summary>
		///     Implicitly convert  the number of <paramref name="seconds" /> to <see cref="Minutes" />.
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public static implicit operator Minutes( Seconds seconds ) => seconds.ToMinutes();

		public static implicit operator Span( Seconds seconds ) => new Span( seconds: seconds );

		public static implicit operator TimeSpan( Seconds seconds ) => TimeSpan.FromSeconds( ( Double )seconds.Value );

		public static Seconds operator -( Seconds seconds ) => new Seconds( seconds.Value * -1 );

		public static Seconds operator -( Seconds left, Seconds right ) => Combine( left: left, right: -right );

		public static Seconds operator -( Seconds left, Decimal seconds ) => Combine( left, -seconds );

		public static Boolean operator !=( Seconds left, Seconds right ) => !Equals( left, right );

		public static Seconds operator +( Seconds left, Seconds right ) => Combine( left, right );

		public static Seconds operator +( Seconds left, Decimal seconds ) => Combine( left, seconds );

		public static Seconds operator +( Seconds left, BigInteger seconds ) => Combine( left, seconds );

		public static Boolean operator <( Seconds left, Seconds right ) => left.Value < right.Value;

		public static Boolean operator <( Seconds left, Milliseconds right ) => left < ( Seconds )right;

		public static Boolean operator <( Seconds left, Minutes right ) => ( Minutes )left < right;

		public static Boolean operator ==( Seconds left, Seconds right ) => Equals( left, right );

		public static Boolean operator >( Seconds left, Minutes right ) => ( Minutes )left > right;

		public static Boolean operator >( Seconds left, Seconds right ) => left.Value > right.Value;

		public static Boolean operator >( Seconds left, Milliseconds right ) => left > ( Seconds )right;

		public int CompareTo( Seconds other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Seconds other ) => Equals( this, other );

		public override Boolean Equals( object obj ) {
			if ( ReferenceEquals( null, obj ) ) {
				return false;
			}
			return obj is Seconds && this.Equals( ( Seconds )obj );
		}

		public override int GetHashCode() => this.Value.GetHashCode();

		[Pure]
		public Milliseconds ToMilliseconds() => new Milliseconds( this.Value * Milliseconds.InOneSecond );

		[Pure]
		public Minutes ToMinutes() => new Minutes( value: this.Value / InOneMinute );

/*
		[Pure]
		public Minutes ToMonths() => new Minutes( value: this.Value / InOneMonth );
*/

		[Pure]
		public PlanckTimes ToPlanckTimes() {
			var seconds = new BigDecimal( this.Value );	//avoid overflow?
			seconds *= PlanckTimes.InOneSecond;
			//return ( BigInteger )seconds; //gets truncated here. oh well.

			return new PlanckTimes( PlanckTimes.InOneSecond * this.Value );
		}

		[ Pure ]
		public Seconds ToSeconds() => this;


		[Pure]
		public override String ToString() => String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "second" ) );

		[Pure]
		public Minutes ToWeeks() => new Minutes( value: this.Value / InOneWeek );

		[Pure]
		public Minutes ToYears() => new Minutes( value: this.Value / InOneCommonYear );
	}
}