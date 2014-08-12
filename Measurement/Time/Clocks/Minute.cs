#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian/Minute.cs" was last cleaned by Rick on 2014/08/11 at 10:15 PM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    /// A simple struct for a <see cref="Minute" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public struct Minute : IPartofaClock {

        /// <summary>
        /// 60
        /// </summary>
        public const Byte Maximum = Minutes.InOneHour;

        /// <summary>
        /// 1
        /// </summary>
        public const Byte Minimum = 1;

        /// <summary>
        /// </summary>
        public static readonly Minute MaxMinute = new Minute( Maximum );

        /// <summary>
        /// </summary>
        public static readonly Minute MinMinute = new Minute( Minimum );

        [DataMember]
        private readonly Byte _value;

        public Minute( Byte minute ) {
            this._value = Validate( minute );
        }

        public Minute( long minute ) {
            this._value = Validate( minute );
        }

        /// <summary>
        /// Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static explicit operator SByte( Minute minute ) {
            return ( SByte )minute._value;
        }

        /// <summary>
        /// Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static implicit operator Byte( Minute minute ) {
            return minute._value;
        }

        private static Byte Validate( long minute ) {
            minute.Should().BeInRange( Minimum, Maximum );

            if ( minute < Minimum || minute > Maximum ) {
                throw new ArgumentOutOfRangeException( "minute", String.Format( "The specified minute {0} is out of the valid range {1} to {2}.", minute, Minimum, Maximum ) );
            }

            return ( Byte )minute;
        }
    }
}