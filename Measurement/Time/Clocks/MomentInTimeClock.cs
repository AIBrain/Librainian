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
// "Librainian/MomentInTimeClock.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>A clock that stays at the set moment in time.</summary>
    [JsonObject]
    [Immutable]
    public class MomentInTimeClock : IStandardClock {

        public MomentInTimeClock() : this( Measurement.Time.Time.Now() ) {
        }

        public MomentInTimeClock( Time time ) {
            this.Hour = time.Hour;
            this.Minute = time.Minute;
            this.Second = time.Second;
            this.Millisecond = time.Millisecond;
        }

        public MomentInTimeClock( DateTime time ) : this( ( Time )time ) {
            this.Hour = new Hour( ( Byte )time.Hour );
            this.Minute = new Minute( ( Byte )time.Minute );
            this.Second = new Second( ( Byte )time.Second );
        }

        [JsonProperty]
        public Hour Hour {
            get;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Millisecond Millisecond {
            get;
        }

        [JsonProperty]
        public Minute Minute {
            get;
        }

        [JsonProperty]
        public Second Second {
            get;
        }

        public Boolean IsAm() => !this.IsPm();

        public Boolean IsPm() => this.Hour >= 12;

        public Time Time() => new Time( hour: this.Hour, minute: this.Minute, second: this.Second );
    }
}