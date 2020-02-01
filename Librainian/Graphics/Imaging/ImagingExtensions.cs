// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ImagingExtensions.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "ImagingExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Drawing;
    using JetBrains.Annotations;

    public static class ImagingExtensions {

        public static Color GetAverageColor( [NotNull] this Bitmap bitmap ) {
            if ( bitmap is null ) {
                throw new ArgumentNullException( nameof( bitmap ) );
            }

            var red = 0;
            var green = 0;
            var blue = 0;

            using ( var faster = new Bitmap( bitmap ) ) {
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