namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    ///     A simple struct for a <see cref="Second" />.
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [Immutable]
    public sealed class Second : IClockPart {
        public static readonly Byte[] ValidSeconds = Enumerable.Range( 0, Seconds.InOneMinute ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        /// <summary>
        ///    should be 59
        /// </summary>
        public static readonly Byte MaximumValue = ValidSeconds.Max();

        /// <summary>
        ///   should be 0
        /// </summary>
        public static readonly Byte MinimumValue = ValidSeconds.Min();

        public static readonly Hour Maximum = new Hour( MaximumValue );

        public static readonly Hour Minimum = new Hour( MinimumValue );

        static Second() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        [DataMember]
        public readonly Byte Value;

        public Second( Byte value ) {
            if ( !ValidSeconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( "value", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", value, MinimumValue, MaximumValue ) );
            }
            this.Value = value;
        }

     
        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte( Second value ) {
            return ( SByte )value.Value;
        }

        /// <summary>
        ///     Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Byte( Second value ) {
            return value.Value;
        }

        /// <summary>
        ///     Provide the next minute.
        /// </summary>
        public Second Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Second( ( byte ) next );
        }

        /// <summary>
        ///     Provide the previous minute.
        /// </summary>
        public Second Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Second( ( byte ) next );
        }
    }
}