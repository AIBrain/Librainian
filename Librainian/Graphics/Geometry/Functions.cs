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
// File "Functions.cs" last formatted on 2020-08-14 at 8:34 PM.

namespace Librainian.Graphics.Geometry {

	using System;
	using System.Numerics;
	using DDD;
	using Logging;

	public static class Functions {

		/// <summary>Angles of a rectangle.</summary>
		[Flags]
		public enum RectAngles {

			None = 0,

			TopLeft = 1,

			TopRight = 2,

			BottomLeft = 4,

			BottomRight = 8,

			All = TopLeft | TopRight | BottomLeft | BottomRight
		}

		/// <summary>
		///     Calculates the intersection line segment between 2 lines (not segments). Returns false if no solution can be
		///     found.
		/// </summary>
		public static Boolean CalculateLineLineIntersection(
			this Vector3 line1Point1,
			Vector3 line1Point2,
			Vector3 line2Point1,
			Vector3 line2Point2,
			out Vector3 resultSegmentPoint1,
			out Vector3 resultSegmentPoint2
		) {

			// Algorithm is ported from the C algorithm of
			// Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
			//resultSegmentPoint1 = Vector3.Empty;
			//resultSegmentPoint2 = Vector3.Empty;

			//var p1 = line1Point1;
			//var p3 = line2Point1;
			//var p4 = line2Point2;
			var p13 = line1Point1 - line2Point1;
			var p43 = line2Point2 - line2Point1;

			if ( p43.LengthSquared() < Single.Epsilon ) {
				resultSegmentPoint1 = new Vector3();
				resultSegmentPoint2 = new Vector3();

				return false;
			}

			var p2 = line1Point2;
			var p21 = p2 - line1Point1;

			if ( p21.LengthSquared() < Single.Epsilon ) {
				resultSegmentPoint1 = new Vector3();
				resultSegmentPoint2 = new Vector3();

				return false;
			}

			var d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
			var d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
			var d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
			var d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
			var d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

			var denom = d2121 * d4343 - d4321 * d4321;

			if ( Math.Abs( denom ) < Single.Epsilon ) {
				resultSegmentPoint1 = new Vector3();
				resultSegmentPoint2 = new Vector3();

				return false;
			}

			var numer = d1343 * d4321 - d1321 * d4343;

			var mua = numer / denom;

			resultSegmentPoint1 = new Vector3 {
				X = line1Point1.X + mua * p21.X,
				Y = line1Point1.Y + mua * p21.Y,
				Z = line1Point1.Z + mua * p21.Z
			};

			var mub = ( d1343 + d4321 * mua ) / d4343;

			resultSegmentPoint2 = new Vector3 {
				X = line2Point1.X + mub * p43.X,
				Y = line2Point1.Y + mub * p43.Y,
				Z = line2Point1.Z + mub * p43.Z
			};

			return true;
		}

		/// <summary>
		///     Calculates the intersection line segment between 2 lines (not segments). Returns false if no solution can be
		///     found.
		/// </summary>
		public static Boolean CalculateLineLineIntersection(
			this CoordinateF? line1Point1,
			CoordinateF? line1Point2,
			CoordinateF? line2Point1,
			CoordinateF? line2Point2,
			out CoordinateF? resultSegmentPoint1,
			out CoordinateF? resultSegmentPoint2
		) {
			"".Break();

			// Algorithm is ported from the C algorithm of Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
			resultSegmentPoint1 = CoordinateF.Empty;
			resultSegmentPoint2 = CoordinateF.Empty;

			var p1 = line1Point1;
			var p2 = line1Point2;
			var p3 = line2Point1;
			var p4 = line2Point2;
			var p13 = p1 - p3;
			var p43 = p4 - p3;

			if ( p43.SquareLength < Single.Epsilon ) {
				return false;
			}

			var p21 = p2 - p1;

			if ( p21.SquareLength < Single.Epsilon ) {
				return false;
			}

			var d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
			var d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
			var d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
			var d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
			var d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

			var denom = d2121 * d4343 - d4321 * d4321;

			if ( Math.Abs( denom ) < Single.Epsilon ) {
				return false;
			}

			var numer = d1343 * d4321 - d1321 * d4343;

			var mua = numer / denom;
			resultSegmentPoint1 = new CoordinateF( p1.X + mua * p21.X, p1.Y + mua * p21.Y, p1.Z + mua * p21.Z );

			var mub = ( d1343 + d4321 * mua ) / d4343;
			resultSegmentPoint2 = new CoordinateF( p3.X + mub * p43.X, p3.Y + mub * p43.Y, p3.Z + mub * p43.Z );

			return true;
		}
	}
}