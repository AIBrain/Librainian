#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Images.cs" was last cleaned by Rick on 2014/08/24 at 6:51 AM

#endregion License & Information

namespace Librainian.Graphics {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using IO;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Parsing;
    using Threading;

    public static class Images {

        public static Matrix3X2 ComputeForwardTransform( IList<Point> baselineLocations, IList<Point> registerLocations ) {
            if ( baselineLocations.Count < 3 || registerLocations.Count < 3 ) {
                throw new Exception( "Unable to compute the forward transform. A minimum of 3 control point pairs are required." );
            }

            if ( baselineLocations.Count != registerLocations.Count ) {
                throw new Exception( "Unable to compute the forward transform. The number of control point pairs in baseline and registration results must be equal." );
            }

            // To compute 
            //    Transform = ((X^T * X)^-1 * X^T)U = (X^T * X)^-1 (X^T * U)

            //    X^T * X =
            //    [ Sum(x_i^2)   Sum(x_i*y_i) Sum(x_i) ]
            //    [ Sum(x_i*y_i) Sum(y_i^2)   Sum(y_i) ]
            //    [ Sum(x_i)     Sum(y_i)     Sum(1)=n ]

            //    X^T * U =
            //    [ Sum(x_i*u_i) Sum(x_i*v_i) ]
            //    [ Sum(y_i*u_i) Sum(y_i*v_i) ]
            //    [ Sum(u_i)     Sum(v_i) ]

            IList<Point> xy = baselineLocations;
            IList<Point> uv = registerLocations;

            double a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0, n = xy.Count;
            double p = 0, q = 0, r = 0, s = 0, t = 0, u = 0;

            for ( int i = 0; i < n; i++ ) {
                // Compute sum of squares for X^T * X
                a += xy[ i ].X * xy[ i ].X;
                b += xy[ i ].X * xy[ i ].Y;
                c += xy[ i ].X;
                d += xy[ i ].X * xy[ i ].Y;
                e += xy[ i ].Y * xy[ i ].Y;
                f += xy[ i ].Y;
                g += xy[ i ].X;
                h += xy[ i ].Y;

                // Compute sum of squares for X^T * U
                p += xy[ i ].X * uv[ i ].X;
                q += xy[ i ].X * uv[ i ].Y;
                r += xy[ i ].Y * uv[ i ].X;
                s += xy[ i ].Y * uv[ i ].Y;
                t += uv[ i ].X;
                u += uv[ i ].Y;
            }

            // Create matrices from the coefficients
            Matrix3X2 uMat = new Matrix3X2( p, q, r, s, t, u );
            Matrix3X3 xMat = new Matrix3X3( a, b, c, d, e, f, g, h, n );

            // Invert X
            Matrix3X3 xInv = xMat.Inverse;

            // Perform the multiplication to get the transform
            uMat.Multiply( xInv );

            // Matrix uMat now holds the image registration transform to go from the current result to baseline
            return uMat;
        }

        [CanBeNull]
        public static DateTime? GetProperteryAsDateTime( [CanBeNull] this PropertyItem item ) {
            if ( null == item ) {
                return null;
            }

            var value = Encoding.ASCII.GetString( item.Value );
            if ( value.EndsWith( "\0", StringComparison.Ordinal ) ) {
                value = value.Replace( "\0", String.Empty );
            }

            if ( value == "0000:00:00 00:00:00" ) {
                return null;
            }

            DateTime result;
            if ( DateTime.TryParse( value, out result ) ) {
                return result;
            }

            if ( DateTime.TryParseExact( value, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result ) ) {
                return result;
            }

            return null;
        }

        [CanBeNull]
        public static DateTime? ImageCreationBestGuess( [CanBeNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            return ImageCreationBestGuess( document.Info );
        }

        /// <summary>
        /// Returns true if the date is 'recent' enough.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Boolean IsDateRecentEnough( this DateTime? dateTime ) => dateTime?.Year >= 1825;

        [CanBeNull]
        public static DateTime? ImageCreationBestGuess( [CanBeNull] this FileSystemInfo info ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }

            DateTime? imageCreationBestGuess;
            if ( InternalImageGetDateTime( info, out imageCreationBestGuess ) ) {
                return imageCreationBestGuess;
            }

            try {
                //try a variety of parsing the dates and times from the file's name.

                //example 1, "blahblahblah_20040823_173454" == "August 23rd, 2010 at 10:34am"
                var justName = Path.GetFileNameWithoutExtension( info.FullName );

                var mostlyDigits = String.Empty;
                foreach ( var c in justName ) {
                    if ( Char.IsDigit( c ) /*|| c == '\\' || c == '-' || c == '/'*/ ) {
                        mostlyDigits += c;
                    }
                    else {
                        mostlyDigits += ParsingExtensions.Singlespace;
                    }
                }
                mostlyDigits = mostlyDigits.Trim( );

                #region Year, Month, Day formats as in digits == "20040823 173454" == "August 23th, 2004 at 5:34pm"

                var patternsYMD = new[ ] { "yyyyMMdd HHmmss", "yyyy MM dd HHmmss", "yyyy MM dd HH mm ss", "yyyy dd MM", "MMddyy HHmmss", "yyyyMMdd", "yyyy MM dd" };

                foreach ( var pattern in patternsYMD ) {
                    DateTime bestGuess;
                    if ( !DateTime.TryParseExact( mostlyDigits.Sub( pattern.Length ), pattern, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out bestGuess ) ) {
                        continue;
                    }
                    if ( IsDateRecentEnough( bestGuess ) ) {
                        return bestGuess;
                    }
                    //if ( Debugger.IsAttached ) {
                    //    Debugger.Break();
                    //}
                }
                #endregion

                #region Day, Month, Year formats as in digits == "23082004 173454" == "August 23rd, 2004 at 5:34pm"

                var patternsDMY = new[ ] { "ddMMyyyy HHmmss", "dd MM yyyy HHmmss", "dd MM yyyy HH mm ss", "dd MM yyyy", "ddMMyy HHmmss", "ddMMyy" };

                foreach ( var pattern in patternsDMY ) {
                    DateTime bestGuess;
                    if ( !DateTime.TryParseExact( mostlyDigits.Sub( pattern.Length ), pattern, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out bestGuess ) ) {
                        continue;
                    }
                    if ( IsDateRecentEnough( bestGuess ) ) {
                        return bestGuess;
                    }
                    //if ( Debugger.IsAttached ) {
                    //    Debugger.Break();
                    //}
                }
                #endregion

                // per http://stackoverflow.com/q/51224/956364
                const string pattern1 = @"^((((0[13578])|([13578])|(1[02]))[\/](([1-9])|([0-2][0-9])|(3[01])))|(((0[469])|([469])|(11))[\/](([1-9])|([0-2][0-9])|(30)))|((2|02)[\/](([1-9])|([0-2][0-9]))))[\/]\d{4}$|^\d{4}$";
                var regs1 = Regex.Matches( justName, pattern1, RegexOptions.IgnorePatternWhitespace, matchTimeout: Seconds.Five );
                foreach ( var reg in regs1 ) {
                    DateTime bestGuess;
                    if ( !DateTime.TryParse( reg.ToString( ), out bestGuess ) ) {
                        continue;
                    }
                    if ( IsDateRecentEnough( bestGuess ) ) {
                        return bestGuess;
                    }
                    //if ( Debugger.IsAttached ) {
                    //    Debugger.Break();
                    //}
                }

                //per http://stackoverflow.com/a/669758/956364
                const string pattern2 = @"^(?:(?:(?:0?[13578]|1[02])(\/|-|\.)31)\1|(?:(?:0?[1,3-9]|1[0-2])(\/|-|\.)(?:29|30)\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:0?2(\/|-|\.)29\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:1[0-2]))(\/|-|\.)(?:0?[1-9]|1\d|2[0-8])\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$";
                var regs2 = Regex.Matches( justName, pattern2, RegexOptions.IgnorePatternWhitespace, matchTimeout: Seconds.Five );
                foreach ( var reg in regs2 ) {
                    DateTime bestGuess;
                    if ( !DateTime.TryParse( reg.ToString( ), out bestGuess ) ) {
                        continue;
                    }
                    if ( IsDateRecentEnough( bestGuess ) ) {
                        return bestGuess;
                    }
                    //if ( Debugger.IsAttached ) {
                    //    Debugger.Break();
                    //}
                }
            }
            catch ( Exception) {
            }

            var lastWriteTime = File.GetLastWriteTime( info.FullName );
            if ( lastWriteTime <= DateTime.UtcNow && IsDateRecentEnough( lastWriteTime ) ) {
                return lastWriteTime;
            }

            var creationTime = File.GetCreationTime( info.FullName );
            if ( creationTime <= DateTime.UtcNow && IsDateRecentEnough( creationTime ) ) {
                return creationTime;
            }

            //if ( Debugger.IsAttached ) {
            //    Debugger.Break();
            //}

            return null;
        }

