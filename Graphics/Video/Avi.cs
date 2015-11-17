namespace Librainian.Graphics.Video {

    using System;
    using System.Runtime.InteropServices;

    public class Avi {
        public const Int32 BmpMagicCookie = 19778;
        public const Int32 OfShareDenyWrite = 32;
        public const Int32 StreamtypeVideo = 1935960438; //mmioStringToFOURCC("vids", 0)

                                                         //ascii string "BM"

        //Create a new stream in an open AVI file
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIFileCreateStream(
            Int32 pfile,
            out IntPtr ppavi,
            ref Avistreaminfo ptrStreaminfo );

        //Close the AVI Library
        [DllImport( "avifil32.dll" )]
        public static extern void AVIFileExit();

        //Get a stream from an open AVI file
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIFileGetStream(
            Int32 pfile,
            out IntPtr ppavi,
            Int32 fccType,
            Int32 lParam );

        //Initialize the AVI Library
        [DllImport( "avifil32.dll" )]
        public static extern void AVIFileInit();

        //Open an AVI file
        [DllImport( "avifil32.dll", PreserveSig = true )]
        public static extern Int32 AVIFileOpen(
            ref Int32 ppfile,
            String szFile,
            Int32 uMode,
            Int32 pclsidHandler );

        //Release an open AVI file
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIFileRelease( Int32 pfile );

        //Get a pointer to a packed DIB (returns 0 on error)
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamGetFrame(
            Int32 pGetFrameObj,
            Int32 lPos );

        //Release the GETFRAME object
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamGetFrameClose(
            Int32 pGetFrameObj );

        //Get a pointer to a GETFRAME object (returns 0 on error)
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamGetFrameOpen(
            IntPtr pAviStream,
            ref Bitmapinfoheader bih );

        //Get information about an open stream
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamInfo(
            Int32 pAviStream,
            ref Avistreaminfo psi,
            Int32 lSize );

        //Get the length of a stream in frames
        [DllImport( "avifil32.dll", PreserveSig = true )]
        public static extern Int32 AVIStreamLength( Int32 pavi );

        //Release an open AVI stream
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamRelease( IntPtr aviStream );

        //Set the format for a new stream
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamSetFormat(
            IntPtr aviStream, Int32 lPos,
            ref Bitmapinfoheader lpFormat, Int32 cbFormat );

        //Get the start position of a stream
        [DllImport( "avifil32.dll", PreserveSig = true )]
        public static extern Int32 AVIStreamStart( Int32 pavi );

        //Write a sample to a stream
        [DllImport( "avifil32.dll" )]
        public static extern Int32 AVIStreamWrite(
            IntPtr aviStream, Int32 lStart, Int32 lSamples,
            IntPtr lpBuffer, Int32 cbBuffer, Int32 dwFlags,
            Int32 dummy1, Int32 dummy2 );

        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct Avistreaminfo {
            public UInt32 fccType;
            public UInt32 fccHandler;
            public UInt32 dwFlags;
            public UInt32 dwCaps;
            public UInt16 wPriority;
            public UInt16 wLanguage;
            public UInt32 dwScale;
            public UInt32 dwRate;
            public UInt32 dwStart;
            public UInt32 dwLength;
            public UInt32 dwInitialFrames;
            public UInt32 dwSuggestedBufferSize;
            public UInt32 dwQuality;
            public UInt32 dwSampleSize;
            public Rect rcFrame;
            public UInt32 dwEditCount;
            public UInt32 dwFormatChangeCount;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 64 )]
            public UInt16[] szName;
        }

        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct Bitmapfileheader {
            public Int16 bfType; //"magic cookie" - must be "BM"
            public Int32 bfSize;
            public Int16 bfReserved1;
            public Int16 bfReserved2;
            public Int32 bfOffBits;
        }

        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct Bitmapinfoheader {
            public UInt32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public UInt32 biCompression;
            public UInt32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public UInt32 biClrUsed;
            public UInt32 biClrImportant;
        }

        [StructLayout( LayoutKind.Sequential, Pack = 1 )]
        public struct Rect {
            public UInt32 left;
            public UInt32 top;
            public UInt32 right;
            public UInt32 bottom;
        }

        /*[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameOpen(
			IntPtr pAVIStream,
			int dummy);*/

        //public const int StreamtypeAUDIO = 1935963489; //mmioStringToFOURCC("auds", 0)
        //public const int StreamtypeMIDI = 1935960429;  //mmioStringToFOURCC("mids", 0)
        //public const int StreamtypeTEXT = 1937012852;  //mmioStringToFOURCC("txts", 0)

        /*[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct AVIFILEINFO{
			public Int32 dwMaxBytesPerSecond;
			public Int32 dwFlags;
			public Int32 dwCaps;
			public Int32 dwStreams;
			public Int32 dwSuggestedBufferSize;
			public Int32 dwWidth;
			public Int32 dwHeight;
			public Int32 dwScale;
			public Int32 dwRate;
			public Int32 dwLength;
			public Int32 dwEditCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public char[] szFileType;
		}*/

        /*[StructLayout(LayoutKind.Sequential, Pack=1)]
		public struct AVICOMPRESSOPTIONS {
			public UInt32   fccType;
			public UInt32   fccHandler;
			public UInt32   dwKeyFrameEvery;  // only used with AVICOMRPESSF_KEYFRAMES
			public UInt32   dwQuality;
			public UInt32   dwBytesPerSecond; // only used with AVICOMPRESSF_DATARATE
			public UInt32   dwFlags;
			public IntPtr   lpFormat;
			public UInt32   cbFormat;
			public IntPtr   lpParms;
			public UInt32   cbParms;
			public UInt32   dwInterleaveEvery;
		}*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVIMakeCompressedStream(
			out IntPtr ppsCompressed, IntPtr aviStream,
			ref AVICOMPRESSOPTIONS ao, int dummy);*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVISaveOptions(
			IntPtr hWnd,
			int uiFlags,
			int nStreams,
			ref IntPtr ppavi,
			ref IntPtr ppOptions);*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVIFileInfo(
			int pfile,
			ref AVIFILEINFO pfi,
			int lSize);*/

        /*[DllImport("winmm.dll", EntryPoint="mmioStringToFOURCCA")]
		public static extern int mmioStringToFOURCC(String sz, int uFlags);*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVIStreamRead(
			IntPtr pavi,
			Int32 lStart,
			Int32 lSamples,
			IntPtr lpBuffer,
			Int32 cbBuffer,
			Int32  plBytes,
			Int32  plSamples
			);*/
    }
}