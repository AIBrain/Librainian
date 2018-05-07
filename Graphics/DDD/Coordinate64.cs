// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Coordinate64.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.DDD {

    using System;
    using System.Data.Linq.Mapping;
    using System.Diagnostics;
    using System.Drawing;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>
    ///     A 3D point; with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> <see cref="Int64"/> integers.
    /// </summary>
    /// <remarks>Code towards speed.</remarks>
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( MemberSerialization.Fields )]
    public class Coordinate64 : IEquatable<Coordinate64>, IComparable<Coordinate64> {
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

        [JsonProperty( "h" )]
        private readonly Int32 _hashCode;

        /// <summary></summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Coordinate64( Int64 x, Int64 y, Int64 z ) {
            this.X = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, x ) );
            this.Y = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, y ) );
            this.Z = Math.Max( Int64.MinValue, Math.Min( Int64.MaxValue, z ) );
            this.Length = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            this._hashCode = Hashing.GetHashCodes( this.X, this.Y, this.Z );
        }

        [Column]
        [JsonProperty( "L" )]
        public Int64 Length {
            get; private set;
        }

        [Column]
        [JsonProperty( "X" )]
        public Int64 X {
            get; private set;
        }

        [Column]
        [JsonProperty( "Y" )]
        public Int64 Y {
            get; private set;
        }

        [Column]
        [JsonProperty( "Z" )]
        public Int64 Z {
            get; private set;
        }

        /// <summary>Calculates the distance between two <see cref="Coordinate64" />.</summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
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
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean Equals( Coordinate64 lhs, Coordinate64 rhs ) {
            if ( ReferenceEquals( lhs, rhs ) ) {
                return true;
            }
            if ( lhs is null ) {
                return false;
            }
            if ( rhs is null ) {
                return false;
            }
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static explicit operator Point( Coordinate64 coordinate ) => new Point( x: ( Int32 )coordinate.X, y: ( Int32 )coordinate.Y );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static explicit operator PointF( Coordinate64 coordinate ) => new PointF( coordinate.X, coordinate.Y );

        /// <summary>
        ///     Allow an explicit conversion from <see cref="Coordinate64" /> to a <see cref="Vector3" />.
        /// </summary>
        /// <param name="bob"></param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static explicit operator Vector3( Coordinate64 bob ) => new Vector3( bob.X, bob.Y, bob.Z );

        /// <summary>
        ///     <para>Returns a new Coordinate64 as a unit <see cref="Coordinate64" />.</para>
        ///     <para>
        ///         The result is a Coordinate64 one unit in length pointing in the same direction as the
        ///         original Coordinate64.
        ///     </para>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Coordinate64 Normalize( Coordinate64 coordinate ) {
            var num = 1.0D / coordinate.Length;
            return new Coordinate64( ( Int64 )( coordinate.X * num ), ( Int64 )( coordinate.Y * num ), ( Int64 )( coordinate.Z * num ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Coordinate64 operator -( Coordinate64 v1, Coordinate64 v2 ) => new Coordinate64( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean operator !=( Coordinate64 lhs, Coordinate64 rhs ) => !Equals( lhs: lhs, rhs: rhs );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Boolean operator ==( Coordinate64 lhs, Coordinate64 rhs ) => Equals( lhs: lhs, rhs: rhs );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Coordinate64 Random() => new Coordinate64( x: Randem.NextInt64(), y: Randem.NextInt64(), z: Randem.NextInt64() );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public Int32 CompareTo( Coordinate64 other ) => this.Length.CompareTo( other.Length );

        /// <summary>
        ///     Calculates the distance between this <see cref="Coordinate64" /> and another <see cref="Coordinate64" />.
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Int64 Distance( Coordinate64 to ) => Distance( this, to );

        /// <summary>Calls the static comparison.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Boolean Equals( Coordinate64 other ) => Equals( this, other );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }
            return obj is Coordinate64 coordinate64 && Equals( this, coordinate64 );
        }

        /// <summary>precomputed hash of <see cref="X" />, <see cref="Y" />, and <see cref="Z" />.</summary>
        public override Int32 GetHashCode() => this._hashCode;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
    }
}