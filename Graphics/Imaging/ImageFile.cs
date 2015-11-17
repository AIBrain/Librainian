// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/ImageFile.cs" was last cleaned by Rick on 2015/10/04 at 11:18 AM

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Untested.
    ///     Pulled from http://www.dreamincode.net/forums/topic/286802-detect-partially-corrupted-image/
    /// </summary>
    internal class ImageFile {

        public enum Types {

            FileNotFound,

            FileEmpty,

            FileNull,

            FileTooLarge,

            FileUnrecognized,

            PNG,

            JPG,

            GIFa,

            GIFb

        }

        private readonly Byte[] _abEndGIF = {0, 59};

        private readonly Byte[] _abEndJPGa = {255, 217, 255, 255};

        private readonly Byte[] _abEndJPGb = {255, 217};

        private readonly Byte[] _abEndPNG = {73, 69, 78, 68, 174, 66, 96, 130};

        private readonly Byte[] _abTagGIFa = {71, 73, 70, 56, 55, 97};

        private readonly Byte[] _abTagGIFb = {71, 73, 70, 56, 57, 97};

        private readonly Byte[] _abTagJPG = {255, 216, 255};

        private readonly Byte[] _abTagPNG = {137, 80, 78, 71, 13, 10, 26, 10};

        public ImageFile( String filename, Boolean cullEndingNullBytes = true, Int32 maxFileSize = Int32.MaxValue ) {
            this.Filename = filename.Trim();
            var fliTmp = new FileInfo( this.Filename );

            // check file exists...
            if ( File.Exists( this.Filename ) ) {
                this.FileType = Types.FileUnrecognized; // default if found

                // check file has data...
                if ( fliTmp.Length == 0 ) {
                    this.FileType = Types.FileEmpty;
                }
                else {
                    // check file isn't like stupid crazy big
                    if ( fliTmp.Length > maxFileSize ) {
                        this.FileType = Types.FileTooLarge;
                    }
                    else {
                        // load the whole file
                        var abtTmp = File.ReadAllBytes( this.Filename );

                        // check the length of actual data
                        var iLength = abtTmp.Length;
                        if ( abtTmp[ abtTmp.Length - 1 ] == 0 ) {
                            for ( var i = abtTmp.Length - 1; i > -1; i-- ) {
                                if ( abtTmp[ i ] != 0 ) {
                                    iLength = i;
                                    break;
                                }
                            }
                        }

                        // check that there is actual data
                        if ( iLength == 0 ) {
                            this.FileType = Types.FileNull;
                        }
                        else {
                            this.EndingNullBytes = abtTmp.Length - iLength;

                            // resize the data so we can work with it
                            Array.Resize( ref abtTmp, iLength );

                            // get the file type
                            if ( _StartsWith( abtTmp, this._abTagPNG ) ) {
                                this.FileType = Types.PNG;
                            }
                            else if ( _StartsWith( abtTmp, this._abTagJPG ) ) {
                                this.FileType = Types.JPG;
                            }
                            else if ( _StartsWith( abtTmp, this._abTagGIFa ) ) {
                                this.FileType = Types.GIFa;
                            }
                            else if ( _StartsWith( abtTmp, this._abTagGIFb ) ) {
                                this.FileType = Types.GIFb;
                            }

                            // check the file is complete
                            switch ( this.FileType ) {
                                case Types.PNG:
                                    this.IsComplete = _EndsWidth( abtTmp, this._abEndPNG );
                                    break;
                                case Types.JPG:
                                    this.IsComplete = _EndsWidth( abtTmp, this._abEndJPGa ) || _EndsWidth( abtTmp, this._abEndJPGb );
                                    break;
                                case Types.GIFa:
                                case Types.GIFb:
                                    this.IsComplete = _EndsWidth( abtTmp, this._abEndGIF );
                                    break;
                                case Types.FileNotFound:
                                    break;
                                case Types.FileEmpty:
                                    break;
                                case Types.FileNull:
                                    break;
                                case Types.FileTooLarge:
                                    break;
                                case Types.FileUnrecognized:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            // get rid of ending null bytes at caller's option
                            if ( this.IsComplete && cullEndingNullBytes ) {
                                File.WriteAllBytes( this.Filename, abtTmp );
                            }
                        }
                    }
                }
            }
        }

        public String Filename { get; }

        public Types FileType { get; } = Types.FileNotFound;

        public Boolean IsComplete { get; }

        public Int32 EndingNullBytes { get; }

        private static Boolean _StartsWith( IReadOnlyList< Byte > data, IReadOnlyCollection< Byte > search ) {
            var blRet = false;
            if ( search.Length() <= data.Length() ) {
                blRet = !search.Where( ( t, i ) => data[ i ] != t )
                               .Any();
            }
            return blRet; // RETURN
        }

        private static Boolean _EndsWidth( IReadOnlyList< Byte > data, IReadOnlyCollection< Byte > search ) {
            var blRet = false;
            if ( search.Length() <= data.Length() ) {
                var iStart = data.Length() - search.Length();
                blRet = !search.Where( ( t, i ) => data[ iStart + i ] != t )
                               .Any();
            }
            return blRet; // RETURN
        }

    }

}
