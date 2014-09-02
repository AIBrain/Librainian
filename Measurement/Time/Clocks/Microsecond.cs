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
// "Librainian/Microsecond.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;
    using Maths;

    /// <summary>
    ///     A simple struct for a <see cref="Microsecond" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Microsecond : IClockPart {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 Maximum = Microseconds.InOneMillisecond;

        /// <summary>
        ///     1
        /// </summary>
        public const Byte Minimum = 1;

        /// <summary>
        ///     60
        /// </summary>
        public static readonly Microsecond Max = new Microsecond( Maximum );

        /// <summary>
        ///     1
        /// </summary>
        public static readonly Microsecond Min = new Microsecond( Minimum );

        [DataMember]
        public readonly UInt16 Value;

        public Microsecond( UInt16 microsecond ) {
            Validate( microsecond );
            this.Value = microsecond;
        }

        public Microsecond( long microsecond ) {
            Validate( microsecond );
            this.Value = ( UInt16 )microsecond;
        }

        /// <summary>
        ///     Provide the next microsecond.
        /// </summary>
        public Microsecond Next {
            get {
                var next = this.Value + 1;
                if ( next > Maximum ) {
                    next = Minimum;
                }
                return new Microsecond( next );
            }
        }

        /// <summary>
        ///     Provide the previous microsecond.
        /// </summary>
        public Microsecond Previous {
            get {
                var next = this.Value - 1;
                if ( next < Minimum ) {
                    next = Maximum;
                }
                return new Microsecond( next );
            }
        }

        /// <summary>
        ///     Allow this class to be visibly cast to an <see cref="Int16" />.
        /// </summary>
        /// <param name="microsecond"></param>
        /// <returns></returns>
        public static explicit operator Int16( Microsecond microsecond ) {
            return ( Int16 )microsecond.Value;
        }

        /// <summary>
        ///     Allow this class to be read as a <see cref="UInt16" />.
        /// </summary>
        /// <param name="microsecond"></param>
        /// <returns></returns>
        public static implicit operator UInt16( Microsecond microsecond ) {
            return microsecond.Value;
        }

        private static void Validate( long microsecond ) {
            microsecond.Should().BeInRange( 1, Maximum );

            if ( !microsecond.Between( Minimum, Maximum ) ) {
                throw new ArgumentOutOfRangeException( "microsecond", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", microsecond, Minimum, Maximum ) );
            }
        }
    }
}