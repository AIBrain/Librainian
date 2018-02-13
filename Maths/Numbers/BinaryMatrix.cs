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
// "Librainian/BinaryMatrix.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

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

        public BinaryMatrix( Boolean[,] matrix ) => this.Matrix = matrix;

	    public BinaryMatrix( Int32 rowsAmount, Int32 columsAmount ) => this.Matrix = new Boolean[ rowsAmount, columsAmount ];

	    public Int32 ColumnAmount => this.Matrix.GetLength( 1 );

        [NotNull]
        public Boolean[,] Matrix {
            get;
        }

        public Int32 RowAmount => this.Matrix.GetLength( 0 );

        public Boolean Get( Int32 row, Int32 column ) => this.Matrix[ row, column ];

	    public Binary GetColumn( Int32 index ) {
            var column = new Boolean[ this.RowAmount ];
            for ( var y = 0; y < this.RowAmount; y++ ) {
                column[ y ] = this.Matrix[ y, index ];
            }

            return new Binary( column );
        }

        public Binary GetRow( Int32 index ) {
            var row = new Boolean[ this.ColumnAmount ];
            for ( var x = 0; x < this.ColumnAmount; x++ ) {
                row[ x ] = this.Matrix[ index, x ];
            }

            return new Binary( row );
        }

		public void Set( Int32 row, Int32 column, Boolean value ) => this.Matrix[ row, column ] = value;

		public override String ToString() {
            var stringBuilder = new StringBuilder( this.Matrix.Length );
            for ( var y = 0; y < this.RowAmount; y++ ) {
                for ( var x = 0; x < this.ColumnAmount; x++ ) {
                    stringBuilder.Append( this.Matrix[ y, x ] ? '1' : '0' );
                }
                stringBuilder.Append( '\n' );
            }
            return stringBuilder.ToString();
        }
    }
}