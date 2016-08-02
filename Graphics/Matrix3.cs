// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Matrix3.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics {

    using System;
    using System.Windows.Media.Media3D;

    public class Matrix3 : Matrix {

        public Matrix3() : base( 3, 3 ) {
        }

        public Matrix3( Single[,] matrix ) : base( matrix ) {
            if ( ( this.Rows != 3 ) || ( this.Cols != 3 ) ) {
                throw new ArgumentException();
            }
        }

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
    }
}