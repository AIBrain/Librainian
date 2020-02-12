// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ImageFile.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "ImageFile.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    // ReSharper disable RedundantUsingDirective
    using File = OperatingSystem.FileSystem.Pri.LongPath.File;
    using FileInfo = OperatingSystem.FileSystem.Pri.LongPath.FileInfo;

    // ReSharper restore RedundantUsingDirective

    /// <summary>Untested. Pulled from http://www.dreamincode.net/forums/topic/286802-detect-partially-corrupted-image/</summary>
    public class ImageFile {

        private readonly Byte[] _abEndGIF = {
            0, 59
        };

        private readonly Byte[] _abEndJPGa = {
            255, 217, 255, 255
        };

        private readonly Byte[] _abEndJPGb = {
            255, 217
        };

        private readonly Byte[] _abEndPNG = {
            73, 69, 78, 68, 174, 66, 96, 130
        };

        private readonly Byte[] _abTagGIFa = {
            71, 73, 70, 56, 55, 97
        };

        private readonly Byte[] _abTagGIFb = {
            71, 73, 70, 56, 57, 97
        };

        private readonly Byte[] _abTagJPG = {
            255, 216, 255
        };

        private readonly Byte[] _abTagPNG = {
            137, 80, 78, 71, 13, 10, 26, 10
        };

        public Int32 EndingNullBytes { get; }

        public String Filename { get; }

        public Types FileType { get; } = Types.FileNotFound;

        public Boolean IsComplete { get; }

        public ImageFile( [NotNull] String filename, Boolean cullEndingNullBytes = true, Int32 maxFileSize = Int32.MaxValue ) {
            this.Filename = filename.Trim();
            var fliTmp = new FileInfo( this.Filename );

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
                                if ( abtTmp[ i ] == 0 ) {
                                    continue;
                                }

                                iLength = i;

                                break;
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

                                case Types.FileNotFound: break;

                                case Types.FileEmpty: break;

                                case Types.FileNull: break;

                                case Types.FileTooLarge: break;

                                case Types.FileUnrecognized: break;

                                default: throw new ArgumentOutOfRangeException();
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

        private static Boolean _EndsWidth( [NotNull] IReadOnlyList<Byte> data, [NotNull] IReadOnlyCollection<Byte> search ) {
            if ( data is null ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            if ( search is null ) {
                throw new ArgumentNullException( nameof( search ) );
            }

            var blRet = false;

            if ( search.Count <= data.Count ) {
                var iStart = data.Count - search.Count;
                blRet = !search.Where( ( t, i ) => data[ iStart + i ] != t ).Any();
            }

            return blRet; // RETURN
        }

        public static Boolean _StartsWith( [NotNull] IReadOnlyList<Byte> data, [NotNull] IReadOnlyList<Byte> search ) {
            if ( data is null ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            if ( search is null ) {
                throw new ArgumentNullException( nameof( search ) );
            }

            return search.Count <= data.Count && search.SequenceEqual( data.Take( search.Count ) );
        }
    }
}