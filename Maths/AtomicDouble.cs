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
// "Librainian/AtomicDouble.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM

#endregion License & Information

namespace Librainian.Maths {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>
    ///     A Double number thread-safe by <see cref="Interlocked" />.
    /// </summary>
    [DataContract]
    [Description( "A Double number thread-safe by Interlocked." )]
    public struct AtomicDouble {

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember]
        
        private Double _value;

        public AtomicDouble( Double value )
            : this() {
            this.Value = value;
        }

        public Double Value {
            get {
                return Interlocked.Exchange( ref this._value, this._value ); //todo buggy use of Exchange ?
            }

            set {
                Interlocked.Exchange( ref this._value, value );
            }
        }

        public static implicit operator Double( AtomicDouble special ) => special.Value;

        public static AtomicDouble operator -( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( a1.Value - a2.Value );

        public static AtomicDouble operator *( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( a1.Value * a2.Value );

        public static AtomicDouble operator +( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( a1.Value + a2.Value );

        public static AtomicDouble operator ++( AtomicDouble a1 ) {
            a1.Value++;
            return a1;
        }

        public static AtomicDouble Parse( [NotNull] String value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }
            return new AtomicDouble( Double.Parse( value ) );
        }

        /// <summary>
        ///     Resets the value to zero if less than zero;
        /// </summary>
        public void CheckReset() {
            if ( this.Value < 0 ) {
                this.Value = 0;
            }
        }

        public override String ToString() => String.Format( "{0:R}", this.Value );
    }
}