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
// "Librainian/Second.cs" was last cleaned by Rick on 2014/08/12 at 12:43 AM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Runtime.Serialization;
    using Librainian.Extensions;

    /// <summary>
    ///     A simple struct for a <see cref="Second" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [Immutable]
    public sealed class Second : ClockPart {
        /// <summary>
        ///     60
        /// </summary>
        public static readonly Second MaxSecond = new Second( Seconds.InOneMinute );

        /// <summary>
        /// </summary>
        public static readonly Second MinSecond = new Second( Minimum );

        [DataMember] public readonly Byte Value;

        public Second( Byte second ) {
            this.Value = this.Validate( second );
        }

        public Second( long second ) {
            this.Value = this.Validate( second );
        }

        public override byte Maximum { get { return Seconds.InOneMinute; } }

        /// <summary>
        ///     Allow this class to be visibly cast to a <see cref="SByte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static explicit operator SByte( Second second ) {
            return ( SByte ) second.Value;
        }

        /// <summary>
        ///     Allow this class to be read as a <see cref="Byte" />.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static implicit operator Byte( Second second ) {
            return second.Value;
        }

        public override byte GetValue() {
            return this.Value;
        }

        /// <summary>
        ///     <para>Returns a new decremented <seealso cref="Second" />.</para>
        ///     <para>Returns true if the value passed <see cref="Second.Minimum" /></para>
        /// </summary>
        public static Boolean Backward( ref Second second ) {
            var value = ( int ) second.Value;
            value--;
            if ( value < MinSecond.Value ) {
                second = MaxSecond;
                return true;
            }
            second = new Second( value );
            return false;
        }

        /// <summary>
        ///     <para>Returns a new incremented <seealso cref="Second" />.</para>
        ///     <para>Returns true if the value passed <see cref="Second.Maximum" /></para>
        /// </summary>
        public static Boolean Forward( ref Second second ) {
            var value = ( int ) second.Value;
            value++;
            if ( value > MaxSecond.Value ) {
                second = MinSecond;
                return true;
            }
            second = new Second( value );
            return false;
        }
    }
}
