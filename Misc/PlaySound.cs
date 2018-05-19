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
// "Librainian/Librainian/PlaySound.cs" was last formatted by Protiguous on 2018/05/17 at 4:16 PM.

namespace Librainian.Misc {

    using System;

    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Threading;

    /// <summary>
    ///     Pulled from https://github.com/PaddiM8/SharpEssentials/blob/master/SharpEssentials/SharpEssentials/PlaySound.cs
    /// </summary>
    /// <remarks>Untested! Possibility won't work in 64 bit?</remarks>
    public static class PlaySound {

        private static Boolean _isOpen;

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
            mciSendString( $"open \"{sFileName}\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero );
            _isOpen = true;

            if ( _isOpen ) { mciSendString( "play MediaFile", null, 0, IntPtr.Zero ); }

            await Task.Delay( GetSoundLength( sFileName ) ).NoUI();
            Stop();
        }

        public static void Stop() {
            mciSendString( "close MediaFile", null, 0, IntPtr.Zero );
            _isOpen = false;
        }
    }
}