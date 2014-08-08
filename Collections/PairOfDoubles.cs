#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/PairOfDoubles.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Runtime.Serialization;

    [DataContract( IsReference = true )]
    public struct PairOfDoubles {

        [DataMember]
        [OptionalField]
        private Double _high;

        [DataMember]
        [OptionalField]
        private Double _low;

        public PairOfDoubles( Double low, Double high ) {
            this._low = Math.Min( low, high );
            this._high = Math.Max( low, high );
        }

        public Double High {
            get { return this._high; }

            set {
                if ( value < this._low ) {
                    this._high = this._low;
                    this._low = value;
                }
                else {
                    this._high = value;
                }
            }
        }

        public Double Low {
            get { return this._low; }

            set {
                if ( value > this._high ) {
                    this._low = this._high;
                    this._high = value;
                }
                else {
                    this._low = value;
                }
            }
        }
    }
}