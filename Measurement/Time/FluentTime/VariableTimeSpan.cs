namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public struct VariableTimeSpan : IEquatable<VariableTimeSpan> {
        private const int MonthsInYear = 12;

        private readonly int _months;
        private readonly TimeSpan _timeSpan;
        private readonly int _years;

        public VariableTimeSpan( int years, int months ) : this( years, months, TimeSpan.Zero ) {
        }

        public VariableTimeSpan( int years, int months, TimeSpan timeSpan ) {
            this._years = years + ( months / MonthsInYear );
            this._months = months % MonthsInYear;
            this._timeSpan = timeSpan;
        }

        public static bool operator !=( VariableTimeSpan one, VariableTimeSpan other ) => !( one == other );

        public static VariableTimeSpan operator +( VariableTimeSpan one, VariableTimeSpan other ) => one.AddTo( other );

        public static VariableTimeSpan operator +( TimeSpan timeSpan, VariableTimeSpan v ) => v.AddTo( timeSpan );

        public static VariableTimeSpan operator +( VariableTimeSpan v, TimeSpan timeSpan ) => v.AddTo( timeSpan );

        public static DateTime operator +( VariableTimeSpan span, DateTime dateTime ) => span.AddTo( dateTime );

        public static DateTime operator +( DateTime dateTime, VariableTimeSpan span ) => span.AddTo( dateTime );

        public static DateTimeOffset operator +( VariableTimeSpan span, DateTimeOffset dateTime ) => span.AddTo( dateTime );

        public static DateTimeOffset operator +( DateTimeOffset dateTime, VariableTimeSpan span ) => span.AddTo( dateTime );

        public static bool operator ==( VariableTimeSpan one, VariableTimeSpan other ) => one.Equals( other );

        public VariableTimeSpan AddTo( VariableTimeSpan other ) => new VariableTimeSpan( this._years + other._years, this._months + other._months, this._timeSpan + other._timeSpan );

        public VariableTimeSpan AddTo( TimeSpan timeSpan ) => new VariableTimeSpan( this._years, this._months, this._timeSpan + timeSpan );

        public DateTime AddTo( DateTime dateTime ) => dateTime.AddYears( this._years ).AddMonths( this._months ).Add( this._timeSpan );

        public DateTimeOffset AddTo( DateTimeOffset dateTime ) => dateTime.AddYears( this._years ).AddMonths( this._months ).Add( this._timeSpan );

        public DateTime After( DateTime dateTime ) => this.AddTo( dateTime );

        public DateTimeOffset After( DateTimeOffset dateTime ) => this.AddTo( dateTime );

        public bool Equals( VariableTimeSpan other ) => this._months == other._months && this._years == other._years && this._timeSpan == other._timeSpan;

        public override bool Equals( object obj ) {
            if ( !( obj is VariableTimeSpan ) )
                return false;
            return this.Equals( ( VariableTimeSpan )obj );
        }

        public override int GetHashCode() => this._months.GetHashCode() ^ this._years.GetHashCode();
    }
}