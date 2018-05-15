// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "Win32.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has been
// overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Win32.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

namespace Librainian.Controls {

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Frequency;

    public enum Speed {

        Slow,

        Regular,

        Fast
    }

    public static class Win32 {

        public const Int32 MF_DISABLED = 0x00000002;

        // ReSharper disable InconsistentNaming
        public const Int32 MF_ENABLED = 0x00000000;

        public const Int32 MF_GRAYED = 0x1;

        public const Int32 SC_CLOSE = 0xF060;

        public const Int32 SC_MAXIMIZE = 0xF030;

        //disabled button status
        public const Int32 SC_MINIMIZE = 0xF020;

        public static Task MoveCursor( [NotNull] this Form form, Int32 x, Int32 y, TimeSpan speed ) {
            if ( form is null ) { throw new ArgumentNullException( nameof( form ) ); }

            return Task.Run( () => {

                // Set the Current cursor, move the cursor's Position, and set its clipping rectangle to the form.
                var cx = Cursor.Position.X;
                var cy = Cursor.Position.Y;

                while ( true ) {
                    if ( Cursor.Position.X == x && Cursor.Position.Y == y ) { break; }

                    if ( Randem.NextBoolean() ) {
                        if ( cx < x ) {
                            var step = ( x - cx ) / 10.0f;

                            if ( step < 1 ) { step = 1; }

                            cx -= ( Int32 )step;
                        }
                        else {
                            var step = ( cx - x ) / 10.0f;

                            if ( step < 1 ) { step = 1; }

                            cx += ( Int32 )step;
                        }
                    }
                    else {
                        if ( cy < y ) {
                            var step = ( y - cy ) / 10.0f;

                            if ( step < 1 ) { step = 1; }

                            cy -= ( Int32 )step;
                        }
                        else {
                            var step = ( cy - y ) / 10.0f;

                            if ( step < 1 ) { step = 1; }

                            cy += ( Int32 )step;
                        }
                    }

                    Cursor.Position = new Point( cx, cy );
                    Task.Delay( Hertz.Sixty ).Wait();
                }
            } );
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct POINT {

            public Int32 x;

            public Int32 y;
        }

        //close button's code in Windows API

        //enabled button status

        //disabled button status (enabled = false)

        //for minimize button on forms

        //for maximize button on forms

        // ReSharper restore InconsistentNaming
    }
}