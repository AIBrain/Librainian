// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ImagingExtensions.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.Graphics.Imaging;

using System.Drawing;
using Exceptions;

public static class ImagingExtensions {

	public static Color GetAverageColor( this Bitmap bitmap ) {
		if ( bitmap is null ) {
			throw new ArgumentEmptyException( nameof( bitmap ) );
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