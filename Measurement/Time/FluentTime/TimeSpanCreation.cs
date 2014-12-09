

namespace Librainian.Measurement.Time.FluentTime {
    using System;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public static class TimeSpanCreation {

        public static TimeSpan Day( this double i ) => Days( i );

        public static TimeSpan Day( this double i, TimeSpan otherTime ) => Days( i, otherTime );

        public static TimeSpan Day( this int i ) => Day( ( double )i );

        public static TimeSpan Day( this int i, TimeSpan otherTime ) => Days( ( double )i, otherTime );

        public static TimeSpan Days( this double i ) => TimeSpan.FromDays( i );

        public static TimeSpan Days( this double i, TimeSpan otherTime ) => Days( i ) + otherTime;

        public static TimeSpan Days( this int i ) => Days( ( double )i );

        public static TimeSpan Days( this int i, TimeSpan otherTime ) => Days( ( double )i, otherTime );

        public static TimeSpan Hour( this double i ) => Hours( i );

        public static TimeSpan Hour( this double i, TimeSpan otherTime ) => Hours( i, otherTime );

        public static TimeSpan Hour( this int i ) => Hour( ( double )i );

        public static TimeSpan Hour( this int i, TimeSpan otherTime ) => Hours( ( double )i, otherTime );

        public static TimeSpan Hours( this double i ) => TimeSpan.FromHours( i );

        public static TimeSpan Hours( this double i, TimeSpan otherTime ) => Hours( i ) + otherTime;

        public static TimeSpan Hours( this int i ) => Hours( ( double )i );

        public static TimeSpan Hours( this int i, TimeSpan otherTime ) => Hours( ( double )i, otherTime );

        public static TimeSpan Millisecond( this double i ) => Milliseconds( i );

        public static TimeSpan Millisecond( this double i, TimeSpan otherTime ) => Milliseconds( i, otherTime );

        public static TimeSpan Millisecond( this int i ) => Millisecond( ( double )i );

        public static TimeSpan Millisecond( this int i, TimeSpan otherTime ) => Milliseconds( ( double )i, otherTime );

        public static TimeSpan Milliseconds( this double i ) => new TimeSpan( ( long )( TimeSpan.TicksPerMillisecond * i ) );

        public static TimeSpan Milliseconds( this double i, TimeSpan otherTime ) => Milliseconds( i ) + otherTime;

        public static TimeSpan Milliseconds( this int i ) => Milliseconds( ( double )i );

        public static TimeSpan Milliseconds( this int i, TimeSpan otherTime ) => Milliseconds( ( double )i, otherTime );

        public static TimeSpan Minute( this double i ) => Minutes( i );

        public static TimeSpan Minute( this double i, TimeSpan otherTime ) => Minutes( i, otherTime );

        public static TimeSpan Minute( this int i ) => Minute( ( double )i );

        public static TimeSpan Minute( this int i, TimeSpan otherTime ) => Minutes( ( double )i, otherTime );

        public static TimeSpan Minutes( this double i ) => TimeSpan.FromMinutes( i );

        public static TimeSpan Minutes( this double i, TimeSpan otherTime ) => Minutes( i ) + otherTime;

        public static TimeSpan Minutes( this int i ) => Minutes( ( double )i );

        public static TimeSpan Minutes( this int i, TimeSpan otherTime ) => Minutes( ( double )i, otherTime );

        public static VariableTimeSpan Month( this int m ) => Months( m );

        public static VariableTimeSpan Month( this int m, VariableTimeSpan otherTime ) => Months( m ) + otherTime;

        public static VariableTimeSpan Months( this int m ) => new VariableTimeSpan( 0, m );

        public static VariableTimeSpan Months( this int m, VariableTimeSpan otherTime ) => Months( m ) + otherTime;

        public static TimeSpan Second( this double i ) => Seconds( i );

        public static TimeSpan Second( this double i, TimeSpan otherTime ) => Seconds( i, otherTime );

        public static TimeSpan Second( this int i ) => Second( ( double )i );

        public static TimeSpan Second( this int i, TimeSpan otherTime ) => Seconds( ( double )i, otherTime );

        public static TimeSpan Seconds( this double i ) => TimeSpan.FromSeconds( i );

        public static TimeSpan Seconds( this double i, TimeSpan otherTime ) => Seconds( i ) + otherTime;

        public static TimeSpan Seconds( this int i ) => Seconds( ( double )i );

        public static TimeSpan Seconds( this int i, TimeSpan otherTime ) => Seconds( ( double )i, otherTime );

        public static TimeSpan Tick( this long i ) => Ticks( i );

        public static TimeSpan Tick( this long i, TimeSpan otherTime ) => Ticks( i, otherTime );

        public static TimeSpan Tick( this int i ) => Tick( ( long )i );

        public static TimeSpan Tick( this int i, TimeSpan otherTime ) => Ticks( i, otherTime );

        public static TimeSpan Ticks( this long i ) => TimeSpan.FromTicks( i );

        public static TimeSpan Ticks( this long i, TimeSpan otherTime ) => Ticks( i ) + otherTime;

        public static TimeSpan Ticks( this int i ) => Ticks( ( long )i );

        public static TimeSpan Ticks( this int i, TimeSpan otherTime ) => Ticks( ( long )i, otherTime );

        public static TimeSpan Week( this double i ) => Weeks( i );

        public static TimeSpan Week( this double i, TimeSpan otherTime ) => Weeks( i, otherTime );

        public static TimeSpan Week( this int i ) => Week( ( double )i );

        public static TimeSpan Week( this int i, TimeSpan otherTime ) => Weeks( ( double )i, otherTime );

        public static TimeSpan Weeks( this double i ) => TimeSpan.FromDays( i * 7 );

        public static TimeSpan Weeks( this double i, TimeSpan otherTime ) => Weeks( i ) + otherTime;

        public static TimeSpan Weeks( this int i ) => Weeks( ( double )i );

        public static TimeSpan Weeks( this int i, TimeSpan otherTime ) => Weeks( ( double )i, otherTime );

        public static VariableTimeSpan Year( this int y ) => Years( y );

        public static VariableTimeSpan Year( this int y, VariableTimeSpan otherTime ) => Years( y ) + otherTime;

        public static VariableTimeSpan Years( this int y ) => new VariableTimeSpan( y, 0 );

        public static VariableTimeSpan Years( this int y, VariableTimeSpan otherTime ) => Years( y ) + otherTime;
    }
}