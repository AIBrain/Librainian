#nullable enable

namespace Librainian.Graphics.DDD {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Maths.Ranges;
	using Newtonsoft.Json;

	/// <summary>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> .</summary>
	/// <remarks>Coded towards speed.</remarks>
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject( MemberSerialization.Fields )]
	public struct CoordinateU64 : IEquatable<CoordinateU64>, IComparable<CoordinateU64> {

		/// <summary>The smallest value a <see cref="CoordinateU64" /> will hold.</summary>
		public const UInt64 Minimum = 1 + UInt64.MinValue; //TODO why is this not Zero ?

		public static readonly CoordinateU64 Empty;

		/// <summary>The largest value a <see cref="CoordinateU64" /> will hold.</summary>
		/// <remarks>the cuberoot of <see cref="UInt64.MaxValue" /> split into x*y*z.</remarks>
		public static readonly UInt64 Maximum = ( UInt64 )Math.Pow( UInt64.MaxValue, 1.0 / 3.0 );

		public static readonly CoordinateU64 MaxValue = new CoordinateU64( UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue );

		public static readonly CoordinateU64 MinValue = new CoordinateU64( Minimum, Minimum, Minimum );

		/// <summary>Maximum - Minimum</summary>
		public static readonly UInt64 Range = Maximum - Minimum;

		[JsonProperty]
		public readonly UInt64 SquareLength;

		[JsonProperty]
		public readonly UInt64 X;

		[JsonProperty]
		public readonly UInt64 Y;

		[JsonProperty]
		public readonly UInt64 Z;

		///// <summary>
		/////   Initialize with a random point.
		///// </summary>
		//public Coordinate() : this( x: Randem.NextFloat(), y: Randem.NextFloat(), z: Randem.NextFloat() ) { }

		public CoordinateU64( UInt64Range x ) : this( x.Min.Next( x.Max ), Minimum.Next( Maximum ), Minimum.Next( Maximum ) ) { }

		public CoordinateU64( UInt64Range x, UInt64Range y, UInt64Range z ) : this( x.Min.Next( x.Max ), y.Min.Next( y.Max ), z.Min.Next( z.Max ) ) { }

		/// <summary></summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public CoordinateU64( UInt64 x, UInt64 y, UInt64 z ) {
			this.X = Math.Max( Minimum, Math.Min( Maximum, x ) );
			this.Y = Math.Max( Minimum, Math.Min( Maximum, y ) );
			this.Z = Math.Max( Minimum, Math.Min( Maximum, z ) );
			this.SquareLength = ( this.X * this.X ) + ( this.Y * this.Y ) + ( this.Z * this.Z );
		}

		/// <summary>Calculates the distance between two <see cref="CoordinateU64" />.</summary>
		public static UInt64 Distance( CoordinateU64 left, CoordinateU64 right ) {
			var num1 = left.X - right.X;
			var num2 = left.Y - right.Y;
			var num3 = left.Z - right.Z;

			return ( UInt64 )Math.Sqrt( ( num1 * num1 ) + ( num2 * num2 ) + ( num3 * num3 ) );
		}

		/// <summary>static comparison.</summary>
		/// <param name="left"></param>
		/// <param name="right"> </param>
		/// <returns></returns>
		public static Boolean Equals( CoordinateU64 left, CoordinateU64 right ) => left.X == right.X && left.Y == right.Y && left.Z == right.Z;

		public static implicit operator Point( CoordinateU64 coordinate ) => new Point( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

		public static implicit operator PointF( CoordinateU64 coordinate ) => new PointF( coordinate.X, coordinate.Y );

		/// <summary>
		///     <para>Returns a new Coordinate as a unit <see cref="CoordinateU64" />.</para>
		///     <para>The result is a Coordinate one unit in length pointing in the same direction as the original Coordinate.</para>
		/// </summary>
		public static CoordinateU64 Normalize( CoordinateU64 coordinate ) {
			var num = 1.0D / coordinate.SquareLength;

			return new CoordinateU64( ( UInt64 )( coordinate.X * num ), ( UInt64 )( coordinate.Y * num ), ( UInt64 )( coordinate.Z * num ) );
		}

		public static CoordinateU64 operator -( CoordinateU64 v1, CoordinateU64 v2 ) => new CoordinateU64( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

		public static Boolean operator !=( CoordinateU64 left, CoordinateU64 right ) => !Equals( left, right );

		public static Boolean operator ==( CoordinateU64 left, CoordinateU64 right ) => Equals( left, right );

		/// <summary>Compares the current <see cref="CoordinateU64" /> with another <see cref="CoordinateU64" />.</summary>
		/// <returns>
		///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
		///     following meanings: Value Meaning Less than zero: This
		///     object is less than the <paramref name="other" /> parameter. Zero: This object is equal to
		///     <paramref name="other" /> . Greater than zero This object is greater than
		///     <paramref name="other" /> .
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		[Pure]
		public Int32 CompareTo( CoordinateU64 other ) => this.SquareLength.CompareTo( other.SquareLength );

		/// <summary>Calculates the distance between this <see cref="CoordinateU64" /> and another <see cref="CoordinateU64" />.</summary>
		public UInt64 Distance( CoordinateU64 right ) {
			var num1 = this.X - right.X;
			var num2 = this.Y - right.Y;
			var num3 = this.Z - right.Z;

			return ( UInt64 )Math.Sqrt( ( num1 * num1 ) + ( num2 * num2 ) + ( num3 * num3 ) );
		}

		/// <summary>Calls the static comparison.</summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Boolean Equals( CoordinateU64 other ) => Equals( this, other );

		public override Boolean Equals( Object? obj ) => obj is CoordinateU64 u64 && Equals( this, u64 );

		/// <summary>
		///     preCalc hash of <see cref="X" />, <see cref="Y" />, and <see cref="Z" />. (I have no clue if GetHashCode is
		///     called once for immutable objects..?)
		/// </summary>
		public override Int32 GetHashCode() => ( this.X, this.Y, this.Z ).GetHashCode();

		[NotNull]
		public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";

	}

}