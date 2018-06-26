// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "StreamString.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "StreamString.cs" was last formatted by Protiguous on 2018/06/26 at 1:36 AM.

namespace Librainian.OperatingSystem.Streams {

	using System;
	using System.IO;
	using System.Text;
	using JetBrains.Annotations;

	/// <summary>
	/// </summary>
	/// <seealso cref="http://github.com/firepacket/anark.it/" />
	public class StreamString {

		private Stream IOStream { get; }

		private UnicodeEncoding StreamEncoding { get; }

		public StreamString( Stream ioStream ) {
			this.IOStream = ioStream;
			this.StreamEncoding = new UnicodeEncoding();
		}

		[NotNull]
		public String ReadString() {
			var len = this.IOStream.ReadByte() * 256;
			len += this.IOStream.ReadByte();
			var inBuffer = new Byte[ len ];
			this.IOStream.Read( inBuffer, 0, len );

			return this.StreamEncoding.GetString( inBuffer );
		}

		public Int32 WriteString( [NotNull] String outString ) {
			var outBuffer = this.StreamEncoding.GetBytes( outString );
			var len = outBuffer.Length;

			if ( len > UInt16.MaxValue ) {
				len = UInt16.MaxValue;
			}

			this.IOStream.WriteByte( ( Byte ) ( len / 256 ) );
			this.IOStream.WriteByte( ( Byte ) ( len & 255 ) );
			this.IOStream.Write( outBuffer, 0, len );
			this.IOStream.Flush();

			return outBuffer.Length + 2;
		}
	}
}