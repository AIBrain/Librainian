// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AviReader.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Video {

    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using Magic;
    using OperatingSystem;

    /// <summary>
    /// Extract bitmaps from AVI files
    /// </summary>
    public class AviReader : ABetterClassDispose {

        //pointers
        private Int32 _aviFile;

        private IntPtr _aviStream;

        //position of the first frame, count of frames in the stream
        private Int32 _firstFrame;

        private Int32 _getFrameObject;

        //stream and header info
        private Avi.Avistreaminfo _streamInfo;

        public Size BitmapSize => new Size( ( Int32 )this._streamInfo.rcFrame.right, ( Int32 )this._streamInfo.rcFrame.bottom );

        public Int32 CountFrames {
            get; private set;
        }

        public UInt32 FrameRate => this._streamInfo.dwRate / this._streamInfo.dwScale;

        /// <summary>
        /// Closes all streams, files and libraries
        /// </summary>
        public void Close() {
            if ( this._getFrameObject != 0 ) {
                NativeMethods.AVIStreamGetFrameClose( this._getFrameObject );
                this._getFrameObject = 0;
            }
            if ( this._aviStream != IntPtr.Zero ) {
                NativeMethods.AVIStreamRelease( this._aviStream );
                this._aviStream = IntPtr.Zero;
            }
            if ( this._aviFile != 0 ) {
                NativeMethods.AVIFileRelease( this._aviFile );
                this._aviFile = 0;
            }
            NativeMethods.AVIFileExit();
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
        }

        /// <summary>
        /// Exports a frame into a bitmap file
        /// </summary>
        /// <param name="position">   Position of the frame</param>
        /// <param name="dstFileName">Name ofthe file to store the bitmap</param>
        public void ExportBitmap( Int32 position, String dstFileName ) {
            if ( position > this.CountFrames ) {
                throw new Exception( "Invalid frame position" );
            }

            //Decompress the frame and return a pointer to the DIB
            var pDib = NativeMethods.AVIStreamGetFrame( this._getFrameObject, this._firstFrame + position );

            //Copy the bitmap header into a managed struct
            var bih = new Avi.Bitmapinfoheader();
            bih = ( Avi.Bitmapinfoheader )Marshal.PtrToStructure( new IntPtr( pDib ), bih.GetType() );

            /*if(bih.biBitCount < 24){
				throw new Exception("Not enough colors! DIB color depth is less than 24 bit.");
			}else */
            if ( bih.biSizeImage < 1 ) {
                throw new Exception( "Exception in AVIStreamGetFrame: Not bitmap decompressed." );
            }

            //Copy the image
            var bitmapData = new Byte[bih.biSizeImage];
            var address = pDib + Marshal.SizeOf( bih );
            for ( var offset = 0; offset < bitmapData.Length; offset++ ) {
                bitmapData[offset] = Marshal.ReadByte( new IntPtr( address ) );
                address++;
            }

            //Copy bitmap info
            var bitmapInfo = new Byte[Marshal.SizeOf( bih )];
            var ptr = Marshal.AllocHGlobal( bitmapInfo.Length );
            Marshal.StructureToPtr( bih, ptr, false );
            address = ptr.ToInt32();
            for ( var offset = 0; offset < bitmapInfo.Length; offset++ ) {
                bitmapInfo[offset] = Marshal.ReadByte( new IntPtr( address ) );
                address++;
            }

            //Create file header
            var bfh = new Avi.Bitmapfileheader {
                bfType = Avi.BmpMagicCookie,
                bfSize = ( Int32 )( 55 + bih.biSizeImage ),
                bfReserved1 = 0,
                bfReserved2 = 0
            };

            //size of file as written to disk
            bfh.bfOffBits = Marshal.SizeOf( bih ) + Marshal.SizeOf( bfh );

            //Create or overwrite the destination file
            using ( var bw = new BinaryWriter( new FileStream( dstFileName, FileMode.Create ) ) ) {

                //Write header
                bw.Write( bfh.bfType );
                bw.Write( bfh.bfSize );
                bw.Write( bfh.bfReserved1 );
                bw.Write( bfh.bfReserved2 );
                bw.Write( bfh.bfOffBits );

                //Write bitmap info
                bw.Write( bitmapInfo );

                //Write bitmap data
                bw.Write( bitmapData );
            }
        }

        /// <summary>
        /// Opens an AVI file and creates a GetFrame object
        /// </summary>
        /// <param name="fileName">Name of the AVI file</param>
        public void Open( String fileName ) {

            //Intitialize AVI Library
            NativeMethods.AVIFileInit();

            //Open the file
            var result = NativeMethods.AVIFileOpen( ref this._aviFile, fileName, Avi.OfShareDenyWrite, 0 );

            if ( result != 0 ) {
                throw new Exception( "Exception in AVIFileOpen: " + result );
            }

            //Get the video stream
            result = NativeMethods.AVIFileGetStream( this._aviFile, out this._aviStream, Avi.StreamtypeVideo, 0 );

            if ( result != 0 ) {
                throw new Exception( "Exception in AVIFileGetStream: " + result );
            }

            this._firstFrame = NativeMethods.AVIStreamStart( this._aviStream.ToInt32() );
            this.CountFrames = NativeMethods.AVIStreamLength( this._aviStream.ToInt32() );

            this._streamInfo = new Avi.Avistreaminfo();
            result = NativeMethods.AVIStreamInfo( this._aviStream.ToInt32(), ref this._streamInfo, Marshal.SizeOf( this._streamInfo ) );

            if ( result != 0 ) {
                throw new Exception( "Exception in AVIStreamInfo: " + result );
            }

            //Open frames

            var bih = new Avi.Bitmapinfoheader { biBitCount = 24, biClrImportant = 0, biClrUsed = 0, biCompression = 0, biHeight = ( Int32 )this._streamInfo.rcFrame.bottom, biWidth = ( Int32 )this._streamInfo.rcFrame.right, biPlanes = 1 };

            //BI_RGB;
            bih.biSize = ( UInt32 )Marshal.SizeOf( bih );
            bih.biXPelsPerMeter = 0;
            bih.biYPelsPerMeter = 0;

            this._getFrameObject = NativeMethods.AVIStreamGetFrameOpen( this._aviStream, ref bih ); //force function to return 24bit DIBS

            //getFrameObject = Avi.AVIStreamGetFrameOpen(aviStream, 0); //return any bitmaps
            if ( this._getFrameObject == 0 ) {
                throw new Exception( "Exception in AVIStreamGetFrameOpen!" );
            }
        }
    }
}