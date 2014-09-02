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
// "Librainian/Second.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    ///     A simple struct for a <see cref="Second" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Second : IClockPart {
        public const Byte Maximum = Seconds.InOneMinute;

        public const Byte Minimum = 1;

        /// <summary>
        ///     60
        /// </summary>
        public static readonly Second Max = new Second( Seconds.InOneMinute );

        /// <summary>
        /// </summary>
        public static readonly Second Min = new Second( 1 );

        [DataMember]
        public readonly Byte Value;

        public Second( Byte second ) {
            Validate( second );
            this.Value = second;
        }

        public Second( long second ) {
            Validate( second );
            this.Value = ( Byte )second;
        }

        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static explicit operator SByte( Second second ) {
            return ( SByte )second.Value;
        }

        /// <summary>
        ///     Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static implicit operator Byte( Second second ) {
            return second.Value;
        }

        /// <summary>
        ///     Provide the next second.
        /// </summary>
        public Second Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > Maximum ) {
                next = Minimum;
                ticked = true;
            }
            return new Second( next );
        }

        /// <summary>
        ///     Provide the previous second.
        /// </summary>
        public Second Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < Minimum ) {
                next = Maximum;
                ticked = true;
            }
            return new Second( next );
        }

        private static void Validate( long second ) {
            second.Should().BeInRange( 1, Maximum );

            if ( second < 1 || second > Maximum ) {
                throw new ArgumentOutOfRangeException( "second", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", second, Minimum, Maximum ) );
            }
        }
    }
}