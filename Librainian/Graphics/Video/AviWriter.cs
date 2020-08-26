// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "AviWriter.cs" last formatted on 2020-08-14 at 8:34 PM.

namespace Librainian.Graphics.Video {

	using System;
	using System.Runtime.InteropServices;
	using JetBrains.Annotations;
	using OperatingSystem;
	using Utilities;

	/// <summary>Create AVI files from bitmaps</summary>
	public class AviWriter : ABetterClassDispose {

		private const UInt32 _fccHandler = 1668707181;

		//"Microsoft Video 1" - Use CVID for default codec: (UInt32)Avi.mmioStringToFOURCC("CVID", 0);
		private const UInt32 _fccType = Avi.StreamtypeVideo;

		private Int32 _aviFile;

		private IntPtr _aviStream = IntPtr.Zero;

		private Int32 _countFrames;

		// vids
		private UInt32 _frameRate;

		private Int32 _height;

		private UInt32 _stride;

		private Int32 _width;

		/// <summary>Creates a new video stream in the AVI file</summary>
		private void CreateStream() {
			var strhdr = new Avi.Avistreaminfo {
				fccType = _fccType, fccHandler = _fccHandler, dwScale = 1, dwRate = this._frameRate, dwSuggestedBufferSize = ( UInt32 )( this._height * this._stride ),
				dwQuality = 10000, rcFrame = {
					bottom = ( UInt32 )this._height, right = ( UInt32 )this._width
				},
				szName = new UInt16[64]
			};

			//highest quality! Compression destroys the hidden message

			var result = NativeMethods.AVIFileCreateStream( this._aviFile, out this._aviStream, ref strhdr );

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

			result = NativeMethods.AVIStreamSetFormat( this._aviStream, 0, ref bi, Marshal.SizeOf( bi ) );

			if ( result != 0 ) {
				throw new Exception( "Error in AVIStreamSetFormat: " + result );
			}
		}

		/// <summary>Closes stream, file and AVI Library</summary>
		public void Close() {
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

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() { }

		/// <summary>Creates a new AVI file</summary>
		/// <param name="fileName"> Name of the new AVI file</param>
		/// <param name="frameRate">Frames per second</param>
		public void Open( [CanBeNull] String? fileName, UInt32 frameRate ) {
			this._frameRate = frameRate;

			NativeMethods.AVIFileInit();

			var hr = NativeMethods.AVIFileOpen( ref this._aviFile, fileName, 4097 /* OF_WRITE | OF_CREATE (winbase.h) */, 0 );

			if ( hr != 0 ) {
				throw new Exception( "Error in AVIFileOpen: " + hr );
			}
		}

	}

}