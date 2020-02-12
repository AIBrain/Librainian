// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CryptUtility.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "CryptUtility.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace LibrainianCore.Security {

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using JetBrains.Annotations;

    public static class CryptUtility {

        //TODO use the real temp filename
        private const String TempFileName = "picturekey_temp.bmp";

        /// <summary>Get the value of a bit</summary>
        /// <param name="b">The byte value</param>
        /// <param name="position">The position of the bit</param>
        /// <returns>The value of the bit</returns>
        private static Boolean GetBit( Byte b, Byte position ) => ( b & ( Byte )( 1 << position ) ) != 0;

        /// <summary>Return one component of a color</summary>
        /// <param name="pixelColor">The Color</param>
        /// <param name="colorComponent">The component to return (0-R, 1-G, 2-B)</param>
        /// <returns>The requested component</returns>
        private static Byte GetColorComponent( Color pixelColor, Int32 colorComponent ) {
            Byte returnValue = 0;

            switch ( colorComponent ) {
                case 0:
                    returnValue = pixelColor.R;

                    break;

                case 1:
                    returnValue = pixelColor.G;

                    break;

                case 2:
                    returnValue = pixelColor.B;

                    break;
            }

            return returnValue;
        }

        //--------------------------------------------- combining the keys
        /// <summary>Combines all key files and passwords into one key stream</summary>
        /// <param name="keys">The keys to combine</param>
        /// <returns>The resulting key stream</returns>
        [NotNull]
        private static MemoryStream GetKeyStream( [NotNull] IReadOnlyList<FilePasswordPair> keys ) {

            //Xor the keys an their passwords
            var keyStreams = new MemoryStream[ keys.Count ];

            for ( var n = 0; n < keys.Count; n++ ) {
                keyStreams[ n ] = CreateKeyStream( keys[ index: n ] );
            }

            //Buffer for the resulting stream
            var resultKeyStream = new MemoryStream();

            //Find length of longest stream
            var maxLength = keyStreams.Select( selector: stream => stream.Length ).Concat( second: new Int64[] {
                0
            } ).Max();

            for ( Int64 n = 0; n <= maxLength; n++ ) {
                for ( var streamIndex = 0; streamIndex < keyStreams.Length; streamIndex++ ) {
                    if ( keyStreams[ streamIndex ] is null ) {
                        continue;
                    }

                    var readByte = keyStreams[ streamIndex ].ReadByte();

                    if ( readByte < 0 ) {

                        //end of stream - close the file
                        //the last loop (n==maxLength) will close the last stream
                        keyStreams[ streamIndex ].Close();
                        keyStreams[ streamIndex ] = null;
                    }
                    else {

                        //copy a byte into the result key
                        resultKeyStream.WriteByte( ( Byte )readByte );
                    }
                }
            }

            return resultKeyStream;
        }

        private static Byte GetReverseKeyByte( [NotNull] Stream keyStream ) {

            //jump to reverse-read position and read from the end of the stream
            var keyPosition = keyStream.Position;
            keyStream.Seek( offset: -keyPosition, origin: SeekOrigin.End );
            var reverseKeyByte = ( Byte )keyStream.ReadByte();

            //jump back to normal read position
            keyStream.Seek( offset: keyPosition, origin: SeekOrigin.Begin );

            return reverseKeyByte;
        }

        /// <summary>Set a bit to [newBitValue]</summary>
        /// <param name="b">The byte value</param>
        /// <param name="position">The position (1-8) of the bit</param>
        /// <param name="newBitValue">The new value of the bit in position [position]</param>
        /// <returns>The new byte value</returns>
        private static Byte SetBit( Byte b, Byte position, Boolean newBitValue ) {
            var mask = ( Byte )( 1 << position );

            if ( newBitValue ) {
                return ( Byte )( b | mask );
            }

            return ( Byte )( b & ~mask );
        }

        /// <summary>Changees one component of a color</summary>
        /// <param name="pixelColor">The Color</param>
        /// <param name="colorComponent">The component to change (0-R, 1-G, 2-B)</param>
        /// <param name="newValue">New value of the component</param>
        private static void SetColorComponent( ref Color pixelColor, Int32 colorComponent, Int32 newValue ) {
            switch ( colorComponent ) {
                case 0:
                    pixelColor = Color.FromArgb( red: newValue, green: pixelColor.G, blue: pixelColor.B );

                    break;

                case 1:
                    pixelColor = Color.FromArgb( red: pixelColor.R, green: newValue, blue: pixelColor.B );

                    break;

                case 2:
                    pixelColor = Color.FromArgb( red: pixelColor.R, green: pixelColor.G, blue: newValue );

                    break;
            }
        }

        [NotNull]
        private static String UnTrimColorString( String color, Int32 desiredLength ) {
            var difference = desiredLength - color.Length;

            if ( difference > 0 ) {
                color = new String( c: '0', count: difference ) + color;
            }

            return color;
        }

        /// <summary>Combines key file and password using XOR</summary>
        /// <param name="key">The key/password pair to combine</param>
        /// <returns>The stream created from key and password</returns>
        [NotNull]
        public static MemoryStream CreateKeyStream( FilePasswordPair key ) {
            var fileStream = new FileStream( key.FileName, mode: FileMode.Open );
            var resultStream = new MemoryStream();
            var passwordIndex = 0;
            Int32 currentByte;

            while ( ( currentByte = fileStream.ReadByte() ) >= 0 ) {

                //combine the key-byte with the corresponding password-byte
                currentByte ^= key.Password[ index: passwordIndex ];

                //add the result to the key stream
                resultStream.WriteByte( ( Byte )currentByte );

                //proceed to the next letter or repeat the password
                passwordIndex++;

                if ( passwordIndex == key.Password.Length ) {
                    passwordIndex = 0;
                }
            }

            fileStream.Close();

            resultStream.Seek( offset: 0, loc: SeekOrigin.Begin );

            return resultStream;
        }

    }
}