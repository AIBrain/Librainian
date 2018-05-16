// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ImagingExtensions.cs",
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
// "Librainian/Librainian/ImagingExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Drawing;
    using JetBrains.Annotations;

    public static class ImagingExtensions {

        public static Color GetAverageColor( [NotNull] this Bitmap bitmap ) {
            if ( bitmap is null ) { throw new ArgumentNullException( nameof( bitmap ) ); }

            var red = 0;
            var green = 0;
            var blue = 0;

            using ( var faster = new FasterBitmap( bitmap ) ) {
                for ( var x = 0; x < bitmap.Width; x++ ) {
                    for ( var y = 0; y < bitmap.Height; y++ ) {
                        var pixel = faster.GetPixel( x, y );
                        red += pixel.R;
                        green += pixel.G;
                        blue += pixel.B;
                    }
                }
            }

            var total = bitmap.Width * bitmap.Height;

            red /= total;
            green /= total;
            blue /= total;

            return Color.FromArgb( red, green, blue );
        }
    }
}