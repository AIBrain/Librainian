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
// "Librainian/Year.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Numerics;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a Year.</summary>
    [JsonObject]
    [Immutable]
    public struct Year : IComparable<Year>, IClockPart {
        public static readonly Year Zero = new Year( 0 );

        public Year( BigInteger value ) : this() => this.Value = value;

	    [JsonProperty]
        public BigInteger Value {
            get;
        }

        public static implicit operator BigInteger( Year value ) => value.Value;

        public static Boolean operator <( Year left, Year right ) => left.Value < right.Value;

        public static Boolean operator >( Year left, Year right ) => left.Value > right.Value;

        public Int32 CompareTo( Year other ) => this.Value.CompareTo( other.Value );

	    public Year Next() => new Year( this.Value + 1 );

	    public Year Previous() => new Year( this.Value - 1 );

    }
}