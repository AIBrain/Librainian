// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/BitmapInfo.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Video {

    using System;
    using System.Drawing;

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

        public void LoadBitmap( String fileName ) {
            this.Bitmap = new Bitmap( fileName );
            this.SourceFileName = fileName;
        }
    }
}