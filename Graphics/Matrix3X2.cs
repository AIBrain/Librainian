namespace Librainian.Graphics {
    using System;


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/a/8696503/956364"/>
    public class Matrix3X2 : ICloneable {
        #region Local Variables

        private double[] coeffs;

        private const int _M11 = 0;
        private const int _M12 = 1;
        private const int _M21 = 2;
        private const int _M22 = 3;
        private const int _M31 = 4;
        private const int _M32 = 5;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3X2"/> class.
        /// </summary>
        public Matrix3X2() {
            coeffs = new double[ 6 ];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3X2"/> class.
        /// </summary>
        /// <param name="coefficients">The coefficients to initialise. The number of elements of the array should
        /// be equal to 6, else an exception will be thrown</param>
        public Matrix3X2( double[] coefficients ) {
            if ( coefficients.GetLength( 0 ) != 6 )
                throw new Exception( "The number of coefficients passed in to the constructor must be 6" );

            coeffs = coefficients;
        }

        public Matrix3X2( double m11, double m12, double m21, double m22, double m31, double m32 ) {
            coeffs = new[] { m11, m12, m21, m22, m31, m32 };
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="Matrix3X2"/> class. The IAffineTransformCoefficients
        ///// passed in is used to populate coefficients M11, M12, M21, M22, M31, M32.
        ///// </summary>
        ///// <param name="affineMatrix">The IAffineTransformCoefficients used to populate M11, M12, M21, M22, M31, M32</param>
        //public Matrix3X2( IAffineTransformCoefficients affineTransform ) {
        //    coeffs = new double[] { affineTransform.M11, affineTransform.M12, 
        //                        affineTransform.M21, affineTransform.M22, 
        //                        affineTransform.OffsetX, affineTransform.OffsetY};
        //}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the M11 coefficient
        /// </summary>
        /// <value>The M11</value>
        public double M11 {
            get {
                return coeffs[ _M11 ];
            }
            set {
                coeffs[ _M11 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M12 coefficient
        /// </summary>
        /// <value>The M12</value>
        public double M12 {
            get {
                return coeffs[ _M12 ];
            }
            set {
                coeffs[ _M12 ] = value;
            }
        }


        /// <summary>
        /// Gets or sets the M21 coefficient
        /// </summary>
        /// <value>The M21</value>
        public double M21 {
            get {
                return coeffs[ _M21 ];
            }
            set {
                coeffs[ _M21 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M22 coefficient
        /// </summary>
        /// <value>The M22</value>
        public double M22 {
            get {
                return coeffs[ _M22 ];
            }
            set {
                coeffs[ _M22 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M31 coefficient
        /// </summary>
        /// <value>The M31</value>
        public double M31 {
            get {
                return coeffs[ _M31 ];
            }
            set {
                coeffs[ _M31 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M32 coefficient
        /// </summary>
        /// <value>The M32</value>
        public double M32 {
            get {
                return coeffs[ _M32 ];
            }
            set {
                coeffs[ _M32 ] = value;
            }
        }

        #endregion

        #region Public Methods

/*
        /// <summary>
        /// Transforms the the ILocation passed in and returns the result in a new ILocation
        /// </summary>
        /// <param name="location">The location to transform</param>
        /// <returns>The transformed location</returns>
        public ILocation Transform( ILocation location ) {
            // Perform the following equation:
            //
            // | x y 1 |   | M11 M12 |   |(xM11 + yM21 + M31) (xM12 + yM22 + M32)|
            //           * | M21 M22 | = 
            //             | M31 M32 | 

            var x = location.X * coeffs[ _M11 ] + location.Y * coeffs[ _M21 ] + coeffs[ _M31 ];
            var y = location.X * coeffs[ _M12 ] + location.Y * coeffs[ _M22 ] + coeffs[ _M32 ];

            return new Location( x, y );
        }
*/

        /// <summary>
        /// Multiplies the 3x3 matrix passed in with the current 3x2 matrix
        /// </summary>
        /// <param name="lhs">The 3x3 Matrix X</param>
        public void Multiply( Matrix3X3 lhs ) {
            // Multiply the 3x3 matrix with the 3x2 matrix and store inside the current 2x3 matrix
            // 
            // [a b c]   [j k]   [(aj + bl + cn) (ak + bm + co)]
            // [d e f] * [l m] = [(dj + el + fn) (dk + em + fo)]
            // [g h i]   [n o]   [(gj + hl + in) (gk + hm + io)]

            // Get coeffs
            var a = lhs.M11;
            var b = lhs.M12;
            var c = lhs.M13;
            var d = lhs.M21;
            var e = lhs.M22;
            var f = lhs.M23;
            var g = lhs.M31;
            var h = lhs.M32;
            var i = lhs.M33;

            var j = coeffs[ _M11 ];
            var k = coeffs[ _M12 ];
            var l = coeffs[ _M21 ];
            var m = coeffs[ _M22 ];
            var n = coeffs[ _M31 ];
            var o = coeffs[ _M32 ];

            coeffs[ _M11 ] = a * j + b * l + c * n;
            coeffs[ _M12 ] = a * k + b * m + c * o;
            coeffs[ _M21 ] = d * j + e * l + f * n;
            coeffs[ _M22 ] = d * k + e * m + f * o;
            coeffs[ _M31 ] = g * j + h * l + i * n;
            coeffs[ _M32 ] = g * k + h * m + i * o;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone() {
            var coeffCopy = ( double[] )coeffs.Clone();
            return new Matrix3X2( coeffCopy );
        }

        #endregion

        #region IAffineTransformCoefficients Members

        //
        // NB: M11, M12, M21, M22 members of IAffineTransformCoefficients are implemented within the
        // #region Public Properties directive
        //

        /// <summary>
        /// Gets or sets the Translation Offset in the X Direction
        /// </summary>
        /// <value>The M31</value>
        public double OffsetX {
            get {
                return coeffs[ _M31 ];
            }
            set {
                coeffs[ _M31 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Translation Offset in the Y Direction
        /// </summary>
        /// <value>The M32</value>
        public double OffsetY {
            get {
                return coeffs[ _M32 ];
            }
            set {
                coeffs[ _M32 ] = value;
            }
        }

        #endregion
    }
}
