// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Matrix.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Matrix.cs" was last formatted by Protiguous on 2019/08/08 at 7:45 AM.

namespace Librainian.Graphics {

    using System;
    using JetBrains.Annotations;

    public class Matrix {

        protected readonly Int32 Cols;

        protected readonly Single[ , ] matrix;

        protected readonly Int32 Rows;

        protected Matrix( [NotNull] Single[ , ] matrix ) {
            this.matrix = matrix;
            this.Rows = matrix.GetLength( 0 );
            this.Cols = matrix.GetLength( 1 );
        }

        protected Matrix( Int32 rows, Int32 cols ) {
            this.matrix = new Single[ rows, cols ];
            this.Rows = rows;
            this.Cols = cols;
        }

        [NotNull]
        private static Single[ , ] Multiply( [NotNull] Matrix matrix1, [NotNull] Matrix matrix2 ) {
            var m1Cols = matrix1.Cols;

            if ( m1Cols != matrix2.Rows ) {
                throw new ArgumentException();
            }

            var m1Rows = matrix1.Rows;
            var m2Cols = matrix2.Cols;
            var m1 = matrix1.matrix;
            var m2 = matrix2.matrix;
            var m3 = new Single[ m1Rows, m2Cols ];

            for ( var i = 0; i < m1Rows; ++i ) {
                for ( var j = 0; j < m2Cols; ++j ) {
                    Single sum = 0;

                    for ( var it = 0; it < m1Cols; ++it ) {
                        sum += m1[ i, it ] * m2[ it, j ];
                    }

                    m3[ i, j ] = sum;
                }
            }

            return m3;
        }

        [NotNull]
        protected static Single[ , ] Multiply( [NotNull] Matrix matrix, Single scalar ) {
            var rows = matrix.Rows;
            var cols = matrix.Cols;
            var m1 = matrix.matrix;
            var m2 = new Single[ rows, cols ];

            for ( var i = 0; i < rows; ++i ) {
                for ( var j = 0; j < cols; ++j ) {
                    m2[ i, j ] = m1[ i, j ] * scalar;
                }
            }

            return m2;
        }

        [NotNull]
        public static Matrix operator *( [NotNull] Matrix m, Single scalar ) => new Matrix( Multiply( m, scalar ) );

        [NotNull]
        public static Matrix operator *( [NotNull] Matrix m1, [NotNull] Matrix m2 ) => new Matrix( Multiply( m1, m2 ) );

        public override String ToString() {
            var res = "";

            for ( var i = 0; i < this.Rows; ++i ) {
                if ( i > 0 ) {
                    res += "|";
                }

                for ( var j = 0; j < this.Cols; ++j ) {
                    if ( j > 0 ) {
                        res += ",";
                    }

                    res += this.matrix[ i, j ];
                }
            }

            return $"({res})";
        }
    }
}