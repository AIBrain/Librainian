// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "BitmapInfo.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Graphics.Video;

using System;
using System.Drawing;
using Exceptions;
using Utilities.Disposables;

public class BitmapInfo : ABetterClassDispose {

	public BitmapInfo() : base( nameof( BitmapInfo ) ) {
	}

	//count of frames in the AVI stream, or 0
	public Int32 AviCountFrames { get; set; }

	//position of the frame in the AVI stream, or -1
	public Int32 AviPosition { get; set; }

	//uncompressed image
	public Bitmap? Bitmap { get; set; }

	//how many bytes will be hidden in this image
	public Int64 MessageBytesToHide { get; set; }

	//path and name of the bitmap file
	public String? SourceFileName { get; set; }

	/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
	public override void DisposeManaged() {
		using ( this.Bitmap ) { }
	}

	public void LoadBitmap( String sourceFileName ) {
		if ( String.IsNullOrWhiteSpace( sourceFileName ) ) {
			throw new NullException( nameof( sourceFileName ) );
		}

		this.Bitmap = new Bitmap( sourceFileName );
		this.SourceFileName = sourceFileName;
	}
}