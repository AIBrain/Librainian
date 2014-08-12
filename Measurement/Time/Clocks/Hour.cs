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
// "Librainian/Hour.cs" was last cleaned by Rick on 2014/08/12 at 12:21 AM

#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.Serialization;
    using Librainian.Extensions;

    /// <summary>
    /// <para>A simple struct for an <see cref="Hour" />.</para>
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Hour : PartialClock {
        public static readonly Hour MaxHour = new Hour( Hours.InOneDay );
        public static readonly Hour MinHour = new Hour( Minimum );

        [DataMember]
        private readonly Byte _value;

        public Hour( Byte hour ) {
            this._value = this.Validate( hour );
        }

        public Hour( long hour ) {
            this._value = this.Validate( hour );
        }

        /// <summary>
        /// 24
        /// </summary>
        public override byte Maximum { get { return Hours.InOneDay; } }

        /// <summary>
        /// Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static explicit operator SByte( Hour hour ) {
            return ( SByte )hour._value;
        }

        public static implicit operator Byte( Hour hour ) {
            return hour._value;
        }

        public override byte GetValue() {
            return this._value;
        }
    }
}