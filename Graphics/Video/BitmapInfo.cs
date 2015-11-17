namespace Librainian.Graphics.Video {
    using System;
    using System.Drawing;

    public struct BitmapInfo {
        //uncompressed image
        public Bitmap Bitmap;
        //position of the frame in the AVI stream, or -1
        public Int32 AviPosition;
        //count of frames in the AVI stream, or 0
        public Int32 AviCountFrames;
        //path and name of the bitmap file
        public String SourceFileName;
        //how many bytes will be hidden in this image
        public Int64 MessageBytesToHide;

        public void LoadBitmap(String fileName) {
            this.Bitmap = new Bitmap( fileName );
            this.SourceFileName = fileName;
        }
    }
}