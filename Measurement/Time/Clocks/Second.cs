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
// "Librainian/Second.cs" was last cleaned by Rick on 2014/08/11 at 10:22 PM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    ///     A simple struct for a <see cref="ClockExtensions.Second" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public struct Second : IPartofaClock {
        /// <summary>
        ///     60
        /// </summary>
        public const Byte Maximum = Seconds.InOneMinute;

        /// <summary>
        ///     1
        /// </summary>
        public const Byte Minimum = 1;

        [DataMember] private readonly Byte _value;

        public Second( Byte second ) {
            this._value = Validate( second );
        }

        public Second( long second ) {
            this._value = Validate( second );
        }

        /// <summary>
        /// Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static explicit operator SByte( Second second ) {
            return ( SByte )second._value;
        }

        /// <summary>
        /// Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static implicit operator Byte( Second second ) {
            return second._value;
        }

        public static Byte Validate( long second ) {
            second.Should().BeInRange( Minimum, Maximum );

            if ( second < Minimum || second > Maximum ) {
                throw new ArgumentOutOfRangeException( "second", String.Format( "The specified second {0} is out of the valid range {1} to {2}.", second, Minimum, Maximum ) );
            }

            return ( Byte ) second;
        }
    }
}
