// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CoordinateF.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "CoordinateF.cs" was last formatted by Protiguous on 2019/08/08 at 7:37 AM.

#pragma warning disable RCS1138 // Add summary to documentation comment.

namespace Librainian.Graphics.DDD {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Maths.Ranges;
    using Newtonsoft.Json;
    using static Maths.Hashings.HashingExtensions;

#pragma warning disable IDE0015 // Use framework type

    /// <summary>
    ///     <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="Single" />).</para>
    /// </summary>
    /// <remarks>Code towards speed.</remarks>
    [Immutable]
#pragma warning restore IDE0015 // Use framework type
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( MemberSerialization.Fields )]
    public class CoordinateF : IEquatable<CoordinateF>, IComparable<CoordinateF> {

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
        public Int32 CompareTo( [NotNull] CoordinateF other ) => this.SquareLength.CompareTo( other.SquareLength );

        public Boolean Equals( CoordinateF other ) => Equals( this, other );

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
        public CoordinateF( SingleRange x, SingleRange y, SingleRange z ) : this( x: Randem.NextFloat( x.Min, x.Max ), y: Randem.NextFloat( y.Min, y.Max ),
            z: Randem.NextFloat( z.Min, z.Max ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateF( Single x, Single y, Single z ) {
            this.X = Math.Max( Single.Epsilon, Math.Min( 1, x ) );
            this.Y = Math.Max( Single.Epsilon, Math.Min( 1, y ) );
            this.Z = Math.Max( Single.Epsilon, Math.Min( 1, z ) );
            this.SquareLength = ( this.X * this.X ) + ( this.Y * this.Y ) + ( this.Z * this.Z );
        }

        /// <summary>
        ///     Calculates the distance between two Coordinates.
        /// </summary>
        public static Single Distance( [NotNull] CoordinateF left, [NotNull] CoordinateF rhs ) {
            var num1 = left.X - rhs.X;
            var num2 = left.Y - rhs.Y;
            var num3 = left.Z - rhs.Z;

            return ( Single )Math.Sqrt( ( num1 * num1 ) + ( num2 * num2 ) + ( num3 * num3 ) );
        }

        /// <summary>
        ///     static comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rhs"> </param>
        /// <returns></returns>
        public static Boolean Equals( CoordinateF left, CoordinateF rhs ) {
            if ( ReferenceEquals( left, rhs ) ) {
                return true;
            }

            if ( left is null ) {
                return false;
            }

            if ( rhs is null ) {
                return false;
            }

            if ( left.X < rhs.X ) {
                return false;
            }

            if ( left.X > rhs.X ) {
                return false;
            }

            if ( left.Y < rhs.Y ) {
                return false;
            }

            if ( left.Y > rhs.Y ) {
                return false;
            }

            if ( left.Z < rhs.Z ) {
                return false;
            }

            return !( left.Z > rhs.Z );
        }

        public static implicit operator Point( [NotNull] CoordinateF coordinate ) => new Point( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

        public static implicit operator PointF( [NotNull] CoordinateF coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        ///     Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in length pointing in the same
        ///     direction as the original Coordinate.
        /// </summary>
        [NotNull]
        public static CoordinateF Normalize( [NotNull] CoordinateF coordinate ) {
            var num = 1.0f / coordinate.SquareLength;

            return new CoordinateF( coordinate.X * num, coordinate.Y * num, coordinate.Z * num );
        }

        [NotNull]
        public static CoordinateF operator -( [NotNull] CoordinateF v1, [NotNull] CoordinateF v2 ) => new CoordinateF( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        public static Boolean operator !=( [CanBeNull] CoordinateF left, [CanBeNull] CoordinateF rhs ) => !Equals( left: left, rhs: rhs );

        public static Boolean operator ==( [CanBeNull] CoordinateF left, [CanBeNull] CoordinateF rhs ) => Equals( left: left, rhs: rhs );

        public Double DistanceTo( [CanBeNull] CoordinateF to ) {
            if ( to == default ) {
                return 0; //BUG ?
            }

            var dx = this.X - to.X;
            var dy = this.Y - to.Y;
            var dz = this.Z - to.Z;

            return Math.Sqrt( ( dx * dx ) + ( dy * dy ) + ( dz * dz ) );
        }

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            return obj is CoordinateF f && Equals( this, f );
        }

        public override Int32 GetHashCode() => GetHashCodes( this.X, this.Y, this.Z );

        public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
    }
}