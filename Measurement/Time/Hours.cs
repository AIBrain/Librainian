// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Hours.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Hours.cs" was last formatted by Protiguous on 2018/06/04 at 4:14 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Numerics;
	using Parsing;

	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public struct Hours : IComparable<Hours>, IQuantityOfTime {

		[JsonProperty]
		public BigRational Value { get; }

		/// <summary>
		///     24
		/// </summary>
		public const Byte InOneDay = 24;

		/// <summary>
		///     Eight <see cref="Hours" /> .
		/// </summary>
		public static readonly Hours Eight = new Hours( 8 );

		/// <summary>
		///     One <see cref="Hours" /> .
		/// </summary>
		public static readonly Hours One = new Hours( 1 );

		/// <summary>
		/// </summary>
		public static readonly Hours Ten = new Hours( 10 );

		/// <summary>
		/// </summary>
		public static readonly Hours Thousand = new Hours( 1000 );

		/// <summary>
		///     Zero <see cref="Hours" />
		/// </summary>
		public static readonly Hours Zero = new Hours( 0 );

		/// <summary>
		///     730 <see cref="Hours" /> in one month, according to WolframAlpha.
		/// </summary>
		/// <see cref="http://www.wolframalpha.com/input/?i=converts+1+month+to+hours" />
		public static BigInteger InOneMonth = 730;

		public Hours( Decimal value ) => this.Value = value;

		public Hours( BigRational value ) => this.Value = value;

		public Hours( Int64 value ) => this.Value = value;

		public Hours( BigInteger value ) => this.Value = value;

		public static Hours Combine( Hours left, Hours right ) => Combine( left, right.Value );

		public static Hours Combine( Hours left, BigRational hours ) => new Hours( left.Value + hours );

		public static Hours Combine( Hours left, BigInteger hours ) => new Hours( ( BigInteger ) left.Value + hours );

		/// <summary>
		///     <para>static equality test</para>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Hours left, Hours right ) => left.Value == right.Value;

		/// <summary>
		///     Implicitly convert the number of <paramref name="hours" /> to <see cref="Days" />.
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static implicit operator Days( Hours hours ) => hours.ToDays();

		/// <summary>
		///     Implicitly convert the number of <paramref name="hours" /> to <see cref="Minutes" />.
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static implicit operator Minutes( Hours hours ) => hours.ToMinutes();

		[NotNull]
		public static implicit operator SpanOfTime( Hours hours ) => new SpanOfTime( hours );

		public static implicit operator TimeSpan( Hours hours ) => TimeSpan.FromHours( ( Double ) hours.Value );

		public static Hours operator -( Hours hours ) => new Hours( hours.Value * -1 );

		public static Hours operator -( Hours left, Hours right ) => Combine( left: left, right: -right );

		public static Hours operator -( Hours left, Decimal hours ) => Combine( left, -hours );

		public static Boolean operator !=( Hours left, Hours right ) => !Equals( left, right );

		public static Hours operator +( Hours left, Hours right ) => Combine( left, right );

		public static Hours operator +( Hours left, Decimal hours ) => Combine( left, hours );

		public static Hours operator +( Hours left, BigInteger hours ) => Combine( left, hours );

		public static Boolean operator <( Hours left, Hours right ) => left.Value < right.Value;

		public static Boolean operator <( Hours left, Minutes right ) => left < ( Hours ) right;

		public static Boolean operator ==( Hours left, Hours right ) => Equals( left, right );

		public static Boolean operator >( Hours left, Minutes right ) => left > ( Hours ) right;

		public static Boolean operator >( Hours left, Hours right ) => left.Value > right.Value;

		public Int32 CompareTo( Hours other ) => this.Value.CompareTo( other.Value );

		public Boolean Equals( Hours other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) {
			if ( obj is null ) { return false; }

			return obj is Hours hours && this.Equals( hours );
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		[Pure]
		public Days ToDays() => new Days( this.Value / InOneDay );

		[Pure]
		public Minutes ToMinutes() => new Minutes( this.Value * Minutes.InOneHour );

		[Pure]
		public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneHour * this.Value );

		[Pure]
		public override String ToString() {
			if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
				var whole = this.Value.GetWholePart();

				return $"{whole} {whole.PluralOf( "hour" )}";
			}

			var dec = ( Decimal ) this.Value;

			return $"{dec} {dec.PluralOf( "hour" )}";
		}

	}

}