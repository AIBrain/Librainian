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
// "Librainian/Month.cs" was last cleaned by Rick on 2014/09/02 at 10:57 AM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;

    /// <summary>
    ///     A simple struct for a <see cref="Month" />.
    /// </summary>
    [ DataContract ]
    [ Serializable ]
    public sealed class Month : IClockPart {
        public static readonly Byte[] ValidMonths = Enumerable.Range( 1, Months.InOneCommonYear + 1 ).Select( i => ( Byte ) i ).OrderBy( b => b ).ToArray();

        /// <summary>
        ///     12
        /// </summary>
        public static readonly Byte MaximumValue = ValidMonths.Max();

        /// <summary>
        ///     1
        /// </summary>
        public static readonly Byte MinimumValue = ValidMonths.Min();

        [DataMember]
        public readonly Byte Value;

        static Month() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        public Month( Byte value ) {
            if ( !ValidMonths.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", value, MinimumValue, MaximumValue ) );
            }
            this.Value = value;
        }

        /// <summary>
        ///     Provide the next <see cref="Month" />.
        /// </summary>
        public Month Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Month( ( Byte ) next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Month" />.
        /// </summary>
        public Month Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Month( ( Byte ) next );
        }
    }
}
