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
// "Librainian/TimeSpanCreation.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class TimeSpanCreation {

        public static TimeSpan Day(this Double i) => Days( i );

        public static TimeSpan Day(this Double i, TimeSpan otherTime) => Days( i, otherTime );

        public static TimeSpan Day(this Int32 i) => Day( ( Double )i );

        public static TimeSpan Day(this Int32 i, TimeSpan otherTime) => Days( ( Double )i, otherTime );

        public static TimeSpan Days(this Double i) => TimeSpan.FromDays( i );

        public static TimeSpan Days(this Double i, TimeSpan otherTime) => Days( i ) + otherTime;

        public static TimeSpan Days(this Int32 i) => Days( ( Double )i );

        public static TimeSpan Days(this Int32 i, TimeSpan otherTime) => Days( ( Double )i, otherTime );

        public static TimeSpan Hour(this Double i) => Hours( i );

        public static TimeSpan Hour(this Double i, TimeSpan otherTime) => Hours( i, otherTime );

        public static TimeSpan Hour(this Int32 i) => Hour( ( Double )i );

        public static TimeSpan Hour(this Int32 i, TimeSpan otherTime) => Hours( ( Double )i, otherTime );

        public static TimeSpan Hours(this Double i) => TimeSpan.FromHours( i );

        public static TimeSpan Hours(this Double i, TimeSpan otherTime) => Hours( i ) + otherTime;

        public static TimeSpan Hours(this Int32 i) => Hours( ( Double )i );

        public static TimeSpan Hours(this Int32 i, TimeSpan otherTime) => Hours( ( Double )i, otherTime );

        public static TimeSpan Millisecond(this Double i) => Milliseconds( i );

        public static TimeSpan Millisecond(this Double i, TimeSpan otherTime) => Milliseconds( i, otherTime );

        public static TimeSpan Millisecond(this Int32 i) => Millisecond( ( Double )i );

        public static TimeSpan Millisecond(this Int32 i, TimeSpan otherTime) => Milliseconds( ( Double )i, otherTime );

        public static TimeSpan Milliseconds(this Double i) => new TimeSpan( ( Int64 )( TimeSpan.TicksPerMillisecond * i ) );

        public static TimeSpan Milliseconds(this Double i, TimeSpan otherTime) => Milliseconds( i ) + otherTime;

        public static TimeSpan Milliseconds(this Int32 i) => Milliseconds( ( Double )i );

        public static TimeSpan Milliseconds(this Int32 i, TimeSpan otherTime) => Milliseconds( ( Double )i, otherTime );

        public static TimeSpan Minute(this Double i) => Minutes( i );

        public static TimeSpan Minute(this Double i, TimeSpan otherTime) => Minutes( i, otherTime );

        public static TimeSpan Minute(this Int32 i) => Minute( ( Double )i );

        public static TimeSpan Minute(this Int32 i, TimeSpan otherTime) => Minutes( ( Double )i, otherTime );

        public static TimeSpan Minutes(this Double i) => TimeSpan.FromMinutes( i );

        public static TimeSpan Minutes(this Double i, TimeSpan otherTime) => Minutes( i ) + otherTime;

        public static TimeSpan Minutes(this Int32 i) => Minutes( ( Double )i );

        public static TimeSpan Minutes(this Int32 i, TimeSpan otherTime) => Minutes( ( Double )i, otherTime );

        public static VariableTimeSpan Month(this Int32 m) => Months( m );

        public static VariableTimeSpan Month(this Int32 m, VariableTimeSpan otherTime) => Months( m ) + otherTime;

        public static VariableTimeSpan Months(this Int32 m) => new VariableTimeSpan( 0, m );

        public static VariableTimeSpan Months(this Int32 m, VariableTimeSpan otherTime) => Months( m ) + otherTime;

        public static TimeSpan Second(this Double i) => Seconds( i );

        public static TimeSpan Second(this Double i, TimeSpan otherTime) => Seconds( i, otherTime );

        public static TimeSpan Second(this Int32 i) => Second( ( Double )i );

        public static TimeSpan Second(this Int32 i, TimeSpan otherTime) => Seconds( ( Double )i, otherTime );

        public static TimeSpan Seconds(this Double i) => TimeSpan.FromSeconds( i );

        public static TimeSpan Seconds(this Double i, TimeSpan otherTime) => Seconds( i ) + otherTime;

        public static TimeSpan Seconds(this Int32 i) => Seconds( ( Double )i );

        public static TimeSpan Seconds(this Int32 i, TimeSpan otherTime) => Seconds( ( Double )i, otherTime );

        public static TimeSpan Tick(this Int64 i) => Ticks( i );

        public static TimeSpan Tick(this Int64 i, TimeSpan otherTime) => Ticks( i, otherTime );

        public static TimeSpan Tick(this Int32 i) => Tick( ( Int64 )i );

        public static TimeSpan Tick(this Int32 i, TimeSpan otherTime) => Ticks( i, otherTime );

        public static TimeSpan Ticks(this Int64 i) => TimeSpan.FromTicks( i );

        public static TimeSpan Ticks(this Int64 i, TimeSpan otherTime) => Ticks( i ) + otherTime;

        public static TimeSpan Ticks(this Int32 i) => Ticks( ( Int64 )i );

        public static TimeSpan Ticks(this Int32 i, TimeSpan otherTime) => Ticks( ( Int64 )i, otherTime );

        public static TimeSpan Week(this Double i) => Weeks( i );

        public static TimeSpan Week(this Double i, TimeSpan otherTime) => Weeks( i, otherTime );

        public static TimeSpan Week(this Int32 i) => Week( ( Double )i );

        public static TimeSpan Week(this Int32 i, TimeSpan otherTime) => Weeks( ( Double )i, otherTime );

        public static TimeSpan Weeks(this Double i) => TimeSpan.FromDays( i * 7 );

        public static TimeSpan Weeks(this Double i, TimeSpan otherTime) => Weeks( i ) + otherTime;

        public static TimeSpan Weeks(this Int32 i) => Weeks( ( Double )i );

        public static TimeSpan Weeks(this Int32 i, TimeSpan otherTime) => Weeks( ( Double )i, otherTime );

        public static VariableTimeSpan Year(this Int32 y) => Years( y );

        public static VariableTimeSpan Year(this Int32 y, VariableTimeSpan otherTime) => Years( y ) + otherTime;

        public static VariableTimeSpan Years(this Int32 y) => new VariableTimeSpan( y, 0 );

        public static VariableTimeSpan Years(this Int32 y, VariableTimeSpan otherTime) => Years( y ) + otherTime;
    }
}