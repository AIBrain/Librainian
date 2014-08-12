namespace Librainian.Measurement.Time.Clocks {
    using System;
    using Librainian.Extensions;

    /// <summary>
    /// A clock that stays at the set moment in time.
    /// </summary>
    [ImmutableAttribute]
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

        public bool IsAM() {
            return this.
        }

        public AMorPM GetAMorPM() {
            throw new NotImplementedException();
        }

        public bool IsPM() {
            throw new NotImplementedException();
        }

        public Time GetTime() {
            throw new NotImplementedException();
        }
    }
}