#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Functions.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Geometry.Line {
    using System;
    using System.Diagnostics;

    public static class Functions {
        //  /// <summary>
        /////   Calculates the intersection line segment between 2 lines (not segments).
        /////   Returns false if no solution can be found.
        ///// </summary>
        ///// <returns></returns>
        //public static Boolean CalculateLineLineIntersection( Vector3 line1Point1, Vector3 line1Point2, Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2 ) {
        //    // Algorithm is ported from the C algorithm of 
        //    // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
        //    resultSegmentPoint1 = Vector3.Empty;
        //    resultSegmentPoint2 = Vector3.Empty;

        //    //var p1 = line1Point1;
        //    //var p3 = line2Point1;
        //    //var p4 = line2Point2;
        //    var p13 = line1Point1 - line2Point1;
        //    var p43 = line2Point2 - line2Point1;

        //    if ( p43.LengthSq() < Single.Epsilon ) {
        //        return false;
        //    }

        //    var p2 = line1Point2;
        //    var p21 = p2 - line1Point1;
        //    if ( p21.LengthSq() < Single.Epsilon ) {
        //        return false;
        //    }

        //    var d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
        //    var d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
        //    var d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
        //    var d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
        //    var d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

        //    var denom = d2121 * d4343 - d4321 * d4321;
        //    if ( Math.Abs( denom ) < Single.Epsilon ) {
        //        return false;
        //    }

        //    var numer = d1343 * d4321 - d1321 * d4343;

        //    var mua = numer / denom;
        //    resultSegmentPoint1.X = line1Point1.X + mua * p21.X;
        //    resultSegmentPoint1.Y = line1Point1.Y + mua * p21.Y;
        //    resultSegmentPoint1.Z = line1Point1.Z + mua * p21.Z;

        //    var mub = ( d1343 + d4321 * ( mua ) ) / d4343;
        //    resultSegmentPoint2.X = line2Point1.X + mub * p43.X;
        //    resultSegmentPoint2.Y = line2Point1.Y + mub * p43.Y;
        //    resultSegmentPoint2.Z = line2Point1.Z + mub * p43.Z;

        //    return true;
        //}

        /// <summary>
        ///     Calculates the intersection line segment between 2 lines (not segments).
        ///     Returns false if no solution can be found.
        /// </summary>
        /// <returns></returns>
        public static Boolean CalculateLineLineIntersection( Coordinate line1Point1, Coordinate line1Point2, Coordinate line2Point1, Coordinate line2Point2, out Coordinate resultSegmentPoint1, out Coordinate resultSegmentPoint2 ) {
            Debugger.Break();

            // Algorithm is ported from the C algorithm of 
            // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            resultSegmentPoint1 = Coordinate.Empty;
            resultSegmentPoint2 = Coordinate.Empty;

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

            var d1343 = p13.X*p43.X + p13.Y*p43.Y + p13.Z*p43.Z;
            var d4321 = p43.X*p21.X + p43.Y*p21.Y + p43.Z*p21.Z;
            var d1321 = p13.X*p21.X + p13.Y*p21.Y + p13.Z*p21.Z;
            var d4343 = p43.X*p43.X + p43.Y*p43.Y + p43.Z*p43.Z;
            var d2121 = p21.X*p21.X + p21.Y*p21.Y + p21.Z*p21.Z;

            var denom = d2121*d4343 - d4321*d4321;
            if ( Math.Abs( denom ) < Single.Epsilon ) {
                return false;
            }
            var numer = d1343*d4321 - d1321*d4343;

            var mua = numer/denom;
            resultSegmentPoint1 = new Coordinate( x: p1.X + mua*p21.X, y: p1.Y + mua*p21.Y, z: p1.Z + mua*p21.Z );

            var mub = ( d1343 + d4321*( mua ) )/d4343;
            resultSegmentPoint2 = new Coordinate( x: p3.X + mub*p43.X, y: p3.Y + mub*p43.Y, z: p3.Z + mub*p43.Z );

            return true;
        }
    }
}