        /// <summary>
        /// if the image file contains a 'valid' date, use that.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="bestGuess"></param>
        /// <returns></returns>
        private static bool InternalImageGetDateTime( FileSystemInfo info, out DateTime? bestGuess ) {
            bestGuess = null;
            try {
                using (var image = Image.FromFile( filename: info.FullName, useEmbeddedColorManagement: false )) {
                    if ( image.PropertyIdList.Contains( PropertyList.DateTimeDigitized ) ) {
                        var asDateTime = image.GetPropertyItem( PropertyList.DateTimeDigitized ).GetProperteryAsDateTime( );
                        if ( asDateTime.HasValue && IsDateRecentEnough( asDateTime ) ) {
                            {
                                bestGuess = asDateTime.Value;
                                return true;
                            }
                        }
                    }

                    if ( image.PropertyIdList.Contains( PropertyList.DateTimeOriginal ) ) {
                        var asDateTime = image.GetPropertyItem( PropertyList.DateTimeOriginal ).GetProperteryAsDateTime( );
                        if ( asDateTime.HasValue && IsDateRecentEnough( asDateTime ) ) {
                            {
                                bestGuess = asDateTime.Value;
                                return true;
                            }
                        }
                    }

                    if ( image.PropertyIdList.Contains( PropertyList.PropertyTagDateTime ) ) {
                        var asDateTime = image.GetPropertyItem( PropertyList.PropertyTagDateTime ).GetProperteryAsDateTime( );
                        if ( asDateTime.HasValue && IsDateRecentEnough( asDateTime ) ) {
                            {
                                bestGuess = asDateTime.Value;
                                return true;
                            }
                        }
                    }
                }
            }
            catch ( Exception) {
                /*swallow*/
            }
            return false;
        }

        /// <summary>
        ///     <para>Returns true if the file could be loaded as an image.</para>
        ///     <para>
        ///         Uses
        ///         <see
        ///             cref="BitmapImage" />
        ///         first, and then
        ///     </para>
        ///     <para><see cref="Image.FromFile(String)" /> next.</para>
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Boolean IsaValidImage( [NotNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( nameof( document ) );
            }
            return IsaValidImage( new FileInfo( document.FullPathWithFileName ) );
        }

        public static async Task<Boolean> IsSameImage( [CanBeNull] this Document fileA, [CanBeNull] Document fileB ) {
            if ( null == fileA || null == fileB ) {
                return false;
            }

            if ( !fileA.Exists( ) || !fileB.Exists( ) ) {
                return false;
            }

            try {
                var imageA = await Task.Run( ( ) => Image.FromFile( fileA.FullPathWithFileName ) );
                var imageB = await Task.Run( ( ) => Image.FromFile( fileB.FullPathWithFileName ) );


                if ( imageA.Width < imageB.Width && imageA.Height < imageB.Height ) {
                    imageA = ResizeImage( imageA, imageB.Size ); //resize because B is larger
                }
                else if ( imageA.Width > imageB.Width && imageA.Height > imageB.Height ) {
                    imageB = ResizeImage( imageB, imageA.Size ); //resize because A is larger
                }

                return ImageComparer.Compare( imageA, imageB );
            }
            catch ( OutOfMemoryException) {
            }
            catch ( InvalidOperationException) {
            }

            return false;
        }

        /// <summary>
        ///     <para>Returns true if the file could be loaded as an image.</para>
        ///     <para>
        ///         Uses
        ///         <see
        ///             cref="BitmapImage" />
        ///         first, and then
        ///     </para>
        ///     <para><see cref="Image.FromFile(String)" /> next.</para>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Boolean IsaValidImage( [CanBeNull] this FileInfo file ) {
            if ( null == file ) {
                return false;
            }
            try {
                //var isImageGood = true; //HACK safe assumption?

                //try {
                //    var bitmapImage = new BitmapImage( );
                //    bitmapImage.DecodeFailed += ( sender, args ) => isImageGood = false;
                //    bitmapImage.DownloadFailed += ( sender, args ) => isImageGood = false;
                //    bitmapImage.DownloadCompleted += ( sender, args ) => isImageGood = true;
                //    bitmapImage.BeginInit( );
                //    bitmapImage.UriSource = new Uri( file.FullName );
                //    bitmapImage.EndInit( );
                //    if ( bitmapImage.Width <= 0 ) {
                //        return false;
                //    }
                //    if ( bitmapImage.Height <= 0 ) {
                //        return false;
                //    }
                //    bitmapImage.UriSource = null;
                //}
                //catch ( Exception) {
                //    return false;
                //}

                //if ( !isImageGood ) {
                //    return false;
                //}

                using (var bob = Image.FromFile( file.FullName )) {
                    bob.Dispose( );
                    return true;
                }
            }
            catch ( ExternalException) { }
            catch ( InvalidOperationException) { }
            catch ( FileNotFoundException) { }
            catch ( NotSupportedException) { }
            catch ( OutOfMemoryException) { }
            catch ( Exception exception ) {
                exception.More( );
            }
            finally {
                GC.Collect( generation: 0, mode: GCCollectionMode.Forced, blocking: true, compacting: true );   //an attempt to prevent the image handle still being 'held' by some process. Most likely real effect: does NOT help performance.
            }
            return false;
        }

        public static Image ResizeImage( [NotNull] this Image imgToResize, Size size ) {
            if ( imgToResize == null ) {
                throw new ArgumentNullException( nameof( imgToResize ) );
            }

            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            var nPercentW = ( size.Width / ( Single )sourceWidth );
            var nPercentH = ( size.Height / ( Single )sourceHeight );

            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destWidth = ( int )( sourceWidth * nPercent );
            var destHeight = ( int )( sourceHeight * nPercent );

            using (var bitmap = new Bitmap( width: destWidth, height: destHeight )) {
                using (var g = Graphics.FromImage( image: bitmap )) {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage( image: imgToResize, x: 0, y: 0, width: destWidth, height: destHeight );
                }
                return bitmap;
            }
        }

        public static class FileNameExtension {

            /// <summary>
            ///     <para>.tif</para>
            /// </summary>
            /// <seealso cref="http://wikipedia.org/wiki/TIFF"/>
            public static String Tiff => ".tif";
        }

        public static class PropertyList {
            public const int PropertyTagDateTime = 0x132;
            public const int DateTimeOriginal = 36867;
            public const int DateTimeDigitized = 36868;
        }

