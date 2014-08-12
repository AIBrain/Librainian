namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    /// A simple struct for a <see cref="Millisecond" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Millisecond : IClockPart {

        /// <summary>
        /// 60
        /// </summary>
        public static readonly Millisecond Max = new Millisecond( Milliseconds.InOneSecond );

        /// <summary>
        /// </summary>
        public static readonly Millisecond Min = new Millisecond( 1 );

        [DataMember]
        public readonly UInt16 Value;

        public Millisecond( Byte second ) {
            ( ( long ) second ).Should().BeInRange( 1, this.Maximum );

            if ( ( long ) second < 1 || ( long ) second > this.Maximum ) {
                throw new ArgumentOutOfRangeException( "quantity", String.Format( "The specified quantity ({0}) is out of the valid range {1} to {2}.", ( long ) second, ( byte ) 1, this.Maximum ) );
            }

            this.Value = ( Byte ) second;
        }

        public Millisecond( long second ) {
            second.Should().BeInRange( 1, this.Maximum );

            if ( second < 1 || second > this.Maximum ) {
                throw new ArgumentOutOfRangeException( "quantity", String.Format( "The specified quantity ({0}) is out of the valid range {1} to {2}.", second, ( byte ) 1, this.Maximum ) );
            }

            this.Value = ( Byte ) second;
        }

        /// <summary>
        /// Provide the next second.
        /// </summary>
        public Millisecond Next {
            get {
                var next = this.Value + 1;
                if ( next > this.Maximum ) {
                    next = 1;
                }
                return new Millisecond( next );
            }
        }

        /// <summary>
        /// Provide the previous minute.
        /// </summary>
        public Millisecond Previous {
            get {
                var next = this.Value - 1;
                if ( next < 1 ) {
                    next = this.Maximum;
                }
                return new Millisecond( next );
            }
        }

        protected override byte Maximum { get { return Seconds.InOneMinute; } }

        /// <summary>
        /// Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static explicit operator SByte( Millisecond second ) {
            return ( SByte )second.Value;
        }

        /// <summary>
        /// Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static implicit operator Byte( Millisecond second ) {
            return second.Value;
        }
    }
}