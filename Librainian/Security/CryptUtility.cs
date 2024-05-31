// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "CryptUtility.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

namespace Librainian.Security;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

/// <summary>Where did this class come from?</summary>
public static class CryptUtility {

	/// <summary>Return one component of a color</summary>
	/// <param name="pixelColor">The Color</param>
	/// <param name="colorComponent">The component to return (0-R, 1-G, 2-B)</param>
	/// <returns>The requested component</returns>
	private static Byte GetColorComponent( Color pixelColor, Int32 colorComponent ) {
		Byte returnValue = colorComponent switch {
			0 => pixelColor.R,
			1 => pixelColor.G,
			2 => pixelColor.B,
			var _ => 0
		};

		return returnValue;
	}

	//--------------------------------------------- combining the keys
	/// <summary>Combines all key files and passwords into one key stream</summary>
	/// <param name="keys">The keys to combine</param>
	/// <returns>The resulting key stream</returns>
	private static MemoryStream GetKeyStream( IReadOnlyList<FilePasswordPair> keys ) {

		//Xor the keys an their passwords
		var keyStreams = new MemoryStream[ keys.Count ];

		for ( var n = 0; n < keys.Count; n++ ) {
			keyStreams[ n ] = CreateKeyStream( keys[ n ] );
		}

		//Buffer for the resulting stream
		var resultKeyStream = new MemoryStream();

		//Find length of longest stream
		var maxLength = keyStreams.Select( stream => stream.Length )
								  .Concat( new Int64[] {
									  0
								  } )
								  .Max();

		for ( Int64 n = 0; n <= maxLength; n++ ) {
			foreach ( var stream in keyStreams ) {
				var readByte = stream.ReadByte();

				if ( readByte < 0 ) {

					//end of stream - close the file
					//the last loop (n==maxLength) will close the last stream
					using ( stream ) {
						stream.Close();
					}
				}
				else {

					//copy a byte into the result key
					resultKeyStream.WriteByte( ( Byte )readByte );
				}
			}
		}

		return resultKeyStream;
	}

	private static Byte GetReverseKeyByte( Stream keyStream ) {

		//jump to reverse-read position and read from the end of the stream
		var keyPosition = keyStream.Position;
		keyStream.Seek( -keyPosition, SeekOrigin.End );
		var reverseKeyByte = ( Byte )keyStream.ReadByte();

		//jump back to normal read position
		keyStream.Seek( keyPosition, SeekOrigin.Begin );

		return reverseKeyByte;
	}

	/// <summary>Combines key file and password using XOR</summary>
	/// <param name="key">The key/password pair to combine</param>
	/// <returns>The stream created from key and password</returns>
	public static MemoryStream CreateKeyStream( FilePasswordPair key ) {
		var fileStream = new FileStream( key.FileName, FileMode.Open );
		var resultStream = new MemoryStream();
		var passwordIndex = 0;
		Int32 currentByte;

		while ( ( currentByte = fileStream.ReadByte() ) >= 0 ) {

			//combine the key-byte with the corresponding password-byte
			currentByte ^= key.Password[ passwordIndex ];

			//add the result to the key stream
			resultStream.WriteByte( ( Byte )currentByte );

			//proceed to the next letter or repeat the password
			passwordIndex++;

			if ( passwordIndex == key.Password.Length ) {
				passwordIndex = 0;
			}
		}

		fileStream.Close();

		resultStream.Seek( 0, SeekOrigin.Begin );

		return resultStream;
	}

	/// <summary>Changees one component of a color</summary>
	/// <param name="pixelColor">The Color</param>
	/// <param name="colorComponent">The component to change (0-R, 1-G, 2-B)</param>
	/// <param name="newValue">New value of the component</param>
	public static void SetColorComponent( ref Color pixelColor, Int32 colorComponent, Int32 newValue ) {
		pixelColor = colorComponent switch {
			0 => Color.FromArgb( newValue, pixelColor.G, pixelColor.B ),
			1 => Color.FromArgb( pixelColor.R, newValue, pixelColor.B ),
			2 => Color.FromArgb( pixelColor.R, pixelColor.G, newValue ),
			var _ => pixelColor
		};
	}

	public static String UnTrimColorString( String color, Int32 desiredLength ) {
		var difference = desiredLength - color.Length;

		if ( difference > 0 ) {
			color = new String( '0', difference ) + color;
		}

		return color;
	}
}