// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "SteganographyHelper.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "SteganographyHelper.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace Librainian.Security {

    using System;
    using System.Drawing;
    using JetBrains.Annotations;

    public static class SteganographyHelper {

        public enum State {

            Hiding,

            FillingWithZeros

        }

        [NotNull]
        public static Bitmap EmbedText( [NotNull] this Bitmap bmp, [CanBeNull] String text ) {

            // initially, we'll be hiding characters in the image
            var state = State.Hiding;

            // holds the index of the character that is being hidden
            var charIndex = 0;

            // holds the value of the character converted to integer
            var charValue = 0;

            // holds the index of the color element (R or G or B) that is currently being processed
            Int64 pixelElementIndex = 0;

            // holds the number of trailing zeros that have been added when finishing the process
            var zeros = 0;

            // hold pixel elements

            // pass through the rows
            for ( var i = 0; i < bmp.Height; i++ ) {

                // pass through each row
                for ( var j = 0; j < bmp.Width; j++ ) {

                    // holds the pixel that is currently being processed
                    var pixel = bmp.GetPixel( x: j, y: i );

                    // now, clear the least significant bit (LSB) from each pixel element
                    var r = pixel.R - pixel.R % 2;
                    var g = pixel.G - pixel.G % 2;
                    var b = pixel.B - pixel.B % 2;

                    // for each pixel, pass through its elements (RGB)
                    for ( var n = 0; n < 3; n++ ) {

                        // check if new 8 bits has been processed
                        if ( pixelElementIndex % 8 == 0 ) {

                            // check if the whole process has finished
                            // we can say that it's finished when 8 zeros are added
                            if ( state == State.FillingWithZeros && zeros == 8 ) {

                                // apply the last pixel on the image
                                // even if only a part of its elements have been affected
                                if ( ( pixelElementIndex - 1 ) % 3 < 2 ) {
                                    bmp.SetPixel( x: j, y: i, color: Color.FromArgb( red: r, green: g, blue: b ) );
                                }

                                // return the bitmap with the text hidden in
                                return bmp;
                            }

                            // check if all characters has been hidden
                            if ( charIndex >= text.Length ) {

                                // start adding zeros to mark the end of the text
                                state = State.FillingWithZeros;
                            }
                            else {

                                // move to the next character and process again
                                charValue = text[ index: charIndex++ ];
                            }
                        }

                        // check which pixel element has the turn to hide a bit in its LSB
                        switch ( pixelElementIndex % 3 ) {
                            case 0: {
                                if ( state == State.Hiding ) {

                                    // the rightmost bit in the character will be (charValue % 2)
                                    // to put this value instead of the LSB of the pixel element
                                    // just add it to it
                                    // recall that the LSB of the pixel element had been cleared
                                    // before this operation
                                    r += charValue % 2;

                                    // removes the added rightmost bit of the character
                                    // such that next time we can reach the next one
                                    charValue /= 2;
                                }
                            }

                                break;

                            case 1: {
                                if ( state == State.Hiding ) {
                                    g += charValue % 2;

                                    charValue /= 2;
                                }
                            }

                                break;

                            case 2: {
                                if ( state == State.Hiding ) {
                                    b += charValue % 2;

                                    charValue /= 2;
                                }

                                bmp.SetPixel( x: j, y: i, color: Color.FromArgb( red: r, green: g, blue: b ) );
                            }

                                break;
                        }

                        pixelElementIndex++;

                        if ( state == State.FillingWithZeros ) {

                            // increment the value of zeros until it is 8
                            zeros++;
                        }
                    }
                }
            }

            return bmp;
        }

        [NotNull]
        public static String ExtractText( [NotNull] this Bitmap bmp ) {
            var colorUnitIndex = 0;
            var charValue = 0;

            // holds the text that will be extracted from the image
            var extractedText = String.Empty;

            // pass through the rows
            for ( var i = 0; i < bmp.Height; i++ ) {

                // pass through each row
                for ( var j = 0; j < bmp.Width; j++ ) {
                    var pixel = bmp.GetPixel( x: j, y: i );

                    // for each pixel, pass through its elements (RGB)
                    for ( var n = 0; n < 3; n++ ) {
                        switch ( colorUnitIndex % 3 ) {
                            case 0: {

                                // get the LSB from the pixel element (will be pixel.R % 2)
                                // then add one bit to the right of the current character
                                // this can be done by (charValue = charValue * 2)
                                // replace the added bit (which value is by default 0) with
                                // the LSB of the pixel element, simply by addition
                                charValue = charValue * 2 + pixel.R % 2;
                            }

                                break;

                            case 1: {
                                charValue = charValue * 2 + pixel.G % 2;
                            }

                                break;

                            case 2: {
                                charValue = charValue * 2 + pixel.B % 2;
                            }

                                break;
                        }

                        colorUnitIndex++;

                        // if 8 bits has been added, then add the current character to the result text
                        if ( colorUnitIndex % 8 == 0 ) {

                            // reverse? of course, since each time the process happens on the right (for simplicity)
                            charValue = ReverseBits( n: charValue );

                            // can only be 0 if it is the stop character (the 8 zeros)
                            if ( charValue == 0 ) {
                                return extractedText;
                            }

                            // convert the character value from int to char
                            var c = ( Char ) charValue;

                            // add the current character to the result text
                            extractedText += c.ToString();
                        }
                    }
                }
            }

            return extractedText;
        }

        public static Int32 ReverseBits( this Int32 n ) {
            var result = 0;

            for ( var i = 0; i < 8; i++ ) {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }

    }

}