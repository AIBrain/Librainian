// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "VariableTimeSpan.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "LibrainianCore", File: "VariableTimeSpan.cs" was last formatted by Protiguous on 2020/03/16 at 3:07 PM.

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public struct VariableTimeSpan : IEquatable<VariableTimeSpan> {

        private const Int32 MonthsInYear = 12;

        private readonly Int32 _months;

        private readonly TimeSpan _timeSpan;

        private readonly Int32 _years;

        public VariableTimeSpan( Int32 years, Int32 months ) : this( years, months, TimeSpan.Zero ) { }

        public VariableTimeSpan( Int32 years, Int32 months, TimeSpan timeSpan ) {
            this._years = years + months / MonthsInYear;
            this._months = months % MonthsInYear;
            this._timeSpan = timeSpan;
        }

        public static Boolean operator !=( VariableTimeSpan one, VariableTimeSpan other ) => !( one == other );

        public static VariableTimeSpan operator +( VariableTimeSpan one, VariableTimeSpan other ) => one.AddTo( other );

        public static VariableTimeSpan operator +( TimeSpan timeSpan, VariableTimeSpan v ) => v.AddTo( timeSpan );

        public static VariableTimeSpan operator +( VariableTimeSpan v, TimeSpan timeSpan ) => v.AddTo( timeSpan );

        public static DateTime operator +( VariableTimeSpan span, DateTime dateTime ) => span.AddTo( dateTime );

        public static DateTime operator +( DateTime dateTime, VariableTimeSpan span ) => span.AddTo( dateTime );

        public static DateTimeOffset operator +( VariableTimeSpan span, DateTimeOffset dateTime ) => span.AddTo( dateTime );

        public static DateTimeOffset operator +( DateTimeOffset dateTime, VariableTimeSpan span ) => span.AddTo( dateTime );

        public static Boolean operator ==( VariableTimeSpan one, VariableTimeSpan other ) => one.Equals( other );

        public VariableTimeSpan AddTo( VariableTimeSpan other ) =>
            new VariableTimeSpan( this._years + other._years, this._months + other._months, this._timeSpan + other._timeSpan );

        public VariableTimeSpan AddTo( TimeSpan timeSpan ) => new VariableTimeSpan( this._years, this._months, this._timeSpan + timeSpan );

        public DateTime AddTo( DateTime dateTime ) => dateTime.AddYears( this._years ).AddMonths( this._months ).Add( this._timeSpan );

        public DateTimeOffset AddTo( DateTimeOffset dateTime ) => dateTime.AddYears( this._years ).AddMonths( this._months ).Add( this._timeSpan );

        public DateTime After( DateTime dateTime ) => this.AddTo( dateTime );

        public DateTimeOffset After( DateTimeOffset dateTime ) => this.AddTo( dateTime );

        public Boolean Equals( VariableTimeSpan other ) => this._months == other._months && this._years == other._years && this._timeSpan == other._timeSpan;

        public override Boolean Equals( Object obj ) {
            if ( !( obj is VariableTimeSpan ) ) {
                return default;
            }

            return this.Equals( ( VariableTimeSpan ) obj );
        }

        public override Int32 GetHashCode() => this._months.GetHashCode() ^ this._years.GetHashCode();

    }

}