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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Millisecond.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Millisecond" />.</summary>
    [JsonObject]
    [Immutable]
    public struct Millisecond : IClockPart {
        public static readonly UInt16[] ValidMilliseconds = Enumerable.Range( 0, Milliseconds.InOneSecond ).Select( u => ( UInt16 )u ).OrderBy( u => u ).ToArray();

        [JsonProperty]
        public readonly UInt16 Value;

        public Millisecond( UInt16 value ) {
            if ( !ValidMilliseconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>0</summary>
        public static UInt16 MinimumValue { get; } = ValidMilliseconds.Min();

        /// <summary>999</summary>
        public static UInt16 MaximumValue { get; } = ValidMilliseconds.Max();

        /// <summary></summary>
        public static Millisecond Maximum { get; } = new Millisecond( MaximumValue );

        /// <summary></summary>
        public static Millisecond Minimum { get; } = new Millisecond( MinimumValue );

        /// <summary>Allow this class to be visibly cast to an <see cref="Int16" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Int16( Millisecond value ) => ( Int16 )value.Value;

        public static implicit operator Millisecond( UInt16 value ) => new Millisecond( value );

	    public static implicit operator UInt16( Millisecond value ) => value.Value;

        /// <summary>Provide the next <see cref="Millisecond" />.</summary>
        public Millisecond Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return ( UInt16 )next;
        }

        /// <summary>Provide the previous <see cref="Millisecond" />.</summary>
        public Millisecond Previous( out Boolean ticked ) {
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