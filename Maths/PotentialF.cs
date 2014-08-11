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
// "Librainian/PotentialF.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Threading;

    /// <summary>
    ///     <para>Restricts the value to between 0.0 and 1.0.</para>
    /// </summary>
    /// <remarks>
    ///     <para>Just wanted a threadsafe wrapper for Min and Max.</para>
    /// </remarks>
    [DataContract( IsReference = true )]
    public sealed class PotentialF {
        /// <summary>
        ///     1
        /// </summary>
        public const Single MaxValue = 1.0f;

        /// <summary>
        ///     <para>0.000000000000000000000000000000000000000000001401298</para>
        ///     <para>"1.401298E-45"</para>
        /// </summary>
        public const Single MinValue = 0.0f;

        /// <summary>
        /// </summary>
        /// <remarks>ONLY used in the getter and setter.</remarks>
        [DataMember] [OptionalField] private Single _value = MinValue;

        /// <summary>
        ///     Initializes a random number between <see cref="MinValue" /> and <see cref="MaxValue" />
        /// </summary>
        public PotentialF( Boolean randomValue ) {
            if ( randomValue ) {
                this.Value = Randem.NextFloat( MinValue, MaxValue );
            }
        }

        /// <summary>
        ///     Initializes with <paramref name="initialValue" />.
        /// </summary>
        /// <param name="initialValue"></param>
        public PotentialF( Single initialValue ) {
            this.Value = initialValue;
        }

        public PotentialF( Single min, Single max ) : this( Randem.NextFloat( min: min, max: max ) ) { }

        /// <summary>
        ///     <para>Thread-safe getter and setter.</para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Constrains the value to stay between <see cref="MinValue" /> and
        ///         <see
        ///             cref="MaxValue" />
        ///         .
        ///     </para>
        /// </remarks>
        public Single Value { get { return Thread.VolatileRead( ref this._value ); } private set { Thread.VolatileWrite( ref this._value, value >= MaxValue ? MaxValue : ( value <= MinValue ? MinValue : value ) ); } }

        public static PotentialF Parse( String value ) {
            return new PotentialF( Single.Parse( value ) );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public override String ToString() {
            return String.Format( "{0:P3}", this.Value );
        }

        public static implicit operator Single( PotentialF special ) {
            return special.Value;
        }

        public void Add( Single amount ) {
            this.Value += amount;
        }

        public void Divide( Single amount ) {
            this.Value /= amount;
        }

        public void Multiply( Single amount ) {
            this.Value *= amount;
        }

        //public void DropByRelative( PotentialF percentage ) {
        //    this.Value -= percentage.Value * this.Value;
        //}

        //public void DropByAbsolute( PotentialF percentage ) {
        //    this.Value -= percentage.Value;
        //}

        //public void RaiseByRelative( PotentialF percentage ) {
        //    this.Value += percentage.Value * this.Value;
        //}

        //public void RaiseByAbsolute( PotentialF percentage ) {
        //    this.Value += percentage.Value;
        //}
        //public static implicit operator PotentialF( Single value ) {
        //    return new PotentialF( value );
        //}
    }
}
