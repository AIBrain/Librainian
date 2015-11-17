#region License & Information

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
// "Librainian/Hour.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;

    /// <summary>
    /// <para>A simple struct for an <see cref="Hour" />.</para></summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Hour : IClockPart {
        public static readonly Byte[] ValidHours = Enumerable.Range( 0, Hours.InOneDay ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        /// <summary>should be 23</summary>
        public static readonly Byte MaximumValue = ValidHours.Max();

        /// <summary>should be 0</summary>
        public static readonly Byte MinimumValue = ValidHours.Min();

        public static readonly Hour Maximum = new Hour( MaximumValue );
        public static readonly Hour Minimum = new Hour( MinimumValue );

        [DataMember]
        public readonly Byte Value;

        static Hour() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        public Hour(Byte value) {
            if ( !ValidHours.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>Allow this class to be visibly cast to a <see cref="SByte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte(Hour value) => ( SByte )value.Value;

        public static implicit operator Byte(Hour value) => value.Value;

        /// <summary>Provide the next <see cref="Hour" />.</summary>
        public Hour Next(out Boolean ticked) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Hour( ( Byte )next );
        }

        /// <summary>Provide the previous <see cref="Hour" />.</summary>
        public Hour Previous(out Boolean ticked) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Hour( ( Byte )next );
        }
    }
}