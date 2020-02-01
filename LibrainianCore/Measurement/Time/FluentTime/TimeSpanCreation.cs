// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeSpanCreation.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "TimeSpanCreation.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace LibrainianCore.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class TimeSpanCreation {

        public static TimeSpan Day( this Double i ) => Days( i );

        public static TimeSpan Day( this Double i, TimeSpan otherTime ) => Days( i, otherTime );

        public static TimeSpan Day( this Int32 i ) => Day( ( Double )i );

        public static TimeSpan Day( this Int32 i, TimeSpan otherTime ) => Days( ( Double )i, otherTime );

        public static TimeSpan Days( this Double i ) => TimeSpan.FromDays( i );

        public static TimeSpan Days( this Double i, TimeSpan otherTime ) => Days( i ) + otherTime;

        public static TimeSpan Days( this Int32 i ) => Days( ( Double )i );

        public static TimeSpan Days( this Int32 i, TimeSpan otherTime ) => Days( ( Double )i, otherTime );

        public static TimeSpan Hour( this Double i ) => Hours( i );

        public static TimeSpan Hour( this Double i, TimeSpan otherTime ) => Hours( i, otherTime );

        public static TimeSpan Hour( this Int32 i ) => Hour( ( Double )i );

        public static TimeSpan Hour( this Int32 i, TimeSpan otherTime ) => Hours( ( Double )i, otherTime );

        public static TimeSpan Hours( this Double i ) => TimeSpan.FromHours( i );

        public static TimeSpan Hours( this Double i, TimeSpan otherTime ) => Hours( i ) + otherTime;

        public static TimeSpan Hours( this Int32 i ) => Hours( ( Double )i );

        public static TimeSpan Hours( this Int32 i, TimeSpan otherTime ) => Hours( ( Double )i, otherTime );

        public static TimeSpan Millisecond( this Double i ) => Milliseconds( i );

        public static TimeSpan Millisecond( this Double i, TimeSpan otherTime ) => Milliseconds( i, otherTime );

        public static TimeSpan Millisecond( this Int32 i ) => Millisecond( ( Double )i );

        public static TimeSpan Millisecond( this Int32 i, TimeSpan otherTime ) => Milliseconds( ( Double )i, otherTime );

        public static TimeSpan Milliseconds( this Double i ) => new TimeSpan( ( Int64 )( TimeSpan.TicksPerMillisecond * i ) );

        public static TimeSpan Milliseconds( this Double i, TimeSpan otherTime ) => Milliseconds( i ) + otherTime;

        public static TimeSpan Milliseconds( this Int32 i ) => Milliseconds( ( Double )i );

        public static TimeSpan Milliseconds( this Int32 i, TimeSpan otherTime ) => Milliseconds( ( Double )i, otherTime );

        public static TimeSpan Minute( this Double i ) => Minutes( i );

        public static TimeSpan Minute( this Double i, TimeSpan otherTime ) => Minutes( i, otherTime );

        public static TimeSpan Minute( this Int32 i ) => Minute( ( Double )i );

        public static TimeSpan Minute( this Int32 i, TimeSpan otherTime ) => Minutes( ( Double )i, otherTime );

        public static TimeSpan Minutes( this Double i ) => TimeSpan.FromMinutes( i );

        public static TimeSpan Minutes( this Double i, TimeSpan otherTime ) => Minutes( i ) + otherTime;

        public static TimeSpan Minutes( this Int32 i ) => Minutes( ( Double )i );

        public static TimeSpan Minutes( this Int32 i, TimeSpan otherTime ) => Minutes( ( Double )i, otherTime );

        public static VariableTimeSpan Month( this Int32 m ) => Months( m );

        public static VariableTimeSpan Month( this Int32 m, VariableTimeSpan otherTime ) => Months( m ) + otherTime;

        public static VariableTimeSpan Months( this Int32 m ) => new VariableTimeSpan( 0, m );

        public static VariableTimeSpan Months( this Int32 m, VariableTimeSpan otherTime ) => Months( m ) + otherTime;

        public static TimeSpan Second( this Double i ) => Seconds( i );

        public static TimeSpan Second( this Double i, TimeSpan otherTime ) => Seconds( i, otherTime );

        public static TimeSpan Second( this Int32 i ) => Second( ( Double )i );

        public static TimeSpan Second( this Int32 i, TimeSpan otherTime ) => Seconds( ( Double )i, otherTime );

        public static TimeSpan Seconds( this Double i ) => TimeSpan.FromSeconds( i );

        public static TimeSpan Seconds( this Double i, TimeSpan otherTime ) => Seconds( i ) + otherTime;

        public static TimeSpan Seconds( this Int32 i ) => Seconds( ( Double )i );

        public static TimeSpan Seconds( this Int32 i, TimeSpan otherTime ) => Seconds( ( Double )i, otherTime );

        public static TimeSpan Tick( this Int64 i ) => Ticks( i );

        public static TimeSpan Tick( this Int64 i, TimeSpan otherTime ) => Ticks( i, otherTime );

        public static TimeSpan Tick( this Int32 i ) => Tick( ( Int64 )i );

        public static TimeSpan Tick( this Int32 i, TimeSpan otherTime ) => Ticks( i, otherTime );

        public static TimeSpan Ticks( this Int64 i ) => TimeSpan.FromTicks( i );

        public static TimeSpan Ticks( this Int64 i, TimeSpan otherTime ) => Ticks( i ) + otherTime;

        public static TimeSpan Ticks( this Int32 i ) => Ticks( ( Int64 )i );

        public static TimeSpan Ticks( this Int32 i, TimeSpan otherTime ) => Ticks( ( Int64 )i, otherTime );

        public static TimeSpan Week( this Double i ) => Weeks( i );

        public static TimeSpan Week( this Double i, TimeSpan otherTime ) => Weeks( i, otherTime );

        public static TimeSpan Week( this Int32 i ) => Week( ( Double )i );

        public static TimeSpan Week( this Int32 i, TimeSpan otherTime ) => Weeks( ( Double )i, otherTime );

        public static TimeSpan Weeks( this Double i ) => TimeSpan.FromDays( i * 7 );

        public static TimeSpan Weeks( this Double i, TimeSpan otherTime ) => Weeks( i ) + otherTime;

        public static TimeSpan Weeks( this Int32 i ) => Weeks( ( Double )i );

        public static TimeSpan Weeks( this Int32 i, TimeSpan otherTime ) => Weeks( ( Double )i, otherTime );

        public static VariableTimeSpan Year( this Int32 y ) => Years( y );

        public static VariableTimeSpan Year( this Int32 y, VariableTimeSpan otherTime ) => Years( y ) + otherTime;

        public static VariableTimeSpan Years( this Int32 y ) => new VariableTimeSpan( y, 0 );

        public static VariableTimeSpan Years( this Int32 y, VariableTimeSpan otherTime ) => Years( y ) + otherTime;
    }
}