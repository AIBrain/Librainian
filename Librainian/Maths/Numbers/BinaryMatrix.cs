// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "BinaryMatrix.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.Diagnostics;
using System.Text;

/// <summary>Based from Hamming code found at http://maciejlis.com/hamming-code-algorithm-c-sharp/</summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
public class BinaryMatrix {

	public BinaryMatrix( Boolean[,] matrix ) => this.Matrix = matrix;

	public BinaryMatrix( Int32 rowsAmount, Int32 columsAmount ) => this.Matrix = new Boolean[ rowsAmount, columsAmount ];

	public Int32 ColumnAmount => this.Matrix.GetLength( 1 );

	public Boolean[,] Matrix { get; }

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