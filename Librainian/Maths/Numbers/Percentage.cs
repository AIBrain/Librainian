// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Percentage.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Percentage.cs" was last formatted by Protiguous on 2018/06/26 at 1:20 AM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Numerics;

	/// <summary>
	///     <para>Restricts the value to between 0.0 and 1.0.</para>
	/// </summary>
	[JsonObject]
	[Immutable]
	public class Percentage : IComparable<Percentage>, IComparable<Double>, IEquatable<Percentage> {

		/// <summary>1</summary>
		public const Double Maximum = 1d;

		/// <summary>0</summary>
		public const Double Minimum = 0d;

		[CanBeNull]
		[JsonProperty]
		public readonly BigInteger? Denominator;

		[CanBeNull]
		[JsonProperty]
		public readonly BigInteger? LeastCommonDenominator;

		[CanBeNull]
		[JsonProperty]
		public readonly BigInteger? Numerator;

		[JsonProperty]
		public readonly BigRational Quotient;

		/// <summary>
		///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
		/// </summary>
		/// <param name="value"></param>
		public Percentage( Double value ) : this( ( BigInteger ) value, BigInteger.One ) { }

		/// <summary>
		///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
		/// </summary>
		/// <param name="numerator"></param>
		/// <param name="denominator"></param>
		public Percentage( Decimal numerator, Decimal denominator ) : this( ( Double ) numerator, ( Double ) denominator ) { }

		/// <summary>
		///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
		/// </summary>
		/// <param name="numerator"></param>
		/// <param name="denominator"></param>
		public Percentage( Double numerator, Double denominator ) {
			if ( Double.IsNaN( numerator ) ) {
				throw new ArgumentOutOfRangeException( nameof( numerator ), "Numerator is not a number." );
			}

			if ( Double.IsNaN( denominator ) ) {
				throw new ArgumentOutOfRangeException( nameof( denominator ), "Denominator is not a number." );
			}

			this.Numerator = new BigInteger( numerator );
			this.Denominator = new BigInteger( denominator );

			this.LeastCommonDenominator = BigRational.LeastCommonDenominator( this.Numerator.Value, this.Denominator.Value );

			this.Quotient = denominator <= 0 ? new BigRational( 0.0 ) : new BigRational( numerator / denominator );

			if ( this.Quotient < Minimum ) {
				this.Quotient = Minimum;
			}
			else if ( this.Quotient > Maximum ) {
				this.Quotient = Maximum;
			}
		}

		/// <summary>
		///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
		/// </summary>
		/// <param name="numerator"></param>
		/// <param name="denominator"></param>
		public Percentage( BigInteger numerator, BigInteger denominator ) {
			this.Numerator = numerator;
			this.Denominator = denominator;
			this.LeastCommonDenominator = BigRational.LeastCommonDenominator( this.Numerator.Value, this.Denominator.Value );

			this.Quotient = denominator == BigInteger.Zero ? new BigRational( 0.0 ) : new BigRational( numerator / denominator );

			if ( this.Quotient < Minimum ) {
				this.Quotient = Minimum;
			}
			else if ( this.Quotient > Maximum ) {
				this.Quotient = Maximum;
			}
		}

		/// <summary>
		///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
		/// </summary>
		/// <param name="value"></param>
		public Percentage( BigRational value ) : this( value.Numerator, value.Denominator ) { }

		/// <summary>Lerp?</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		[NotNull]
		public static Percentage Combine( [NotNull] Percentage left, [NotNull] Percentage right ) => new Percentage( ( left.Quotient + right.Quotient ) / 2.0 );

		//TODO BigDecimal any better here?
		/// <summary>static comparison</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Percentage left, Percentage right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null ) {
				return false;
			}

			if ( right is null ) {
				return false;
			}

			return left.Quotient == right.Quotient;
		}

		public static implicit operator Double( [NotNull] Percentage special ) => ( Double ) special.Quotient;

		[NotNull]
		public static implicit operator Percentage( Single value ) => new Percentage( value );

		[NotNull]
		public static implicit operator Percentage( Double value ) => new Percentage( value );

		[NotNull]
		public static Percentage operator +( [NotNull] Percentage left, [NotNull] Percentage right ) => Combine( left, right );

		[NotNull]
		public static Percentage Parse( [NotNull] String value ) {
			if ( value is null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			return new Percentage( Double.Parse( value ) );
		}

		public static Boolean TryParse( [NotNull] String numberString, [NotNull] out Percentage result ) {
			if ( numberString is null ) {
				throw new ArgumentNullException( nameof( numberString ) );
			}

			if ( !Double.TryParse( numberString, out var value ) ) {
				value = Double.NaN;
			}

			result = new Percentage( value );

			return !Double.IsNaN( value );
		}

		[Pure]
		public Int32 CompareTo( Double other ) => ( ( Double ) this.Quotient ).CompareTo( other );

		[Pure]
		public Int32 CompareTo( [NotNull] Percentage other ) {
			if ( other is null ) {
				throw new ArgumentNullException( nameof( other ) );
			}

			return this.Quotient.CompareTo( other.Quotient );
		}

		public Boolean Equals( Percentage other ) => Equals( this, other );

		public override String ToString() => $"{this.Quotient}";
	}
}