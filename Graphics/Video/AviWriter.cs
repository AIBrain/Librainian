namespace Librainian.Graphics.Video {
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    /// <summary>Create AVI files from bitmaps</summary>
    public class AviWriter {

        private Int32 _aviFile;
        private IntPtr _aviStream = IntPtr.Zero;
        private UInt32 _frameRate;
        private Int32 _countFrames;
        private Int32 _width;
        private Int32 _height;
        private UInt32 _stride;
        private UInt32 _fccType = Avi.StreamtypeVideo; // vids
        private UInt32 _fccHandler = 1668707181; //"Microsoft Video 1" - Use CVID for default codec: (UInt32)Avi.mmioStringToFOURCC("CVID", 0);

        /// <summary>Creates a new AVI file</summary>
        /// <param name="fileName">Name of the new AVI file</param>
        /// <param name="frameRate">Frames per second</param>
        public void Open(String fileName, UInt32 frameRate) {
            this._frameRate = frameRate;

            Avi.AVIFileInit();

            var hr = Avi.AVIFileOpen(
                ref this._aviFile, fileName,
                4097 /* OF_WRITE | OF_CREATE (winbase.h) */, 0 );
            if ( hr != 0 ) {
                throw new Exception( "Error in AVIFileOpen: " + hr );
            }
        }

        /// <summary>Adds a new frame to the AVI stream</summary>
        /// <param name="bmp">The image to add</param>
        public void AddFrame(Bitmap bmp) {

            bmp.RotateFlip( RotateFlipType.RotateNoneFlipY );

            var bmpDat = bmp.LockBits(
                new Rectangle( 0, 0, bmp.Width, bmp.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            if ( this._countFrames == 0 ) {
                //this is the first frame - get size and create a new stream
                this._stride = ( UInt32 )bmpDat.Stride;
                this._width = bmp.Width;
                this._height = bmp.Height;
                this.CreateStream();
            }

            var result = Avi.AVIStreamWrite( this._aviStream,
                                             this._countFrames, 1,
                                             bmpDat.Scan0, //pointer to the beginning of the image data
                                             ( Int32 )( this._stride * this._height ),
                                             0, 0, 0 );

            if ( result != 0 ) {
                throw new Exception( "Error in AVIStreamWrite: " + result );
            }

            bmp.UnlockBits( bmpDat );
            this._countFrames++;
        }

        /// <summary>Closes stream, file and AVI Library</summary>
        public void Close() {
            if ( this._aviStream != IntPtr.Zero ) {
                Avi.AVIStreamRelease( this._aviStream );
                this._aviStream = IntPtr.Zero;
            }
            if ( this._aviFile != 0 ) {
                Avi.AVIFileRelease( this._aviFile );
                this._aviFile = 0;
            }
            Avi.AVIFileExit();
        }

        /// <summary>Creates a new video stream in the AVI file</summary>
        private void CreateStream() {
            var strhdr = new Avi.Avistreaminfo {
                                                   fccType = this._fccType,
                                                   fccHandler = this._fccHandler,
                                                   dwScale = 1,
                                                   dwRate = this._frameRate,
                                                   dwSuggestedBufferSize = ( UInt32 ) ( this._height*this._stride ),
                                                   dwQuality = 10000,
                                                   rcFrame = {
                                                                 bottom = ( UInt32 ) this._height,
                                                                 right = ( UInt32 ) this._width
                                                             },
                                                   szName = new UInt16[64]
                                               };
            //highest quality! Compression destroys the hidden message

            var result = Avi.AVIFileCreateStream( this._aviFile, out this._aviStream, ref strhdr );
            if ( result != 0 ) {
                throw new Exception( "Error in AVIFileCreateStream: " + result );
            }

            //define the image format

            var bi = new Avi.Bitmapinfoheader();
            bi.biSize = ( UInt32 )Marshal.SizeOf( bi );
            bi.biWidth = this._width;
            bi.biHeight = this._height;
            bi.biPlanes = 1;
            bi.biBitCount = 24;
            bi.biSizeImage = ( UInt32 )( this._stride * this._height );

            result = Avi.AVIStreamSetFormat( this._aviStream, 0, ref bi, Marshal.SizeOf( bi ) );
            if ( result != 0 ) {
                throw new Exception( "Error in AVIStreamSetFormat: " + result );
            }
        }
    }
}