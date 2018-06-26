// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Binary.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Binary.cs" was last formatted by Protiguous on 2018/06/04 at 4:05 PM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using JetBrains.Annotations;

	/// <summary>
	///     Based from Hamming code found at http://maciejlis.com/hamming-code-algorithm-c-sharp/
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	public class Binary : IEnumerable<Boolean> {

		public IEnumerator<Boolean> GetEnumerator() => this.Booleans.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.Booleans.GetEnumerator();

		[NotNull]
		public List<Boolean> Booleans { get; }

		public Boolean this[ Int32 index ] {
			get => this.Booleans[ index ];

			set => this.Booleans[ index ] = value;
		}

		public Int32 Length => this.Booleans.Count;

		[NotNull]
		public static Binary Concatenate( [NotNull] Binary a, [NotNull] Binary b ) {
			var result = new Binary( new Boolean[ a.Length + b.Length ] );
			var n = 0;

			foreach ( var bit in a ) {
				result[ n ] = bit;
				n++;
			}

			foreach ( var bit in b ) {
				result[ n ] = bit;
				n++;
			}

			return result;
		}

		[NotNull]
		public static IEnumerable<Boolean> ConvertToBinary( Int32 value ) {
			var binaryString = Convert.ToString( value, 2 );

			return binaryString.Select( c => c == '1' );
		}

		[NotNull]
		public static IEnumerable<Boolean> ConvertToBinary( Int32 value, Int32 minSize ) {
			var toBinary = new List<Boolean>( ConvertToBinary( value ) );

			while ( toBinary.Count != minSize ) { toBinary.Insert( 0, false ); }

			return toBinary;
		}

		[NotNull]
		public static Binary operator &( [NotNull] Binary a, [NotNull] Binary b ) {
			if ( a.Length != b.Length ) { throw new ArgumentException( "Binaries must have same length" ); }

			var result = new Boolean[ a.Length ];

			for ( var i = 0; i < a.Length; i++ ) { result[ i ] = a[ i ] & b[ i ]; }

			return new Binary( result );
		}

		[NotNull]
		public static Binary operator ^( [NotNull] Binary a, [NotNull] Binary b ) {
			if ( a.Length != b.Length ) { throw new ArgumentException( "Binaries must have same length" ); }

			var result = new Boolean[ a.Length ];

			for ( var i = 0; i < a.Length; i++ ) { result[ i ] = a[ i ] ^ b[ i ]; }

			return new Binary( result );
		}

		public Int32 CountOnes() => this.Booleans.Count( bit => bit );

		public Int32 CountZeroes() => this.Booleans.Count( bit => !bit );

		public override String ToString() {
			var stringBuilder = new StringBuilder( this.Length );

			foreach ( var bit in this.Booleans ) { stringBuilder.Append( bit ? '1' : '0' ); }

			return stringBuilder.ToString();
		}

		public Binary( [NotNull] IReadOnlyCollection<Boolean> booleans ) {
			this.Booleans = booleans.ToList();
			this.Booleans.Capacity = this.Booleans.Count;
		}

		public Binary( [NotNull] IEnumerable<Boolean> binary ) : this( ( IReadOnlyCollection<Boolean> ) binary ) { }

		public Binary( Int32 value ) : this( ConvertToBinary( value ) ) { }

		public Binary( Int32 value, Int32 minSize ) : this( ConvertToBinary( value, minSize ) ) { }

	}

}