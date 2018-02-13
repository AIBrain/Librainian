// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Second.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Second" />.</summary>
    [JsonObject]
    [Immutable]
    public sealed class Second : IClockPart {
        public static readonly Byte[] ValidSeconds = Enumerable.Range( 0, Seconds.InOneMinute ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        [JsonProperty]
        public readonly Byte Value;

        public Second( Byte value ) {
            if ( !ValidSeconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>should be 0</summary>
        public static Byte MinimumValue { get; } = ValidSeconds.Min();

        /// <summary>should be 59</summary>
        public static Byte MaximumValue { get; } = ValidSeconds.Max();

        public static Second Maximum { get; } = new Second( MaximumValue );

        public static Second Minimum { get; } = new Second( MinimumValue );

        /// <summary>Allow this class to be visibly cast to a <see cref="SByte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte( Second value ) => ( SByte )value.Value;

        /// <summary>Allow this class to be read as a <see cref="Byte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Byte( Second value ) => value.Value;

        public static implicit operator Second( Byte value ) {
            return new Second( value );
        }

        /// <summary>Provide the next second.</summary>
        public Second Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                tocked = true;
            }
            return ( Byte )next;
        }

        /// <summary>Provide the previous second.</summary>
        public Second Previous( out Boolean tocked ) {
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