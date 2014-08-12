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
// "Librainian/Millisecond.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Clocks;
    using FluentAssertions;

    /// <summary>
    ///     <para>A simple struct for a <see cref="Millisecond" />.</para>
    ///     <para>Should be threadsafe.</para>
    /// </summary>
    [DataContract]
    [Serializable]
    public struct Millisecond : PartofaClock {
        /// <summary>
        ///     60
        /// </summary>
        public const UInt16 Maximum = Milliseconds.InOneSecond;

        /// <summary>
        ///     1
        /// </summary>
        public const Byte Minimum = 1;

        private long _value;

        static Millisecond() {
            Maximum.Should().BeGreaterThan( Minimum );
        }

        public Millisecond( UInt16 millisecond ) : this() {
            this.Set( millisecond );
        }

        [DataMember]
        public UInt16 Value {
            get { return ( UInt16 ) Interlocked.Read( ref this._value ); }
            set {
                value.Should().BeInRange( Minimum, Maximum );

                if ( value < Minimum || value > Maximum ) {
                    throw new ArgumentOutOfRangeException( "value", String.Format( "The specified millisecond {0} is out of the valid range {1} to {2}.", value, Minimum, Maximum ) );
                }
                Interlocked.Exchange( ref this._value, value );
            }
        }

        /// <summary>
        ///     Decrease the current millisecond.
        /// </summary>
        public Boolean Rewind() {
            var value = ( Int32 ) this.Value;
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
        ///     Increase the current millisecond.
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

        public void Set( UInt16 value ) {
            this.Value = value;
        }
    }
}
