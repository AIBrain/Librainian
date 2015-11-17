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
// "Librainian/Month.cs" was last cleaned by Rick on 2015/10/07 at 9:08 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;

    /// <summary>
    ///     A simple struct for a <see cref="Month" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Month : IComparable<Month>, IClockPart {

        public static readonly Byte[] ValidMonths = Enumerable.Range( 1, Months.InOneCommonYear + 1 )   //TODO //BUG ??
                                                      .Select( i => ( Byte )i )
                                                      .OrderBy( b => b )
                                                      .ToArray();

        /// <summary>
        ///     12
        /// </summary>
        public static readonly Byte MaximumValue = ValidMonths.Max();

        /// <summary>
        ///     1
        /// </summary>
        public static readonly Byte MinimumValue = ValidMonths.Min();

        static Month() {
            MaximumValue.Should()
                        .BeGreaterThan( MinimumValue );
        }

        public Month( Byte value ) {
            if ( !ValidMonths.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        [DataMember]
        public Byte Value {
            get;
        }

        public Int32 CompareTo( Month other ) {
            return this.Value.CompareTo( other.Value );
        }

        /// <summary>
        ///     Provide the next <see cref="Month" />.
        /// </summary>
        public Month Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Month( ( Byte )next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Month" />.
        /// </summary>
        public Month Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Month( ( Byte )next );
        }

    }

}
