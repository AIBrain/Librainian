// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CoordinateF.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/CoordinateF.cs" was last formatted by Protiguous on 2018/05/24 at 7:11 PM.

namespace Librainian.Graphics.DDD {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using Extensions;
    using Maths;
    using Maths.Ranges;
    using Newtonsoft.Json;
    using static Maths.Hashing;

    /// <summary>
    ///     <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="float" />).</para>
    /// </summary>
    /// <remarks>Code towards speed.</remarks>
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( MemberSerialization.Fields )]
    public class CoordinateF : IEquatable<CoordinateF>, IComparable<CoordinateF> {

        public static CoordinateF Empty { get; }

        public static CoordinateF One { get; } = new CoordinateF( x: 1, y: 1, z: 1 );

        public static CoordinateF Zero { get; } = new CoordinateF( x: 0, y: 0, z: 0 );

        [JsonProperty]
        public Single SquareLength { get; }

        [JsonProperty]
        public Single X { get; }

        [JsonProperty]
        public Single Y { get; }

        [JsonProperty]
        public Single Z { get; }

        /// <summary>
        ///     Initialize with a random point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateF( SingleRange x, SingleRange y, SingleRange z ) : this( x: Randem.NextFloat( x.Min, x.Max ), y: Randem.NextFloat( y.Min, y.Max ), z: Randem.NextFloat( z.Min, z.Max ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateF( Single x, Single y, Single z ) {
            this.X = Math.Max( Single.Epsilon, Math.Min( 1, x ) );
            this.Y = Math.Max( Single.Epsilon, Math.Min( 1, y ) );
            this.Z = Math.Max( Single.Epsilon, Math.Min( 1, z ) );
            this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        ///     Calculates the distance between two Coordinates.
        /// </summary>
        public static Single Distance( CoordinateF left, CoordinateF rhs ) {
            var num1 = left.X - rhs.X;
            var num2 = left.Y - rhs.Y;
            var num3 = left.Z - rhs.Z;

            return ( Single )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        /// <summary>
        ///     static comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rhs"> </param>
        /// <returns></returns>
        public static Boolean Equals( CoordinateF left, CoordinateF rhs ) {
            if ( ReferenceEquals( left, rhs ) ) { return true; }

            if ( left is null ) { return false; }

            if ( rhs is null ) { return false; }

            if ( left.X < rhs.X ) { return false; }

            if ( left.X > rhs.X ) { return false; }

            if ( left.Y < rhs.Y ) { return false; }

            if ( left.Y > rhs.Y ) { return false; }

            if ( left.Z < rhs.Z ) { return false; }

            return !( left.Z > rhs.Z );
        }

        public static implicit operator Point( CoordinateF coordinate ) => new Point( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

        public static implicit operator PointF( CoordinateF coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        ///     Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in length pointing in the same
        ///     direction as the original Coordinate.
        /// </summary>
        public static CoordinateF Normalize( CoordinateF coordinate ) {
            var num = 1.0f / coordinate.SquareLength;

            return new CoordinateF( coordinate.X * num, coordinate.Y * num, coordinate.Z * num );
        }

        public static CoordinateF operator -( CoordinateF v1, CoordinateF v2 ) => new CoordinateF( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        public static Boolean operator !=( CoordinateF left, CoordinateF rhs ) => !Equals( left: left, rhs: rhs );

        public static Boolean operator ==( CoordinateF left, CoordinateF rhs ) => Equals( left: left, rhs: rhs );

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
        ///     following meanings: Value Meaning Less than zero This object is less than the
        ///     <paramref
        ///         name="other" />
        ///     parameter. Zero This object is equal to <paramref name="other" /> . Greater than zero This object is greater than
        ///     <paramref name="other" /> .
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( CoordinateF other ) => this.SquareLength.CompareTo( other.SquareLength );

        public Double DistanceTo( CoordinateF to ) {
            if ( to == default ) {
                return 0; //BUG ?
            }

            var dx = this.X - to.X;
            var dy = this.Y - to.Y;
            var dz = this.Z - to.Z;

            return Math.Sqrt( dx * dx + dy * dy + dz * dz );
        }

        public Boolean Equals( CoordinateF other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is CoordinateF f && Equals( this, f );
        }

        public override Int32 GetHashCode() => GetHashCodes( this.X, this.Y, this.Z );

        public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
    }
}