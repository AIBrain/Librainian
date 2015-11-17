#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Microsecond.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;

    /// <summary>A simple struct for a <see cref="Microsecond" />.</summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [Immutable]
    public sealed class Microsecond : IClockPart {
        public static readonly UInt16[] ValidMicroseconds = Enumerable.Range( 0, Microseconds.InOneMillisecond ).Select( u => ( UInt16 )u ).OrderBy( u => u ).ToArray();

        /// <summary>999</summary>
        public static readonly UInt16 MaximumValue = ValidMicroseconds.Max();

        /// <summary>0</summary>
        public static readonly UInt16 MinimumValue = ValidMicroseconds.Min();

        /// <summary></summary>
        public static readonly Microsecond Maxium = new Microsecond( MaximumValue );

        /// <summary></summary>
        public static readonly Microsecond Minimum = new Microsecond( MinimumValue );

        [DataMember]
        public readonly UInt16 Value;

        static Microsecond() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        public Microsecond(UInt16 value) {
            if ( !ValidMicroseconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>Allow this class to be visibly cast to an <see cref="Int16" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Int16(Microsecond value) => ( Int16 )value.Value;

        public static implicit operator UInt16(Microsecond value) => value.Value;

        /// <summary>Provide the next <see cref="Microsecond" />.</summary>
        public Microsecond Next(out Boolean ticked) {
            ticked = false;
            var next = this.Value + 1;
            if ( next <= MaximumValue ) {
                return new Microsecond( ( UInt16 )next );
            }
            next = MinimumValue;
            ticked = true;
            return new Microsecond( ( UInt16 )next );
        }

        /// <summary>Provide the previous <see cref="Microsecond" />.</summary>
        public Microsecond Previous(out Boolean ticked) {
            ticked = false;
            var next = this.Value - 1;
            if ( next >= MinimumValue ) {
                return new Microsecond( ( UInt16 )next );
            }
            next = MaximumValue;
            ticked = true;
            return new Microsecond( ( UInt16 )next );
        }
    }
}