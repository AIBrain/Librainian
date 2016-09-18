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
// "Librainian/CryptUtility.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Graphics.Video;

    public class CryptUtility {

        //TODO use the real temp filename
        private const String TempFileName = "picturekey_temp.bmp";

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
                currentByte = currentByte ^ key.Password[ passwordIndex ];

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

        /// <summary>Extracts an hidden message from a bitmap</summary>
        /// <param name="keys"></param>
        /// <param name="messageStream">Empty stream to receive the message</param>
        /// <param name="imageFiles"></param>
        /// <param name="splitBytes"></param>
        public static void ExtractMessageFromBitmap( CarrierImage[] imageFiles, FilePasswordPair[] keys, ref Stream messageStream, Boolean splitBytes ) {
            HideOrExtract( ref messageStream, imageFiles, keys, splitBytes, true );
        }

        /// <summary>Hides a message in a bitmap</summary>
        /// <param name="messageStream">The message to hide</param>
        /// <param name="imageFiles"></param>
        /// <param name="keys"></param>
        /// <param name="splitBytes"></param>
        public static void HideMessageInBitmap( Stream messageStream, CarrierImage[] imageFiles, FilePasswordPair[] keys, Boolean splitBytes ) {
            HideOrExtract( ref messageStream, imageFiles, keys, splitBytes, false );

            // ReSharper disable once RedundantAssignment
            messageStream = null; //BUG why is this here?
        }

        /// <summary>Get the value of a bit</summary>
        /// <param name="b">The byte value</param>
        /// <param name="position">The position of the bit</param>
        /// <returns>The value of the bit</returns>
        private static Boolean GetBit( Byte b, Byte position ) {
            return ( b & ( Byte )( 1 << position ) ) != 0;
        }

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
        private static MemoryStream GetKeyStream( IReadOnlyList< FilePasswordPair > keys ) {

            //Xor the keys an their passwords
            var keyStreams = new MemoryStream[ keys.Count ];
            for ( var n = 0; n < keys.Count; n++ ) {
                keyStreams[ n ] = CreateKeyStream( keys[ n ] );
            }

            //Buffer for the resulting stream
            var resultKeyStream = new MemoryStream();

            //Find length of longest stream
            var maxLength = keyStreams.Select( stream => stream.Length ).Concat( new Int64[] { 0 } ).Max();

            for ( Int64 n = 0; n <= maxLength; n++ ) {
                for ( var streamIndex = 0; streamIndex < keyStreams.Length; streamIndex++ ) {
                    if ( keyStreams[ streamIndex ] == null ) {
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

        private static Byte GetReverseKeyByte( Stream keyStream ) {

            //jump to reverse-read position and read from the end of the stream
            var keyPosition = keyStream.Position;
            keyStream.Seek( -keyPosition, SeekOrigin.End );
            var reverseKeyByte = ( Byte )keyStream.ReadByte();

            //jump back to normal read position
            keyStream.Seek( keyPosition, SeekOrigin.Begin );
            return reverseKeyByte;
        }

        private static void HideBits( Stream keyStream, Stream messageStream, Int64 messageLength, AviReader aviReader, AviWriter aviWriter, CarrierImage[] imageFiles, BitmapInfo bitmapInfo, Boolean extract ) {

            //Color component to hide the next byte in (0-R, 1-G, 2-B)
            //Rotates with every hidden byte
            var currentColorComponent = 0;

            //Index of the current bitmap
            var indexBitmaps = 0;

            //Maximum X and Y position in the current bitmap
            var bitmapWidth = bitmapInfo.Bitmap.Width - 1;

            //Current position in the carrier bitmap
            //Start with 1, because (0,0) contains the message length
            var pixelPosition = new Point( 1, 0 );

            //Count of bytes already hidden in the current image
            var countBytesInCurrentImage = 0;

            //Stores the color of a pixel

            //A value read from the key stream in reverse direction

            //The current byte of the message stream

            for ( var messageIndex = 0; messageIndex < messageLength; messageIndex++ ) {
                var currentReverseKeyByte = GetReverseKeyByte( keyStream );

                Byte currentByte;
                if ( extract ) {
                    currentByte = 0;
                }
                else {
                    currentByte = ( Byte )messageStream.ReadByte();

                    //To add a bit of confusion, xor the byte with a byte read from the keyStream
                    currentByte = ( Byte )( currentByte ^ currentReverseKeyByte );
                }

                for ( Byte bitPosition = 0; bitPosition < 8; bitPosition++ ) {
                    MovePixelPosition( extract, aviReader, aviWriter, imageFiles, keyStream, ref countBytesInCurrentImage, ref indexBitmaps, ref pixelPosition, ref bitmapWidth, ref bitmapInfo );

                    //Get color of the "clean" pixel
                    var pixelColor = bitmapInfo.Bitmap.GetPixel( pixelPosition.X, pixelPosition.Y );

                    if ( extract ) {

                        //Extract the hidden message-byte from the color
                        var foundByte = GetColorComponent( pixelColor, currentColorComponent );
                        var foundBit = GetBit( foundByte, 0 );
                        currentByte = SetBit( currentByte, bitPosition, foundBit );

                        //Rotate color components
                        currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                    }
                    else {
                        var currentBit = GetBit( currentByte, bitPosition );

                        if ( imageFiles[ indexBitmaps ].UseGrayscale ) {
                            var r = SetBit( pixelColor.R, 0, currentBit );
                            var g = SetBit( pixelColor.G, 0, currentBit );
                            var b = SetBit( pixelColor.B, 0, currentBit );
                            pixelColor = Color.FromArgb( r, g, b );
                        }
                        else {

                            //Change one component of the color to the message-byte
                            var colorComponentValue = GetColorComponent( pixelColor, currentColorComponent );
                            colorComponentValue = SetBit( colorComponentValue, 0, currentBit );
                            SetColorComponent( ref pixelColor, currentColorComponent, colorComponentValue );

                            //Rotate color components
                            currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                        }
                        bitmapInfo.Bitmap.SetPixel( pixelPosition.X, pixelPosition.Y, pixelColor );
                    }
                }

                if ( extract ) {
                    currentByte = ( Byte )( currentByte ^ currentReverseKeyByte );
                    messageStream.WriteByte( currentByte );
                }

                countBytesInCurrentImage++;
            }

            //Save last image
            if ( !extract ) {
                if ( bitmapInfo.AviPosition < 0 ) {

                    //Save bitmap
                    SaveBitmap( bitmapInfo.Bitmap, imageFiles[ indexBitmaps ].ResultFileName );
                }
                else {

                    //Write frame
                    aviWriter.AddFrame( bitmapInfo.Bitmap );
                }
            }
            bitmapInfo.Bitmap.Dispose();
            /*if(bitmapInfo.aviPosition >= 0){

				//the bitmap is a temporary extrated AVI frame
				File.Delete(bitmapInfo.sourceFileName);
			}*/
        }

        /// <summary>Loop over the message and hide each byte in one pixel</summary>
        /// <param name="keyStream">The key</param>
        /// <param name="messageStream">
        ///     A stream containing the message (extract==false) or an empty stream (extract==true)
        /// </param>
        /// <param name="messageLength">Expected length of the message</param>
        /// <param name="aviWriter"></param>
        /// <param name="imageFiles">CarrierImages describing the bitmaps</param>
        /// <param name="bitmapInfo"></param>
        /// <param name="extract">Hide the message (false) or extract it (true)</param>
        /// <param name="aviReader"></param>
        private static void HideBytes( Stream keyStream, Stream messageStream, Int64 messageLength, AviReader aviReader, AviWriter aviWriter, CarrierImage[] imageFiles, BitmapInfo bitmapInfo, Boolean extract ) {

            //Color component to hide the next byte in (0-R, 1-G, 2-B)
            //Rotates with every hidden byte
            var currentColorComponent = 0;

            //Index of the current bitmap
            var indexBitmaps = 0;

            //Maximum X and Y position in the current bitmap
            var bitmapWidth = bitmapInfo.Bitmap.Width - 1;

            //int bitmapHeight = bitmaps[0].Height-1;

            //Current position in the carrier bitmap
            //Start with 1, because (0,0) contains the message length
            var pixelPosition = new Point( 1, 0 );

            //Count of bytes already hidden in the current image
            var countBytesInCurrentImage = 0;

            //Stores the color of a pixel

            //A value read from the key stream in reverse direction

            for ( var messageIndex = 0; messageIndex < messageLength; messageIndex++ ) {
                MovePixelPosition( extract, aviReader, aviWriter, imageFiles, keyStream, ref countBytesInCurrentImage, ref indexBitmaps, ref pixelPosition, ref bitmapWidth, ref bitmapInfo );
                var currentReverseKeyByte = GetReverseKeyByte( keyStream );
                countBytesInCurrentImage++;

                //Get color of the "clean" pixel
                var pixelColor = bitmapInfo.Bitmap.GetPixel( pixelPosition.X, pixelPosition.Y );

                if ( extract ) {

                    //Extract the hidden message-byte from the color
                    var foundByte = ( Byte )( currentReverseKeyByte ^ GetColorComponent( pixelColor, currentColorComponent ) );
                    messageStream.WriteByte( foundByte );

                    //Rotate color components
                    currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                }
                else {

                    //To add a bit of confusion, xor the byte with a byte read from the keyStream
                    var currentByte = messageStream.ReadByte() ^ currentReverseKeyByte;

                    if ( imageFiles[ indexBitmaps ].UseGrayscale ) {
                        pixelColor = Color.FromArgb( currentByte, currentByte, currentByte );
                    }
                    else {

                        //Change one component of the color to the message-byte
                        SetColorComponent( ref pixelColor, currentColorComponent, currentByte );

                        //Rotate color components
                        currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                    }
                    bitmapInfo.Bitmap.SetPixel( pixelPosition.X, pixelPosition.Y, pixelColor );
                }
            }

            //Save last image
            if ( !extract ) {
                if ( bitmapInfo.AviPosition < 0 ) {

                    //Save bitmap
                    SaveBitmap( bitmapInfo.Bitmap, imageFiles[ indexBitmaps ].ResultFileName );
                }
                else {

                    //Write frame
                    aviWriter.AddFrame( bitmapInfo.Bitmap );
                }
            }
            bitmapInfo.Bitmap.Dispose();
        }

        /// <summary>
        ///     Steps through the pixels of bitmaps using a key pattern and hides or extracts a message
        /// </summary>
        /// <param name="messageStream">
        ///     If exctract is false, the message to hide - otherwise an empty stream to receive the
        ///     extracted message
        /// </param>
        /// <param name="splitBytes"></param>
        /// <param name="extract">
        ///     Extract a hidden message (true), or hide a message in a clean carrier bitmap (false)
        /// </param>
        /// <param name="imageFiles"></param>
        /// <param name="keys"></param>
        private static void HideOrExtract( ref Stream messageStream, CarrierImage[] imageFiles, IReadOnlyList< FilePasswordPair > keys, Boolean splitBytes, Boolean extract ) {
            var aviWriter = new AviWriter();
            var aviReader = new AviReader();

            //index for imageFiles
            Int32 indexBitmaps;

            //count available pixels
            Int64 countPixels = 0;
            for ( indexBitmaps = 0; indexBitmaps < imageFiles.Length; indexBitmaps++ ) {
                countPixels += imageFiles[ indexBitmaps ].CountPixels;
            }

            //load the first bitmap
            var bitmapInfo = LoadBitmap( imageFiles[ 0 ], aviReader, aviWriter );

            //Stores the color of a pixel
            Color pixelColor;

            //Length of the message
            Int32 messageLength;

            //combine all keys
            Stream keyStream = GetKeyStream( keys );

            if ( extract ) {

                //Read the length of the hidden message from the first pixel
                pixelColor = bitmapInfo.Bitmap.GetPixel( 0, 0 );
                messageLength = ( pixelColor.R << 16 ) + ( pixelColor.G << 8 ) + pixelColor.B;
                messageStream = new MemoryStream( messageLength );
            }
            else {
                messageLength = ( Int32 )messageStream.Length;

                if ( messageStream.Length >= 16777215 ) {

                    //The message is too long
                    var exceptionMessage = "The message is too long, only 16777215 bytes are allowed.";
                    throw new Exception( exceptionMessage );
                }
            }

            //calculate count of message-bytes to hide in (or extract from) each image
            Int64 sumBytes = 0;
            for ( var n = 0; n < imageFiles.Length; n++ ) {
                var pixels = imageFiles[ n ].CountPixels / ( Single )countPixels;
                imageFiles[ n ].SetCountBytesToHide( ( Int64 )Math.Ceiling( messageLength * pixels ) );
                sumBytes += imageFiles[ n ].MessageBytesToHide;
            }
            if ( sumBytes > messageLength ) {

                //correct Math.Ceiling effects
                imageFiles[ imageFiles.Length - 1 ].MessageBytesToHide -= sumBytes - messageLength;
            }

            //set count of bytes for the first image
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if ( bitmapInfo.AviPosition >= 0 ) {

                //video
                bitmapInfo.MessageBytesToHide = imageFiles[ 0 ].AviMessageBytesToHide[ 0 ];
            }
            else {

                //bitmap
                bitmapInfo.MessageBytesToHide = imageFiles[ 0 ].MessageBytesToHide;
            }

            if ( !extract ) {

                //Check size of the carrier image

                var errorMessage = String.Empty;
                for ( var n = 0; n < imageFiles.Length; n++ ) {

                    //One pixel of the first image is used for the message's length
                    Int64 countRequiredPixels = n == 0 ? 1 : 0;

                    //Count pixels
                    Int64 countRequiredPixelsImage;
                    if ( splitBytes ) {

                        //use 8 pixels for a message byte
                        countRequiredPixelsImage = imageFiles[ n ].MessageBytesToHide * 8;
                    }
                    else {

                        //use one pixel for a message byte
                        countRequiredPixelsImage = imageFiles[ n ].MessageBytesToHide;
                    }
                    for ( var countBytes = 0; countBytes < countRequiredPixelsImage; countBytes++ ) {
                        var readByte = keyStream.ReadByte();
                        if ( readByte < 0 ) {
                            keyStream.Seek( 0, SeekOrigin.Begin );
                            readByte = keyStream.ReadByte();
                        }
                        countRequiredPixels += readByte;
                    }

                    if ( countRequiredPixels > imageFiles[ n ].CountPixels ) {
                        errorMessage += $"The images {imageFiles[ n ].SourceFileName} is too small for this message and key. {countRequiredPixels} pixels are required.\n";
                    }
                }

                if ( errorMessage.Length > 0 ) {

                    //One or more images are too small
                    throw new Exception( errorMessage );
                }

                //Write length of the bitmap into the first pixel
                var colorValue = messageLength;
                var red = colorValue >> 16;
                colorValue -= red << 16;
                var green = colorValue >> 8;
                var blue = colorValue - ( green << 8 );
                pixelColor = Color.FromArgb( red, green, blue );
                bitmapInfo.Bitmap.SetPixel( 0, 0, pixelColor );
            }

            //Reset the streams
            keyStream.Seek( 0, SeekOrigin.Begin );
            messageStream.Seek( 0, SeekOrigin.Begin );

            //Loop over the message and hide each byte
            if ( splitBytes ) {
                HideBits( keyStream, messageStream, messageLength, aviReader, aviWriter, imageFiles, bitmapInfo, extract );
            }
            else {
                HideBytes( keyStream, messageStream, messageLength, aviReader, aviWriter, imageFiles, bitmapInfo, extract );
            }

            //Close AVI files
            aviWriter.Close();
            aviReader.Close();

            //Delete temporary file
            var fileName = Application.ExecutablePath;
            var index = fileName.LastIndexOf( "\\", StringComparison.Ordinal ) + 1;
            fileName = fileName.Substring( 0, index ) + TempFileName;
            if ( File.Exists( fileName ) ) {
                File.Delete( fileName );
            }

            keyStream.Close();
        }

        private static BitmapInfo LoadBitmap( CarrierImage imageFile, AviReader aviReader, AviWriter aviWriter ) {
            var bitmapInfo = new BitmapInfo();

            if ( imageFile.SourceFileName.ToLower().EndsWith( ".avi" ) ) {
                try {

                    //first carrier image is a video - extract the first frame
                    aviReader.Open( imageFile.SourceFileName );
                    if ( imageFile.ResultFileName.Length > 0 ) {
                        aviWriter.Open( imageFile.ResultFileName, aviReader.FrameRate );
                    }

                    var fileName = Application.ExecutablePath;
                    var index = fileName.LastIndexOf( "\\", StringComparison.Ordinal ) + 1;
                    fileName = fileName.Substring( 0, index ) + TempFileName;

                    aviReader.ExportBitmap( 0, fileName );
                    bitmapInfo.LoadBitmap( fileName );
                    bitmapInfo.AviPosition = 0;
                    bitmapInfo.AviCountFrames = aviReader.CountFrames;
                    if ( imageFile.AviMessageBytesToHide != null ) {
                        bitmapInfo.MessageBytesToHide = imageFile.AviMessageBytesToHide[ 0 ];
                    }
                }
                catch ( Exception ) {
                    aviReader.Close();
                    aviWriter.Close();
                    throw;
                }
            }
            else {

                //first carrier file is a bitmap
                bitmapInfo.LoadBitmap( imageFile.SourceFileName );
                bitmapInfo.MessageBytesToHide = imageFile.MessageBytesToHide;
                bitmapInfo.AviPosition = -1;
                bitmapInfo.AviCountFrames = 0;
            }
            return bitmapInfo;
        }

        private static void MovePixelPosition( Boolean extract, AviReader aviReader, AviWriter aviWriter, CarrierImage[] imageFiles, Stream keyStream, ref Int32 countBytesInCurrentImage, ref Int32 indexBitmaps, ref Point pixelPosition, ref Int32 bitmapWidth, ref BitmapInfo bitmapInfo ) {

            //Repeat the key, if it is shorter than the message
            if ( keyStream.Position == keyStream.Length ) {
                keyStream.Seek( 0, SeekOrigin.Begin );
            }

            //Get the next pixel-count from the key, use "1" if it's 0
            var currentKeyByte = ( Byte )keyStream.ReadByte();
            var currentStepWidth = currentKeyByte == 0 ? 1 : currentKeyByte;

            //Perform line breaks, if current step is wider than the image
            while ( currentStepWidth > bitmapWidth ) {
                currentStepWidth -= bitmapWidth;
                pixelPosition.Y++;
            }

            //Move X-position
            if ( bitmapWidth - pixelPosition.X < currentStepWidth ) {
                pixelPosition.X = currentStepWidth - ( bitmapWidth - pixelPosition.X );
                pixelPosition.Y++;
            }
            else {
                pixelPosition.X += currentStepWidth;
            }

            //Proceed to next bitmap
            if ( countBytesInCurrentImage == bitmapInfo.MessageBytesToHide ) {

                //Reset indices
                pixelPosition.Y = 0;
                countBytesInCurrentImage = 0;

                if ( !extract ) {
                    if ( bitmapInfo.AviPosition < 0 ) {

                        //Save bitmap
                        SaveBitmap( bitmapInfo.Bitmap, imageFiles[ indexBitmaps ].ResultFileName );
                    }
                    else {

                        //Write frame
                        aviWriter.AddFrame( bitmapInfo.Bitmap );
                    }
                }

                //Load next bitmap

                bitmapInfo.Bitmap.Dispose();

                var nextFile = true;
                if ( bitmapInfo.AviPosition >= 0 ) {
                    if ( bitmapInfo.AviPosition == bitmapInfo.AviCountFrames - 1 ) {

                        //Last frame - close AVI file
                        aviWriter.Close();

                        //Delete temporary file
                        //bitmapInfo.Bitmap.Dispose();      //BUG memory leak?
                        File.Delete( bitmapInfo.SourceFileName );
                    }
                    else {

                        //Overwrite temporary file with the next bitmap
                        bitmapInfo.AviPosition++;
                        aviReader.ExportBitmap( bitmapInfo.AviPosition, bitmapInfo.SourceFileName );
                        bitmapInfo.Bitmap = new Bitmap( bitmapInfo.SourceFileName );
                        bitmapInfo.MessageBytesToHide = imageFiles[ indexBitmaps ].AviMessageBytesToHide[ bitmapInfo.AviPosition ];
                        nextFile = false;
                    }
                }
                if ( nextFile ) {
                    indexBitmaps++;
                    bitmapInfo = LoadBitmap( imageFiles[ indexBitmaps ], aviReader, aviWriter );
                    bitmapWidth = bitmapInfo.Bitmap.Width - 1;
                }

                if ( pixelPosition.X > bitmapWidth ) {
                    pixelPosition.X = 0;
                }
            }
        }

        private static void SaveBitmap( Bitmap bitmap, String fileName ) {
            var fileNameLower = fileName.ToLower();

            var format = ImageFormat.Bmp;
            if ( fileNameLower.EndsWith( "tif" ) || fileNameLower.EndsWith( "tiff" ) ) {
                format = ImageFormat.Tiff;
            }
            else if ( fileNameLower.EndsWith( "png" ) ) {
                format = ImageFormat.Png;
            }

            //copy the bitmap
            Image img = new Bitmap( bitmap );

            //close bitmap file
            bitmap.Dispose();

            //save new bitmap
            img.Save( fileName, format );
            img.Dispose();
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
                    pixelColor = Color.FromArgb( newValue, pixelColor.G, pixelColor.B );
                    break;

                case 1:
                    pixelColor = Color.FromArgb( pixelColor.R, newValue, pixelColor.B );
                    break;

                case 2:
                    pixelColor = Color.FromArgb( pixelColor.R, pixelColor.G, newValue );
                    break;
            }
        }

        private static String UnTrimColorString( String color, Int32 desiredLength ) {
            var difference = desiredLength - color.Length;
            if ( difference > 0 ) {
                color = new String( '0', difference ) + color;
            }
            return color;
        }
    }
}