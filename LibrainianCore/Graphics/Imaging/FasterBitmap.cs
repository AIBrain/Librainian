// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FasterBitmap.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "FasterBitmap.cs" was last formatted by Protiguous on 2019/08/08 at 7:41 AM.

namespace LibrainianCore.Graphics.Imaging {

    /*
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Magic;

    public unsafe class FasterBitmap : ABetterClassDispose {

        private struct PixelData {

            public Byte Alpha;

            public Byte Blue;

            public Byte Green;

            public Byte Red;

            public override String ToString() => $"({this.Alpha}, {this.Red}, {this.Green}, {this.Blue})";
        }

        private readonly Rectangle _bounds;

        private readonly Bitmap _workingBitmap;

        private BitmapData _bitmapData;

        private Byte* _pBase = null;

        private PixelData* _pixelData = null;

        public Int32 Height { get; }

        public Boolean IsLocked { get; private set; }

        public Int32 Width { get; }

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
    }
    */
}