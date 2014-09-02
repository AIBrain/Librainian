#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MomentInTimeClock.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using Librainian.Extensions;

    /// <summary>
    ///     A clock that stays at the set moment in time.
    /// </summary>
    [DataContract(IsReference = true)]
    [Immutable]
    public class MomentInTimeClock : IStandardClock {

        public MomentInTimeClock() {
            var now = Time.Now;
            this.Hour = new Hour( now.Hour );
            this.Minute = new Minute( now.Minute );
            this.Second = new Second( now.Second );
        }

        public MomentInTimeClock( Time time ) {
            this.Hour = new Hour( time.Hour );
            this.Minute = new Minute( time.Minute );
            this.Second = new Second( time.Second );
        }

        public MomentInTimeClock( DateTime time ) {
            this.Hour = new Hour( ( byte ) time.Hour );
            this.Minute = new Minute( ( byte ) time.Minute );
            this.Second = new Second( ( byte ) time.Second );
        }

        [DataMember]
        public Hour Hour {
            get;
            private set;
        }

        [DataMember]
        public Minute Minute {
            get;
            private set;
        }

        [DataMember]
        public Second Second {
            get;
            private set;
        }

        public Time GetTime() {
            return new Time( hour: this.Hour, minute: this.Minute, second: this.Second );
        }

        public Boolean IsAM() {
            return !this.IsPM();
        }

        public Boolean IsPM() {
            return this.Hour >= 12;
        }
    }
}