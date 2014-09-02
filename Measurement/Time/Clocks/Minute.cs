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
// "Librainian/Minute.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    ///     A simple struct for a <see cref="Minute" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Minute : IClockPart {
        public static readonly Byte[] ValidMinutes = Enumerable.Range( 0, Minutes.InOneHour ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        /// <summary>
        ///    should be 23
        /// </summary>
        public static readonly Byte MaximumValue = ValidMinutes.Max();

        /// <summary>
        ///   should be 0
        /// </summary>
        public static readonly Byte MinimumValue = ValidMinutes.Min();

        public static readonly Hour MaximumMinute = new Hour( MaximumValue );

        public static readonly Hour MinimumMinute = new Hour( MinimumValue );

        static Minute() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        [DataMember]
        public readonly Byte Value;

        public Minute( Byte value ) {
            if ( !ValidMinutes.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( "value", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", value, MinimumValue, MaximumValue ) );
            }
            this.Value = value;
        }

     
        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static explicit operator SByte( Minute minute ) {
            return ( SByte )minute.Value;
        }

        /// <summary>
        ///     Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static implicit operator Byte( Minute minute ) {
            return minute.Value;
        }

        /// <summary>
        ///     Provide the next minute.
        /// </summary>
        public Minute Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > Maximum ) {
                next = Minimum;
                ticked = true;
            }
            return new Minute( next );
        }

        /// <summary>
        ///     Provide the previous minute.
        /// </summary>
        public Minute Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < Minimum ) {
                next = Maximum;
                ticked = true;
            }
            return new Minute( next );
        }

        private static void Validate( long minute ) {
            minute.Should().BeInRange( Minimum, Maximum );

            if ( minute < 1 || minute > Maximum ) {
                throw new ArgumentOutOfRangeException( "minute", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", minute, Minimum, Maximum ) );
            }
        }
    }
}