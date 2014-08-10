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
// "Librainian/Percentage.cs" was last cleaned by Rick on 2014/08/10 at 4:32 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using Annotations;
    using Librainian.Extensions;
    using Numerics;

    /// <summary>
    ///     <para>Restricts the value to between 0.0 and 1.0</para>
    /// </summary>
    /// <remarks>Uses memory barriers to help thread safety.</remarks>
    [DataContract( IsReference = true )]
    [Serializable]
    [Immutable]
    public class Percentage : IComparable<Percentage>, IComparable<Double>, IEquatable<Percentage> {
        /// <summary>
        ///     1
        /// </summary>
        public const Double Maximum = 1;

        /// <summary>
        ///     0
        /// </summary>
        public const Double Minimum = 0f;

        [DataMember]
        public readonly BigRational Value;

        /// <summary>
        ///     Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.
        /// </summary>
        /// <param name="value"></param>
        public Percentage( Double value ) {
            if ( value >= Maximum ) {
                this.Value = Maximum;
            }
            else if ( value <= Minimum ) {
                this.Value = Minimum;
            }
            else {
                this.Value = value;
            }
        }
        
        /// <summary>
        ///     Restricts the value to between <see cref="Minimum" /> and <see cref="Maximum" />.
        /// </summary>
        /// <param name="value"></param>
        public Percentage( BigRational value ) {
            if ( value >= Maximum ) {
                this.Value = Maximum;
            }
            else if ( value <= Minimum ) {
                this.Value = Minimum;
            }
            else {
                this.Value = value;
            }
        }

        public int CompareTo( Double other ) {
            return this.Value.CompareTo( other );
        }

        public int CompareTo( [NotNull] Percentage other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return this.Value.CompareTo( other.Value );
        }

        public bool Equals( [NotNull] Percentage other ) {
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
            if ( left == null )
                throw new ArgumentNullException( "left" );
            if ( right == null )
                throw new ArgumentNullException( "right" );
            return left.Value == right.Value ;
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
            return String.Format( "{0:P1}", (Decimal)this.Value );
        }

        /// <summary>
        ///     Lerp?
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Percentage Combine( Percentage left, Percentage right ) {
            return new Percentage( ( left.Value + right.Value ) / 2.0 );
        }

        public static implicit operator Double( Percentage special ) {
            return special.Value;
        }

        public static implicit operator Percentage( Single value ) {
            return new Percentage( value );
        }

        public static Percentage operator +( Percentage left, Percentage right ) {
            return Combine( left, right );
        }
    }
}
