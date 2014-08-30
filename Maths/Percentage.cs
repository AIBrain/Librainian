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
// "Librainian/Percentage.cs" was last cleaned by Rick on 2014/08/22 at 9:40 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;
    using Numerics;

    /// <summary>
    ///     <para>Restricts the value to between 0.0 and 1.0.</para>
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [Immutable]
    public class Percentage : IComparable<Percentage>, IComparable<Double>, IEquatable<Percentage> {

        /// <summary>
        ///     1
        /// </summary>
        public const Double Maximum = 1d;

        /// <summary>
        ///     0
        /// </summary>
        public const Double Minimum = 0d;

        [CanBeNull]
        [DataMember]
        public readonly BigInteger? Denominator;

        [CanBeNull]
        [DataMember]
        public readonly BigInteger? LeastCommonDenominator;

        [CanBeNull]
        [DataMember]
        public readonly BigInteger? Numerator;
        
        [DataMember]
        public readonly BigRational Quotient;  //TODO BigDecimal any better here?

        /// <summary>
        ///     <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
        /// </summary>
        /// <param name="value"></param>
        public Percentage( Double value ) : this( ( BigInteger ) value, BigInteger.One) {}

        /// <summary>
        /// <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Percentage(Decimal numerator,Decimal denominator ) : this( ( Double )numerator, ( Double )denominator ) {}

        /// <summary>
        /// <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Percentage( Double numerator, Double denominator ) {
            if ( Double.IsNaN( numerator ) ) {
                throw new ArgumentOutOfRangeException( "numerator", "Numerator is not a number." );
            }
            if ( Double.IsNaN( denominator ) ) {
                throw new ArgumentOutOfRangeException( "denominator", "Denominator is not a number." );
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
        /// <para>Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.</para>
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Percentage( BigInteger numerator, BigInteger denominator ) {
            this.Numerator = numerator;
            this.Denominator = denominator;
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
        /// <param name="value"></param>
        public Percentage( BigRational value ) : this( value.Numerator, value.Denominator ) {
        }

        [Pure]
        public int CompareTo( Double other ) {
            return this.Quotient.CompareTo( other );
        }

        [Pure]
        public int CompareTo( [NotNull] Percentage other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return this.Quotient.CompareTo( other.Quotient );
        }

        public Boolean Equals( [NotNull] Percentage other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return Equals( this, other );
        }

        /// <summary>
        ///     static comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [NotNull] Percentage left, [NotNull] Percentage right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            return left.Quotient == right.Quotient;
        }

        public static Percentage Parse( [NotNull] String value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }
            return new Percentage( Double.Parse( value ) );
        }

        public static Boolean TryParse( [NotNull] String numberString, out Percentage result ) {
            if ( numberString == null ) {
                throw new ArgumentNullException( "numberString" );
            }
            Double value;
            if ( !Double.TryParse( numberString, out value ) ) {
                value = Double.NaN;
            }
            result = new Percentage( value );
            return !Double.IsNaN( value );
        }

        public override String ToString() {
            return String.Format( "{0}", this.Quotient );
        }

        /// <summary>
        ///     Lerp?
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Percentage Combine( Percentage left, Percentage right ) {
            return new Percentage( ( left.Quotient + right.Quotient ) / 2.0 );
        }

        public static implicit operator Double( Percentage special ) {
            return ( Double )special.Quotient;
        }

        //public static implicit operator  System.Decimal( Percentage special ) {
        //    return (  System.Decimal )special.Value;
        //}

        public static implicit operator Percentage( Single value ) {
            return new Percentage( value );
        }

        public static implicit operator Percentage( Double value ) {
            return new Percentage( value );
        }

        public static Percentage operator +( Percentage left, Percentage right ) {
            return Combine( left, right );
        }
    }
}
