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
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/LineExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Line {

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Numerics;
    using System.Windows.Media.Media3D;
    using Maths.Numbers;

    public static class LineExtensions {

        // evaluate a point on a bezier-curve. t goes from 0 to 1.0
        public static Point Bezier( this Point a, Point b, Point c, Point d, ZeroToOne t ) {
            var ab = a.Lerp( b, t );
            var bc = b.Lerp( c, t ); // point between b and c (green)
            var cd = c.Lerp( d, t ); // point between c and d (green)
            var abbc = ab.Lerp( bc, t ); // point between ab and bc (blue)
            var bccd = bc.Lerp( cd, t ); // point between bc and cd (blue)
            return abbc.Lerp( bccd, t ); // point on the bezier-curve (black)
        }

        public static IEnumerable<Point> BezierPath( Point start, Point end, Single stepping, Int32 height ) {
            yield return start;

            var offesetX = Math.Abs( end.X - start.X ) / 2;

            var c = new Point( start.X + offesetX / 2, start.Y - height / 2 );
            var d = new Point( end.X - offesetX / 2, start.Y + height / 2 );

            var at = 0.0f;
            while ( at < 1.0f ) {
                var point = Bezier( start, end, c, d, at );
                yield return point;
                at += stepping;
            }

            yield return end;
        }

        /// <summary>
        ///     simple linear interpolation between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Point Lerp( this Point a, Point b, ZeroToOne t ) {
            var dest = new Point { X = ( Int32 )( a.X + ( b.X - a.X ) * t ), Y = ( Int32 )( a.Y + ( b.Y - a.Y ) * t ) };
            return dest;
        }

        /// <summary>
        ///     simple linear interpolation between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Point3D Lerp( this Point3D a, Point3D b, ZeroToOne t ) {
            var dest = new Point3D { X = a.X + ( b.X - a.X ) * t, Y = a.Y + ( b.Y - a.Y ) * t, Z = a.Z + ( b.Z - a.Z ) * t };
            return dest;
        }

        /// <summary>
        ///     simple linear interpolation between two 3D points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 Lerp( this Vector3 a, Vector3 b, Single t ) {
            var dest = new Vector3 { X = a.X + ( b.X - a.X ) * t, Y = a.Y + ( b.Y - a.Y ) * t, Z = a.Z + ( b.Z - a.Z ) * t };
            return dest;
        }

        public static Point3Di Lerp( Point3Di a, Point3Di b, Single t ) {
            var dest = new Point3Di( ( Int32 )( a.X + ( b.X - a.X ) * t ), ( Int32 )( a.Y + ( b.Y - a.Y ) * t ), ( Int32 )( a.Z + ( b.Z - a.Z ) * t ) );
            return dest;
        }
    }
}