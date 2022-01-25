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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Percentage.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

#nullable enable

namespace Librainian.Maths.Numbers;

using System;
using System.Diagnostics;
using Exceptions;
using Extensions;
using Measurement;
using Newtonsoft.Json;
using Rationals;
using Utilities;

/// <summary>
///     <para>Restricts the value to between 0.0 and 1.0.</para>
/// </summary>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record Percentage : IComparable<Percentage>, IComparable<Double>, IComparable<Decimal> {
	public const Decimal MinimumValue = 0m;

	public const Decimal MaximumValue = 1m;

	private readonly Decimal _value;

	public Percentage( Rational numerator, Rational denominator ) : this( ( Decimal )Rational.Divide( numerator, denominator ) ) { }

	public Percentage( Rational value ) : this( ( Decimal )value ) { }

	public Percentage( Single value ) : this( ( Decimal )value ) { }

	public Percentage( Double value ) : this( ( Decimal )value ) { }

	public Percentage( Decimal value ) => this.Value = value;

	public Percentage( Int32 value ) : this( ( Decimal )value ) { }

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

	[NeedsTesting]
	public Int32 CompareTo( Decimal other ) => this.Value.CompareTo( other );

	[NeedsTesting]
	public Int32 CompareTo( Double other ) => this.Value.CompareTo( other );

	[NeedsTesting]
	public Int32 CompareTo( Percentage? other ) {
		if ( other is null ) {
			return SortingOrder.Before;
		}

		return this.Value.CompareTo( other.Value );
	}

	/*

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>
	/// <see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.
	/// </returns>
	public override Boolean Equals( Object? obj ) => Equals( this, obj as Percentage );
	*/

	/*

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override Int32 GetHashCode() => this.Value.GetHashCode();
	*/

	/*
	[NeedsTesting]
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

	//public Boolean Equals( [NeedsTesting] Percentage? other ) => Equals( this, other );

	/// <summary>Lerp?</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Percentage Combine( Percentage left, Percentage right ) => new( ( left.Value + right.Value ) / 2m );

	/// <summary>static comparison</summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Percentage? left, Percentage? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		return left.Value == right.Value;
	}

	public static explicit operator Double( Percentage special ) => ( Double )special.Value;

	public static explicit operator Single( Percentage special ) => ( Single )special.Value;

	public static implicit operator Percentage( Single value ) => new( value );

	public static implicit operator Percentage( Double value ) => new( value );

	public static implicit operator Percentage( Decimal value ) => new( value );

	public static implicit operator Percentage( Int32 value ) => new( value );

	/*

	/// <summary>
	/// Returns a value that indicates whether two <see cref="T:Librainian.Maths.Numbers.Percentage" /> objects have different values.
	/// </summary>
	/// <param name="left">The first value to compare.</param>
	/// <param name="right">The second value to compare.</param>
	/// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
	public static Boolean operator !=( [NeedsTesting] Percentage? left, [NeedsTesting] Percentage? right ) => !Equals( left, right );
	*/

	public static Percentage operator +( Percentage left, Percentage right ) => Combine( left, right );

	public static Percentage? Parse( String value ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
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

	public static Boolean TryParse( String numberString, out Percentage? result ) {
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

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public static Boolean operator <( Percentage left, Percentage right ) => left.CompareTo( right ) < 0;

	public static Boolean operator <=( Percentage left, Percentage right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >( Percentage left, Percentage right ) => left.CompareTo( right ) > 0;

	public static Boolean operator >=( Percentage left, Percentage right ) => left.CompareTo( right ) >= 0;
}