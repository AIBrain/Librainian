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
// "Librainian/Hour.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    ///     <para>A simple struct for an <see cref="Hour" />.</para>
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Hour : IClockPart {

        public static readonly Byte[] ValidHours = Enumerable.Range( 0, Hours.InOneDay ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        /// <summary>
        ///    should be 23
        /// </summary>
        public static readonly Byte MaximumValue = ValidHours.Max();

        /// <summary>
        ///   should be 0
        /// </summary>
        public static readonly Byte MinimumValue = ValidHours.Min();

        public static readonly Hour Maximum = new Hour( MaximumValue );

        public static readonly Hour Minimum = new Hour( MinimumValue );

        static Hour() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        [DataMember]
        public readonly Byte Value;

        public Hour( Byte value ) {
            if ( !ValidHours.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( "value", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", value, MinimumValue, MaximumValue ) );
            }
            this.Value = value;
        }

        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte( Hour value ) => ( SByte )value.Value;

        public static implicit operator Byte( Hour value ) => value.Value;

        /// <summary>
        ///     Provide the next <see cref="Hour"/>.
        /// </summary>
        public Hour Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Hour( ( Byte )next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Hour"/>.
        /// </summary>
        public Hour Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Hour( ( Byte )next );
        }
    }
}