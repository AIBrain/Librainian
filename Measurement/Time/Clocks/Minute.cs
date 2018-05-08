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
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Minute.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Minute" />.</summary>
    [JsonObject]
    [Immutable]
    public struct Minute : IClockPart {
        public static readonly Byte[] ValidMinutes = Enumerable.Range( 0, Minutes.InOneHour ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        [JsonProperty]
        public readonly Byte Value;

        public Minute( Byte value ) {
            if ( !ValidMinutes.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>should be 0</summary>
        public static Byte MinimumValue { get; } = ValidMinutes.Min();

        /// <summary>should be 59</summary>
        public static Byte MaximumValue { get; } = ValidMinutes.Max();

        public static Minute Maximum { get; } = new Minute( MaximumValue );

        public static Minute Minimum { get; } = new Minute( MinimumValue );

        /// <summary>Allow this class to be visibly cast to a <see cref="SByte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte( Minute value ) => ( SByte )value.Value;

        /// <summary>Allow this class to be read as a <see cref="Byte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Byte( Minute value ) => value.Value;

        public static implicit operator Minute( Byte value ) => new Minute( value );

	    /// <summary>Provide the next minute.</summary>
        public Minute Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                tocked = true;
            }
            return ( Byte )next;
        }

        /// <summary>Provide the previous minute.</summary>
        public Minute Previous( out Boolean tocked ) {
            tocked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                tocked = true;
            }
            return ( Byte )next;
        }
    }
}