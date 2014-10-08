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
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Media.Imaging;
    using Annotations;
    using IO;

    public static class Images {

        [CanBeNull]
        public static DateTime? GetProperteryAsDateTime( [CanBeNull] this PropertyItem item ) {
            if ( null == item ) {
                return null;
            }

            var value = Encoding.ASCII.GetString( item.Value );
            if ( value.EndsWith( "\0" ) ) {
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

        public static DateTime ImageCreationBestGuess( [NotNull] this Document document ) {
            if ( document == null ) {
                throw new ArgumentNullException( "document" );
            }
            return ImageCreationBestGuess( new FileInfo( document.FullPathWithFileName) );
        }

        public static DateTime ImageCreationBestGuess( [CanBeNull] this FileSystemInfo info ) {
            if ( info == null ) {
                throw new ArgumentNullException( "info" );
            }
            var bestGuess = DateTime.Now;

            var fileCreation = File.GetCreationTime( info.FullName );
            if ( fileCreation <= bestGuess ) {
                bestGuess = fileCreation;
            }

            var lastWrite = File.GetLastWriteTime( info.FullName );
            if ( lastWrite <= bestGuess ) {
                bestGuess = lastWrite;
            }

            try {
                using ( var image = Image.FromFile( filename: info.FullName, useEmbeddedColorManagement: false ) ) {
                    if ( image.PropertyIdList.Contains( PropertyList.DateTimeDigitized ) ) {
                        //
                        var asDateTime = image.GetPropertyItem( PropertyList.DateTimeDigitized ).GetProperteryAsDateTime();
                        if ( asDateTime.HasValue && asDateTime.Value < bestGuess ) {
                            bestGuess = asDateTime.Value;
                        }
                    }

                    if ( image.PropertyIdList.Contains( PropertyList.DateTimeOriginal ) ) {
                        var asDateTime = image.GetPropertyItem( PropertyList.DateTimeOriginal ).GetProperteryAsDateTime();
                        if ( asDateTime.HasValue && asDateTime.Value < bestGuess ) {
                            bestGuess = asDateTime.Value;
                        }
                    }

                    if ( image.PropertyIdList.Contains( PropertyList.PropertyTagDateTime ) ) {
                        var asDateTime = image.GetPropertyItem( PropertyList.PropertyTagDateTime ).GetProperteryAsDateTime();
                        if ( asDateTime.HasValue && asDateTime.Value < bestGuess ) {
                            bestGuess = asDateTime.Value;
                        }
                    }
                }
            }
            catch ( Exception ) {
                /*swallow. .*/
            }
            return bestGuess;
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
                throw new ArgumentNullException( "document" );
            }
            return IsaValidImage( new FileInfo( document.FullPathWithFileName ) );
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
                var isImageGood = true; //HACK safe assumption?

                try {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.DecodeFailed += ( sender, args ) => { isImageGood = false; };
                    bitmapImage.DownloadFailed += ( sender, args ) => { isImageGood = false; };
                    bitmapImage.DownloadCompleted += ( sender, args ) => { isImageGood = true; };
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri( file.FullName );
                    bitmapImage.EndInit();
                    if ( bitmapImage.Width <= 0 ) {
                        return false;
                    }
                    if ( bitmapImage.Height <= 0 ) {
                        return false;
                    }
                }
                catch ( Exception ) {
                    return false;
                }

                if ( !isImageGood ) {
                    return false;
                }

                //just wondering.. nothing to *see* here.
                //try {
                //    var bob = new photo.exif.ExifItem();
                //    var exifItems = bob.( file.FullName );
                //    foreach ( var exifItem in exifItems ) {
                //        exifItem.
                //    }
                //}
                //catch ( exce ) {
                //    return false;
                //}

                using ( Image.FromFile( file.FullName ) ) {
                    return true;
                }
            }
            catch ( ExternalException ) { }
            catch ( InvalidOperationException ) { }
            catch ( FileNotFoundException ) { }
            catch ( NotSupportedException ) { }
            catch ( OutOfMemoryException ) { }
            catch ( Exception exception ) {
                exception.Error();
            }
            return false;
        }

        public static Image ResizeImage( [NotNull] this Image imgToResize, Size size ) {
            if ( imgToResize == null ) {
                throw new ArgumentNullException( "imgToResize" );
            }

            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            var nPercentW = ( size.Width / ( Single )sourceWidth );
            var nPercentH = ( size.Height / ( Single )sourceHeight );

            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destWidth = ( int )( sourceWidth * nPercent );
            var destHeight = ( int )( sourceHeight * nPercent );

            using ( var bitmap = new Bitmap( width: destWidth, height: destHeight ) ) {
                using ( var g = Graphics.FromImage( image: bitmap ) ) {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage( image: imgToResize, x: 0, y: 0, width: destWidth, height: destHeight );
                }
                return bitmap;
            }
        }

        public static class FileNameExtension {

            /// <summary>
            ///     <see cref="http://wikipedia.org/wiki/TIFF" />
            /// </summary>
            public static String Tiff { get { return ".tif"; } }
        }

        public static class PropertyList {
            public const int DateTimeDigitized = 36868;
            public const int DateTimeOriginal = 36867;
            public const int PropertyTagDateTime = 306;
        }
    }
}