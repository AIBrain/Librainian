namespace Librainian.Measurement.Time.Clocks {
    using System;

    public interface IClock {
        /// <summary>
        /// </summary>
        Hour Hour { get; }

        /// <summary>
        /// </summary>
        Minute Minute { get; }

        /// <summary>
        /// </summary>
        Second Second { get; }

        void Set( DateTime time );

        /// <summary>
        ///     <para>
        ///         Updates the <see cref="TickingClock.Hour" />, <see cref="TickingClock.Minute" />, and <see cref="TickingClock.Second" />.
        ///     </para>
        /// </summary>
        /// <param name="time"></param>
        void Set( Time time );

        Boolean IsAM();

        AMorPM GetAMorPM();

        Boolean IsPM();

        Time GetTime();
    }
}