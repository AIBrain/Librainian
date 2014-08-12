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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Millisecond.cs" was last cleaned by Rick on 2014/08/12 at 7:39 AM

#endregion License & Information

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
        public const Byte Maximum = Seconds.InOneMinute;
        public const Byte Minimum = 1;

        /// <summary>
        /// 60
        /// </summary>
        public static readonly Millisecond Max = new Millisecond( Maximum );

        /// <summary>
        /// 1
        /// </summary>
        public static readonly Millisecond Min = new Millisecond( Minimum );

        [DataMember]
        public readonly UInt16 Value;

        public Millisecond( UInt16 second ) {
            Validate( second );
            this.Value = second;
        }

        public Millisecond( long second ) {
            Validate( second );
            this.Value = ( UInt16 )second;
        }

        /// <summary>
        /// Provide the next millisecond.
        /// </summary>
        public Millisecond Next {
            get {
                var next = this.Value + 1;
                if ( next > Maximum ) {
                    next = Minimum;
                }
                return new Millisecond( next );
            }
        }

        /// <summary>
        /// Provide the previous millisecond.
        /// </summary>
        public Millisecond Previous {
            get {
                var next = this.Value - 1;
                if ( next < Minimum ) {
                    next = Maximum;
                }
                return new Millisecond( next );
            }
        }

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
        public static implicit operator UInt16( Millisecond second ) {
            return second.Value;
        }

        private static void Validate( long millisecond ) {
            millisecond.Should().BeInRange( 1, Maximum );

            if ( millisecond < 1 || millisecond > Maximum ) {
                throw new ArgumentOutOfRangeException( "millisecond", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", millisecond, Minimum, Maximum ) );
            }
        }
    }
}