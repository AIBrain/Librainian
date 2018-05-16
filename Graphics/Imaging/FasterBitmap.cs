// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FasterBitmap.cs",
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
// "Librainian/Librainian/FasterBitmap.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Magic;

    public unsafe class FasterBitmap : ABetterClassDispose {

        private readonly Rectangle _bounds;

        private readonly Bitmap _workingBitmap;

        private BitmapData _bitmapData;

        private Byte* _pBase = null;

        private PixelData* _pixelData = null;

        public FasterBitmap( Bitmap inputBitmap ) {
            this._workingBitmap = inputBitmap;

            this._bounds = new Rectangle( Point.Empty, this._workingBitmap.Size );

            this.Width = this._bounds.Width * sizeof( PixelData );

            if ( this.Width % 4 != 0 ) {
                this.Width = 4 * ( this.Width / 4 + 1 ); //why align?
            }

            this.Height = this._workingBitmap.Height;

            this.LockImage();
        }

        public Int32 Height { get; }

        public Boolean IsLocked { get; private set; }

        public Int32 Width { get; }

        public override void DisposeManaged() {
            if ( this.IsLocked ) { this.UnlockImage(); }
        }

        public Color GetPixel( Int32 x, Int32 y ) {
            this._pixelData = ( PixelData* )( this._pBase + y * this.Width + x * sizeof( PixelData ) );

            return Color.FromArgb( this._pixelData->Alpha, this._pixelData->Red, this._pixelData->Green, this._pixelData->Blue );
        }

        public Color GetPixelNext() {
            this._pixelData++;

            return Color.FromArgb( this._pixelData->Alpha, this._pixelData->Red, this._pixelData->Green, this._pixelData->Blue );
        }

        public void LockImage() {
            this._bitmapData = this._workingBitmap.LockBits( this._bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
            this._pBase = ( Byte* )this._bitmapData.Scan0.ToPointer();
            this.IsLocked = true;
        }

        public void SetPixel( Int32 x, Int32 y, Color color ) {
            var data = ( PixelData* )( this._pBase + y * this.Width + x * sizeof( PixelData ) );
            data->Alpha = color.A;
            data->Red = color.R;
            data->Green = color.G;
            data->Blue = color.B;
        }

        public void UnlockImage() {
            this._workingBitmap.UnlockBits( this._bitmapData );
            this._bitmapData = null;
            this._pBase = null;
            this.IsLocked = false;
        }

        private struct PixelData {

            public Byte Alpha;

            public Byte Blue;

            public Byte Green;

            public Byte Red;

            public override String ToString() => $"({this.Alpha}, {this.Red}, {this.Green}, {this.Blue})";
        }
    }
}