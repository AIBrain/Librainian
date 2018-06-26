// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "BitmapInfo.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "BitmapInfo.cs" was last formatted by Protiguous on 2018/06/04 at 3:58 PM.

namespace Librainian.Graphics.Video {

	using System;
	using System.Drawing;
	using JetBrains.Annotations;

	public struct BitmapInfo {

		//count of frames in the AVI stream, or 0
		public Int32 AviCountFrames;

		//position of the frame in the AVI stream, or -1
		public Int32 AviPosition;

		//uncompressed image
		public Bitmap Bitmap;

		//how many bytes will be hidden in this image
		public Int64 MessageBytesToHide;

		//path and name of the bitmap file
		public String SourceFileName;

		public void LoadBitmap( [NotNull] String fileName ) {
			this.Bitmap = new Bitmap( fileName );
			this.SourceFileName = fileName;
		}

	}

}