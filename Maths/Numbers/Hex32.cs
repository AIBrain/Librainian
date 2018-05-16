// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Hex32.cs",
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
// "Librainian/Librainian/Hex32.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using Extensions;

    /// <summary>
    ///     Wrapper for Int32 that represents a hexadecimal number
    /// </summary>
    [Immutable]
    public struct Hex32 : IEquatable<Hex32>, IComparable<Hex32> {

        /// <summary>
        ///     Gets or sets the int value.
        /// </summary>
        /// <value>The int value.</value>
        public readonly Int32 Value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Hex32" /> struct.
        /// </summary>
        /// <param name="i">The i.</param>
        public Hex32( Int32 i ) => this.Value = i;

        /// <summary>
        ///     Hex32s the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public static implicit operator Hex32( Int32 i ) => new Hex32( i );

        /// <summary>
        ///     Int32s the specified h.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public static implicit operator Int32( Hex32 h ) => h.Value;

        /// <summary>
        ///     -s the specified h1.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator -( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value - h2.Value );

        /// <summary>
        ///     Checks if h1 is not equal to h2
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator !=( Hex32 h1, Hex32 h2 ) => !h1.Equals( h2 );

        /// <summary>
        ///     *s the specified h1.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator *( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value * h2.Value );

        /// <summary>
        ///     /s the specified h1.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator /( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value / h2.Value );

        /// <summary>
        ///     +s the specified h1.
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Hex32 operator +( Hex32 h1, Hex32 h2 ) => new Hex32( h1.Value + h2.Value );

        /// <summary>
        ///     Checks if h1 is smaller than h2
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator <( Hex32 h1, Hex32 h2 ) => h1.CompareTo( h2 ) < 0;

        /// <summary>
        ///     Checks if h1 is equal to h2
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator ==( Hex32 h1, Hex32 h2 ) => h1.Equals( h2 );

        /// <summary>
        ///     Checks if h1 is greater then h2
        /// </summary>
        /// <param name="h1">The h1.</param>
        /// <param name="h2">The h2.</param>
        /// <returns></returns>
        public static Boolean operator >( Hex32 h1, Hex32 h2 ) => h1.CompareTo( h2 ) > 0;

        public Int32 CompareTo( Hex32 other ) => this.Value.CompareTo( other.Value );

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override Boolean Equals( Object obj ) => obj is Hex32 hex32 && this.Equals( hex32 );

        /// <summary>
        ///     Gibt an, ob das aktuelle Objekt einem anderen Objekt des gleichen Typs entspricht.
        /// </summary>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        /// <returns>true, wenn das aktuelle Objekt gleich dem <paramref name="other" />-Parameter ist, andernfalls false.</returns>
        public Boolean Equals( Hex32 other ) => this.Value.Equals( other.Value );

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override Int32 GetHashCode() => this.Value * 0x00010000 + this.Value;

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override String ToString() => "0x" + this.Value.ToString( "X" );
    }
}