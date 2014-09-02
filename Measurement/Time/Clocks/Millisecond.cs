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
// "Librainian/Millisecond.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;
    using Maths;

    /// <summary>
    ///     A simple struct for a <see cref="Millisecond" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Millisecond : IClockPart {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 Maximum = Milliseconds.InOneSecond;

        /// <summary>
        ///     1
        /// </summary>
        public const Byte Minimum = 1;

        /// <summary>
        ///     1000
        /// </summary>
        public static readonly Millisecond Max = new Millisecond( Maximum );

        /// <summary>
        ///     1
        /// </summary>
        public static readonly Millisecond Min = new Millisecond( Minimum );

        [DataMember]
        public readonly UInt16 Value;

        public Millisecond( UInt16 millisecond ) {
            Validate( millisecond );
            this.Value = millisecond;
        }

        public Millisecond( long millisecond ) {
            Validate( millisecond );
            this.Value = ( UInt16 )millisecond;
        }

        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="Int16" />.
        /// </summary>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static explicit operator Int16( Millisecond millisecond ) {
            return ( Int16 )millisecond.Value;
        }

        /// <summary>
        ///     Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static implicit operator UInt16( Millisecond millisecond ) {
            return millisecond.Value;
        }

        /// <summary>
        ///     Provide the next millisecond.
        /// </summary>
        public Millisecond Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > Maximum ) {
                next = Minimum;
                ticked = true;
            }

            return new Millisecond( next );
        }

        /// <summary>
        ///     Provide the previous millisecond.
        /// </summary>
        public Millisecond Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < Minimum ) {
                ticked = true;
                next = Maximum;
            }
            return new Millisecond( next );
        }

        private static void Validate( long millisecond ) {
            millisecond.Should().BeInRange( 1, Maximum );

            if ( !millisecond.Between( Minimum, Maximum ) ) {
                throw new ArgumentOutOfRangeException( "millisecond", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", millisecond, Minimum, Maximum ) );
            }
        }
    }
}