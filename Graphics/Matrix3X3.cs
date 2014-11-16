namespace Librainian.Graphics {
    using System;
    using Maths;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/a/8696503/956364"/>
    public class Matrix3X3 : ICloneable {
        #region Local Variables

        private readonly double[] _coeffs;

        private const int _M11 = 0;
        private const int _M12 = 1;
        private const int _M13 = 2;
        private const int _M21 = 3;
        private const int _M22 = 4;
        private const int _M23 = 5;
        private const int _M31 = 6;
        private const int _M32 = 7;
        private const int _M33 = 8;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3X3"/> class.
        /// </summary>
        public Matrix3X3() {
            _coeffs = new double[ 9 ];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3X3"/> class.
        /// </summary>
        /// <param name="coefficients">The coefficients to initialise. The number of elements of the array should
        /// be equal to 9, else an exception will be thrown</param>
        public Matrix3X3( double[] coefficients ) {
            if ( coefficients.GetLength( 0 ) != 9 )
                throw new Exception( "The number of coefficients passed in to the constructor must be 9" );

            _coeffs = coefficients;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3X3"/> class. 
        /// </summary>
        /// <param name="m11">The M11 coefficient</param>
        /// <param name="m12">The M12 coefficien</param>
        /// <param name="m13">The M13 coefficien</param>
        /// <param name="m21">The M21 coefficien</param>
        /// <param name="m22">The M22 coefficien</param>
        /// <param name="m23">The M23 coefficien</param>
        /// <param name="m31">The M31 coefficien</param>
        /// <param name="m32">The M32 coefficien</param>
        /// <param name="m33">The M33 coefficien</param>
        public Matrix3X3( double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33 ) {
            // The 3x3 matrix is constructed as follows
            //
            // | M11 M12 M13 | 
            // | M21 M22 M23 | 
            // | M31 M32 M33 | 

            _coeffs = new[] { m11, m12, m13, m21, m22, m23, m31, m32, m33 };
        }

        /*
                /// <summary>
                /// Initializes a new instance of the <see cref="Matrix3X3"/> class. The IAffineTransformCoefficients
                /// passed in is used to populate coefficients M11, M12, M21, M22, M31, M32. The remaining column (M13, M23, M33)
                /// is populated with homogenous values 0 0 1.
                /// </summary>
                /// <param name="affineMatrix">The IAffineTransformCoefficients used to populate M11, M12, M21, M22, M31, M32</param>
                public Matrix3X3( IAffineTransformCoefficients affineTransform ) {
                    _coeffs = new double[] { affineTransform.M11, affineTransform.M12, 0, 
                                        affineTransform.M21, affineTransform.M22, 0, 
                                        affineTransform.OffsetX, affineTransform.OffsetY, 1};
                }
        */

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the M11 coefficient
        /// </summary>
        /// <value>The M11</value>
        public double M11 {
            get {
                return _coeffs[ _M11 ];
            }
            set {
                _coeffs[ _M11 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M12 coefficient
        /// </summary>
        /// <value>The M12</value>
        public double M12 {
            get {
                return _coeffs[ _M12 ];
            }
            set {
                _coeffs[ _M12 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M13 coefficient
        /// </summary>
        /// <value>The M13</value>
        public double M13 {
            get {
                return _coeffs[ _M13 ];
            }
            set {
                _coeffs[ _M13 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M21 coefficient
        /// </summary>
        /// <value>The M21</value>
        public double M21 {
            get {
                return _coeffs[ _M21 ];
            }
            set {
                _coeffs[ _M21 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M22 coefficient
        /// </summary>
        /// <value>The M22</value>
        public double M22 {
            get {
                return _coeffs[ _M22 ];
            }
            set {
                _coeffs[ _M22 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M23 coefficient
        /// </summary>
        /// <value>The M23</value>
        public double M23 {
            get {
                return _coeffs[ _M23 ];
            }
            set {
                _coeffs[ _M23 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M31 coefficient
        /// </summary>
        /// <value>The M31</value>
        public double M31 {
            get {
                return _coeffs[ _M31 ];
            }
            set {
                _coeffs[ _M31 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M32 coefficient
        /// </summary>
        /// <value>The M32</value>
        public double M32 {
            get {
                return _coeffs[ _M32 ];
            }
            set {
                _coeffs[ _M32 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the M33 coefficient
        /// </summary>
        /// <value>The M33</value>
        public double M33 {
            get {
                return _coeffs[ _M33 ];
            }
            set {
                _coeffs[ _M33 ] = value;
            }
        }

        /// <summary>
        /// Gets the determinant of the matrix
        /// </summary>
        /// <value>The determinant</value>
        public double Determinant {
            get {
                //                                |a b c|
                // In general, for a 3X3 matrix   |d e f|
                //                                |g h i|
                //
                // The determinant can be found as follows:
                // a(ei-fh) - b(di-fg) + c(dh-eg)

                // Get coeffs
                var a = _coeffs[ _M11 ];
                var b = _coeffs[ _M12 ];
                var c = _coeffs[ _M13 ];
                var d = _coeffs[ _M21 ];
                var e = _coeffs[ _M22 ];
                var f = _coeffs[ _M23 ];
                var g = _coeffs[ _M31 ];
                var h = _coeffs[ _M32 ];
                var i = _coeffs[ _M33 ];
                var ei = e * i;
                var fh = f * h;
                var di = d * i;
                var fg = f * g;
                var dh = d * h;
                var eg = e * g;

                // Compute the determinant
                return ( a * ( ei - fh ) ) - ( b * ( di - fg ) ) + ( c * ( dh - eg ) );
            }
        }

        /// <summary>
        /// Gets a value indicating whether this matrix is singular. If it is singular, it cannot be inverted
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is singular; otherwise, <c>false</c>.
        /// </value>
        public bool IsSingular {
            get {
                return Determinant.Near( 0 );
            }
        }

        /// <summary>
        /// Gets the inverse of this matrix. If the matrix is singular, this method will throw an exception
        /// </summary>
        /// <value>The inverse</value>
        public Matrix3X3 Inverse {
            get {
                // Taken from http://everything2.com/index.pl?node_id=1271704
                //                                                  a b c
                //In general, the inverse matrix of a 3X3 matrix    d e f
                //                                                  g h i

                //is 

                //        1                              (ei-fh)   (bi-ch)   (bf-ce)
                // -----------------------------   x     (fg-di)   (ai-cg)   (cd-af)
                // a(ei-fh) - b(di-fg) + c(dh-eg)        (dh-eg)   (bg-ah)   (ae-bd)

                // Get coeffs
                var a = _coeffs[ _M11 ];
                var b = _coeffs[ _M12 ];
                var c = _coeffs[ _M13 ];
                var d = _coeffs[ _M21 ];
                var e = _coeffs[ _M22 ];
                var f = _coeffs[ _M23 ];
                var g = _coeffs[ _M31 ];
                var h = _coeffs[ _M32 ];
                var i = _coeffs[ _M33 ];

                //// Compute often used components
                var ei = e * i;
                var fh = f * h;
                var di = d * i;
                var fg = f * g;
                var dh = d * h;
                var eg = e * g;
                var bi = b * i;
                var ch = c * h;
                var ai = a * i;
                var cg = c * g;
                var cd = c * d;
                var bg = b * g;
                var ah = a * h;
                var ae = a * e;
                var bd = b * d;
                var bf = b * f;
                var ce = c * e;
                var cf = c * d;
                var af = a * f;

                // Construct the matrix using these components
                var tempMat = new Matrix3X3( ei - fh, ch - bi, bf - ce, fg - di, ai - cg, cd - af, dh - eg, bg - ah, ae - bd );

                // Compute the determinant

                if ( Determinant.Near( 0.0 ) ) {
                    throw new Exception( "Unable to invert the matrix as it is singular" );
                }

                // Scale the matrix by 1/determinant
                tempMat.Scale( 1.0 / Determinant );

                return tempMat;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this matrix is affine. This will be true if the right column 
        /// (M13, M23, M33) is 0 0 1
        /// </summary>
        /// <value><c>true</c> if this instance is affine; otherwise, <c>false</c>.</value>
        public bool IsAffine {
            get {
                return ( _coeffs[ _M13 ].Near( 0 ) && _coeffs[ _M23 ].Near( 0 ) && _coeffs[ _M33 ].Near( 1 ) );
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Multiplies the current matrix by the 3x3 matrix passed in
        /// </summary>
        /// <param name="rhs"></param>
        public void Multiply( Matrix3X3 rhs ) {
            // Get coeffs
            var a = _coeffs[ _M11 ];
            var b = _coeffs[ _M12 ];
            var c = _coeffs[ _M13 ];
            var d = _coeffs[ _M21 ];
            var e = _coeffs[ _M22 ];
            var f = _coeffs[ _M23 ];
            var g = _coeffs[ _M31 ];
            var h = _coeffs[ _M32 ];
            var i = _coeffs[ _M33 ];

            var j = rhs.M11;
            var k = rhs.M12;
            var l = rhs.M13;
            var m = rhs.M21;
            var n = rhs.M22;
            var o = rhs.M23;
            var p = rhs.M31;
            var q = rhs.M32;
            var r = rhs.M33;

            // Perform multiplication. Formula taken from
            // http://www.maths.surrey.ac.uk/explore/emmaspages/option1.html

            _coeffs[ _M11 ] = a * j + b * m + c * p;
            _coeffs[ _M12 ] = a * k + b * n + c * q;
            _coeffs[ _M13 ] = a * l + b * o + c * r;
            _coeffs[ _M21 ] = d * j + e * m + f * p;
            _coeffs[ _M22 ] = d * k + e * n + f * q;
            _coeffs[ _M23 ] = d * l + e * o + f * r;
            _coeffs[ _M31 ] = g * j + h * m + i * p;
            _coeffs[ _M32 ] = g * k + h * n + i * q;
            _coeffs[ _M33 ] = g * l + h * o + i * r;
        }

        /// <summary>
        /// Scales the matrix by the specified scalar value
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        public void Scale( double scalar ) {
            _coeffs[ 0 ] *= scalar;
            _coeffs[ 1 ] *= scalar;
            _coeffs[ 2 ] *= scalar;
            _coeffs[ 3 ] *= scalar;
            _coeffs[ 4 ] *= scalar;
            _coeffs[ 5 ] *= scalar;
            _coeffs[ 6 ] *= scalar;
            _coeffs[ 7 ] *= scalar;
            _coeffs[ 8 ] *= scalar;
        }

        /// <summary>
        /// Makes the matrix an affine matrix by setting the right column (M13, M23, M33) to 0 0 1
        /// </summary>
        public void MakeAffine() {
            _coeffs[ _M13 ] = 0;
            _coeffs[ _M23 ] = 0;
            _coeffs[ _M33 ] = 1;
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
            var coeffCopy = ( double[] )_coeffs.Clone();
            return new Matrix3X3( coeffCopy );
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
                return _coeffs[ _M31 ];
            }
            set {
                _coeffs[ _M31 ] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Translation Offset in the Y Direction
        /// </summary>
        /// <value>The M32</value>
        public double OffsetY {
            get {
                return _coeffs[ _M32 ];
            }
            set {
                _coeffs[ _M32 ] = value;
            }
        }

        #endregion
    }
}
