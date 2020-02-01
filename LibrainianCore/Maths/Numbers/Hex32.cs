// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Hex32.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Hex32.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace LibrainianCore.Maths.Numbers {

    using System;
    using Extensions;

    /// <summary>Wrapper for Int32 that represents a hexadecimal number</summary>
    [Immutable]
    public struct Hex32 : IEquatable<Hex32>, IComparable<Hex32> {

        /// <summary>Gets or sets the int value.</summary>
        /// <value>The int value.</value>
        public readonly Int32 Value;

        /// <summary>Initializes a new instance of the <see cref="Hex32" /> struct.</summary>
        /// <param name="i">The i.</param>
        public Hex32( Int32 i ) => this.Value = i;

        /// <summary>Hex32s the specified i.</summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public static implicit operator Hex32( Int32 i ) => new Hex32( i );

        /// <summary>Int32s the specified h.</summary>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public static implicit operator Int32( Hex32 h ) => h.Value;

        /// <summary>-s the specified h1.</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator -( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value - h2.Value );

        /// <summary>Checks if h1 is not equal to h2</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator !=( Hex32 h1, Hex32 h2 ) => !h1.Equals( h2 );

        /// <summary>*s the specified h1.</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator *( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value * h2.Value );

        /// <summary>/s the specified h1.</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator /( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value / h2.Value );

        /// <summary>+s the specified h1.</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator +( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value + h2.Value );

        /// <summary>Checks if h1 is smaller than h2</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator <( Hex32 h1, Hex32 h2 ) => h1.CompareTo( h2 ) < 0;

        /// <summary>Checks if h1 is equal to h2</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator ==( Hex32 h1, Hex32 h2 ) => h1.Equals( h2 );

        /// <summary>Checks if h1 is greater then h2</summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator >( Hex32 h1, Hex32 h2 ) => h1.CompareTo( h2 ) > 0;

        public Int32 CompareTo( Hex32 other ) => this.Value.CompareTo( other.Value );

        /// <summary>Determines whether the specified <see cref="Object" /> is equal to this instance.</summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override Boolean Equals( Object obj ) => obj is Hex32 hex32 && this.Equals( hex32 );

        /// <summary>Gibt an, ob das aktuelle Objekt einem anderen Objekt des gleichen Typs entspricht.</summary>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        /// <returns>true, wenn das aktuelle Objekt gleich dem <paramref name="other" />-Parameter ist, andernfalls false.</returns>
        public Boolean Equals( Hex32 other ) => this.Value.Equals( other.Value );

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override Int32 GetHashCode() => this.Value * 0x00010000 + this.Value;

        /// <summary>Returns a <see cref="String" /> that represents this instance.</summary>
        /// <returns>A <see cref="String" /> that represents this instance.</returns>
        public override String ToString() => "0x" + this.Value.ToString( "X" );
    }
}