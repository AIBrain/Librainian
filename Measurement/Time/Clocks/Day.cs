#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Day.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;
    using Maths;

    /// <summary>
    ///     A simple struct for a Day of the month.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public struct Day : IClockPart {

        public static readonly Byte[] ValidDays = 1.To( Days.InOneMonth ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();


        /// <summary>
        ///     should be 31
        /// </summary>
        public static readonly Byte MaximumValue = ValidDays.Max();

        /// <summary>
        ///     should be 1
        /// </summary>
        public static readonly Byte MinimumValue = ValidDays.Min();

        public static readonly Day Maximum = new Day( MaximumValue );

        public static readonly Day Minimum = new Day( MinimumValue );

        [DataMember] public readonly Byte Value;

        static Day() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        public Day( Byte value ): this() {
            if ( !ValidDays.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( "value", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", value, MinimumValue, MaximumValue ) );
            }
            this.Value = value;
        }


        public static explicit operator SByte( Day value ) {
            return ( SByte )value.Value;
        }

        public static implicit operator Byte( Day value ) {
            return value.Value;
        }

        /// <summary>
        ///     Provide the next <see cref="Day"/>.
        /// </summary>
        public Day Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Day( ( Byte )next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Day"/>.
        /// </summary>
        public Day Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Day( ( Byte )next );
        }
    }
}