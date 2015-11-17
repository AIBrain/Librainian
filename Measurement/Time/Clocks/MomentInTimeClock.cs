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
// "Librainian/MomentInTimeClock.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using Extensions;

    /// <summary>A clock that stays at the set moment in time.</summary>
    [DataContract( IsReference = true )]
    [Immutable]
    public class MomentInTimeClock : IStandardClock {

        [DataMember]
        public Hour Hour {
            get;
        }

        [DataMember]
        public Minute Minute {
            get;
        }

        [DataMember]
        public Second Second {
            get;
        }

        public MomentInTimeClock() {
            var now = Measurement.Time.Time.Now();
            this.Hour = new Hour( now.Hour );
            this.Minute = new Minute( now.Minute );
            this.Second = new Second( now.Second );
        }

        public MomentInTimeClock(Time time) {
            this.Hour = new Hour( time.Hour );
            this.Minute = new Minute( time.Minute );
            this.Second = new Second( time.Second );
        }

        public MomentInTimeClock(DateTime time) {
            this.Hour = new Hour( ( Byte )time.Hour );
            this.Minute = new Minute( ( Byte )time.Minute );
            this.Second = new Second( ( Byte )time.Second );
        }

        public Boolean IsAm() => !this.IsPm();

        public Boolean IsPm() => this.Hour >= 12;

        public Time Time() => new Time( hour: this.Hour, minute: this.Minute, second: this.Second );
    }
}