// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "BitmapInfo.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "BitmapInfo.cs" was last formatted by Protiguous on 2020/03/16 at 3:00 PM.

namespace Librainian.Graphics.Video {

    using System;
    using System.Drawing;
    using JetBrains.Annotations;
    using Utilities;

    public class BitmapInfo : ABetterClassDispose {

        //count of frames in the AVI stream, or 0
        public Int32 AviCountFrames;

        //position of the frame in the AVI stream, or -1
        public Int32 AviPosition;

        //uncompressed image
        [NotNull]
        public Bitmap Bitmap;

        //how many bytes will be hidden in this image
        public Int64 MessageBytesToHide;

        //path and name of the bitmap file
        [NotNull]
        public String SourceFileName;

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        public override void DisposeManaged() {
            using ( this.Bitmap ) { }
        }

        public void LoadBitmap( [NotNull] String fileName ) {
            if ( String.IsNullOrWhiteSpace( value: fileName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fileName ) );
            }

            this.Bitmap = new Bitmap( filename: fileName );
            this.SourceFileName = fileName;
        }
    }
}