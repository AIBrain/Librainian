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
// "Librainian/Point3DI.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics {

    using System;
    using Maths;

    /// <summary>Represents a location in 3D integer space.</summary>
    /// <remarks>
    ///     Culled from the file CPI.Plot3D.cs I don't know where that file came from otherwise I'd attribute it!
    /// </remarks>
    public struct Point3Di : IEquatable<Point3Di> {

        /// <summary>
        ///     The maximum distance two coordinates can be from each other for them to be considered
        ///     approximately equal.
        /// </summary>
        public const Int32 Tolerance = 0;

        /// <summary>Instantiates a new Point3D.</summary>
        /// <param name="x">The point's X coordinate.</param>
        /// <param name="y">The point's Y coordinate.</param>
        /// <param name="z">The point's Z coordinate.</param>
        public Point3Di( Int32 x, Int32 y, Int32 z ) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>The point's X coordinate.</summary>
        public Int32 X {
            get;
        }

        /// <summary>The point's Y coordinate.</summary>
        public Int32 Y {
            get;
        }

        /// <summary>The point's Z coordinate.</summary>
        public Int32 Z {
            get;
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

        /// <summary>
        ///     Static comparison.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean Equals( Point3Di lhs, Point3Di rhs ) => lhs.ApproximatelyEquals( rhs );

        /// <summary>Determines whether the specified Point3D instances are unequal.</summary>
        /// <param name="a">The first Point3D instance to compare.</param>
        /// <param name="b">The second Point3D instance to compare.</param>
        /// <returns>True if the Point3D instances are unequal; false otherwise.</returns>
        public static Boolean operator !=( Point3Di a, Point3Di b ) => !a.Equals( b );

        /// <summary>Determines whether the specified Point3D instances are equal.</summary>
        /// <param name="a">The first Point3D instance to compare.</param>
        /// <param name="b">The second Point3D instance to compare.</param>
        /// <returns>True if the Point3D instances are equal; false otherwise.</returns>
        public static Boolean operator ==( Point3Di a, Point3Di b ) => a.Equals( b );

        /// <summary>
        ///     Determines whether this instance is very nearly equal to a specified Point3D structure.
        /// </summary>
        /// <remarks>
        ///     Since floating point math is kind of fuzzy, we're taking a "close enough" approach to
        ///     equality with this method. If the individual coordinates of two points fall within a
        ///     small tolerance, we'll consider them to be approximately equal. Remember, though, that
        ///     the uncertainty here can be cumulative. For example: if pointA.Equals(pointB) and
        ///     pointB.Equals(pointC), then it's an absolute certainty that pointA.Equals(pointC).
        ///     However, if pointD.ApproximatelyEquals(pointE) and pointE.ApproximatelyEquals(pointF),
        ///     it is NOT certain whether pointD.ApproximatelyEquals(pointF).
        /// </remarks>
        /// <param name="other">A Point3D structure to compare to this instance.</param>
        /// <returns>True if the X,Y,Z components are approximately equal; false otherwise.</returns>
        public Boolean ApproximatelyEquals( Point3Di other ) => Math.Abs( this.X - other.X ) <= Point3Di.Tolerance && Math.Abs( this.Y - other.Y ) <= Point3Di.Tolerance && Math.Abs( this.Z - other.Z ) <= Point3Di.Tolerance;

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if the object equals this instance; false otherwise.</returns>
        public override Boolean Equals( Object obj ) => obj is Point3Di && this.Equals( ( Point3Di )obj );

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified Point3D structure.
        /// </summary>
        /// <param name="other">A Point3D structure to compare to this instance.</param>
        /// <returns>True if the X,Y,Z components are the same; false otherwise.</returns>
        public Boolean Equals( Point3Di other ) => Equals( this, other );

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override Int32 GetHashCode() => Hashing.GetHashCodes( this.X, this.Y, this.Z );

        /// <summary>Returns a String representation of the point in [X,Y,Z] format.</summary>
        /// <returns>A String representing the point's XYZ coordinates.</returns>
        public override String ToString() => $"[{this.X}, {this.Y}, {this.Z}]";
    }
}