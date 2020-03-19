// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Point3DI.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "Point3DI.cs" was last formatted by Protiguous on 2020/03/16 at 3:05 PM.

namespace Librainian.Graphics {

    using System;
    using JetBrains.Annotations;

    /// <summary>Represents a location in 3D integer space.</summary>
    /// <remarks>Culled from the file CPI.Plot3D.cs I don't know where that file came from otherwise I'd attribute it!</remarks>
    public struct Point3Di : IEquatable<Point3Di> {

        /// <summary>The maximum distance two coordinates can be from each other for them to be considered approximately equal.</summary>
        public const Int32 Tolerance = 0;

        /// <summary>The point's X coordinate.</summary>
        public Int32 X { get; }

        /// <summary>The point's Y coordinate.</summary>
        public Int32 Y { get; }

        /// <summary>The point's Z coordinate.</summary>
        public Int32 Z { get; }

        /// <summary>Instantiates a new Point3D.</summary>
        /// <param name="x">The point's X coordinate.</param>
        /// <param name="y">The point's Y coordinate.</param>
        /// <param name="z">The point's Z coordinate.</param>
        public Point3Di( Int32 x, Int32 y, Int32 z ) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        ///// <summary>
        /////   Gets the location of the point projected onto the XY plane at the Z origin, from a specified
        /////   camera's perspective.
        ///// </summary>
        ///// <param name = "cameraPosition">The location to base perspective calculations on.</param>
        ///// <returns>A PointF representing the projected point.</returns>
        ///// <remarks>
        /////   Calculations are internally performed with Double precision arithmetic, then rounded
        /////   down to floats at the end.
        ///// </remarks>
        //public PointF GetScreenPosition( Point3DI cameraPosition ) {
        //    var returnValue = new PointF {
        //        X = ( ( ( this.X - cameraPosition.X ) * ( -1 * cameraPosition.Z ) ) / ( this.Z - cameraPosition.Z ) ) + cameraPosition.X,
        //        Y = ( ( ( this.Y - cameraPosition.Y ) * ( -1 * cameraPosition.Z ) ) / ( this.Z - cameraPosition.Z ) ) + cameraPosition.Y
        //    };

        //    return returnValue;
        //}

        /// <summary>Static comparison.</summary>
        /// <param name="left"></param>
        /// <param name="right"> </param>
        /// <returns></returns>
        public static Boolean Equals( Point3Di left, Point3Di right ) => left.ApproximatelyEquals( other: right );

        /// <summary>Determines whether the specified Point3D instances are unequal.</summary>
        /// <param name="a">The first Point3D instance to compare.</param>
        /// <param name="b">The second Point3D instance to compare.</param>
        /// <returns>True if the Point3D instances are unequal; false otherwise.</returns>
        public static Boolean operator !=( Point3Di a, Point3Di b ) => !a.Equals( other: b );

        /// <summary>Determines whether the specified Point3D instances are equal.</summary>
        /// <param name="a">The first Point3D instance to compare.</param>
        /// <param name="b">The second Point3D instance to compare.</param>
        /// <returns>True if the Point3D instances are equal; false otherwise.</returns>
        public static Boolean operator ==( Point3Di a, Point3Di b ) => a.Equals( other: b );

        /// <summary>Determines whether this instance is very nearly equal to a specified Point3D structure.</summary>
        /// <remarks>
        /// Since floating point math is kind of fuzzy, we're taking a "close enough" approach to equality with this method. If the individual coordinates of two points fall within a
        /// small tolerance, we'll consider them to be approximately equal. Remember, though, that the uncertainty here can be cumulative. For example: if pointA.Equals(pointB) and
        /// pointB.Equals(pointC), then it's an absolute certainty that pointA.Equals(pointC). However, if pointD.ApproximatelyEquals(pointE) and pointE.ApproximatelyEquals(pointF), it is NOT
        /// certain whether pointD.ApproximatelyEquals(pointF).
        /// </remarks>
        /// <param name="other">A Point3D structure to compare to this instance.</param>
        /// <returns>True if the X,Y,Z components are approximately equal; false otherwise.</returns>
        public Boolean ApproximatelyEquals( Point3Di other ) =>
            Math.Abs( value: this.X - other.X ) <= Tolerance && Math.Abs( value: this.Y - other.Y ) <= Tolerance && Math.Abs( value: this.Z - other.Z ) <= Tolerance;

        /// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if the object equals this instance; false otherwise.</returns>
        public override Boolean Equals( Object obj ) => obj is Point3Di di && this.Equals( other: di );

        /// <summary>Returns a value indicating whether this instance is equal to a specified Point3D structure.</summary>
        /// <param name="other">A Point3D structure to compare to this instance.</param>
        /// <returns>True if the X,Y,Z components are the same; false otherwise.</returns>
        public Boolean Equals( Point3Di other ) => Equals( left: this, right: other );

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override Int32 GetHashCode() => (this.X, this.Y, this.Z).GetHashCode();

        /// <summary>Returns a String representation of the point in [X,Y,Z] format.</summary>
        /// <returns>A String representing the point's XYZ coordinates.</returns>
        [NotNull]
        public override String ToString() => $"[{this.X}, {this.Y}, {this.Z}]";
    }
}