// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Matrix4.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", File: "Matrix4.cs" was last formatted by Protiguous on 2020/03/16 at 4:46 PM.

namespace Librainian.Graphics {

    using System;
    using System.Windows.Media.Media3D;
    using JetBrains.Annotations;

    internal class Matrix4 : Matrix {

        public static Matrix4 I = NewI();

        public Matrix4() : base( rows: 4, cols: 4 ) { }

        public Matrix4( [NotNull] Single[ , ] matrix ) : base( matrix: matrix ) {
            if ( this.Rows != 4 || this.Cols != 4 ) {
                throw new ArgumentException();
            }
        }

        [NotNull]
        public static Matrix4 NewI() =>
            new Matrix4( matrix: new[ , ] {
                {
                    1.0f, 0.0f, 0.0f, 0.0f
                }, {
                    0.0f, 1.0f, 0.0f, 0.0f
                }, {
                    0.0f, 0.0f, 1.0f, 0.0f
                }, {
                    0.0f, 0.0f, 0.0f, 1.0f
                }
            } );

        public static Vector3D operator *( [NotNull] Matrix4 matrix4, Vector3D v ) {
            var m = matrix4.matrix;
            var w = m[ 3, 0 ] * v.X + m[ 3, 1 ] * v.Y + m[ 3, 2 ] * v.Z + m[ 3, 3 ];

            return new Vector3D( x: ( m[ 0, 0 ] * v.X + m[ 0, 1 ] * v.Y + m[ 0, 2 ] * v.Z + m[ 0, 3 ] ) / w,
                y: ( m[ 1, 0 ] * v.X + m[ 1, 1 ] * v.Y + m[ 1, 2 ] * v.Z + m[ 1, 3 ] ) / w, z: ( m[ 2, 0 ] * v.X + m[ 2, 1 ] * v.Y + m[ 2, 2 ] * v.Z + m[ 2, 3 ] ) / w );
        }

        [NotNull]
        public static Matrix4 operator *( [NotNull] Matrix4 mat1, [NotNull] Matrix4 mat2 ) {
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

            return new Matrix4( matrix: m3 );
        }

        [NotNull]
        public static Matrix4 operator *( [NotNull] Matrix4 m, Single scalar ) => new Matrix4( matrix: Multiply( matrix: m, scalar: scalar ) );

    }

}