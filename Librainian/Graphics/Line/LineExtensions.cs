// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "LineExtensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "LineExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 7:44 AM.

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

            var c = new Point( start.X + (offesetX / 2), start.Y - (height / 2) );
            var d = new Point( end.X - (offesetX / 2), start.Y + (height / 2) );

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
            var dest = new Point {
                X = ( Int32 ) ( a.X + (( b.X - a.X ) * t) ), Y = ( Int32 ) ( a.Y + (( b.Y - a.Y ) * t) )
            };

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
            var dest = new Point3D {
                X = a.X + (( b.X - a.X ) * t), Y = a.Y + (( b.Y - a.Y ) * t), Z = a.Z + (( b.Z - a.Z ) * t)
            };

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
            var dest = new Vector3 {
                X = a.X + (( b.X - a.X ) * t), Y = a.Y + (( b.Y - a.Y ) * t), Z = a.Z + (( b.Z - a.Z ) * t)
            };

            return dest;
        }

        public static Point3Di Lerp( Point3Di a, Point3Di b, Single t ) {
            var dest = new Point3Di( ( Int32 ) ( a.X + (( b.X - a.X ) * t) ), ( Int32 ) ( a.Y + (( b.Y - a.Y ) * t) ), ( Int32 ) ( a.Z + (( b.Z - a.Z ) * t) ) );

            return dest;
        }
    }
}