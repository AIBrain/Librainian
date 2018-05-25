// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ManipulationExtensions.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/ManipulationExtensions.cs" was last formatted by Protiguous on 2018/05/24 at 7:13 PM.

namespace Librainian.Graphics.Manipulation {

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using ComputerSystems.FileSystem;
    using JetBrains.Annotations;
    using Maths;

    public static class ManipulationExtensions {

        /// <summary>
        ///     Pulled from http://notes.ericwillis.com/2009/11/pixelate-an-image-with-csharp/
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rectangle"></param>
        /// <param name="pixelateSize"></param>
        /// <returns></returns>
        private static Bitmap Pixelate( this Bitmap image, Rectangle rectangle, Int32 pixelateSize ) {
            var pixelated = new Bitmap( image.Width, image.Height );

            // make an exact copy of the bitmap provided
            using ( var graphics = Graphics.FromImage( pixelated ) ) {
                graphics.DrawImage( image, new Rectangle( 0, 0, image.Width, image.Height ), new Rectangle( 0, 0, image.Width, image.Height ), GraphicsUnit.Pixel );
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
                    var pixel = pixelated.GetPixel( xx + offsetX, yy + offsetY );

                    // for each pixel in the pixelate size, set it to the center color
                    for ( var x = xx; x < xx + pixelateSize && x < image.Width; x++ ) {
                        for ( var y = yy; y < yy + pixelateSize && y < image.Height; y++ ) { pixelated.SetPixel( x, y, pixel ); }
                    }
                }
            }

            return pixelated;
        }

        [CanBeNull]
        public static Bitmap LoadAndResize( this String document, Single multiplier ) => LoadAndResize( new Document( document ), multiplier );

        [CanBeNull]
        public static Bitmap LoadAndResize( Document document, Single multiplier ) {
            if ( !multiplier.IsNumber() ) { return null; }

            try {
                var image = Image.FromFile( document.FullPathWithFileName );
                var newSize = new Size( ( Int32 )( image.Size.Width * multiplier ), ( Int32 )( image.Size.Height * multiplier ) );

                return new Bitmap( image, newSize );
            }
            catch ( FileNotFoundException ) { return null; }
            catch ( OutOfMemoryException ) { return null; }
        }

        public static Bitmap MakeGrayscale( this Bitmap original ) {

            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap( original.Width, original.Height );

            //get a graphics object from the new image
            using ( var g = Graphics.FromImage( newBitmap ) ) {

                //create some image attributes
                var attributes = new ImageAttributes();

                //create the grayscale ColorMatrix
                var colorMatrix = new ColorMatrix( new[] { new[] { .3f, .3f, .3f, 0, 0 }, new[] { .59f, .59f, .59f, 0, 0 }, new[] { .11f, .11f, .11f, 0, 0 }, new[] { 0.0f, 0, 0, 1, 0 }, new[] { 0.0f, 0, 0, 0, 1 } } );

                //set the color matrix attribute
                attributes.SetColorMatrix( colorMatrix );

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage( original, new Rectangle( 0, 0, original.Width, original.Height ), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes );
            }

            return newBitmap;
        }
    }
}