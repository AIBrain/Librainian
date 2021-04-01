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
// File "Percentage.cs" last formatted on 2020-08-14 at 8:35 PM.

#nullable enable
namespace Librainian.Maths.Numbers {

	using System;
	using System.Diagnostics;
	using Extensions;
	using JetBrains.Annotations;
	using Measurement;
	using Newtonsoft.Json;
	using Rationals;

	/// <summary>
	///     <para>Restricts the value to between 0.0 and 1.0.</para>
	/// </summary>
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public record Percentage : IComparable<Percentage>, IComparable<Double>, IComparable<Decimal> {

		public Percentage( Rational numerator, Rational denominator ) : this( ( Decimal )Rational.Divide( numerator, denominator ) ) { }
		public Percentage( Rational value ) : this( ( Decimal )value ) { }

		public Percentage( Single value ) : this( ( Decimal )value ) { }

		public Percentage( Double value ) : this( ( Decimal )value ) { }

		public Percentage( Decimal value ) => this.Value = value;

		public Percentage( Int32 value ) : this( ( Decimal )value ) { }

		public const Decimal MinimumValue = 0m;
		public const Decimal MaximumValue = 1m;

		private readonly Decimal _value;

		/// <summary>The number resulting from the division of one number by another.</summary>
		[JsonProperty]
		public Decimal Value {
			get => this._value;

			init {
				this._value = value switch {
					< MinimumValue => MinimumValue,
					> MaximumValue => MaximumValue,
					var _ => value
				};
			}
		}

		[Pure]
		public Int32 CompareTo( Decimal other ) => this.Value.CompareTo( other );

		[Pure]
		public Int32 CompareTo( Double other ) => this.Value.CompareTo( other );

		[Pure]
		public Int32 CompareTo( [CanBeNull] Percentage? other ) {
			if ( other is null ) {
				return Order.Before;
			}

			return this.Value.CompareTo( other.Value );
		}

		//public Boolean Equals( [CanBeNull] Percentage? other ) => Equals( this, other );

		/// <summary>Lerp?</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		[NotNull]
		public static Percentage Combine( [NotNull] Percentage left, [NotNull] Percentage right ) =>
			new( ( left.Value + right.Value ) / 2m );

		/// <summary>static comparison</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] Percentage? left, [CanBeNull] Percentage? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			return left.Value == right.Value;
		}

		public static explicit operator Double( [NotNull] Percentage special ) => ( Double )special.Value;
		public static explicit operator Single( [NotNull] Percentage special ) => ( Single)special.Value;

		[NotNull]
		public static implicit operator Percentage( Single value ) => new( value );

		[NotNull]
		public static implicit operator Percentage( Double value ) => new( value );

		[NotNull]
		public static implicit operator Percentage( Decimal value ) => new( value );

		[NotNull]
		public static implicit operator Percentage( Int32 value ) => new( value );

		/*
		/// <summary>
		///     Returns a value that indicates whether two <see cref="T:Librainian.Maths.Numbers.Percentage" /> objects have
		///     different values.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
		public static Boolean operator !=( [CanBeNull] Percentage? left, [CanBeNull] Percentage? right ) => !Equals( left, right );
		*/

		[NotNull]
		public static Percentage operator +( [NotNull] Percentage left, [NotNull] Percentage right ) => Combine( left, right );

		/*
		/// <summary>
		///     Returns a value that indicates whether the values of two <see cref="T:Librainian.Maths.Numbers.Percentage" />
		///     objects are equal.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
		///     false.
		/// </returns>
		public static Boolean operator ==( [CanBeNull] Percentage? left, [CanBeNull] Percentage? right ) => Equals( left, right );
		*/

		[CanBeNull]
		public static Percentage? Parse( [NotNull] String value ) {
			if ( value is null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			if ( Decimal.TryParse( value, out var dec ) ) {
				return new Percentage( dec );
			}

			if ( Rational.TryParse( value, out var rational ) ) {
				return new Percentage( rational );
			}

			if ( Double.TryParse( value, out var dob ) ) {
				return new Percentage( dob );
			}

			return default( Percentage );
		}

		public static Boolean TryParse( [NotNull] String numberString, [CanBeNull] out Percentage? result ) {
			if ( Decimal.TryParse( numberString, out var dec ) ) {
				result = new Percentage( dec );

				return true;
			}
			if ( Rational.TryParse( numberString, out var rational ) ) {
				result = new Percentage( rational );

				return true;
			}

			if ( Double.TryParse( numberString, out var dob ) ) {
				result = new Percentage( dob );

				return true;
			}

			result = default( Percentage );

			return false;
		}

		/*
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
		///     <see langword="false" />.
		/// </returns>
		public override Boolean Equals( Object? obj ) => Equals( this, obj as Percentage );
		*/

		/*
		/// <summary>Serves as the default hash function.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override Int32 GetHashCode() => this.Value.GetHashCode();
		*/

		/*
		[NotNull]
		public override String ToString() => $"{this.Value.ToString()}";
		*/

		public virtual Boolean Equals( Percentage? other ) {
			if ( other is null ) {
				return false;
			}

			if ( ReferenceEquals( this, other ) ) {
				return true;
			}

			return this.Value == other.Value;
		}

		public override Int32 GetHashCode() => this.Value.GetHashCode();

	}

}