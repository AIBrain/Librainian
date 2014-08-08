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
// "Librainian2/Percentage.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;

    /// <summary>
    ///     Restricts the value to between 0.0 and 1.0
    /// </summary>
    /// <remarks>Uses memory barriers to help thread safety.</remarks>
    [DataContract( IsReference = true )]
    public struct Percentage {
        public const Single MaxValue = 1.0f;
        public const Single MinValue = 0.0f;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] private Single _value;

        /// <summary>
        ///     Uses Interlocked to ensure thread safety and restricts the value to between 0.0 and 1.0
        /// </summary>
        /// <param name="value"></param>
        public Percentage( Single value ) : this() {
            this.Value = value;
        }

        public Single Value {
            get { return Thread.VolatileRead( ref this._value ); }

            set {
                var correctedvalue = value;
                if ( value > MaxValue ) {
                    correctedvalue = MaxValue;
                }
                else if ( value < MinValue ) {
                    correctedvalue = MinValue;
                }
                Thread.VolatileWrite( ref this._value, correctedvalue );
            }
        }

        /// <summary>
        ///     Lerp?
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static Percentage Combine( Percentage value1, Percentage value2 ) {
            return new Percentage( ( value1 + value2 )/2.0f );
        }

        public static implicit operator Percentage( Single value ) {
            return new Percentage( value );
        }

        public static implicit operator Single( Percentage special ) {
            return special.Value;
        }

        public static Percentage Parse( [NotNull] String value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }
            return new Percentage( Single.Parse( value ) );
        }

        public override String ToString() {
            return String.Format( "{0:P1}", this.Value );
        }
    }
}
