// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ControlExtensions.cs" belongs to Protiguous@Protiguous.com
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
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
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
// Project: "LibrainianCore", "ControlExtensions.cs" was last formatted by Protiguous on 2019/12/22 at 8:52 AM.

namespace LibrainianCore.Controls {

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using JetBrains.Annotations;
    using Maths;

    public static class ControlExtensions {

        public static Color Blend( this Color thisColor, Color blendToColor, Double blendToPercent ) {

            blendToPercent = i().ForceBounds( 0, 1 );

            return Color.FromArgb( red(), green(), blue() );

            Byte red() => ( Byte ) ( ( thisColor.R * blendToPercent ) + ( blendToColor.R * i() ) );

            Byte green() => ( Byte ) ( ( thisColor.G * blendToPercent ) + ( blendToColor.G * i() ) );

            Byte blue() => ( Byte ) ( ( thisColor.B * blendToPercent ) + ( blendToColor.B * i() ) );

            Double i() => 1 - blendToPercent;
        }

        /// <summary>
        /// Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the lightForeColor is returned. If the BackColor is light, then the
        /// darkForeColor is returned.
        /// </summary>
        public static Color DetermineForecolor( this Color thisColor, Color lightForeColor, Color darkForeColor ) {

            // Counting the perceptive luminance - human eye favors green color...
            return a() < 0.5 ? darkForeColor : lightForeColor;

            Double a() => ( 1 - ( ( 0.299 * thisColor.R ) + ( 0.587 * thisColor.G ) + ( 0.114 * thisColor.B ) ) ) / 255;
        }

        /// <summary>
        /// Returns a contrasting ForeColor for the specified BackColor. If the source BackColor is dark, then the White is returned. If the BackColor is light, then the Black is
        /// returned.
        /// </summary>
        public static Color DetermineForecolor( this Color thisColor ) => DetermineForecolor( thisColor, Color.White, Color.Black );

        [NotNull]
        public static T InvokeFunction<T>( [NotNull] this T invokable, [NotNull] Func<T> function, [CanBeNull] Object[] arguments = null )
            where T : class, ISynchronizeInvoke {
            if ( invokable is null ) {
                throw new ArgumentNullException( paramName: nameof( invokable ) );
            }

            if ( function is null ) {
                throw new ArgumentNullException( paramName: nameof( function ) );
            }

            return invokable.Invoke( function, arguments ) as T ?? throw new InvalidOperationException();
        }

        public static Color MakeDarker( this Color thisColor, Double darknessPercent ) {
            darknessPercent = darknessPercent.ForceBounds( 0, 1 );

            return Blend( thisColor, Color.Black, darknessPercent );
        }

        public static Color MakeLighter( this Color thisColor, Double lightnessPercent ) {
            lightnessPercent = lightnessPercent.ForceBounds( 0, 1 );

            return Blend( thisColor, Color.White, lightnessPercent );
        }

        public static Color MakeTransparent( this Color thisColor, Double transparentPercent ) {
            transparentPercent = 255 - ( transparentPercent.ForceBounds( 0, 1 ) * 255 );

            return Color.FromArgb( thisColor.ToArgb() + ( ( Int32 ) transparentPercent * 0x1000000 ) );
        }

        public static Int32 ToBGR( this Color thisColor ) => ( thisColor.B << 16 ) | ( thisColor.G << 8 ) | ( thisColor.R << 0 );

        public static Int32 ToRGB( this Color thisColor ) => thisColor.ToArgb() & 0xFFFFFF;

    }

}