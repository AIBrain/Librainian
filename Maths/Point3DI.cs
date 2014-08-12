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
// "Librainian/Point3DI.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Maths {
    using System;

    /// <summary>
    ///     Represents a location in 3D space.
    /// </summary>
    /// <remarks>
    ///     Culled from the file CPI.Plot3D.cs
    ///     I don't know where it came from otherwise I'd attribute it!
    /// </remarks>
    public struct Point3DI : IEquatable< Point3DI > {
        # region Constants
        /// <summary>
        ///     The maximum distance two coordinates can be from each other
        ///     for them to be considered approximately equal.
        /// </summary>
        public const int Tolerance = 0;
        # endregion

        # region Private Fields
        # endregion

        # region Constructors
        /// <summary>
        ///     Instantiates a new Point3D.
        /// </summary>
        /// <param name="x">The point's X coordinate.</param>
        /// <param name="y">The point's Y coordinate.</param>
        /// <param name="z">The point's Z coordinate.</param>
        public Point3DI( int x, int y, int z ) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        # endregion

        # region Properties
        /// <summary>
        ///     The point's X coordinate.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        ///     The point's Y coordinate.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        ///     The point's Z coordinate.
        /// </summary>
        public int Z { get; private set; }
        # endregion

        # region Methods
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
        ///     Determines whether this instance is very nearly equal to a specified Point3D structure.
        /// </summary>
        /// <remarks>
        ///     Since floating point math is kind of fuzzy, we're taking a "close enough" approach
        ///     to equality with this method.  If the individual coordinates of two points fall within
        ///     a small tolerance, we'll consider them to be approximately equal.
        ///     Remember, though, that the uncertainty here can be cumulative.  For example:
        ///     if pointA.Equals(pointB) and pointB.Equals(pointC), then it's an absolute certainty
        ///     that pointA.Equals(pointC).
        ///     However, if pointD.ApproximatelyEquals(pointE) and pointE.ApproximatelyEquals(pointF),
        ///     it is NOT certain whether pointD.ApproximatelyEquals(pointF).
        /// </remarks>
        /// <param name="other">A Point3D structure to compare to this instance.</param>
        /// <returns>True if the X,Y,Z components are approximately equal; false otherwise.</returns>
        public Boolean ApproximatelyEquals( Point3DI other ) {
            return ( ( Math.Abs( this.X - other.X ) < Tolerance ) && ( Math.Abs( this.Y - other.Y ) < Tolerance ) && ( Math.Abs( this.Z - other.Z ) < Tolerance ) );
        }
        # endregion

        # region Overridden Methods
        /// <summary>
        ///     Returns a String representation of the point in [X,Y,Z] format.
        /// </summary>
        /// <returns>A String representing the point's XYZ coordinates.</returns>
        public override String ToString() {
            return String.Format( "[{0}, {1}, {2}]", this.X, this.Y, this.Z );
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>True if the object equals this instance; false otherwise.</returns>
        public override Boolean Equals( object obj ) {
            return obj is Point3DI && this.Equals( ( Point3DI ) obj );
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <remarks>
        ///     The hash code is based on the hash codes of the X, Y, and Z coordinates of the point,
        ///     but we can't just XOR them all together, otherwise [3,4,5] would return the same hash
        ///     code as [5,3,4], and we wouldn't want that.  So to get a more even distribution, we
        ///     rotate hashY's bits by 8, and hashZ's bits by 16, then we XOR them all together.
        ///     (It's also worth pointing out that we're casting the individual hash codes to uints before
        ///     operating on them because we want our shift operations to use unsigned semantics.)
        /// </remarks>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() {
            return this.X.GetHashMerge( this.Y.GetHashMerge( this.Z ) );
        }
        # endregion

        # region Overloaded Operators
        /// <summary>
        ///     Determines whether the specified Point3D instances are equal.
        /// </summary>
        /// <param name="a">The first Point3D instance to compare.</param>
        /// <param name="b">The second Point3D instance to compare.</param>
        /// <returns>True if the Point3D instances are equal; false otherwise.</returns>
        public static Boolean operator ==( Point3DI a, Point3DI b ) {
            return a.Equals( b );
        }

        /// <summary>
        ///     Determines whether the specified Point3D instances are unequal.
        /// </summary>
        /// <param name="a">The first Point3D instance to compare.</param>
        /// <param name="b">The second Point3D instance to compare.</param>
        /// <returns>True if the Point3D instances are unequal; false otherwise.</returns>
        public static Boolean operator !=( Point3DI a, Point3DI b ) {
            return !a.Equals( b );
        }
        # endregion

        #region IEquatable<Point3DI> Members
        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified Point3D structure.
        /// </summary>
        /// <param name="other">A Point3D structure to compare to this instance.</param>
        /// <returns>True if the X,Y,Z components are the same; false otherwise.</returns>
        public Boolean Equals( Point3DI other ) {
            return ( this.X == other.X && this.Y == other.Y && this.Z == other.Z );
        }
        #endregion
    }
}
