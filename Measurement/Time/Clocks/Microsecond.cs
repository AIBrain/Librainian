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
// "Librainian/Microsecond.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Microsecond" />.</summary>
    [JsonObject]
    [Immutable]
    public struct Microsecond : IClockPart {
        public static readonly UInt16[] ValidMicroseconds = Enumerable.Range( 0, Microseconds.InOneMillisecond ).Select( u => ( UInt16 )u ).OrderBy( u => u ).ToArray();

        [JsonProperty]
        public readonly UInt16 Value;

        public Microsecond( UInt16 value ) {
            if ( !ValidMicroseconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>0</summary>
        public static UInt16 MinimumValue { get; } = ValidMicroseconds.Min();

        /// <summary>999</summary>
        public static UInt16 MaximumValue { get; } = ValidMicroseconds.Max();

        /// <summary></summary>
        public static Microsecond Maxium { get; } = new Microsecond( MaximumValue );

        /// <summary></summary>
        public static Microsecond Minimum { get; } = new Microsecond( MinimumValue );

        /// <summary>Allow this class to be visibly cast to an <see cref="Int16" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Int16( Microsecond value ) => ( Int16 )value.Value;

        public static implicit operator Microsecond( UInt16 value ) {
            return new Microsecond( value );
        }

        public static implicit operator UInt16( Microsecond value ) => value.Value;

        /// <summary>Provide the next <see cref="Microsecond" />.</summary>
        public Microsecond Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return ( UInt16 )next;
        }

        /// <summary>Provide the previous <see cref="Microsecond" />.</summary>
        public Microsecond Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return ( UInt16 )next;
        }
    }
}