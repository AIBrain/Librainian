// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/WhenRange.cs" was last cleaned by Rick on 2015/06/12 at 3:03 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a <see cref="UniversalDateTime" /> range with minimum and maximum values.
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    public struct WhenRange {

        /// <summary>Length of the range (difference between maximum and minimum values).</summary>
        [DataMember]
        public readonly Span Length;

        /// <summary>Maximum value</summary>
        [DataMember]
        public readonly UniversalDateTime Max;

        /// <summary>Minimum value</summary>
        [DataMember]
        public readonly UniversalDateTime Min;

        /// <summary>Initializes a new instance of the <see cref="WhenRange" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public WhenRange(UniversalDateTime min, UniversalDateTime max) {
            if ( min < max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }
            var δ = this.Max.Value - this.Min.Value;
            this.Length = new Span( planckTimes: δ );
        }
    }
}