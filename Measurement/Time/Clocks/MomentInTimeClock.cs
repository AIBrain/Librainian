namespace Librainian.Measurement.Time.Clocks {
    using System;
    using Librainian.Extensions;

    /// <summary>
    /// A clock that stays at the set moment in time.
    /// </summary>
    [ImmutableAttribute]
    public class MomentInTimeClock : IClock {

        public Hour Hour { get; private set; }
        public Minute Minute { get; private set; }
        public Second Second { get; private set; }

        public void Set( DateTime time ) {
            this.Hour.Set( time.Hour );
        }

        public void Set( Time time ) {
            throw new NotImplementedException();
        }

        public bool IsAM() {
            throw new NotImplementedException();
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