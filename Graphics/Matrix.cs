// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Matrix.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics {

    using System;

    public class Matrix {
        protected readonly Int32 Cols;
        protected readonly Single[,] matrix;
        protected readonly Int32 Rows;

        protected Matrix( Single[,] matrix ) {
            this.matrix = matrix;
            this.Rows = matrix.GetLength( 0 );
            this.Cols = matrix.GetLength( 1 );
        }

        protected Matrix( Int32 rows, Int32 cols ) {
            this.matrix = new Single[ rows, cols ];
            this.Rows = rows;
            this.Cols = cols;
        }

        public static Matrix operator *( Matrix m, Single scalar ) => new Matrix( Multiply( m, scalar ) );

        public static Matrix operator *( Matrix m1, Matrix m2 ) => new Matrix( Multiply( m1, m2 ) );

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

        protected static Single[,] Multiply( Matrix matrix, Single scalar ) {
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

        private static Single[,] Multiply( Matrix matrix1, Matrix matrix2 ) {
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
    }
}