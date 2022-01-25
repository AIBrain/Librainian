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
// File "StreamString.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.OperatingSystem.Streams;

using System;
using System.IO;
using System.Text;

/// <summary>
/// </summary>
/// <see cref="http://github.com/firepacket/anark.it/" />
public class StreamString {

	public StreamString( Stream? ioStream ) {
		this.IOStream = ioStream;
		this.StreamEncoding = new UnicodeEncoding();
	}

	private Stream? IOStream { get; }

	private UnicodeEncoding StreamEncoding { get; }

	public String? ReadString() {
		var ioStream = this.IOStream;

		if ( ioStream is null ) {
			return default( String? );
		}

		var len = ioStream.ReadByte() * 256;
		len += ioStream.ReadByte();
		var inBuffer = new Byte[ len ];
		ioStream.Read( inBuffer, 0, len );

		return this.StreamEncoding.GetString( inBuffer );
	}

	public Int32 WriteString( String outString ) {
		var outBuffer = this.StreamEncoding.GetBytes( outString );
		var len = outBuffer.Length;

		if ( len > UInt16.MaxValue ) {
			len = UInt16.MaxValue;
		}

		this.IOStream?.WriteByte( ( Byte )( len / 256 ) );
		this.IOStream?.WriteByte( ( Byte )( len & 255 ) );
		this.IOStream?.Write( outBuffer, 0, len );
		this.IOStream?.Flush();

		return outBuffer.Length + 2;
	}
}