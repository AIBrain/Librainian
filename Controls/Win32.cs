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
// "Librainian/Win32.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Controls {

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Measurement.Frequency;
    using Threading;

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

        [DllImport( "User32.Dll" )]
        public static extern Boolean ClientToScreen( IntPtr hWnd, ref POINT point );

        public static Task MoveCursor( [NotNull] this Form form, Int32 x, Int32 y, TimeSpan speed ) {
            if ( form == null ) {
                throw new ArgumentNullException( nameof( form ) );
            }

            return Task.Run( () => {

                // Set the Current cursor, move the cursor's Position,
                // and set its clipping rectangle to the form.
                var cx = Cursor.Position.X;
                var cy = Cursor.Position.Y;

                while ( true ) {
                    if ( ( Cursor.Position.X == x ) && ( Cursor.Position.Y == y ) ) {
                        break;
                    }
                    if ( Randem.NextBoolean() ) {
                        if ( cx < x ) {
                            var step = ( x - cx ) / 10.0f;
                            if ( step < 1 ) {
                                step = 1;
                            }
                            cx -= ( Int32 )step;
                        }
                        else {
                            var step = ( cx - x ) / 10.0f;
                            if ( step < 1 ) {
                                step = 1;
                            }
                            cx += ( Int32 )step;
                        }
                    }
                    else {
                        if ( cy < y ) {
                            var step = ( y - cy ) / 10.0f;
                            if ( step < 1 ) {
                                step = 1;
                            }
                            cy -= ( Int32 )step;
                        }
                        else {
                            var step = ( cy - y ) / 10.0f;
                            if ( step < 1 ) {
                                step = 1;
                            }
                            cy += ( Int32 )step;
                        }
                    }
                    Cursor.Position = new Point( cx, cy );
                    Task.Delay( Hertz.Sixty ).Wait();
                }
            } );
        }

        [DllImport( "User32.Dll" )]
        public static extern Int64 SetCursorPos( Int32 x, Int32 y );

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