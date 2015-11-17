namespace Librainian.Graphics.Video {
    using System;

    public struct CarrierImage {
        //file name of the clean image
        public String SourceFileName;
        //file name to save the new image
        public String ResultFileName;
        //width * height
        public Int64 CountPixels;
        //count of frames in the video stream, or 0
        public Int32 AviCountFrames;
        //produce colorful (false) or grayscale noise (true) for this picture
        public Boolean UseGrayscale;
        //how many bytes will be hidden in this image - this field is set by CryptUtility.HideOrExtract()
        public Int64 MessageBytesToHide;
        public Int64[] AviMessageBytesToHide;

        public void SetCountBytesToHide(Int64 messageBytesToHide) {
            this.MessageBytesToHide = messageBytesToHide;

            if ( this.SourceFileName.ToLower().EndsWith( ".avi" ) ) {
                this.AviMessageBytesToHide = new Int64[ this.AviCountFrames ];

                //calculate count of message-bytes to hide in (or extract from) each image
                Int64 sumBytes = 0;
                for ( Int32 n = 0; n < this.AviCountFrames; n++ ) {
                    this.AviMessageBytesToHide[ n ] = ( Int64 )Math.Ceiling( messageBytesToHide / ( Single )this.AviCountFrames );
                    sumBytes += this.AviMessageBytesToHide[ n ];
                }
                if ( sumBytes > messageBytesToHide ) { //correct Math.Ceiling effects
                    this.AviMessageBytesToHide[ this.AviCountFrames - 1 ] -= sumBytes - messageBytesToHide;
                }
            }
        }

        public CarrierImage(String sourceFileName, String resultFileName, Int64 countPixels, Int32 aviCountFrames, Boolean useGrayscale) {
            this.SourceFileName = sourceFileName;
            this.ResultFileName = resultFileName;
            this.CountPixels = countPixels;
            this.AviCountFrames = aviCountFrames;
            this.UseGrayscale = useGrayscale;
            this.MessageBytesToHide = 0;
            this.AviMessageBytesToHide = null;
        }
    }
}