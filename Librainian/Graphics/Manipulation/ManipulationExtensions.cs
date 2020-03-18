// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ManipulationExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "ManipulationExtensions.cs" was last formatted by Protiguous on 2020/03/16 at 9:40 PM.

namespace Librainian.Graphics.Manipulation {

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using JetBrains.Annotations;
    using Maths;
    using OperatingSystem.FileSystem;

    public static class ManipulationExtensions {

        /// <summary>Pulled from http://notes.ericwillis.com/2009/11/pixelate-an-image-with-csharp/</summary>
        /// <param name="image"></param>
        /// <param name="rectangle"></param>
        /// <param name="pixelateSize"></param>
        /// <returns></returns>
        [NotNull]
        private static Bitmap Pixelate( [NotNull] this Bitmap image, Rectangle rectangle, Int32 pixelateSize ) {
            var pixelated = new Bitmap( width: image.Width, height: image.Height );

            // make an exact copy of the bitmap provided
            using ( var graphics = Graphics.FromImage( image: pixelated ) ) {
                graphics.DrawImage( image: image, destRect: new Rectangle( x: 0, y: 0, width: image.Width, height: image.Height ),
                    srcRect: new Rectangle( x: 0, y: 0, width: image.Width, height: image.Height ), srcUnit: GraphicsUnit.Pixel );
            }

            // look at every pixel in the rectangle while making sure we're within the image bounds
            for ( var xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx += pixelateSize ) {
                for ( var yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy += pixelateSize ) {
                    var offsetX = pixelateSize / 2;
                    var offsetY = pixelateSize / 2;

                    // make sure that the offset is within the boundry of the image
                    while ( xx + offsetX >= image.Width ) {
                        offsetX--; //BUG a loop??
                    }

                    while ( yy + offsetY >= image.Height ) {
                        offsetY--; //BUG a loop??
                    }

                    // get the pixel color in the center of the soon to be pixelated area
                    var pixel = pixelated.GetPixel( x: xx + offsetX, y: yy + offsetY );

                    // for each pixel in the pixelate size, set it to the center color
                    for ( var x = xx; x < xx + pixelateSize && x < image.Width; x++ ) {
                        for ( var y = yy; y < yy + pixelateSize && y < image.Height; y++ ) {
                            pixelated.SetPixel( x: x, y: y, color: pixel );
                        }
                    }
                }
            }

            return pixelated;
        }

        [CanBeNull]
        public static Bitmap LoadAndResize( [NotNull] this String document, Single multiplier ) =>
            LoadAndResize( document: new Document( fullPath: document ), multiplier: multiplier );

        [CanBeNull]
        public static Bitmap LoadAndResize( [CanBeNull] Document document, Single multiplier ) {
            if ( !multiplier.IsNumber() ) {
                return null;
            }

            try {
                var image = Image.FromFile( filename: document.FullPath );
                var newSize = new Size( width: ( Int32 ) ( image.Size.Width * multiplier ), height: ( Int32 ) ( image.Size.Height * multiplier ) );

                return new Bitmap( original: image, newSize: newSize );
            }
            catch ( FileNotFoundException ) {
                return null;
            }
            catch ( OutOfMemoryException ) {
                return null;
            }
        }

        [NotNull]
        public static Bitmap MakeGrayscale( [NotNull] this Bitmap original ) {

            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap( width: original.Width, height: original.Height );

            //get a graphics object from the new image
            using ( var g = Graphics.FromImage( image: newBitmap ) ) {

                //create some image attributes
                var attributes = new ImageAttributes();

                //create the grayscale ColorMatrix
                var colorMatrix = new ColorMatrix( newColorMatrix: new[] {
                    new[] {
                        .3f, .3f, .3f, 0, 0
                    },
                    new[] {
                        .59f, .59f, .59f, 0, 0
                    },
                    new[] {
                        .11f, .11f, .11f, 0, 0
                    },
                    new[] {
                        0.0f, 0, 0, 1, 0
                    },
                    new[] {
                        0.0f, 0, 0, 0, 1
                    }
                } );

                //set the color matrix attribute
                attributes.SetColorMatrix( newColorMatrix: colorMatrix );

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage( image: original, destRect: new Rectangle( x: 0, y: 0, width: original.Width, height: original.Height ), srcX: 0, srcY: 0,
                    srcWidth: original.Width, srcHeight: original.Height, srcUnit: GraphicsUnit.Pixel, imageAttr: attributes );
            }

            return newBitmap;
        }

    }

}