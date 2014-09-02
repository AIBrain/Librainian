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
    using System.Linq;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Librainian.Extensions;

    /// <summary>
    ///     A simple struct for a <see cref="Millisecond" />.
    /// </summary>
        [DataContract( IsReference = true )]
    [Serializable]
    [Immutable]
    public sealed class Millisecond : IClockPart {

        public static readonly UInt16[] ValidMilliseconds = Enumerable.Range( 0, Milliseconds.InOneSecond ).Select( u => ( UInt16 ) u ).OrderBy( u => u ).ToArray();

        /// <summary>
        ///     999
        /// </summary>
        public static readonly UInt16 MaximumValue = ValidMilliseconds.Max();

        /// <summary>
        ///     0
        /// </summary>
        public static readonly UInt16 MinimumValue = ValidMilliseconds.Min();

        /// <summary>
        ///     
        /// </summary>
        public static readonly Millisecond Maxium = new Millisecond( MaximumValue );

        /// <summary>
        ///     
        /// </summary>
        public static readonly Millisecond Minimum = new Millisecond( MinimumValue );

        [DataMember]
        public readonly UInt16 Value;

        static Millisecond() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        public Millisecond( UInt16 value ) {
            if ( !ValidMilliseconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( "value", String.Format( "The specified value ({0}) is out of the valid range of {1} to {2}.", value, MinimumValue, MaximumValue ) );
            }
            this.Value = value;
        }


        /// <summary>
        ///     Allow this class to be visibly cast to an <see cref="Int16" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Int16( Millisecond value ) {
            return ( Int16 )value.Value;
        }

        public static implicit operator UInt16( Millisecond value ) {
            return value.Value;
        }

        /// <summary>
        ///     Provide the next <see cref="Millisecond"/>.
        /// </summary>
        public Millisecond Next( out Boolean ticked ) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Millisecond( ( UInt16 )next );
        }

        /// <summary>
        ///     Provide the previous <see cref="Millisecond"/>.
        /// </summary>
        public Millisecond Previous( out Boolean ticked ) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Millisecond( ( UInt16 )next );
        }
    }
}