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

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Graphics.Video;
    using JetBrains.Annotations;

    public static class CryptUtility {

        //TODO use the real temp filename
        private const String TempFileName = "picturekey_temp.bmp";

        /// <summary>Get the value of a bit</summary>
        /// <param name="b">The byte value</param>
        /// <param name="position">The position of the bit</param>
        /// <returns>The value of the bit</returns>
        private static Boolean GetBit( Byte b, Byte position ) => ( b & ( Byte ) ( 1 << position ) ) != 0;

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
                        resultKeyStream.WriteByte( ( Byte ) readByte );
                    }
                }
            }

            return resultKeyStream;
        }

        private static Byte GetReverseKeyByte( [NotNull] Stream keyStream ) {

            //jump to reverse-read position and read from the end of the stream
            var keyPosition = keyStream.Position;
            keyStream.Seek( offset: -keyPosition, origin: SeekOrigin.End );
            var reverseKeyByte = ( Byte ) keyStream.ReadByte();

            //jump back to normal read position
            keyStream.Seek( offset: keyPosition, origin: SeekOrigin.Begin );

            return reverseKeyByte;
        }

        private static void HideBits( [NotNull] Stream keyStream, [NotNull] Stream messageStream, Int64 messageLength, [NotNull] AviReader aviReader,
            [NotNull] AviWriter aviWriter, [NotNull] CarrierImage[] imageFiles, [NotNull] BitmapInfo bitmapInfo, Boolean extract ) {
            if ( keyStream == null ) {
                throw new ArgumentNullException( paramName: nameof( keyStream ) );
            }

            if ( messageStream == null ) {
                throw new ArgumentNullException( paramName: nameof( messageStream ) );
            }

            if ( aviReader == null ) {
                throw new ArgumentNullException( paramName: nameof( aviReader ) );
            }

            if ( aviWriter == null ) {
                throw new ArgumentNullException( paramName: nameof( aviWriter ) );
            }

            if ( imageFiles == null ) {
                throw new ArgumentNullException( paramName: nameof( imageFiles ) );
            }

            if ( bitmapInfo == null ) {
                throw new ArgumentNullException( paramName: nameof( bitmapInfo ) );
            }

            //Color component to hide the next byte in (0-R, 1-G, 2-B)
            //Rotates with every hidden byte
            var currentColorComponent = 0;

            //Index of the current bitmap
            var indexBitmaps = 0;

            //Maximum X and Y position in the current bitmap
            var bitmapWidth = bitmapInfo.Bitmap.Width - 1;

            //Current position in the carrier bitmap
            //Start with 1, because (0,0) contains the message length
            var pixelPosition = new Point( x: 1, y: 0 );

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
                    currentByte = ( Byte ) messageStream.ReadByte();

                    //To add a bit of confusion, xor the byte with a byte read from the keyStream
                    currentByte = ( Byte ) ( currentByte ^ currentReverseKeyByte );
                }

                for ( Byte bitPosition = 0; bitPosition < 8; bitPosition++ ) {
                    MovePixelPosition( extract: extract, aviReader: aviReader, aviWriter: aviWriter, imageFiles: imageFiles, keyStream: keyStream,
                        countBytesInCurrentImage: ref countBytesInCurrentImage, indexBitmaps: ref indexBitmaps, pixelPosition: ref pixelPosition, bitmapWidth: ref bitmapWidth,
                        bitmapInfo: ref bitmapInfo );

                    //Get color of the "clean" pixel
                    var pixelColor = bitmapInfo.Bitmap.GetPixel( x: pixelPosition.X, y: pixelPosition.Y );

                    if ( extract ) {

                        //Extract the hidden message-byte from the color
                        var foundByte = GetColorComponent( pixelColor: pixelColor, colorComponent: currentColorComponent );
                        var foundBit = GetBit( b: foundByte, position: 0 );
                        currentByte = SetBit( b: currentByte, position: bitPosition, newBitValue: foundBit );

                        //Rotate color components
                        currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                    }
                    else {
                        var currentBit = GetBit( b: currentByte, position: bitPosition );

                        if ( imageFiles[ indexBitmaps ].UseGrayscale ) {
                            var r = SetBit( b: pixelColor.R, position: 0, newBitValue: currentBit );
                            var g = SetBit( b: pixelColor.G, position: 0, newBitValue: currentBit );
                            var b = SetBit( b: pixelColor.B, position: 0, newBitValue: currentBit );
                            pixelColor = Color.FromArgb( red: r, green: g, blue: b );
                        }
                        else {

                            //Change one component of the color to the message-byte
                            var colorComponentValue = GetColorComponent( pixelColor: pixelColor, colorComponent: currentColorComponent );
                            colorComponentValue = SetBit( b: colorComponentValue, position: 0, newBitValue: currentBit );
                            SetColorComponent( pixelColor: ref pixelColor, colorComponent: currentColorComponent, newValue: colorComponentValue );

                            //Rotate color components
                            currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                        }

                        bitmapInfo.Bitmap.SetPixel( x: pixelPosition.X, y: pixelPosition.Y, color: pixelColor );
                    }
                }

                if ( extract ) {
                    currentByte = ( Byte ) ( currentByte ^ currentReverseKeyByte );
                    messageStream.WriteByte( currentByte );
                }

                countBytesInCurrentImage++;
            }

            //Save last image
            if ( !extract ) {
                if ( bitmapInfo.AviPosition < 0 ) {

                    //Save bitmap
                    SaveBitmap( bitmap: bitmapInfo.Bitmap, fileName: imageFiles[ indexBitmaps ].ResultFileName );
                }
                else {

                    //Write frame
                    aviWriter.AddFrame( bmp: bitmapInfo.Bitmap );
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
        /// <param name="messageStream">A stream containing the message (extract==false) or an empty stream (extract==true)</param>
        /// <param name="messageLength">Expected length of the message</param>
        /// <param name="aviWriter"></param>
        /// <param name="imageFiles">CarrierImages describing the bitmaps</param>
        /// <param name="bitmapInfo"></param>
        /// <param name="extract">Hide the message (false) or extract it (true)</param>
        /// <param name="aviReader"></param>
        private static void HideBytes( [NotNull] Stream keyStream, [NotNull] Stream messageStream, Int64 messageLength, [NotNull] AviReader aviReader,
            [NotNull] AviWriter aviWriter, [NotNull] CarrierImage[] imageFiles, [NotNull] BitmapInfo bitmapInfo, Boolean extract ) {
            if ( keyStream == null ) {
                throw new ArgumentNullException( paramName: nameof( keyStream ) );
            }

            if ( messageStream == null ) {
                throw new ArgumentNullException( paramName: nameof( messageStream ) );
            }

            if ( aviReader == null ) {
                throw new ArgumentNullException( paramName: nameof( aviReader ) );
            }

            if ( aviWriter == null ) {
                throw new ArgumentNullException( paramName: nameof( aviWriter ) );
            }

            if ( imageFiles == null ) {
                throw new ArgumentNullException( paramName: nameof( imageFiles ) );
            }

            if ( bitmapInfo == null ) {
                throw new ArgumentNullException( paramName: nameof( bitmapInfo ) );
            }

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
            var pixelPosition = new Point( x: 1, y: 0 );

            //Count of bytes already hidden in the current image
            var countBytesInCurrentImage = 0;

            //Stores the color of a pixel

            //A value read from the key stream in reverse direction

            for ( var messageIndex = 0; messageIndex < messageLength; messageIndex++ ) {
                MovePixelPosition( extract: extract, aviReader: aviReader, aviWriter: aviWriter, imageFiles: imageFiles, keyStream: keyStream,
                    countBytesInCurrentImage: ref countBytesInCurrentImage, indexBitmaps: ref indexBitmaps, pixelPosition: ref pixelPosition, bitmapWidth: ref bitmapWidth,
                    bitmapInfo: ref bitmapInfo );

                var currentReverseKeyByte = GetReverseKeyByte( keyStream: keyStream );
                countBytesInCurrentImage++;

                //Get color of the "clean" pixel
                var pixelColor = bitmapInfo.Bitmap.GetPixel( x: pixelPosition.X, y: pixelPosition.Y );

                if ( extract ) {

                    //Extract the hidden message-byte from the color
                    var foundByte = ( Byte ) ( currentReverseKeyByte ^ GetColorComponent( pixelColor: pixelColor, colorComponent: currentColorComponent ) );
                    messageStream.WriteByte( foundByte );

                    //Rotate color components
                    currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                }
                else {

                    //To add a bit of confusion, xor the byte with a byte read from the keyStream
                    var currentByte = messageStream.ReadByte() ^ currentReverseKeyByte;

                    if ( imageFiles[ indexBitmaps ].UseGrayscale ) {
                        pixelColor = Color.FromArgb( red: currentByte, green: currentByte, blue: currentByte );
                    }
                    else {

                        //Change one component of the color to the message-byte
                        SetColorComponent( pixelColor: ref pixelColor, colorComponent: currentColorComponent, newValue: currentByte );

                        //Rotate color components
                        currentColorComponent = currentColorComponent == 2 ? 0 : currentColorComponent + 1;
                    }

                    bitmapInfo.Bitmap.SetPixel( x: pixelPosition.X, y: pixelPosition.Y, color: pixelColor );
                }
            }

            //Save last image
            if ( !extract ) {
                if ( bitmapInfo.AviPosition < 0 ) {

                    //Save bitmap
                    SaveBitmap( bitmap: bitmapInfo.Bitmap, fileName: imageFiles[ indexBitmaps ].ResultFileName );
                }
                else {

                    //Write frame
                    aviWriter.AddFrame( bmp: bitmapInfo.Bitmap );
                }
            }

            bitmapInfo.Bitmap.Dispose();
        }

        /// <summary>Steps through the pixels of bitmaps using a key pattern and hides or extracts a message</summary>
        /// <param name="messageStream">If exctract is false, the message to hide - otherwise an empty stream to receive the extracted message</param>
        /// <param name="splitBytes"></param>
        /// <param name="extract">Extract a hidden message (true), or hide a message in a clean carrier bitmap (false)</param>
        /// <param name="imageFiles"></param>
        /// <param name="keys"></param>
        private static void HideOrExtract( [NotNull] ref Stream messageStream, [NotNull] CarrierImage[] imageFiles, [NotNull] IReadOnlyList<FilePasswordPair> keys,
            Boolean splitBytes, Boolean extract ) {
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
            var bitmapInfo = LoadBitmap( imageFile: imageFiles[ 0 ], aviReader: aviReader, aviWriter: aviWriter );

            //Stores the color of a pixel
            Color pixelColor;

            //Length of the message
            Int32 messageLength;

            //combine all keys
            Stream keyStream = GetKeyStream( keys: keys );

            if ( extract ) {

                //Read the length of the hidden message from the first pixel
                pixelColor = bitmapInfo.Bitmap.GetPixel( x: 0, y: 0 );
                messageLength = ( pixelColor.R << 16 ) + ( pixelColor.G << 8 ) + pixelColor.B;
                messageStream = new MemoryStream( capacity: messageLength );
            }
            else {
                messageLength = ( Int32 ) messageStream.Length;

                if ( messageStream.Length >= 0xFFFFFF ) {
                    throw new Exception( "The message is too long, only 16777215 bytes are allowed." );
                }
            }

            //calculate count of message-bytes to hide in (or extract from) each image
            Int64 sumBytes = 0;

            for ( var n = 0; n < imageFiles.Length; n++ ) {
                var pixels = imageFiles[ n ].CountPixels / ( Single ) countPixels;
                imageFiles[ n ].SetCountBytesToHide( messageBytesToHide: ( Int64 ) Math.Ceiling( a: messageLength * pixels ) );
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
                            keyStream.Seek( offset: 0, origin: SeekOrigin.Begin );
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
                pixelColor = Color.FromArgb( red: red, green: green, blue: blue );
                bitmapInfo.Bitmap.SetPixel( x: 0, y: 0, color: pixelColor );
            }

            //Reset the streams
            keyStream.Seek( offset: 0, origin: SeekOrigin.Begin );
            messageStream.Seek( offset: 0, origin: SeekOrigin.Begin );

            //Loop over the message and hide each byte
            if ( splitBytes ) {
                HideBits( keyStream: keyStream, messageStream: messageStream, messageLength: messageLength, aviReader: aviReader, aviWriter: aviWriter, imageFiles: imageFiles,
                    bitmapInfo: bitmapInfo, extract: extract );
            }
            else {
                HideBytes( keyStream: keyStream, messageStream: messageStream, messageLength: messageLength, aviReader: aviReader, aviWriter: aviWriter,
                    imageFiles: imageFiles, bitmapInfo: bitmapInfo, extract: extract );
            }

            //Close AVI files
            aviWriter.Close();
            aviReader.Close();

            //Delete temporary file
            var fileName = Application.ExecutablePath;
            var index = fileName.LastIndexOf( "\\", comparisonType: StringComparison.Ordinal ) + 1;
            fileName = fileName.Substring( startIndex: 0, index ) + TempFileName;

            if ( File.Exists( fileName ) ) {
                File.Delete( fileName );
            }

            keyStream.Close();
        }

        [NotNull]
        private static BitmapInfo LoadBitmap( CarrierImage imageFile, [CanBeNull] AviReader aviReader, [CanBeNull] AviWriter aviWriter ) {
            var bitmapInfo = new BitmapInfo();

            if ( imageFile.SourceFileName.ToLower().EndsWith( ".avi" ) ) {
                try {

                    //first carrier image is a video - extract the first frame
                    aviReader.Open( fileName: imageFile.SourceFileName );

                    if ( imageFile.ResultFileName.Length > 0 ) {
                        aviWriter.Open( fileName: imageFile.ResultFileName, frameRate: aviReader.FrameRate );
                    }

                    var fileName = Application.ExecutablePath;
                    var index = fileName.LastIndexOf( "\\", comparisonType: StringComparison.Ordinal ) + 1;
                    fileName = fileName.Substring( startIndex: 0, index ) + TempFileName;

                    aviReader.ExportBitmap( position: 0, dstFileName: fileName );
                    bitmapInfo.LoadBitmap( fileName: fileName );
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
                bitmapInfo.LoadBitmap( fileName: imageFile.SourceFileName );
                bitmapInfo.MessageBytesToHide = imageFile.MessageBytesToHide;
                bitmapInfo.AviPosition = -1;
                bitmapInfo.AviCountFrames = 0;
            }

            return bitmapInfo;
        }

        private static void MovePixelPosition( Boolean extract, [CanBeNull] AviReader aviReader, [CanBeNull] AviWriter aviWriter, [CanBeNull] CarrierImage[] imageFiles,
            [NotNull] Stream keyStream, ref Int32 countBytesInCurrentImage, ref Int32 indexBitmaps, ref Point pixelPosition, ref Int32 bitmapWidth,
            ref BitmapInfo bitmapInfo ) {

            //Repeat the key, if it is shorter than the message
            if ( keyStream.Position == keyStream.Length ) {
                keyStream.Seek( offset: 0, origin: SeekOrigin.Begin );
            }

            //Get the next pixel-count from the key, use "1" if it's 0
            var currentKeyByte = ( Byte ) keyStream.ReadByte();
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
                        SaveBitmap( bitmap: bitmapInfo.Bitmap, fileName: imageFiles[ indexBitmaps ].ResultFileName );
                    }
                    else {

                        //Write frame
                        aviWriter.AddFrame( bmp: bitmapInfo.Bitmap );
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
                        aviReader.ExportBitmap( position: bitmapInfo.AviPosition, dstFileName: bitmapInfo.SourceFileName );
                        bitmapInfo.Bitmap = new Bitmap( filename: bitmapInfo.SourceFileName );
                        bitmapInfo.MessageBytesToHide = imageFiles[ indexBitmaps ].AviMessageBytesToHide[ bitmapInfo.AviPosition ];
                        nextFile = false;
                    }
                }

                if ( nextFile ) {
                    indexBitmaps++;
                    bitmapInfo = LoadBitmap( imageFile: imageFiles[ indexBitmaps ], aviReader: aviReader, aviWriter: aviWriter );
                    bitmapWidth = bitmapInfo.Bitmap.Width - 1;
                }

                if ( pixelPosition.X > bitmapWidth ) {
                    pixelPosition.X = 0;
                }
            }
        }

        private static void SaveBitmap( [NotNull] Bitmap bitmap, [NotNull] String fileName ) {
            var fileNameLower = fileName.ToLower();

            var format = ImageFormat.Bmp;

            if ( fileNameLower.EndsWith( "tif" ) || fileNameLower.EndsWith( "tiff" ) ) {
                format = ImageFormat.Tiff;
            }
            else if ( fileNameLower.EndsWith( "png" ) ) {
                format = ImageFormat.Png;
            }

            //copy the bitmap
            Image img = new Bitmap( original: bitmap );

            //close bitmap file
            bitmap.Dispose();

            //save new bitmap
            img.Save( filename: fileName, format: format );
            img.Dispose();
        }

        /// <summary>Set a bit to [newBitValue]</summary>
        /// <param name="b">The byte value</param>
        /// <param name="position">The position (1-8) of the bit</param>
        /// <param name="newBitValue">The new value of the bit in position [position]</param>
        /// <returns>The new byte value</returns>
        private static Byte SetBit( Byte b, Byte position, Boolean newBitValue ) {
            var mask = ( Byte ) ( 1 << position );

            if ( newBitValue ) {
                return ( Byte ) ( b | mask );
            }

            return ( Byte ) ( b & ~mask );
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
                resultStream.WriteByte( ( Byte ) currentByte );

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

        /// <summary>Extracts an hidden message from a bitmap</summary>
        /// <param name="keys"></param>
        /// <param name="messageStream">Empty stream to receive the message</param>
        /// <param name="imageFiles"></param>
        /// <param name="splitBytes"></param>
        public static void ExtractMessageFromBitmap( [NotNull] CarrierImage[] imageFiles, [NotNull] FilePasswordPair[] keys, [NotNull] ref Stream messageStream,
            Boolean splitBytes ) =>
            HideOrExtract( messageStream: ref messageStream, imageFiles: imageFiles, keys: keys, splitBytes: splitBytes, extract: true );

        /// <summary>Hides a message in a bitmap</summary>
        /// <param name="messageStream">The message to hide</param>
        /// <param name="imageFiles"></param>
        /// <param name="keys"></param>
        /// <param name="splitBytes"></param>
        public static void HideMessageInBitmap( Stream messageStream, [NotNull] CarrierImage[] imageFiles, [NotNull] FilePasswordPair[] keys, Boolean splitBytes ) {
            HideOrExtract( messageStream: ref messageStream, imageFiles: imageFiles, keys: keys, splitBytes: splitBytes, extract: false );

            // ReSharper disable once RedundantAssignment
            messageStream = null; //BUG why is this here?
        }

    }

}