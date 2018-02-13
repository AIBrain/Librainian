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
// "Librainian/Coordinate.cs" was last cleaned by Rick on 2016/07/08 at 9:28 AM

namespace Librainian.Graphics.DDD {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using Extensions;
    using Maths;
    using Maths.Ranges;
    using Newtonsoft.Json;
    using static Maths.MathHashing;

    /// <summary>
    ///     <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="Single" />).</para>
    /// </summary>
    /// <remarks>Code towards speed.</remarks>
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( MemberSerialization.Fields )]
    public class CoordinateF : IEquatable<CoordinateF>, IComparable<CoordinateF> {

        /// <summary>
        /// Initialize with a random point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateF( SingleRange x, SingleRange y, SingleRange z ) : this( x: Randem.NextFloat( x.Min, x.Max ), y: Randem.NextFloat( y.Min, y.Max ), z: Randem.NextFloat( z.Min, z.Max ) ) {
        }

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

        public static CoordinateF Empty { get; }

        public static CoordinateF One { get; } = new CoordinateF( x: 1, y: 1, z: 1 );

        public static CoordinateF Zero { get; } = new CoordinateF( x: 0, y: 0, z: 0 );

        [JsonProperty]
        public Single SquareLength {
            get;
        }

        [JsonProperty]
        public Single X {
            get;
        }

        [JsonProperty]
        public Single Y {
            get;
        }

        [JsonProperty]
        public Single Z {
            get;
        }

        /// <summary>
        ///     Calculates the distance between two Coordinates.
        /// </summary>
        public static Single Distance( CoordinateF lhs, CoordinateF rhs ) {
            var num1 = lhs.X - rhs.X;
            var num2 = lhs.Y - rhs.Y;
            var num3 = lhs.Z - rhs.Z;
            return ( Single )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        public Double DistanceTo( CoordinateF to ) {
            if ( to == default ) {
                return 0;   //BUG ?
            }
            var dx = this.X - to.X;
            var dy = this.Y - to.Y;
            var dz = this.Z - to.Z;
            return Math.Sqrt( dx * dx + dy * dy + dz * dz );
        }

        /// <summary>
        ///     static comparison.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Equals( CoordinateF lhs, CoordinateF rhs ) {
            if ( ReferenceEquals( lhs, rhs ) ) {
                return true;
            }
            if ( lhs is null ) {
                return false;
            }
            if ( rhs is null ) {
                return false;
            }
            if ( lhs.X < rhs.X ) {
                return false;
            }
            if ( lhs.X > rhs.X ) {
                return false;
            }
            if ( lhs.Y < rhs.Y ) {
                return false;
            }
            if ( lhs.Y > rhs.Y ) {
                return false;
            }
            if ( lhs.Z < rhs.Z ) {
                return false;
            }
            return !( lhs.Z > rhs.Z );
        }

        public static implicit operator Point( CoordinateF coordinate ) => new Point( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

        public static implicit operator PointF( CoordinateF coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        ///     Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in
        ///     length pointing in the same direction as the original Coordinate.
        /// </summary>
        public static CoordinateF Normalize( CoordinateF coordinate ) {
            var num = 1.0f / coordinate.SquareLength;
            return new CoordinateF( coordinate.X * num, coordinate.Y * num, coordinate.Z * num );
        }

        public static CoordinateF operator -( CoordinateF v1, CoordinateF v2 ) => new CoordinateF( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        public static Boolean operator !=( CoordinateF lhs, CoordinateF rhs ) => !Equals( lhs: lhs, rhs: rhs );

        public static Boolean operator ==( CoordinateF lhs, CoordinateF rhs ) => Equals( lhs: lhs, rhs: rhs );

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared.
        ///     The return value has the following meanings: Value Meaning Less than zero This object is
        ///     less than the <paramref name="other" /> parameter. Zero This object is equal to
        ///     <paramref name="other" /> . Greater than zero This object is greater than
        ///     <paramref name="other" /> .
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( CoordinateF other ) => this.SquareLength.CompareTo( other.SquareLength );

        public Boolean Equals( CoordinateF other ) => Equals( this, other );

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