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
// "Librainian2/Minute.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using FluentAssertions;

    /// <summary>
    ///     A simple struct for a <see cref="Minute" />.
    /// </summary>
    [DataContract]
    [Serializable]
    public struct Minute : IPartofaClock {
        /// <summary>
        ///     60
        /// </summary>
        public const Byte Maximum = Minutes.InOneHour;

        /// <summary>
        ///     1
        /// </summary>
        public const Byte Minimum = 1;

        private long _value;

        static Minute() {
            Maximum.Should().BeGreaterThan( Minimum );
        }

        public Minute( Byte minute ) : this() {
            this.Set( minute );
        }

        [DataMember]
        public Byte Value {
            get { return ( Byte ) Interlocked.Read( ref this._value ); }
            set {
                value.Should().BeInRange( Minimum, Maximum );

                if ( value < Minimum || value > Maximum ) {
                    throw new ArgumentOutOfRangeException( "value", String.Format( "The specified minute {0} is out of the valid range {1} to {2}.", value, Minimum, Maximum ) );
                }
                Interlocked.Exchange( ref this._value, value );
            }
        }

        /// <summary>
        ///     <para>Decrease the current minute.</para>
        ///     <para>Returns true if the value passed <see cref="Maximum" /></para>
        /// </summary>
        public Boolean Rewind() {
            var value = ( Int16 ) this.Value;
            value--;
            if ( value < Minimum ) {
                this.Value = Maximum;
                return true;
            }
            this.Value = ( Byte ) value;
            return false;
        }

        public void Set( Byte value ) {
            this.Value = value;
        }

        /// <summary>
        ///     <para>Increase the current minute.</para>
        ///     <para>Returns true if the value passed <see cref="Maximum" /></para>
        /// </summary>
        public Boolean Tick() {
            var value = this.Value;
            value++;
            if ( value > Maximum ) {
                this.Value = Minimum;
                return true;
            }
            this.Value = value;
            return false;
        }
    }
}
