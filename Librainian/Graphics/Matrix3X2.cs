// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Matrix3X2.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Graphics;

using System;

/// <see cref="http://stackoverflow.com/a/8696503/956364" />
public class Matrix3X2 : ICloneable {

	private const Int32 _M11 = 0;

	private const Int32 _M12 = 1;

	private const Int32 _M21 = 2;

	private const Int32 _M22 = 3;

	private const Int32 _M31 = 4;

	private const Int32 _M32 = 5;

	private readonly Double[] _coeffs;

	/// <summary>Initializes a new instance of the <see cref="Matrix3X2" /> class.</summary>
	public Matrix3X2() => this._coeffs = new Double[ 6 ];

	/// <summary>Initializes a new instance of the <see cref="Matrix3X2" /> class.</summary>
	/// <param name="coefficients">
	/// The coefficients to initialise. The number of elements of the array should be equal to 6, else an exception will be thrown
	/// </param>
	public Matrix3X2( Double[] coefficients ) {
		if ( coefficients.GetLength( 0 ) != 6 ) {
			throw new Exception( "The number of coefficients passed in to the constructor must be 6" );
		}

		this._coeffs = coefficients;
	}

	public Matrix3X2( Double m11, Double m12, Double m21, Double m22, Double m31, Double m32 ) =>
		this._coeffs = new[] {
			m11, m12, m21, m22, m31, m32
		};

	/// <summary>Gets or sets the M11 coefficient</summary>
	/// <value>The M11</value>
	public Double M11 {
		get => this._coeffs[ _M11 ];

		set => this._coeffs[ _M11 ] = value;
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
	/// <summary>Gets or sets the M12 coefficient</summary>
	/// <value>The M12</value>
	public Double M12 {
		get => this._coeffs[ _M12 ];

		set => this._coeffs[ _M12 ] = value;
	}

	/// <summary>Gets or sets the M21 coefficient</summary>
	/// <value>The M21</value>
	public Double M21 {
		get => this._coeffs[ _M21 ];

		set => this._coeffs[ _M21 ] = value;
	}

	/// <summary>Gets or sets the M22 coefficient</summary>
	/// <value>The M22</value>
	public Double M22 {
		get => this._coeffs[ _M22 ];

		set => this._coeffs[ _M22 ] = value;
	}

	/// <summary>Gets or sets the M31 coefficient</summary>
	/// <value>The M31</value>
	public Double M31 {
		get => this._coeffs[ _M31 ];

		set => this._coeffs[ _M31 ] = value;
	}

	/// <summary>Gets or sets the M32 coefficient</summary>
	/// <value>The M32</value>
	public Double M32 {
		get => this._coeffs[ _M32 ];

		set => this._coeffs[ _M32 ] = value;
	}

	/// <summary>Gets or sets the Translation Offset in the X Direction</summary>
	/// <value>The M31</value>
	public Double OffsetX {
		get => this._coeffs[ _M31 ];

		set => this._coeffs[ _M31 ] = value;
	}

	// NB: M11, M12, M21, M22 members of IAffineTransformCoefficients are implemented within the #region Public Properties directive
	/// <summary>Gets or sets the Translation Offset in the Y Direction</summary>
	/// <value>The M32</value>
	public Double OffsetY {
		get => this._coeffs[ _M32 ];

		set => this._coeffs[ _M32 ] = value;
	}

	/// <summary>Creates a new object that is a copy of the current instance.</summary>
	/// <returns>A new object that is a copy of this instance.</returns>
	public Object Clone() {
		var coeffCopy = ( Double[] )this._coeffs.Clone();

		return new Matrix3X2( coeffCopy );
	}

	/*

            /// <summary>Transforms the the ILocation passed in and returns the result in a new ILocation</summary>
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
	/// <param name="left">The 3x3 Matrix X</param>
	public void Multiply( Matrix3X3 left ) {

		// Multiply the 3x3 matrix with the 3x2 matrix and store inside the current 2x3 matrix
		//
		// [a b c] [j k] [(aj + bl + cn) (ak + bm + co)] [d e f] * [l m] = [(dj + el + fn) (dk + em + fo)] [g h i] [n o] [(gj +
		// hl + in) (gk + hm + io)]

		// Get coeffs
		var a = left.M11;
		var b = left.M12;
		var c = left.M13;
		var d = left.M21;
		var e = left.M22;
		var f = left.M23;
		var g = left.M31;
		var h = left.M32;
		var i = left.M33;

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