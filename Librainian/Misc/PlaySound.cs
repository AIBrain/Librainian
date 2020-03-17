// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "PlaySound.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "PlaySound.cs" was last formatted by Protiguous on 2020/03/16 at 3:00 PM.

namespace Librainian.Misc {

    using System;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using OperatingSystem;

    /// <summary>Pulled from https://github.com/PaddiM8/SharpEssentials/blob/master/SharpEssentials/SharpEssentials/PlaySound.cs</summary>
    /// <remarks>Untested! Does it work in 64 bit?</remarks>
    public class PlaySound {

        private Boolean _isOpen;

        public Int32 GetSoundLength( [CanBeNull] String? fileName ) {
            var lengthBuf = new StringBuilder( capacity: 32 );

            NativeMethods.mciSendString( strCommand: $"open \"{fileName}\" type waveaudio alias wave", strReturn: null, iReturnLength: 0, hwndCallback: IntPtr.Zero );
            NativeMethods.mciSendString( strCommand: "status wave length", strReturn: lengthBuf, iReturnLength: lengthBuf.Capacity, hwndCallback: IntPtr.Zero );
            NativeMethods.mciSendString( strCommand: "close wave", strReturn: null, iReturnLength: 0, hwndCallback: IntPtr.Zero );

            Int32.TryParse( s: lengthBuf.ToString(), result: out var length );

            return length;
        }

        public async Task Start( [CanBeNull] String? fileName ) {
            NativeMethods.mciSendString( strCommand: $"open \"{fileName}\" type mpegvideo alias MediaFile", strReturn: null, iReturnLength: 0, hwndCallback: IntPtr.Zero );
            this._isOpen = true;

            if ( this._isOpen ) {
                NativeMethods.mciSendString( strCommand: "play MediaFile", strReturn: null, iReturnLength: 0, hwndCallback: IntPtr.Zero );
            }

            await Task.Delay( millisecondsDelay: this.GetSoundLength( fileName: fileName ) ).ConfigureAwait( continueOnCapturedContext: false );
            this.Stop();
        }

        public void Stop() {
            NativeMethods.mciSendString( strCommand: "close MediaFile", strReturn: null, iReturnLength: 0, hwndCallback: IntPtr.Zero );
            this._isOpen = false;
        }
    }
}