// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Year.cs" was last cleaned by Rick on 2015/10/07 at 9:04 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Extensions;

    /// <summary>A simple struct for a Year.</summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public struct Year : IComparable<Year>, IClockPart {

        public Year( BigInteger value ) : this() {
            this.Value = value;
        }

        [DataMember]
        public BigInteger Value { get; }

        public static implicit operator BigInteger( Year value ) => value.Value;

        public static Boolean operator <( Year left, Year right ) => left.Value < right.Value;

        public static Boolean operator >( Year left, Year right ) => left.Value > right.Value;

        public Year Previous() {
            return new Year( this.Value - 1 );
        }

        public Year Next() {
            return new Year( this.Value + 1 );
        }

        public Int32 CompareTo( Year other ) {
            return this.Value.CompareTo( other.Value );
        }

    }

}
