// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Percentage.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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
        public readonly BigRational Quotient; //TODO BigDecimal any better here?

        /// <summary>
        ///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
        /// </summary>
        /// <param name="value"></param>
        public Percentage( Double value ) : this( ( BigInteger )value, BigInteger.One ) { }

        /// <summary>
        ///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Percentage( Decimal numerator, Decimal denominator ) : this( ( Double )numerator, ( Double )denominator ) { }

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
        public static Percentage Combine( Percentage left, Percentage right ) => new Percentage( ( left.Quotient + right.Quotient ) / 2.0 );

        /// <summary>static comparison</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Percentage left, Percentage right ) {
            if ( ReferenceEquals(left, right) ) {
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

        public static implicit operator Double( Percentage special ) => ( Double )special.Quotient;

        public static implicit operator Percentage( Single value ) => new Percentage( value );

        public static implicit operator Percentage( Double value ) => new Percentage( value );

        public static Percentage operator +( Percentage left, Percentage right ) => Combine( left, right );

        public static Percentage Parse( [NotNull] String value ) {
            if ( value is null ) {
                throw new ArgumentNullException( nameof( value ) );
            }
            return new Percentage( Double.Parse( value ) );
        }

        public static Boolean TryParse( [NotNull] String numberString, out Percentage result ) {
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
        public Int32 CompareTo( Double other ) => ( ( Double )this.Quotient ).CompareTo( other );

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