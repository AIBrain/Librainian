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
// "Librainian/Year.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Numerics;
    using System.Runtime.Serialization;

    /// <summary>
    ///     A simple struct for a Year.
    /// </summary>
    [DataContract]
    [Serializable]
    public struct Year : IClockPart {

        public Year( BigInteger value )
            : this() {
                this.Value = value;
        }

        [DataMember]
        public BigInteger Value {
            get;
            private set;
        }

        public static implicit operator BigInteger( Year value ) {
            return value.Value;
        }

        public static Boolean operator <( Year left, Year right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( Year left, Year right ) {
            return left.Value > right.Value;
        }

        /// <summary>
        ///     Decrease the current year.
        /// </summary>
        public Boolean Previous() {
            this.Value--;
            return false;
        }

      

        /// <summary>
        ///     <para>Increase the year.</para>
        /// </summary>
        public Boolean Next() {
            this.Value++;
            return false;
        }
    }
}