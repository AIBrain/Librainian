// Copyright 2018 Protiguous.
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
// "Librainian/Hour.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A simple struct for an <see cref="Hour" />.</para>
    /// </summary>
    [JsonObject]
    [Immutable]
    public struct Hour : IClockPart {
        public static readonly Byte[] ValidHours = Enumerable.Range( start: 0, count: Hours.InOneDay ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        [JsonProperty]
        public readonly Byte Value;

        public Hour( Byte value ) {
            if ( !ValidHours.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>
        ///     should be 0
        /// </summary>
        public static Byte MinimumValue { get; } = ValidHours.Min();

        /// <summary>
        ///     should be 23
        /// </summary>
        public static Byte MaximumValue { get; } = ValidHours.Max();

        public static Hour Maximum { get; } = new Hour( MaximumValue );

        public static Hour Minimum { get; } = new Hour( MinimumValue );

        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte( Hour value ) => ( SByte )value.Value;

        public static implicit operator Byte( Hour value ) => value.Value;

        public static implicit operator Hour( Byte value ) => new Hour( value );

	    /// <summary>
        ///     Provide the next <see cref="Hour" />.
        /// </summary>
        public Hour Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                tocked = true;
            }
            return ( Byte )next;
        }

        /// <summary>
        ///     Provide the previous <see cref="Hour" />.
        /// </summary>
        public Hour Previous( out Boolean tocked ) {
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