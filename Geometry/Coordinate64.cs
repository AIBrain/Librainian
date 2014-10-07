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

namespace Librainian.Geometry {
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
    [ DataContract( IsReference = true ) ]
    public struct Coordinate64 : IEquatable< Coordinate64 >, IComparable< Coordinate64 > {
        /// <summary>
        ///     The smallest value a <see cref="Coordinate64" /> will hold.
        /// </summary>
        public const UInt64 Minimum = 1 + UInt32.MinValue; //TODO why is this not Zero ?

        /// <summary>
        ///     The largest value a <see cref="Coordinate64" /> will hold.
        /// </summary>
        /// <remarks>
        ///     the squareroot of <see cref="ulong.MaxValue" /> split into x,y,z.
        /// </remarks>
        public static readonly UInt64 Maximum = ( UInt64 ) ( Math.Sqrt( UInt64.MaxValue ) / 3.0f );

        public static readonly Coordinate64 Empty = default( Coordinate64 );

        public static readonly Coordinate64 MaxValue = new Coordinate64( x: Maximum, y: Maximum, z: Maximum );

        public static readonly Coordinate64 MinValue = new Coordinate64( x: Minimum, y: Minimum, z: Minimum );

        /// <summary>
        ///     Maximum - Minimum
        /// </summary>
        public static readonly UInt64 Range = Maximum - Minimum;

        [ DataMember ] [ OptionalField ] public readonly UInt64 SquareLength;

        [ DataMember ] [ OptionalField ] public readonly UInt64 X;

        [ DataMember ] [ OptionalField ] public readonly UInt64 Y;

        [ DataMember ] [ OptionalField ] public readonly UInt64 Z;

        ///// <summary>
        /////   Initialize with a random point.
        ///// </summary>
        //public Coordinate() : this( x: Randem.NextFloat(), y: Randem.NextFloat(), z: Randem.NextFloat() ) { }

        public Coordinate64( UInt64Range x ) : this( x: Randem.Next( x.Min, x.Max ), y: Randem.Next( Minimum, Maximum ), z: Randem.Next( Minimum, Maximum ) ) { }

        public Coordinate64( UInt64Range x, UInt64Range y, UInt64Range z ) : this( Randem.Next( x.Min, x.Max ), y: Randem.Next( y.Min, y.Max ), z: Randem.Next( z.Min, z.Max ) ) { }

        /// <summary>
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <param name="z"> </param>
        public Coordinate64( UInt64 x, UInt64 y, UInt64 z ) {
            this.X = Math.Max( Minimum, Math.Min( Maximum, x ) );
            this.Y = Math.Max( Minimum, Math.Min( Maximum, y ) );
            this.Z = Math.Max( Minimum, Math.Min( Maximum, z ) );
            this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

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
        [ Pure ]
        public int CompareTo( Coordinate64 other ) {
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
        public Boolean Equals( Coordinate64 other ) {
            return Equals( this, other );
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

        /// <summary>
        ///     <para>Returns a new Coordinate as a unit <see cref="Coordinate64" />.</para>
        ///     <para>The result is a Coordinate one unit in length pointing in the same direction as the original Coordinate.</para>
        /// </summary>
        public static Coordinate64 Normalize( Coordinate64 coordinate ) {
            if ( coordinate == null ) {
                throw new ArgumentNullException( "coordinate" );
            }
            var num = 1.0D / coordinate.SquareLength;
            return new Coordinate64( ( UInt64 ) ( coordinate.X * num ), ( UInt64 ) ( coordinate.Y * num ), ( UInt64 ) ( coordinate.Z * num ) );
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
            return ( UInt64 ) Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
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
        ///     Calculates the distance between this <see cref="Coordinate64" /> and another <see cref="Coordinate64" />.
        /// </summary>
        public UInt64 Distance( Coordinate64 rhs ) {
            if ( rhs == null ) {
                throw new ArgumentNullException( "rhs" );
            }
            var num1 = this.X - rhs.X;
            var num2 = this.Y - rhs.Y;
            var num3 = this.Z - rhs.Z;
            return ( UInt64 ) Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Coordinate64 && Equals( this, ( Coordinate64 ) obj );
        }

        public static implicit operator Point( Coordinate64 coordinate ) {
            return new Point( x: ( int ) coordinate.X, y: ( int ) coordinate.Y );
        }

        public static implicit operator PointF( Coordinate64 coordinate ) {
            return new PointF( coordinate.X, coordinate.Y );
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
