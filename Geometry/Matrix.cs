#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Matrix.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Geometry {
    using System;

    public class Matrix {
        protected readonly int Cols;

        protected readonly Single[,] matrix;

        protected readonly int Rows;

        protected Matrix( Single[,] matrix ) {
            this.matrix = matrix;
            this.Rows = matrix.GetLength( 0 );
            this.Cols = matrix.GetLength( 1 );
        }

        protected Matrix( int rows, int cols ) {
            this.matrix = new Single[rows, cols];
            this.Rows = rows;
            this.Cols = cols;
        }

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
            return String.Format( "({0})", res );
        }

        protected static Single[,] Multiply( Matrix matrix, Single scalar ) {
            var rows = matrix.Rows;
            var cols = matrix.Cols;
            var m1 = matrix.matrix;
            var m2 = new Single[rows, cols];
            for ( var i = 0; i < rows; ++i ) {
                for ( var j = 0; j < cols; ++j ) {
                    m2[ i, j ] = m1[ i, j ]*scalar;
                }
            }
            return m2;
        }

        private static Single[,] Multiply( Matrix matrix1, Matrix matrix2 ) {
            var m1cols = matrix1.Cols;
            if ( m1cols != matrix2.Rows ) {
                throw new ArgumentException();
            }
            var m1rows = matrix1.Rows;
            var m2cols = matrix2.Cols;
            var m1 = matrix1.matrix;
            var m2 = matrix2.matrix;
            var m3 = new Single[m1rows, m2cols];
            for ( var i = 0; i < m1rows; ++i ) {
                for ( var j = 0; j < m2cols; ++j ) {
                    Single sum = 0;
                    for ( var it = 0; it < m1cols; ++it ) {
                        sum += m1[ i, it ]*m2[ it, j ];
                    }
                    m3[ i, j ] = sum;
                }
            }
            return m3;
        }

        public static Matrix operator *( Matrix m, Single scalar ) {
            return new Matrix( Multiply( m, scalar ) );
        }

        public static Matrix operator *( Matrix m1, Matrix m2 ) {
            return new Matrix( Multiply( m1, m2 ) );
        }
    }
}
