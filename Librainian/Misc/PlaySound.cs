// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PlaySound.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "PlaySound.cs" was last formatted by Protiguous on 2018/07/13 at 1:32 AM.

namespace Librainian.Misc {

	using System;
	using System.Text;
	using System.Threading.Tasks;
	using OperatingSystem;
	using Threading;

	/// <summary>
	///     Pulled from https://github.com/PaddiM8/SharpEssentials/blob/master/SharpEssentials/SharpEssentials/PlaySound.cs
	/// </summary>
	/// <remarks>Untested! Does it work in 64 bit?</remarks>
	public class PlaySound {

		private Boolean _isOpen;

		public Int32 GetSoundLength( String fileName ) {
			var lengthBuf = new StringBuilder( 32 );

			NativeMethods.mciSendString( $"open \"{fileName}\" type waveaudio alias wave", null, 0, IntPtr.Zero );
			NativeMethods.mciSendString( "status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero );
			NativeMethods.mciSendString( "close wave", null, 0, IntPtr.Zero );

			Int32.TryParse( lengthBuf.ToString(), out var length );

			return length;
		}

		public async Task Start( String fileName ) {
			NativeMethods.mciSendString( $"open \"{fileName}\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero );
			this._isOpen = true;

			if ( this._isOpen ) { NativeMethods.mciSendString( "play MediaFile", null, 0, IntPtr.Zero ); }

			await Task.Delay( this.GetSoundLength( fileName ) ).NoUI();
			this.Stop();
		}

		public void Stop() {
			NativeMethods.mciSendString( "close MediaFile", null, 0, IntPtr.Zero );
			this._isOpen = false;
		}
	}
}