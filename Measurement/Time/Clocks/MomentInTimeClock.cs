namespace Librainian.Measurement.Time.Clocks {
    using System;
    using Librainian.Extensions;

    /// <summary>
    /// A clock that stays at the set moment in time.
    /// </summary>
    [Immutable]
    public class MomentInTimeClock : IStandardClock {

        public Hour Hour { get; private set; }
        public Minute Minute { get; private set; }
        public Second Second { get; private set; }

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
            this.Hour = new Hour( time.Hour );
            this.Minute = new Minute( time.Minute );
            this.Second = new Second( time.Second );
        }

        public Boolean IsAM() {
            return !this.IsPM();
        }

        public Boolean IsPM() {
            return this.Hour >= 12;
        }

        public Time GetTime() {
            return new Time( hour: this.Hour, minute: this.Minute, second: this.Second );
        }
    }
}