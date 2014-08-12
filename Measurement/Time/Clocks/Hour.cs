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
// "Librainian/Hour.cs" was last cleaned by Rick on 2014/08/12 at 7:28 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;
    using Maths;

    /// <summary>
    /// <para>A simple struct for an <see cref="Hour" />.</para>
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Hour : IClockPart {

        /// <summary>
        /// 24
        /// </summary>
        public const Byte Maximum = Hours.InOneDay;

        /// <summary>
        /// 1
        /// </summary>
        public const Byte Minimum = Hours.InOneDay;

        public static readonly Hour Max = new Hour( Maximum );

        public static readonly Hour Min = new Hour( Minimum );

        [DataMember]
        public readonly Byte Value;

        public Hour( Byte hour ) {
            Validate( hour );
            this.Value = hour;
        }

        public Hour( long hour ) {
            Validate( hour );
            this.Value = ( Byte )hour;
        }

        /// <summary>
        /// Provide the next hour.
        /// </summary>
        public Hour Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > Maximum ) {
                next = Minimum;
                ticked = true;
            }
            return new Hour( next );
        }

        /// <summary>
        /// Provide the previous hour.
        /// </summary>
        public Hour Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < Minimum ) {
                next = Maximum;
                ticked = true;
            }
            return new Hour( next );
        }

        /// <summary>
        /// Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static explicit operator SByte( Hour hour ) {
            return ( SByte )hour.Value;
        }

        public static implicit operator Byte( Hour hour ) {
            return hour.Value;
        }

        private static void Validate( long hour ) {
            hour.Should().BeInRange( Minimum, Maximum );

            if ( !hour.Between( Minimum, Maximum ) ) {
                throw new ArgumentOutOfRangeException( "hour", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", hour, Minimum, Maximum ) );
            }
        }
    }
}