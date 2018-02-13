// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/GeometryEx.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

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
            var onLine = Math.Abs( line.Y1 - point.Y - slope * ( line.X1 - point.X ) ) < Single.Epsilon;
            return onLine && heightIsRight ? Intersection.Tangent : Intersection.None;
        }

        public static Intersection IntersectionOf( Line line1, Line line2 ) {

            // Fail if either line segment is zero-length.
            if ( Math.Abs( line1.X1 - line1.X2 ) < Single.Epsilon && Math.Abs( line1.Y1 - line1.Y2 ) < Single.Epsilon || Math.Abs( line2.X1 - line2.X2 ) < Single.Epsilon && Math.Abs( line2.Y1 - line2.Y2 ) < Single.Epsilon ) {
                return Intersection.None;
            }

            if ( Math.Abs( line1.X1 - line2.X1 ) < Single.Epsilon && Math.Abs( line1.Y1 - line2.Y1 ) < Single.Epsilon || Math.Abs( line1.X2 - line2.X1 ) < Single.Epsilon && Math.Abs( line1.Y2 - line2.Y1 ) < Single.Epsilon ) {
                return Intersection.Intersection;
            }
            if ( Math.Abs( line1.X1 - line2.X2 ) < Single.Epsilon && Math.Abs( line1.Y1 - line2.Y2 ) < Single.Epsilon || Math.Abs( line1.X2 - line2.X2 ) < Single.Epsilon && Math.Abs( line1.Y2 - line2.Y2 ) < Single.Epsilon ) {
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
            var distAb = Math.Sqrt( line1.X2 * line1.X2 + line1.Y2 * line1.Y2 );

            // (2) Rotate the system so that point B is on the positive X axis.
            var theCos = line1.X2 / distAb;
            var theSin = line1.Y2 / distAb;
            var newX = line2.X1 * theCos + line2.Y1 * theSin;
            line2.Y1 = ( Single )( line2.Y1 * theCos - line2.X1 * theSin );
            line2.X1 = ( Single )newX;
            newX = line2.X2 * theCos + line2.Y2 * theSin;
            line2.Y2 = ( Single )( line2.Y2 * theCos - line2.X2 * theSin );
            line2.X2 = ( Single )newX;

            // Fail if segment C-D doesn't cross line A-B.
            if ( line2.Y1 < 0 && line2.Y2 < 0 || line2.Y1 >= 0 && line2.Y2 >= 0 ) {
                return Intersection.None;
            }

            // (3) Discover the position of the intersection point along line A-B.
            Double posAb = line2.X2 + ( line2.X1 - line2.X2 ) * line2.Y2 / ( line2.Y2 - line2.Y1 );

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
                                Double xinters = ( point.Y - p1.Y ) * ( p2.X - p1.X ) / ( p2.Y - p1.Y ) + p1.X;
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