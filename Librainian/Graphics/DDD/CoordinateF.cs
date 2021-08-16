// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "CoordinateF.cs" last formatted on 2020-08-14 at 8:34 PM.

#nullable enable

namespace Librainian.Graphics.DDD {

	using System;
	using System.Diagnostics;
	using System.Drawing;
	using Extensions;
	using Maths;
	using Maths.Ranges;
	using Measurement;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="Single" />).</para>
	/// </summary>
	/// <remarks>Code towards speed.</remarks>
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class CoordinateF : IEquatable<CoordinateF>, IComparable<CoordinateF> {

		public static CoordinateF? Empty { get; } = new( Single.Epsilon, Single.Epsilon, Single.Epsilon );

		public static CoordinateF One { get; } = new( 1, 1, 1 );

		public static CoordinateF Zero { get; } = new( 0, 0, 0 );

		[JsonProperty]
		public Single SquareLength { get; }

		[JsonProperty]
		public Single X { get; }

		[JsonProperty]
		public Single Y { get; }

		[JsonProperty]
		public Single Z { get; }

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
			this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
		}

		/// <summary>Calculates the distance between two Coordinates.</summary>
		public static Single Distance( CoordinateF left, CoordinateF right ) {
			var num1 = left.X - right.X;
			var num2 = left.Y - right.Y;
			var num3 = left.Z - right.Z;

			return ( Single )Math.Sqrt( num1 * num1 + num2 * num2 + num3 * num3 );
		}

		/// <summary>static comparison.</summary>
		/// <param name="left"></param>
		/// <param name="right"> </param>
		public static Boolean Equals( CoordinateF? left, CoordinateF? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null ) {
				return false;
			}

			if ( right is null ) {
				return false;
			}

			if ( left.X < right.X ) {
				return false;
			}

			if ( left.X > right.X ) {
				return false;
			}

			if ( left.Y < right.Y ) {
				return false;
			}

			if ( left.Y > right.Y ) {
				return false;
			}

			if ( left.Z < right.Z ) {
				return false;
			}

			return !( left.Z > right.Z );
		}

		public static implicit operator Point( CoordinateF coordinate ) => new( ( Int32 )coordinate.X, ( Int32 )coordinate.Y );

		public static implicit operator PointF( CoordinateF coordinate ) => new( coordinate.X, coordinate.Y );

		/// <summary>
		///     Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in length pointing in the
		///     same direction as the original Coordinate.
		/// </summary>
		public static CoordinateF Normalize( CoordinateF coordinate ) {
			var num = 1.0f / coordinate.SquareLength;

			return new CoordinateF( coordinate.X * num, coordinate.Y * num, coordinate.Z * num );
		}

		public static CoordinateF operator -( CoordinateF v1, CoordinateF v2 ) => new( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );

		public static Boolean operator !=( CoordinateF? left, CoordinateF? right ) => !Equals( left, right );

		public static Boolean operator ==( CoordinateF? left, CoordinateF? right ) => Equals( left, right );

		/// <summary>Compares the current object with another object of the same type.</summary>
		/// <returns>
		///     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the
		///     following meanings: Value Meaning Less than zero This
		///     object is less than the <paramref name="other" /> parameter. Zero This object is equal to <paramref name="other" />
		///     . Greater than zero This object is greater than
		///     <paramref name="other" /> .
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public Int32 CompareTo( CoordinateF? other ) {
			if ( other is null ) {
				return Order.Before;
			}

			return this.SquareLength.CompareTo( other.SquareLength );
		}

		public Double DistanceTo( CoordinateF to ) {
			if ( this == to ) {
				return 0;
			}

			var dx = this.X - to.X;
			var dy = this.Y - to.Y;
			var dz = this.Z - to.Z;

			return Math.Sqrt( dx * dx + dy * dy + dz * dz );
		}

		public Boolean Equals( CoordinateF? other ) => Equals( this, other );

		public override Boolean Equals( Object? obj ) => obj is CoordinateF f && Equals( this, f );

		public override Int32 GetHashCode() => (this.X, this.Y, this.Z).GetHashCode();

		public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
	}
}