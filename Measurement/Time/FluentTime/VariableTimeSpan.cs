// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "VariableTimeSpan.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/VariableTimeSpan.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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

        public VariableTimeSpan AddTo( VariableTimeSpan other ) => new VariableTimeSpan( this._years + other._years, this._months + other._months, this._timeSpan + other._timeSpan );

        public VariableTimeSpan AddTo( TimeSpan timeSpan ) => new VariableTimeSpan( this._years, this._months, this._timeSpan + timeSpan );

        public DateTime AddTo( DateTime dateTime ) => dateTime.AddYears( this._years ).AddMonths( this._months ).Add( this._timeSpan );

        public DateTimeOffset AddTo( DateTimeOffset dateTime ) => dateTime.AddYears( this._years ).AddMonths( this._months ).Add( this._timeSpan );

        public DateTime After( DateTime dateTime ) => this.AddTo( dateTime );

        public DateTimeOffset After( DateTimeOffset dateTime ) => this.AddTo( dateTime );

        public Boolean Equals( VariableTimeSpan other ) => this._months == other._months && this._years == other._years && this._timeSpan == other._timeSpan;

        public override Boolean Equals( Object obj ) {
            if ( !( obj is VariableTimeSpan ) ) { return false; }

            return this.Equals( ( VariableTimeSpan )obj );
        }

        public override Int32 GetHashCode() => this._months.GetHashCode() ^ this._years.GetHashCode();
    }
}