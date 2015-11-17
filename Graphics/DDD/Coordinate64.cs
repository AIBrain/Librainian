#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
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
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Coordinate64.cs" was last cleaned by Rick on 2015/06/28 at 3:42 AM
#endregion License & Information

namespace Librainian.Graphics.DDD {
    using System;
    using System.Data.Linq.Mapping;
    using System.Drawing;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Threading;

    /// <summary>
    /// A 3D point; with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> long integers.
    /// </summary>
    /// <remarks>Coded towards speed.</remarks>
    [DataContract( IsReference = false )]
    [Immutable]
    public struct Coordinate64 : IEquatable<Coordinate64>, IComparable<Coordinate64> {

        //public static readonly Coordinate64 AtMaxValues = new Coordinate64( x: Int64.MaxValue, y: Int64.MaxValue, z: Int64.MaxValue );
        //public static readonly Coordinate64 AtMinValues = new Coordinate64( x: Int64.MinValue, y: Int64.MinValue, z: Int64.MinValue );

        public static readonly Coordinate64 Zeroth = new Coordinate64( x: 0, y: 0, z: 0 );

        private readonly Int32 _hashCode;

        /// <summary></summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Coordinate64( Int64 x, Int64 y, Int64 z ) {
            this.X = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, x ) );
            this.Y = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, y ) );
            this.Z = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, z ) );
            this.Length = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            this._hashCode = this.X.GetHashMerge( this.Y.GetHashMerge( this.Z ) );
        }

        [Column]
        [DataMember]
        public Int64 Length {
            get; private set;
        }

        [Column]
        [DataMember]
        public Int64 X {
            get; private set;
        }

        [Column]
        [DataMember]
        public Int64 Y {
            get; private set;
        }

        [Column]
        [DataMember]
        public Int64 Z {
            get; private set;
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
        public Int32 CompareTo( Coordinate64 other ) => this.Length.CompareTo( other.Length );

        /// <summary>Calls the static comparison.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( Coordinate64 other ) => Equals( this, other );

        /// <summary>
        /// Allow an explicit conversion from <see cref="Coordinate64" /> to a <see cref="Vector3" />.
        /// </summary>
        /// <param name="bob"></param>
        public static explicit operator Vector3( Coordinate64 bob ) => new Vector3( bob.X, bob.Y, bob.Z );

        /// <summary>Calculates the distance between two <see cref="Coordinate64" />.</summary>
        public static Int64 Distance( Coordinate64 lhs, Coordinate64 rhs ) {
            var num1 = lhs.X - rhs.X;
            var num2 = lhs.Y - rhs.Y;
            var num3 = lhs.Z - rhs.Z;
            return ( Int64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        /// <summary>static comparison.</summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean Equals( Coordinate64 lhs, Coordinate64 rhs ) => ( lhs.X == rhs.X ) && ( lhs.Y == rhs.Y ) && ( lhs.Z == rhs.Z );

        public static implicit operator Point( Coordinate64 coordinate ) => new Point( x: ( Int32 )coordinate.X, y: ( Int32 )coordinate.Y );

        public static implicit operator PointF( Coordinate64 coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        /// <para>Returns a new Coordinate as a unit <see cref="Coordinate64" />.</para>
        /// <para>
        /// The result is a Coordinate one unit in length pointing in the same direction as the
        /// original Coordinate.
        /// </para>
        /// </summary>
        public static Coordinate64 Normalize( Coordinate64 coordinate ) {
            var num = 1.0D / coordinate.Length;
            return new Coordinate64( ( Int64 )( coordinate.X * num ), ( Int64 )( coordinate.Y * num ), ( Int64 )( coordinate.Z * num ) );
        }

        public static Coordinate64 operator -( Coordinate64 v1, Coordinate64 v2 ) => new Coordinate64( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        public static Boolean operator !=( Coordinate64 lhs, Coordinate64 rhs ) => !Equals( lhs: lhs, rhs: rhs );

        public static Boolean operator ==( Coordinate64 lhs, Coordinate64 rhs ) => Equals( lhs: lhs, rhs: rhs );

        public static Coordinate64 Random() => new Coordinate64( x: Randem.NextInt64(), y: Randem.NextInt64(), z: Randem.NextInt64() );

        /// <summary>
        /// Calculates the distance between this <see cref="Coordinate64" /> and another <see cref="Coordinate64" />.
        /// </summary>
        public Int64 Distance( Coordinate64 to ) {
            var num1 = this.X - to.X;
            var num2 = this.Y - to.Y;
            var num3 = this.Z - to.Z;
            return ( Int64 )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
        }

        public override Boolean Equals( Object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Coordinate64 && Equals( this, ( Coordinate64 )obj );
        }

        /// <summary>hash of <see cref="X" />, <see cref="Y" />, and <see cref="Z" />.</summary>
        public override Int32 GetHashCode() => this._hashCode;

        ///// <summary>
        ///// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        ///// </summary>
        ///// <returns>
        ///// A value that indicates the relative order of the objects being compared.
        ///// The return value has these meanings:
        /////     Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order.
        ///// </returns>
        ///// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception>
        //public int CompareTo( object obj ) {
        //    if ( obj is Coordinate64) {
        //        return this.Length.CompareTo( ((Coordinate64)obj).Length );
        //    }
        //    return 0;
        //}

        public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
    }
}