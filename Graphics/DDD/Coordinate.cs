// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian/Coordinate.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM

namespace Librainian.Graphics.DDD {
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;
    using Maths;
    using Threading;

    /// <summary>
    /// <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="float"/>).</para>
    /// </summary>
    /// <remarks>Coded towards speed.</remarks>
    [DataContract( IsReference = true )]
    public struct Coordinate : IEquatable<Coordinate>, IComparable<Coordinate> {
        public static readonly Coordinate Empty = default( Coordinate );
        public static readonly Coordinate One = new Coordinate( x: 1, y: 1, z: 1 );
        public static readonly Coordinate Zero = new Coordinate( x: 0, y: 0, z: 0 );

        [ DataMember ]
        
        public float X { get; }

        [DataMember]
        
        public readonly Single Y;

        [DataMember]
        
        public readonly Single Z;

        [DataMember]
        
        public Single SquareLength;

        ///// <summary>
        /////   Initialize with a random point.
        ///// </summary>
        //public Coordinate() : this( x: Randem.NextFloat(), y: Randem.NextFloat(), z: Randem.NextFloat() ) { }

        public Coordinate( SingleRange x, SingleRange y, SingleRange z ) : this( x: Randem.NextFloat( x.Min, x.Max ), y: Randem.NextFloat( y.Min, y.Max ), z: Randem.NextFloat( z.Min, z.Max ) ) {
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Coordinate( Single x, Single y, Single z ) {
            this.X = Math.Max( Single.Epsilon, Math.Min( 1, x ) );
            this.Y = Math.Max( Single.Epsilon, Math.Min( 1, y ) );
            this.Z = Math.Max( Single.Epsilon, Math.Min( 1, z ) );
            this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        /// Calculates the distance between two Coordinates.
        /// </summary>
        public static Single Distance( Coordinate lhs, Coordinate rhs ) {
            var num1 = lhs.X - rhs.X;
            var num2 = lhs.Y - rhs.Y;
            var num3 = lhs.Z - rhs.Z;
            return ( Single )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        /// <summary>
        /// static comparison.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean Equals( Coordinate lhs, Coordinate rhs ) {
            if ( lhs.X < rhs.X )
                return false;
            if ( lhs.X > rhs.X )
                return false;
            if ( lhs.Y < rhs.Y )
                return false;
            if ( lhs.Y > rhs.Y )
                return false;
            if ( lhs.Z < rhs.Z )
                return false;
            return !( lhs.Z > rhs.Z );
        }

        public static implicit operator Point( Coordinate coordinate ) => new Point( ( int )coordinate.X, ( int )coordinate.Y );

        public static implicit operator PointF( Coordinate coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        /// Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in
        /// length pointing in the same direction as the original Coordinate.
        /// </summary>
        public static Coordinate Normalize( Coordinate coordinate ) {
            var num = 1.0f / coordinate.SquareLength;
            return new Coordinate( coordinate.X * num, coordinate.Y * num, coordinate.Z * num );
        }

        public static Coordinate operator -( Coordinate v1, Coordinate v2 ) => new Coordinate( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        public static Boolean operator !=( Coordinate lhs, Coordinate rhs ) => !Equals( lhs: lhs, rhs: rhs );

        public static Boolean operator ==( Coordinate lhs, Coordinate rhs ) => Equals( lhs: lhs, rhs: rhs );

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is
        /// less than the <paramref name="other" /> parameter. Zero This object is equal to
        /// <paramref name="other" /> . Greater than zero This object is greater than
        /// <paramref name="other" /> .
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo( Coordinate other ) => this.SquareLength.CompareTo( other.SquareLength );

        public Boolean Equals( Coordinate other ) => Equals( this, other );

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Coordinate && Equals( this, (Coordinate)obj );
        }


        public override int GetHashCode() => this.X.GetHashMerge( this.Y ).GetHashMerge( this.Z );

        public override String ToString() => String.Format( "{0}, {1}, {2}", this.X, this.Y, this.Z );
    }
}