// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Matrix3.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Matrix3.cs" was last formatted by Protiguous on 2018/06/04 at 3:57 PM.

namespace Librainian.Graphics {

	using System;
	using System.Windows.Media.Media3D;

	public class Matrix3 : Matrix {

		public static Matrix3 I() => new Matrix3( new[ , ] { { 1.0f, 0.0f, 0.0f }, { 0.0f, 1.0f, 0.0f }, { 0.0f, 0.0f, 1.0f } } );

		public static Vector3D operator *( Matrix3 matrix3, Vector3D v ) {
			var m = matrix3.matrix;

			return new Vector3D( m[ 0, 0 ] * v.X + m[ 0, 1 ] * v.Y + m[ 0, 2 ] * v.Z, m[ 1, 0 ] * v.X + m[ 1, 1 ] * v.Y + m[ 1, 2 ] * v.Z, m[ 2, 0 ] * v.X + m[ 2, 1 ] * v.Y + m[ 2, 2 ] * v.Z );
		}

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

		public static Matrix3 operator *( Matrix3 m, Single scalar ) => new Matrix3( Multiply( m, scalar ) );

		public Matrix3() : base( 3, 3 ) { }

		public Matrix3( Single[ , ] matrix ) : base( matrix ) {
			if ( this.Rows != 3 || this.Cols != 3 ) { throw new ArgumentException(); }
		}

	}

}