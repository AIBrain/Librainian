// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AtomicInt.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>An integer, thread-safe by <see cref="Interlocked" />.</summary>
    [JsonObject]
    public sealed class AtomicInt {

        /// <summary>ONLY always somtimes used in the getter and setter.</summary>
        [JsonProperty]
        private Int64 _value;

        public AtomicInt( Int32 value = 0 ) {
            this.Value = value;
        }

        public Int32 Value {
            get {

                // this page says to use the CompareExchange
                // http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.THREADING.INTERLOCKED.READ);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true
                //return Interlocked.CompareExchange( ref this._value, 0, 0);
                return ( Int32 )Interlocked.Read( ref this._value );

                //return Interlocked.Exchange( ref this._value, this._value );
            }

            set {
                Interlocked.Exchange( ref this._value, value );
            }
        }

        //public long Increment( long byAmount ) {
        //    return Interlocked.Add( ref this._value, byAmount );
        //}

        //public long Decrement( long byAmount ) {
        //    return Interlocked.Add( ref this._value, -byAmount );
        //}

        public static implicit operator Int32( AtomicInt special ) => special.Value;

        public static AtomicInt operator -( AtomicInt a1, AtomicInt a2 ) => new AtomicInt( a1.Value - a2.Value );

        public static AtomicInt operator *( AtomicInt a1, AtomicInt a2 ) => new AtomicInt( a1.Value * a2.Value );

        public static AtomicInt operator +( AtomicInt a1, AtomicInt a2 ) => new AtomicInt( a1.Value + a2.Value );

        public static AtomicInt operator ++( AtomicInt a1 ) {
            a1.Value++;
            return a1;
        }

        public static AtomicInt Parse( String value ) => new AtomicInt( Int32.Parse( value ) );

        /// <summary>Resets the value to zero if less than zero at this moment in time;</summary>
        public void CheckReset() {
            if ( this.Value < 0 ) {
                this.Value = 0;
            }
        }

        public override String ToString() => $"{this.Value}";
    }
}