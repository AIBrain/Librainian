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

	/// <summary>
	///     <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="Single" />).</para>
	/// </summary>
	/// <remarks>Code towards speed.</remarks>
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject( MemberSerialization.Fields )]
	public class CoordinateF : IEquatable<CoordinateF>, IComparable<CoordinateF> {

		/// <summary>Initialize with a random point.</summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public CoordinateF( SingleRange x, SingleRange y, SingleRange z ) : this( Randem.NextFloat( x.Min, x.Max ), Randem.NextFloat( y.Min, y.Max ),
																				  Randem.NextFloat( z.Min, z.Max ) ) { }

		/// <summary></summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public CoordinateF( Single x, Single y, Single z ) {
			this.X = Math.Max( Single.Epsilon, Math.Min( 1, x ) );
			this.Y = Math.Max( Single.Epsilon, Math.Min( 1, y ) );
			this.Z = Math.Max( Single.Epsilon, Math.Min( 1, z ) );
			this.SquareLength = ( this.X * this.X ) + ( this.Y * this.Y ) + ( this.Z * this.Z );
		}

		public static CoordinateF? Empty { get; }

		public static CoordinateF One { get; } = new CoordinateF( 1, 1, 1 );

		public static CoordinateF Zero { get; } = new CoordinateF( 0, 0, 0 );

		[JsonProperty]
		public Single SquareLength { get; }

		[JsonProperty]
		public Single X { get; }

		[JsonProperty]
		public Single Y { get; }

		[JsonProperty]
		public Single Z { get; }

		/// <summary>Compares the current object with another object of the same type.</summary>
		/// <returns>
		///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
		///     following meanings: Value Meaning Less than zero This
		///     object is less than the <paramref name="other" /> parameter. Zero This object is equal to <paramref name="other" />
		///     . Greater than zero This object is greater than
		///     <paramref name="other" /> .
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public Int32 CompareTo( [NotNull] CoordinateF other ) => this.SquareLength.CompareTo( other.SquareLength );

		public Boolean Equals( CoordinateF other ) => Equals( this, other );

		/// <summary>Calculates the distance between two Coordinates.</summary>
		public static Single Distance( [NotNull] CoordinateF left, [NotNull] CoordinateF right ) {
			var num1 = left.X - right.X;
			var num2 = left.Y - right.Y;
			var num3 = left.Z - right.Z;

			return ( Single )Math.Sqrt( ( num1 * num1 ) + ( num2 * num2 ) + ( num3 * num3 ) );
		}

		/// <summary>static comparison.</summary>
		/// <param name="left"></param>
		/// <param name="right"> </param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] CoordinateF? left, [CanBeNull] CoordinateF? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null ) {
				return default;
			}

			if ( right is null ) {
				return default;
			}

			if ( left.X < right.X ) {
				return default;
			}

			if ( left.X > right.X ) {
				return default;
			}

			if ( left.Y < right.Y ) {
				return default;
			}

			if ( left.Y > right.Y ) {
				return default;
			}

			if ( left.Z < right.Z ) {
				return default;
			}

			return !( left.Z > right.Z );
		}

		public static implicit operator Point( [NotNull] CoordinateF coordinate ) => new Point( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

		public static implicit operator PointF( [NotNull] CoordinateF coordinate ) => new PointF( coordinate.X, coordinate.Y );

		/// <summary>
		///     Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in length pointing in the
		///     same direction as the original Coordinate.
		/// </summary>
		[NotNull]
		public static CoordinateF Normalize( [NotNull] CoordinateF coordinate ) {
			var num = 1.0f / coordinate.SquareLength;

			return new CoordinateF( coordinate.X * num, coordinate.Y * num, coordinate.Z * num );
		}

		[NotNull]
		public static CoordinateF operator -( [NotNull] CoordinateF v1, [NotNull] CoordinateF v2 ) => new CoordinateF( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

		public static Boolean operator !=( [CanBeNull] CoordinateF? left, [CanBeNull] CoordinateF? right ) => !Equals( left, right );

		public static Boolean operator ==( [CanBeNull] CoordinateF? left, [CanBeNull] CoordinateF? right ) => Equals( left, right );

		public Double DistanceTo( [NotNull] CoordinateF to ) {
			if ( this == to ) {
				return 0;
			}

			var dx = this.X - to.X;
			var dy = this.Y - to.Y;
			var dz = this.Z - to.Z;

			return Math.Sqrt( ( dx * dx ) + ( dy * dy ) + ( dz * dz ) );
		}

		public override Boolean Equals( Object? obj ) => obj is CoordinateF f && Equals( this, f );

		public override Int32 GetHashCode() => ( this.X, this.Y, this.Z ).GetHashCode();

		[NotNull]
		public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";

	}

}