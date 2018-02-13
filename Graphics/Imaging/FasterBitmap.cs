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
// "Librainian/FasterBitmap.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

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

        public Int32 Height {
            get;
        }

        public Boolean IsLocked {
            get; private set;
        }

        public Int32 Width {
            get;
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

        protected override void DisposeManaged() {
            if ( this.IsLocked ) {
                this.UnlockImage();
            }
        }

        private struct PixelData {
            public Byte Alpha;
            public Byte Red;
            public Byte Green;
            public Byte Blue;

            public override String ToString() {
                return $"({this.Alpha}, {this.Red}, {this.Green}, {this.Blue})";
            }
        }
    }
}
