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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Coordinate64.cs" was last cleaned by Rick on 2014/10/07 at 12:18 PM
#endregion

namespace Librainian.Graphics.DDD {
    using System;
    using System.Data.Linq.Mapping;
    using System.Drawing;
    using System.Runtime.Serialization;
    using Annotations;
    using Maths;
    using Threading;

    /// <summary>
    ///     A 3D point; with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> integers.
    /// </summary>
    /// <remarks>
    ///     Coded towards speed.
    /// </remarks>
    [DataContract]
    public struct Coordinate64 : IEquatable<Coordinate64>, IComparable<Coordinate64> {

        public static readonly Coordinate64 AtMaxValues = new Coordinate64( x: Int64.MaxValue, y: Int64.MaxValue, z: Int64.MaxValue );

        public static readonly Coordinate64 AtMinValues = new Coordinate64( x: Int64.MinValue, y: Int64.MinValue, z: Int64.MinValue );

        public static readonly Coordinate64 Center = new Coordinate64( x: 0, y: 0, z: 0 );

        [Column]
        [DataMember]
        [OptionalField]
        public readonly Int64 Length;

        [Column]
        [DataMember]
        [OptionalField]
        public readonly Int64 X;

        [Column]
        [DataMember]
        [OptionalField]
        public readonly Int64 Y;

        [Column]
        [DataMember]
        [OptionalField]
        public readonly Int64 Z;

        /// <summary>
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <param name="z"> </param>
        public Coordinate64( Int64 x, Int64 y, Int64 z ) {
            this.X = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, x ) );
            this.Y = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, y ) );
            this.Z = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, z ) );
            this.Length = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        ///     Calculates the distance between two <see cref="Coordinate64" />.
        /// </summary>
        public static UInt64 Distance( Coordinate64 lhs, Coordinate64 rhs ) {
            if ( lhs == null ) {
                throw new ArgumentNullException( "lhs" );
            }
            if ( rhs == null ) {
                throw new ArgumentNullException( "rhs" );
            }
            var num1 = lhs.X - rhs.X;
            var num2 = lhs.Y - rhs.Y;
            var num3 = lhs.Z - rhs.Z;
            return ( UInt64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        /// <summary>
        ///     static comparison.
        /// </summary>
        /// <param name="lhs"> </param>
        /// <param name="rhs"> </param>
        /// <returns> </returns>
        public static Boolean Equals( Coordinate64 lhs, Coordinate64 rhs ) {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
        }

        public static implicit operator Point( Coordinate64 coordinate ) {
            return new Point( x: ( int )coordinate.X, y: ( int )coordinate.Y );
        }

        public static implicit operator PointF( Coordinate64 coordinate ) {
            return new PointF( coordinate.X, coordinate.Y );
        }

        /// <summary>
        ///     <para>Returns a new Coordinate as a unit <see cref="Coordinate64" />.</para>
        ///     <para>The result is a Coordinate one unit in length pointing in the same direction as the original Coordinate.</para>
        /// </summary>
        public static Coordinate64 Normalize( Coordinate64 coordinate ) {
            if ( coordinate == null ) {
                throw new ArgumentNullException( "coordinate" );
            }
            var num = 1.0D / coordinate.Length;
            return new Coordinate64( ( Int64 )( coordinate.X * num ), ( Int64 )( coordinate.Y * num ), ( Int64 )( coordinate.Z * num ) );
        }

        public static Coordinate64 operator -( Coordinate64 v1, Coordinate64 v2 ) {
            return new Coordinate64( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );
        }

        public static Boolean operator !=( Coordinate64 lhs, Coordinate64 rhs ) {
            return !Equals( lhs: lhs, rhs: rhs );
        }

        public static Boolean operator ==( Coordinate64 lhs, Coordinate64 rhs ) {
            return Equals( lhs: lhs, rhs: rhs );
        }

        public static Coordinate64 Random() {
            return new Coordinate64( x: Randem.NextInt64(), y: Randem.NextInt64(), z: Randem.NextInt64() );
        }
        ///// <summary>
        /////   Initialize with a random point.
        ///// </summary>
        //public Coordinate() : this( x: Randem.NextInt64(), y: Randem.NextInt64(), z: Randem.NextInt64() ) { }
        /// <summary>
        ///     Compares the current <see cref="Coordinate64" /> with another <see cref="Coordinate64" />.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that indicates the relative order of the objects being compared.
        ///     The return value has the following meanings:
        ///     Value Meaning
        ///     Less than zero: This object is less than the <paramref name="other" /> parameter.
        ///     Zero: This object is equal to <paramref name="other" /> .
        ///     Greater than zero This object is greater than
        ///     <paramref
        ///         name="other" />
        ///     .
        /// </returns>
        /// <param name="other"> An object to compare with this object. </param>
        [Pure]
        public int CompareTo( Coordinate64 other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return this.Length.CompareTo( other.Length );
        }

        /// <summary>
        ///     Calculates the distance between this <see cref="Coordinate64" /> and another <see cref="Coordinate64" />.
        /// </summary>
        public UInt64 Distance( Coordinate64 rhs ) {
            if ( rhs == null ) {
                throw new ArgumentNullException( "rhs" );
            }
            var num1 = this.X - rhs.X;
            var num2 = this.Y - rhs.Y;
            var num3 = this.Z - rhs.Z;
            return ( UInt64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        /// <summary>
        ///     Calls the static comparison.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( Coordinate64 other ) {
            return Equals( this, other );
        }
        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Coordinate64 && Equals( this, ( Coordinate64 )obj );
        }

        /// <summary>
        ///     hash of <see cref="X" />, <see cref="Y" />, and <see cref="Z" />.
        /// //TODO is GetHashCode is only called once for immutable objects..?
        /// </summary>
        public override int GetHashCode() {
            return this.X.GetHashMerge( this.Y.GetHashMerge( this.Z ) );
        }

        public override String ToString() {
            return String.Format( "{0}, {1}, {2}", this.X, this.Y, this.Z );
        }
    }
}