        /// <summary>
        /// <para>Exif 2.2 Standard (reference: <see cref="http://www.exiv2.org/tags.html"/>)</para>
        /// </summary>
        /// Pulled from project "ExifOrganizer" @ <seealso cref="http://github.com/RiJo/ExifOrganizer/blob/master/MetaParser/Parsers/ExifParser.cs"/>
        public enum ExifId : int {
            ImageProcessingSoftware = 0x000b, // The name and version of the software used to post-process the picture.
            ImageNewSubfileType = 0x00fe, // A general indication of the kind of data contained in this subfile.
            ImageSubfileType = 0x00ff, // A general indication of the kind of data contained in this subfile. This field is deprecated. The NewSubfileType field should be used instead.
            ImageImageWidth = 0x0100, // The number of columns of image data, equal to the number of pixels per row. In JPEG compressed data a JPEG marker is used instead of this tag.
            ImageImageLength = 0x0101, // The number of rows of image data. In JPEG compressed data a JPEG marker is used instead of this tag.
            ImageBitsPerSample = 0x0102, // The number of bits per image component. In this standard each component of the image is 8 bits, so the value for this tag is 8. See also <SamplesPerPixel>. In JPEG compressed data a JPEG marker is used instead of this tag.
            ImageCompression = 0x0103, // The compression scheme used for the image data. When a primary image is JPEG compressed, this designation is not necessary and is omitted. When thumbnails use JPEG compression, this tag value is set to 6.
            ImagePhotometricInterpretation = 0x0106, // The pixel composition. In JPEG compressed data a JPEG marker is used instead of this tag.
            ImageThreshholding = 0x0107, // For black and white TIFF files that represent shades of gray, the technique used to convert from gray to black and white pixels.
            ImageCellWidth = 0x0108, // The width of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.
            ImageCellLength = 0x0109, // The length of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.
            ImageFillOrder = 0x010a, // The logical order of bits within a byte
            ImageDocumentName = 0x010d, // The name of the document from which this image was scanned
            ImageImageDescription = 0x010e, // A character string giving the title of the Image It may be a comment such as "1988 company picnic" or the like. Two-bytes character codes cannot be used. When a 2-bytes code is necessary, the Exif Private tag <UserComment> is to be used.
            ImageMake = 0x010f, // The manufacturer of the recording equipment. This is the manufacturer of the DSC, scanner, video digitizer or other equipment that generated the Image When the field is left blank, it is treated as unknown.
            ImageModel = 0x0110, // The model name or model number of the equipment. This is the model name or number of the DSC, scanner, video digitizer or other equipment that generated the Image When the field is left blank, it is treated as unknown.
            ImageStripOffsets = 0x0111, // For each strip, the byte offset of that strip. It is recommended that this be selected so the number of strip bytes does not exceed 64 Kbytes. With JPEG compressed data this designation is not needed and is omitted. See also <RowsPerStrip> and <StripByteCounts>.
            ImageOrientation = 0x0112, // The image orientation viewed in terms of rows and columns.
            ImageSamplesPerPixel = 0x0115, // The number of components per pixel. Since this standard applies to RGB and YCbCr images, the value set for this tag is 3. In JPEG compressed data a JPEG marker is used instead of this tag.
            ImageRowsPerStrip = 0x0116, // The number of rows per strip. This is the number of rows in the image of one strip when an image is divided into strips. With JPEG compressed data this designation is not needed and is omitted. See also <StripOffsets> and <StripByteCounts>.
            ImageStripByteCounts = 0x0117, // The total number of bytes in each strip. With JPEG compressed data this designation is not needed and is omitted.
            ImageXResolution = 0x011a, // The number of pixels per <ResolutionUnit> in the <ImageWidth> direction. When the image resolution is unknown, 72 [dpi] is designated.
            ImageYResolution = 0x011b, // The number of pixels per <ResolutionUnit> in the <ImageLength> direction. The same value as <XResolution> is designated.
            ImagePlanarConfiguration = 0x011c, // Indicates whether pixel components are recorded in a chunky or planar format. In JPEG compressed files a JPEG marker is used instead of this tag. If this field does not exist, the TIFF default of 1 (chunky) is assumed.
            ImageGrayResponseUnit = 0x0122, // The precision of the information contained in the GrayResponseCurve.
            ImageGrayResponseCurve = 0x0123, // For grayscale data, the optical density of each possible pixel value.
            ImageT4Options = 0x0124, // T.4-encoding options.
            ImageT6Options = 0x0125, // T.6-encoding options.
            ImageResolutionUnit = 0x0128, // The unit for measuring <XResolution> and <YResolution>. The same unit is used for both <XResolution> and <YResolution>. If the image resolution is unknown, 2 (inches) is designated.
            ImageTransferFunction = 0x012d, // A transfer function for the image, described in tabular style. Normally this tag is not necessary, since color space is specified in the color space information tag (<ColorSpace>).
            ImageSoftware = 0x0131, // This tag records the name and version of the software or firmware of the camera or image input device used to generate the Image The detailed format is not specified, but it is recommended that the example shown below be followed. When the field is left blank, it is treated as unknown.
            ImageDateTime = 0x0132, // The date and time of image creation. In Exif standard, it is the date and time the file was changed.
            ImageArtist = 0x013b, // This tag records the name of the camera owner, photographer or image creator. The detailed format is not specified, but it is recommended that the information be written as in the example below for ease of Interoperability. When the field is left blank, it is treated as unknown. Ex.) "Camera owner, John Smith; Photographer, Michael Brown; Image creator, Ken James"
            ImageHostComputer = 0x013c, // This tag records information about the host computer used to generate the Image
            ImagePredictor = 0x013d, // A predictor is a mathematical operator that is applied to the image data before an encoding scheme is applied.
            ImageWhitePoint = 0x013e, // The chromaticity of the white point of the Image Normally this tag is not necessary, since color space is specified in the colorspace information tag (<ColorSpace>).
            ImagePrimaryChromaticities = 0x013f, // The chromaticity of the three primary colors of the Image Normally this tag is not necessary, since colorspace is specified in the colorspace information tag (<ColorSpace>).
            ImageColorMap = 0x0140, // A color map for palette color images. This field defines a Red-Green-Blue color map (often called a lookup table) for palette-color images. In a palette-color image, a pixel value is used to index into an RGB lookup table.
            ImageHalftoneHints = 0x0141, // The purpose of the HalftoneHints field is to convey to the halftone function the range of gray levels within a colorimetrically-specified image that should retain tonal detail.
            ImageTileWidth = 0x0142, // The tile width in pixels. This is the number of columns in each tile.
            ImageTileLength = 0x0143, // The tile length (height) in pixels. This is the number of rows in each tile.
            ImageTileOffsets = 0x0144, // For each tile, the byte offset of that tile, as compressed and stored on disk. The offset is specified with respect to the beginning of the TIFF file. Note that this implies that each tile has a location independent of the locations of other tiles.
            ImageTileByteCounts = 0x0145, // For each tile, the number of (compressed) bytes in that tile. See TileOffsets for a description of how the byte counts are ordered.
            ImageSubIFDs = 0x014a, // Defined by Adobe Corporation to enable TIFF Trees within a TIFF file.
            ImageInkSet = 0x014c, // The set of inks used in a separated (PhotometricInterpretation=5) Image
            ImageInkNames = 0x014d, // The name of each ink used in a separated (PhotometricInterpretation=5) Image
            ImageNumberOfInks = 0x014e, // The number of inks. Usually equal to SamplesPerPixel, unless there are extra samples.
            ImageDotRange = 0x0150, // The component values that correspond to a 0% dot and 100% dot.
            ImageTargetPrinter = 0x0151, // A description of the printing environment for which this separation is intended.
            ImageExtraSamples = 0x0152, // Specifies that each pixel has m extra components whose interpretation is defined by one of the values listed below.
            ImageSampleFormat = 0x0153, // This field specifies how to interpret each data sample in a pixel.
            ImageSMinSampleValue = 0x0154, // This field specifies the minimum sample value.
            ImageSMaxSampleValue = 0x0155, // This field specifies the maximum sample value.
            ImageTransferRange = 0x0156, // Expands the range of the TransferFunction
            ImageClipPath = 0x0157, // A TIFF ClipPath is intended to mirror the essentials of PostScript's path creation functionality.
            ImageXClipPathUnits = 0x0158, // The number of units that span the width of the image, in terms of integer ClipPath coordinates.
            ImageYClipPathUnits = 0x0159, // The number of units that span the height of the image, in terms of integer ClipPath coordinates.
            ImageIndexed = 0x015a, // Indexed images are images where the 'pixels' do not represent color values, but rather an index (usually 8-bit) into a separate color table, the ColorMap.
            ImageJPEGTables = 0x015b, // This optional tag may be used to encode the JPEG quantization andHuffman tables for subsequent use by the JPEG decompression process.
            ImageOPIProxy = 0x015f, // OPIProxy gives information concerning whether this image is a low-resolution proxy of a high-resolution image (Adobe OPI).
            ImageJPEGProc = 0x0200, // This field indicates the process used to produce the compressed data
            ImageJPEGInterchangeFormat = 0x0201, // The offset to the start byte (SOI) of JPEG compressed thumbnail data. This is not used for primary image JPEG data.
            ImageJPEGInterchangeFormatLength = 0x0202, // The number of bytes of JPEG compressed thumbnail data. This is not used for primary image JPEG data. JPEG thumbnails are not divided but are recorded as a continuous JPEG bitstream from SOI to EOI. Appn and COM markers should not be recorded. Compressed thumbnails must be recorded in no more than 64 Kbytes, including all other data to be recorded in APP1.
            ImageJPEGRestartInterval = 0x0203, // This Field indicates the length of the restart interval used in the compressed image data.
            ImageJPEGLosslessPredictors = 0x0205, // This Field points to a list of lossless predictor-selection values, one per component.
            ImageJPEGPointTransforms = 0x0206, // This Field points to a list of point transform values, one per component.
            ImageJPEGQTables = 0x0207, // This Field points to a list of offsets to the quantization tables, one per component.
            ImageJPEGDCTables = 0x0208, // This Field points to a list of offsets to the DC Huffman tables or the lossless Huffman tables, one per component.
            ImageJPEGACTables = 0x0209, // This Field points to a list of offsets to the Huffman AC tables, one per component.
            ImageYCbCrCoefficients = 0x0211, // The matrix coefficients for transformation from RGB to YCbCr image data. No default is given in TIFF; but here the value given in Appendix E, "Color Space Guidelines", is used as the default. The color space is declared in a color space information tag, with the default being the value that gives the optimal image characteristics Interoperability this condition.
            ImageYCbCrSubSampling = 0x0212, // The sampling ratio of chrominance components in relation to the luminance component. In JPEG compressed data a JPEG marker is used instead of this tag.
            ImageYCbCrPositioning = 0x0213, // The position of chrominance components in relation to the luminance component. This field is designated only for JPEG compressed data or uncompressed YCbCr data. The TIFF default is 1 (centered); but when Y:Cb:Cr = 4:2:2 it is recommended in this standard that 2 (co-sited) be used to record data, in order to improve the image quality when viewed on TV systems. When this field does not exist, the reader shall assume the TIFF default. In the case of Y:Cb:Cr = 4:2:0, the TIFF default (centered) is recommended. If the reader does not have the capability of supporting both kinds of <YCbCrPositioning>, it shall follow the TIFF default regardless of the value in this field. It is preferable that readers be able to support both centered and co-sited positioning.
            ImageReferenceBlackWhite = 0x0214, // The reference black point value and reference white point value. No defaults are given in TIFF, but the values below are given as defaults here. The color space is declared in a color space information tag, with the default being the value that gives the optimal image characteristics Interoperability these conditions.
            ImageXMLPacket = 0x02bc, // XMP Metadata (Adobe technote 9-14-02)
            ImageRating = 0x4746, // Rating tag used by Windows
            ImageRatingPercent = 0x4749, // Rating tag used by Windows, value in percent
            ImageImageID = 0x800d, // ImageID is the full pathname of the original, high-resolution image, or any other identifying string that uniquely identifies the original image (Adobe OPI).
            ImageCFARepeatPatternDim = 0x828d, // Contains two values representing the minimum rows and columns to define the repeating patterns of the color filter array
            ImageCFAPattern = 0x828e, // Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used. It does not apply to all sensing methods
            ImageBatteryLevel = 0x828f, // Contains a value of the battery level as a fraction or string
            ImageCopyright = 0x8298, // Copyright information. In this standard the tag is used to indicate both the photographer and editor copyrights. It is the copyright notice of the person or organization claiming rights to the Image The Interoperability copyright statement including date and rights should be written in this field; e.g., "Copyright, John Smith, 19xx. All rights reserved.". In this standard the field records both the photographer and editor copyrights, with each recorded in a separate part of the statement. When there is a clear distinction between the photographer and editor copyrights, these are to be written in the order of photographer followed by editor copyright, separated by NULL (in this case since the statement also ends with a NULL, there are two NULL codes). When only the photographer copyright is given, it is terminated by one NULL code . When only the editor copyright is given, the photographer copyright part consists of one space followed by a terminating NULL code, then the editor copyright is given. When the field is left blank, it is treated as unknown.
            ImageExposureTime = 0x829a, // Exposure time, given in seconds.
            ImageFNumber = 0x829d, // The F number.
            ImageIPTCNAA = 0x83bb, // Contains an IPTC/NAA record
            ImageImageResources = 0x8649, // Contains information embedded by the Adobe Photoshop application
            ImageExifTag = 0x8769, // A pointer to the Exif IFD. Interoperability, Exif IFD has the same structure as that of the IFD specified in TIFF. ordinarily, however, it does not contain image data as in the case of TIFF.
            ImageInterColorProfile = 0x8773, // Contains an InterColor Consortium (ICC) format color space characterization/profile
            ImageExposureProgram = 0x8822, // The class of the program used by the camera to set exposure when the picture is taken.
            ImageSpectralSensitivity = 0x8824, // Indicates the spectral sensitivity of each channel of the camera used.
            ImageGPSTag = 0x8825, // A pointer to the GPS Info IFD. The Interoperability structure of the GPS Info IFD, like that of Exif IFD, has no image data.
            ImageISOSpeedRatings = 0x8827, // Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.
            ImageOECF = 0x8828, // Indicates the Opto-Electric Conversion Function (OECF) specified in ISO 14524.
            ImageInterlace = 0x8829, // Indicates the field number of multifield images.
            ImageTimeZoneOffset = 0x882a, // This optional tag encodes the time zone of the camera clock (relativeto Greenwich Mean Time) used to create the DataTimeOriginal tag-valuewhen the picture was taken. It may also contain the time zone offsetof the clock used to create the DateTime tag-value when the image wasmodified.
            ImageSelfTimerMode = 0x882b, // Number of seconds image capture was delayed from button press.
            ImageDateTimeOriginal = 0x9003, // The date and time when the original image data was generated.
            ImageCompressedBitsPerPixel = 0x9102, // Specific to compressed data; states the compressed bits per pixel.
            ImageShutterSpeedValue = 0x9201, // Shutter speed.
            ImageApertureValue = 0x9202, // The lens aperture.
            ImageBrightnessValue = 0x9203, // The value of brightness.
            ImageExposureBiasValue = 0x9204, // The exposure bias.
            ImageMaxApertureValue = 0x9205, // The smallest F number of the lens.
            ImageSubjectDistance = 0x9206, // The distance to the subject, given in meters.
            ImageMeteringMode = 0x9207, // The metering mode.
            ImageLightSource = 0x9208, // The kind of light source.
            ImageFlash = 0x9209, // Indicates the status of flash when the image was shot.
            ImageFocalLength = 0x920a, // The actual focal length of the lens, in mm.
            ImageFlashEnergy = 0x920b, // Amount of flash energy (BCPS).
            ImageSpatialFrequencyResponse = 0x920c, // SFR of the camera.
            ImageNoise = 0x920d, // Noise measurement values.
            ImageFocalPlaneXResolution = 0x920e, // Number of pixels per FocalPlaneResolutionUnit (37392) in ImageWidth direction for main Image
            ImageFocalPlaneYResolution = 0x920f, // Number of pixels per FocalPlaneResolutionUnit (37392) in ImageLength direction for main Image
            ImageFocalPlaneResolutionUnit = 0x9210, // Unit of measurement for FocalPlaneXResolution(37390) and FocalPlaneYResolution(37391).
            ImageImageNumber = 0x9211, // Number assigned to an image, e.g., in a chained image burst.
            ImageSecurityClassification = 0x9212, // Security classification assigned to the Image
            ImageImageHistory = 0x9213, // Record of what has been done to the Image
            ImageSubjectLocation = 0x9214, // Indicates the location and area of the main subject in the overall scene.
            ImageExposureIndex = 0x9215, // Encodes the camera exposure index setting when image was captured.
            ImageTIFFEPStandardID = 0x9216, // Contains four ASCII characters representing the TIFF/EP standard version of a TIFF/EP file, eg '1', '0', '0', '0'
            ImageSensingMethod = 0x9217, // Type of image sensor.
            ImageXPTitle = 0x9c9b, // Title tag used by Windows, encoded in UCS2
            ImageXPComment = 0x9c9c, // Comment tag used by Windows, encoded in UCS2
            ImageXPAuthor = 0x9c9d, // Author tag used by Windows, encoded in UCS2
            ImageXPKeywords = 0x9c9e, // Keywords tag used by Windows, encoded in UCS2
            ImageXPSubject = 0x9c9f, // Subject tag used by Windows, encoded in UCS2
            ImagePrintImageMatching = 0xc4a5, // Print Image Matching, description needed.
            ImageDNGVersion = 0xc612, // This tag encodes the DNG four-tier version number. For files compliant with version 1.1.0.0 of the DNG specification, this tag should contain the bytes: 1, 1, 0, 0.
            ImageDNGBackwardVersion = 0xc613, // This tag specifies the oldest version of the Digital Negative specification for which a file is compatible. Readers shouldnot attempt to read a file if this tag specifies a version number that is higher than the version number of the specification the reader was based on. In addition to checking the version tags, readers should, for all tags, check the types, counts, and values, to verify it is able to correctly read the file.
            ImageUniqueCameraModel = 0xc614, // Defines a unique, non-localized name for the camera model that created the image in the raw file. This name should include the manufacturer's name to avoid conflicts, and should not be localized, even if the camera name itself is localized for different markets (see LocalizedCameraModel). This string may be used by reader software to index into per-model preferences and replacement profiles.
            ImageLocalizedCameraModel = 0xc615, // Similar to the UniqueCameraModel field, except the name can be localized for different markets to match the localization of the camera name.
            ImageCFAPlaneColor = 0xc616, // Provides a mapping between the values in the CFAPattern tag and the plane numbers in LinearRaw space. This is a required tag for non-RGB CFA images.
            ImageCFALayout = 0xc617, // Describes the spatial layout of the CFA.
            ImageLinearizationTable = 0xc618, // Describes a lookup table that maps stored values into linear values. This tag is typically used to increase compression ratios by storing the raw data in a non-linear, more visually uniform space with fewer total encoding levels. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
            ImageBlackLevelRepeatDim = 0xc619, // Specifies repeat pattern size for the BlackLevel tag.
            ImageBlackLevel = 0xc61a, // Specifies the zero light (a.k.a. thermal black or black current) encoding level, as a repeating pattern. The origin of this pattern is the top-left corner of the ActiveArea rectangle. The values are stored in row-column-sample scan order.
            ImageBlackLevelDeltaH = 0xc61b, // If the zero light encoding level is a function of the image column, BlackLevelDeltaH specifies the difference between the zero light encoding level for each column and the baseline zero light encoding level. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
            ImageBlackLevelDeltaV = 0xc61c, // If the zero light encoding level is a function of the image row, this tag specifies the difference between the zero light encoding level for each row and the baseline zero light encoding level. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
            ImageWhiteLevel = 0xc61d, // This tag specifies the fully saturated encoding level for the raw sample values. Saturation is caused either by the sensor itself becoming highly non-linear in response, or by the camera's analog to digital converter clipping.
            ImageDefaultScale = 0xc61e, // DefaultScale is required for cameras with non-square pixels. It specifies the default scale factors for each direction to convert the image to square pixels. Typically these factors are selected to approximately preserve total pixel count. For CFA images that use CFALayout equal to 2, 3, 4, or 5, such as the Fujifilm SuperCCD, these two values should usually differ by a factor of 2.0.
            ImageDefaultCropOrigin = 0xc61f, // Raw images often store extra pixels around the edges of the final Image These extra pixels help prevent interpolation artifacts near the edges of the final Image DefaultCropOrigin specifies the origin of the final image area, in raw image coordinates (i.e., before the DefaultScale has been applied), relative to the top-left corner of the ActiveArea rectangle.
            ImageDefaultCropSize = 0xc620, // Raw images often store extra pixels around the edges of the final Image These extra pixels help prevent interpolation artifacts near the edges of the final Image DefaultCropSize specifies the size of the final image area, in raw image coordinates (i.e., before the DefaultScale has been applied).
            ImageColorMatrix1 = 0xc621, // ColorMatrix1 defines a transformation matrix that converts XYZ values to reference camera native color space values, under the first calibration illuminant. The matrix values are stored in row scan order. The ColorMatrix1 tag is required for all non-monochrome DNG files.
            ImageColorMatrix2 = 0xc622, // ColorMatrix2 defines a transformation matrix that converts XYZ values to reference camera native color space values, under the second calibration illuminant. The matrix values are stored in row scan order.
            ImageCameraCalibration1 = 0xc623, // CameraClalibration1 defines a calibration matrix that transforms reference camera native space values to individual camera native space values under the first calibration illuminant. The matrix is stored in row scan order. This matrix is stored separately from the matrix specified by the ColorMatrix1 tag to allow raw converters to swap in replacement color matrices based on UniqueCameraModel tag, while still taking advantage of any per-individual camera calibration performed by the camera manufacturer.
            ImageCameraCalibration2 = 0xc624, // CameraCalibration2 defines a calibration matrix that transforms reference camera native space values to individual camera native space values under the second calibration illuminant. The matrix is stored in row scan order. This matrix is stored separately from the matrix specified by the ColorMatrix2 tag to allow raw converters to swap in replacement color matrices based on UniqueCameraModel tag, while still taking advantage of any per-individual camera calibration performed by the camera manufacturer.
            ImageReductionMatrix1 = 0xc625, // ReductionMatrix1 defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values, under the first calibration illuminant. This tag may only be used if ColorPlanes is greater than 3. The matrix is stored in row scan order.
            ImageReductionMatrix2 = 0xc626, // ReductionMatrix2 defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values, under the second calibration illuminant. This tag may only be used if ColorPlanes is greater than 3. The matrix is stored in row scan order.
            ImageAnalogBalance = 0xc627, // Normally the stored raw values are not white balanced, since any digital white balancing will reduce the dynamic range of the final image if the user decides to later adjust the white balance; however, if camera hardware is capable of white balancing the color channels before the signal is digitized, it can improve the dynamic range of the final Image AnalogBalance defines the gain, either analog (recommended) or digital (not recommended) that has been applied the stored raw values.
            ImageAsShotNeutral = 0xc628, // Specifies the selected white balance at time of capture, encoded as the coordinates of a perfectly neutral color in linear reference space values. The inclusion of this tag precludes the inclusion of the AsShotWhiteXY tag.
            ImageAsShotWhiteXY = 0xc629, // Specifies the selected white balance at time of capture, encoded as x-y chromaticity coordinates. The inclusion of this tag precludes the inclusion of the AsShotNeutral tag.
            ImageBaselineExposure = 0xc62a, // Camera models vary in the trade-off they make between highlight headroom and shadow noise. Some leave a significant amount of highlight headroom during a normal exposure. This allows significant negative exposure compensation to be applied during raw conversion, but also means normal exposures will contain more shadow noise. Other models leave less headroom during normal exposures. This allows for less negative exposure compensation, but results in lower shadow noise for normal exposures. Because of these differences, a raw converter needs to vary the zero point of its exposure compensation control from model to model. BaselineExposure specifies by how much (in EV units) to move the zero point. Positive values result in brighter default results, while negative values result in darker default results.
            ImageBaselineNoise = 0xc62b, // Specifies the relative noise level of the camera model at a baseline ISO value of 100, compared to a reference camera model. Since noise levels tend to vary approximately with the square root of the ISO value, a raw converter can use this value, combined with the current ISO, to estimate the relative noise level of the current Image
            ImageBaselineSharpness = 0xc62c, // Specifies the relative amount of sharpening required for this camera model, compared to a reference camera model. Camera models vary in the strengths of their anti-aliasing filters. Cameras with weak or no filters require less sharpening than cameras with strong anti-aliasing filters.
            ImageBayerGreenSplit = 0xc62d, // Only applies to CFA images using a Bayer pattern filter array. This tag specifies, in arbitrary units, how closely the values of the green pixels in the blue/green rows track the values of the green pixels in the red/green rows. A value of zero means the two kinds of green pixels track closely, while a non-zero value means they sometimes diverge. The useful range for this tag is from 0 (no divergence) to about 5000 (quite large divergence).
            ImageLinearResponseLimit = 0xc62e, // Some sensors have an unpredictable non-linearity in their response as they near the upper limit of their encoding range. This non-linearity results in color shifts in the highlight areas of the resulting image unless the raw converter compensates for this effect. LinearResponseLimit specifies the fraction of the encoding range above which the response may become significantly non-linear.
            ImageCameraSerialNumber = 0xc62f, // CameraSerialNumber contains the serial number of the camera or camera body that captured the Image
            ImageLensInfo = 0xc630, // Contains information about the lens that captured the Image If the minimum f-stops are unknown, they should be encoded as 0/0.
            ImageChromaBlurRadius = 0xc631, // ChromaBlurRadius provides a hint to the DNG reader about how much chroma blur should be applied to the Image If this tag is omitted, the reader will use its default amount of chroma blurring. Normally this tag is only included for non-CFA images, since the amount of chroma blur required for mosaic images is highly dependent on the de-mosaic algorithm, in which case the DNG reader's default value is likely optimized for its particular de-mosaic algorithm.
            ImageAntiAliasStrength = 0xc632, // Provides a hint to the DNG reader about how strong the camera's anti-alias filter is. A value of 0.0 means no anti-alias filter (i.e., the camera is prone to aliasing artifacts with some subjects), while a value of 1.0 means a strong anti-alias filter (i.e., the camera almost never has aliasing artifacts).
            ImageShadowScale = 0xc633, // This tag is used by Adobe Camera Raw to control the sensitivity of its 'Shadows' slider.
            ImageDNGPrivateData = 0xc634, // Provides a way for camera manufacturers to store private data in the DNG file for use by their own raw converters, and to have that data preserved by programs that edit DNG files.
            ImageMakerNoteSafety = 0xc635, // MakerNoteSafety lets the DNG reader know whether the EXIF MakerNote tag is safe to preserve along with the rest of the EXIF data. File browsers and other image management software processing an image with a preserved MakerNote should be aware that any thumbnail image embedded in the MakerNote may be stale, and may not reflect the current state of the full size Image
            ImageCalibrationIlluminant1 = 0xc65a, // The illuminant used for the first set of color calibration tags (ColorMatrix1, CameraCalibration1, ReductionMatrix1). The legal values for this tag are the same as the legal values for the LightSource EXIF tag.
            ImageCalibrationIlluminant2 = 0xc65b, // The illuminant used for an optional second set of color calibration tags (ColorMatrix2, CameraCalibration2, ReductionMatrix2). The legal values for this tag are the same as the legal values for the CalibrationIlluminant1 tag; however, if both are included, neither is allowed to have a value of 0 (unknown).
            ImageBestQualityScale = 0xc65c, // For some cameras, the best possible image quality is not achieved by preserving the total pixel count during conversion. For example, Fujifilm SuperCCD images have maximum detail when their total pixel count is doubled. This tag specifies the amount by which the values of the DefaultScale tag need to be multiplied to achieve the best quality image size.
            ImageRawDataUniqueID = 0xc65d, // This tag contains a 16-byte unique identifier for the raw image data in the DNG file. DNG readers can use this tag to recognize a particular raw image, even if the file's name or the metadata contained in the file has been changed. If a DNG writer creates such an identifier, it should do so using an algorithm that will ensure that it is very unlikely two different images will end up having the same identifier.
            ImageOriginalRawFileName = 0xc68b, // If the DNG file was converted from a non-DNG raw file, then this tag contains the file name of that original raw file.
            ImageOriginalRawFileData = 0xc68c, // If the DNG file was converted from a non-DNG raw file, then this tag contains the compressed contents of that original raw file. The contents of this tag always use the big-endian byte order. The tag contains a sequence of data blocks. Future versions of the DNG specification may define additional data blocks, so DNG readers should ignore extra bytes when parsing this tag. DNG readers should also detect the case where data blocks are missing from the end of the sequence, and should assume a default value for all the missing blocks. There are no padding or alignment bytes between data blocks.
            ImageActiveArea = 0xc68d, // This rectangle defines the active (non-masked) pixels of the sensor. The order of the rectangle coordinates is: top, left, bottom, right.
            ImageMaskedAreas = 0xc68e, // This tag contains a list of non-overlapping rectangle coordinates of fully masked pixels, which can be optionally used by DNG readers to measure the black encoding level. The order of each rectangle's coordinates is: top, left, bottom, right. If the raw image data has already had its black encoding level subtracted, then this tag should not be used, since the masked pixels are no longer useful.
            ImageAsShotICCProfile = 0xc68f, // This tag contains an ICC profile that, in conjunction with the AsShotPreProfileMatrix tag, provides the camera manufacturer with a way to specify a default color rendering from camera color space coordinates (linear reference values) into the ICC profile connection space. The ICC profile connection space is an output referred colorimetric space, whereas the other color calibration tags in DNG specify a conversion into a scene referred colorimetric space. This means that the rendering in this profile should include any desired tone and gamut mapping needed to convert between scene referred values and output referred values.
            ImageAsShotPreProfileMatrix = 0xc690, // This tag is used in conjunction with the AsShotICCProfile tag. It specifies a matrix that should be applied to the camera color space coordinates before processing the values through the ICC profile specified in the AsShotICCProfile tag. The matrix is stored in the row scan order. If ColorPlanes is greater than three, then this matrix can (but is not required to) reduce the dimensionality of the color data down to three components, in which case the AsShotICCProfile should have three rather than ColorPlanes input components.
            ImageCurrentICCProfile = 0xc691, // This tag is used in conjunction with the CurrentPreProfileMatrix tag. The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage as the AsShotICCProfile and AsShotPreProfileMatrix tag pair, except they are for use by raw file editors rather than camera manufacturers.
            ImageCurrentPreProfileMatrix = 0xc692, // This tag is used in conjunction with the CurrentICCProfile tag. The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage as the AsShotICCProfile and AsShotPreProfileMatrix tag pair, except they are for use by raw file editors rather than camera manufacturers.
            ImageColorimetricReference = 0xc6bf, // The DNG color model documents a transform between camera colors and CIE XYZ values. This tag describes the colorimetric reference for the CIE XYZ values. 0 = The XYZ values are scene-referred. 1 = The XYZ values are output-referred, using the ICC profile perceptual dynamic range. This tag allows output-referred data to be stored in DNG files and still processed correctly by DNG readers.
            ImageCameraCalibrationSignature = 0xc6f3, // A UTF-8 encoded string associated with the CameraCalibration1 and CameraCalibration2 tags. The CameraCalibration1 and CameraCalibration2 tags should only be used in the DNG color transform if the string stored in the CameraCalibrationSignature tag exactly matches the string stored in the ProfileCalibrationSignature tag for the selected camera profile.
            ImageProfileCalibrationSignature = 0xc6f4, // A UTF-8 encoded string associated with the camera profile tags. The CameraCalibration1 and CameraCalibration2 tags should only be used in the DNG color transfer if the string stored in the CameraCalibrationSignature tag exactly matches the string stored in the ProfileCalibrationSignature tag for the selected camera profile.
            ImageAsShotProfileName = 0xc6f6, // A UTF-8 encoded string containing the name of the "as shot" camera profile, if any.
            ImageNoiseReductionApplied = 0xc6f7, // This tag indicates how much noise reduction has been applied to the raw data on a scale of 0.0 to 1.0. A 0.0 value indicates that no noise reduction has been applied. A 1.0 value indicates that the "ideal" amount of noise reduction has been applied, i.e. that the DNG reader should not apply additional noise reduction by default. A value of 0/0 indicates that this parameter is unknown.
            ImageProfileName = 0xc6f8, // A UTF-8 encoded string containing the name of the camera profile. This tag is optional if there is only a single camera profile stored in the file but is required for all camera profiles if there is more than one camera profile stored in the file.
            ImageProfileHueSatMapDims = 0xc6f9, // This tag specifies the number of input samples in each dimension of the hue/saturation/value mapping tables. The data for these tables are stored in ProfileHueSatMapData1 and ProfileHueSatMapData2 tags. The most common case has ValueDivisions equal to 1, so only hue and saturation are used as inputs to the mapping table.
            ImageProfileHueSatMapData1 = 0xc6fa, // This tag contains the data for the first hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees; the second entry is saturation scale factor; and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
            ImageProfileHueSatMapData2 = 0xc6fb, // This tag contains the data for the second hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees; the second entry is a saturation scale factor; and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
            ImageProfileToneCurve = 0xc6fc, // This tag contains a default tone curve that can be applied while processing the image as a starting point for user adjustments. The curve is specified as a list of 32-bit IEEE floating-point value pairs in linear gamma. Each sample has an input value in the range of 0.0 to 1.0, and an output value in the range of 0.0 to 1.0. The first sample is required to be (0.0, 0.0), and the last sample is required to be (1.0, 1.0). Interpolated the curve using a cubic spline.
            ImageProfileEmbedPolicy = 0xc6fd, // This tag contains information about the usage rules for the associated camera profile.
            ImageProfileCopyright = 0xc6fe, // A UTF-8 encoded string containing the copyright information for the camera profile. This string always should be preserved along with the other camera profile tags.
            ImageForwardMatrix1 = 0xc714, // This tag defines a matrix that maps white balanced camera colors to XYZ D50 colors.
            ImageForwardMatrix2 = 0xc715, // This tag defines a matrix that maps white balanced camera colors to XYZ D50 colors.
            ImagePreviewApplicationName = 0xc716, // A UTF-8 encoded string containing the name of the application that created the preview stored in the IFD.
            ImagePreviewApplicationVersion = 0xc717, // A UTF-8 encoded string containing the version number of the application that created the preview stored in the IFD.
            ImagePreviewSettingsName = 0xc718, // A UTF-8 encoded string containing the name of the conversion settings (for example, snapshot name) used for the preview stored in the IFD.
            ImagePreviewSettingsDigest = 0xc719, // A unique ID of the conversion settings (for example, MD5 digest) used to render the preview stored in the IFD.
            ImagePreviewColorSpace = 0xc71a, // This tag specifies the color space in which the rendered preview in this IFD is stored. The default value for this tag is sRGB for color previews and Gray Gamma 2.2 for monochrome previews.
            ImagePreviewDateTime = 0xc71b, // This tag is an ASCII string containing the name of the date/time at which the preview stored in the IFD was rendered. The date/time is encoded using ISO 8601 format.
            ImageRawImageDigest = 0xc71c, // This tag is an MD5 digest of the raw image data. All pixels in the image are processed in row-scan order. Each pixel is zero padded to 16 or 32 bits deep (16-bit for data less than or equal to 16 bits deep, 32-bit otherwise). The data for each pixel is processed in little-endian byte order.
            ImageOriginalRawFileDigest = 0xc71d, // This tag is an MD5 digest of the data stored in the OriginalRawFileData tag.
            ImageSubTileBlockSize = 0xc71e, // Normally, the pixels within a tile are stored in simple row-scan order. This tag specifies that the pixels within a tile should be grouped first into rectangular blocks of the specified size. These blocks are stored in row-scan order. Within each block, the pixels are stored in row-scan order. The use of a non-default value for this tag requires setting the DNGBackwardVersion tag to at least 1.2.0.0.
            ImageRowInterleaveFactor = 0xc71f, // This tag specifies that rows of the image are stored in interleaved order. The value of the tag specifies the number of interleaved fields. The use of a non-default value for this tag requires setting the DNGBackwardVersion tag to at least 1.2.0.0.
            ImageProfileLookTableDims = 0xc725, // This tag specifies the number of input samples in each dimension of a default "look" table. The data for this table is stored in the ProfileLookTableData tag.
            ImageProfileLookTableData = 0xc726, // This tag contains a default "look" table that can be applied while processing the image as a starting point for user adjustment. This table uses the same format as the tables stored in the ProfileHueSatMapData1 and ProfileHueSatMapData2 tags, and is applied in the same color space. However, it should be applied later in the processing pipe, after any exposure compensation and/or fill light stages, but before any tone curve stage. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees, the second entry is a saturation scale factor, and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
            ImageOpcodeList1 = 0xc740, // Specifies the list of opcodes that should be applied to the raw image, as read directly from the file.
            ImageOpcodeList2 = 0xc741, // Specifies the list of opcodes that should be applied to the raw image, just after it has been mapped to linear reference values.
            ImageOpcodeList3 = 0xc74e, // Specifies the list of opcodes that should be applied to the raw image, just after it has been demosaiced.
            ImageNoiseProfile = 0xc761, // NoiseProfile describes the amount of noise in a raw Image Specifically, this tag models the amount of signal-dependent photon (shot) noise and signal-independent sensor readout noise, two common sources of noise in raw images. The model assumes that the noise is white and spatially independent, ignoring fixed pattern effects and other sources of noise (e.g., pixel response non-uniformity, spatially-dependent thermal effects, etc.).
            PhotoExposureTime = 0x829a, // Exposure time, given in seconds (sec).
            PhotoFNumber = 0x829d, // The F number.
            PhotoExposureProgram = 0x8822, // The class of the program used by the camera to set exposure when the picture is taken.
            PhotoSpectralSensitivity = 0x8824, // Indicates the spectral sensitivity of each channel of the camera used. The tag value is an ASCII string compatible with the standard developed by the ASTM Technical Committee.
            PhotoISOSpeedRatings = 0x8827, // Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.
            PhotoOECF = 0x8828, // Indicates the Opto-Electoric Conversion Function (OECF) specified in ISO 14524. <OECF> is the relationship between the camera optical input and the image values.
            PhotoSensitivityType = 0x8830, // The SensitivityType tag indicates PhotographicSensitivity tag. which one of the parameters of ISO12232 is the Although it is an optional tag, it should be recorded when a PhotographicSensitivity tag is recorded. Value = 4, 5, 6, or 7 may be used in case that the values of plural parameters are the same.
            PhotoStandardOutputSensitivity = 0x8831, // This tag indicates the standard output sensitivity value of a camera or input device defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.
            PhotoRecommendedExposureIndex = 0x8832, // This tag indicates the recommended exposure index value of a camera or input device defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.
            PhotoISOSpeed = 0x8833, // This tag indicates the ISO speed value of a camera or input device that is defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.
            PhotoISOSpeedLatitudeyyy = 0x8834, // This tag indicates the ISO speed latitude yyy value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded without ISOSpeed and ISOSpeedLatitudezzz.
            PhotoISOSpeedLatitudezzz = 0x8835, // This tag indicates the ISO speed latitude zzz value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded without ISOSpeed and ISOSpeedLatitudeyyy.
            PhotoExifVersion = 0x9000, // The version of this standard supported. Nonexistence of this field is taken to mean nonconformance to the standard.
            PhotoDateTimeOriginal = 0x9003, // The date and time when the original image data was generated. For a digital still camera the date and time the picture was taken are recorded.
            PhotoDateTimeDigitized = 0x9004, // The date and time when the image was stored as digital data.
            PhotoComponentsConfiguration = 0x9101, // Information specific to compressed data. The channels of each component are arranged in order from the 1st component to the 4th. For uncompressed data the data arrangement is given in the <PhotometricInterpretation> tag. However, since <PhotometricInterpretation> can only express the order of Y, Cb and Cr, this tag is provided for cases when compressed data uses components other than Y, Cb, and Cr and to enable support of other sequences.
            PhotoCompressedBitsPerPixel = 0x9102, // Information specific to compressed data. The compression mode used for a compressed image is indicated in unit bits per pixel.
            PhotoShutterSpeedValue = 0x9201, // Shutter speed. The unit is the APEX (Additive System of Photographic Exposure) setting.
            PhotoApertureValue = 0x9202, // The lens aperture. The unit is the APEX value.
            PhotoBrightnessValue = 0x9203, // The value of brightness. The unit is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
            PhotoExposureBiasValue = 0x9204, // The exposure bias. The units is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
            PhotoMaxApertureValue = 0x9205, // The smallest F number of the lens. The unit is the APEX value. Ordinarily it is given in the range of 00.00 to 99.99, but it is not limited to this range.
            PhotoSubjectDistance = 0x9206, // The distance to the subject, given in meters.
            PhotoMeteringMode = 0x9207, // The metering mode.
            PhotoLightSource = 0x9208, // The kind of light source.
            PhotoFlash = 0x9209, // This tag is recorded when an image is taken using a strobe light (flash).
            PhotoFocalLength = 0x920a, // The actual focal length of the lens, in mm. Conversion is not made to the focal length of a 35 mm film camera.
            PhotoSubjectArea = 0x9214, // This tag indicates the location and area of the main subject in the overall scene.
            PhotoMakerNote = 0x927c, // A tag for manufacturers of Exif writers to record any desired information. The contents are up to the manufacturer.
            PhotoUserComment = 0x9286, // A tag for Exif users to write keywords or comments on the image besides those in <ImageDescription>, and without the character code limitations of the <ImageDescription> tag.
            PhotoSubSecTime = 0x9290, // A tag used to record fractions of seconds for the <DateTime> tag.
            PhotoSubSecTimeOriginal = 0x9291, // A tag used to record fractions of seconds for the <DateTimeOriginal> tag.
            PhotoSubSecTimeDigitized = 0x9292, // A tag used to record fractions of seconds for the <DateTimeDigitized> tag.
            PhotoFlashpixVersion = 0xa000, // The FlashPix format version supported by a FPXR file.
            PhotoColorSpace = 0xa001, // The color space information tag is always recorded as the color space specifier. Normally sRGB is used to define the color space based on the PC monitor conditions and environment. If a color space other than sRGB is used, Uncalibrated is set. Image data recorded as Uncalibrated can be treated as sRGB when it is converted to FlashPix.
            PhotoPixelXDimension = 0xa002, // Information specific to compressed data. When a compressed file is recorded, the valid width of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file.
            PhotoPixelYDimension = 0xa003, // Information specific to compressed data. When a compressed file is recorded, the valid height of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file. Since data padding is unnecessary in the vertical direction, the number of lines recorded in this valid image height tag will in fact be the same as that recorded in the SOF.
            PhotoRelatedSoundFile = 0xa004, // This tag is used to record the name of an audio file related to the image data. The only relational information recorded here is the Exif audio file name and extension (an ASCII string consisting of 8 characters + '.' + 3 characters). The path is not recorded.
            PhotoInteroperabilityTag = 0xa005, // Interoperability IFD is composed of tags which stores the information to ensure the Interoperability and pointed by the following tag located in Exif IFD. The Interoperability structure of Interoperability IFD is the same as TIFF defined IFD structure but does not contain the image data characteristically compared with normal TIFF IFD.
            PhotoFlashEnergy = 0xa20b, // Indicates the strobe energy at the time the image is captured, as measured in Beam Candle Power Seconds (BCPS).
            PhotoSpatialFrequencyResponse = 0xa20c, // This tag records the camera or input device spatial frequency table and SFR values in the direction of image width, image height, and diagonal direction, as specified in ISO 12233.
            PhotoFocalPlaneXResolution = 0xa20e, // Indicates the number of pixels in the image width (X) direction per <FocalPlaneResolutionUnit> on the camera focal plane.
            PhotoFocalPlaneYResolution = 0xa20f, // Indicates the number of pixels in the image height (V) direction per <FocalPlaneResolutionUnit> on the camera focal plane.
            PhotoFocalPlaneResolutionUnit = 0xa210, // Indicates the unit for measuring <FocalPlaneXResolution> and <FocalPlaneYResolution>. This value is the same as the <ResolutionUnit>.
            PhotoSubjectLocation = 0xa214, // Indicates the location of the main subject in the scene. The value of this tag represents the pixel at the center of the main subject relative to the left edge, prior to rotation processing as per the <Rotation> tag. The first value indicates the X column number and second indicates the Y row number.
            PhotoExposureIndex = 0xa215, // Indicates the exposure index selected on the camera or input device at the time the image is captured.
            PhotoSensingMethod = 0xa217, // Indicates the image sensor type on the camera or input device.
            PhotoFileSource = 0xa300, // Indicates the image source. If a DSC recorded the image, this tag value of this tag always be set to 3, indicating that the image was recorded on a DSC.
            PhotoSceneType = 0xa301, // Indicates the type of scene. If a DSC recorded the image, this tag value must always be set to 1, indicating that the image was directly photographed.
            PhotoCFAPattern = 0xa302, // Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used. It does not apply to all sensing methods.
            PhotoCustomRendered = 0xa401, // This tag indicates the use of special processing on image data, such as rendering geared to output. When special processing is performed, the reader is expected to disable or minimize any further processing.
            PhotoExposureMode = 0xa402, // This tag indicates the exposure mode set when the image was shot. In auto-bracketing mode, the camera shoots a series of frames of the same scene at different exposure settings.
            PhotoWhiteBalance = 0xa403, // This tag indicates the white balance mode set when the image was shot.
            PhotoDigitalZoomRatio = 0xa404, // This tag indicates the digital zoom ratio when the image was shot. If the numerator of the recorded value is 0, this indicates that digital zoom was not used.
            PhotoFocalLengthIn35mmFilm = 0xa405, // This tag indicates the equivalent focal length assuming a 35mm film camera, in mm. A value of 0 means the focal length is unknown. Note that this tag differs from the <FocalLength> tag.
            PhotoSceneCaptureType = 0xa406, // This tag indicates the type of scene that was shot. It can also be used to record the mode in which the image was shot. Note that this differs from the <SceneType> tag.
            PhotoGainControl = 0xa407, // This tag indicates the degree of overall image gain adjustment.
            PhotoContrast = 0xa408, // This tag indicates the direction of contrast processing applied by the camera when the image was shot.
            PhotoSaturation = 0xa409, // This tag indicates the direction of saturation processing applied by the camera when the image was shot.
            PhotoSharpness = 0xa40a, // This tag indicates the direction of sharpness processing applied by the camera when the image was shot.
            PhotoDeviceSettingDescription = 0xa40b, // This tag indicates information on the picture-taking conditions of a particular camera model. The tag is used only to indicate the picture-taking conditions in the reader.
            PhotoSubjectDistanceRange = 0xa40c, // This tag indicates the distance to the subject.
            PhotoImageUniqueID = 0xa420, // This tag indicates an identifier assigned uniquely to each Image It is recorded as an ASCII string equivalent to hexadecimal notation and 128-bit fixed length.
            PhotoCameraOwnerName = 0xa430, // This tag records the owner of a camera used in photography as an ASCII string.
            PhotoBodySerialNumber = 0xa431, // This tag records the serial number of the body of the camera that was used in photography as an ASCII string.
            PhotoLensSpecification = 0xa432, // This tag notes minimum focal length, maximum focal length, minimum F number in the minimum focal length, and minimum F number in the maximum focal length, which are specification information for the lens that was used in photography. When the minimum F number is unknown, the notation is 0/0
            PhotoLensMake = 0xa433, // This tag records the lens manufactor as an ASCII string.
            PhotoLensModel = 0xa434, // This tag records the lens's model name and model number as an ASCII string.
            PhotoLensSerialNumber = 0xa435, // This tag records the serial number of the interchangeable lens that was used in photography as an ASCII string.
            IopInteroperabilityIndex = 0x0001, // Indicates the identification of the Interoperability rule. Use "R98" for stating ExifR98 Rules. Four bytes used including the termination code (NULL). see the separate volume of Recommended Exif Interoperability Rules (ExifR98) for other tags used for ExifR98.
            IopInteroperabilityVersion = 0x0002, // Interoperability version
            IopRelatedImageFileFormat = 0x1000, // File format of image file
            IopRelatedImageWidth = 0x1001, // Image width
            IopRelatedImageLength = 0x1002, // Image height
            GPSVersionID = 0x0000, // Indicates the version of <GPSInfoIFD>. The version is given as 2.0.0.0. This tag is mandatory when <GPSInfo> tag is present. (Note: The <GPSVersionID> tag is given in bytes, unlike the <ExifVersion> tag. When the version is 2.0.0.0, the tag value is 02000000.H).
            GPSLatitudeRef = 0x0001, // Indicates whether the latitude is north or south latitude. The ASCII value 'N' indicates north latitude, and 'S' is south latitude.
            GPSLatitude = 0x0002, // Indicates the latitude. The latitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. When degrees, minutes and seconds are expressed, the format is dd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is dd/1,mmmm/100,0/1.
            GPSLongitudeRef = 0x0003, // Indicates whether the longitude is east or west longitude. ASCII 'E' indicates east longitude, and 'W' is west longitude.
            GPSLongitude = 0x0004, // Indicates the longitude. The longitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. When degrees, minutes and seconds are expressed, the format is ddd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is ddd/1,mmmm/100,0/1.
            GPSAltitudeRef = 0x0005, // Indicates the altitude used as the reference altitude. If the reference is sea level and the altitude is above sea level, 0 is given. If the altitude is below sea level, a value of 1 is given and the altitude is indicated as an absolute value in the GSPAltitude tag. The reference unit is meters. Note that this tag is BYTE type, unlike other reference tags.
            GPSAltitude = 0x0006, // Indicates the altitude based on the reference in GPSAltitudeRef. Altitude is expressed as one RATIONAL value. The reference unit is meters.
            GPSTimeStamp = 0x0007, // Indicates the time as UTC (Coordinated Universal Time). <TimeStamp> is expressed as three RATIONAL values giving the hour, minute, and second (atomic clock).
            GPSSatellites = 0x0008, // Indicates the GPS satellites used for measurements. This tag can be used to describe the number of satellites, their ID number, angle of elevation, azimuth, SNR and other information in ASCII notation. The format is not specified. If the GPS receiver is incapable of taking measurements, value of the tag is set to NULL.
            GPSStatus = 0x0009, // Indicates the status of the GPS receiver when the image is recorded. "A" means measurement is in progress, and "V" means the measurement is Interoperability.
            GPSMeasureMode = 0x000a, // Indicates the GPS measurement mode. "2" means two-dimensional measurement and "3" means three-dimensional measurement is in progress.
            GPSDOP = 0x000b, // Indicates the GPS DOP (data degree of precision). An HDOP value is written during two-dimensional measurement, and PDOP during three-dimensional measurement.
            GPSSpeedRef = 0x000c, // Indicates the unit used to express the GPS receiver speed of movement. "K" "M" and "N" represents kilometers per hour, miles per hour, and knots.
            GPSSpeed = 0x000d, // Indicates the speed of GPS receiver movement.
            GPSTrackRef = 0x000e, // Indicates the reference for giving the direction of GPS receiver movement. "T" denotes true direction and "M" is magnetic direction.
            GPSTrack = 0x000f, // Indicates the direction of GPS receiver movement. The range of values is from 0.00 to 359.99.
            GPSImgDirectionRef = 0x0010, // Indicates the reference for giving the direction of the image when it is captured. "T" denotes true direction and "M" is magnetic direction.
            GPSImgDirection = 0x0011, // Indicates the direction of the image when it was captured. The range of values is from 0.00 to 359.99.
            GPSMapDatum = 0x0012, // Indicates the geodetic survey data used by the GPS receiver. If the survey data is restricted to Japan, the value of this tag is "TOKYO" or "WGS-84".
            GPSDestLatitudeRef = 0x0013, // Indicates whether the latitude of the destination point is north or south latitude. The ASCII value "N" indicates north latitude, and "S" is south latitude.
            GPSDestLatitude = 0x0014, // Indicates the latitude of the destination point. The latitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. If latitude is expressed as degrees, minutes and seconds, a typical format would be dd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format would be dd/1,mmmm/100,0/1.
            GPSDestLongitudeRef = 0x0015, // Indicates whether the longitude of the destination point is east or west longitude. ASCII "E" indicates east longitude, and "W" is west longitude.
            GPSDestLongitude = 0x0016, // Indicates the longitude of the destination point. The longitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. If longitude is expressed as degrees, minutes and seconds, a typical format would be ddd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format would be ddd/1,mmmm/100,0/1.
            GPSDestBearingRef = 0x0017, // Indicates the reference used for giving the bearing to the destination point. "T" denotes true direction and "M" is magnetic direction.
            GPSDestBearing = 0x0018, // Indicates the bearing to the destination point. The range of values is from 0.00 to 359.99.
            GPSDestDistanceRef = 0x0019, // Indicates the unit used to express the distance to the destination point. "K", "M" and "N" represent kilometers, miles and knots.
            GPSDestDistance = 0x001a, // Indicates the distance to the destination point.
            GPSProcessingMethod = 0x001b, // A character string recording the name of the method used for location finding. The first byte indicates the character code used, and this is followed by the name of the method.
            GPSAreaInformation = 0x001c, // A character string recording the name of the GPS area. The first byte indicates the character code used, and this is followed by the name of the GPS area.
            GPSDateStamp = 0x001d, // A character string recording date and time information relative to UTC (Coordinated Universal Time). The format is "YYYY:MM:DD.".
            GPSDifferential = 0x001e, // Indicates whether differential correction is applied to the GPS receiver.

