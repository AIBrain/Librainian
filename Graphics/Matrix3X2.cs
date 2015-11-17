// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Matrix3X2.cs" was last cleaned by Rick on 2015/06/12 at 2:55 PM

namespace Librainian.Graphics {

    using System;

    /// <summary></summary>
    /// <seealso cref="http://stackoverflow.com/a/8696503/956364" />
    public class Matrix3X2 : ICloneable {
        private const Int32 _M11 = 0;
        private const Int32 _M12 = 1;
        private const Int32 _M21 = 2;
        private const Int32 _M22 = 3;
        private const Int32 _M31 = 4;
        private const Int32 _M32 = 5;
        private readonly Double[] _coeffs;

        /// <summary>Gets or sets the M11 coefficient</summary>
        /// <value>The M11</value>
        public Double M11 {
            get {
                return this._coeffs[ _M11 ];
            }

            set {
                this._coeffs[ _M11 ] = value;
            }
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
        /// <summary>
        ///     Gets or sets the M12 coefficient
        /// </summary>
        /// <value>The M12</value>
        public Double M12 {
            get {
                return this._coeffs[ _M12 ];
            }

            set {
                this._coeffs[ _M12 ] = value;
            }
        }

        /// <summary>Gets or sets the M21 coefficient</summary>
        /// <value>The M21</value>
        public Double M21 {
            get {
                return this._coeffs[ _M21 ];
            }

            set {
                this._coeffs[ _M21 ] = value;
            }
        }

        /// <summary>Gets or sets the M22 coefficient</summary>
        /// <value>The M22</value>
        public Double M22 {
            get {
                return this._coeffs[ _M22 ];
            }

            set {
                this._coeffs[ _M22 ] = value;
            }
        }

        /// <summary>Gets or sets the M31 coefficient</summary>
        /// <value>The M31</value>
        public Double M31 {
            get {
                return this._coeffs[ _M31 ];
            }

            set {
                this._coeffs[ _M31 ] = value;
            }
        }

        /// <summary>Gets or sets the M32 coefficient</summary>
        /// <value>The M32</value>
        public Double M32 {
            get {
                return this._coeffs[ _M32 ];
            }

            set {
                this._coeffs[ _M32 ] = value;
            }
        }

        /// <summary>Gets or sets the Translation Offset in the X Direction</summary>
        /// <value>The M31</value>
        public Double OffsetX {
            get {
                return this._coeffs[ _M31 ];
            }

            set {
                this._coeffs[ _M31 ] = value;
            }
        }

        // NB: M11, M12, M21, M22 members of IAffineTransformCoefficients are implemented within the
        // #region Public Properties directive
        /// <summary>Gets or sets the Translation Offset in the Y Direction</summary>
        /// <value>The M32</value>
        public Double OffsetY {
            get {
                return this._coeffs[ _M32 ];
            }

            set {
                this._coeffs[ _M32 ] = value;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="Matrix3X2" /> class.</summary>
        public Matrix3X2() {
            this._coeffs = new Double[ 6 ];
        }

        /// <summary>Initializes a new instance of the <see cref="Matrix3X2" /> class.</summary>
        /// <param name="coefficients">
        /// The coefficients to initialise. The number of elements of the array should be equal to
        /// 6, else an exception will be thrown
        /// </param>
        public Matrix3X2(Double[] coefficients) {
            if ( coefficients.GetLength( 0 ) != 6 ) {
                throw new Exception( "The number of coefficients passed in to the constructor must be 6" );
            }

            this._coeffs = coefficients;
        }

        public Matrix3X2(Double m11, Double m12, Double m21, Double m22, Double m31, Double m32) {
            this._coeffs = new[] { m11, m12, m21, m22, m31, m32 };
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public Object Clone() {
            var coeffCopy = ( Double[] )this._coeffs.Clone();
            return new Matrix3X2( coeffCopy );
        }

        /*

                /// <summary>
                /// Transforms the the ILocation passed in and returns the result in a new ILocation
                /// </summary>
                /// <param name="location">The location to transform</param>
                /// <returns>The transformed location</returns>
                public ILocation Transform( ILocation location ) {

                    // Perform the following equation:
                    // 
                    // | x y 1 | | M11 M12 | |(xM11 + yM21 + M31) (xM12 + yM22 + M32)|
                    // * | M21 M22 | = | M31 M32 |

                    var x = location.X * coeffs[ _M11 ] + location.Y * coeffs[ _M21 ] + coeffs[ _M31 ];
                    var y = location.X * coeffs[ _M12 ] + location.Y * coeffs[ _M22 ] + coeffs[ _M32 ];

                    return new Location( x, y );
                }
        */

        /// <summary>Multiplies the 3x3 matrix passed in with the current 3x2 matrix</summary>
        /// <param name="lhs">The 3x3 Matrix X</param>
        public void Multiply(Matrix3X3 lhs) {

            // Multiply the 3x3 matrix with the 3x2 matrix and store inside the current 2x3 matrix
            // 
            // [a b c] [j k] [(aj + bl + cn) (ak + bm + co)] [d e f] * [l m] = [(dj + el + fn) (dk +
            // em + fo)] [g h i] [n o] [(gj + hl + in) (gk + hm + io)]

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

            var j = this._coeffs[ _M11 ];
            var k = this._coeffs[ _M12 ];
            var l = this._coeffs[ _M21 ];
            var m = this._coeffs[ _M22 ];
            var n = this._coeffs[ _M31 ];
            var o = this._coeffs[ _M32 ];

            this._coeffs[ _M11 ] = a * j + b * l + c * n;
            this._coeffs[ _M12 ] = a * k + b * m + c * o;
            this._coeffs[ _M21 ] = d * j + e * l + f * n;
            this._coeffs[ _M22 ] = d * k + e * m + f * o;
            this._coeffs[ _M31 ] = g * j + h * l + i * n;
            this._coeffs[ _M32 ] = g * k + h * m + i * o;
        }
    }
}