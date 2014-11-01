namespace Librainian.Graphics.DDD {
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;
    using Annotations;
    using Maths;
    using Threading;

    /// <summary>
    ///     A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> .
    /// </summary>
    /// <remarks>
    ///     Coded towards speed.
    /// </remarks>
    [DataContract( IsReference = true )]
    public struct CoordinateU64 : IEquatable<CoordinateU64>, IComparable<CoordinateU64> {
        /// <summary>
        ///     The smallest value a <see cref="CoordinateU64" /> will hold.
        /// </summary>
        public const UInt64 Minimum = 1 + UInt64.MinValue; //TODO why is this not Zero ?

        /// <summary>
        ///     The largest value a <see cref="CoordinateU64" /> will hold.
        /// </summary>
        /// <remarks>
        ///     the squareroot of <see cref="ulong.MaxValue" /> split into x,y,z.
        /// </remarks>
        public static readonly UInt64 Maximum = ( UInt64 )( Math.Sqrt( UInt64.MaxValue ) / 3.0f );

        public static readonly CoordinateU64 Empty = default( CoordinateU64 );

        public static readonly CoordinateU64 MaxValue = new CoordinateU64( x: UInt64.MaxValue, y: UInt64.MaxValue, z: UInt64.MaxValue );

        public static readonly CoordinateU64 MinValue = new CoordinateU64( x: Minimum, y: Minimum, z: Minimum );

        /// <summary>
        ///     Maximum - Minimum
        /// </summary>
        public static readonly UInt64 Range = Maximum - Minimum;

        [DataMember]
        [OptionalField]
        public readonly UInt64 SquareLength;

        [DataMember]
        [OptionalField]
        public readonly UInt64 X;

        [DataMember]
        [OptionalField]
        public readonly UInt64 Y;

        [DataMember]
        [OptionalField]
        public readonly UInt64 Z;

        ///// <summary>
        /////   Initialize with a random point.
        ///// </summary>
        //public Coordinate() : this( x: Randem.NextFloat(), y: Randem.NextFloat(), z: Randem.NextFloat() ) { }

        public CoordinateU64( UInt64Range x ) : this( x: Randem.Next( x.Min, x.Max ), y: Randem.Next( Minimum, Maximum ), z: Randem.Next( Minimum, Maximum ) ) {
        }

        public CoordinateU64( UInt64Range x, UInt64Range y, UInt64Range z ) : this( Randem.Next( x.Min, x.Max ), y: Randem.Next( y.Min, y.Max ), z: Randem.Next( z.Min, z.Max ) ) {
        }

        /// <summary>
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <param name="z"> </param>
        public CoordinateU64( UInt64 x, UInt64 y, UInt64 z ) {
            this.X = Math.Max( Minimum, Math.Min( Maximum, x ) );
            this.Y = Math.Max( Minimum, Math.Min( Maximum, y ) );
            this.Z = Math.Max( Minimum, Math.Min( Maximum, z ) );
            this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>
        ///     Compares the current <see cref="CoordinateU64" /> with another <see cref="CoordinateU64" />.
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
        public int CompareTo( CoordinateU64 other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return this.SquareLength.CompareTo( other.SquareLength );
        }

        /// <summary>
        ///     Calls the static comparison.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( CoordinateU64 other ) {
            return Equals( this, other );
        }

        /// <summary>
        ///     static comparison.
        /// </summary>
        /// <param name="lhs"> </param>
        /// <param name="rhs"> </param>
        /// <returns> </returns>
        public static Boolean Equals( CoordinateU64 lhs, CoordinateU64 rhs ) {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
        }

        /// <summary>
        ///     <para>Returns a new Coordinate as a unit <see cref="CoordinateU64" />.</para>
        ///     <para>The result is a Coordinate one unit in length pointing in the same direction as the original Coordinate.</para>
        /// </summary>
        public static CoordinateU64 Normalize( CoordinateU64 coordinate ) {
            if ( coordinate == null ) {
                throw new ArgumentNullException( "coordinate" );
            }
            var num = 1.0D / coordinate.SquareLength;
            return new CoordinateU64( ( UInt64 )( coordinate.X * num ), ( UInt64 )( coordinate.Y * num ), ( UInt64 )( coordinate.Z * num ) );
        }

        /// <summary>
        ///     Calculates the distance between two <see cref="CoordinateU64" />.
        /// </summary>
        public static UInt64 Distance( CoordinateU64 lhs, CoordinateU64 rhs ) {
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
        ///     preCalc hash of <see cref="X" />, <see cref="Y" />, and <see cref="Z" />. (I have no clue if GetHashCode is called
        ///     once for immutable objects..?)
        /// </summary>
        public override int GetHashCode() {
            return this.X.GetHashMerge( this.Y.GetHashMerge( this.Z ) );
        }

        public override String ToString() {
            return String.Format( "{0}, {1}, {2}", this.X, this.Y, this.Z );
        }

        /// <summary>
        ///     Calculates the distance between this <see cref="CoordinateU64" /> and another <see cref="CoordinateU64" />.
        /// </summary>
        public UInt64 Distance( CoordinateU64 rhs ) {
            if ( rhs == null ) {
                throw new ArgumentNullException( "rhs" );
            }
            var num1 = this.X - rhs.X;
            var num2 = this.Y - rhs.Y;
            var num3 = this.Z - rhs.Z;
            return ( UInt64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is CoordinateU64 && Equals( this, ( CoordinateU64 )obj );
        }

        public static implicit operator Point( CoordinateU64 coordinate ) {
            return new Point( x: ( int )coordinate.X, y: ( int )coordinate.Y );
        }

        public static implicit operator PointF( CoordinateU64 coordinate ) {
            return new PointF( coordinate.X, coordinate.Y );
        }

        public static CoordinateU64 operator -( CoordinateU64 v1, CoordinateU64 v2 ) {
            return new CoordinateU64( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );
        }

        public static Boolean operator !=( CoordinateU64 lhs, CoordinateU64 rhs ) {
            return !Equals( lhs: lhs, rhs: rhs );
        }

        public static Boolean operator ==( CoordinateU64 lhs, CoordinateU64 rhs ) {
            return Equals( lhs: lhs, rhs: rhs );
        }

        ///// <summary>
        /////   Calculates the distance between this <see cref="Coordinate64"/> and another <see cref="Tidbit"/>.
        ///// </summary>
        //
        //public UInt64 Distance( Tidbit tidbit ) {
        //    if ( tidbit == null ) {
        //        throw new ArgumentNullException( "tidbit" );
        //    }
        //    var rhs = tidbit.Coordinate;
        //    var num1 = this.X - rhs.X;
        //    var num2 = this.Y - rhs.Y;
        //    var num3 = this.Z - rhs.Z;
        //    return ( UInt64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        //}

        ///// <summary>
        /////   Calculates the distance between two <see cref="Tidbit"/>.
        ///// </summary>
        //
        //public static UInt64 Distance( Tidbit pre, Tidbit post ) {
        //    return pre.Coordinate.Distance( post );
        //}
    }
}