            // Missing
            ThumbnailImageHeight = 0x5021,

            ThumbnailImageWidth = 0x5020,
            ThumbnailBitsPerSample = 0x5022,
            ThumbnailCompression = 0x5023,
            ThumbnailPhotometricInterpretation = 0x5024,
            ThumbnailImageDescription = 0x5025,
            ThumbnailMake = 0x5026,
            ThumbnailModel = 0x5027,
            ThumbnailStripOffsets = 0x5028,
            ThumbnailOrientation = 0x5029,
            ThumbnailSamplesPerPixel = 0x502A,
            ThumbnailRowsPerStrip = 0x502B,
            ThumbnailStripBytesCount = 0x502C,
            ThumbnailXResolution = 0x502D,
            ThumbnailYResolution = 0x502E,
            ThumbnailPlanarConfig = 0x502F,
            ThumbnailResolutionUnit = 0x5030,
            ThumbnailTransferFunction = 0x5031,
            ThumbnailSoftware = 0x5032,
            ThumbnailDateTime = 0x5033,
            ThumbnailArtist = 0x5034,
            ThumbnailWhitePoint = 0x5035,
            ThumbnailPrimaryChromaticities = 0x5036,
            ThumbnailYCbCrCoefficients = 0x5037,
            ThumbnailYCbCrSubSampling = 0x5038,
            ThumbnailYCbCrPositioning = 0x5039,
            ThumbnailReferenceBlackWhite = 0x503A,
            ThumbnailCopyright = 0x503B,
            ThumbnailColorDepth = 0x5015,
            ThumbnailCompressedSize = 0x5019,
            ThumbnailData = 0x501B,
            ThumbnailFormat = 0x5012,
            ThumbnailHeight = 0x5014,
            ThumbnailPlanes = 0x5016,
            ThumbnailRawBytes = 0x5017,
            Thumbnail = 0x5018,
            ThumbnailWidth = 0x5013,
            JpegQuality = 0x5010,
            JpegRestartInterval = 0x0203,
            LoopCount = 0x5101,
            LuminanceTable = 0x5090,
            PaletteHistogram = 0x5113,
            PixelPerUnitX = 0x5111,
            PixelPerUnitY = 0x5112,
            PixelUnit = 0x5110,
            PrintFlags = 0x5005,
            PrintFlagsBleedWidth = 0x5008,
            PrintFlagsBleedWidthScale = 0x5009,
            PrintFlagsCrop = 0x5007,
            PrintFlagsVersion = 0x5006,
            ResolutionXLengthUnit = 0x5003,
            ResolutionXUnit = 0x5001,
            ResolutionYLengthUnit = 0x5004,
            ResolutionYUnit = 0x5002,
            InteroperabilityIndex = 0x5041,
            ChrominanceTable = 0x5091,
            ColorTransferFunction = 0x501A,
            GridSize = 0x5011,
            HalftoneDegree = 0x500C,
            HalftoneLPI = 0x500A,
            HalftoneLPIUnit = 0x500B,
            HalftoneMisc = 0x500E,
            HalftoneScreen = 0x500F,
            HalftoneShape = 0x500D,
        }
    }
}