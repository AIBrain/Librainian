// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Matrix3X3.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Matrix3X3.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Graphics {

    using System;
    using Maths;

    /// <summary>
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/a/8696503/956364" />
    public class Matrix3X3 : ICloneable {

        private const Int32 _M11 = 0;

        private const Int32 _M12 = 1;

        private const Int32 _M13 = 2;

        private const Int32 _M21 = 3;

        private const Int32 _M22 = 4;

        private const Int32 _M23 = 5;

        private const Int32 _M31 = 6;

        private const Int32 _M32 = 7;

        private const Int32 _M33 = 8;

        private readonly Double[] _coeffs;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3X3" /> class.
        /// </summary>
        public Matrix3X3() => this._coeffs = new Double[9];

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3X3" /> class.
        /// </summary>
        /// <param name="coefficients">
        ///     The coefficients to initialise. The number of elements of the array should be equal to 9,
        ///     else an exception will be thrown
        /// </param>
        public Matrix3X3( Double[] coefficients ) {
            if ( coefficients.GetLength( 0 ) != 9 ) { throw new Exception( "The number of coefficients passed in to the constructor must be 9" ); }

            this._coeffs = coefficients;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3X3" /> class.
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
        public Matrix3X3( Double m11, Double m12, Double m13, Double m21, Double m22, Double m23, Double m31, Double m32, Double m33 ) => this._coeffs = new[] { m11, m12, m13, m21, m22, m23, m31, m32, m33 };

        /// <summary>
        ///     Gets the determinant of the matrix
        /// </summary>
        /// <value>The determinant</value>
        public Double Determinant {
            get {

                // |a b c| In general, for a 3X3 matrix |d e f| |g h i|
                //
                // The determinant can be found as follows: a(ei-fh) - b(di-fg) + c(dh-eg)

                // Get coeffs
                var a = this._coeffs[_M11];
                var b = this._coeffs[_M12];
                var c = this._coeffs[_M13];
                var d = this._coeffs[_M21];
                var e = this._coeffs[_M22];
                var f = this._coeffs[_M23];
                var g = this._coeffs[_M31];
                var h = this._coeffs[_M32];
                var i = this._coeffs[_M33];
                var ei = e * i;
                var fh = f * h;
                var di = d * i;
                var fg = f * g;
                var dh = d * h;
                var eg = e * g;

                // Compute the determinant
                return a * ( ei - fh ) - b * ( di - fg ) + c * ( dh - eg );
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this matrix is affine. This will be true if the right column (M13, M23, M33) is 0 0
        ///     1
        /// </summary>
        /// <value><c>true</c> if this instance is affine; otherwise, <c>false</c>.</value>
        public Boolean IsAffine => this._coeffs[_M13].Near( 0 ) && this._coeffs[_M23].Near( 0 ) && this._coeffs[_M33].Near( 1 );

        /// <summary>
        ///     Gets a value indicating whether this matrix is singular. If it is singular, it cannot be inverted
        /// </summary>
        /// <value><c>true</c> if this instance is singular; otherwise, <c>false</c>.</value>
        public Boolean IsSingular => this.Determinant.Near( 0 );

        /// <summary>
        ///     Gets or sets the M11 coefficient
        /// </summary>
        /// <value>The M11</value>
        public Double M11 {
            get => this._coeffs[_M11];

            set => this._coeffs[_M11] = value;
        }

        /// <summary>
        ///     Gets or sets the M12 coefficient
        /// </summary>
        /// <value>The M12</value>
        public Double M12 {
            get => this._coeffs[_M12];

            set => this._coeffs[_M12] = value;
        }

        /// <summary>
        ///     Gets or sets the M13 coefficient
        /// </summary>
        /// <value>The M13</value>
        public Double M13 {
            get => this._coeffs[_M13];

            set => this._coeffs[_M13] = value;
        }

        /// <summary>
        ///     Gets or sets the M21 coefficient
        /// </summary>
        /// <value>The M21</value>
        public Double M21 {
            get => this._coeffs[_M21];

            set => this._coeffs[_M21] = value;
        }

        /// <summary>
        ///     Gets or sets the M22 coefficient
        /// </summary>
        /// <value>The M22</value>
        public Double M22 {
            get => this._coeffs[_M22];

            set => this._coeffs[_M22] = value;
        }

        /// <summary>
        ///     Gets or sets the M23 coefficient
        /// </summary>
        /// <value>The M23</value>
        public Double M23 {
            get => this._coeffs[_M23];

            set => this._coeffs[_M23] = value;
        }

        /// <summary>
        ///     Gets or sets the M31 coefficient
        /// </summary>
        /// <value>The M31</value>
        public Double M31 {
            get => this._coeffs[_M31];

            set => this._coeffs[_M31] = value;
        }

        /// <summary>
        ///     Gets or sets the M32 coefficient
        /// </summary>
        /// <value>The M32</value>
        public Double M32 {
            get => this._coeffs[_M32];

            set => this._coeffs[_M32] = value;
        }

        /// <summary>
        ///     Gets or sets the M33 coefficient
        /// </summary>
        /// <value>The M33</value>
        public Double M33 {
            get => this._coeffs[_M33];

            set => this._coeffs[_M33] = value;
        }

        /// <summary>
        ///     Gets or sets the Translation Offset in the X Direction
        /// </summary>
        /// <value>The M31</value>
        public Double OffsetX {
            get => this._coeffs[_M31];

            set => this._coeffs[_M31] = value;
        }

        // NB: M11, M12, M21, M22 members of IAffineTransformCoefficients are implemented within the #region Public Properties directive
        /// <summary>
        ///     Gets or sets the Translation Offset in the Y Direction
        /// </summary>
        /// <value>The M32</value>
        public Double OffsetY {
            get => this._coeffs[_M32];

            set => this._coeffs[_M32] = value;
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public Object Clone() {
            var coeffCopy = ( Double[] )this._coeffs.Clone();

            return new Matrix3X3( coeffCopy );
        }

        /// <summary>
        ///     Gets the inverse of this matrix. If the matrix is singular, this method will throw an exception
        /// </summary>
        /// <returns>The inverse</returns>
        public Matrix3X3 Inverse() {

            // Taken from http://everything2.com/index.pl?node_id=1271704
            //                                                  a b c
            //In general, the inverse matrix of a 3X3 matrix    d e f
            //                                                  g h i

            //is

            // 1 (ei-fh) (bi-ch) (bf-ce)
            // ----------------------------- x (fg-di) (ai-cg) (cd-af) a(ei-fh) - b(di-fg) + c(dh-eg) (dh-eg) (bg-ah) (ae-bd)

            // Get coeffs
            var a = this._coeffs[_M11];
            var b = this._coeffs[_M12];
            var c = this._coeffs[_M13];
            var d = this._coeffs[_M21];
            var e = this._coeffs[_M22];
            var f = this._coeffs[_M23];
            var g = this._coeffs[_M31];
            var h = this._coeffs[_M32];
            var i = this._coeffs[_M33];

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

            if ( this.Determinant.Near( 0.0 ) ) { throw new Exception( "Unable to invert the matrix as it is singular" ); }

            // Scale the matrix by 1/determinant
            tempMat.Scale( 1.0 / this.Determinant );

            return tempMat;
        }

        /*

                /// <summary>
                /// Initializes a new instance of the <see cref="Matrix3X3"/> class. The IAffineTransformCoefficients passed in is used to populate coefficients M11, M12, M21, M22, M31, M32. The remaining column (M13,
                /// M23, M33) is populated with homogenous values 0 0 1.
                /// </summary>
                /// <param name="affineMatrix">The IAffineTransformCoefficients used to populate M11, M12, M21, M22, M31, M32</param>
                public Matrix3X3( IAffineTransformCoefficients affineTransform ) {
                    _coeffs = new double[] { affineTransform.M11, affineTransform.M12, 0,
                                        affineTransform.M21, affineTransform.M22, 0,
                                        affineTransform.OffsetX, affineTransform.OffsetY, 1};
                }
        */

        /// <summary>
        ///     Makes the matrix an affine matrix by setting the right column (M13, M23, M33) to 0 0 1
        /// </summary>
        public void MakeAffine() {
            this._coeffs[_M13] = 0;
            this._coeffs[_M23] = 0;
            this._coeffs[_M33] = 1;
        }

        /// <summary>
        ///     Multiplies the current matrix by the 3x3 matrix passed in
        /// </summary>
        /// <param name="rhs"></param>
        public void Multiply( Matrix3X3 rhs ) {

            // Get coeffs
            var a = this._coeffs[_M11];
            var b = this._coeffs[_M12];
            var c = this._coeffs[_M13];
            var d = this._coeffs[_M21];
            var e = this._coeffs[_M22];
            var f = this._coeffs[_M23];
            var g = this._coeffs[_M31];
            var h = this._coeffs[_M32];
            var i = this._coeffs[_M33];

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
            // http: //www.maths.surrey.ac.uk/explore/emmaspages/option1.html

            this._coeffs[_M11] = a * j + b * m + c * p;
            this._coeffs[_M12] = a * k + b * n + c * q;
            this._coeffs[_M13] = a * l + b * o + c * r;
            this._coeffs[_M21] = d * j + e * m + f * p;
            this._coeffs[_M22] = d * k + e * n + f * q;
            this._coeffs[_M23] = d * l + e * o + f * r;
            this._coeffs[_M31] = g * j + h * m + i * p;
            this._coeffs[_M32] = g * k + h * n + i * q;
            this._coeffs[_M33] = g * l + h * o + i * r;
        }

        /// <summary>
        ///     Scales the matrix by the specified scalar value
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        public void Scale( Double scalar ) {
            this._coeffs[0] *= scalar;
            this._coeffs[1] *= scalar;
            this._coeffs[2] *= scalar;
            this._coeffs[3] *= scalar;
            this._coeffs[4] *= scalar;
            this._coeffs[5] *= scalar;
            this._coeffs[6] *= scalar;
            this._coeffs[7] *= scalar;
            this._coeffs[8] *= scalar;
        }
    }
}