// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Avi.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Avi.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Graphics.Video {

    using System;
    using System.Runtime.InteropServices;

    public class Avi {

        public const Int32 BmpMagicCookie = 19778;

        public const Int32 OfShareDenyWrite = 32;

        public const Int32 StreamtypeVideo = 1935960438;

        //Create a new stream in an open AVI file

        //ascii string "BM"
        //Close the AVI Library

        //mmioStringToFOURCC("vids", 0)
        //Get a stream from an open AVI file

        //Initialize the AVI Library

        //Open an AVI file

        //Release an open AVI file

        //Get a pointer to a packed DIB (returns 0 on error)

        //Release the GETFRAME object

        //Get a pointer to a GETFRAME object (returns 0 on error)

        //Get information about an open stream

        //Get the length of a stream in frames

        //Release an open AVI stream

        //Set the format for a new stream

        //Get the start position of a stream

        //Write a sample to a stream

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
		public static extern int AVIStreamRead(
			IntPtr pavi,
			Int32 lStart,
			Int32 lSamples,
			IntPtr lpBuffer,
			Int32 cbBuffer,
			Int32  plBytes,
			Int32  plSamples
			);*/

        /*[DllImport("winmm.dll", EntryPoint="mmioStringToFOURCCA")]
		public static extern int mmioStringToFOURCC(String sz, int uFlags);*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVIFileInfo(
			int pfile,
			ref AVIFILEINFO pfi,
			int lSize);*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVISaveOptions(
			IntPtr hWnd,
			int uiFlags,
			int nStreams,
			ref IntPtr ppavi,
			ref IntPtr ppOptions);*/

        /*[DllImport("avifil32.dll")]
		public static extern int AVIMakeCompressedStream(
			out IntPtr ppsCompressed, IntPtr aviStream,
			ref AVICOMPRESSOPTIONS ao, int dummy);*/

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

        //public const int StreamtypeTEXT = 1937012852;  //mmioStringToFOURCC("txts", 0)
        //public const int StreamtypeMIDI = 1935960429;  //mmioStringToFOURCC("mids", 0)

        //public const int StreamtypeAUDIO = 1935963489; //mmioStringToFOURCC("auds", 0)

        /*[DllImport("avifil32.dll")]
		public static extern int AVIStreamGetFrameOpen(
			IntPtr pAVIStream,
			int dummy);*/
    }
}