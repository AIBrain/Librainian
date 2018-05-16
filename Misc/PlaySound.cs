// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PlaySound.cs",
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
// "Librainian/Librainian/PlaySound.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Misc {

    namespace SharpEssentials {

        using System;
        using System.Diagnostics.CodeAnalysis;
        using System.Runtime.InteropServices;
        using System.Text;
        using System.Threading.Tasks;

        /// <summary>
        ///     Pulled from https://github.com/PaddiM8/SharpEssentials/blob/master/SharpEssentials/SharpEssentials/PlaySound.cs
        /// </summary>
        /// <remarks>Untested! Possibility won't work in 64 bit.</remarks>
        public static class PlaySound {

            private static String _command;

            private static Boolean _isOpen;

            [SuppressMessage( "Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1" )]
            [SuppressMessage( "Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0" )]
            [SuppressMessage( "Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return" )]
            [SuppressMessage( "Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass" )]
            [DllImport( "winmm.dll" )]
            private static extern Int64 mciSendString( String strCommand, StringBuilder strReturn, Int32 iReturnLength, IntPtr hwndCallback );

            public static Int32 GetSoundLength( String fileName ) {
                var lengthBuf = new StringBuilder( 32 );

                mciSendString( $"open \"{fileName}\" type waveaudio alias wave", null, 0, IntPtr.Zero );
                mciSendString( "status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero );
                mciSendString( "close wave", null, 0, IntPtr.Zero );

                Int32.TryParse( lengthBuf.ToString(), out var length );

                return length;
            }

            public static async Task Start( String sFileName ) {
                _command = $"open \"{sFileName}\" type mpegvideo alias MediaFile";
                mciSendString( _command, null, 0, IntPtr.Zero );
                _isOpen = true;

                if ( _isOpen ) {
                    _command = "play MediaFile";

                    //if (loop)
                    //    _command += " REPEAT";
                    mciSendString( _command, null, 0, IntPtr.Zero );
                }

                await Task.Delay( GetSoundLength( sFileName ) ).ConfigureAwait( false );
                Stop();
            }

            public static void Stop() {
                _command = "close MediaFile";
                mciSendString( _command, null, 0, IntPtr.Zero );
                _isOpen = false;
            }
        }
    }
}