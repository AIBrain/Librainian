// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "AtomicInt.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/AtomicInt.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

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

        public Int32 Value {
            get => ( Int32 )Interlocked.Read( ref this._value );

            set => Interlocked.Exchange( ref this._value, value );
        }

        public AtomicInt( Int32 value = 0 ) => this.Value = value;

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
            if ( this.Value < 0 ) { this.Value = 0; }
        }

        public override String ToString() => $"{this.Value}";
    }
}