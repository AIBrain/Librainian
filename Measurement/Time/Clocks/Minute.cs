// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Minute.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Minute.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
            if ( !ValidMinutes.Contains( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." ); }

            this.Value = value;
        }

        public static Minute Maximum { get; } = new Minute( MaximumValue );

        /// <summary>should be 59</summary>
        public static Byte MaximumValue { get; } = ValidMinutes.Max();

        public static Minute Minimum { get; } = new Minute( MinimumValue );

        /// <summary>should be 0</summary>
        public static Byte MinimumValue { get; } = ValidMinutes.Min();

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