// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Matrix4.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "Matrix4.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace LibrainianCore.Graphics {

    using System;
    using JetBrains.Annotations;

    internal class Matrix4 : Matrix {

        public static Matrix4 I = NewI();

        public Matrix4() : base( 4, 4 ) { }

        public Matrix4( [NotNull] Single[,] matrix ) : base( matrix ) {
            if ( this.Rows != 4 || this.Cols != 4 ) {
                throw new ArgumentException();
            }
        }

        [NotNull]
        public static Matrix4 NewI() =>
            new Matrix4( new[ , ] {
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

            return new Matrix4( m3 );
        }

        [NotNull]
        public static Matrix4 operator *( [NotNull] Matrix4 m, Single scalar ) => new Matrix4( Multiply( m, scalar ) );
    }
}