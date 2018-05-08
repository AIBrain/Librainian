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
// "Librainian/Month.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     A simple struct for a <see cref="Month" />.
    /// </summary>
    [JsonObject]
    [Immutable]
    public struct Month : IComparable<Month>, IClockPart {

        public static readonly Byte[] ValidMonths = Enumerable.Range( 1, Months.InOneCommonYear + 1 ) //TODO //BUG ??
                                                              .Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        public Month( Byte value ) {
            if ( !ValidMonths.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {Minimum} to {Maximum}." );
            }
            this.Value = value;
        }

        /// <summary>
        ///     12
        /// </summary>
        public static Byte Maximum { get; } = ValidMonths.Max();

        /// <summary>
        ///     1
        /// </summary>
        public static Byte Minimum { get; } = ValidMonths.Min();

        [JsonProperty]
        public Byte Value {
            get;
        }

        public Int32 CompareTo( Month other ) => this.Value.CompareTo( other.Value );

	    /// <summary>
        ///     Provide the next <see cref="Month" />.
        /// </summary>
        /// <param name="tocked"></param>
        /// <returns></returns>
        public Month Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;
            if ( next > Maximum ) {
                next = Minimum;
                tocked = true;
            }
            return new Month( ( Byte )next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Month" />.
        /// </summary>
        public Month Previous( out Boolean tocked ) {
            tocked = false;
            var next = this.Value - 1;
            if ( next < Minimum ) {
                next = Maximum;
                tocked = true;
            }
            return new Month( ( Byte )next );
        }
    }
}