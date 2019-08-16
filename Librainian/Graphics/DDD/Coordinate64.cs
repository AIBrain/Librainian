// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Coordinate64.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Coordinate64.cs" was last formatted by Protiguous on 2019/08/08 at 7:37 AM.

#pragma warning disable RCS1138 // Add summary to documentation comment.

namespace Librainian.Graphics.DDD {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Maths.Hashings;
    using Newtonsoft.Json;

#pragma warning disable IDE0015 // Use framework type

    /// <summary>
    ///     A 3D point; with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /><see cref="long" /> integers.
    /// </summary>
    /// <remarks>Code towards speed.</remarks>
    [Immutable]
#pragma warning restore IDE0015 // Use framework type
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( MemberSerialization.Fields )]
    public class Coordinate64 : IEquatable<Coordinate64>, IComparable<Coordinate64> {

        [Pure]
        public Int32 CompareTo( [NotNull] Coordinate64 other ) => this.Length.CompareTo( other.Length );

        /// <summary>
        ///     Calls the static comparison.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( Coordinate64 other ) => Equals( this, other );

        [JsonProperty( "h" )]
        private readonly Int32 _hashCode;

        [JsonProperty( "L" )]
        public Int64 Length { get; private set; }

        [JsonProperty( "X" )]
        public Int64 X { get; private set; }

        [JsonProperty( "Y" )]
        public Int64 Y { get; private set; }

        [JsonProperty( "Z" )]
        public Int64 Z { get; private set; }

        public static readonly Coordinate64 Maximum = new Coordinate64( x: Int64.MaxValue, y: Int64.MaxValue, z: Int64.MaxValue );

        public static readonly Coordinate64 Minimum = new Coordinate64( x: Int64.MinValue, y: Int64.MinValue, z: Int64.MinValue );

        public static readonly Coordinate64 Zero = new Coordinate64( x: 0, y: 0, z: 0 );

        public static Coordinate64 Backward = new Coordinate64( 0, 0, 1 );

        public static Coordinate64 Down = new Coordinate64( 0, -1, 0 );

        public static Coordinate64 Forward = new Coordinate64( x: 0, y: 0, z: -1 );

        public static Coordinate64 Left = new Coordinate64( -1, 0, 0 );

        public static Coordinate64 One = new Coordinate64( x: 1, y: 1, z: 1 );

        public static Coordinate64 Right = new Coordinate64( 1, 0, 0 );

        public static Coordinate64 UnitX = new Coordinate64( x: 1, y: 0, z: 0 );

        public static Coordinate64 UnitY = new Coordinate64( x: 0, y: 1, z: 0 );

        public static Coordinate64 UnitZ = new Coordinate64( 0, 0, 1 );

        public static Coordinate64 Up = new Coordinate64( 0, 1, 0 );

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Coordinate64( Int64 x, Int64 y, Int64 z ) {
            this.X = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, x ) );
            this.Y = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, y ) );
            this.Z = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, z ) );
            this.Length = (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
            this._hashCode = HashingExtensions.GetHashCodes( this.X, this.Y, this.Z );
        }

        /// <summary>
        ///     Calculates the distance between two <see cref="Coordinate64" />.
        /// </summary>
        public static Int64 Distance( [NotNull] Coordinate64 left, [NotNull] Coordinate64 rhs ) {
            var num1 = left.X - rhs.X;
            var num2 = left.Y - rhs.Y;
            var num3 = left.Z - rhs.Z;

            return ( Int64 ) Math.Sqrt( (num1 * num1) + (num2 * num2) + (num3 * num3) );
        }

        /// <summary>
        ///     static comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rhs"> </param>
        /// <returns></returns>
        public static Boolean Equals( Coordinate64 left, Coordinate64 rhs ) {
            if ( ReferenceEquals( left, rhs ) ) {
                return true;
            }

            if ( left == null ) {
                return false;
            }

            if ( rhs == null ) {
                return false;
            }

            return left.X == rhs.X && left.Y == rhs.Y && left.Z == rhs.Z;
        }

        public static explicit operator Point( [NotNull] Coordinate64 coordinate ) => new Point( x: ( Int32 ) coordinate.X, y: ( Int32 ) coordinate.Y );

        public static explicit operator PointF( [NotNull] Coordinate64 coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        ///     Allow an explicit conversion from <see cref="Coordinate64" /> to a <see cref="Vector3" />.
        /// </summary>
        /// <param name="bob"></param>
        public static explicit operator Vector3( [NotNull] Coordinate64 bob ) => new Vector3( bob.X, bob.Y, bob.Z );

        /// <summary>
        ///     <para>Returns a new Coordinate64 as a unit <see cref="Coordinate64" />.</para>
        ///     <para>The result is a Coordinate64 one unit in length pointing in the same direction as the original Coordinate64.</para>
        /// </summary>
        [NotNull]
        public static Coordinate64 Normalize( [NotNull] Coordinate64 coordinate ) {
            var num = 1.0D / coordinate.Length;

            return new Coordinate64( ( Int64 ) ( coordinate.X * num ), ( Int64 ) ( coordinate.Y * num ), ( Int64 ) ( coordinate.Z * num ) );
        }

        [NotNull]
        public static Coordinate64 operator -( [NotNull] Coordinate64 v1, [NotNull] Coordinate64 v2 ) => new Coordinate64( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        public static Boolean operator !=( [CanBeNull] Coordinate64 left, [CanBeNull] Coordinate64 rhs ) => !Equals( left: left, rhs: rhs );

        public static Boolean operator ==( [CanBeNull] Coordinate64 left, [CanBeNull] Coordinate64 rhs ) => Equals( left: left, rhs: rhs );

        [NotNull]
        public static Coordinate64 Random() => new Coordinate64( x: Randem.NextInt64(), y: Randem.NextInt64(), z: Randem.NextInt64() );

        /// <summary>
        ///     Calculates the distance between this <see cref="Coordinate64" /> and another <see cref="Coordinate64" />.
        /// </summary>
        public Int64 Distance( [NotNull] Coordinate64 to ) => Distance( this, to );

        public override Boolean Equals( Object obj ) {
            if ( obj == null ) {
                return false;
            }

            return obj is Coordinate64 coordinate64 && Equals( this, coordinate64 );
        }

        /// <summary>
        ///     precomputed hash of <see cref="X" />, <see cref="Y" />, and <see cref="Z" />.
        /// </summary>
        public override Int32 GetHashCode() => this._hashCode;

        public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
    }
}