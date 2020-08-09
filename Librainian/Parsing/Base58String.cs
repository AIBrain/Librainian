// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Parsing {

	using System;
	using System.Linq;
	using System.Numerics;
	using System.Text;
	using JetBrains.Annotations;

	public static class Base58String {

		public const String Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

		[NotNull]
		public static String FromByteArray( [NotNull] this Byte[] b ) {
			var sb = new StringBuilder();

			var bi = new BigInteger( b.Reverse().Concat( new Byte[] {
				0x00
			} ).ToArray() ); // concat adds sign byte

			// Calc base58 representation
			while ( bi > 0 ) {
				var mod = ( Int32 )( bi % 58 );
				bi /= 58;
				sb.Insert( 0, Base58Chars[ mod ] );
			}

			// Add 1s for leading 0x00 bytes
			for ( var i = 0; i < b.Length && b[ i ] == 0x00; i++ ) {
				sb.Insert( 0, '1' );
			}

			return sb.ToString();
		}

		[NotNull]
		public static Byte[] ToByteArray( [NotNull] this String s ) {
			BigInteger bi = 0;

			// Decode base58
			foreach ( var charVal in s.Select( c => Base58Chars.IndexOf( c ) ).Where( charVal => charVal != -1 ) ) {
				bi *= 58;
				bi += charVal;
			}

			var b = bi.ToByteArray();

			// Remove 0x00 sign byte if present.
			if ( b[ b.Length - 1 ] == 0x00 ) {
				b = b.Take( b.Length - 1 ).ToArray();
			}

			// Add leading 0x00 bytes
			var num0S = s.IndexOf( s.First( c => c != '1' ) );

			return b.Concat( new Byte[ num0S ] ).Reverse().ToArray();
		}

	}

}