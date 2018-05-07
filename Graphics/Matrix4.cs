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
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Matrix4.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics {

    using System;
    using System.Windows.Media.Media3D;

    internal class Matrix4 : Matrix {
        public static Matrix4 I = NewI();

        public Matrix4() : base( 4, 4 ) {
        }

        public Matrix4( Single[,] matrix ) : base( matrix ) {
            if ( this.Rows != 4 || this.Cols != 4 ) {
                throw new ArgumentException();
            }
        }

        public static Matrix4 NewI() => new Matrix4( new[ , ] { { 1.0f, 0.0f, 0.0f, 0.0f }, { 0.0f, 1.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 1.0f, 0.0f }, { 0.0f, 0.0f, 0.0f, 1.0f } } );

        public static Vector3D operator *( Matrix4 matrix4, Vector3D v ) {
            var m = matrix4.matrix;
            var w = m[ 3, 0 ] * v.X + m[ 3, 1 ] * v.Y + m[ 3, 2 ] * v.Z + m[ 3, 3 ];
            return new Vector3D( ( m[ 0, 0 ] * v.X + m[ 0, 1 ] * v.Y + m[ 0, 2 ] * v.Z + m[ 0, 3 ] ) / w, ( m[ 1, 0 ] * v.X + m[ 1, 1 ] * v.Y + m[ 1, 2 ] * v.Z + m[ 1, 3 ] ) / w, ( m[ 2, 0 ] * v.X + m[ 2, 1 ] * v.Y + m[ 2, 2 ] * v.Z + m[ 2, 3 ] ) / w );
        }

        public static Matrix4 operator *( Matrix4 mat1, Matrix4 mat2 ) {
            var m1 = mat1.matrix;
            var m2 = mat2.matrix;
            var m3 = new Single[ 4, 4 ];
            m3[ 0, 0 ] = m1[ 0, 0 ] * m2[ 0, 0 ] + m1[ 0, 1 ] * m2[ 1, 0 ] + m1[ 0, 2 ] * m2[ 2, 0 ] + m1[ 0, 3 ] * m2[ 3, 0 ];
            m3[ 0, 1 ] = m1[ 0, 0 ] * m2[ 0, 1 ] + m1[ 0, 1 ] * m2[ 1, 1 ] + m1[ 0, 2 ] * m2[ 2, 1 ] + m1[ 0, 3 ] * m2[ 3, 1 ];
            m3[ 0, 2 ] = m1[ 0, 0 ] * m2[ 0, 2 ] + m1[ 0, 1 ] * m2[ 1, 2 ] + m1[ 0, 2 ] * m2[ 2, 2 ] + m1[ 0, 3 ] * m2[ 3, 2 ];
            m3[ 0, 3 ] = m1[ 0, 0 ] * m2[ 0, 3 ] + m1[ 0, 1 ] * m2[ 1, 3 ] + m1[ 0, 2 ] * m2[ 2, 3 ] + m1[ 0, 3 ] * m2[ 3, 3 ];
            m3[ 1, 0 ] = m1[ 1, 0 ] * m2[ 0, 0 ] + m1[ 1, 1 ] * m2[ 1, 0 ] + m1[ 1, 2 ] * m2[ 2, 0 ] + m1[ 1, 3 ] * m2[ 3, 0 ];
            m3[ 1, 1 ] = m1[ 1, 0 ] * m2[ 0, 1 ] + m1[ 1, 1 ] * m2[ 1, 1 ] + m1[ 1, 2 ] * m2[ 2, 1 ] + m1[ 1, 3 ] * m2[ 3, 1 ];
            m3[ 1, 2 ] = m1[ 1, 0 ] * m2[ 0, 2 ] + m1[ 1, 1 ] * m2[ 1, 2 ] + m1[ 1, 2 ] * m2[ 2, 2 ] + m1[ 1, 3 ] * m2[ 3, 2 ];
            m3[ 1, 3 ] = m1[ 1, 0 ] * m2[ 0, 3 ] + m1[ 1, 1 ] * m2[ 1, 3 ] + m1[ 1, 2 ] * m2[ 2, 3 ] + m1[ 1, 3 ] * m2[ 3, 3 ];
            m3[ 2, 0 ] = m1[ 2, 0 ] * m2[ 0, 0 ] + m1[ 2, 1 ] * m2[ 1, 0 ] + m1[ 2, 2 ] * m2[ 2, 0 ] + m1[ 2, 3 ] * m2[ 3, 0 ];
            m3[ 2, 1 ] = m1[ 2, 0 ] * m2[ 0, 1 ] + m1[ 2, 1 ] * m2[ 1, 1 ] + m1[ 2, 2 ] * m2[ 2, 1 ] + m1[ 2, 3 ] * m2[ 3, 1 ];
            m3[ 2, 2 ] = m1[ 2, 0 ] * m2[ 0, 2 ] + m1[ 2, 1 ] * m2[ 1, 2 ] + m1[ 2, 2 ] * m2[ 2, 2 ] + m1[ 2, 3 ] * m2[ 3, 2 ];
            m3[ 2, 3 ] = m1[ 2, 0 ] * m2[ 0, 3 ] + m1[ 2, 1 ] * m2[ 1, 3 ] + m1[ 2, 2 ] * m2[ 2, 3 ] + m1[ 2, 3 ] * m2[ 3, 3 ];
            m3[ 3, 0 ] = m1[ 3, 0 ] * m2[ 0, 0 ] + m1[ 3, 1 ] * m2[ 1, 0 ] + m1[ 3, 2 ] * m2[ 2, 0 ] + m1[ 3, 3 ] * m2[ 3, 0 ];
            m3[ 3, 1 ] = m1[ 3, 0 ] * m2[ 0, 1 ] + m1[ 3, 1 ] * m2[ 1, 1 ] + m1[ 3, 2 ] * m2[ 2, 1 ] + m1[ 3, 3 ] * m2[ 3, 1 ];
            m3[ 3, 2 ] = m1[ 3, 0 ] * m2[ 0, 2 ] + m1[ 3, 1 ] * m2[ 1, 2 ] + m1[ 3, 2 ] * m2[ 2, 2 ] + m1[ 3, 3 ] * m2[ 3, 2 ];
            m3[ 3, 3 ] = m1[ 3, 0 ] * m2[ 0, 3 ] + m1[ 3, 1 ] * m2[ 1, 3 ] + m1[ 3, 2 ] * m2[ 2, 3 ] + m1[ 3, 3 ] * m2[ 3, 3 ];
            return new Matrix4( m3 );
        }

        public static Matrix4 operator *( Matrix4 m, Single scalar ) => new Matrix4( Multiply( m, scalar ) );
    }
}