// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "BinaryMatrix.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/BinaryMatrix.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Diagnostics;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    ///     Based from Hamming code found at http://maciejlis.com/hamming-code-algorithm-c-sharp/
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
    public class BinaryMatrix {

        public Int32 ColumnAmount => this.Matrix.GetLength( 1 );

        [NotNull]
        public Boolean[,] Matrix { get; }

        public Int32 RowAmount => this.Matrix.GetLength( 0 );

        public BinaryMatrix( Boolean[,] matrix ) => this.Matrix = matrix;

        public BinaryMatrix( Int32 rowsAmount, Int32 columsAmount ) => this.Matrix = new Boolean[rowsAmount, columsAmount];

        public Boolean Get( Int32 row, Int32 column ) => this.Matrix[row, column];

        public Binary GetColumn( Int32 index ) {
            var column = new Boolean[this.RowAmount];

            for ( var y = 0; y < this.RowAmount; y++ ) { column[y] = this.Matrix[y, index]; }

            return new Binary( column );
        }

        public Binary GetRow( Int32 index ) {
            var row = new Boolean[this.ColumnAmount];

            for ( var x = 0; x < this.ColumnAmount; x++ ) { row[x] = this.Matrix[index, x]; }

            return new Binary( row );
        }

        public void Set( Int32 row, Int32 column, Boolean value ) => this.Matrix[row, column] = value;

        public override String ToString() {
            var stringBuilder = new StringBuilder( this.Matrix.Length );

            for ( var y = 0; y < this.RowAmount; y++ ) {
                for ( var x = 0; x < this.ColumnAmount; x++ ) { stringBuilder.Append( this.Matrix[y, x] ? '1' : '0' ); }

                stringBuilder.Append( '\n' );
            }

            return stringBuilder.ToString();
        }
    }
}