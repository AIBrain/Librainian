namespace Librainian.Measurement.Time.Clocks {
    using System;

    public interface IStandardClockWithSmallUnits {

        Hour Hour { get; }

        Minute Minute { get; }

        Second Second { get; }

        Millisecond Millisecond { get; }

        Microsecond Microsecond { get; }

        Time GetTime();

        Boolean IsAM();

        Boolean IsPM();
    }
}