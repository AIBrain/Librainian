// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AtomicDouble.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;
    using System.ComponentModel;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>A Double number thread-safe by <see cref="Interlocked" />.</summary>
    [JsonObject]
    [Description( "A Double number thread-safe by Interlocked." )]
    public struct AtomicDouble {

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public AtomicDouble( Double value ) : this() => this.Value = value;

	    public Double Value {
            get => Interlocked.Exchange( ref this._value, this._value );

	        set => Interlocked.Exchange( ref this._value, value );
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
            if ( value is null ) {
                throw new ArgumentNullException( nameof( value ) );
            }
            return new AtomicDouble( Double.Parse( value ) );
        }

        /// <summary>Resets the value to zero if less than zero;</summary>
        public void CheckReset() {
            if ( this.Value < 0 ) {
                this.Value = 0;
            }
        }

        public override String ToString() => $"{this.Value:R}";
    }
}