// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CarrierImage.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Video {

    using System;

    public struct CarrierImage {

        //count of frames in the video stream, or 0
        public Int32 AviCountFrames;

        public Int64[] AviMessageBytesToHide;

        //width * height
        public Int64 CountPixels;

        //how many bytes will be hidden in this image - this field is set by CryptUtility.HideOrExtract()
        public Int64 MessageBytesToHide;

        //file name to save the new image
        public String ResultFileName;

        //file name of the clean image
        public String SourceFileName;

        //produce colorful (false) or grayscale noise (true) for this picture
        public Boolean UseGrayscale;

        public CarrierImage( String sourceFileName, String resultFileName, Int64 countPixels, Int32 aviCountFrames, Boolean useGrayscale ) {
            this.SourceFileName = sourceFileName;
            this.ResultFileName = resultFileName;
            this.CountPixels = countPixels;
            this.AviCountFrames = aviCountFrames;
            this.UseGrayscale = useGrayscale;
            this.MessageBytesToHide = 0;
            this.AviMessageBytesToHide = null;
        }

        public void SetCountBytesToHide( Int64 messageBytesToHide ) {
            this.MessageBytesToHide = messageBytesToHide;

            if ( this.SourceFileName.ToLower().EndsWith( ".avi" ) ) {
                this.AviMessageBytesToHide = new Int64[ this.AviCountFrames ];

                //calculate count of message-bytes to hide in (or extract from) each image
                Int64 sumBytes = 0;
                for ( var n = 0; n < this.AviCountFrames; n++ ) {
                    this.AviMessageBytesToHide[ n ] = ( Int64 )Math.Ceiling( messageBytesToHide / ( Single )this.AviCountFrames );
                    sumBytes += this.AviMessageBytesToHide[ n ];
                }
                if ( sumBytes > messageBytesToHide ) {

                    //correct Math.Ceiling effects
                    this.AviMessageBytesToHide[ this.AviCountFrames - 1 ] -= sumBytes - messageBytesToHide;
                }
            }
        }
    }
}