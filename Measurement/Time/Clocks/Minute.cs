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
// "Librainian/Minute.cs" was last cleaned by Rick on 2014/08/12 at 12:25 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using Librainian.Extensions;

    /// <summary>
    /// A simple struct for a <see cref="Minute" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Minute : PartialClock {

        /// <summary>
        /// </summary>
        public static readonly Minute MaxMinute = new Minute( Minutes.InOneHour );

        /// <summary>
        /// </summary>
        public static readonly Minute MinMinute = new Minute( Minimum );

        [DataMember]
        private readonly Byte _value;

        public Minute( Byte minute ) {
            this._value = this.Validate( minute );
        }

        public Minute( long minute ) {
            this._value = this.Validate( minute );
        }

        public override byte Maximum { get { return Minutes.InOneHour; } }

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

        public override byte GetValue() {
            return this._value;
        }
    }
}