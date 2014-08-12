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
// "Librainian/Year.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Clocks;

    /// <summary>
    ///     A simple struct for a Year.
    /// </summary>
    [DataContract]
    [Serializable]
    public struct Year : PartofaClock {
        public Year( BigInteger year ) : this() {
            this.Value = year;
        }

        [DataMember]
        public BigInteger Value { get; private set; }

        /// <summary>
        ///     Decrease the current month.
        /// </summary>
        public Boolean Rewind() {
            this.Value--;
            return false;
        }

        public void Set( Byte value ) {
            this.Value = value;
        }

        /// <summary>
        ///     <para>Increase the year.</para>
        /// </summary>
        public Boolean Tick() {
            this.Value++;
            return false;
        }

        public static Boolean operator <( Year left, Year right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator >( Year left, Year right ) {
            return left.Value > right.Value;
        }

        public static implicit operator BigInteger( Year year ) {
            return year.Value;
        }
    }
}
