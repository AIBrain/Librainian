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
// File "Matrix3.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Graphics;

using System;

public class Matrix3 : Matrix {

	public Matrix3() : base( 3, 3 ) {
	}

	public Matrix3( Single[,] matrix ) : base( matrix ) {
		if ( this.Rows != 3 || this.Cols != 3 ) {
			throw new InvalidOperationException( $"{nameof( matrix )} must be 3x3." );
		}
	}

	public static Matrix3 I() =>
		new( new[ , ] {
			{
				1.0f, 0.0f, 0.0f
			}, {
				0.0f, 1.0f, 0.0f
			}, {
				0.0f, 0.0f, 1.0f
			}
		} );

	public static Matrix3 operator *( Matrix3 mat1, Matrix3 mat2 ) {
		var m1 = mat1.matrix;
		var m2 = mat2.matrix;
		var m3 = new Single[ 3, 3 ];
		m3[ 0, 0 ] = m1[ 0, 0 ] * m2[ 0, 0 ] + m1[ 0, 1 ] * m2[ 1, 0 ] + m1[ 0, 2 ] * m2[ 2, 0 ];
		m3[ 0, 1 ] = m1[ 0, 0 ] * m2[ 0, 1 ] + m1[ 0, 1 ] * m2[ 1, 1 ] + m1[ 0, 2 ] * m2[ 2, 1 ];
		m3[ 0, 2 ] = m1[ 0, 0 ] * m2[ 0, 2 ] + m1[ 0, 1 ] * m2[ 1, 2 ] + m1[ 0, 2 ] * m2[ 2, 2 ];
		m3[ 1, 0 ] = m1[ 1, 0 ] * m2[ 0, 0 ] + m1[ 1, 1 ] * m2[ 1, 0 ] + m1[ 1, 2 ] * m2[ 2, 0 ];
		m3[ 1, 1 ] = m1[ 1, 0 ] * m2[ 0, 1 ] + m1[ 1, 1 ] * m2[ 1, 1 ] + m1[ 1, 2 ] * m2[ 2, 1 ];
		m3[ 1, 2 ] = m1[ 1, 0 ] * m2[ 0, 2 ] + m1[ 1, 1 ] * m2[ 1, 2 ] + m1[ 1, 2 ] * m2[ 2, 2 ];
		m3[ 2, 0 ] = m1[ 2, 0 ] * m2[ 0, 0 ] + m1[ 2, 1 ] * m2[ 1, 0 ] + m1[ 2, 2 ] * m2[ 2, 0 ];
		m3[ 2, 1 ] = m1[ 2, 0 ] * m2[ 0, 1 ] + m1[ 2, 1 ] * m2[ 1, 1 ] + m1[ 2, 2 ] * m2[ 2, 1 ];
		m3[ 2, 2 ] = m1[ 2, 0 ] * m2[ 0, 2 ] + m1[ 2, 1 ] * m2[ 1, 2 ] + m1[ 2, 2 ] * m2[ 2, 2 ];

		return new Matrix3( m3 );
	}

	public static Matrix3 operator *( Matrix3 m, Single scalar ) => new( Multiply( m, scalar ) );
}