// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Graphics.DD {

	using System;
	using System.Drawing;

	public static class GeometryEx {

		public static Intersection IntersectionOf( Line line, Polygon polygon ) {
			switch ( polygon.Length ) {
				case 0:
					return Intersection.None;

				case 1:
					return IntersectionOf( polygon[ 0 ], line );
			}

			var tangent = false;

			for ( var index = 0; index < polygon.Length; index++ ) {
				var index2 = ( index + 1 ) % polygon.Length;
				var intersection = IntersectionOf( line, new Line( polygon[ index ], polygon[ index2 ] ) );

				switch ( intersection ) {
					case Intersection.Intersection:
						return intersection;

					case Intersection.Tangent:
						tangent = true;

						break;
				}
			}

			return tangent ? Intersection.Tangent : IntersectionOf( line.P1, polygon );
		}

		public static Intersection IntersectionOf( PointF point, Line line ) {
			var bottomY = Math.Min( line.Y1, line.Y2 );
			var topY = Math.Max( line.Y1, line.Y2 );
			var heightIsRight = point.Y >= bottomY && point.Y <= topY;

			//Vertical line, slope is divideByZero error!
			if ( Math.Abs( line.X1 - line.X2 ) < Single.Epsilon ) {
				if ( Math.Abs( point.X - line.X1 ) < Single.Epsilon && heightIsRight ) {
					return Intersection.Tangent;
				}

				return Intersection.None;
			}

			var slope = ( line.X2 - line.X1 ) / ( line.Y2 - line.Y1 );
			var onLine = Math.Abs( line.Y1 - point.Y - ( slope * ( line.X1 - point.X ) ) ) < Single.Epsilon;

			return onLine && heightIsRight ? Intersection.Tangent : Intersection.None;
		}

		public static Intersection IntersectionOf( Line line1, Line line2 ) {

			// Fail if either line segment is zero-length.
			if ( ( Math.Abs( line1.X1 - line1.X2 ) < Single.Epsilon && Math.Abs( line1.Y1 - line1.Y2 ) < Single.Epsilon ) ||
				 ( Math.Abs( line2.X1 - line2.X2 ) < Single.Epsilon && Math.Abs( line2.Y1 - line2.Y2 ) < Single.Epsilon ) ) {
				return Intersection.None;
			}

			if ( ( Math.Abs( line1.X1 - line2.X1 ) < Single.Epsilon && Math.Abs( line1.Y1 - line2.Y1 ) < Single.Epsilon ) ||
				 ( Math.Abs( line1.X2 - line2.X1 ) < Single.Epsilon && Math.Abs( line1.Y2 - line2.Y1 ) < Single.Epsilon ) ) {
				return Intersection.Intersection;
			}

			if ( ( Math.Abs( line1.X1 - line2.X2 ) < Single.Epsilon && Math.Abs( line1.Y1 - line2.Y2 ) < Single.Epsilon ) ||
				 ( Math.Abs( line1.X2 - line2.X2 ) < Single.Epsilon && Math.Abs( line1.Y2 - line2.Y2 ) < Single.Epsilon ) ) {
				return Intersection.Intersection;
			}

			// (1) Translate the system so that point A is on the origin.
			line1.X2 -= line1.X1;
			line1.Y2 -= line1.Y1;
			line2.X1 -= line1.X1;
			line2.Y1 -= line1.Y1;
			line2.X2 -= line1.X1;
			line2.Y2 -= line1.Y1;

			// Discover the length of segment A-B.
			var distAb = Math.Sqrt( ( line1.X2 * line1.X2 ) + ( line1.Y2 * line1.Y2 ) );

			// (2) Rotate the system so that point B is on the positive X axis.
			var theCos = line1.X2 / distAb;
			var theSin = line1.Y2 / distAb;
			var newX = ( line2.X1 * theCos ) + ( line2.Y1 * theSin );
			line2.Y1 = ( Single )( ( line2.Y1 * theCos ) - ( line2.X1 * theSin ) );
			line2.X1 = ( Single )newX;
			newX = ( line2.X2 * theCos ) + ( line2.Y2 * theSin );
			line2.Y2 = ( Single )( ( line2.Y2 * theCos ) - ( line2.X2 * theSin ) );
			line2.X2 = ( Single )newX;

			// Fail if segment C-D doesn't cross line A-B.
			if ( ( line2.Y1 < 0 && line2.Y2 < 0 ) || ( line2.Y1 >= 0 && line2.Y2 >= 0 ) ) {
				return Intersection.None;
			}

			// (3) Discover the position of the intersection point along line A-B.
			Double posAb = line2.X2 + ( ( ( line2.X1 - line2.X2 ) * line2.Y2 ) / ( line2.Y2 - line2.Y1 ) );

			// Fail if segment C-D crosses line A-B outside of segment A-B.
			if ( posAb < 0 || posAb > distAb ) {
				return Intersection.None;
			}

			// (4) Apply the discovered position to line A-B in the original coordinate system.
			return Intersection.Intersection;
		}

		public static Intersection IntersectionOf( PointF point, Polygon polygon ) {
			switch ( polygon.Length ) {
				case 0:
					return Intersection.None;

				case 1:

					if ( Math.Abs( polygon[ 0 ].X - point.X ) < Single.Epsilon && Math.Abs( polygon[ 0 ].Y - point.Y ) < Single.Epsilon ) {
						return Intersection.Tangent;
					}

					return Intersection.None;

				case 2:
					return IntersectionOf( point, new Line( polygon[ 0 ], polygon[ 1 ] ) );
			}

			var counter = 0;
			Int32 i;
			var n = polygon.Length;
			var p1 = polygon[ 0 ];

			if ( point == p1 ) {
				return Intersection.Tangent;
			}

			for ( i = 1; i <= n; i++ ) {
				var p2 = polygon[ i % n ];

				if ( point == p2 ) {
					return Intersection.Tangent;
				}

				if ( point.Y > Math.Min( p1.Y, p2.Y ) ) {
					if ( point.Y <= Math.Max( p1.Y, p2.Y ) ) {
						if ( point.X <= Math.Max( p1.X, p2.X ) ) {
							if ( Math.Abs( p1.Y - p2.Y ) > Single.Epsilon ) {
								Double xinters = ( ( ( point.Y - p1.Y ) * ( p2.X - p1.X ) ) / ( p2.Y - p1.Y ) ) + p1.X;

								if ( Math.Abs( p1.X - p2.X ) < Single.Epsilon || point.X <= xinters ) {
									counter++;
								}
							}
						}
					}
				}

				p1 = p2;
			}

			return counter % 2 == 1 ? Intersection.Containment : Intersection.None;
		}
	}
}